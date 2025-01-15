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

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.SupplierGroupTree
{
    public class SupplierTreeBase : ComponentBase
    {
        [Inject]
        IXafApplicationProvider ApplicationProvider { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        [Parameter]
        public View View { get; set; }

        protected DxTreeView treeView;
        //public IList<SupplierTreeItem> Items { get; set; } = new List<SupplierTreeItem>();
        //public IList<Supplier> SupplierItems { get; set; }

        public IList<SupplierTreeItem> Items { get; set; } = new List<SupplierTreeItem>();
        public IList<Supplier> SupplierItems { get; set; }

        private static BlazorApplication blazorApplication;
        private static IObjectSpace objectSpace;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            blazorApplication = ApplicationProvider.GetApplication();
            objectSpace = blazorApplication.CreateObjectSpace();

            SupplierItems = objectSpace.GetObjects<Supplier>()
                .OrderBy(s => s.Name)
                .ToList();

            var supplierTreeItems = Mapper.Map<IEnumerable<SupplierTreeItem>>(SupplierItems);
            //var supplierTreeItems = Mapper.Map<IEnumerable<TreeItem>>(SupplierItems);
            SupplierTreeItem allSuppliers = new SupplierTreeItem()
            //TreeItem allSuppliers = new TreeItem()
            {
                Name = "All Suppliers",
                SupplierCollection = supplierTreeItems
            };
            Items.Add(allSuppliers);
        }

        protected void SelectionChanged(TreeViewNodeEventArgs e)
        {
            var selectedSupplier = e.NodeInfo?.DataItem as SupplierTreeItem;
            //var selectedSupplier = e.NodeInfo?.DataItem as TreeItem;
            if (View is ListView listView && selectedSupplier is not null)
            {
                if (selectedSupplier.Name == "All Suppliers")
                {
                    listView.CollectionSource.Criteria["FilterBySupplier"] =
                        CriteriaOperator.FromLambda<ISupplierFilter>(null);
                }
                else
                {
                    listView.CollectionSource.Criteria["FilterBySupplier"] =
                        CriteriaOperator.FromLambda<ISupplierFilter>(p => p.Supplier.Oid == selectedSupplier.Oid);
                }
            }
        }
    }
}
