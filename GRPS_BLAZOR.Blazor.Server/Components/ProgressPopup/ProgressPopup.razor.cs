using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using XafCustomComponents;

namespace GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup
{
    public partial class ProgressPopup : ComponentBase
    {
        [CascadingParameter] DialogInstance Dialog { get; set; }
        [Parameter] public BackgroundWorker Worker { get; set; }

        private void CancelClickedHandler(MouseEventArgs e)
        {
            Dialog?.Cancel();
        }
    }
}
