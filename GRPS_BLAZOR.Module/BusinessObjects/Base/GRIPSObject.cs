using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Module.BusinessObjects.Base
{
    [NonPersistent]
    public abstract class GRIPSObject : CustomBaseClass
    {
        public GRIPSObject(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        string code;
        [NonCloneable]
        [VisibleInListView(true)]
        [RuleRequiredField(DefaultContexts.Save,
            CustomMessageTemplate = "Code is required")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Code
        {
            get => code;
            set => SetPropertyValue(nameof(Code), ref code, value);
        }
    }
}