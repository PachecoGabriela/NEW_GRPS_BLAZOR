namespace TestSideTrees.Blazor.Server.POCOs
{
    public class TreeItem
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public IEnumerable<TreeItem> ChildrenCollection { get; set; }
    }
}
