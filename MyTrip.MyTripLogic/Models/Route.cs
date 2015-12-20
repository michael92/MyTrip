using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Route
    {

        public Route()
        {
            this.points = new List<Point>();
        }

        public int id { get; set; }
        public List<Point> points { get; set; }
    }
}