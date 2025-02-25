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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using GRPS_BLAZOR.Module.BusinessObjects;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class EmailFilterController : ViewController<ListView>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public EmailFilterController()
        {
            InitializeComponent();  
        }
        protected override void OnActivated()
        {
            base.OnActivated();
             if (View.Id == "EmailObject_ListView_Received")
             {
                if (View is ListView listView && listView.CollectionSource is CollectionSourceBase collectionSource)
                {

                    var currentUser = SecuritySystem.CurrentUser as ApplicationUser;
                    if (currentUser is not null)
                    {
                        collectionSource.Criteria["CustomFilter"] = CriteriaOperator.Parse("Contains(ToEmail, ?)", currentUser.Email);
                    }
                }
             }

             if (View.Id == "EmailObject_ListView_Draft" || View.Id == "EmailObject_ListView_Sent")
             {
                if (View is ListView listView && listView.CollectionSource is CollectionSourceBase collectionSource)
                {

                    var currentUser = SecuritySystem.CurrentUser as ApplicationUser;
                    if (currentUser is not null)
                    {
                        collectionSource.Criteria["CustomFilter"] = CriteriaOperator.Parse("Contains(From, ?)", currentUser.Email);
                    }
                }
             }

            

        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
