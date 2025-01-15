using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GRPS_BLAZOR.Module.BusinessObjects.Criteria;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{

    public partial class ComplianceRule
    {
        public ComplianceRule(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }

        FilteringCriterion criteriaRule;
        public FilteringCriterion CriteriaRule
        {
            get => criteriaRule;
            set => SetPropertyValue(nameof(CriteriaRule), ref criteriaRule, value);
        }
    }

}
