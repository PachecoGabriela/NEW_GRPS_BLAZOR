namespace TestSideTrees.Blazor.Server.POCOs
{
    public class SupplierTreeItem
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public IEnumerable<SupplierTreeItem> SupplierCollection { get; set; }
    }
}
