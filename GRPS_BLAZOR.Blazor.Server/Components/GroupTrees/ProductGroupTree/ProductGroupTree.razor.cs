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

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.ProductGroupTree
{
    public class ProductGroupTreeBase : ComponentBase
    {
        [Inject]
        IXafApplicationProvider ApplicationProvider { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        [Parameter]
        public View View { get; set; }

        protected DxTreeView treeView;
        public IList<ProductGroupTreeItem> Items { get; set; } = new List<ProductGroupTreeItem>();
        public IList<ProductGroup> ProductGroupItems { get; set; }

        private static BlazorApplication blazorApplication;
        private static IObjectSpace objectSpace;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            blazorApplication = ApplicationProvider.GetApplication();
            objectSpace = blazorApplication.CreateObjectSpace();

            ProductGroupItems = objectSpace.GetObjects<ProductGroup>(CriteriaOperator.FromLambda<ProductGroup>(pg => pg.Parent == null))
                .OrderBy(i => i.Name)
                .ToList();

            //ProductGroupItems = objectSpace.GetObjects<ProductGroup>(CriteriaOperator.FromLambda<ProductGroup>(pg => pg.Parent.Oid == 1))
            //    .OrderBy(i => i.Name)
            //    .ToList();

            var productGroupTreeItems = Mapper.Map<IEnumerable<ProductGroupTreeItem>>(ProductGroupItems);
            ProductGroupTreeItem allProducts = new ProductGroupTreeItem()
            {
                Name = "All Products",
                ProductGroupCollection = productGroupTreeItems
            };
            Items.Add(allProducts);
        }

        protected void SelectionChanged(TreeViewNodeEventArgs e)
        {
            var selectedProductGroup = e.NodeInfo?.DataItem as ProductGroupTreeItem;
            if (View is ListView listView && selectedProductGroup is not null)
            {
                if (selectedProductGroup.Name == "All Products")
                {
                    listView.CollectionSource.Criteria["FilterByProductGroup"] =
                        CriteriaOperator.FromLambda<IProductGroupFilter>(null);
                }
                else
                {
                    listView.CollectionSource.Criteria["FilterByProductGroup"] =
                        CriteriaOperator.FromLambda<IProductGroupFilter>(o => o.Group.Oid == selectedProductGroup.Oid);
                }
            }
        }
    }
}
