using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{
    public class Trip
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublic { get; set; }
    }
}