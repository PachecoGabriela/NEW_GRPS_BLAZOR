using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Components;

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.GroupNameWrapper
{
    public class GroupNameWrapperBase : ComponentBase
    {
        [Parameter] public View View { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public bool ChangeSize { get; set; }

        protected const string ProductsDashboardId = "Products_Dashboard";
        protected const string ProductDashboardInnerProductLvItem = "ProductListView_Custom_DashboardItem";

        public bool IsProductGroupVisible { get; set; }
        public bool IsSupplierGroupVisible { get; set; }
        public ListView ListView { get; set; }
        public bool FullSize { get; set; } = true;

        DashboardViewItem ProductLvDashboardViewItem;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Initialize();
        }

        private void Initialize()
        {
            IsProductGroupVisible = false;
            IsSupplierGroupVisible = false;
            if (View is DashboardView dashboardView)
            {
                if (dashboardView.Id == ProductsDashboardId)
                {
                    IsProductGroupVisible = true;
                    IsSupplierGroupVisible = true;
                    ProductLvDashboardViewItem = dashboardView.FindItem(ProductDashboardInnerProductLvItem) as DashboardViewItem;
                    ProductLvDashboardViewItem.ControlCreated += DashboardViewItem_ControlCreated;
                }
            }
        }

        private void DashboardViewItem_ControlCreated(object sender, EventArgs e)
        {
            ProductLvDashboardViewItem.ControlCreated -= DashboardViewItem_ControlCreated;
            ListView listView = ((DashboardViewItem)sender).InnerView as ListView;
            SetVisibilityAndListView(listView);
        }

        private void SetVisibilityAndListView(ListView listView)
        {
            if (listView is not null)
            {
                ListView = listView;
                StateHasChanged();
            }
            else
            {
                IsProductGroupVisible = false;
                IsSupplierGroupVisible = false;
            }
        }

        protected void ToggleSize()
        {
            FullSize = !FullSize;
        }
    }
}
