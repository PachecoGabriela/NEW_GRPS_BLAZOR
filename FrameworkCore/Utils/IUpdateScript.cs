using System;
using DevExpress.ExpressApp;

namespace FrameworkCore.Utils
{
    public interface IUpdateScript
    {
        Guid UpdateID { get; }
        string Description { get; }
        DateTime CreatedDate { get; }
        string Run(IObjectSpace space);
    }
}
