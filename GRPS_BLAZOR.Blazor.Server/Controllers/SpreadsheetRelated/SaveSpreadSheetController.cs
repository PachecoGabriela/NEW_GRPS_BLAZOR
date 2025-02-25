using DevExpress.Blazor.Internal.Grid;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GRPS_BLAZOR.Blazor.Server.Services;
using GRPS_BLAZOR.Module.BusinessObjects;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;
using Microsoft.JSInterop;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SpreadsheetRelated
{

    public partial class SaveSpreadSheetController : ObjectViewController<DetailView, SpreadsheetContainer>
    {
        IJSRuntime blazorIJSRuntime;
        ModificationsController saveController;
        SimpleAction SendEmailButton;
        SimpleAction NotificationButton;
        SpreadsheetContainer CurrentObject;
        ApplicationUser currentUser;
        public SendGridClientManager SendGridM { get; set; }
        public SaveSpreadSheetController()
        {
            InitializeComponent();

            SendEmailButton = new SimpleAction(this, "SendEmailToSupplier", PredefinedCategory.View);
            SendEmailButton.Caption = "Send email to suppliers";
            SendEmailButton.ImageName = "Actions_EnvelopeOpen";
            SendEmailButton.Execute += SendEmailButton_Execute;

            NotificationButton = new SimpleAction(this, "NotificationButton", PredefinedCategory.View);
            NotificationButton.Caption = "Complete";
            NotificationButton.ToolTip = "Sends a notification that spreadsheet is completed";
            NotificationButton.ImageName = "Action_MarkCompleted";
            NotificationButton.Execute += NotificationButton_Execute;
        }

        private async void NotificationButton_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var session = ((XPObjectSpace)ObjectSpace).Session;
            var CurrentObject = (SpreadsheetContainer)View.CurrentObject;

            if (CurrentObject == null || CurrentObject.SpreadsheetFile == null)
            {
                throw new UserFriendlyException("No hay archivo para validar.");
            }

            bool isValid = ValidateExcel(CurrentObject.SpreadsheetFile);
            if (isValid)
            {
                SendGridM = Application.ServiceProvider.GetRequiredService<SendGridClientManager>();
                string Message = string.Format("{0} informs that their spreadsheet is completed", CurrentObject.CompanyName);
                bool response = await SendGridM.SendEmail(CurrentObject.CreatedBy.Email, Message, "Completion of Spreadsheet", null);
                if (response)
                {
                    EmailObject NewEmail = new EmailObject(session);

                    NewEmail.Subject = "Completion of Spreadsheet";
                    NewEmail.ToEmail = CurrentObject.CreatedBy.Email;
                    NewEmail.Message = Message;
                    NewEmail.Received = true;
                    NewEmail.Sent = true;
                    NewEmail.From = currentUser.Email;

                    CurrentObject.Status = SpreadSheetStatus.Completed;
                    session.CommitTransaction();
                }
            }
            else
                Application.ShowViewStrategy.ShowMessage("There are empty fields.");
           
            
        }

        private bool ValidateExcel(byte[] fileData, int startRow = 10)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                using (MemoryStream stream = new MemoryStream(fileData))
                {
                    IWorkbook workbook = application.Workbooks.Open(stream);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    int rowCount = worksheet.Rows.Length; // Obtiene la cantidad real de filas con datos
                    int colCount = worksheet.UsedRange.LastColumn - 3; // Última columna con datos

                    // Encontrar la última fila con datos reales
                    int lastRowWithData = startRow;
                    for (int row = startRow; row < rowCount; row++)
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            IRange cell = worksheet[row, col];
                            if (!string.IsNullOrWhiteSpace(cell.DisplayText))
                            {
                                lastRowWithData = row; // Guarda la última fila donde se encontró dato
                                break;
                            }
                        }
                    }

                    // Validar solo las filas con datos
                    for (int row = startRow; row <= lastRowWithData; row++)
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            IRange cell = worksheet[row, col];
                            if (string.IsNullOrWhiteSpace(cell.DisplayText))
                            {
                                return false; // Hay celdas vacías en una fila con datos
                            }
                        }
                    }
                }
            }
            return true; // Todas las filas con datos están completas
        }


        private void SendEmailButton_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            CurrentObject = (SpreadsheetContainer)View.CurrentObject;
            if (CurrentObject is not null)
            {
                if (!string.IsNullOrEmpty(CurrentObject.FileName) && CurrentObject.SpreadsheetFile is not null)
                {
                    if (!string.IsNullOrEmpty(currentUser.Email))
                    {
                        var objectSpace = Application.CreateObjectSpace();
                        var NewEmail = objectSpace.CreateObject<EmailObject>();
                        NewEmail.From = currentUser.Email;
                        NewEmail.OriginContainer = objectSpace.GetObject(CurrentObject); 

                        if (objectSpace.IsModified)
                            objectSpace.CommitChanges();

                        var detailView = Application.CreateDetailView(objectSpace, "EmailObject_DetailView_Draft", true, NewEmail);
                        var showViewParameters = new ShowViewParameters(detailView)
                        {
                            Context = TemplateContext.PopupWindow
                        };

                        var sourceFrame = this.Frame;
                        Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(sourceFrame, SendEmailButton));
                    }
                    else
                        Application.ShowViewStrategy.ShowMessage("Please define an email address before to proceed");
                }
                else
                   Application.ShowViewStrategy.ShowMessage("Make sure Name and File fields are not empty");

            }

            
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            blazorIJSRuntime = ((BlazorApplication)Application).ServiceProvider.GetRequiredService<IJSRuntime>();
            saveController = Frame.GetController<ModificationsController>();

            currentUser = SecuritySystem.CurrentUser as ApplicationUser;
            if (currentUser is not null)
            {
                var Role = currentUser.Roles.FirstOrDefault(x => x.Name == "Supplier");
                if (Role is not null)
                {
                    this.SendEmailButton.Active.SetItemValue("HideEmailButton",false);
                }
                else
                    this.NotificationButton.Active.SetItemValue("HideNotificationButton", false);
            }
        }
        private void SaveAction_Executing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            blazorIJSRuntime.InvokeVoidAsync("SaveSpreadSheet");
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            saveController.SaveAction.Executing -= SaveAction_Executing;
        }
    }
}
