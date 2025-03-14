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
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    [ApiController]
    [Route("api/downloadfile")]
    public partial class FileController : ControllerBase
    {
        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public FileController(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        [HttpDelete("delete/{oid}")]
        public IActionResult DeleteFile(Guid oid)
        {
            using (var objectSpace = _objectSpaceFactory.CreateObjectSpace(typeof(FileDataEmail)))
            {
                var file = objectSpace.GetObjectByKey<FileDataEmail>(oid);
                if (file == null)
                    return NotFound(new { message = "File not found" });

                objectSpace.Delete(file);
                objectSpace.CommitChanges();

                return Ok(new { message = "File deleted succesfully" });
            }
        }
    }
}
