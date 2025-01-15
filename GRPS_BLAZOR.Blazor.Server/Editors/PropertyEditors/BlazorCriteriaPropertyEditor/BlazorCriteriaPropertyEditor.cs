using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using System;
using System.Linq;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using GRPS_BLAZOR.Module.BusinessObjects.Criteria;
using GRPS_BLAZOR.Module.Helpers.Editors;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.BlazorCriteriaPropertyEditor
{
    [PropertyEditor(typeof(string), CustomEditorAliases.BlazorCriteriaPropertyEditor, false)]
    public class BlazorCriteriaPropertyEditor : BlazorPropertyEditorBase, IComplexViewItem
    {
        public BlazorCriteriaPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        private CriteriaPropertyEditorHelper helper;

        protected override IComponentAdapter CreateComponentAdapter()
        {
            Type objectType =  helper.GetCriteriaObjectType(CurrentObject);

            ((INotifyPropertyChanged)CurrentObject).PropertyChanged += BlazorCriteriaPropertyEditor_PropertyChanged;

            BlazorCriteriaModel model = new BlazorCriteriaModel();
            model.ObjectType = objectType;

            return new BlazorCriteriaAdapter(model);
        }

        private void BlazorCriteriaPropertyEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilteringCriterion.ObjectType))
            {
                BlazorCriteriaModel model = ((BlazorCriteriaAdapter)Control).ComponentModel;
                Type objectType = ((FilteringCriterion)CurrentObject).ObjectType;
                if (model.ObjectType != objectType)
                {
                    model.ObjectType = objectType;
                }
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
