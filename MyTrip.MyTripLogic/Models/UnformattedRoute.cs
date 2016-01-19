using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class UnformattedRoute
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string TripId { get; set; }

        public string Route { get; set; }

    }
}