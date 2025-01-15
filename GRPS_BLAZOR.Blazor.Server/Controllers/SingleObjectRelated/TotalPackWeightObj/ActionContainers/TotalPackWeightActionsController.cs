using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using FrameworkUtils.Utils;
using GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup;
using GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup.ProcessAction;
using GRPS_BLAZOR.Module.BusinessObjects.Criteria;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using GRPS_BLAZOR.Module.Enums;
using GRPS_BLAZOR.Module.Helpers;
using GRPS_BLAZOR.Module.Helpers.ProgressIndicator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using XafCustomComponents;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SingleObjectRelated.TotalPackWeightObj.ActionContainers
{
    public partial class TotalPackWeightActionsController : ViewController
    {
        #region Services

        IServiceProvider ServiceProvider;
        IDialogService DialogService;

        #endregion

        #region Controllers



        #endregion

        #region Actions

        SimpleAction ProcessAction;

        #endregion

        BackgroundWorker ProcessActionWorker;
        WorkerStatus ProcessActionWorkerStatus;

        public TotalPackWeightActionsController()
        {
            InitializeComponent();

            TargetObjectType = typeof(TotalPackWeight);

            ProcessAction = new SimpleAction(this, "TotalPackWeight_ProcessAction", PredefinedCategory.RecordEdit)
            {
                Caption = "Process",
            };
            ProcessAction.Execute += ProcessAction_Execute;
        }

        #region ViewController Overrided methods

        protected override void OnActivated()
        {
            base.OnActivated();

            ServiceProvider = Application?.ServiceProvider;
            DialogService = ServiceProvider?.GetRequiredService<IDialogService>();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            ReleaseWorkers();
            base.OnDeactivated();
        }

        #endregion

        #region Actions EventHandlers

        private async void ProcessAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            InitializeProcessActionWorker();
            ProcessActionWorker.RunWorkerAsync();

            DialogParameters parameters = new DialogParameters() { { "Worker", ProcessActionWorker } };
            IDialogReference dialog = DialogService.Show<ProcessActionProgress>("Process Action", parameters);

            var result = await dialog.Result;
            if (result.Cancelled)
                ProcessActionWorkerStatus = WorkerStatus.Cancelled;
        }

        #endregion

        #region Methods

        private void InitializeProcessActionWorker()
        {
            ProcessActionWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            ProcessActionWorker.DoWork += ProcessActionWorker_DoWork;
            ProcessActionWorker.RunWorkerCompleted += ProcessActionWorker_RunWorkerCompleted;
            ProcessActionWorkerStatus = WorkerStatus.Running;
        }

        private void ProcessActionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            IList<ComplianceRule> rules = objectSpace.GetObjects<ComplianceRule>();

            ApplyRules(e, objectSpace, rules);
            CalculateTotalPackaging(e, objectSpace, rules);
            Operation3(e);
            Operation4(e);
        }

        private void ProcessActionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error is not null)
            {
                //TODO: Show exception message
            }
            else if (e.Cancelled) 
            {
                //View.ObjectSpace.Rollback(false);
            }
            else
            {
                if (View.ObjectSpace.IsModified)
                {
                    //View.ObjectSpace.CommitChanges();
                }
            }
            ProcessActionWorkerStatus = WorkerStatus.Stopped;
            ProcessActionWorker?.Dispose();
        }

        private void ApplyRules(DoWorkEventArgs e, IObjectSpace objectSpace, IList<ComplianceRule> rules)
        {
            if (ProcessActionWorkerStatus != WorkerStatus.Running)
            {
                e.Cancel = true;
                //objectSpace.Rollback(false);
                return;
            }

            int seq = 0;
            int total = rules.Count;
            int percentage = 0;
            ProcessActionState state = new ProcessActionState();
            state.CurrentOperation = ProcessOperation.ApplyingRules;
            foreach (ComplianceRule rule in rules)
            {
                if (ProcessActionWorkerStatus != WorkerStatus.Running)
                {
                    e.Cancel = true;
                    return;
                }

                percentage = ProgressIndicatorHelper.GetIntegerPercentage(seq, total);
                seq++;
                state.CurrentOperationPercentage = percentage;
                state.OperationElementsDetail = $"Applying: rule {seq} of {total}";
                state.OperationCurrentElementDetail = $"{rule.Name}";
                ProcessActionWorker.ReportProgress(percentage, state);

                RuleProcessor ruleProcessor = new RuleProcessor(rule, objectSpace);
                ruleProcessor.ProcessRule();

                percentage = ProgressIndicatorHelper.GetIntegerPercentage(seq, total);
                state.CurrentOperationPercentage = percentage;
                ProcessActionWorker.ReportProgress(percentage, state);
                System.Threading.Thread.Sleep(100);
            }

            CommitChangesAndReportProgress(objectSpace, percentage, state);
        }

        private void CalculateTotalPackaging(DoWorkEventArgs e, IObjectSpace objectSpace, IList<ComplianceRule> rules)
        {
            if (ProcessActionWorkerStatus != WorkerStatus.Running)
            {
                e.Cancel = true;
                return;
            }

            int seq = 0;
            int percentage = 0;
            int total = rules.Count;
            ProcessActionWorker.ReportProgress(percentage);
            ProcessActionState state = new ProcessActionState();
            state.CurrentOperation = ProcessOperation.CalculatingTotalPackaging;

            var uow = new UnitOfWork(objectSpace.Session().DataLayer);

            foreach (ComplianceRule rule in rules)
            {
                if (ProcessActionWorkerStatus != WorkerStatus.Running)
                {
                    e.Cancel = true;
                    return;
                }

                //TotalPackagingProcessor totalPackagingProcessor = new TotalPackagingProcessor(rule, objectSpace, Application);

                TotalPackagingProcessor totalPackagingProcessor = new TotalPackagingProcessor(rule, objectSpace, uow);

                percentage = ProgressIndicatorHelper.GetIntegerPercentage(seq, total);
                seq++;
                state.CurrentOperationPercentage = percentage;
                state.OperationElementsDetail = $"Calculating for: rule {seq} of {total}";
                state.OperationCurrentElementDetail = $"Removing old TotalPackWeight records for rule: {rule.Name}";
                ProcessActionWorker.ReportProgress(percentage, state);
                totalPackagingProcessor.RemoveOldTotalPackWeightInfo();
                System.Threading.Thread.Sleep(300);

                state.OperationCurrentElementDetail = $"Calculating new TotalPackWeight records for rule: {rule.Name}";
                ProcessActionWorker.ReportProgress(percentage, state);
                totalPackagingProcessor.CalculateTotalPackWeight();
                percentage = ProgressIndicatorHelper.GetIntegerPercentage(seq, total);
                state.CurrentOperationPercentage = percentage;
                ProcessActionWorker.ReportProgress(percentage, state);
            }

            //CommitChangesAndReportProgress(objectSpace, percentage, state);

            CommitChangesAndReportProgress(uow, percentage, state);

            //state.OperationElementsDetail = "Saving the new TotalPackWeight records in the database";
            //state.OperationCurrentElementDetail = string.Empty;
            //ProcessActionWorker.ReportProgress(percentage, state);
            //if (objectSpace.IsModified)
            //    objectSpace.CommitChanges();
        }

        private void Operation3(DoWorkEventArgs e)
        {
            if (ProcessActionWorkerStatus != WorkerStatus.Running)
            {
                e.Cancel = true;
                return;
            }
            int percentage = 0;
            ProcessActionWorker.ReportProgress(percentage);
            ProcessActionState state = new ProcessActionState();
            state.CurrentOperation = ProcessOperation.Operation3;
            System.Threading.Thread.Sleep(500);
            for (int i = 1; i <= 10; i++)
            {
                if (ProcessActionWorkerStatus != WorkerStatus.Running)
                {
                    e.Cancel = true;
                    return;
                }
                percentage = ProgressIndicatorHelper.GetIntegerPercentage(i, 10);
                state.CurrentOperationPercentage = percentage;
                state.OperationElementsDetail = $"Calculating: item {i} of {10}";
                state.OperationCurrentElementDetail = $"Name of item {i}";
                ProcessActionWorker.ReportProgress(percentage, state);
                System.Threading.Thread.Sleep(500);
            }
        }

        private void Operation4(DoWorkEventArgs e)
        {
            if (ProcessActionWorkerStatus != WorkerStatus.Running)
            {
                e.Cancel = true;
                return;
            }
            int percentage = 0;
            ProcessActionWorker.ReportProgress(percentage);
            ProcessActionState state = new ProcessActionState();
            state.CurrentOperation = ProcessOperation.Operation4;
            System.Threading.Thread.Sleep(2000);
            for (int i = 1; i <= 15; i++)
            {
                if (ProcessActionWorkerStatus != WorkerStatus.Running)
                {
                    e.Cancel = true;
                    return;
                }
                percentage = ProgressIndicatorHelper.GetIntegerPercentage(i, 15);
                state.CurrentOperationPercentage = percentage;
                state.OperationElementsDetail = $"Calculating: item {i} of {15}";
                state.OperationCurrentElementDetail = $"Name of item {i}";
                ProcessActionWorker.ReportProgress(percentage, state);
                System.Threading.Thread.Sleep(500);
            }
        }

        private void ReleaseWorkers()
        {
            ProcessActionWorkerStatus = WorkerStatus.Stopped;
            ProcessActionWorker?.Dispose();
        }


        private void CommitChangesAndReportProgress(IObjectSpace objectSpace, int percentage, ProcessActionState state, string text = "Saving changes in the database")
        {
            if (objectSpace.IsModified)
            {
                state.Indeterminate = true;
                state.OperationElementsDetail = text;
                state.OperationCurrentElementDetail = string.Empty;
                ProcessActionWorker.ReportProgress(percentage, state);
                objectSpace.CommitChanges();
                state.Indeterminate = false;
                state.OperationElementsDetail = string.Empty;
                state.OperationCurrentElementDetail = string.Empty;
                ProcessActionWorker.ReportProgress(percentage, state);
            }
        }

        private void CommitChangesAndReportProgress(UnitOfWork uow, int percentage, ProcessActionState state, string text = "Saving changes in the database")
        {
            state.Indeterminate = true;
            state.OperationElementsDetail = text;
            state.OperationCurrentElementDetail = string.Empty;
            ProcessActionWorker.ReportProgress(percentage, state);
            uow.CommitChanges();
            state.Indeterminate = false;
            state.OperationElementsDetail = string.Empty;
            state.OperationCurrentElementDetail = string.Empty;
            ProcessActionWorker.ReportProgress(percentage, state);
        }

        #endregion
    }
}
