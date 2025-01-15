using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using FrameworkUtils.Utils;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPS_BLAZOR.Module.Helpers
{
    public class TotalPackagingProcessor
    {
        private ComplianceRule _rule;
        private IObjectSpace _objectSpace;
        private XafApplication _xafApplication;

        private UnitOfWork _uow;

        private IList<SalesVolume> _sales;

        public TotalPackagingProcessor(ComplianceRule rule, IObjectSpace objectSpace, XafApplication xafApplication)
        {
            _rule = rule;
            _objectSpace = objectSpace;
            _xafApplication = xafApplication;

            _sales = _objectSpace.GetObjects<SalesVolume>(CriteriaOperator.FromLambda<SalesVolume>(sale => sale.Rule.Oid == _rule.Oid));
        }

        public TotalPackagingProcessor(ComplianceRule rule, IObjectSpace objectSpace, UnitOfWork uow)
        {
            _rule = rule;
            _objectSpace = objectSpace;
            _uow = uow;

            _sales = _objectSpace.GetObjects<SalesVolume>(CriteriaOperator.FromLambda<SalesVolume>(sale => sale.Rule.Oid == _rule.Oid));
        }

        public IList<SalesVolume> Sales
        {
            private set { _sales = value; }
            get { return _sales; }
        }

        public IList<vwSalesPackWeight> VwSalesPackWeights => _objectSpace.GetObjects<vwSalesPackWeight>(CriteriaOperator.FromLambda<vwSalesPackWeight>(viewSalePack => viewSalePack.comprule.Oid == _rule.Oid));

        //public IList<vwSalesPackWeight> VwSalesPackWeights => _uow.GetObjects<vwSalesPackWeight>(CriteriaOperator.FromLambda<vwSalesPackWeight>(viewSalePack => viewSalePack.comprule.Oid == _rule.Oid));

        /// <summary>
        /// Removes from storage the TotalPackWeight records that matches the <see cref="ComplianceRule"/> of this instance
        /// </summary>
        /// <remarks>Executes a commit</remarks>
        public void RemoveOldTotalPackWeightInfo()
        {
            //We can't use _objectSpace field because the remove operation executes a commit, this will cause a commit of the previous transactions (using this ObjectSpace). In order to avoid
            //the later behaviour another ObjectSpace is created (and committed) for the remove operation
            //IObjectSpace objectSpace = _xafApplication.CreateObjectSpace();
            //IList<TotalPackWeight> totalPackWeights = objectSpace.GetObjects<TotalPackWeight>(CriteriaOperator.FromLambda<TotalPackWeight>(pack => pack.CompRule.Oid == _rule.Oid));
            //objectSpace.Delete(totalPackWeights);

            //if (objectSpace.IsModified)
            //    objectSpace.CommitChanges();

            //IObjectSpace objectSpace = _xafApplication.CreateObjectSpace();
            IList<TotalPackWeight> totalPackWeights = _objectSpace.GetObjects<TotalPackWeight>(CriteriaOperator.FromLambda<TotalPackWeight>(pack => pack.CompRule.Oid == _rule.Oid));
            //_objectSpace.Delete(totalPackWeights);
            //Commit();

            foreach (var item in totalPackWeights)
            {
                var i = _uow.GetObjectByKey<TotalPackWeight>(item.Oid);
                _uow.Delete(i);
            }

        }

        /// <summary>
        /// Create the TotalPackWeight objets for the <see cref="ComplianceRule"/> of this instance
        /// </summary>
        public void CalculateTotalPackWeight()
        {
            var result = VwSalesPackWeights
                .GroupBy(viewSalePack => (viewSalePack.packtype, viewSalePack.Source, viewSalePack.destination, viewSalePack.Material, viewSalePack.MaterialCategory, viewSalePack.comprule, viewSalePack.srattrname, viewSalePack.srreporttype))
                .Select(g => (g.Key.packtype, g.Key.Source, g.Key.destination, g.Key.Material, g.Key.MaterialCategory, g.Key.comprule, g.Key.srattrname, g.Key.srreporttype, tonnesSum: g.Sum(viewSalePack => viewSalePack.tonnes)));

            decimal totalSales = Sales.Sum(sale => sale.Volume);
            decimal totalSalesWithSpec = Sales.Where(sale => sale.BOM != null).Sum(sale => sale.Volume);
            decimal uplift = GetUplift(totalSalesWithSpec, totalSales);

            foreach (var item in result)
            {
                //TotalPackWeight totalPackWeight = _objectSpace.CreateObject<TotalPackWeight>();
                TotalPackWeight totalPackWeight = new TotalPackWeight(_uow);
                //totalPackWeight.PackType = item.packtype;
                //totalPackWeight.Source = item.Source;
                //totalPackWeight.Destination = item.destination;
                //totalPackWeight.MaterialCategory = item.MaterialCategory;
                //totalPackWeight.MaterialType = item.Material;
                //totalPackWeight.CompRule = _rule;
                //totalPackWeight.SRAttrName = item.srattrname;
                //totalPackWeight.SRReportType = item.srreporttype;
                //totalPackWeight.Tonnes = Convert.ToDouble(item.tonnesSum);

                totalPackWeight.PackType = GetEnumInstance(item.packtype);
                totalPackWeight.Source = GetEnumInstance(item.Source);
                totalPackWeight.Destination = GetEnumInstance(item.destination);
                totalPackWeight.MaterialCategory = GetEnumInstance(item.MaterialCategory);
                totalPackWeight.MaterialType = GetEnumInstance(item.Material);
                totalPackWeight.CompRule = _uow.GetObjectByKey<ComplianceRule>(_rule.Oid);
                totalPackWeight.SRAttrName = GetEnumInstance(item.srattrname);
                totalPackWeight.SRReportType = GetEnumInstance(item.srreporttype);
                totalPackWeight.Tonnes = Convert.ToDouble(item.tonnesSum);

                totalPackWeight.TotalSales = Convert.ToDouble(totalSales);
                totalPackWeight.WithSpec = Convert.ToDouble(uplift);
            }
            //Commit();
        }

        private decimal GetUplift(decimal totalSalesWithSpec, decimal totalSales)
        {
            return totalSales != 0 ? totalSalesWithSpec / totalSales : 0;
        }

        private EnumInstance GetEnumInstance(EnumInstance enumInstance)
        {
            if (enumInstance != null)
            {
                return _uow.GetObjectByKey<EnumInstance>(enumInstance.Oid);
            }
            return null;
        }

        private void Commit()
        {
            if (_objectSpace.IsModified)
                _objectSpace.CommitChanges();
        }
    }
}
