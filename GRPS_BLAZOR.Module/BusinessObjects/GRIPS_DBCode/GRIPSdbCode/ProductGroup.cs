using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Base;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{
    //[NavigationItem("Grips Desktop")]
    public partial class ProductGroup
    {
        public ProductGroup(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
