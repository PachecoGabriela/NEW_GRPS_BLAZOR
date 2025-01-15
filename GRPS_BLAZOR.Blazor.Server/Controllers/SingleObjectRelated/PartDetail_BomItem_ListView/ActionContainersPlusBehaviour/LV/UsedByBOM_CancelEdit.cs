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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SingleObjectRelated.PartDetail_BomItem_ListView.ActionContainersPlusBehaviour.LV
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class UsedByBOM_CancelEdit : ViewController<ListView>
    {
        ColumnChooserController chooserController;
        NewObjectViewController newObjectViewController;
        RefreshController refreshController;
        FilterController filterController;
        BlazorExportController blazorExportController;
        DeleteObjectsViewController deleteObjectsViewController;
        LinkUnlinkController unlinkController;

        public UsedByBOM_CancelEdit()
        {
            InitializeComponent();
            TargetViewId = "Part_BOMItems_ListView";
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ListViewProcessCurrentObjectController listViewProcess = Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcess.CustomProcessSelectedItem += ListViewProcess_CustomProcessSelectedItem;
            chooserController = Frame.GetController<ColumnChooserController>();
            newObjectViewController = Frame.GetController<NewObjectViewController>();
            deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            refreshController = Frame.GetController<RefreshController>();
            filterController = Frame.GetController<FilterController>();
            blazorExportController = Frame.GetController<BlazorExportController>();
            unlinkController= Frame.GetController<LinkUnlinkController>();

            chooserController.Active["MyReason"] = false;
            newObjectViewController.NewObjectAction.Active["MyReason"] = false;
            deleteObjectsViewController.DeleteAction.Active["MyReason"] = false;
            refreshController.RefreshAction.Active["MyReason"] = false;
            filterController.FullTextFilterAction.Active["MyReason"] = false;
            blazorExportController.ExportAction.Active["MyReason"] = false;
            unlinkController.LinkAction.Active["MyReason"] = false;
            unlinkController.UnlinkAction.Active["MyReason"] = false;
            // Perform various tasks depending on the target View.
        }

        private void ListViewProcess_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            chooserController.Active["MyReason"] = true;
            newObjectViewController.NewObjectAction.Active["MyNewReason"] = true;
            deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = true;
            refreshController.RefreshAction.Active["ReasonToDeactivate"] = true;
            filterController.FullTextFilterAction.Active["ReasonToDeactivate"] = true;
            blazorExportController.ExportAction.Active["DeactivateInDashboard"] = true;
            unlinkController.LinkAction.Active["MyReason"] = true;
            unlinkController.UnlinkAction.Active["MyReason"] = true;
            base.OnDeactivated();
        }
    }
}
