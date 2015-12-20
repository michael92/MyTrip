using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Media
    {
        public string Id { get; set; }

        public string TripId { get; set; }

        public MediaStatus Status { get; set; }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

    }
}