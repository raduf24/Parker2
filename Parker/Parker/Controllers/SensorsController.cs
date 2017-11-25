using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Parker.Controllers
{
    public class SensorsController : ApiController
    {
        // GET: api/Sensors
        public IEnumerable<object> Get()
        {
            string sensorsPath = HttpContext.Current.Server.MapPath("~/Sensors/");
            var result = new List<object>();
            var js = new JavaScriptSerializer();
            foreach (string dir in Directory.GetDirectories(sensorsPath))
            {
                var sensorDto = js.Deserialize<SensorDto>(File.ReadAllText(dir + "/sensor.json"));
                var urlName = dir.Replace(sensorsPath, "");
                result.Add(new
                {
                    Name = sensorDto.Name,
                    Url = HttpContext.Current.Request.Url.OriginalString+"?sensorName="+urlName+"&outputJson=true",
                    MapUrl = sensorDto.MapUrl,
                    Coordinates = sensorDto.Coordinates
                });
            }            
            return result.ToArray();
        }

        // GET: api/Sensors/name
        public HttpResponseMessage Get(string sensorName, bool outputJSON)
        {                       
            var io = new ImageOperations();
            var sensorPath =HttpContext.Current.Server.MapPath($"~/Sensors/{sensorName}");
            var sensorFileContent = File.ReadAllText($"{sensorPath}/sensor.json");
            var js = new JavaScriptSerializer();
            var sensorDto = js.Deserialize<SensorDto>(sensorFileContent);
            var parkingSpacesOutputs = new List<ParkingSpaceOutputDto>(sensorDto.ParkingSpaces.Length);

            var inputImage = (Bitmap)Bitmap.FromFile($"{sensorPath}/{sensorDto.inputSample}");
            var emptyImage = (Bitmap)Bitmap.FromFile($"{sensorPath}/{sensorDto.EmptyImg}");

            var outputImage = (Bitmap)inputImage.Clone();
            using (Graphics outputGraphics = Graphics.FromImage(outputImage))
            {
                foreach (var parkingSpaceDto in sensorDto.ParkingSpaces)
                {
                    var inputCrop = (Bitmap)inputImage.Clone();
                    var emptyCrop = (Bitmap)emptyImage.Clone();

                    var sensorMask = (Bitmap)Bitmap.FromFile($"{sensorPath}/{parkingSpaceDto.SensorMask}");
                    var locationMask = (Bitmap)Bitmap.FromFile($"{sensorPath}/{parkingSpaceDto.LocationMask}");

                    var sensorPixelCount = io.CountNonEmptyPixels(sensorMask);

                    io.CropImageWithMask(inputCrop, sensorMask);
                    io.CropImageWithMask(emptyCrop, sensorMask);
                    io.DiffImage(inputCrop, emptyCrop);
                    var diffRGBSum = io.SumAllRGB(inputCrop);
                    bool vacant = (diffRGBSum / sensorPixelCount) < parkingSpaceDto.Threshold;

                    if (vacant)
                    {
                        io.SetMaskColor(locationMask, 0, 255, 0);
                    }
                    else
                    {
                        io.SetMaskColor(locationMask, 255, 0, 0);
                    }
                    outputGraphics.DrawImage(locationMask, 0, 0, outputImage.Width, outputImage.Height);

                    parkingSpacesOutputs.Add(new ParkingSpaceOutputDto { Name = parkingSpaceDto.Name, IsAvailable = vacant, ConfidenceLevel = ((float)diffRGBSum / sensorPixelCount / 255) });
                }
            }
            if (outputJSON)
            {
                var outputDto = new SensorOutputDto { ImageUrl = HttpContext.Current.Request.Url.OriginalString.Replace("outputJson=true", "outputJson=false"), ParkingSpaces = parkingSpacesOutputs.ToArray() };
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StringContent(js.Serialize(outputDto).Replace(@"\u0026", "&")); // JavaScriptSerializer messes up &
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return result;
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    outputImage.Save(ms, ImageFormat.Png);
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(ms.ToArray());
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    return result;
                }                
            }
        }

        // POST: api/Sensors
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Sensors/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sensors/5
        public void Delete(int id)
        {
        }
    }
}
