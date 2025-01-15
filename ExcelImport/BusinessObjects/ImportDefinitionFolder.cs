using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using FrameworkCore;

namespace ExcelImport.BusinessObjects
{
    public class ImportDefinitionFolder : BaseObject
    {
        public ImportDefinitionFolder(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            this.IsActive = true;
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetPropertyValue<string>(nameof(Name), ref _name, value); }
        }

        private string _folderPath;
        [EditorAlias(CoreEditorAliases.FolderDialogPropertyEditor)]
        [Size(254)]
        public string FolderPath
        {
            get { return _folderPath; }
            set { SetPropertyValue<string>(nameof(FolderPath), ref _folderPath, value); }
        }

        private string _errorFolderPath;
        [EditorAlias(CoreEditorAliases.FolderDialogPropertyEditor)]
        [Size(254)]
        [VisibleInListView(false), VisibleInLookupListView(false)]
        public string ErrorFolderPath
        {
            get { return _errorFolderPath; }
            set { SetPropertyValue<string>(nameof(ErrorFolderPath), ref _errorFolderPath, value); }
        }

        private string _processedFolderPath;
        [EditorAlias(CoreEditorAliases.FolderDialogPropertyEditor)]
        [Size(254)]
        [VisibleInListView(false), VisibleInLookupListView(false)]
        public string ProcessedFolderPath
        {
            get { return _processedFolderPath; }
            set { SetPropertyValue<string>(nameof(ProcessedFolderPath), ref _processedFolderPath, value); }
        }

        private string _lookupCode;
        [Size(50)]
        [VisibleInListView(false), VisibleInLookupListView(false)]
        public string LookupCode
        {
            get { return _lookupCode; }
            set { SetPropertyValue<string>(nameof(LookupCode), ref _lookupCode, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetPropertyValue<bool>(nameof(IsActive), ref _isActive, value); }
        }

        private ImportDefinition importDefinition;
        public ImportDefinition ImportDefinition
        {
            get { return importDefinition; }
            set { SetPropertyValue<ImportDefinition>(nameof(ImportDefinition), ref importDefinition, value); }
        }
    }
}
