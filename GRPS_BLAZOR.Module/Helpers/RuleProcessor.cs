using DevExpress.Charts.Native;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPS_BLAZOR.Module.Helpers
{
    public class RuleProcessor
    {
        private ComplianceRule _rule;
        private IObjectSpace _objectSpace;

        public RuleProcessor(ComplianceRule rule, IObjectSpace objectSpace)
        {
            _rule = rule;
            _objectSpace = objectSpace;
        }

        public IList<SalesVolume> Sales
        {
            get
            {
                if (!string.IsNullOrEmpty(_rule?.CriteriaRule?.Criteria))
                {
                    return _objectSpace.GetObjects<SalesVolume>(CriteriaOperator.Parse(_rule.CriteriaRule.Criteria));
                }
                else
                {
                    return new List<SalesVolume>();
                }

            }
        }

        public int SalesNum
        {
            get { return Sales.Count; }
        }

        public void ProcessRule()
        {
            foreach (SalesVolume sale in Sales)
            {
                sale.Rule = _rule;
                sale.BOM = sale.Product?.ActiveBOM;
            }
        }
    }
}
