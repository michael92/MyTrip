using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Media
    {
        public int Id { get; set; }

        public int TripId { get; set; }

        public string Status { get; set; }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

    }
}