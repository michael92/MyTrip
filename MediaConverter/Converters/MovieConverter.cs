using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace MediaConverter.Converters
{
    public class MovieConverter : IConverter
    {
        public void ConvertData(QueueMessage msg)
        {
            DocumentDb moviedb = new DocumentDb("MyTripDb", "movie");
            DocumentClient movieDBClient = moviedb.getClient();

            var movie = movieDBClient.CreateDocumentQuery<Media>(new Uri(moviedb.getCollection().SelfLink)).Where(t => t.Id == msg.tripId && t.Url == t.ThumbnailUrl).FirstOrDefault();
            // movie.ThumbnailUrl = "thumbnail-" + movie.Id + ".png";
            movie.ThumbnailUrl = null;
            movie.Url = "https://mytripblob.blob.core.windows.net/movie/" + movie.Id + ".avi";
            movie.Status = MediaStatus.Formatted;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnectionString"));
            CloudBlobClient blobClientDownload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClientDownload.GetContainerReference("movie");
            CloudBlockBlob blockBlobDownload = container.GetBlockBlobReference(movie.Id);

            MemoryStream memoryStream = new MemoryStream();
            blockBlobDownload.DownloadToStream(memoryStream);

            //todo: konwersja do .avi
        
            CloudBlockBlob blockBlobUpload = container.GetBlockBlobReference(movie.Id + ".avi");
            blockBlobUpload.UploadFromByteArray(memoryStream.ToArray(), 0, (int)memoryStream.Length);


            //todo: dodać miniaturkę

            //Image newThumbnailImage = newImage.GetThumbnailImage(200, 200, null, IntPtr.Zero);
            //MemoryStream thumb = new MemoryStream();
            //newThumbnailImage.Save(thumb, ImageFormat.Png);
            //CloudBlockBlob blockBlobUploadThumb = container.GetBlockBlobReference(photo.ThumbnailUrl);
            //blockBlobUploadThumb.UploadFromByteArray(thumb.ToArray(), 0, (int)thumb.Length);



            movieDBClient.ReplaceDocumentAsync(new Uri(moviedb.getCollection().SelfLink), movie);
        }
    }
}
