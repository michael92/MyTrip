using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MyTrip.MyTripLogic.Models
{
    public class Rootobject
    {
        public List<Result> results { get; set; }
        public string status { get; set; }

        public Rootobject()
        {
            results = new List<Result>();
        }

        public static Rootobject Deserialize(string msg)
        {
            return new JavaScriptSerializer().Deserialize<Rootobject>(msg);
        }
    }

    public class Result
    {
        public List<Address_Components> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
        public bool partial_match { get; set; }

        public Result()
        {
            this.address_components = new List<Address_Components>();
            this.types = new List<string>();
        }
    }

    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport
    {
        public Northeast1 northeast { get; set; }
        public Southwest1 southwest { get; set; }
    }

    public class Northeast1
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest1
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }

        public Address_Components()
        {
            this.types = new List<string>();
        }
    }
}