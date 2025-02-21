using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
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
    public static class GlobalContext
    {
        public static List<Supplier> SelectedSuppliers { get; set; } = new List<Supplier>();
    }

    public partial class EmailDraftController : ViewController<DetailView>
    {
        SimpleAction SendEmail;
        PopupWindowShowAction ShowSuppliers;
        EmailObject CurrentObject;
        public SendGridClientManager SendGridM { get; set; }
        public EmailDraftController()
        {
            InitializeComponent();

            TargetObjectType = typeof(EmailObject);
            TargetViewId = "EmailObject_DetailView_Draft";

            SendEmail = new SimpleAction(this, "SendEmailButton", PredefinedCategory.View);
            SendEmail.Caption = "Send Email";
            SendEmail.ToolTip = "Send this email to the suppliers selected";
            SendEmail.Execute += SendEmail_Execute;

            ShowSuppliers = new PopupWindowShowAction(this, "ShowSuppliersButton", PredefinedCategory.View);
            ShowSuppliers.Caption = "Add Suppliers";
            ShowSuppliers.ToolTip = "Show List of Suppliers";
            ShowSuppliers.CustomizePopupWindowParams += ShowSuppliers_CustomizePopupWindowParams; ;
            ShowSuppliers.Executed += ShowSuppliers_Executed;
        }

        private void ShowSuppliers_Executed(object sender, ActionBaseEventArgs e)
        {
            var popupWindowView = e as PopupWindowShowActionExecuteEventArgs;

            var selectedObjects = popupWindowView.PopupWindowView.SelectedObjects.OfType<Supplier>().ToList();

            if (selectedObjects.Any())
            {
                GlobalContext.SelectedSuppliers = selectedObjects;
            }
            else
            {
                throw new InvalidOperationException("No se seleccionaron objetos del tipo Supplier.");
            }

            List<string> SupplierCodes = new List<string>();

            if (GlobalContext.SelectedSuppliers.Any())
            {
                foreach (var supplier in GlobalContext.SelectedSuppliers)
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

                if (GlobalContext.SelectedSuppliers is not null && GlobalContext.SelectedSuppliers.Any())
                {
                    listView.ControlsCreated += (sender, args) =>
                    {
                        DxGridListEditor dxGridListEditor = listView.Editor as DxGridListEditor;
                       
                        var selectedOids = GlobalContext.SelectedSuppliers?.Select(s => s.Oid).ToList();

                        if (selectedOids != null && selectedOids.Any())
                        {
                            var objectsToSelect = newObjectSpace.GetObjectsQuery<Supplier>()
                                .Where(s => selectedOids.Contains(s.Oid))
                                .ToList();

                            dxGridListEditor.SetSelectedObjects(objectsToSelect);
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

            if (GlobalContext.SelectedSuppliers.Any())
            {
                if (!string.IsNullOrEmpty(CurrentObject.Subject))
                {
                    foreach (var Supplier in GlobalContext.SelectedSuppliers)
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
                            if (GlobalContextSpreadSheet.SelectedRecord is not null)
                            {
                                var session = ((XPObjectSpace)ObjectSpace).Session;

                                CreateRecords(GlobalContextSpreadSheet.SelectedRecord, CurrentSupplier, CurrentObject, supplierEmailsList, session);
                            }
                        }
                    }

                    if (response)
                    {
                        CurrentObject.Sent = true;
                        View.ObjectSpace.CommitChanges();
                    }
                }
                else
                    Application.ShowViewStrategy.ShowMessage("Make sure you define a subject before sending");
            }
            else
                Application.ShowViewStrategy.ShowMessage("Make sure you select a supplier before sending");
            
        }

        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(fileName, out string mimeType))
            {
                return mimeType;
            }
            return "application/octet-stream"; // Tipo genérico para archivos desconocidos
        }

        private Session CreateSessionForConnection(string connectionString)
        {
            var dataStore = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists);
            var dataLayer = new SimpleDataLayer(new ReflectionDictionary(), dataStore);
            return new Session(dataLayer);
        }

        private string GetMasterDatabaseConnectionString()
        {
            var configuration = Application.ServiceProvider.GetRequiredService<IConfiguration>();
            return configuration.GetConnectionString("DefaultConnection"); 
        }

        private Session CreateSessionForTenant(string connectionString)
        {
            var dataStore = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists);
            var dataLayer = new SimpleDataLayer(new ReflectionDictionary(), dataStore);
            return new Session(dataLayer);
        }

        private void CreateRecords(SpreadsheetContainer selectedRecord, Supplier currentSupplier, EmailObject currentObject, string supplierEmailsList, Session session)
        {
            QueryParameterCollection SsParameter = new QueryParameterCollection();
            var IdNewSource = Guid.NewGuid();
            var IdSource = IdNewSource.ToString();

            SsParameter.Add(IdSource);
            SsParameter.Add(selectedRecord.FileName);
            SsParameter.Add(currentSupplier.Code);
            SsParameter.Add(currentSupplier.Name);
            SsParameter.Add(selectedRecord.SpreadsheetFile);
            SsParameter.Add(selectedRecord.CreatedBy.Oid);

            string sqlDs = $"INSERT INTO [SpreadsheetContainer] ([Oid],[FileName],[CompanyCode],[CompanyName],[SpreadsheetFile],[CreatedBy]) values (@p0,@p1,@p2,@p3,@p4,@p5)";
            session.ExecuteNonQuery(sqlDs, SsParameter);

            QueryParameterCollection EParameter = new QueryParameterCollection();
            var IdNewEmail = Guid.NewGuid();
            var IdEmail = IdNewSource.ToString();

            EParameter.Add(IdSource);
            EParameter.Add(currentObject.Subject);
            EParameter.Add(supplierEmailsList);
            EParameter.Add(currentObject.Message);
            EParameter.Add(currentSupplier.Code);
            EParameter.Add(currentSupplier.Name);
            EParameter.Add(1);
            

            string sqlEmail = $"INSERT INTO [EmailObject] ([Oid],[Subject],[ToEmail],[Message],[CompanyCode],[EmailSentFrom],[Received]) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
            session.ExecuteNonQuery(sqlEmail, EParameter);
        }

        static IDataLayer CreateDataLayer(bool threadSafe, string connStr)
        {
            ReflectionDictionary dictionary = new ReflectionDictionary();
            AutoCreateOption autoCreateOption = AutoCreateOption.SchemaAlreadyExists;
            IDataStore provider = XpoDefault.GetConnectionProvider(connStr, autoCreateOption);
            var localdl = threadSafe ? (IDataLayer)new ThreadSafeDataLayer(dictionary, provider) : new SimpleDataLayer(dictionary, provider);

            return localdl;
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

            GlobalContext.SelectedSuppliers.Clear();
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
