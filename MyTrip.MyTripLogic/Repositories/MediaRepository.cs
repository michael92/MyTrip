using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MyTrip.MyTripLogic.Repositories
{
    public class MediaRepository
    {
        private readonly CloudStorageAccount storageAccount;

        public MediaRepository()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
        }


        public async Task<Document> CreatePhoto(string id, string url, string tripId, string thumbnailUrl)
        {
            DocumentDb photodb = new DocumentDb("MyTripDb", "photo");
            Media m = new Media();
            m.Id = id;
            m.Url = url;
            m.TripId = tripId;
            m.ThumbnailUrl = thumbnailUrl;
            m.Status = MediaStatus.Formatting;
            DocumentClient dc = photodb.Client;

            return await dc.CreateDocumentAsync(photodb.Collection.SelfLink, m);
        }

        public void CreatePhotoInBlob(string id, Stream inputStream)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("photo");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
            blockBlob.UploadFromStream(inputStream);
        }

        public async Task<Document> CreateMovie(string id, string url, string tripId, string thumbnailUrl)
        {
            DocumentDb moviedb = new DocumentDb("MyTripDb", "movie");
            Media m = new Media();
            m.Id = id;
            m.Url = url;
            m.TripId = tripId;
            m.ThumbnailUrl = thumbnailUrl;
            m.Status = MediaStatus.Formatting;
            DocumentClient dc = moviedb.Client;

            return await dc.CreateDocumentAsync(moviedb.Collection.SelfLink, m);
        }

        public void CreateMovieInBlob(string id, Stream inputStream)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("movie");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
            blockBlob.UploadFromStream(inputStream);
        }

        public IEnumerable<Media> GetPhotos(string tripId)
        {
            DocumentDb documentDb = new DocumentDb("MyTripDb", "photo");
            DocumentClient dc = documentDb.Client;

            var photos = dc.CreateDocumentQuery<Media>(documentDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.TripId == tripId)
                .ToList();

            return photos;
        }

        public IEnumerable<Media> GetMovies(string tripId)
        {
            DocumentDb documentDb = new DocumentDb("MyTripDb", "movie");
            var dc = documentDb.Client;

            var movies = dc.CreateDocumentQuery<Media>(documentDb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.TripId == tripId)
                .ToList();

            return movies;
        }

        public async Task DeletePhoto(string photoId)
        {
            DocumentDb tripDB = new DocumentDb("MyTripDb", "photo");
            DocumentClient tripDBClient = tripDB.Client;

            var photo = tripDBClient.CreateDocumentQuery<Document>(tripDB.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == photoId)
                .FirstOrDefault();
            if (photo != null)
            {
                await tripDBClient.DeleteDocumentAsync(photo.SelfLink);
                DeletePhotoFromBlob(photo.GetPropertyValue<String>("Url"));
                DeletePhotoFromBlob(photo.GetPropertyValue<String>("ThumbnailUrl"));
                
                // Changed Trip Data - Delete Poster
                DeletePoster(photo.GetPropertyValue<String>("TripId"));
            }
        }

        public void DeletePhotoFromBlob(string blobName)
        {
            if (!string.IsNullOrEmpty(blobName))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("photo");
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                blob.DeleteIfExists();
            }
        }

        public async Task DeleteMovie(string movieId)
        {
            DocumentDb tripDB = new DocumentDb("MyTripDb", "movie");
            DocumentClient tripDBClient = tripDB.Client;

            var movie = tripDBClient.CreateDocumentQuery<Document>(tripDB.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.Id == movieId)
                .FirstOrDefault();
            if (movie != null)
            {
                await tripDBClient.DeleteDocumentAsync(movie.SelfLink);
                DeleteMovieFromBlob(movie.GetPropertyValue<String>("Url"));
                DeleteMovieFromBlob(movie.GetPropertyValue<String>("ThumbnailUrl"));

                // Changed Trip Data - Delete Poster
                DeletePoster(movie.GetPropertyValue<String>("TripId"));
            }
        }

        private void DeleteMovieFromBlob(string blobName)
        {
            if (!string.IsNullOrEmpty(blobName))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("movie");
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                blob.DeleteIfExists();
            }
        }

        public void SendPhotoToQueue(string photoId, string tripId)
        {
            QueueMessage qm = new QueueMessage();
            qm.tripId = tripId;
            qm.taskType = QueueTaskType.ConvertPhoto;
            qm.url = "https://filmsphotos.blob.core.windows.net/photo/" + photoId;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QueueConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("converter");
            queue.CreateIfNotExists();
            CloudQueueMessage message = QueueMessage.SerializeMessage(qm);
            queue.AddMessage(message);
        }

        public void SendMovieToQueue(string movieId, string tripId)
        {
            QueueMessage qm = new QueueMessage();
            qm.tripId = tripId;
            qm.taskType = QueueTaskType.ConvertMovie;
            qm.url = "https://filmsphotos.blob.core.windows.net/movie/" + movieId;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QueueConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("converter");
            queue.CreateIfNotExists();
            CloudQueueMessage message = QueueMessage.SerializeMessage(qm);
            queue.AddMessage(message);
        }

        public void DeletePoster(string tripId)
        {
            DocumentDb db = new DocumentDb("MyTripDb", "poster");
            DocumentClient dc = db.Client;

            var poster = dc.CreateDocumentQuery<Poster>(db.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.TripId == tripId)
                .FirstOrDefault();

            if (poster != null)
            {
                var doc = db.GetDocument(poster.Id);
                dc.DeleteDocumentAsync(doc.SelfLink).Wait();
                DeletePhotoFromBlob(poster.Url);
            }

        }

    }
}