using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExcelImport.BusinessObjects
{
    [CreatableItem(false)]
    //HACK: Disabled ImportCrossReference 
    //[DefaultClassOptions]
    [ImageName("Replace")]
    [ModelDefault("IsCloneable", "true")]
    [OptimisticLocking(false)]
    public class ImportCrossReference : BaseObject
    {
        public ImportCrossReference(Session session) : base(session) { }

        private string _name;
        [Size(100)]
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue<string>(nameof(Name), ref _name, value); }
        }

        private Type _recordType;
        [ValueConverter(typeof(TypeToStringConverter))]
        public Type RecordType
        {
            get { return _recordType; }
            set
            {
                if (SetPropertyValue<Type>(nameof(RecordType), ref _recordType, value))
                    if (!this.IsLoading)
                        RecordTypeChanged();
            }
        }
        private void RecordTypeChanged()
        {
            if (string.IsNullOrEmpty(this.Name) && this.RecordType != null)
                this.Name = $"{CaptionHelper.GetClassCaption(this.RecordType.FullName)} - {DateTime.Now:yyyy-MM-dd}";
        }

        [Association, Aggregated]
        public XPCollection<ImportCrossReferenceLine> Lines
        {
            get { return GetCollection<ImportCrossReferenceLine>(nameof(Lines)); }
        }
    }
}
