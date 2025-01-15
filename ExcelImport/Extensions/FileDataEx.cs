using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExcelImport.Extensions
{
    public class FileDataEx : FileData, ISupportFullName
    {
        public FileDataEx(Session session) : base(session) { }

        private string _fullName;

        [ModelDefault("AllowEdit", "False")]
        public string FullName
        {
            get { return _fullName; }
            set { SetPropertyValue("FullName", ref _fullName, value); }
        }
    }
}
