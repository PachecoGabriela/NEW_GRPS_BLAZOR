
using DevExpress.Persistent.Base;

namespace ExcelImport.Controllers
{
    partial class ImportFromExcelViewViewController
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
            this.actionImportFromExcel = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // actionImportFromExcel
            // 
            this.actionImportFromExcel.Caption = "Import From Excel";
            this.actionImportFromExcel.Category = "RecordEdit";
            this.actionImportFromExcel.ConfirmationMessage = null;
            this.actionImportFromExcel.Id = "actionImportFromExcel";
            this.actionImportFromExcel.ImageName = "ExportToXLSX";
            this.actionImportFromExcel.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
            this.actionImportFromExcel.ToolTip = null;
            this.actionImportFromExcel.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ActionImportFromExcel_CustomizePopupWindowParams);
            this.actionImportFromExcel.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ActionImportFromExcel_Execute);
            // 
            // ImportFromExcelController
            // 
            this.Actions.Add(this.actionImportFromExcel);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction actionImportFromExcel;
    }
}
