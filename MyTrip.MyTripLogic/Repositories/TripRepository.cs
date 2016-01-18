using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.DB;

namespace MyTrip.MyTripLogic.Repositories
{
    public class TripRepository
    {
        public IEnumerable<Trip> GetTrips(int limit, int offset, string userId, bool isPublic)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.getClient();
            var result = dc.CreateDocumentQuery<Trip>(tripDb.getCollection().DocumentsLink)
                .AsEnumerable()
                .Where(t => t.UserId == userId && t.IsPublic == isPublic)
                .Select(t => new Trip { Id = t.Id, Name = t.Name, Description = t.Description })
                .OrderByDescending(t => t.Date)
                .Skip(offset)
                .Take(limit)
                .ToList();
            return result;
        }

        public Trip GetTrip(string id)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.getClient();
            var result = dc.CreateDocumentQuery<Trip>(tripDb.getCollection().DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == id)
                .FirstOrDefault();
            return result;
        }

        public async void EditTrip(string id, string name, string description, bool? isPublic = null)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.getClient();
            var trip = dc.CreateDocumentQuery<Trip>(tripDb.getCollection().DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == id)
                .FirstOrDefault();

            if (trip != null)
            {
                trip.Name = name ?? trip.Name;
                trip.Description = description ?? trip.Description;
                trip.IsPublic = isPublic ?? trip.IsPublic;
            }
            Document doc = tripDb.GetDocumentById(id);
            var client = tripDb.getClient();
            await client.ReplaceDocumentAsync(doc.SelfLink, trip);
        }

        public IEnumerable<Media> GetPhotosAndMovies(string tripId)
        {
            // TODO: Poprawić na implementację jak wyżej
            // Ta nie działa !!!
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