using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using Microsoft.AspNetCore.Components;
using TestSideTrees.Blazor.Server.POCOs;
using AutoMapper;

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.PlasticAnalysisTree
{
    public class PlasticAnalysisTreeBase : ComponentBase
    {
        [Inject]
        IXafApplicationProvider ApplicationProvider { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        [CascadingParameter(Name = "View")]
        public View View { get; set; }

        protected DxTreeView treeView;
        public IList<TreeItem> Items { get; set; } = new List<TreeItem>();
        //public IList<EnumDomain> PlasticAnalysisItems { get; set; }

        private static BlazorApplication blazorApplication;
        private static IObjectSpace objectSpace;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            blazorApplication = ApplicationProvider.GetApplication();
            objectSpace = blazorApplication.CreateObjectSpace();

            //PlasticAnalysisItems = objectSpace.GetObjects<EnumDomain>(CriteriaOperator.FromLambda<EnumDomain>(ed => ed.Name == "AttributeName"))
            //    .OrderBy(i => i.Name)
            //    .ToList();

            var plasticAnalysisRootItem = objectSpace.FindObject<EnumDomain>(CriteriaOperator.FromLambda<EnumDomain>(ed => ed.Name == "AttributeName"));

            var plasticAnalysisRootTreeItem = Mapper.Map<TreeItem>(plasticAnalysisRootItem);

            //Repak Tree Items
            var repakItems = objectSpace.GetObjects<EnumInstance>(CriteriaOperator.FromLambda<EnumInstance>(ei => ei.Domain.Name == "Repak"));
            var repakTreeItems = Mapper.Map<IEnumerable<TreeItem>>(repakItems);

            //Recyclable In Ireland Tree Items
            var recyclableInIrelandItems = objectSpace.GetObjects<EnumInstance>(CriteriaOperator.FromLambda<EnumInstance>(ei => ei.Domain.Name == "Recyclable In Ireland"));
            var recyclableInIrelandTreeItems = Mapper.Map<IEnumerable<TreeItem>>(recyclableInIrelandItems);

            //Germany Tree Items
            var germanyItems = objectSpace.GetObjects<EnumInstance>(CriteriaOperator.FromLambda<EnumInstance>(ei => ei.Domain.Name == "Germany"));
            var germanyTreeItems = Mapper.Map<IEnumerable<TreeItem>>(germanyItems);

            foreach (TreeItem item in plasticAnalysisRootTreeItem.ChildrenCollection)
            {
                switch (item.Name)
                {
                    case "Repak":
                        item.ChildrenCollection = repakTreeItems;
                        break;
                    case "Recyclable In Ireland":
                        item.ChildrenCollection = recyclableInIrelandTreeItems;
                        break;
                    case "Germany":
                        item.ChildrenCollection = germanyTreeItems;
                        break;
                }
            }

            #region Keith Implementation

            //Repak Items
            //var repakItems = plasticAnalysisTreeItems.Where(i => i.Oid >= 158 && i.Oid <= 163);
            //TreeItem repakParentItem = new TreeItem()
            //{
            //    Name = "Repak",
            //    ChildrenCollection = repakItems
            //};

            //Recyclable In Ireland Items
            //var recyclableInIrelandItems = plasticAnalysisTreeItems.Where(i => i.Oid >= 234 && i.Oid <= 239);
            //TreeItem recyclableInIrelandParentItem = new TreeItem()
            //{
            //    Name = "Recyclable In Ireland",
            //    ChildrenCollection = recyclableInIrelandItems
            //};

            //Germany Items
            //var germanyItems = plasticAnalysisTreeItems.Where(i => i.Oid >= 212 && i.Oid <= 215);
            //TreeItem germanyParentItem = new TreeItem()
            //{
            //    Name = "Germany",
            //    ChildrenCollection = germanyItems
            //};

            #endregion

            TreeItem allPartTypes = new TreeItem()
            {
                Name = "All Part Types",
                ChildrenCollection = plasticAnalysisRootTreeItem.ChildrenCollection
            };
            Items.Add(allPartTypes);
        }

        protected void SelectionChanged(TreeViewNodeEventArgs e)
        {
            var selectedAttribute = e.NodeInfo?.DataItem as TreeItem;
            if (View is ListView listView && selectedAttribute is not null
                && selectedAttribute.Name != "Recyclable In Ireland"
                && selectedAttribute.Name != "Repak"
                && selectedAttribute.Name != "Germany")
            {
                ClearOtherTabsFilters(listView);
                if (selectedAttribute.Name == "All Part Types")
                {
                    listView.CollectionSource.Criteria["FilterByPlasticAnalysis"] = null;
                }
                else
                {
                    listView.CollectionSource.Criteria["FilterByPlasticAnalysis"] =
                        CriteriaOperator.FromLambda<Part>(p => p.SRAttributes.Any(attribute => attribute.Oid == selectedAttribute.Oid));
                }
            }
        }

        private static void ClearOtherTabsFilters(ListView listView)
        {
            listView.CollectionSource.Criteria["FilterByPartGroup"] = null;
            listView.CollectionSource.Criteria["FilterByPartType"] = null;
        }
    }
}
