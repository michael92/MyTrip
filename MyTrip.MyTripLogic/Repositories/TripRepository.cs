using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.DB;
using Microsoft.Azure.Documents.Client;

namespace MyTrip.MyTripLogic.Repositories
{
    public class TripRepository
    {
        public IEnumerable<Trip> GetTrips()
        {
            return new List<Trip>()
            {
                new Trip() {Id=1 , Date = DateTime.Now, IsPublic = true},
                new Trip() {Id=2 , Date = DateTime.Now.AddDays(-1), IsPublic = false},
            };
        }

        public Trip GetTrip(int id)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.getClient();
            return dc.CreateDocumentQuery<Trip>(new Uri(tripDb.getCollection().SelfLink)).Where(t => t.Id == id).FirstOrDefault();
        }

        public IEnumerable<Media> GetPhotosAndMovies(int tripId)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "photo");
            DocumentClient dc = tripDb.getClient();
            var photos = dc.CreateDocumentQuery<Media>(new Uri(tripDb.getCollection().SelfLink)).Where(t => t.TripId == tripId);
            tripDb = new DocumentDb("MyTripDb", "movie");
            dc = tripDb.getClient();
            var movies = dc.CreateDocumentQuery<Media>(new Uri(tripDb.getCollection().SelfLink)).Where(t => t.TripId == tripId);
            return photos.Concat(movies);
        }

    }
}