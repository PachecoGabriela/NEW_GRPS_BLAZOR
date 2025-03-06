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
using DevExpress.Persistent.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class DownloadFileController : ViewController
    {
        public DownloadFileController()
        {
            var downloadAction = new SimpleAction(this, "Download a File", PredefinedCategory.Save);

            downloadAction.Execute += async (s, e) => {
                IJSRuntime jsRuntime = Application.ServiceProvider.GetRequiredService<IJSRuntime>();
                NavigationManager navigationManager = Application.ServiceProvider.GetRequiredService<NavigationManager>();
                var fileName = "Planificación - Copy";
                var fileURL = $"{navigationManager.BaseUri}api/File/Download?fileName={fileName}";
                await jsRuntime.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
            };
        }
    }
}
