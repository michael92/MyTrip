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
        public IEnumerable<Trip> GetTrips(int limit, int offset, string userId)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.getClient();
            var result = dc.CreateDocumentQuery<Trip>(new Uri(tripDb.getCollection().SelfLink))
                //.Where()
                .OrderBy(t => t.Date)
                .Skip(offset)
                .Take(limit)
                .ToList();

            return result;
        }

        public Trip GetTrip(string id)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.getClient();
            return dc.CreateDocumentQuery<Trip>(new Uri(tripDb.getCollection().SelfLink)).Where(t => t.Id == id).FirstOrDefault();
        }

        public IEnumerable<Media> GetPhotosAndMovies(string tripId)
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