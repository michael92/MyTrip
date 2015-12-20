using Microsoft.Azure.Documents.Client;
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

namespace MyTrip.MyTripLogic.Repositories
{
    public class MediaRepository
    {
   


        private readonly CloudStorageAccount storageAccount;
        public MediaRepository()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
        }


        public async Task CreatePhoto(string id, string url, string tripId, string thumbnailUrl, Stream inputStream)
        {
            DocumentDb photodb = new DocumentDb("MyTripDb", "photo");
            Media m = new Media();
            m.Id = id;
            m.Url = url;
            m.TripId = tripId;
            m.ThumbnailUrl = thumbnailUrl;
            m.Status = MediaStatus.Formatting;
            DocumentClient dc = photodb.getClient();

            var doc = await dc.CreateDocumentAsync(photodb.getCollection().SelfLink, m);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("photo");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(""+id);
            blockBlob.UploadFromStream(inputStream);
        }

        public async Task CreateMovie(string id, string url, string tripId, string thumbnailUrl, Stream inputStream)
        {
            DocumentDb moviedb = new DocumentDb("MyTripDb", "movie");
            Media m = new Media();
            m.Id = id;
            m.Url = url;
            m.TripId = tripId;
            m.ThumbnailUrl = thumbnailUrl;
            m.Status = MediaStatus.Formatting;
            DocumentClient dc = moviedb.getClient();

            var doc = await dc.CreateDocumentAsync(moviedb.getCollection().SelfLink, m);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("movie");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("" + id);
            blockBlob.UploadFromStream(inputStream);
        }




    }
}