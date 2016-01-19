using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
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
    public class PhotoConverter : IConverter
    {
        public void ConvertData(QueueMessage msg)
        {
            DocumentDb photodb = new DocumentDb("MyTripDb", "photo");
            DocumentClient photoDBClient = photodb.Client;

            var photo = photoDBClient.CreateDocumentQuery<Media>(photodb.Collection.DocumentsLink)
                .AsEnumerable()
                .Where(t => t.TripId == msg.tripId && t.Url == t.ThumbnailUrl)
                .FirstOrDefault();

            photo.ThumbnailUrl = "https://filmsphotos.blob.core.windows.net/photo/thumbnail-" + photo.Id + ".png";
            photo.Url = "https://filmsphotos.blob.core.windows.net/photo/" + photo.Id + ".png";
            photo.Status = MediaStatus.Formatted;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnectionString"));
            CloudBlobClient blobClientDownload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClientDownload.GetContainerReference("photo");
            CloudBlockBlob blockBlobDownload = container.GetBlockBlobReference(photo.Id);

            MemoryStream memoryStream = new MemoryStream();
            blockBlobDownload.DownloadToStream(memoryStream);


            Image newImage = Image.FromStream(memoryStream);
            MemoryStream image = new MemoryStream();
            newImage.Save(image, ImageFormat.Png);
            CloudBlockBlob blockBlobUpload = container.GetBlockBlobReference(photo.Id + ".png");
            blockBlobUpload.UploadFromByteArray(image.ToArray(), 0, (int)image.Length);

            Image newThumbnailImage = newImage.GetThumbnailImage(200, 200, null, IntPtr.Zero);
            MemoryStream thumb = new MemoryStream();
            newThumbnailImage.Save(thumb, ImageFormat.Png);
            CloudBlockBlob blockBlobUploadThumb = container.GetBlockBlobReference("thumbnail-" + photo.Id + ".png");
            blockBlobUploadThumb.UploadFromByteArray(thumb.ToArray(), 0, (int)thumb.Length);

            var doc = photodb.GetDocument(photo.Id);
            photoDBClient.ReplaceDocumentAsync(doc.SelfLink, photo);
        }

    }
}
