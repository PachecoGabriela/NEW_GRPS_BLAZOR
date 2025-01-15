using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using ExcelImport.BusinessObjects;

namespace ExcelImport
{
    /// <summary>
    /// Loads excel file and its active worksheet.
    /// Creates new records based on the loaded excel file and the mappings passed in the parameters.
    /// </summary>
    public class ExcelImportHelper
    {
        public const int MaxErrors = 10;

        private ExcelImportHelper()
        {
        }

        public BackgroundWorker Worker { get; set; }
        public ImportExcelWorkerStatus WorkerStatus { get; set; }

        public Workbook Workbook { get; private set; }
        public Worksheet ActiveWorksheet { get { return Workbook?.Worksheets?.ActiveWorksheet; } set { Workbook.Worksheets.ActiveWorksheet = value; } }

        public static ExcelImportHelper LoadDocument(object excelSource)
        {
            ExcelImportHelper instance = new ExcelImportHelper();
            Workbook workbook = new Workbook();
            WorkbookLoadDocument(workbook, excelSource);
            instance.Workbook = workbook;

            return instance;
        }

        private static void WorkbookLoadDocument(Workbook workbook, object excelSource)
        {
            if (excelSource is string filepath)
                workbook.LoadDocument(filepath);
            else if (excelSource is Stream stream)
                workbook.LoadDocument(stream);
            else if (excelSource is byte[] byteArray)
                workbook.LoadDocument(byteArray);
        }

        public ImportObjectResult ImportRecords(ImportDefinition importDefinition, IObjectSpace objectSpace, DoWorkEventArgs e)
        {
            if (WorkerStatus != ImportExcelWorkerStatus.Running)
            {
                e.Cancel = true;
                return null;
            }
            Guard.ArgumentNotNull(importDefinition, nameof(importDefinition));
            Guard.ArgumentNotNull(importDefinition.TargetObjectType, nameof(importDefinition.TargetObjectType));

            Session session = importDefinition.Session;
            List<ImportDefinitionColumnMapping> columnMappings = importDefinition.ColumnMappings.ToList();
            List<ImportDefinitionCellMapping> cellMappings = importDefinition.CellMappings.ToList();
            Type targetType = importDefinition.TargetObjectType;
            string identifyRecordBy = importDefinition.IdentifyRecordBy;
            ImportMode importMode = importDefinition.ImportMode;
            string recordPostprocessCodeMethod = importDefinition.RecordPostprocessCodeMethod;


            ImportObjectResult importObjectResult = new ImportObjectResult();
            List<string> importDefinitionEmails = new List<string>();
            if (!string.IsNullOrEmpty(importDefinition.EmailTo))
                importDefinitionEmails.AddRange(importDefinition.EmailTo.Split(new char[] { ';', ',' }));

            ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(targetType);

            Dictionary<string, (IMemberInfo, object)> cellMappingValues = GetCellValues(session, cellMappings, targetTypeInfo, importObjectResult);
            Dictionary<string, IMemberInfo> columnMappingMembers = GetColumnMappingMembers(columnMappings, targetType);

            if (importDefinition.EmailToMapping != null)
            {
                IMemberInfo emailToMemberInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(ImportDefinition)).Members.Where(x => x.Name == nameof(ImportDefinition.EmailTo)).FirstOrDefault();
                if (emailToMemberInfo != null)
                {
                    int row = importDefinition.EmailToMapping.RowNumber - 1;
                    int column = importDefinition.EmailToMapping.ColumnNumber - 1;
                    object mappingValue = GetMappingValue(session, importDefinition.EmailToMapping, emailToMemberInfo, row, column, importObjectResult);
                    if (mappingValue != null)
                    {
                        string excelCellEmails = mappingValue.ToString();
                        importDefinitionEmails.AddRange(excelCellEmails.Split(new char[] { ';', ',' }));
                    }
                }
            }
            importObjectResult.EmailTo = string.Join("; ", importDefinitionEmails.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLower()).Distinct());

            ClearLookupCache();

            int rows = ActiveWorksheet.Rows.LastUsedIndex;

            int startRow = importDefinition.StartRow;
            if (!importDefinition.DataContainsHeaders)
            {
                startRow--; //HACK: If Excel don't contains headers the import starts on row 0
            }

            int seq = 0;
            int total = startRow == 0 ? rows++ : rows;
            int percentage;
            ImportExcelState state = new ImportExcelState();

            for (int i = startRow; i <= rows; i++)
            {
                if (WorkerStatus != ImportExcelWorkerStatus.Running)
                {
                    e.Cancel = true;
                    return null;
                }

                if (IsRowEmpty(i, importDefinition.StartColumn))
                    continue;

                ImportRecord(session, targetTypeInfo, i, cellMappingValues, columnMappings, columnMappingMembers, importMode, identifyRecordBy,
                    recordPostprocessCodeMethod, importObjectResult, objectSpace);

                seq++;
                percentage = ImportExcelProgressIndicatorHelper.GetIntegerPercentage(seq, total);
                state.Percentage = percentage;
                state.ImportRecordsDetail = $"Importing Records: record {seq} of {total}";
                Worker.ReportProgress(percentage, state);

                if (importObjectResult.Errors.Count >= MaxErrors)
                {
                    break;
                }
                    
            }

            // postprocess - run any global methods from code
            ImportRecordsPostProcess(session, importDefinition.ImportPostprocessCodeMethod);

            if (importObjectResult.Errors.Count == 0)
            {
                if (importObjectResult.ModifiedObjects.Count == 0)
                    importObjectResult.Warnings.Add("This file contains no records to import!");
                else
                    importObjectResult.Infos.Add($"Processed {importObjectResult.ModifiedObjects.Count} records");
            }
            return importObjectResult;
        }


        /// <summary>
        /// Creates new records based on the loaded complex excel file and the type of object from ObjectSpace.
        /// </summary>
        public ImportObjectResult ImportComplexRecords(ComplexFileDefinition complexDefinition, IObjectSpace objectSpace, DoWorkEventArgs e, Dictionary<string, IMemberInfo> columnMappingMembers)
        {
            if (WorkerStatus != ImportExcelWorkerStatus.Running)
            {
                e.Cancel = true;
                return null;
            }
            Guard.ArgumentNotNull(complexDefinition, nameof(complexDefinition));
            Guard.ArgumentNotNull(complexDefinition.TargetObjectType, nameof(complexDefinition.TargetObjectType));

            Session session = complexDefinition.Session;
            List<ImportDefinitionColumnMapping> columnMappings = complexDefinition.columnMappings;

            Type targetType = complexDefinition.TargetObjectType;
            string identifyRecordBy = complexDefinition.IdentifyRecordBy;
            ImportMode importMode = complexDefinition.ImportMode;
            //string recordPostprocessCodeMethod = complexDefinition.RecordPostprocessCodeMethod;

            ImportObjectResult importObjectResult = new ImportObjectResult();

            //List<string> importDefinitionEmails = new List<string>();
            //if (!string.IsNullOrEmpty(complexDefinition.EmailTo))
            //    importDefinitionEmails.AddRange(complexDefinition.EmailTo.Split(new char[] { ';', ',' }));

            ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(targetType);

            //Dictionary<string, IMemberInfo> columnMappingMembers = GetColumnMappingMembers(columnMappings, targetType);

            //if (complexDefinition.EmailToMapping != null)
            //{
            //    IMemberInfo emailToMemberInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(ComplexFileDefinition)).Members.Where(x => x.Name == nameof(ComplexFileDefinition.EmailTo)).FirstOrDefault();
            //    if (emailToMemberInfo != null)
            //    {
            //        int row = complexDefinition.EmailToMapping.RowNumber - 1;
            //        int column = complexDefinition.EmailToMapping.ColumnNumber - 1;
            //        object mappingValue = GetMappingValue(session, complexDefinition.EmailToMapping, emailToMemberInfo, row, column, importObjectResult);
            //        if (mappingValue != null)
            //        {
            //            string excelCellEmails = mappingValue.ToString();
            //            importDefinitionEmails.AddRange(excelCellEmails.Split(new char[] { ';', ',' }));
            //        }
            //    }
            //}
            //importObjectResult.EmailTo = string.Join("; ", importDefinitionEmails.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLower()).Distinct());

            ClearLookupCache();

            int rows = ActiveWorksheet.Rows.LastUsedIndex;
            int startRow = 1;
            int seq = 0;
            int total = startRow == 0 ? rows++ : rows;
            int percentage;
            ImportExcelState state = new ImportExcelState();

            for (int i = startRow; i <= rows; i++)
            {
                if (WorkerStatus != ImportExcelWorkerStatus.Running)
                {
                    e.Cancel = true;
                    return null;
                }

                //if (IsRowEmpty(i, importDefinition.StartColumn))
                //    continue;

                if (targetType.Name == "Part")
                {
                    //ComplexPartDefinitionColumnsHelper columnsHelper = new ComplexPartDefinitionColumnsHelper();
                    //columnMappings = columnsHelper.GetComplexPartColumnMapping(session);
                    ImportComplexPartRecord(session, targetTypeInfo, i, columnMappings, columnMappingMembers, importMode, identifyRecordBy, importObjectResult, objectSpace);
                }
                    

                seq++;
                percentage = ImportExcelProgressIndicatorHelper.GetIntegerPercentage(seq, total);
                state.Percentage = percentage;
                state.ImportRecordsDetail = $"Importing Records: record {seq} of {total}";
                Worker.ReportProgress(percentage, state);

                if (importObjectResult.Errors.Count >= MaxErrors)
                {
                    break;
                }

                //ImportRecordsPostProcess(session, importDefinition.ImportPostprocessCodeMethod);

                if (importObjectResult.Errors.Count == 0)
                {
                    if (importObjectResult.ModifiedObjects.Count == 0)
                        importObjectResult.Warnings.Add("This file contains no records to import!");
                    else
                        importObjectResult.Infos.Add($"Processed {importObjectResult.ModifiedObjects.Count} records");
                }

            }


            //TODO: Must implement this
            return importObjectResult;
        }

        /// <summary>
        /// Creates new records based on the loaded excel file.
        /// </summary>
        private void ImportComplexPartRecord(Session session, ITypeInfo targetTypeInfo, int i, List<ImportDefinitionColumnMapping> columnMapping,
            Dictionary<string, IMemberInfo> columnMappingMembers, ImportMode importMode, string identifyRecordBy, ImportObjectResult importObjectResult, IObjectSpace objectSpace)
        {
            Type targetType = targetTypeInfo.Type;
            int row = -2;
            int column = -2;
            try
            {
                Dictionary<string, (IMemberInfo, object)> columnMappingValues = new Dictionary<string, (IMemberInfo, object)>();
                foreach (ImportDefinitionColumnMapping mapping in columnMapping.OrderBy(m => m.OrderNumber))
                {
                    row = i;
                    column = mapping.ColumnNumber - 1;

                    IMemberInfo member = null;
                    if (!string.IsNullOrEmpty(mapping.PropertyName) && columnMappingMembers.ContainsKey(mapping.PropertyName))
                        member = columnMappingMembers[mapping.PropertyName];
                    if (member == null)
                        continue;

                    object valueToAssign = GetMappingValue(session, mapping, member, row, column, importObjectResult);
                    if (importObjectResult.Errors.Count >= MaxErrors)
                        break;

                    if (!columnMappingValues.ContainsKey(mapping.PropertyName))
                        columnMappingValues.Add(mapping.PropertyName, (member, valueToAssign));
                }
                Dictionary<string, (IMemberInfo, object)> cellMappingValues = new Dictionary<string, (IMemberInfo, object)>();

                CriteriaOperator identifyRecordByCriteria = GetIdentifyRecordByCriteria(session, targetTypeInfo, identifyRecordBy, cellMappingValues, columnMappingValues);

                object recordToImport = GetComplexRecordToImport(session, targetType, identifyRecordByCriteria, importMode);
                if (!importObjectResult.ModifiedObjects.Contains(recordToImport))
                    importObjectResult.ModifiedObjects.Add(recordToImport);

                foreach (var property in columnMappingValues)
                {
                    string propertyName = property.Key;
                    IMemberInfo member = property.Value.Item1;
                    object propertyValue = property.Value.Item2;

                    SetPropertyValue(member, propertyName, recordToImport, propertyValue, importObjectResult, row, column);
                }

            }
            catch (ValidationException ex)
            {
                importObjectResult.Errors.Add($"Error on row {row + 1}: {ex.Message}");
            }
            catch (Exception ex)
            {
                string cellName = GetExcelCellName(row + 1, column + 1);
                importObjectResult.Errors.Add($"Error on cell {cellName} [{row + 1}, {column + 1}]: {ex.Message}");
            }
        }

        private static object GetComplexRecordToImport(Session session, Type targetType, CriteriaOperator identifyRecordByCriteria, ImportMode importMode)
        {
            object recordToImport = session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, targetType, identifyRecordByCriteria);

            switch (importMode)
            {
                case ImportMode.CreateOnly:
                    if (recordToImport != null)
                    {
                        if (session.IsNewObject(recordToImport))
                            throw new Exception($"Excel file already contains a record that matches the following criteria: {identifyRecordByCriteria}");
                        else
                            throw new Exception($"Database already contains a record that matches the following criteria: {identifyRecordByCriteria}");
                    }
                    break;
                case ImportMode.UpdateOnly:
                    if (recordToImport == null)
                        throw new Exception($"There is no record that matches the following criteria: {identifyRecordByCriteria}");
                    break;
                case ImportMode.CreateOrUpdate:
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (recordToImport == null)
            {
                recordToImport = Activator.CreateInstance(targetType, session);
            }

            return recordToImport;
        }

        /// <summary>
        /// Creates new records based on the loaded excel file and the mappings passed in the parameters.
        /// </summary>
        public ImportObjectResult ImportRecords(ImportDefinition importDefinition, IObjectSpace objectSpace)
        {
            Guard.ArgumentNotNull(importDefinition, nameof(importDefinition));
            Guard.ArgumentNotNull(importDefinition.TargetObjectType, nameof(importDefinition.TargetObjectType));

            Session session = importDefinition.Session;
            List<ImportDefinitionColumnMapping> columnMappings = importDefinition.ColumnMappings.ToList();
            List<ImportDefinitionCellMapping> cellMappings = importDefinition.CellMappings.ToList();
            Type targetType = importDefinition.TargetObjectType;
            string identifyRecordBy = importDefinition.IdentifyRecordBy;
            ImportMode importMode = importDefinition.ImportMode;
            string recordPostprocessCodeMethod = importDefinition.RecordPostprocessCodeMethod;


            ImportObjectResult importObjectResult = new ImportObjectResult();
            List<string> importDefinitionEmails = new List<string>();
            if (!string.IsNullOrEmpty(importDefinition.EmailTo))
                importDefinitionEmails.AddRange(importDefinition.EmailTo.Split(new char[] { ';', ',' }));

            ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(targetType);

            Dictionary<string, (IMemberInfo, object)> cellMappingValues = GetCellValues(session, cellMappings, targetTypeInfo, importObjectResult);
            Dictionary<string, IMemberInfo> columnMappingMembers = GetColumnMappingMembers(columnMappings, targetType);

            if (importDefinition.EmailToMapping != null)
            {
                IMemberInfo emailToMemberInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(ImportDefinition)).Members.Where(x => x.Name == nameof(ImportDefinition.EmailTo)).FirstOrDefault();
                if (emailToMemberInfo != null)
                {
                    int row = importDefinition.EmailToMapping.RowNumber - 1;
                    int column = importDefinition.EmailToMapping.ColumnNumber - 1;
                    object mappingValue = GetMappingValue(session, importDefinition.EmailToMapping, emailToMemberInfo, row, column, importObjectResult);
                    if (mappingValue != null)
                    {
                        string excelCellEmails = mappingValue.ToString();
                        importDefinitionEmails.AddRange(excelCellEmails.Split(new char[] { ';', ',' }));
                    }
                }
            }
            importObjectResult.EmailTo = string.Join("; ", importDefinitionEmails.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLower()).Distinct());

            ClearLookupCache();

            int rows = ActiveWorksheet.Rows.LastUsedIndex;

            int startRow = importDefinition.StartRow;
            if (!importDefinition.DataContainsHeaders)
            {
                startRow--; //HACK: If Excel don't contains headers the import starts on row 0
            }

            for (int i = startRow; i <= rows; i++)
            {
                if (IsRowEmpty(i, importDefinition.StartColumn))
                    continue;

                ImportRecord(session, targetTypeInfo, i, cellMappingValues, columnMappings, columnMappingMembers, importMode, identifyRecordBy,
                    recordPostprocessCodeMethod, importObjectResult, objectSpace);

                if (importObjectResult.Errors.Count >= MaxErrors)
                    break;
            }

            // postprocess - run any global methods from code
            ImportRecordsPostProcess(session, importDefinition.ImportPostprocessCodeMethod);

            if (importObjectResult.Errors.Count == 0)
            {
                if (importObjectResult.ModifiedObjects.Count == 0)
                    importObjectResult.Warnings.Add("This file contains no records to import!");
                else
                    importObjectResult.Infos.Add($"Processed {importObjectResult.ModifiedObjects.Count} records");
            }
            return importObjectResult;
        }

        private void ImportRecordsPostProcess(Session session, string postprocessCodeMethod)
        {
            MethodInfo methodInfo = FrameworkUtils.Utils.ReflectionHelper.GetCodeMethod(postprocessCodeMethod);
            if (methodInfo != null)
                methodInfo.Invoke(null, new object[] { session });
        }

        /// <summary>
        /// Creates new record based on the loaded excel file and the mappings passed in the parameters.
        /// </summary>
        private void ImportRecord(Session session, ITypeInfo targetTypeInfo, int i, Dictionary<string, (IMemberInfo, object)> cellMappingValues,
            List<ImportDefinitionColumnMapping> columnMappings, Dictionary<string, IMemberInfo> columnMappingMembers, ImportMode importMode,
            string identifyRecordBy, string recordPostprocessCodeMethod, ImportObjectResult importObjectResult, IObjectSpace objectSpace)
        {
            Type targetType = targetTypeInfo.Type;
            int row = -2;
            int column = -2;
            try
            {
                // calculate column mappings
                Dictionary<string, (IMemberInfo, object)> columnMappingValues = new Dictionary<string, (IMemberInfo, object)>();
                foreach (ImportDefinitionColumnMapping mapping in columnMappings.OrderBy(m => m.OrderNumber))
                {
                    row = i;
                    column = mapping.ColumnNumber - 1;

                    IMemberInfo member = null;
                    if (!string.IsNullOrEmpty(mapping.PropertyName) && columnMappingMembers.ContainsKey(mapping.PropertyName))
                        member = columnMappingMembers[mapping.PropertyName];
                    if (member == null)
                        continue;

                    object valueToAssign = GetMappingValue(session, mapping, member, row, column, importObjectResult);
                    if (importObjectResult.Errors.Count >= MaxErrors)
                        break;

                    if (!columnMappingValues.ContainsKey(mapping.PropertyName))
                        columnMappingValues.Add(mapping.PropertyName, (member, valueToAssign));
                }

                CriteriaOperator identifyRecordByCriteria = GetIdentifyRecordByCriteria(session, targetTypeInfo, identifyRecordBy, cellMappingValues, columnMappingValues);

                object recordToImport = GetRecordToImport(session, targetType, identifyRecordByCriteria, importMode);
                if (!importObjectResult.ModifiedObjects.Contains(recordToImport))
                    importObjectResult.ModifiedObjects.Add(recordToImport);

                // assign cell mappings
                foreach (var property in cellMappingValues)
                {
                    string propertyName = property.Key;
                    IMemberInfo member = property.Value.Item1;
                    object propertyValue = property.Value.Item2;

                    SetPropertyValue(member, propertyName, recordToImport, propertyValue, importObjectResult, row, column);
                }

                // assign column mappings
                foreach (var property in columnMappingValues)
                {
                    string propertyName = property.Key;
                    IMemberInfo member = property.Value.Item1;
                    object propertyValue = property.Value.Item2;

                    SetPropertyValue(member, propertyName, recordToImport, propertyValue, importObjectResult, row, column);
                }

                // postprocess single record from code
                ImportRecordPostProcess(session, recordToImport, recordPostprocessCodeMethod);

                //Validate BusinessObject RuleSet
                IsBusinessObjectRuleSetValid(recordToImport, objectSpace);
            }
            catch (ValidationException ex)
            {
                importObjectResult.Errors.Add($"Error on row {row + 1}: {ex.Message}");
            }
            catch (Exception ex)
            {
                string cellName = GetExcelCellName(row + 1, column + 1);
                importObjectResult.Errors.Add($"Error on cell {cellName} [{row + 1}, {column + 1}]: {ex.Message}");
            }
        }

        private void IsBusinessObjectRuleSetValid(object recordToImport, IObjectSpace objectSpace)
        {
            Validator.RuleSet.Validate(objectSpace, recordToImport, DefaultContexts.Save);
        }

        private void ImportRecordPostProcess(Session session, object recordToImport, string postprocessCodeMethod)
        {
            if (session == null || recordToImport == null)
                return;

            MethodInfo methodInfo = FrameworkUtils.Utils.ReflectionHelper.GetCodeMethod(postprocessCodeMethod);
            if (methodInfo != null)
                methodInfo.Invoke(null, new object[] { session, recordToImport });
        }

        /// <summary>
        /// Creates criteria from the string provided in identifyRecordBy. Finds phrases '[Some.Property.Path] = ?' and adds corresponding values.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="targetTypeInfo"></param>
        /// <param name="identifyRecordBy">Expected value: "[JOB_NO] = ? AND [ID_NUMBER] = ?"</param>
        /// <param name="cellMappingValues"></param>
        /// <param name="columnMappingValues"></param>
        /// <returns></returns>
        private CriteriaOperator GetIdentifyRecordByCriteria(Session session, ITypeInfo targetTypeInfo, string identifyRecordBy,
            Dictionary<string, (IMemberInfo, object)> cellMappingValues, Dictionary<string, (IMemberInfo, object)> columnMappingValues)
        {
            if (targetTypeInfo == null || string.IsNullOrEmpty(identifyRecordBy))
                return null;

            CriteriaOperator result = null;

            // find phrases '[Some.Property.Path] = ?'
            Regex regex = new Regex(@"\[(?<path>[^]]*)\] = \?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(identifyRecordBy);

            List<object> criteriaParameters = new List<object>();
            foreach (Match match in matches)
            {
                Group propertyPath = match.Groups["path"];

                object mappedValue = null;

                var cellMapping = cellMappingValues.Where(m => m.Key == propertyPath.Value).FirstOrDefault();
                if (cellMapping.Value.Item1 != null)
                    mappedValue = cellMapping.Value.Item2;
                if (mappedValue == null)
                {
                    var columnMapping = columnMappingValues.Where(m => m.Key == propertyPath.Value).FirstOrDefault();
                    if (columnMapping.Value.Item1 != null)
                        mappedValue = columnMapping.Value.Item2;
                }

                criteriaParameters.Add(mappedValue);
            }
            result = session.ParseCriteria(identifyRecordBy, criteriaParameters.ToArray());

            return result;
        }

        /// <summary>
        /// Find or create the record instance.
        /// Throws exception when ImportMode is violated (CreateOnly, UpdateOnly).
        /// </summary>
        private static object GetRecordToImport(Session session, Type targetType, CriteriaOperator identifyRecordByCriteria, ImportMode importMode)
        {
            object recordToImport = session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, targetType, identifyRecordByCriteria);

            switch (importMode)
            {
                case ImportMode.CreateOnly:
                    if (recordToImport != null)
                    {
                        if (session.IsNewObject(recordToImport))
                            throw new Exception($"Excel file already contains a record that matches the following criteria: {identifyRecordByCriteria}");
                        else
                            throw new Exception($"Database already contains a record that matches the following criteria: {identifyRecordByCriteria}");
                    }
                    break;
                case ImportMode.UpdateOnly:
                    if (recordToImport == null)
                        throw new Exception($"There is no record that matches the following criteria: {identifyRecordByCriteria}");
                    break;
                case ImportMode.CreateOrUpdate:
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (recordToImport == null)
            {
                recordToImport = Activator.CreateInstance(targetType, session);
            }

            return recordToImport;
        }

        private object GetMappingValue(Session session, ImportDefinitionMappingBase mapping, IMemberInfo member, int row, int column, ImportObjectResult importObjectResult)
        {
            string stringValue = null;
            object valueToAssign = null;
            Cell cell = null;
            try
            {
                if (row >= 0 && column >= 0)
                    cell = ActiveWorksheet.Cells[row, column];
                object value = cell?.Value.ToObject();

                stringValue = (Convert.ToString(value) ?? "").Trim();
                // skip ignored prefix
                if (!string.IsNullOrEmpty(mapping.IgnoredPrefix) && stringValue.StartsWith(mapping.IgnoredPrefix, StringComparison.OrdinalIgnoreCase))
                    stringValue = stringValue.Substring(mapping.IgnoredPrefix.Length).Trim();
                // if empty, assign DefaultValue
                if (string.IsNullOrWhiteSpace(stringValue))
                    stringValue = mapping.DefaultValue;

                if (!string.IsNullOrEmpty(mapping.FindByField))
                {
                    valueToAssign = FindLookupValue(session, mapping, member, stringValue);
                }
                else if (mapping.CrossReference != null)
                {
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        bool foundMatch = false;
                        foreach (ImportCrossReferenceLine line in mapping.CrossReference.Lines.OrderBy(l => l.OrderNumber))
                        {
                            if (line.IsMatch(stringValue))
                            {
                                stringValue = GetOutputValue(line, stringValue, importObjectResult);
                                foundMatch = true;
                                break;
                            }
                        }
                        // T00256 - Add cross ref validation rule for import tool
                        if (!foundMatch)
                            throw new Exception($"unknown abbreviation type (could not find a Cross Reference match for the mapping '{mapping}')");
                    }
                    valueToAssign = NormalizePropertyValue(member, stringValue);
                }
                else
                {
                    valueToAssign = NormalizePropertyValue(member, stringValue);
                }
            }
            catch (SqlExecutionErrorException ex)
            {
                importObjectResult.Errors.Add($"Error on cell {cell?.GetReferenceA1()} [{row + 1}, {column + 1}], with input value '{stringValue}': Possible error finding lookup values. In your Import Definition, check your Column Mapping criterias");
            }
            catch (Exception ex)
            {
                importObjectResult.Errors.Add($"Error on cell {cell?.GetReferenceA1()} [{row + 1}, {column + 1}], with input value '{stringValue}': {ex.Message}");
            }

            return valueToAssign;
        }
        /// <summary>
        /// Translates line.OutputValue into a concrete value.
        /// E.g. from '{Value}-{CurrentDate:yyyy_mm}' into 'ABC-2021_06'
        /// 
        /// NOTE: invalid tags are treated as static text.  E.g. '{Value}-{CurrentDate123:yyyy_mm}'  is translated into 'ABC-{CurrentDate123:yyyy_mm}'
        /// </summary>
        private string GetOutputValue(ImportCrossReferenceLine line, string cellValue, ImportObjectResult importObjectResult)
        {
            string outputValue = line.OutputValue;
            if (string.IsNullOrEmpty(outputValue))
                return outputValue;

            StringBuilder outputBuilder = new StringBuilder();

            int startAtIndex = 0;
            while (startAtIndex < outputValue.Length)
            {
                int openTagIndex = outputValue.IndexOf('{', startAtIndex);
                if (openTagIndex != -1)
                {
                    outputBuilder.Append(outputValue.Substring(startAtIndex, openTagIndex - startAtIndex));  // add portion before '{' but without '{'.
                    int closeTagIndex = outputValue.IndexOf('}', openTagIndex);
                    if (closeTagIndex != -1)
                    {
                        string tagName = outputValue.Substring(openTagIndex, closeTagIndex - openTagIndex + 1);   // e.g. "{Value}"
                        string tagValue = EvaluateTag(tagName, cellValue, importObjectResult);

                        outputBuilder.Append(tagValue);
                        startAtIndex = closeTagIndex + 1;
                    }
                    else
                    {
                        outputBuilder.Append(outputValue.Substring(openTagIndex));
                        break;
                    }
                }
                else
                {
                    outputBuilder.Append(outputValue.Substring(startAtIndex));
                    break;
                }
            }
            return outputBuilder.ToString();
        }

        /// <summary>
        /// Evaluates the Tag's value. Available tags:  {TimeStamp}, {CurrentDate}, {CurrentDate:your_format}, {Value}, {Value:substring_zeroBasedStartIndex, substring_length}.
        /// E.g. {Value:2,4}, {Value:17,3}
        /// </summary>
        /// <param name="tagName">Tag name with braces, e.g. {Value}</param>
        /// <param name="cellValue">Excel cell string value</param>
        /// <returns></returns>
        private string EvaluateTag(string tagName, string cellValue, ImportObjectResult importObjectResult)
        {
            string tagNameNoBrackets = tagName.Trim('{', '}');
            string tag = tagNameNoBrackets;
            string format = "";

            int colonIndex = tagNameNoBrackets.IndexOf(':');
            if (colonIndex != -1)
            {
                tag = tagNameNoBrackets.Substring(0, colonIndex);
                format = tagNameNoBrackets.Substring(colonIndex + 1);
            }

            if (tag == "Value")
            {
                // {Value}
                if (string.IsNullOrEmpty(format))
                    return cellValue;

                // {Value:2,4}
                try
                {
                    int commaIndex = format.IndexOf(',');
                    if (commaIndex == -1)
                        throw new UserFriendlyException("Value tag has incorrect format: comma (',') is expected.");

                    string[] formatSplit = format.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (formatSplit.Length != 2)
                        throw new UserFriendlyException("Value tag has incorrect format: two numbers are expected.");

                    int substringStartAt = -1;
                    int substringLength = -1;
                    if (int.TryParse(formatSplit[0], out int startAt))
                        substringStartAt = startAt - 1;  // '-1' as it is 1-based on the user side.
                    if (int.TryParse(formatSplit[1], out int length))
                        substringLength = length;
                    if (substringStartAt < 0 || substringLength < 0)
                        throw new UserFriendlyException("Value tag has incorrect format: two positive numbers are expected.");

                    int lastNeededIndex = substringStartAt + substringLength; // {Value:2,4} -> substringStartAt=1, substringLength=4, lastNeededIndex=5
                    if (!string.IsNullOrEmpty(cellValue) && lastNeededIndex >= cellValue.Length)
                        throw new UserFriendlyException($"Cell value is too short for this Value tag. It should be at least {lastNeededIndex + 1} characters long.");

                    return cellValue.Substring(substringStartAt, substringLength);
                }
                catch (Exception ex)
                {
                    string warningPrefix = $"An error occurred during the cross reference import of Excel Value '{cellValue}' with a cross reference Output Tag of '{tagName}'. Check the Cross Reference mapping definition:";
                    importObjectResult.Warnings.Add($"{warningPrefix} {ex.Message}");
                    return cellValue;
                }
            }
            else if (tag == "TimeStamp" || tag == "Timestamp") return DateTime.Now.ToString("yyyyMMdd_HHmmss");
            else if (tag == "CurrentDate") return DateTime.Now.ToString(format);
            else return tagName;
        }

        private object FindLookupValue(Session session, ImportDefinitionMappingBase mapping, IMemberInfo member, string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return null;

            object lookupValue = GetCachedLookupValue(mapping.PropertyType, mapping.FindByField, stringValue);

            if (lookupValue == null)
            {
                IMemberInfo findByFieldMember = member.MemberTypeInfo.FindMember(mapping.FindByField);
                if (findByFieldMember == null)
                    throw new Exception($"Could not find field {mapping.FindByField} on type {CaptionHelper.GetClassCaption(member.MemberType.FullName)}. Check 'Find By Field' on the import definition.");
                object findByFieldNormalized = NormalizePropertyValue(findByFieldMember, stringValue);
                //TODO: Add support for aditional Criteria in case findByFieldNormalized has the same value
                CriteriaOperator criteria = CriteriaOperator.Parse($"{mapping.FindByField} = ?", findByFieldNormalized);
                if (!string.IsNullOrEmpty(mapping.AdditionalCriteria))
                {
                    criteria = CriteriaOperator.Parse($"{mapping.FindByField} = ? And {mapping.AdditionalCriteria}", findByFieldNormalized);
                }
                lookupValue = session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, member.MemberType, criteria);
                if (lookupValue == null)
                {
                    switch (mapping.WhenFindingLookupValue)
                    {
                        case FindReferenceMappingsMode.ErrorIfNotFound:
                            throw new Exception($"Could not find {CaptionHelper.GetClassCaption(member.MemberType.FullName)} record with criteria: '{criteria}'.");
                        case FindReferenceMappingsMode.CreateIfNotFound:
                            lookupValue = member.MemberTypeInfo.CreateInstance(session);
                            findByFieldMember.SetValue(lookupValue, findByFieldNormalized);
                            break;
                        case FindReferenceMappingsMode.LeaveBlankIfNotFound:
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                AddToLookupCache(mapping.PropertyType, mapping.FindByField, stringValue, lookupValue);
            }
            return lookupValue;
        }

        /// <summary>
        /// T00236 - Add new Feature to import tool (Network Hot Folder)
        /// 
        /// That method is invoked by the Workflow.
        /// Imports Excel documents from the folder directory specified ImportDefinitionFolder.FolderPath.
        /// Moves imported documents to the folder directory specified by ImportDefinitionFolder.ProcessedFolderPath.
        /// </summary>
        public static void ImportObjectsAndMoveImportedFiles(List<ImportDefinitionFolder> importDefinitionFolders, Session session, IObjectSpace objectSpace)
        {
            ImportObjectResult importObjectResult = new ImportObjectResult();
            if (importDefinitionFolders == null || importDefinitionFolders.Count == 0)
            {
                importObjectResult.Errors.Add("The workflow could not find active Import Definition Folders");
                ImportLog.CreateImportLog(session, importObjectResult);
                (session as UnitOfWork).CommitChanges();
            }
            else
            {
                foreach (ImportDefinitionFolder folder in importDefinitionFolders)
                {
                    if (string.IsNullOrEmpty(folder.FolderPath))
                        importObjectResult.Errors.Add("The Folder Path of Import Definition Folder is not specified.");
                    if (string.IsNullOrEmpty(folder.ErrorFolderPath))
                        importObjectResult.Errors.Add("The Error Folder Path of Import Definition Folder is not specified.");
                    if (string.IsNullOrEmpty(folder.ProcessedFolderPath))
                        importObjectResult.Errors.Add("The Processed Folder Path of Import Definition Folder is not specified.");
                    if (!Directory.Exists(folder.FolderPath))
                        importObjectResult.Errors.Add("The import folder directory does not exist.");
                    if (!Directory.Exists(folder.ErrorFolderPath))
                        importObjectResult.Errors.Add("The error folder directory does not exist.");
                    if (!Directory.Exists(folder.ProcessedFolderPath))
                        importObjectResult.Errors.Add("The processed folder directory does not exist.");

                    ImportDefinition importDefinition = folder.ImportDefinition;
                    if (importDefinition == null)
                    {
                        importObjectResult.Errors.Add("The Import Definition of Import Definition Folder is not specified.");
                    }

                    if (importObjectResult.HasErrors())
                    {
                        ImportLog.CreateImportLog(session, importObjectResult);
                        (session as UnitOfWork).CommitChanges();
                        return;
                    }

                    string targetFileName = string.Empty;

                    // get files only from the main folder. Ignore subfolders:
                    string[] exts = new string[] { ".xls", ".xlsx" };
                    string[] importPathFilePaths = Directory.EnumerateFiles(folder.FolderPath, "*.*")
                        .Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase))).ToArray();

                    foreach (string importFilePath in importPathFilePaths)
                    {
                        try
                        {
                            ExcelImportHelper helper = ExcelImportHelper.LoadDocument(importFilePath);
                            if (helper.Workbook != null)
                                importObjectResult = helper.ImportRecords(importDefinition, objectSpace);

                            if (importObjectResult.Errors.Count > 0)
                            {
                                if (File.Exists(importFilePath))
                                {
                                    string importFileName = Path.GetFileName(importFilePath);
                                    targetFileName = Path.Combine(folder.ErrorFolderPath, importFileName);
                                    targetFileName = AddSuffixIfFileExists(targetFileName);
                                    File.Move(importFilePath, targetFileName);
                                }

                                importObjectResult.Errors.Insert(0, $"An error occured during import from Excel sheet {importFilePath}");
                                throw new Exception(importObjectResult.GetLogMessage());
                            }
                            else
                            {
                                if (File.Exists(importFilePath))
                                {
                                    string importFileName = Path.GetFileName(importFilePath);
                                    targetFileName = Path.Combine(folder.ProcessedFolderPath, importFileName);
                                    targetFileName = AddSuffixIfFileExists(targetFileName);
                                    File.Move(importFilePath, targetFileName);
                                }

                                ImportLog.CreateImportLog(session, importObjectResult, targetFileName);
                            }
                            (session as UnitOfWork).CommitChanges();
                        }
                        catch (Exception)
                        {
                            session.RollbackTransaction();
                            if (importObjectResult.Errors.Count > 0 || importObjectResult.Warnings.Count > 0 || importObjectResult.Infos.Count > 0)
                                ImportLog.CreateImportLog(session, importObjectResult, targetFileName);
                            (session as UnitOfWork).CommitChanges();
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the file path does not exist, returns filePath unchanged.
        /// If the file already exists, adds a '(1)' suffix to the filename.
        ///   If the suffixed file name also exists, then tries again with another number in the suffix. 
        ///   Etc.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="maxTries">The maximum number of increasing the suffix number, e.g. from (1) to (10).</param>
        public static string AddSuffixIfFileExists(string filePath, int maxTries = 10, bool forceAddSuffix = false, string suffixFormat = " ({0:0})")
        {
            string result = filePath;

            if (string.IsNullOrEmpty(result))
                return result;
            if (!forceAddSuffix)
            {
                if (!File.Exists(result))
                    return result;
            }

            if (maxTries <= 0)
                maxTries = 10;

            string directoryName = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath).TrimEnd();
            string fileExtension = Path.GetExtension(filePath);

            for (int i = 1; i <= maxTries; i++)
            {
                // Add suffix ' (i)' if the file exists
                result = Path.Combine(directoryName, $"{fileNameWithoutExtension}{string.Format(suffixFormat, i)}{fileExtension}");
                if (!File.Exists(result))
                    return result;
            }

            throw new Exception($"There is already the maximum number of {maxTries} file copies with the name '{fileName}' in the directory.");
        }

        /// <summary>
        /// Checks if current row is considered empty: check 3 first cells, trim white characters and check if empty.
        /// </summary>
        private bool IsRowEmpty(int row, int column)
        {
            for (int i = 0; i < 3; i++)
            {
                Cell cell = ActiveWorksheet.Cells[row, column + i];
                object value = cell.Value.ToObject();
                string stringValue = (Convert.ToString(value) ?? "").Trim();
                if (!string.IsNullOrEmpty(stringValue))
                    return false;
            }

            return true;
        }

        private void SetPropertyValue(IMemberInfo member, string propertyName, object target, object value, ImportObjectResult importObjectResult, int row, int column)
        {
            if (member == null)
                return;

            //object normalized = NormalizePropertyValue(member, value);

            if (member.MemberType == typeof(string))
            {
                string stringValue = value as string;
                if (stringValue != null && member.Size >= 0 && member.Size < stringValue.Length)
                {
                    value = FrameworkUtils.Utils.StringHelper.Truncate(stringValue, member.Size, true);

                    string cellName = GetExcelCellName(row + 1, column + 1);
                    importObjectResult.Warnings.Add($"Warning on cell {cellName} [{row + 1}, {column + 1}]: field size is {member.Size} and the input value has been truncated. Original input value: '{stringValue}'");
                }
            }

            if (member.Name == propertyName)
                member.SetValue(target, value);
            else
                FrameworkUtils.Utils.XpoHelper.SetNestedMemberValue(target as XPBaseObject, propertyName, value);
        }

        private object NormalizePropertyValue(IMemberInfo member, object value)
        {
            if (member == null || value == null || string.IsNullOrEmpty(value.ToString()) || value.ToString().ToUpper() == "NULL")
                return null;

            object converted;

            if (member.MemberType == typeof(DateTime))
            {
                DateTime dt = DateTime.MinValue;
                string stringDate = value.ToString();
                if (!DateTime.TryParse(stringDate, new CultureInfo("en-US"), DateTimeStyles.None, out dt))
                {
                    if (stringDate != null && stringDate.Length == "dd/MM/yyyy".Length)
                        stringDate += " 00:00:00";
                    if (!string.IsNullOrEmpty(stringDate))
                        dt = DateTime.ParseExact(stringDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }

                converted = dt;
            }
            else if (member.MemberType.IsEnum)
            {
                string stringValue = value.ToString();
                converted = Enum.Parse(member.MemberType, stringValue);
            }
            else
            {
                converted = Convert.ChangeType(value, member.MemberType);
            }

            return converted;
        }

        /// <summary>
        /// Retrieves members from column mappings as pairs (field name, IMemberInfo).
        /// </summary>
        private Dictionary<string, IMemberInfo> GetColumnMappingMembers(List<ImportDefinitionColumnMapping> columnMappings, Type targetType)
        {
            Dictionary<string, IMemberInfo> result = new Dictionary<string, IMemberInfo>();

            ITypeInfo info = XafTypesInfo.Instance.FindTypeInfo(targetType);

            foreach (ImportDefinitionColumnMapping columnMapping in columnMappings)
            {
                if (!result.ContainsKey(columnMapping.PropertyName))
                {
                    IMemberInfo member = FindMemberByName(columnMapping.PropertyName, info);
                    if (member != null)
                        result.Add(columnMapping.PropertyName, member);
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves property values from cell mappings as pairs (field name, (IMemberInfo, value)).
        /// </summary>
        private Dictionary<string, (IMemberInfo, object)> GetCellValues(Session session, List<ImportDefinitionCellMapping> cellMappings, ITypeInfo targetTypeInfo, ImportObjectResult importObjectResult)
        {
            Dictionary<string, (IMemberInfo, object)> result = new Dictionary<string, (IMemberInfo, object)>();

            if (cellMappings == null || cellMappings.Count == 0 || this.ActiveWorksheet == null)
                return result;

            foreach (ImportDefinitionCellMapping cellMapping in cellMappings.OrderBy(m => m.OrderNumber).ToList())
            {
                int row = cellMapping.RowNumber - 1;
                int column = cellMapping.ColumnNumber - 1;
                IMemberInfo member = null;
                if (!string.IsNullOrEmpty(cellMapping.PropertyName))
                    member = FindMemberByName(cellMapping.PropertyName, targetTypeInfo);
                if (member == null)
                    continue;

                if (!result.ContainsKey(cellMapping.PropertyName))
                {
                    object valueToAssign = GetMappingValue(session, cellMapping, member, row, column, importObjectResult);
                    if (importObjectResult.Errors.Count >= MaxErrors)
                        break;

                    result.Add(cellMapping.PropertyName, (member, valueToAssign));
                }
            }

            if (importObjectResult.Errors.Count > 0)
                throw new Exception(importObjectResult.GetLogMessage());

            return result;
        }

        public IMemberInfo FindMemberByCaption(string memberCaption, ITypeInfo info)
        {
            IMemberInfo result = null;

            if (info != null)
                result = info.Members.Where(x => x.IsPublic && !x.IsReadOnly && x.IsProperty && x.IsPersistent && !x.IsKey && !x.IsList
                    && CaptionHelper.GetMemberCaption(x) == memberCaption).FirstOrDefault();

            return result;
        }

        public IMemberInfo FindMemberByName(string memberName, ITypeInfo info)
        {
            IMemberInfo result = FrameworkUtils.Utils.XpoHelper.GetNestedMember(info, memberName, false);

            if (result != null && (!result.IsPublic || result.IsReadOnly || !result.IsProperty || !result.IsPersistent || result.IsKey || result.IsList))
                result = null;

            return result;
        }

        /// <summary>
        /// Converts a column number (e.g. 127) into an Excel column (e.g. AA)
        /// 
        /// https://stackoverflow.com/questions/181596/how-to-convert-a-column-number-e-g-127-into-an-excel-column-e-g-aa
        /// </summary>
        /// <param name="columnNumber">1-based column number</param>
        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// Converts a column and row numbers (e.g. 127,3) into an Excel cell name (e.g. AA3)
        /// </summary>
        /// <param name="rowNumber">1-based row number</param>
        /// <param name="columnNumber">1-based column number</param>
        private static string GetExcelCellName(int rowNumber, int columnNumber)
        {
            return $"{GetExcelColumnName(columnNumber)}{rowNumber}";
        }

        #region Lookup Values Cache

        private Dictionary<LookupCacheWrapper, object> _lookupCache = new Dictionary<LookupCacheWrapper, object>();

        private void ClearLookupCache()
        {
            _lookupCache.Clear();
        }

        private object GetCachedLookupValue(Type propertyType, string findByField, string stringValue)
        {
            LookupCacheWrapper search = new LookupCacheWrapper() { PropertyType = propertyType, FindByField = findByField, StringValue = stringValue };
            if (_lookupCache.ContainsKey(search))
                return _lookupCache[search];
            return null;
        }

        private void AddToLookupCache(Type propertyType, string findByField, string stringValue, object lookupValue)
        {
            LookupCacheWrapper search = new LookupCacheWrapper() { PropertyType = propertyType, FindByField = findByField, StringValue = stringValue };
            if (!_lookupCache.ContainsKey(search))
                _lookupCache[search] = lookupValue;
        }

        #endregion

        //public static bool IsBusinessObjectRuleSetValid(IObjectSpace objectSpace)
        //{
        //    var validationResult = Validator.RuleSet.ValidateAllTargets(objectSpace, objectSpace.ModifiedObjects, DefaultContexts.Save);
        //    return validationResult.State == ValidationState.Valid;
        //}

        //public static bool IsBusinessObjectRuleSetValid(IObjectSpace objectSpace)
        //{
        //    var validationResult = Validator.RuleSet.ValidateAll(objectSpace, objectSpace.ModifiedObjects, DefaultContexts.Save);
        //    return validationResult;
        //}

        public static string GetBusinessObjectRuleSetValidationErrors(IObjectSpace objectSpace)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var validationResult = Validator.RuleSet.ValidateAllTargets(objectSpace, objectSpace.ModifiedObjects, DefaultContexts.Save);
            if (validationResult.State != ValidationState.Valid)
            {
                int rowCount = 0;
                int totalValidationErrors = validationResult.Results.Where(r => r.State == ValidationState.Invalid).Count();
                stringBuilder.AppendLine($"{totalValidationErrors} validation errors occurred while importing the document:");
                stringBuilder.AppendLine();
                foreach (var result in validationResult.Results)
                {
                    rowCount++;
                    if (result.State != ValidationState.Valid)
                    {
                        stringBuilder.AppendLine($"Error on row {rowCount}: {result.ErrorMessage}");
                    }
                }
            }

            return stringBuilder.ToString();
        }

    }

    public struct LookupCacheWrapper
    {
        public Type PropertyType { get; set; }
        public string FindByField { get; set; }
        public string StringValue { get; set; }
    }

    public class ImportObjectResult
    {
        public List<object> ModifiedObjects { get; } = new List<object>();
        public List<string> Errors { get; } = new List<string>();
        public List<string> Infos { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public string EmailTo { get; set; } = string.Empty;
        public ImportDefinition ImportDefinition { get; set; }
        public ComplexFileDefinition ComplexFileDefinition { get; set; }

        public LogLevel MostSevereLogType
        {
            get
            {
                if (Errors.Count > 0)
                    return LogLevel.Error;
                else if (Warnings.Count > 0)
                    return LogLevel.Warning;
                else
                    return LogLevel.Information;
            }
        }

        public bool HasErrors()
        {
            return this.Errors.Count > 0;
        }

        public bool HasWarnings()
        {
            return this.Warnings.Count > 0;
        }

        public string GetWarnings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Warnings.Count > 0)
            {
                stringBuilder.AppendLine("Warnings:");
                foreach (string warning in Warnings)
                    stringBuilder.AppendLine(warning);
            }
            return stringBuilder.ToString();
        }

        public string GetInformation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Infos.Count > 0)
            {
                stringBuilder.AppendLine("Information:");
                foreach (string information in Infos)
                    stringBuilder.AppendLine(information);
            }
            return stringBuilder.ToString();
        }

        public string GetLogMessage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Errors.Count > 0)
            {
                string atLeast = Errors.Count >= ExcelImportHelper.MaxErrors ? "At least " : "";
                stringBuilder.AppendLine($"{atLeast}{Errors.Count} errors occurred while importing the document:");
                stringBuilder.AppendLine();
                foreach (string error in Errors)
                {
                    stringBuilder.AppendLine(error);
                    stringBuilder.AppendLine();
                }
            }
            if (Warnings.Count > 0)
            {
                stringBuilder.AppendLine("Warnings:");
                foreach (string warning in Warnings)
                    stringBuilder.AppendLine(warning);
            }
            if (Infos.Count > 0)
            {
                stringBuilder.AppendLine("Information:");
                foreach (string information in Infos)
                    stringBuilder.AppendLine(information);
            }
            return stringBuilder.ToString();
        }
    }
    public enum LogLevel
    {
        [ImageName("Error")]
        Error = 0,

        [ImageName("Warning")]
        Warning = 1,

        [ImageName("State_Validation_Information")]
        Information = 2
    }

    public enum ImportExcelWorkerStatus
    {
        Stopped,
        Running,
        Cancelled,
    }

    public class ImportExcelState
    {
        public int Percentage { get; set; }
        public string ImportRecordsDetail { get; set; }
        public string ImportRecordsAdditionalDetails { get; set; }
    }

    public static class ImportExcelProgressIndicatorHelper
    {
        public static int GetIntegerPercentage(double value, double total)
        {
            int result = 0;
            result = (int)Math.Floor(value / total * 100);
            return result;
        }

        public static string GetCompletionTime(long execTime, double value, double total)
        {
            double pendingOperations = total - value;
            double expectedCompletionTimeInMs = pendingOperations * execTime;

            TimeSpan time = TimeSpan.FromMilliseconds(expectedCompletionTimeInMs);
            if (time.Hours > 0)
            {
                return $"{time.Hours} hour(s) and {time.Minutes} minute(s)";
            }
            else if (time.Minutes > 0)
            {
                return $"{time.Minutes} minute(s) and {time.Seconds} seconds";
            }
            else
            {
                return $"{time.Seconds} seconds";
            }
        }
    }
}
