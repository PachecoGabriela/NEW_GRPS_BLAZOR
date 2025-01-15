using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using FrameworkCore;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using FrameworkUtils.Converters;
using FrameworkUtils.Utils;

namespace ExcelImport.BusinessObjects
{
    [Appearance("ImportDefinitionMappingBase_CrossReference_Appearance", "PropertyType != 'System.String'", TargetItems = nameof(CrossReference), Enabled = false)]
    [Appearance("ImportDefinitionMappingBase_FindByField_Appearance", Method = nameof(FindByFieldAppearance), TargetItems = "FindByField;AdditionalCriteria", Enabled = false)]
    [DefaultProperty(nameof(DisplayName))]
    [ImageName("ExportToXLSX")]
    [NonPersistent]
    public class ImportDefinitionMappingBase : BaseObject, IHaveDeleteInlineButton
    {
        public ImportDefinitionMappingBase(Session session) : base(session)
        {
        }

        // [MemberDesignTimeVisibility(false)]
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string DisplayName
        {
            get { return GetDisplayName(); }
        }
        protected virtual string GetDisplayName()
        {
            return $"[{ColumnNumber}] {PropertyName}";
        }


        private int _orderNumber;
        [Index(1)]
        [ToolTip("Order Number (#)")]
        [VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [XafDisplayName("#")]
        public int OrderNumber
        {
            get { return _orderNumber; }
            set
            {
                if (SetPropertyValue<int>(nameof(OrderNumber), ref _orderNumber, value))
                    if (!this.IsLoading)
                        OrderNumberChanged();
            }
        }
        protected virtual void OrderNumberChanged()
        {

        }

        private int _columnNumber = 1;
        [Index(1)]
        [VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        [XafDisplayName("File Column Number")]
        public int ColumnNumber
        {
            get { return _columnNumber; }
            set { SetPropertyValue<int>(nameof(ColumnNumber), ref _columnNumber, value); }
        }

        private string _propertyName;
        [Index(2)]
        //[ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (SetPropertyValue<string>(nameof(PropertyName), ref _propertyName, value))
                    if (!IsLoading)
                        PropertyNameChanged();
            }
        }

        private void PropertyNameChanged()
        {
            ImportDefinition importDefinition = this.GetImportDefinition();
            if (importDefinition != null)
            {
                Type targetObjectType = importDefinition.TargetObjectType;
                if (targetObjectType != null)
                {
                    ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(targetObjectType);
                    IMemberInfo memberInfo = XpoHelper.GetNestedMember(typeInfo, this.PropertyName, false);

                    string memberCaption = string.Empty;
                    if (memberInfo != null)
                        memberCaption = DevExpress.ExpressApp.Utils.CaptionHelper.GetMemberCaption(memberInfo);

                    Type memberType = memberInfo?.MemberType;
                    this.PropertyType = memberType;

                    if (!string.IsNullOrEmpty(memberCaption))
                        this.PropertyCaption = memberCaption;
                    else
                        this.PropertyCaption = string.Empty;
                }
            }
        }


        string propertyClass;
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string PropertyClass
        {
            get => propertyClass;
            set => SetPropertyValue(nameof(PropertyClass), ref propertyClass, value);
        }

        private string _caption;
        [Index(3)]
        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string PropertyCaption
        {
            get { return _caption; }
            set { SetPropertyValue<string>(nameof(PropertyCaption), ref _caption, value); }
        }

        private Type _propertyType;
        //TODO: This is the actual needed value for lookup properties. I don't like the PropertyName hack, used because maybe ImportDefinition TargetObjectType prop is still not set when coming
        //to Column Mappings. So my idea is to clear the ColumnMappings every time the TargetObjectType is changed and hide ColumnMappings ListView until TargetObjectType is set.
        //To get the PropertyType collection source I'll manually populate it using the selected ImportDefinition TargetObjectType prop.
        //Note: Consider to hide this prop and show a collection of strings with the DisplayName and set PropertyType based in the selected DisplayName
        [Index(4)]
        [ModelDefault("AllowEdit", "False")]
        [ValueConverter(typeof(FrameworkUtils.Converters.TypeToStringConverter))]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public Type PropertyType
        {
            get { return _propertyType; }
            set { SetPropertyValue<Type>(nameof(PropertyType), ref _propertyType, value); }
        }

        private string _defaultValue;
        [ToolTip("Default excel cell value which is going to be used for empty cells.")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { SetPropertyValue<string>(nameof(DefaultValue), ref _defaultValue, value); }
        }

        private string _findByField;
        // TODO for multiple columns mapping:
        //[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        //[Size(SizeAttribute.Unlimited)]
        [EditorAlias(CoreEditorAliases.ObjectPropertyLookup), CriteriaOptions(nameof(PropertyType))]
        [ToolTip("For finding reference values. Which field should be looked up to find a record.")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string FindByField
        {
            get { return _findByField; }
            set { SetPropertyValue<string>(nameof(FindByField), ref _findByField, value); }
        }

        private string _additionalCriteria;
        [EditorAlias(CoreEditorAliases.ObjectPropertyLookup), CriteriaOptions(nameof(PropertyType))]
        [ToolTip("For finding reference values. Which field should be looked up to find a record.")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string AdditionalCriteria
        {
            get { return _additionalCriteria; }
            set { SetPropertyValue<string>(nameof(AdditionalCriteria), ref _additionalCriteria, value); }
        }

        public bool FindByFieldAppearance()
        {
            if (this.PropertyType != null && !this.PropertyType.IsEnum && !TypeHelper.PrimitiveTypes.Contains(this.PropertyType))
                return false;
            return true;
        }

        private FindReferenceMappingsMode _whenFindingLookupValue;

        [VisibleInListView(false), VisibleInLookupListView(false)]
        public FindReferenceMappingsMode WhenFindingLookupValue
        {
            get { return _whenFindingLookupValue; }
            set { SetPropertyValue<FindReferenceMappingsMode>(nameof(WhenFindingLookupValue), ref _whenFindingLookupValue, value); }
        }

        private ImportCrossReference _crossReference;
        [DataSourceProperty(nameof(CrossReferenceDataSource))]
        [ToolTip("Cross Reference lookup table. Available only for string mappings.")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        [Browsable(false)] //HACK: Disabled CrossReference
        public ImportCrossReference CrossReference
        {
            get { return _crossReference; }
            set
            {
                if (SetPropertyValue<ImportCrossReference>(nameof(CrossReference), ref _crossReference, value))
                    if (!this.IsLoading)
                        CrossReferenceChanged();
            }
        }
        private void CrossReferenceChanged()
        {
            if (this.CrossReference != null)
            {
                Type type = GetImportDefinition()?.TargetObjectType;

                if (string.IsNullOrEmpty(this.CrossReference.Name))
                    this.CrossReference.Name = $"[{CaptionHelper.GetClassCaption(type?.FullName)}].[{this.PropertyName}]";
                if (this.CrossReference.RecordType == null)
                    this.CrossReference.RecordType = type;
            }

        }

        private XPCollection<ImportCrossReference> _crossReferenceDataSource;
        [MemberDesignTimeVisibility(false)]
        public XPCollection<ImportCrossReference> CrossReferenceDataSource
        {
            get
            {
                if (_crossReferenceDataSource == null)
                {
                    CriteriaOperator criteria = null;
                    ImportDefinition importDefinition = GetImportDefinition();
                    if (importDefinition != null)
                        criteria = CriteriaOperator.Parse("RecordType = ?", importDefinition.TargetObjectType);
                    _crossReferenceDataSource = new XPCollection<ImportCrossReference>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session, criteria);
                }
                return _crossReferenceDataSource;
            }
        }

        // Validation

        [Browsable(false)]
        [RuleFromBoolProperty(CustomMessageTemplate = "Find By Field is required for reference type {PropertyType}.", UsedProperties = nameof(FindByField),
            SkipNullOrEmptyValues = false, ResultType = ValidationResultType.Error)]
        public bool ValidateFindByField
        {
            get
            {
                bool result = true;
                if (this.PropertyType != null && typeof(XPCustomObject).IsAssignableFrom(this.PropertyType))
                {
                    if (string.IsNullOrEmpty(this.FindByField))
                        result = false;
                }
                return result;
            }
        }

        private string _ignoredPrefix;
        [Size(50)]
        [ToolTip("Ignored Prefix. A prefix text that will be ignored during the import process.")]
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        [Browsable(false)] //HACK: Disabled IgnoredPrefix
        public string IgnoredPrefix
        {
            get { return _ignoredPrefix; }
            set { SetPropertyValue<string>(nameof(IgnoredPrefix), ref _ignoredPrefix, value); }
        }

        #region IHaveDeleteInlineButton members

        void IHaveDeleteInlineButton.DoDelete()
        {
            this.Delete();
        }

        #endregion

        // Functionality

        public virtual ImportDefinition GetImportDefinition()
        {
            return null;
        }
    }

    public enum FindReferenceMappingsMode
    {
        ErrorIfNotFound = 0,
        CreateIfNotFound = 1,
        LeaveBlankIfNotFound = 2,
    }
}
