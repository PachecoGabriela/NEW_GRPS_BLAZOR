using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SplitViewControllers
{
    public partial class ListViewCSSClassController : ViewController<ListView>
    {
        public ListViewCSSClassController()
        {
            InitializeComponent();
            TargetViewId = "BOMItem_ListView_Custom;PackWeight_ListView_Custom;Part_ListView_Custom;PackWeight_ListView_PartDashboard;Product_ListView_Custom;PackWeight_ListView_ProductDashboard_Second;BOMItem_ListView_Custom_ProductDashboard;PackWeight_ListView_Custom_ProductDashboard";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View.Editor is DxGridListEditor gridListEditor)
            {
                if (View.Id == "BOMItem_ListView_Custom" || View.Id == "Part_ListView_Custom")
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    dataGridAdapter.GridModel.CssClass = "ListViewScrolling";
                }
                if (View.Id == "PackWeight_ListView_Custom" || View.Id == "PackWeight_ListView_PartDashboard")
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    dataGridAdapter.GridModel.CssClass = "ListViewShowing";
                }
                if (View.Id == "Product_ListView_Custom")
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    dataGridAdapter.GridModel.CssClass = "ProductScrolling";
                }
                if (View.Id == "PackWeight_ListView_ProductDashboard_Second")
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    dataGridAdapter.GridModel.CssClass = "PackWeightSecond_Scrolling";
                }
                if (View.Id == "BOMItem_ListView_Custom_ProductDashboard")
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    dataGridAdapter.GridModel.CssClass = "BOMItemProductDashboard_Scrolling";
                }
                if (View.Id == "PackWeight_ListView_Custom_ProductDashboard")
                {
                    IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                    dataGridAdapter.GridModel.CssClass = "PackWeightProductDashboard_Scrolling";
                }


            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
