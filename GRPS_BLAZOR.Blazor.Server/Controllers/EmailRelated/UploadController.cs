using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRPS_BLAZOR.Blazor.Server.Controllers.EmailRelated
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("Upload")]
        public ActionResult Upload(IFormFile Editor)
        {
            try
            {
                var myFile = Request.Form.Files[0];
                if (myFile != null && myFile.Length > 0)
                {

                    // Logic for saving the file here.
                }
                else
                {
                    // return BadRequest("No file provided.");
                }
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
