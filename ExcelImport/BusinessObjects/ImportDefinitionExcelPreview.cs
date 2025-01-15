using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using ExcelImport.Extensions;

namespace ExcelImport.BusinessObjects
{
    /// <summary>
    /// Wrapper class for displaying excel preview with ColumnMappingControl custom control.
    /// </summary>
    [NonPersistent]
    public class ImportDefinitionExcelPreview : BaseObject
    {
        public ImportDefinitionExcelPreview(Session session) : base(session) { }

        public ImportDefinition Definition { get; set; }

        public ComplexFileDefinition ComplexFileDefinition { get; set; }

        private string _fileName;
        [ModelDefault("AllowEdit", "False")]
        public string FileName
        {
            get { return _fileName; }
            set { SetPropertyValue<string>(nameof(FileName), ref _fileName, value); }
        }

        private byte[] _fileContent;
        public byte[] FileContent
        {
            get { return _fileContent; }
            set { SetPropertyValue<byte[]>(nameof(FileContent), ref _fileContent, value); }
        }


    }
}
