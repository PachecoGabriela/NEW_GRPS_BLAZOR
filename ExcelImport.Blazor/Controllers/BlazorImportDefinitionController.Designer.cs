using System;
using DevExpress.ExpressApp.DC;

namespace ExcelImport.Blazor.Controllers
{
    partial class ImportDefinitionWinController
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
            this.importExcelDataAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);

            // 
            // ImportDefinitionController
            // 
            this.Actions.Add(this.saveMappingsAction);
            this.Actions.Add(this.cancelExcelImportAction);
            this.Actions.Add(this.importExcelDataAction);
        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction saveMappingsAction;
        private DevExpress.ExpressApp.Actions.SimpleAction cancelExcelImportAction;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction importExcelDataAction;
    }

    public enum ImportExcelSendEmailModes
    {
        Always = 0,
        OnSuccess = 1,
        DoNotSend = 2
    }
}
