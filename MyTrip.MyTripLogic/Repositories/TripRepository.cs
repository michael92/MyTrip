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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

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
                .Where(t => (t.IsPublic == isPublic && t.UserId != userId))
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

            // Changed Trip Data - Delete Poster
            var mediaRepo = new MediaRepository();
            mediaRepo.DeletePoster(id);
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

            // Changed Trip Data - Delete Poster
            var mediaRepo = new MediaRepository();
            mediaRepo.DeletePoster(id);
        }

        public async Task<Poster> GetPosterByTripId(string tripId)
        {
            DocumentDb db = new DocumentDb("MyTripDb", "poster");
            DocumentClient dc = db.Client;

            var poster = dc.CreateDocumentQuery<Poster>(db.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.TripId == tripId)
                .FirstOrDefault();

            if (poster == null)
            {
                poster = await CreatePoster(tripId);
                AddGeneratingPosterToQueue(tripId);
            }

            return poster;
        }

        private async Task<Poster> CreatePoster(string tripId)
        {
            Poster poster = new Poster
            {
                TripId = tripId,
                PosterStatus = PosterStatus.Generating,
            };

            DocumentDb db = new DocumentDb("MyTripDb", "poster");
            DocumentClient dc = db.Client;
            var doc = await dc.CreateDocumentAsync(db.Collection.SelfLink, poster);

            poster.Id = doc.Resource.Id;
            return poster;
        }

        private void AddGeneratingPosterToQueue(string tripId)
        {
            QueueMessage qm = new QueueMessage();
            qm.tripId = tripId;
            qm.taskType = QueueTaskType.GeneratePoster;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QueueConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("generate");
            queue.CreateIfNotExists();
            CloudQueueMessage message = QueueMessage.SerializeMessage(qm);
            queue.AddMessage(message);
        }

    }
}