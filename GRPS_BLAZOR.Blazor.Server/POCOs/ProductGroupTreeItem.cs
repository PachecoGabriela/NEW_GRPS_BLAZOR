using GRPS_BLAZOR.Module.Interfaces;

namespace TestSideTrees.Blazor.Server.POCOs
{
    public class ProductGroupTreeItem
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public ProductGroupTreeItem Parent { get; set; }
        public IEnumerable<ProductGroupTreeItem> ProductGroupCollection { get; set; }
    }
}
