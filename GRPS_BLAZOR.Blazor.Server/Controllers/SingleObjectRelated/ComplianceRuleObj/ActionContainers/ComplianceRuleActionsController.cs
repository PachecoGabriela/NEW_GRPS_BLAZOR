using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup;
using GRPS_BLAZOR.Module.BusinessObjects.Criteria;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using GRPS_BLAZOR.Module.Helpers.ProgressIndicator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XafCustomComponents;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SingleObjectRelated.ComplianceRuleObj.ActionContainers
{
    public partial class ComplianceRuleActionsController : ViewController
    {
        #region Services

        IServiceProvider ServiceProvider;
        IDialogService DialogService;

        #endregion

        #region Controllers



        #endregion

        #region Actions

        SimpleAction ApplyNowAction;

        #endregion

        BackgroundWorker ApplyNowActionWorker;

        public ComplianceRuleActionsController()
        {
            InitializeComponent();
            TargetObjectType = typeof(ComplianceRule);

            ApplyNowAction = new SimpleAction(this, "ComplianceRule_ApplyNowAction", PredefinedCategory.RecordEdit)
            {
                Caption = "Apply Now",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
            };
            ApplyNowAction.Execute += ApplyNowAction_Execute;
        }

        #region Overrided methods

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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
            ReleaseWorkers();
            base.OnDeactivated();
        }

        #endregion

        #region Actions EventHandlers

        private async void ApplyNowAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ComplianceRule currentRule = e.CurrentObject as ComplianceRule;
            FilteringCriterion criteria = currentRule?.CriteriaRule;

            InitializeApplyNowActionWorker();
        
            ApplyNowActionWorker.RunWorkerAsync(currentRule);

            DialogParameters parameters = new DialogParameters() { { "Worker", ApplyNowActionWorker } };
            IDialogReference dialog = DialogService.Show<ProgressMessageBox>("Test Progress", parameters);
            var result = await dialog.Result;
        }

        #endregion

        private void InitializeApplyNowActionWorker()
        {
            ApplyNowActionWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            ApplyNowActionWorker.DoWork += ApplyNowActionWorker_DoWork;
            ApplyNowActionWorker.RunWorkerCompleted += ApplyNowActionWorker_RunWorkerCompleted;
        }

        private void ApplyNowActionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ComplianceRule currentRule = e.Argument as ComplianceRule;
            //(IObjectSpace ObjectSpace, ComplianceRule CurrentRule) WorkerArgs = ((IObjectSpace ObjectSpace, ComplianceRule CurrentRule))e.Argument;
            FilteringCriterion criteria = currentRule.CriteriaRule;

            if (criteria is not null)
            {
                IList<SalesVolume> sales = View.ObjectSpace.GetObjects<SalesVolume>(CriteriaOperator.Parse(criteria.Criteria));
                int percentage = 0;
                int total = sales.Count;
                ProgressIndicatorState state = new ProgressIndicatorState();
                state.TotalOperations = total;
                foreach (SalesVolume sale in sales)
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    sale.Rule = currentRule;
                    sale.BOM = sale.Product?.ActiveBOM;
                    System.Threading.Thread.Sleep(500);
                    watch.Stop();
                    percentage++;
                    int realPercentage = ProgressIndicatorHelper.GetIntegerPercentage(percentage, total);
                    int expectedCompletionTime = ProgressIndicatorHelper.GetCompletionTime(watch.ElapsedMilliseconds, percentage, total);
                    state.OperationNum = percentage;
                    state.ExpectedCompletionTime = expectedCompletionTime;
                    ApplyNowActionWorker.ReportProgress(realPercentage, state);
                }
            }
        }

        private void ApplyNowActionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (View.ObjectSpace.IsModified)
                View.ObjectSpace.CommitChanges();
            ApplyNowActionWorker?.Dispose();
        }

        private void ReleaseWorkers()
        {
            ApplyNowActionWorker?.CancelAsync();
            ApplyNowActionWorker?.Dispose();
        }
    }
}
