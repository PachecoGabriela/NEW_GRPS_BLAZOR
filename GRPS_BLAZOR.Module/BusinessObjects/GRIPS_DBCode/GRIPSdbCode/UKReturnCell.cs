using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{

    public partial class UKReturnCell
    {
        public UKReturnCell(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
