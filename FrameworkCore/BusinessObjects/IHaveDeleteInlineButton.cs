namespace FrameworkCore
{
    /// <summary>
    /// When implemented by a business object, it adds unbound Delete column to the grid.
    /// </summary>
    public interface IHaveDeleteInlineButton
    {
        void DoDelete();
    }
}