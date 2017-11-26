using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parker.Models
{
    public class HomeViewModel
    {
        public IList<Sensor> Sensors { get; set; }
    }

    public class Sensor
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string MapUrl { get; set; }
        public string Coordinates { get; set; }
        public bool IsSelected { get; set; }
    }
}