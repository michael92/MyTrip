using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Poster
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DateTime CreationDate { get; set; }

        public PosterStatus PosterStatus { get; set; }

        public string TripId { get; set; }

        public string Url { get; set; }

      

    }
}