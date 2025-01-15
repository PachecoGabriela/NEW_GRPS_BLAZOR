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
using ExcelImport.BusinessObjects;
using ExcelImport.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelImport.Blazor.Controllers
{
    public partial class BlazorComplexFileDefinitionController : ComplexFileDefinitionController
    {
        public BlazorComplexFileDefinitionController()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(ComplexFileDefinition);
            this.TargetViewType = ViewType.DetailView;
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

        public override ExcelImportHelper LoadExcelImportDocument(ComplexFileDefinition complexDefinition)
        {
            return ExcelImportHelper.LoadDocument(complexDefinition.ExcelPreview.FileContent);
        }
    }
}
