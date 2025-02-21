using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.BusinessObjects;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;
using Syncfusion.EJ2.Spreadsheet;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.SpreadsheetRelated
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class SpreadSheetContainerFilterController : ViewController<ListView>
    {
        public SpreadSheetContainerFilterController()
        {
            InitializeComponent();
            TargetObjectType = typeof(SpreadsheetContainer);
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (View is ListView listView && listView.CollectionSource is CollectionSourceBase collectionSource)
            {
                var currentUser = SecuritySystem.CurrentUser as ApplicationUser;

                var Supplier = currentUser.Roles.FirstOrDefault(x => x.Name == "Supplier");
                if (Supplier is not null)
                {
                    
                    var contacts = new XPCollection<SupplierContact>(session);
                    var Contact = contacts.FirstOrDefault(x => x.Email == currentUser.Email);
                    if (Contact is not null)
                    {
                        if (Contact.Supplier is not null)
                            collectionSource.Criteria["CustomFilter"] = CriteriaOperator.Parse("Contains(companyName, ?)", Contact.Supplier.Name);
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
        }
        protected override void OnDeactivated()
        {
            
            base.OnDeactivated();
        }
    }
}
