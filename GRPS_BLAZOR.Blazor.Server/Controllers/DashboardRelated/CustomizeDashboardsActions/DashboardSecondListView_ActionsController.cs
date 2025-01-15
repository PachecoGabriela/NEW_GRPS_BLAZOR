using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExcelImport.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.CustomizeDashboardsActions
{
    public partial class DashboardSecondListView_ActionsController : ViewController
    {
        NewObjectViewController newObjectViewController;
        DeleteObjectsViewController deleteObjectsViewController;
        ColumnChooserController columnChooserController;
        RefreshController refreshController;
        FilterController filterController;
        BlazorExportController blazorExportController;
        FilterEditorController filterEditorController;
        ImportFromExcelViewViewController ImportFromExcelViewViewController;
        public DashboardSecondListView_ActionsController()
        {
            InitializeComponent();
            TargetViewId = "PackWeight_ListView_Custom;PackWeight_ListView_PartDashboard;PackWeight_ListView_ProductDashboard_Second;BOMItem_ListView_Custom_ProductDashboard;PackWeight_ListView_Custom_ProductDashboard";
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.Id == "PackWeight_ListView_Custom" || View.Id == "PackWeight_ListView_PartDashboard" || View.Id == "PackWeight_ListView_ProductDashboard_Second" || View.Id == "BOMItem_ListView_Custom_ProductDashboard" || View.Id == "PackWeight_ListView_Custom_ProductDashboard")
            {
                newObjectViewController = Frame.GetController<NewObjectViewController>();
                deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
                columnChooserController = Frame.GetController<ColumnChooserController>();
                refreshController = Frame.GetController<RefreshController>();
                filterController = Frame.GetController<FilterController>();
                blazorExportController = Frame.GetController<BlazorExportController>();
                filterEditorController = Frame.GetController<FilterEditorController>();
                ImportFromExcelViewViewController = Frame.GetController<ImportFromExcelViewViewController>();

                if (newObjectViewController != null)
                {
                    newObjectViewController.NewObjectAction.Active["MyNewReason"] = false;
                    deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = false;
                    columnChooserController.Active["MyReason"] = false;
                    refreshController.RefreshAction.Active["ReasonToDeactivate"] = false;
                    filterController.FullTextFilterAction.Active["ReasonToDeactivate"] = false;
                    blazorExportController.ExportAction.Active["DeactivateInDashboard"] = false;
                    filterEditorController.FilterEditorAction.Active["DeactivateInDashboard"] = false;
                    ImportFromExcelViewViewController.Active["Deactivate"] = false;
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            if (View.Id == "PackWeight_ListView_Custom" || View.Id == "PackWeight_ListView_PartDashboard" || View.Id == "PackWeight_ListView_ProductDashboard_Second" || View.Id == "BOMItem_ListView_Custom_ProductDashboard" || View.Id == "PackWeight_ListView_Custom_ProductDashboard")
            {
                if (newObjectViewController != null)
                {
                    newObjectViewController.NewObjectAction.Active["MyNewReason"] = true;
                    deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = true;
                    columnChooserController.Active["MyReason"] = true;
                    refreshController.RefreshAction.Active["ReasonToDeactivate"] = true;
                    filterController.FullTextFilterAction.Active["ReasonToDeactivate"] = true;
                    blazorExportController.ExportAction.Active["DeactivateInDashboard"] = true;
                    filterEditorController.FilterEditorAction.Active["DeactivateInDashboard"] = true;
                    ImportFromExcelViewViewController.Active["Deactivate"] = true;
                }

            }
            base.OnDeactivated();
        }
    }
}
