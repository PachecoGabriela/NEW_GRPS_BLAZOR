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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRPS_BLAZOR.Module.Interfaces;
using DevExpress.ExpressApp.Blazor.SystemModule;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using DevExpress.ExpressApp.Xpo;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.CustomizeDashboardsActions
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CustomDualDashboard_PopupController : ViewController
    {
        PopupWindowShowAction ShowDetailView;
        NewObjectViewController newObjectViewController;
        DeleteObjectsViewController deleteObjectsViewController;
        ColumnChooserController columnChooserController;
        SingleChoiceAction newAction;
        public CustomDualDashboard_PopupController()
        {
            InitializeComponent();
            TargetViewId = "BOMItem_ListView_Custom;Part_ListView_Custom";
            ShowDetailView = new PopupWindowShowAction(this, "Edit", PredefinedCategory.Edit)
            {
                Caption = "Edit",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                ImageName = "Actions_Edit"
            };
            ShowDetailView.CustomizePopupWindowParams += ShowDetailView_CustomizePopupWindowParams;
        }

        private void ShowDetailView_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            if (View.Id == "BOMItem_ListView_Custom")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(BOM));
                Object Selected = objectSpace.GetObject(View.CurrentObject);
                if (Selected is BOMItem)
                {
                    BOM obj = objectSpace.GetObjectByKey<BOM>(((BOMItem)Selected).BOM.Oid);
                    DetailView createdView = Application.CreateDetailView(objectSpace, obj);
                    createdView.ViewEditMode = ViewEditMode.Edit;
                    e.View = createdView;
                }
            }

            if (View.Id == "Part_ListView_Custom")
            {
                IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Part));
                Object objectToShow = newObjectSpace.GetObject(View.CurrentObject);
                if (objectToShow != null)
                {
                    DetailView createdView = Application.CreateDetailView(newObjectSpace, objectToShow);
                    createdView.ViewEditMode = ViewEditMode.Edit;
                    e.View = createdView;
                }
            }

            //Object objectToShow = newObjectSpace.GetObject(View.CurrentObject);
            //if (objectToShow != null)
            //{
            //    DetailView createdView = Application.CreateDetailView(newObjectSpace, objectToShow);
            //    createdView.ViewEditMode = ViewEditMode.Edit;
            //    e.View = createdView;
            //}
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            newObjectViewController = Frame.GetController<NewObjectViewController>();
            deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            columnChooserController = Frame.GetController<ColumnChooserController>();
            if (View.Id == "BOMItem_ListView_Custom")
            {
                newObjectViewController.NewObjectAction.Active["MyNewReason"] = false;
                deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = false;
                columnChooserController.Active["MyReason"] = false;
            }
            if (View.Id == "Part_ListView_Custom")
            {
                deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = false;
                columnChooserController.Active["MyReason"] = false;
                newAction = newObjectViewController?.NewObjectAction;
                if (newAction is not null)
                {
                    newAction.Execute += NewObjectAction_Execute;
                }
            }
        }

        private void NewObjectAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {


            if (View.Id == "BOMItem_ListView_Custom")
            {
                newObjectViewController.NewObjectAction.Active["MyNewReason"] = true;
                deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = true;
                columnChooserController.Active["MyReason"] = true;
            }
            if (View.Id == "Part_ListView_Custom")
            {
                deleteObjectsViewController.DeleteAction.Active["MyReasonToDisable"] = true;
                columnChooserController.Active["MyReason"] = true;
                if (newAction is not null)
                {
                    newAction.Execute -= NewObjectAction_Execute;
                }
            }


            base.OnDeactivated();

        }
    }
}
