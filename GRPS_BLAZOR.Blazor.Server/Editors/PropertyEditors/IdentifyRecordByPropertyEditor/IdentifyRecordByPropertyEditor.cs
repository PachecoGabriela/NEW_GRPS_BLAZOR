using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using GRPS_BLAZOR.Module.Helpers.Editors;
using System.ComponentModel;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.IdentifyRecordByPropertyEditor
{
    [PropertyEditor(typeof(string), CustomEditorAliases.IdentifyRecordByPropertyEditor, false)]
    public class IdentifyRecordByPropertyEditor : BlazorPropertyEditorBase, IComplexViewItem
    {
        private CriteriaPropertyEditorHelper helper;

        public IdentifyRecordByPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override IComponentAdapter CreateComponentAdapter()
        {
            ((INotifyPropertyChanged)CurrentObject).PropertyChanged += IdentifyRecordByPropertyEditor_PropertyChanged;

            Type objectType = helper.GetCriteriaObjectType(CurrentObject);
            IdentifyRecordByModel model = new IdentifyRecordByModel();
            model.ObjectType = objectType;

            return new IdentifyRecordByAdapter(model);
        }

        private void IdentifyRecordByPropertyEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IdentifyRecordByModel model = ((IdentifyRecordByAdapter)Control).ComponentModel;
            Type objectType = helper.GetCriteriaObjectType(CurrentObject);
            if (model.ObjectType != objectType)
            {
                model.ObjectType = objectType;
            }
        }

        #region IComplexViewItem
        private IObjectSpace objectSpace;
        private XafApplication application;

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            helper = new CriteriaPropertyEditorHelper(MemberInfo);
            this.objectSpace = objectSpace;
            this.application = application;
        }
        #endregion
    }
}
