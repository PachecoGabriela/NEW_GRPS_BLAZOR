using System;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace ExcelImport.BusinessObjects
{
    [Appearance("ImportDefinition_ImportErrors_Appearance", null, TargetItems = nameof(ImportErrors), FontColor = "Red", BackColor = "WhiteSmoke")]
    [CreatableItem(false)]
    [DefaultClassOptions]
    [DefaultProperty(nameof(Name))]
    [ImageName("ExportToXLSX")]
    [ModelDefault("IsCloneable", "true")]
    [NavigationItem("Import Tool")]
    public class ImportDefinition : BaseObject
    {
        public ImportDefinition(Session session) : base(session)
        {
        }

        private string _name;
        [ToolTip("Name of this import definition.")]
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue<string>(nameof(Name), ref _name, value); }
        }

        private Type _targetObjectType;
        [RuleRequiredField]
        [ValueConverter(typeof(TypeToStringConverter))]
        public Type TargetObjectType
        {
            get { return _targetObjectType; }
            set
            {
                if (SetPropertyValue<Type>(nameof(TargetObjectType), ref _targetObjectType, value))
                    if (!this.IsLoading)
                        TargetObjectTypeChanged();
            }
        }
        private void TargetObjectTypeChanged()
        {
            if (string.IsNullOrEmpty(this.Name) && this.TargetObjectType != null)
                this.Name = $"{CaptionHelper.GetClassCaption(this.TargetObjectType.FullName)} - {DateTime.Now:yyyy-MM-dd}";
        }

        private bool _dataContainsHeaders;

        [ImmediatePostData]
        public bool DataContainsHeaders
        {
            get { return _dataContainsHeaders; }
            set { SetPropertyValue<bool>(nameof(DataContainsHeaders), ref _dataContainsHeaders, value); }
        }

        private int _startRow = 1;
        [ImmediatePostData]
        [Browsable(false)] //HACK: Disabled StartRow
        public int StartRow
        {
            get { return _startRow; }
            set
            {
                //StartRowChanging(ref value);
                SetPropertyValue<int>(nameof(StartRow), ref _startRow, value);
            }
        }
        private void StartRowChanging(ref int value)
        {
            if (value < 1)
                value = 1;
        }

        private int _startColumn = 1;
        [ImmediatePostData]
        [Browsable(false)] //HACK: Disabled StartColumn
        public int StartColumn
        {
            get { return _startColumn; }
            set
            {
                //StartColumnChanging(ref value);
                SetPropertyValue<int>(nameof(StartColumn), ref _startColumn, value);
            }
        }
        private void StartColumnChanging(ref int value)
        {
            if (value < 1)
                value = 1;
        }

        private ImportMode _importMode;
        public ImportMode ImportMode
        {
            get { return _importMode; }
            set { SetPropertyValue<ImportMode>(nameof(ImportMode), ref _importMode, value); }
        }

        private string _identifyRecordBy;
        [CriteriaOptions(nameof(TargetObjectType))]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        [ModelDefault("RowCount", "1")]
        [Size(SizeAttribute.Unlimited)]
        [ToolTip(@"Identify Record By.
Specifies how to identify the record uniqueness. Used for duplicate checks.
Empty values (question marks) should be left in the criteria.
Sample criteria: '[JOB_NO] = ? And [ID_NUMBER] = ? And [Deleted Reason] = ?'", "", ToolTipIconType.Information)]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string IdentifyRecordBy
        {
            get { return _identifyRecordBy; }
            set { SetPropertyValue<string>(nameof(IdentifyRecordBy), ref _identifyRecordBy, value); }
        }

        private ImportDefinitionExcelPreview _excelPreview;
        /// <summary>
        /// Wrapper for displaying excel preview with ColumnMappingControl custom control.
        /// </summary>
        [NonPersistent]
        [VisibleInListView(false), VisibleInLookupListView(false)]
        [Browsable(false)] //HACK: Disabled ExcelPreview
        public ImportDefinitionExcelPreview ExcelPreview
        {
            get
            {
                if (_excelPreview == null)
                {
                    _excelPreview = new ImportDefinitionExcelPreview(this.Session);
                    _excelPreview.Definition = this;
                }
                return _excelPreview;
            }
        }

        private string _ImportErrors;
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("RowCount", "2")]
        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string ImportErrors
        {
            get { return _ImportErrors; }
            set { SetPropertyValue<string>(nameof(ImportErrors), ref _ImportErrors, value); }
        }

        private string _emailTo = string.Empty;
        [ModelDefault("RowCount", "1")]
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)] //HACK: Disabled EmailTo
        public string EmailTo
        {
            get { return _emailTo; }
            set { SetPropertyValue<string>(nameof(EmailTo), ref _emailTo, value); }
        }

        private ImportDefinitionCellMapping _emailToMapping;
        [ModelDefault("AllowEdit", "False")]
        [ToolTip("This field is populated by right clicking on a cell (on Excel Preview)")]
        [Browsable(false)] //HACK: Disabled EmailToMapping
        public ImportDefinitionCellMapping EmailToMapping
        {
            get { return _emailToMapping; }
            set { SetPropertyValue<ImportDefinitionCellMapping>(nameof(EmailToMapping), ref _emailToMapping, value); }
        }

        private string _recordPostprocessCodeMethod;
        /// <summary>
        /// Record Postprocess Code Method. (Optional parameter). Method in code that is invoked after processing each record.
        /// The method requires to be static and have the following parameters:
        /// - Session session
        /// - object recordToImport
        /// Sample value: MySolution.MyProject.SomeClass.HelperMethod
        /// </summary>
        [Size(254)]
        [ToolTip(@"Record Postprocess Code Method. (Optional parameter). Method in code that is invoked after processing each record.
The method requires to be static and have the following parameters:
- Session session
- object recordToImport
Sample value: MySolution.MyProject.SomeClass.HelperMethod")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        [Browsable(false)] //HACK: Disabled RecordPostprocessCodeMethod
        public string RecordPostprocessCodeMethod
        {
            get { return _recordPostprocessCodeMethod; }
            set { SetPropertyValue<string>(nameof(RecordPostprocessCodeMethod), ref _recordPostprocessCodeMethod, value); }
        }

        private string _importPostprocessCodeMethod;
        /// <summary>
        /// Import Postprocess Code Method. (Optional parameter). Method in code that is invoked after the whole import process.
        /// The method requires to be static and have the following parameters:
        /// - Session session
        /// Sample value: MySolution.MyProject.SomeClass.HelperMethod
        /// </summary>
        [Size(254)]
        [ToolTip(@"Import Postprocess Code Method. (Optional parameter). Method in code that is invoked after the whole import process.
The method requires to be static and have the following parameters:
- Session session
Sample value: MySolution.MyProject.SomeClass.HelperMethod")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        [Browsable(false)] //HACK: Disabled ImportPostprocessCodeMethod
        public string ImportPostprocessCodeMethod
        {
            get { return _importPostprocessCodeMethod; }
            set { SetPropertyValue<string>(nameof(ImportPostprocessCodeMethod), ref _importPostprocessCodeMethod, value); }
        }

        [Association, Aggregated]
        public XPCollection<ImportDefinitionColumnMapping> ColumnMappings
        {
            get { return GetCollection<ImportDefinitionColumnMapping>(nameof(ColumnMappings)); }
        }

        [Association, Aggregated]
        [Browsable(false)] //HACK: Disabled CellMappings
        public XPCollection<ImportDefinitionCellMapping> CellMappings
        {
            get { return GetCollection<ImportDefinitionCellMapping>(nameof(CellMappings)); }
        }
    }

    public enum ImportMode
    {
        CreateOnly = 0,
        UpdateOnly = 1,
        CreateOrUpdate = 2
    }
}
