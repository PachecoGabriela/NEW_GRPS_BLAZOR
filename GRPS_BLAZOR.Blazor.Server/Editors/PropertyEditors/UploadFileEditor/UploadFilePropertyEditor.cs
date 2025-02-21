using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using GRPS_BLAZOR.Blazor.Server.Middleware;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.UploadFileEditor
{
    [PropertyEditor(typeof(FileDataEmail), "FileData", false)]
    public class UploadFilePropertyEditor : BlazorPropertyEditorBase, IComplexViewItem
    {
        readonly Guid _guid = Guid.NewGuid();
        private readonly UploadFileAdapter _uploadFileAdapter;

        public UploadFilePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
            _uploadFileAdapter = new UploadFileAdapter(new UploadFileModel(_guid));
        }

        protected override IComponentAdapter CreateComponentAdapter() => _uploadFileAdapter;

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            UploadFileMiddleware.FormFile.Where(t => t.editor == _guid.ToString())
                .Buffer(_uploadFileAdapter.ComponentModel.Uploaded)
                .SelectMany(list => list)
                .Do(formFile =>
                {
                    var fileData = (FileDataEmail)objectSpace.CreateObject(typeof(FileDataEmail));
                    fileData.LoadFromStream(formFile.name, new MemoryStream(formFile.bytes));

                    ((EmailObject)CurrentObject).UploadFile = fileData;
                    objectSpace.CommitChanges();

                })
                .Subscribe();
        }
    }
}
