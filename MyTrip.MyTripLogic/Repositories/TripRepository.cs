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
                new Trip() {Id=Guid.NewGuid() , Date = DateTime.Now, IsPublic = true},
                new Trip() {Id=Guid.NewGuid() , Date = DateTime.Now.AddDays(-1), IsPublic = false},
            };
        }
    }
}