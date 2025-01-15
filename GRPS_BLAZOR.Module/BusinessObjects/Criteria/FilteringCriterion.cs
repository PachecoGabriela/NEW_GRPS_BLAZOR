using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.Helpers.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPS_BLAZOR.Module.BusinessObjects.Criteria
{
    public class MyLocalizedClassInfoTypeConverter : LocalizedClassInfoTypeConverter
    {

        public MyLocalizedClassInfoTypeConverter()
        {

        }

        public override void AddCustomItems(List<Type> list)
        {
            base.AddCustomItems(list);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val)
        {
            return base.ConvertFrom(context, culture, val);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType)
        {
            return base.ConvertTo(context, culture, val, destType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return base.CreateInstance(context, propertyValues);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return base.GetCreateInstanceSupported(context);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return base.GetProperties(context, value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return base.GetPropertiesSupported(context);
        }

        public override List<Type> GetSourceCollection(ITypeDescriptorContext context)
        {
            List<Type> types = base.GetSourceCollection(context);
            return types;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesExclusive(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesSupported(context);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }

        protected override string GetClassCaption(string fullName)
        {
            return base.GetClassCaption(fullName);
        }

        protected override List<Type> GetTypesFromTypesInfo(ITypesInfo typesInfo)
        {
            List<Type> types = base.GetTypesFromTypesInfo(typesInfo);
            return types;
        }

        protected override bool IsValueAllowed(ITypeDescriptorContext context, object value)
        {
            return base.IsValueAllowed(context, value);
        }
    }

    [ModelDefault("IsCloneable", "true")]
    [DefaultClassOptions, ImageName("Action_Filter")]
    public class FilteringCriterion : BaseObject
    {
        public FilteringCriterion(Session session) : base(session) { }

        [NonCloneable]
        public string Description
        {
            get { return GetPropertyValue<string>(nameof(Description)); }
            set { SetPropertyValue(nameof(Description), value); }
        }
        [ValueConverter(typeof(TypeToStringConverter)), ImmediatePostData]
        [TypeConverter(typeof(MyLocalizedClassInfoTypeConverter))]
        public Type ObjectType
        {
            get { return GetPropertyValue<Type>(nameof(ObjectType)); }
            set
            {
                SetPropertyValue(nameof(ObjectType), value);
                CriterionPlusCriteriaRuleJson = string.Empty;
            }
        }

        private string criterion;
        [Browsable(false)]
        //[CriteriaOptions("ObjectType")]
        [Size(SizeAttribute.Unlimited)]
        public string Criteria
        {
            set => SetPropertyValue(nameof(Criteria), ref criterion, value);
            get => criterion;
        }

        string criterionPlusCriteriaRuleJson;
        [CriteriaOptions("ObjectType")]
        [System.ComponentModel.DisplayName(nameof(Criteria))]
        [EditorAlias(CustomEditorAliases.BlazorCriteriaPropertyEditor)]
        [Size(SizeAttribute.Unlimited)]
        public string CriterionPlusCriteriaRuleJson
        {
            get => criterionPlusCriteriaRuleJson;
            set
            {
                if (SetPropertyValue(nameof(CriterionPlusCriteriaRuleJson), ref criterionPlusCriteriaRuleJson, value)
                    && !IsLoading)
                {
                    UpdateCriterion();
                }
            }
        }

        private void UpdateCriterion()
        {
            string separator = "|||";
            Criteria = CriterionPlusCriteriaRuleJson.Split(separator).First();
        }
    }
}
