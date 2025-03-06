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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public partial class FileController : ControllerBase
    {
        [HttpGet(nameof(Download))]
        public IActionResult Download(string fileName)
        {

            // Replace this line with an actual stream, e.g. file stream File.OpenRead(filePath);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("YourData"));
            return File(stream, "text/plain", fileName);
        }
    }
}
