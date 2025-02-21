using DevExpress.ExpressApp;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadFileController : ControllerBase
    {
        private readonly IObjectSpaceProvider objectSpaceProvider;

        public DownloadFileController(IObjectSpaceProvider objectSpaceProvider)
        {
            this.objectSpaceProvider = objectSpaceProvider;
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadFile(Guid id)
        {
            using (var objectSpace = objectSpaceProvider.CreateObjectSpace())
            {
                // Obtén el archivo usando el id (Oid)
                var fileDataEmail = objectSpace.GetObjectByKey<FileDataEmail>(id);

                if (fileDataEmail == null)
                {
                    return NotFound(); // Retorna un 404 si no se encuentra el archivo
                }

                // Determina el tipo de contenido según la extensión del archivo
                string contentType;
                string fileExtension = Path.GetExtension(fileDataEmail.FileName).ToLowerInvariant();

                switch (fileExtension)
                {
                    case ".csv":
                        contentType = "text/csv";
                        break;
                    case ".txt":
                        contentType = "text/plain";
                        break;
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    // Agrega más tipos de contenido según tus necesidades
                    default:
                        contentType = "application/octet-stream"; // Tipo por defecto
                        break;
                }

                // Configura la respuesta para la descarga del archivo
                var result = new FileContentResult(fileDataEmail.Content, contentType)
                {
                    FileDownloadName = fileDataEmail.FileName
                };

                return result; // Devuelve el archivo como resultado
            }
        }
    }
}
