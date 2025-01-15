using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using ExcelImport.BusinessObjects;
using ExcelImport.Controllers;
using ExcelImport.Extensions;

namespace ExcelImport.Blazor.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class BlazorImportFromExcelController : ImportFromExcelViewViewController
    {
        private ImportDefinition _importDefinition = null;
        private ComplexFileDefinition _complexFileDefinition = null;
        private FileDataEx _fileData = null;

        public override ImportDefinition ChooseImportDefinition(IObjectSpace objectSpace, Type targetObjectType, PopupWindowShowActionExecuteEventArgs e)
        {
            if (targetObjectType != null)
            {
                _fileData = ((OpenFileEntry)e.PopupWindowViewCurrentObject).File;

                IObjectSpace nestedObjectSpace = objectSpace.CreateNestedObjectSpace();
                Session session = (nestedObjectSpace as XPObjectSpace)?.Session;

                BinaryOperator targetObjectTypeBinaryOperator = new BinaryOperator(nameof(ImportDefinition.TargetObjectType), targetObjectType);

                // skip popup if nothing to choose
                List<ImportDefinition> importDefinitions = nestedObjectSpace.GetObjects<ImportDefinition>(targetObjectTypeBinaryOperator).ToList();
                if (importDefinitions.Count == 0)
                {
                    _importDefinition = new ImportDefinition(session) { TargetObjectType = targetObjectType };
                    nestedObjectSpace.CommitChanges();
                }
                else
                {
                    // show popup so the user can select an import definition
                    string viewId = this.Application.FindListViewId(typeof(ImportDefinition));
                    CollectionSourceBase collectionSource = this.Application.CreateCollectionSource(nestedObjectSpace, typeof(ImportDefinition), viewId);
                    collectionSource.Criteria.Add(nameof(ChooseImportDefinition), targetObjectTypeBinaryOperator);

                    View view = this.Application.CreateListView(viewId, collectionSource, true);
                    view.Caption = "Choose Import Definition you want to use...";


                    Application.ShowViewStrategy.ShowViewInPopupWindow(view, () => ChooseImportDefinition_Dialog_Accepting(view, new DialogControllerAcceptingEventArgs(null, null)));
                }
            }
            return _importDefinition;
        }

        private void ChooseImportDefinition_Dialog_Accepting(View view, DialogControllerAcceptingEventArgs dialogControllerAcceptingEventArgs)
        {
            if (view.SelectedObjects.Count != 1)
                throw new UserFriendlyException("A single Import Definition should be selected.");

            IObjectSpace objectSpace = Application.CreateObjectSpace();
            _importDefinition = objectSpace.GetObject(view.SelectedObjects[0]) as ImportDefinition;
            
            ShowImportDefinitionDetailView(objectSpace);
        }

        public override void ImportFromExcel(PopupWindowShowActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            // choose import definition if any available
            _importDefinition = ChooseImportDefinition(objectSpace, this.View.ObjectTypeInfo.Type, e);

            if (_fileData != null)
                ShowImportDefinitionDetailView(objectSpace);
        }

        private void ShowImportDefinitionDetailView(IObjectSpace objectSpace)
        {
            _importDefinition = objectSpace.GetObject(_importDefinition);
            if (_importDefinition != null)
            {
                _importDefinition.ExcelPreview.FileContent = _fileData.Content;
                _importDefinition.ExcelPreview.FileName = _fileData.FileName;

                View view = Application.CreateDetailView(objectSpace, _importDefinition);

                ShowViewParameters parameters = new();
                parameters.CreatedView = view;
                parameters.Context = TemplateContext.PopupWindow;
                parameters.CreateAllControllers = true;
                parameters.TargetWindow = TargetWindow.NewWindow;
                parameters.NewWindowTarget = NewWindowTarget.Separate;

                DialogController dialogController = Application.CreateController<DialogController>();
                dialogController.AcceptAction.Active[this.GetType().Name] = false;
                dialogController.CancelAction.Active[this.GetType().Name] = false;
                parameters.Controllers.Add(dialogController);

                Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(this.Frame, null));

                _importDefinition = null;
                _fileData = null;
            }
        }


        public override void ImportFromComplexFile(PopupWindowShowActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            _fileData = ((OpenFileEntry)e.PopupWindowViewCurrentObject).File;
            //TODO: We need to decide if there will be fixed templates or if we will allow the user to create their own templates. In this case this method will need to be modified to allow the user to select a custom definition.
            if (_fileData != null)
                ShowComplexFileDefinitionDetailView(objectSpace, View.ObjectTypeInfo.Type);
        }

        private void ShowComplexFileDefinitionDetailView(IObjectSpace objectSpace, Type targetObjectType)
        {
            _complexFileDefinition = objectSpace.CreateObject<ComplexFileDefinition>();
            _complexFileDefinition.TargetObjectType = targetObjectType;
            _complexFileDefinition.Name = targetObjectType.Name + " Import Definition";
            if (_complexFileDefinition != null)
            {
                _complexFileDefinition.ExcelPreview.FileContent = _fileData.Content;
                _complexFileDefinition.ExcelPreview.FileName = _fileData.FileName;

                View view = Application.CreateDetailView(objectSpace, _complexFileDefinition);

                ShowViewParameters parameters = new();
                parameters.CreatedView = view;
                parameters.Context = TemplateContext.PopupWindow;
                parameters.CreateAllControllers = true;
                parameters.TargetWindow = TargetWindow.NewWindow;
                parameters.NewWindowTarget = NewWindowTarget.Separate;

                DialogController dialogController = Application.CreateController<DialogController>();
                dialogController.AcceptAction.Active[this.GetType().Name] = false;
                dialogController.CancelAction.Active[this.GetType().Name] = false;
                parameters.Controllers.Add(dialogController);

                Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(this.Frame, null));

                _complexFileDefinition = null;
                _fileData = null;
            }
        }

    }
}

