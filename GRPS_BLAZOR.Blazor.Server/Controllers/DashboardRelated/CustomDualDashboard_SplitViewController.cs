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
using DevExpress.Web;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SplitViewControllers
{
    public partial class CustomDualDashboard_SplitViewController : ViewController<DashboardView>
    {
        DashboardView dashboardView;
        public string DashboardViewId;
        private DashboardViewItem ListView1_DashboardItem;
        private DashboardViewItem ListView2_DashboardItem;
        private ListView UpperlistView;
        private ListView LowerlistView;
        IEnumerable<PackWeight> packWeights;



        public CustomDualDashboard_SplitViewController()
        {
            InitializeComponent();
            this.TargetViewId = "BillofMaterials_Dashboard;PartSpecifications_Dashboard";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            dashboardView = (DashboardView)View;
            DashboardViewId= dashboardView.Id;
            if(DashboardViewId == "BillofMaterials_Dashboard") 
            {
                ListView1_DashboardItem = (DashboardViewItem)dashboardView.FindItem("BOMItem_ListView_DashboardItem");
                ListView2_DashboardItem = (DashboardViewItem)dashboardView.FindItem("PackWeight_ListView_Custom_DashboardItem");
            }
            if (DashboardViewId == "PartSpecifications_Dashboard")
            {
                ListView1_DashboardItem = (DashboardViewItem)dashboardView.FindItem("Part_Custom_PartSpecifications_DashboardItem");
                ListView2_DashboardItem = (DashboardViewItem)dashboardView.FindItem("PackWeight_Custom_PartSpecifications_DashboardItem");
            }
            ListView1_DashboardItem.ControlCreated += ListView1_DashboardItem_ControlCreated;
            ListView2_DashboardItem.ControlCreated += ListView2_DashboardItem_ControlCreated;
            Frame.GetController<RefreshController>().Active[DashboardViewId] = false;
            Frame.GetController<DashboardCustomizationController>().Active[DashboardViewId] = false;

        }

        private void ListView1_DashboardItem_ControlCreated(Object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController listViewProcessCurrentObjectController = ListView1_DashboardItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcessCurrentObjectController.CustomProcessSelectedItem += ListViewProcessCurrentObjectController_CustomProcessSelectedItem; // Process current Object
            UpperlistView = (ListView)ListView1_DashboardItem.InnerView;
        }

        private void ListView2_DashboardItem_ControlCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController processObject = ListView2_DashboardItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            processObject.CustomProcessSelectedItem += ProcessObject_CustomProcessSelectedItem;
            LowerlistView = (ListView)ListView2_DashboardItem.InnerView;
            LowerlistView.CollectionSource.Criteria["FullTextSearchCriteria"] = CriteriaOperator.Parse("StartsWith([Part.Code], 'A233241dsaasfdascsa')");
        }

        private void ProcessObject_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
        }

        private void ListViewProcessCurrentObjectController_CustomProcessSelectedItem(Object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            if (DashboardViewId == "BillofMaterials_Dashboard")
            {
                if (e.InnerArgs.CurrentObject != null)
                {
                    e.Handled = true;
                    BOMItem selectedBOMItem = e.InnerArgs.CurrentObject as BOMItem;
                    Part part = selectedBOMItem?.Part;

                    if (part is not null)
                    {
                        RemoveFromPackWeights();
                        packWeights = LowerlistView.ObjectSpace.GetObjects<PackWeight>(
                        CriteriaOperator.FromLambda<PackWeight>(pw => pw.Part.Oid == part.Oid)
                        );
                        foreach (var item in packWeights)
                        {
                            LowerlistView.CollectionSource.Add(item);
                        }
                    }
                };
            }

            if (DashboardViewId == "PartSpecifications_Dashboard")
            {
                if (e.InnerArgs.CurrentObject != null)
                {
                    e.Handled = true;
                    Part selectedPart = e.InnerArgs.CurrentObject as Part;

                    if (selectedPart is not null)
                    {
                        RemoveFromPackWeights();
                        packWeights = LowerlistView.ObjectSpace.GetObjects<PackWeight>(
                        CriteriaOperator.FromLambda<PackWeight>(pw => pw.Part.Oid == selectedPart.Oid)
                        );
                        foreach (var item in packWeights)
                        {
                            LowerlistView.CollectionSource.Add(item);
                        }
                    }
                };
            }


        }

        private void RemoveFromPackWeights()
        {
            if (packWeights is not null)
            {
                List<PackWeight> itemsToRemove = packWeights.ToList();
                foreach (PackWeight item in itemsToRemove)
                {
                    LowerlistView.CollectionSource.Remove(item);
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            Frame.GetController<RefreshController>().Active.RemoveItem(DashboardViewId);
            Frame.GetController<DashboardCustomizationController>().Active.RemoveItem(DashboardViewId);
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
