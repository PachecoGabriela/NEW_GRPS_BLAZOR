using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;
using Microsoft.AspNetCore.Mvc;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    [Route("api/downloadfile")]
    [ApiController]
    public partial class DownloadFileController : ControllerBase
    {
        private readonly IObjectSpaceFactory objectSpaceFactory;

        public DownloadFileController(IObjectSpaceFactory objectSpaceFactory)
        {
            this.objectSpaceFactory = objectSpaceFactory;
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadFile(Guid id)
        {
            using (var objectSpace = objectSpaceFactory.CreateObjectSpace(typeof(FileDataEmail)))
            {
                var file = objectSpace.GetObjectByKey<FileDataEmail>(id);
                if (file == null || file.Content == null)
                    return NotFound("El archivo no existe.");

                return File(file.Content, "application/octet-stream", file.FileName);
            }
        }
    }
}
