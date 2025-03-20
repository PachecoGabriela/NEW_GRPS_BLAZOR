using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.MultiTenancy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraCharts;
using GRPS_BLAZOR.Blazor.Server.Controllers.SpreadsheetRelated;
using GRPS_BLAZOR.Blazor.Server.Services;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;
using Microsoft.AspNetCore.StaticFiles;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    public partial class EmailDraftController : ViewController<DetailView>
    {
        SimpleAction SendEmail;
        PopupWindowShowAction ShowSuppliers;
        EmailObject CurrentObject;
        List<Supplier> selectedObjects = new List<Supplier>();
        public SendGridClientManager SendGridM { get; set; }
        public EmailDraftController()
        {
            InitializeComponent();

            TargetObjectType = typeof(EmailObject);
            TargetViewId = "EmailObject_DetailView_Draft";

            SendEmail = new SimpleAction(this, "SendEmailButton", PredefinedCategory.View);
            SendEmail.Caption = "Send Email";
            SendEmail.ToolTip = "Send this email to the suppliers selected";
            SendEmail.ImageName = "Actions_EnvelopeOpen";
            SendEmail.Execute += SendEmail_Execute;

            ShowSuppliers = new PopupWindowShowAction(this, "ShowSuppliersButton", PredefinedCategory.View);
            ShowSuppliers.Caption = "Add Suppliers";
            ShowSuppliers.ToolTip = "Show List of Suppliers";
            ShowSuppliers.ImageName = "ListBullets_32x32";
            ShowSuppliers.CustomizePopupWindowParams += ShowSuppliers_CustomizePopupWindowParams; ;
            ShowSuppliers.Executed += ShowSuppliers_Executed;
        }

        private void DownloadAttachment_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            CurrentObject = View.CurrentObject as EmailObject;
            var archivos = CurrentObject.Files.ToList();

            var httpContext = ((BlazorApplication)Application).ServiceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            if (httpContext?.HttpContext == null || archivos.Count == 0)
            {
                return;
            }

            foreach (var archivo in archivos)
            {
                if (archivo.Content != null)
                {
                    string fileName = archivo.FileName;
                    byte[] fileContent = archivo.Content;

                    httpContext.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    httpContext.HttpContext.Response.ContentType = "application/octet-stream";
                    httpContext.HttpContext.Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                    httpContext.HttpContext.Response.Body.Flush();
                }
            }
        }

        private void ShowSuppliers_Executed(object sender, ActionBaseEventArgs e)
        {
            var popupWindowView = e as PopupWindowShowActionExecuteEventArgs;

            selectedObjects = popupWindowView.PopupWindowView.SelectedObjects.OfType<Supplier>().ToList();

            if (selectedObjects.Any())
            {
                string suppliersList = string.Join(";", selectedObjects.Select(s => s.Oid));


                CurrentObject.SuppliersSelectedOids = suppliersList;
                CurrentObject.Session.CommitTransaction();
            }
            else
            {
                throw new InvalidOperationException("No suppliers were selected.");
            }

            List<string> SupplierCodes = new List<string>();

            if (selectedObjects.Any())
            {
                foreach (var supplier in selectedObjects)
                {
                    SupplierCodes.Add(supplier.Code);
                }

                string suppliersList = string.Join("; ", SupplierCodes);


                var emailObject = View.CurrentObject as EmailObject;
                if (emailObject != null)
                {
                    emailObject.To = suppliersList;
                }
            }
        }

        private void ShowSuppliers_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            CurrentObject = View.CurrentObject as EmailObject;
            if (CurrentObject is not null)
            {
                var newObjectSpace = Application.CreateObjectSpace(typeof(Supplier));

                var collectionSource = new CollectionSource(newObjectSpace, typeof(Supplier));
                var listView = Application.CreateListView("Supplier_ListView", collectionSource, false);

                if (!string.IsNullOrEmpty(CurrentObject.SuppliersSelectedOids))
                {
                    listView.ControlsCreated += (sender, args) =>
                    {
                        DxGridListEditor dxGridListEditor = listView.Editor as DxGridListEditor;

                        AssignSuppliers(CurrentObject, newObjectSpace);

                        if (selectedObjects.Any())
                        {
                            dxGridListEditor.SetSelectedObjects(selectedObjects);
                        }

                    };
                }

                e.View = listView;
            }
        }

        private async void SendEmail_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            CurrentObject = View.CurrentObject as EmailObject;
            bool response = false;
            Supplier CurrentSupplier;
            string supplierEmailsList;
            var session = ((XPObjectSpace)ObjectSpace).Session;

            if (!string.IsNullOrEmpty(CurrentObject.To))
            {
                AssignSuppliers(CurrentObject, View.ObjectSpace);

                if (selectedObjects.Any())
                { 
                    if (!string.IsNullOrEmpty(CurrentObject.Subject))
                    {
                        var Attachments = CurrentObject.Files.ToList();
                        foreach (var Supplier in selectedObjects)
                        {
                            CurrentSupplier = Supplier;
                            List<string> ContactEmails = new List<string>();

                            foreach (var Contact in Supplier.SupplierContacts)
                            {
                                ContactEmails.Add(Contact.Email);
                            }
                            supplierEmailsList = string.Join("; ", ContactEmails);

                            if (!string.IsNullOrEmpty(supplierEmailsList))
                            {
                                SendGridM = Application.ServiceProvider.GetRequiredService<SendGridClientManager>();

                                var UploadedAtachments = CurrentObject.Files;

                                List<(string, byte[], string)> attachments = UploadedAtachments
                                    .Select(a => (a.FileName, a.Content, GetMimeType(a.FileName)))
                                    .ToList();

                                response = await SendGridM.SendEmail(supplierEmailsList, CurrentObject.Message, CurrentObject.Subject, attachments);
                            }
                            else
                                Application.ShowViewStrategy.ShowMessage("Make sure the supplier you have selected contains contacts");


                            if (response)
                            {
                                if (CurrentObject.OriginContainer is not null)
                                {
                                    CreateRecords(CurrentObject.OriginContainer, CurrentSupplier, CurrentObject, supplierEmailsList, session, Attachments);
                                    session.CommitTransaction();
                                }

                           
                            }
                        }

                        if (response)
                        {
                            CurrentObject.Sent = true;
                            View.ObjectSpace.CommitChanges();
                            Application.ShowViewStrategy.ShowMessage("Emails and spreadheets created succesfully");
                        }
                    }
                    else
                        Application.ShowViewStrategy.ShowMessage("Make sure you define a subject before sending");
                }
            }
            else
                Application.ShowViewStrategy.ShowMessage("Make sure you select a supplier before sending");
            
        }

        private void AssignSuppliers(EmailObject currentObject, IObjectSpace newObjectSpace)
        {
            var selectedOids = CurrentObject.SuppliersSelectedOids.Split(";").ToList();

            if (selectedOids.Any())
            {
                selectedObjects = newObjectSpace.GetObjectsQuery<Supplier>()
                    .Where(s => selectedOids.Contains(s.Oid.ToString()))
                    .ToList();
            }
        }

        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(fileName, out string mimeType))
            {
                return mimeType;
            }
            return "application/octet-stream"; 
        }

        private void CreateRecords(SpreadsheetContainer selectedRecord, Supplier currentSupplier, EmailObject currentObject, string supplierEmailsList, Session session, List<FileDataEmail> attachments)
        {
            var newContainer = new SpreadsheetContainer(session)
            {
                FileName = selectedRecord.FileName,
                CompanyCode = currentSupplier.Code,
                CompanyName = currentSupplier.Name,
                SpreadsheetFile = selectedRecord.SpreadsheetFile,
                CreatedBy = selectedRecord.CreatedBy,
                Status = SpreadSheetStatus.Created
            };

            var newEmail = new EmailObject(session)
            {
                Subject = currentObject.Subject,
                ToEmail = supplierEmailsList,
                Message = currentObject.Message,
                Received = true
            };

            if (attachments?.Count > 0)
            {
                newEmail.Files.AddRange(attachments.Select(doc => new FileDataEmail(session)
                {
                    FileName = doc.FileName,
                    Content = doc.Content
                }));
            }

            session.CommitTransaction();

        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.Id == "EmailObject_DetailView_Draft")
            {
                var modificationsController = Frame.GetController<ModificationsController>();
                if (modificationsController != null)
                {
                    modificationsController.SaveAction.Active.SetItemValue("HideSave", false);
                    modificationsController.SaveAndNewAction.Active.SetItemValue("HideSaveAndNew", false);
                    modificationsController.SaveAndCloseAction.Active.SetItemValue("HideSaveAndClose", false);
                }
            }

            if ((ObjectSpace is NonPersistentObjectSpace) && (View.CurrentObject == null))
            {
                View.CurrentObject = ObjectSpace.CreateObject(View.ObjectTypeInfo.Type);
                View.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            }


            if (View.ObjectSpace != null)
                View.ObjectSpace.ModifiedChanged += ObjectSpace_ModifiedChanged;
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            View.ObjectSpace.CommitChanges();
        }

        private void ObjectSpace_ModifiedChanged(object sender, EventArgs e)
        {
            View.ObjectSpace.CommitChanges();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
        }
        protected override void OnDeactivated()
        {
            if (View.ObjectSpace != null)
                View.ObjectSpace.ModifiedChanged -= ObjectSpace_ModifiedChanged;
            base.OnDeactivated();
        }
    }
}
