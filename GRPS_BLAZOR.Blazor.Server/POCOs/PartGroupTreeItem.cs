namespace TestSideTrees.Blazor.Server.POCOs
{
    public class PartGroupTreeItem
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public IEnumerable<PartGroupTreeItem> PartGroupCollection { get; set; }
    }
}
