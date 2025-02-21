using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachment.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.FileAttachments.Blazor.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class UploadCustomizeController : ObjectViewController<DetailView, EmailObject>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public UploadCustomizeController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            View.CustomizeViewItemControl<FileDataPropertyEditor>(this, customizeAction);
        }

        private void customizeAction(FileDataPropertyEditor obj)
        {
            if (obj.Control is FileDataEditorComponentAdapter fileDataEditorComponentAdapter)
            {
                var type = fileDataEditorComponentAdapter.GetType();
                var createComponent = type.GetMethod("CreateComponent", BindingFlags.NonPublic | BindingFlags.Instance);
                var ttt = createComponent.GetType();
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
