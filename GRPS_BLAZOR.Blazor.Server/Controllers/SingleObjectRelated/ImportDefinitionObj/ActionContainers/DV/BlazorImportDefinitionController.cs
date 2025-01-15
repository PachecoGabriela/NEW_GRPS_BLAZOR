using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExcelImport;
using ExcelImport.Blazor.Controllers;
using ExcelImport.BusinessObjects;
using GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup.ImportExcelData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XafCustomComponents;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SingleObjectRelated.ImportDefinitionObj.ActionContainers.DV
{
    public partial class BlazorImportDefinitionController : ImportDefinitionWinController
    {
        #region Services

        IServiceProvider ServiceProvider;
        IDialogService DialogService;
        INotificationService NotificationService;

        #endregion

        IDialogReference excelImportProgressDialog;
        BackgroundWorker ImportDataWorker;
        ImportExcelWorkerStatus ImportDataWorkerStatus;
        ImportDefinition importDefinition;
        ExcelImportHelper helper;

        public BlazorImportDefinitionController()
        {
            InitializeComponent();
            ImportData.Execute += ImportData_Execute;
        }

        #region Actions EventHandlers

        private async void ImportData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            InitializeImporDataActionWorker();

            DetailView view = this.View as DetailView;
            if (view == null)
                return;

            if (!CheckControl())
                return;

            importDefinition = view.CurrentObject as ImportDefinition;
            if (importDefinition == null)
                return;

            // commit import definition first.
            this.ObjectSpace.CommitChanges();

            ImportDataWorker.RunWorkerAsync();

            DialogParameters parameters = new DialogParameters() { { "Worker", ImportDataWorker } };
            excelImportProgressDialog = DialogService.Show<ImportExcelDataProgress>("Import Excel", parameters);

            var result = await excelImportProgressDialog.Result;
            if (result.Cancelled)
            {
                ImportDataWorkerStatus = ImportExcelWorkerStatus.Cancelled;
                helper.WorkerStatus = ImportExcelWorkerStatus.Cancelled;
            }
        }

        private void InitializeImporDataActionWorker()
        {
            ImportDataWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            ImportDataWorker.DoWork += ImportDataWorker_DoWork;
            ImportDataWorker.RunWorkerCompleted += ImportDataWorker_RunWorkerCompleted;
            ImportDataWorkerStatus = ImportExcelWorkerStatus.Running;
        }

        #endregion

        #region ViewController Overrided methods

        protected override void OnActivated()
        {
            base.OnActivated();
            ServiceProvider = Application?.ServiceProvider;
            DialogService = ServiceProvider?.GetRequiredService<IDialogService>();
            NotificationService = ServiceProvider?.GetRequiredService<INotificationService>();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        #endregion

        #region ImportDefinitionWinController Overrided methods

        public override void ShowSuccessMessage(string message, ImportObjectResult importObjectResult)
        {
            Application.ShowViewStrategy.ShowMessage(message, InformationType.Success, 3000, InformationPosition.Bottom);
        }

        public override bool CheckControl()
        {
            return true;
        }

        public override void ShowDataValueIncorrectDialog(IObjectSpace space, ImportObjectResult importObjectResult, bool shouldCloseView)
        {
            // We don't want to show dialog in blazor. Instead of this show message.
            Application.ShowViewStrategy.ShowMessage(importObjectResult.GetInformation(), InformationType.Warning, 3000, InformationPosition.Bottom);
        }

        #endregion

        #region Methods

        private void ImportDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ImportObjectResult importObjectResult = null;

            using (IObjectSpace space = this.Application.CreateObjectSpace())
            {
                bool shouldCreateLog = true;
                bool shouldCloseView = false;

                IAuditTrailService auditTrailService = Application.ServiceProvider.GetRequiredService<IAuditTrailService>();

                try
                {

                    auditTrailService.SaveAuditTrailData += Instance_SaveAuditTrailData;

                    helper = LoadExcelImportDocument(importDefinition);
                    ImportDefinition importDefinitionOnMySession = space.GetObject(importDefinition);

                    helper.Worker = ImportDataWorker;
                    helper.WorkerStatus = ImportDataWorkerStatus;

                    importObjectResult = helper.ImportRecords(importDefinitionOnMySession, space, e);
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
                    NotificationService.ShowErrorMessage(ex.Message);
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
                        View.Close();
                    auditTrailService.SaveAuditTrailData -= Instance_SaveAuditTrailData;
                }
            }
        }

        private void ImportDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error is not null)
            {
                //Close Progress Dialog in case of an error
                excelImportProgressDialog?.Close();
            }

            ImportDataWorkerStatus = ImportExcelWorkerStatus.Stopped;
            ImportDataWorker?.Dispose();
        }

        private void Instance_SaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }
}
