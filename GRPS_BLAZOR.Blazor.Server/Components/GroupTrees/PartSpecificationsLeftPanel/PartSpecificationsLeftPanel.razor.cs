using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Components;

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.PartSpecificationsLeftPanel
{
    public class PartSpecificationsLeftPanelBase : ComponentBase
    {
        [Parameter] public View View { get; set; }

        protected const string PartDashboardId = "PartSpecifications_Dashboard";
        protected const string PartDashboardInnerPartLvItem = "Part_Custom_PartSpecifications_DashboardItem";

        public bool FullSize { get; set; } = true;
        public bool IsPartLeftPanelVisible { get; set; }
        public ListView ListView { get; set; }

        DashboardViewItem PartLvDashboardViewItem;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Initialize();
        }

        private void Initialize()
        {
            IsPartLeftPanelVisible = false;
            if (View is DashboardView dashboardView)
            {
                if (dashboardView.Id == PartDashboardId)
                {
                    IsPartLeftPanelVisible = true;
                    PartLvDashboardViewItem = dashboardView.FindItem(PartDashboardInnerPartLvItem) as DashboardViewItem;
                    PartLvDashboardViewItem.ControlCreated += PartLvDashboardViewItem_ControlCreated;
                }
            }
        }

        private void PartLvDashboardViewItem_ControlCreated(object sender, EventArgs e)
        {
            PartLvDashboardViewItem.ControlCreated -= PartLvDashboardViewItem_ControlCreated;
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
                IsPartLeftPanelVisible = false;
            }
        }

        protected void ToggleSize()
        {
            FullSize = !FullSize;
        }
    }
}
