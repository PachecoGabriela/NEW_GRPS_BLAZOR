using ExcelImport;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using XafCustomComponents;

namespace GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup.ImportExcelData
{
    public partial class ImportExcelDataProgress : ComponentBase
    {
        [Inject] IDialogService DialogService { get; set; }
        [CascadingParameter] DialogInstance Dialog { get; set; }
        [Parameter] public BackgroundWorker Worker { get; set; }

        public ImportExcelState State { get; set; }
        public int Percentage { get; set; }
        public string ImportRecordsDetail { get; set; }
        public string ImportRecordsAdditionalDetails { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dialog.CloseOnEscapeKey = false;
            Dialog.CloseOnOutsideClick = false;
            Dialog.CloseButton = false;
        }

        private void ProgressChangedHandler(object state)
        {
            State = state as ImportExcelState;
            Percentage = State.Percentage;
            ImportRecordsDetail = State.ImportRecordsDetail;
            ImportRecordsAdditionalDetails = State.ImportRecordsAdditionalDetails;
        }

        private async void CancelClickedHandler(MouseEventArgs e)
        {
            bool? cancelOperation = await DialogService.ShowMessageBox("Cancel Operation", "Are you sure you want to cancel this operation?", yesText: "Yes", noText: "No");
            if (cancelOperation is not null && (bool)cancelOperation)
                Dialog?.Cancel();
        }

        private void OkClickedHandler(MouseEventArgs e)
        {
            Dialog.Close();
        }
    }

}
