using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ExcelImport.BusinessObjects;
using ExcelImport.Extensions;
using ExcelImport.Interfaces;

namespace ExcelImport.Controllers
{
    public partial class ImportFromExcelViewViewController : ViewController, IModelExtender
    {
        PopupWindowShowAction ImportComplexFile;
        public ImportFromExcelViewViewController()
        {
            InitializeComponent();
            this.TargetViewType = ViewType.ListView;
            this.TargetViewNesting = Nesting.Root;
            this.TargetObjectType = typeof(IImportFromExcel);

            ImportComplexFile = new PopupWindowShowAction(this, "ImportComplexFile", PredefinedCategory.RecordEdit)
            {
                Caption = "Import Complex File",
                ImageName = "ExportToXLSX",
                PaintStyle = ActionItemPaintStyle.CaptionAndImage,
                ConfirmationMessage = null,
                ToolTip = null
            };
            ImportComplexFile.CustomizePopupWindowParams += ImportComplexFile_CustomizePopupWindowParams;
            ImportComplexFile.Execute += ImportComplexFile_Execute;
        }

        private void ImportComplexFile_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            os = (NonPersistentObjectSpace)e.Application.CreateObjectSpace(typeof(OpenFileEntry));
            os.PopulateAdditionalObjectSpaces(Application);
            e.View = e.Application.CreateDetailView(os, os.CreateObject<OpenFileEntry>());
        }

        private void ImportComplexFile_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                Validator.RuleSet.Validate(os, e.PopupWindowViewCurrentObject, "DialogOK");
                ImportFromComplexFile(e);
            }
            catch (ValidationException ex)
            {
                throw new UserFriendlyException(ex);
            }
        }

        public virtual void ImportFromComplexFile(PopupWindowShowActionExecuteEventArgs e)
        {
        }

        private void ActionImportFromExcel_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                Validator.RuleSet.Validate(os, e.PopupWindowViewCurrentObject, "DialogOK");
                ImportFromExcel(e);
            }
            catch (ValidationException ex)
            {
                //throw new UserFriendlyException(ex.Result.Results.First().ErrorMessage);
                throw new UserFriendlyException(ex);
            }
        }

        NonPersistentObjectSpace os;
        private void ActionImportFromExcel_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            os = (NonPersistentObjectSpace)e.Application.CreateObjectSpace(typeof(OpenFileEntry));
            os.PopulateAdditionalObjectSpaces(Application);
            e.View = e.Application.CreateDetailView(os, os.CreateObject<OpenFileEntry>());
        }

        public virtual ImportDefinition ChooseImportDefinition(IObjectSpace objectSpace, Type targetObjectType, PopupWindowShowActionExecuteEventArgs e)
        {
            return null;
        }

        public virtual void ImportFromExcel(PopupWindowShowActionExecuteEventArgs e)
        {
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelAllowExcelImport>();
        }

    }

    [DomainComponent]
    public class OpenFileEntry : NonPersistentObjectImpl
    {
        private FileDataEx file;
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [RuleRequiredField("ExcelFileIsRequired", DefaultContexts.Save, CustomMessageTemplate = "File should be assigned", TargetContextIDs ="DialogOK")]
        public FileDataEx File
        {
            get { return file; }
            set { SetPropertyValue<FileDataEx>(ref file, value); }
        }
    }
    public interface IModelAllowExcelImport
    {
        bool AllowExcelImport { get; set; }
    }
}
