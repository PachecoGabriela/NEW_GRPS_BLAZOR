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

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.PartTypeTree
{
    public class PartTypeTreeBase : ComponentBase
    {
        [Inject]
        IXafApplicationProvider ApplicationProvider { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        [CascadingParameter(Name = "View")]
        public View View { get; set; }

        protected DxTreeView treeView;
        public IList<TreeItem> Items { get; set; } = new List<TreeItem>();
        public IList<EnumInstance> PartTypes { get; set; }

        private static BlazorApplication blazorApplication;
        private static IObjectSpace objectSpace;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            blazorApplication = ApplicationProvider.GetApplication();
            objectSpace = blazorApplication.CreateObjectSpace();

            PartTypes = objectSpace.GetObjects<EnumInstance>(CriteriaOperator.FromLambda<EnumInstance>(ei => ei.Domain.Name == "PartType"))
                .OrderBy(i => i.Name)
                .ToList();

            var partTypeTreeItems = Mapper.Map<IEnumerable<TreeItem>>(PartTypes);
            TreeItem allPartTypes = new TreeItem()
            {
                Name = "All Part Types",
                ChildrenCollection = partTypeTreeItems
            };
            Items.Add(allPartTypes);
        }

        protected void SelectionChanged(TreeViewNodeEventArgs e)
        {
            var selectedPartType = e.NodeInfo?.DataItem as TreeItem;
            if (View is ListView listView && selectedPartType is not null)
            {
                ClearOtherTabsFilters(listView);
                if (selectedPartType.Name == "All Part Types")
                {
                    listView.CollectionSource.Criteria["FilterByPartType"] = null;
                }
                else
                {
                    listView.CollectionSource.Criteria["FilterByPartType"] =
                        CriteriaOperator.FromLambda<IPartTypeFilter>(p => p.PartType.Oid == selectedPartType.Oid);
                }
            }
        }

        private static void ClearOtherTabsFilters(ListView listView)
        {
            listView.CollectionSource.Criteria["FilterByPartGroup"] = null;
            listView.CollectionSource.Criteria["FilterByPlasticAnalysis"] = null;
        }
    }
}
