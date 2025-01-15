using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraEditors;
using ExcelImport.BusinessObjects;
using ExcelImport.Controllers;

namespace ExcelImport.Blazor.Controllers
{
    /// <summary>
    /// Controller with actions "Save Mappings", "Cancel", "Import Data".
    /// Save Mappings persists the excel import control to the database (as Import Definition record).
    /// </summary>
    public partial class ImportDefinitionWinController : ImportDefinitionController
    {
        public ImportDefinitionWinController()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(ImportDefinition);
            this.TargetViewType = ViewType.DetailView;
            //Hack: Disabled showConfirmationWindowAction
            //SimpleAction showConfirmationWindowAction = new SimpleAction(this, "CustomConfirmationWindow", PredefinedCategory.View);
        }


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

        public override ExcelImportHelper LoadExcelImportDocument(ImportDefinition importDefinition)
        {
            return ExcelImportHelper.LoadDocument(importDefinition.ExcelPreview.FileContent);
        }
    }
}
