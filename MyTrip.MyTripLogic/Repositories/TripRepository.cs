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
            DocumentClient dc = tripDb.Client;
            if (isPublic)
            {
                var result = dc.CreateDocumentQuery<Trip>(tripDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.IsPublic == isPublic)
                .Select(t => new Trip { Id = t.Id, Name = t.Name, Description = t.Description })
                .OrderByDescending(t => t.Date)
                .Skip(offset)
                .Take(limit)
                .ToList();
                return result;
            }
            else
            {
                var result = dc.CreateDocumentQuery<Trip>(tripDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.UserId == userId)
                .Select(t => new Trip { Id = t.Id, Name = t.Name, Description = t.Description })
                .OrderByDescending(t => t.Date)
                .Skip(offset)
                .Take(limit)
                .ToList();
                return result;
            }
        }

        public Trip GetTrip(string id)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.Client;
            var result = dc.CreateDocumentQuery<Trip>(tripDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == id)
                .FirstOrDefault();
            return result;
        }

        public void EditTrip(string id, string name, string description, bool? isPublic = null)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.Client;
            var trip = dc.CreateDocumentQuery<Trip>(tripDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == id)
                .FirstOrDefault();

            if (trip != null)
            {
                trip.Name = name ?? trip.Name;
                trip.Description = description ?? trip.Description;
                trip.IsPublic = isPublic ?? trip.IsPublic;
            }
            Document doc = tripDb.GetDocument(id);
            var client = tripDb.Client;
            client.ReplaceDocumentAsync(doc.SelfLink, trip).Wait();
        }

        public void EditRoute(string id, Route route)
        {
            DocumentDb tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.Client;
            var trip = dc.CreateDocumentQuery<Trip>(tripDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == id)
                .FirstOrDefault();

            if (trip != null)
            {
                trip.Route = route;
            }
            Document doc = tripDb.GetDocument(id);
            var client = tripDb.Client;
            client.ReplaceDocumentAsync(doc.SelfLink, trip).Wait();
        }

    }
}