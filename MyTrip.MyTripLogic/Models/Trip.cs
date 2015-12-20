using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Trip
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublic { get; set; }

        public Route Route { get; set; }

        public string UserId { get; set; }

        public RouteStatus RouteStatus { get; set; }

    }
}