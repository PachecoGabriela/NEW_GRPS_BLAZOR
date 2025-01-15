using System;
using DevExpress.ExpressApp.DC;

namespace ExcelImport.Controllers
{
    partial class ImportDefinitionController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.saveMappingsAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cancelExcelImportAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            //this.importExcelDataAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // importExcelDataAction
            // 
            //this.importExcelDataAction.Caption = "Import Data";
            //this.importExcelDataAction.Category = "PopupActions";
            //this.importExcelDataAction.ConfirmationMessage = null;
            //this.importExcelDataAction.Id = "ImportExcelDataAction";
            //this.importExcelDataAction.ImageName = "ExportToXLSX";
            //this.importExcelDataAction.ToolTip = "Imports Excel records into the corresponding data type.";
            //this.importExcelDataAction.Items.Add(new DevExpress.ExpressApp.Actions.ChoiceActionItem() { Caption = "Import Data and Send Email", Data = ImportExcelSendEmailModes.Always, ImageName = "ExportToXLSX" });
            //this.importExcelDataAction.Items.Add(new DevExpress.ExpressApp.Actions.ChoiceActionItem() { Caption = "Import Data and Send Email On Success", Data = ImportExcelSendEmailModes.OnSuccess, ImageName = "ExportToXLSX" });
            //this.importExcelDataAction.Items.Add(new DevExpress.ExpressApp.Actions.ChoiceActionItem() { Caption = "Import Data and Do Not Send Email", Data = ImportExcelSendEmailModes.DoNotSend, ImageName = "ExportToXLSX" });
            //this.importExcelDataAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            //this.importExcelDataAction.ShowItemsOnClick = true;
            //this.importExcelDataAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.ImportExcelDataAction_Execute);
            // 
            // saveMappingsAction
            // 
            this.saveMappingsAction.Caption = "Save Mappings";
            this.saveMappingsAction.Category = "PopupActions";
            this.saveMappingsAction.ConfirmationMessage = null;
            this.saveMappingsAction.Id = "SaveMappingsAction";
            this.saveMappingsAction.ImageName = "Action_Save";
            this.saveMappingsAction.ToolTip = "Saves the mappings in the currently opened Import Definition record.";
            this.saveMappingsAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SaveMappingsAction_Execute);
            // 
            // cancelExcelImportAction
            // 
            this.cancelExcelImportAction.Caption = "Cancel";
            this.cancelExcelImportAction.Category = "PopupActions";
            this.cancelExcelImportAction.ConfirmationMessage = null;
            this.cancelExcelImportAction.Id = "CancelExcelImportAction";
            this.cancelExcelImportAction.ImageName = "";
            this.cancelExcelImportAction.ToolTip = null;
            this.cancelExcelImportAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CancelExcelImportAction_Execute);
            // 
            // ImportDefinitionController
            // 
            this.Actions.Add(this.saveMappingsAction);
            //this.Actions.Add(this.importExcelDataAction);
            this.Actions.Add(this.cancelExcelImportAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction saveMappingsAction;
        private DevExpress.ExpressApp.Actions.SimpleAction cancelExcelImportAction;
        //private DevExpress.ExpressApp.Actions.SingleChoiceAction importExcelDataAction;
    }

    public enum ImportExcelSendEmailModes
    {
        Always = 0,
        OnSuccess = 1,
        DoNotSend = 2
    }
}
