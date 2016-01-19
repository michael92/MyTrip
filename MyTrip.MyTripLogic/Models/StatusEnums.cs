using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public enum RouteStatus
    {
        Formatting,
        Success,
        InvalidFormat
    }

    public enum MediaStatus
    {
        Formatting,
        Formatted
    }

    public enum PosterStatus
    {
        Generating,
        Generated
    }
}