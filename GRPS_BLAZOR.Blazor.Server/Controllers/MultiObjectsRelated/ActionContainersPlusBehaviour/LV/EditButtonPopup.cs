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
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using GRPS_BLAZOR.Module.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.MultiObjectsRelated.ActionContainersPlusBehaviour.LV
{
    public partial class EditButtonPopup : ViewController<ListView>
    {
        PopupWindowShowAction ShowDetailView;
        ColumnChooserController chooserController;
        NewObjectViewController newObjectViewController;
        ListViewProcessCurrentObjectController listViewProcessCurrentObjectController;
        SingleChoiceAction newAction;
        public EditButtonPopup()
        {
            InitializeComponent();
            TargetObjectType = typeof(IEditInPopupButton);
            TargetViewType = ViewType.ListView;
            ShowDetailView = new PopupWindowShowAction(this, "Edit2", PredefinedCategory.Edit)
            {
                Caption = "Edit",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                ImageName = "Actions_Edit"
            };
            ShowDetailView.CustomizePopupWindowParams += ShowDetailView_CustomizePopupWindowParams;
        }

        private void ShowDetailView_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Supplier));
            Object objectToShow = objectSpace.GetObject(View.CurrentObject);
            if (objectToShow != null)
            {
                DetailView createdView = Application.CreateDetailView(objectSpace, objectToShow);
                createdView.ViewEditMode = ViewEditMode.Edit;
                e.View = createdView;
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcessCurrentObjectController.CustomProcessSelectedItem += ListViewProcessCurrentObjectController_CustomProcessSelectedItem;
            chooserController = Frame.GetController<ColumnChooserController>();
            chooserController.Active["MyReason"] = false;

            newObjectViewController = Frame.GetController<NewObjectViewController>();
            newAction = newObjectViewController?.NewObjectAction;
            if (newAction is not null)
            {
                newAction.Execute += NewObjectAction_Execute;
            }

        }

        private void NewObjectAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        }

        private void ListViewProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
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
            newAction.Execute -= NewObjectAction_Execute;
            listViewProcessCurrentObjectController.CustomProcessSelectedItem -= ListViewProcessCurrentObjectController_CustomProcessSelectedItem;
            base.OnDeactivated();
        }
    }
}
