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
    public class PhotoConverter : IConverter
    {
        public void ConvertData(QueueMessage msg)
        {
            DocumentDb photodb = new DocumentDb("MyTripDb", "photo");
            DocumentClient dc = photodb.getClient();
            var photo = DocumentDb.GetMedia(msg.routeId).FirstOrDefault();
            MemoryStream memoryStream = new MemoryStream();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("photo");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(photo.Id);
            blockBlob.DownloadToStream(memoryStream);

            Image fullsizeImage = Image.FromStream(memoryStream);
            Image newImage = fullsizeImage.GetThumbnailImage(200, 200, null, IntPtr.Zero);
            MemoryStream myResult = new MemoryStream();
            newImage.Save(myResult, ImageFormat.Jpeg);


            CloudBlockBlob blockBlob2 = container.GetBlockBlobReference("thumbnail-" + photo.Id);
            blockBlob2.UploadFromByteArray(myResult.ToArray(), 0, (int)myResult.Length);
            photo.ThumbnailUrl = "https://mytripblob.blob.core.windows.net/photo/thumbnail-" + photo.Id;
            photo.Status= MediaStatus.Formatted;
            DocumentDb.UpdateItemAsync(photo);
        }

    }
}
