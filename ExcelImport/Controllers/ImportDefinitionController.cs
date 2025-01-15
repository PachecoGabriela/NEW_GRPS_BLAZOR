using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Validation;
using ExcelImport.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;

namespace ExcelImport.Controllers
{
    /// <summary>
    /// Controller with actions "Save Mappings", "Cancel", "Import Data".
    /// Save Mappings persists the excel import control to the database (as Import Definition record).
    /// </summary>
    public partial class ImportDefinitionController : ViewController
    {

        public SimpleAction ImportData { get; set; }

        public ImportDefinitionController()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(ImportDefinition);
            this.TargetViewType = ViewType.DetailView;
            ImportData = new SimpleAction(this, "ImportData", DevExpress.Persistent.Base.PredefinedCategory.PopupActions)
            {
                Caption = "Import Data",
                ImageName = "ExportToXLSX",
                ToolTip = "Imports Excel records into the corresponding data type"
            };
            //ImportData.Execute += ImportData_Execute;
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            IModelActionContainer container = ((IModelActionDesignContainerMapping)Application.Model.ActionDesign).ActionToContainerMapping["PopupActions"];
            ((IModelIndexedNode)container["SaveMappingsAction"]).Index = 1;
            ((IModelIndexedNode)container["ImportData"]).Index = 2;
            ((IModelIndexedNode)container["CancelExcelImportAction"]).Index = 3;
        }

        private void ImportData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DetailView view = this.View as DetailView;
            if (view == null)
                return;

            if (!CheckControl())
                return;

            ImportDefinition importDefinition = view.CurrentObject as ImportDefinition;
            if (importDefinition == null)
                return;

            // commit import definition first.
            this.ObjectSpace.CommitChanges();

            ImportObjectResult importObjectResult = null;

            using (IObjectSpace space = this.Application.CreateObjectSpace())
            {
                bool shouldCreateLog = true;
                bool shouldCloseView = false;

                IAuditTrailService auditTrailService = Application.ServiceProvider.GetRequiredService<IAuditTrailService>();

                try
                {
                    //AuditTrailService.Instance.SaveAuditTrailData += Instance_SaveAuditTrailData;

                    auditTrailService.SaveAuditTrailData += Instance_SaveAuditTrailData;

                    ExcelImportHelper helper = LoadExcelImportDocument(importDefinition);
                    ImportDefinition importDefinitionOnMySession = space.GetObject(importDefinition);

                    importObjectResult = helper.ImportRecords(importDefinitionOnMySession, space);
                    importObjectResult.ImportDefinition = importDefinitionOnMySession;

                    if (importObjectResult.HasErrors())
                    {
                        string message = importObjectResult.GetLogMessage();
                        importDefinition.ImportErrors = message;
                        throw new Exception("Errors occurred during the import process.");
                    }
                    else if (importObjectResult.HasWarnings())
                    {
                        string warnings = string.Join("\r\n", importObjectResult.GetWarnings(), importObjectResult.GetInformation());
                        importDefinition.ImportErrors = warnings;
                        ShowDataValueIncorrectDialog(space, importObjectResult, shouldCloseView);
                    }
                    else
                    {
                        space.CommitChanges();
                        string message = importObjectResult.GetLogMessage();
                        if (string.IsNullOrWhiteSpace(message))
                            message = "Success";
                        ShowSuccessMessage(message, importObjectResult);

                        shouldCloseView = true;
                    }
                }
                catch (Exception ex)
                {
                    space.Rollback();
                    throw new UserFriendlyException(ex);
                }
                finally
                {
                    if (shouldCreateLog)
                    {
                        if (importObjectResult != null && !importObjectResult.HasErrors())
                        {
                            importObjectResult.ImportDefinition = space.GetObject(importDefinition); // prevents from Object Disposed exception
                            ImportLog importLog = ImportLog.CreateImportLog((space as XPObjectSpace).Session, importObjectResult, importDefinition.ExcelPreview.FileName);
                            space.CommitChanges();
                        }
                    }
                    if (shouldCloseView)
                        view.Close();
                    //AuditTrailService.Instance.SaveAuditTrailData -= Instance_SaveAuditTrailData;
                    auditTrailService.SaveAuditTrailData -= Instance_SaveAuditTrailData;
                }
            }
        }

        //private void ImportExcelDataAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        //{
        //    DetailView view = this.View as DetailView;
        //    if (view == null)
        //        return;

        //    if (!CheckControl())
        //        return;

        //    ImportDefinition importDefinition = view.CurrentObject as ImportDefinition;
        //    if (importDefinition == null)
        //        return;

        //    // commit import definition first.
        //    this.ObjectSpace.CommitChanges();

        //    ImportObjectResult importObjectResult = null;
        //    ChoiceActionItem item = this.importExcelDataAction.SelectedItem;

        //    using (IObjectSpace space = this.Application.CreateObjectSpace())
        //    {
        //        bool shouldCreateLog = true;
        //        bool shouldCloseView = false;

        //        try
        //        {
        //            AuditTrailService.Instance.SaveAuditTrailData += Instance_SaveAuditTrailData;

        //            ExcelImportHelper helper = LoadExcelImportDocument(importDefinition);
        //            ImportDefinition importDefinitionOnMySession = space.GetObject(importDefinition);

        //            importObjectResult = helper.ImportRecords(importDefinitionOnMySession);
        //            importObjectResult.ImportDefinition = importDefinitionOnMySession;

        //            if (importObjectResult.HasErrors())
        //            {
        //                string message = importObjectResult.GetLogMessage();
        //                importDefinition.ImportErrors = message;
        //                throw new Exception("Errors occurred during the import process.");
        //            }
        //            else if (importObjectResult.HasWarnings())
        //            {
        //                string warnings = string.Join("\r\n", importObjectResult.GetWarnings(), importObjectResult.GetInformation());
        //                importDefinition.ImportErrors = warnings;
        //                ShowDataValueIncorrectDialog(space, importObjectResult, shouldCloseView);
        //            }
        //            else
        //            {
        //                space.CommitChanges();
        //                string message = importObjectResult.GetLogMessage();
        //                if (string.IsNullOrWhiteSpace(message))
        //                    message = "Success";
        //                ShowSuccessMessage(message, importObjectResult);

        //                shouldCloseView = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            space.Rollback();
        //            throw new UserFriendlyException(ex);
        //        }
        //        finally
        //        {
        //            if (shouldCreateLog && item != null && item.Data is ImportExcelSendEmailModes itemImportExcelDataModes)
        //            {
        //                if (importObjectResult != null && !importObjectResult.HasErrors())
        //                {
        //                    importObjectResult.ImportDefinition = space.GetObject(importDefinition); // prevents from Object Disposed exception
        //                    ImportLog importLog = ImportLog.CreateImportLog((space as XPObjectSpace).Session, importObjectResult, importDefinition.ExcelPreview.FileName);
        //                    space.CommitChanges();
        //                }
        //            }
        //            if (shouldCloseView)
        //                view.Close();
        //            AuditTrailService.Instance.SaveAuditTrailData -= Instance_SaveAuditTrailData;
        //        }
        //    }
        //}


        /// <summary>
        /// Commit changes, if they validate.
        /// </summary>
        public virtual void SaveMappingsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }

        /// <summary>
        /// Closes the view.
        /// Propmts the user if there are unsaved changes.
        /// </summary>
        public virtual void CancelExcelImportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            this.View.Close();
        }

        public virtual void ShowDataValueIncorrectDialog(IObjectSpace space, ImportObjectResult importObjectResult, bool shouldCloseView)
        {
        }

        public virtual void ShowSuccessMessage(string message, ImportObjectResult importObjectResult)
        {
        }

        public virtual bool CheckControl()
        {
            return false;
        }

        public virtual ExcelImportHelper LoadExcelImportDocument(ImportDefinition importDefinition)
        {
            return null;
        }

        private void Instance_SaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs e)
        {
            e.Handled = true;
        }
    }
}
