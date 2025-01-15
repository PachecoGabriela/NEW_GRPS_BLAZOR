using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using GRPS_BLAZOR.Module.Interfaces;
using Microsoft.AspNetCore.Components;
using TestSideTrees.Blazor.Server.POCOs;
using AutoMapper;

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.PartGroupTree
{
    public class PartGroupTreeBase : ComponentBase
    {
        [Inject]
        IXafApplicationProvider ApplicationProvider { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        [CascadingParameter(Name = "View")]
        public View View { get; set; }

        protected DxTreeView treeView;
        public IList<PartGroupTreeItem> Items { get; set; } = new List<PartGroupTreeItem>();
        public IList<PartGroup> PartGroupItems { get; set; }

        private static BlazorApplication blazorApplication;
        private static IObjectSpace objectSpace;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            blazorApplication = ApplicationProvider.GetApplication();
            objectSpace = blazorApplication.CreateObjectSpace();

            PartGroupItems = objectSpace.GetObjects<PartGroup>(CriteriaOperator.FromLambda<PartGroup>(pg => pg.Parent == null))
                .OrderBy(i => i.Name)
                .ToList();

            var partGroupTreeItems = Mapper.Map<IEnumerable<PartGroupTreeItem>>(PartGroupItems);
            PartGroupTreeItem allParts = new PartGroupTreeItem()
            {
                Name = "All Parts",
                PartGroupCollection = partGroupTreeItems
            };
            Items.Add(allParts);
        }

        protected void SelectionChanged(TreeViewNodeEventArgs e)
        {
            var selectedPartGroup = e.NodeInfo?.DataItem as PartGroupTreeItem;
            if (View is ListView listView && selectedPartGroup is not null)
            {
                ClearOtherTabsFilters(listView);
                if (selectedPartGroup.Name == "All Parts")
                {
                    listView.CollectionSource.Criteria["FilterByPartGroup"] = null;
                }
                else
                {
                    listView.CollectionSource.Criteria["FilterByPartGroup"] =
                        CriteriaOperator.FromLambda<IPartGroupFilter>(o => o.PartGroup.Oid == selectedPartGroup.Oid);
                }
            }
        }

        private static void ClearOtherTabsFilters(ListView listView)
        {
            listView.CollectionSource.Criteria["FilterByPartType"] = null;
            listView.CollectionSource.Criteria["FilterByPlasticAnalysis"] = null;
        }
    }
}
