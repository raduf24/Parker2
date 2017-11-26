using Parker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    public class HomeController : Controller
    {
        private SensorsController sensorsController = new SensorsController();
        public ActionResult Index()
        {
            ViewBag.Title = "Parker";
            var model = new HomeViewModel();
            model.Sensors = GetSensors();
            model.Sensors.First().IsSelected = true;
            ViewBag.Items = model.Sensors;
            return View(model);
        }

        public IList<Sensor> GetSensors()
        {
            string url = "http://localhost:49808/api/Sensors"; 
            return sensorsController.GetWithLocalizedUrl(url).Select(s => new Sensor {
                Name = (string)(s.GetType()).GetProperty("Name").GetValue(s, null),
                Url = (string)(s.GetType()).GetProperty("Url").GetValue(s, null),
                MapUrl = (string)(s.GetType()).GetProperty("MapUrl").GetValue(s, null),
                Coordinates = (string)(s.GetType()).GetProperty("Coordinates").GetValue(s, null),
                IsSelected = false
            }).ToList();
        }

        public static IEnumerable<SelectListItem> GetSensorsDropDown(IList<Sensor> sensors)
        {
            var dropDown = sensors.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Url
            });

            return dropDown;
        }
    }
}
