using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    using Newtonsoft.Json;
    public class Media
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "TripId")]
        public string TripId { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public MediaStatus Status { get; set; }

        [JsonProperty(PropertyName = "Url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "ThumbnailUrl")]
        public string ThumbnailUrl { get; set; }

    }
}