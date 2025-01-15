using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using FrameworkCore;

namespace ExcelImport.BusinessObjects
{
    [NavigationItem("Import Tool")]
    [Appearance("ImportLog_DisableEditing", "1 = 1", TargetItems = "*", Enabled = false)]
    public class ImportLog : BaseObject
    {
        public ImportLog(Session session) : base(session) { }

        private LogLevel _mostSevereLogType;
        public LogLevel MostSevereLogType
        {
            get { return _mostSevereLogType; }
            set { SetPropertyValue<LogLevel>(nameof(MostSevereLogType), ref _mostSevereLogType, value); }
        }

        private DateTime _createdOn;
        [ModelDefault("DisplayFormat", "{0:g}"), ModelDefault("EditMask", "g")]
        public DateTime CreatedOn
        {
            get { return _createdOn; }
            set { SetPropertyValue<DateTime>(nameof(CreatedOn), ref _createdOn, value); }
        }

        private string _logDetails;
        [Size(SizeAttribute.Unlimited)]
        public string LogDetails
        {
            get { return _logDetails; }
            set { SetPropertyValue<string>(nameof(LogDetails), ref _logDetails, value); }
        }

        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string LogDetailsAsHTML
        {
            get { return this.LogDetails?.Replace("\r", "").Replace("\n", "<br/>"); }
        }

        private string _importedFilePath;
        [XafDisplayName("File Name")]
        [EditorAlias(CoreEditorAliases.FolderDialogPropertyEditor)]
        //[VisibleInListView(false)]
        [Size(254)]
        public string ImportedFilePath
        {
            get { return _importedFilePath; }
            set { SetPropertyValue<string>(nameof(ImportedFilePath), ref _importedFilePath, value); }
        }

        private int _modifiedObjects;
        public int ModifiedObjects
        {
            get { return _modifiedObjects; }
            set { SetPropertyValue<int>(nameof(ModifiedObjects), ref _modifiedObjects, value); }
        }

        private string _emailTo;
        [Browsable(false)] //HACK: Disabled EmailTo
        [VisibleInListView(false), VisibleInLookupListView(false)]
        public string EmailTo
        {
            get { return _emailTo; }
            set { SetPropertyValue<string>(nameof(EmailTo), ref _emailTo, value); }
        }

        private ImportDefinition _importDefinition;
        [VisibleInLookupListView(false)]
        public ImportDefinition ImportDefinition
        {
            get { return _importDefinition; }
            set { SetPropertyValue<ImportDefinition>(nameof(ImportDefinition), ref _importDefinition, value); }
        }

        public static ImportLog CreateImportLog(Session session, ImportObjectResult importObjectResult, string targetFileName = "")
        {
            return new ImportLog(session)
            {
                MostSevereLogType = importObjectResult.MostSevereLogType,
                CreatedOn = DateTime.Now,
                LogDetails = importObjectResult.GetLogMessage(),
                ImportedFilePath = targetFileName,
                ModifiedObjects = importObjectResult.ModifiedObjects.Count(),
                EmailTo = importObjectResult.EmailTo,
                ImportDefinition = importObjectResult.ImportDefinition
            };
        }
    }
}
