using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.ProductDashboardControllers
{
    public partial class ProductDashboard_SplitviewController : ViewController
    {
        private DashboardView dashboardView;
        public string dashboardViewID;
        private DashboardViewItem ListView1_DashboardItem;
        private DashboardViewItem ListView2_DashboardItem;
        private DashboardViewItem ListView3_DashboardItem;
        private DashboardViewItem ListView4_DashboardItem;
        private ListView ProductListView;
        private ListView BOMItemListView;
        private ListView PackWeightListView;
        private ListView PackWeightSecondListView;
        IEnumerable<BOMItem> bOMItems;
        IEnumerable<PackWeight> packWeights;
        
        
        public ProductDashboard_SplitviewController()
        {
            InitializeComponent();
            TargetViewId = "Products_Dashboard";
        }


        protected override void OnActivated()
        {
            base.OnActivated();
            dashboardView = (DashboardView)View;
            dashboardViewID = dashboardView.Id;
            if (dashboardViewID == "Products_Dashboard")
            {
                ListView1_DashboardItem = (DashboardViewItem)dashboardView.FindItem("ProductListView_Custom_DashboardItem");
                ListView2_DashboardItem = (DashboardViewItem)dashboardView.FindItem("BOMItem_Custom_ProductsDashboard");
                ListView3_DashboardItem = (DashboardViewItem)dashboardView.FindItem("PackWeight_Custm_ProductsDashboard");
                ListView4_DashboardItem = (DashboardViewItem)dashboardView.FindItem("PackWeight_Second_ProductDashboard");
            }
            ListView1_DashboardItem.ControlCreated += ListView1_DashboardItem_ControlCreated;
            ListView2_DashboardItem.ControlCreated += ListView2_DashboardItem_ControlCreated;
            ListView3_DashboardItem.ControlCreated += ListView3_DashboardItem_ControlCreated;
            ListView4_DashboardItem.ControlCreated += ListView4_DashboardItem_ControlCreated;
            Frame.GetController<RefreshController>().Active[dashboardViewID] = false;
            Frame.GetController<DashboardCustomizationController>().Active[dashboardViewID] = false;
        }


        private void ListView1_DashboardItem_ControlCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController listView1ProcessCurrentObjectController = ListView1_DashboardItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            listView1ProcessCurrentObjectController.CustomProcessSelectedItem += ListView1ProcessCurrentObjectController_CustomProcessSelectedItem; // Process current Object
            ProductListView = (ListView)ListView1_DashboardItem.InnerView;
        }

        private void ListView1ProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            if (e.InnerArgs.CurrentObject != null)
            {
                e.Handled = true;
                Product selectedProduct = e.InnerArgs.CurrentObject as Product;

                if (selectedProduct is not null)
                {
                    RemoveFromBOMItems();
                    if (selectedProduct.ActiveBOM is not null)
                    {
                        bOMItems = BOMItemListView.ObjectSpace.GetObjects<BOMItem>(
                        CriteriaOperator.FromLambda<BOMItem>(bi => bi.BOM.Oid == selectedProduct.ActiveBOM.Oid)
                        );
                    }
                    if (bOMItems is not null)
                    {
                        foreach (var item in bOMItems)
                        {
                            BOMItemListView.CollectionSource.Add(item);
                        }
                    }

                }
            };
        }

        private void ListView2_DashboardItem_ControlCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController listView2ProcessCurrentObjectController = ListView2_DashboardItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            listView2ProcessCurrentObjectController.CustomProcessSelectedItem += ListView2ProcessCurrentObjectController_CustomProcessSelectedItem; // Process current Object
            BOMItemListView = (ListView)ListView2_DashboardItem.InnerView;
            BOMItemListView.CollectionSource.Criteria["FullTextSearchCriteria"] = CriteriaOperator.Parse("StartsWith([BOM.Product.Code], 'A233241dsaasfdascsa')");
        }

        private void ListView2ProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            if (e.InnerArgs.CurrentObject != null)
            {
                e.Handled = true;
                BOMItem selectedBOMItem = e.InnerArgs.CurrentObject as BOMItem;
                Part part = selectedBOMItem?.Part;

                if (part is not null)
                {
                    RemoveFromPackWeight();
                    packWeights = PackWeightListView.ObjectSpace.GetObjects<PackWeight>(
                        CriteriaOperator.FromLambda<PackWeight>(pw => pw.Part.Oid == part.Oid));
                    
                    if (packWeights is not null)
                    {
                        foreach (var item in packWeights)
                        {
                            PackWeightListView.CollectionSource.Add(item);
                        }
                    }
                    packWeights = PackWeightSecondListView.ObjectSpace.GetObjects<PackWeight>(
                         CriteriaOperator.FromLambda<PackWeight>(pw => pw.Part.Oid == part.Oid));

                    if (packWeights is not null)
                    {
                        foreach (var item in packWeights)
                        {
                            PackWeightSecondListView.CollectionSource.Add(item);
                        }
                    }
                }
            };
        }


        private void RemoveFromBOMItems()
        {
            if (bOMItems is not null)
            {
                List<BOMItem> itemsToRemove = bOMItems.ToList();
                foreach (BOMItem item in itemsToRemove)
                {
                    BOMItemListView.CollectionSource.Remove(item);
                }
                bOMItems = null;
            }
        }

        private void RemoveFromPackWeight()
        {
            if (packWeights is not null)
            {
                List<PackWeight> itemsToRemove = packWeights.ToList();
                foreach (PackWeight item in itemsToRemove)
                {
                    PackWeightListView.CollectionSource.Remove(item);
                    PackWeightSecondListView.CollectionSource.Remove(item);
                }
                packWeights= null;
            }
        }



        private void ListView3_DashboardItem_ControlCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController processListView3_Object = ListView3_DashboardItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            processListView3_Object.CustomProcessSelectedItem += ProcessObject_CustomProcessSelectedItem;
            PackWeightListView = (ListView)ListView3_DashboardItem.InnerView;
            PackWeightListView.CollectionSource.Criteria["FullTextSearchCriteria"] = CriteriaOperator.Parse("StartsWith([Part.Code], 'A233241dsaasfdascsa')");
        }


        private void ListView4_DashboardItem_ControlCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController processListView4_Object = ListView4_DashboardItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            processListView4_Object.CustomProcessSelectedItem += ProcessObject_CustomProcessSelectedItem;
            PackWeightSecondListView = (ListView)ListView4_DashboardItem.InnerView;
            PackWeightSecondListView.CollectionSource.Criteria["FullTextSearchCriteria"] = CriteriaOperator.Parse("StartsWith([Part.Code], 'A233241dsaasfdascsa')");
        }

        private void ProcessObject_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }


        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Frame.GetController<RefreshController>().Active.RemoveItem(dashboardViewID);
            Frame.GetController<DashboardCustomizationController>().Active.RemoveItem(dashboardViewID);
            if (ListView1_DashboardItem != null)
            {
                ListView1_DashboardItem.ControlCreated -= ListView1_DashboardItem_ControlCreated;
            }
            if (ListView2_DashboardItem != null)
            {
                ListView2_DashboardItem.ControlCreated -= ListView2_DashboardItem_ControlCreated;
            }
        }
    }
}
