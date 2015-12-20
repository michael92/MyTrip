using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Point
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string city { get; set; }
    }
}