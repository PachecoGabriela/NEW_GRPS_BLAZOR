using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using Microsoft.AspNetCore.Components;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.UploadFileEditor
{
    public class LabelPropertyEditor : BlazorPropertyEditorBase
    {
        public LabelPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }

        protected override IComponentAdapter CreateComponentAdapter()
        {
            return new HtmlContentAdapter(PropertyValue as string);
        }
    }

    public class HtmlContentAdapter : ComponentAdapterBase
    {
        private string? htmlContent;

        public HtmlContentAdapter(string? initialContent)
        {
            htmlContent = initialContent;
        }

        public override object GetValue() => htmlContent ?? string.Empty;
        public override void SetValue(object value) => htmlContent = value as string;

        public override void SetAllowEdit(bool allowEdit) { }
        public override void SetAllowNull(bool allowNull) { }
        public override void SetDisplayFormat(string displayFormat) { }
        public override void SetEditMask(string editMask) { }
        public override void SetEditMaskType(EditMaskType editMaskType) { }
        public override void SetErrorIcon(ImageInfo errorIcon) { }
        public override void SetErrorMessage(string errorMessage) { }
        public override void SetIsPassword(bool isPassword) { }
        public override void SetMaxLength(int maxLength) { }
        public override void SetNullText(string nullText) { }

        public override IComponentModel ComponentModel => new HtmlContentModel(htmlContent);

        protected override RenderFragment CreateComponent()
        {
            return builder => {
                builder.OpenElement(0, "div");
                builder.AddMarkupContent(1, htmlContent ?? string.Empty);
                builder.CloseElement();
            };
        }
    }

    public class HtmlContentModel : ComponentModelBase {
        public string? HtmlContent { get; set; }

        public HtmlContentModel(string? initialContent)
        {
            HtmlContent = initialContent;
        }
    }
}
