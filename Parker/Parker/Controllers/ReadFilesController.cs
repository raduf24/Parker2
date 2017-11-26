using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Parker.Controllers
{
    public class ReadFilesController : ApiController
    {
        [HttpPost]
        [Route("api/ReadFiles/{location}")]
        public IHttpActionResult Post(string location)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var uploadedFile = HttpContext.Current.Request.Files[0];
            var currentDirectory = HttpRuntime.AppDomainAppPath;
            var filePath = currentDirectory + "/Photos/" + location + "/" + uploadedFile.FileName;
            (new FileInfo(filePath)).Directory.Create();
            uploadedFile.SaveAs(filePath);
            SensorsController.LoadSensorInput(location, filePath);
            return Ok();
        }
    }
}
