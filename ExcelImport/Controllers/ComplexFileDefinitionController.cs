using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExcelImport.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelImport.Controllers
{
    public partial class ComplexFileDefinitionController : ViewController
    {
        public SimpleAction ImportComplexData;
        SimpleAction CancelComplexExcelImportAction;
        public ComplexFileDefinitionController()
        {
            InitializeComponent();
            TargetObjectType = typeof(ComplexFileDefinition);
            TargetViewType = ViewType.DetailView;
            ImportComplexData = new SimpleAction(this, "ImportComplexData", PredefinedCategory.PopupActions)
            {
                Caption = "Import Data",
                ImageName = "ExportToXLSX",
                ToolTip = "Imports Excel records into the corresponding data type",
            };
            //ImportComplexData.Execute += ImportComplexData_Execute;
            CancelComplexExcelImportAction = new SimpleAction(this, "CancelComplexExcelImportAction", PredefinedCategory.PopupActions)
            {
                Caption = "Cancel",
                ToolTip = "Cancel Excel Import",
            };
            CancelComplexExcelImportAction.Execute += CancelComplexExcelImportAction_Execute;
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            IModelActionContainer container = ((IModelActionDesignContainerMapping)Application.Model.ActionDesign).ActionToContainerMapping["PopupActions"];
            ((IModelIndexedNode)container["ImportData"]).Index = 1;
            ((IModelIndexedNode)container["CancelExcelImportAction"]).Index = 2;
        }

        private void CancelComplexExcelImportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            this.View.Close();
        }

        private void ImportComplexData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
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

        public virtual ExcelImportHelper LoadExcelImportDocument(ComplexFileDefinition ComplexDefinition)
        {
            return null;
        }

        private void Instance_SaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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
    }
}
