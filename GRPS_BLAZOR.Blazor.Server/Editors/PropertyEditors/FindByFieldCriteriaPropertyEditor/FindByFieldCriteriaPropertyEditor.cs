using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using GRPS_BLAZOR.Module.Helpers.Editors;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.FindByFieldCriteriaPropertyEditor
{
    [PropertyEditor(typeof(string), CustomEditorAliases.FindByFieldCriteriaPropertyEditor, false)]
    public class FindByFieldCriteriaPropertyEditor : BlazorPropertyEditorBase
    {
        public FindByFieldCriteriaPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override IComponentAdapter CreateComponentAdapter() => new FindByFieldCriteriaAdapter(new FindByFieldCriteriaModel());
    }
}
