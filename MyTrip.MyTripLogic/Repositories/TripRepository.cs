using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTrip.MyTripLogic.Models;

namespace MyTrip.MyTripLogic.Repositories
{
    public class TripRepository
    {
        public IEnumerable<Trip> GetTrips()
        {
            return new List<Trip>()
            {
                new Trip() {Id=Guid.NewGuid() , Name  = "Name 1"},
                new Trip() {Id=Guid.NewGuid() , Name  = "Name 2"},
            };
        }
    }
}