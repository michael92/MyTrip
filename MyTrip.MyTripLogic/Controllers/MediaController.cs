using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyTrip.MyTripLogic.Controllers
{
    [RoutePrefix("api/Media")]
    [Authorize]
    public class MediaController : ApiController
    {
        private readonly MediaRepository _repo;

        public MediaController()
        {
            _repo = new MediaRepository();
        }

        [HttpPost]
        [Route("addPhoto")]
        public async Task<IHttpActionResult> addPhoto([FromUri] string id, [FromUri] string tripId)
        {

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    await _repo.CreatePhoto(id, "https://mytripblob.blob.core.windows.net/photo/" + id, tripId, "https://mytripblob.blob.core.windows.net/photo/" + id, hpf.InputStream);
                    QueueMessage qm = new QueueMessage();
                    qm.tripId = tripId;
                    qm.taskType = QueueTaskType.ConvertPhoto;
                    qm.url = "https://mytripblob.blob.core.windows.net/photo/" + id;
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QueueConnectionString"]);
                    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                    CloudQueue queue = queueClient.GetQueueReference("converter");
                    queue.CreateIfNotExists();
                    CloudQueueMessage message = QueueMessage.SerializeMessage(qm);
                    queue.AddMessage(message);
                    return Ok();
                }
            }

            return InternalServerError();
        }

        [HttpPost]
        [Route("addMovie")]
        public async Task<IHttpActionResult> addMovie([FromUri] string id, [FromUri] string tripId)
        {

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {

                    if (hpf.ContentLength > 0)
                    {
                        await _repo.CreateMovie(id, "https://mytripblob.blob.core.windows.net/movie/" + id, tripId, "https://mytripblob.blob.core.windows.net/movie/" + id, hpf.InputStream);
                        QueueMessage qm = new QueueMessage();
                        qm.tripId = tripId;
                        qm.taskType = QueueTaskType.ConvertMovie;
                        qm.url = "https://mytripblob.blob.core.windows.net/movie/" + id;
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QueueConnectionString"]);
                        CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                        CloudQueue queue = queueClient.GetQueueReference("converter");
                        queue.CreateIfNotExists();
                        CloudQueueMessage message = QueueMessage.SerializeMessage(qm);
                        queue.AddMessage(message);
                        return Ok();
                    }
                }
            }

            return InternalServerError();
        }

        [HttpDelete]
        [Route("deletePhoto")]
        public async Task<IHttpActionResult> deletePhoto([FromUri] string photoId)
        {

            DocumentDb tripDB = new DocumentDb("MyTripDb", "photo");
            DocumentClient tripDBClient = tripDB.Client;

            var photo = tripDBClient.CreateDocumentQuery<Document>(new Uri(tripDB.Collection.SelfLink)).Where(t => t.Id == photoId).FirstOrDefault();
            if (photo != null)
            {
                await tripDBClient.DeleteDocumentAsync(photo.SelfLink);
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("photo");
            CloudBlockBlob blob = container.GetBlockBlobReference(photo.GetPropertyValue<String>("Url"));
            blob.DeleteIfExists();
            return Ok();
        }

        [HttpDelete]
        [Route("deleteMovie")]
        public async Task<IHttpActionResult> deleteMovie([FromUri] string movieId)
        {
            DocumentDb tripDB = new DocumentDb("MyTripDb", "movie");
            DocumentClient tripDBClient = tripDB.Client;

            var movie = tripDBClient.CreateDocumentQuery<Document>(new Uri(tripDB.Collection.SelfLink)).Where(t => t.Id == movieId).FirstOrDefault();
            if(movie!= null)
            {
                await tripDBClient.DeleteDocumentAsync(movie.SelfLink);
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("movie");
            CloudBlockBlob blob = container.GetBlockBlobReference(movie.GetPropertyValue<String>("Url"));
            blob.DeleteIfExists();
            return Ok();
        }

        [HttpGet]
        [Route("getPhotos")]
        public IHttpActionResult GetPhotos([FromUri] string tripId)
        {
            return Ok(_repo.GetPhotos(tripId));
        }

        [HttpGet]
        [Route("getMovies")]
        public IHttpActionResult GetMovies([FromUri] string tripId)
        {
            return Ok(_repo.GetMovies(tripId));
        }

    }
}
