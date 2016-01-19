using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;



namespace MediaGenerator.Generators
{
    public class GeneratePoster : IGenerate
    {
        public static string APIKey = "AIzaSyClxzgq0eDl9kteGNujCgTNljSIN0JyPps";
        public static string mapGoogle = @"http://maps.googleapis.com/maps/api/staticmap?";
        public void GenerateData(QueueMessage msg)
        {
            DocumentDb tripdb = new DocumentDb("MyTripDb", "trip");
            DocumentClient tripDBClient = tripdb.Client;
            Trip trip = tripDBClient.CreateDocumentQuery<Trip>(tripdb.Collection.DocumentsLink)
                       .AsEnumerable()
                       .Where(t => t.Id == msg.tripId)
                       .FirstOrDefault();
             
            
            DocumentDb photodb = new DocumentDb("MyTripDb", "photo");
            DocumentClient photoDBClient = photodb.Client;
            var photos = photoDBClient.CreateDocumentQuery<Media>(photodb.Collection.DocumentsLink)
                       .AsEnumerable()
                       .Where(t => t.Id == msg.tripId)
                       .ToList();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnectionString"));
            CloudBlobClient blobClientDownload = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClientDownload.GetContainerReference("photo");

           
            using (WebClient wc = new WebClient())
            {
                Stream mapStream = wc.OpenRead(GenerateMap(trip.Route));
                StreamReader sr = new StreamReader(mapStream);

                Image mapImage = Image.FromStream(sr.BaseStream);
                Bitmap b = new Bitmap(650, 700);
                using (Graphics g = Graphics.FromImage(b))
                {
                    //tlo
                    g.FillRectangle(Brushes.White, 0, 0, b.Width, b.Height);
                    //nazwa
                    g.DrawString(trip.Name, new Font(FontFamily.GenericSansSerif, 50, FontStyle.Regular), new SolidBrush(Color.Black), 15, 25);
                    //mapa
                    g.DrawImage(mapImage, 0, 100, 650, 300);
                    //miniaturki
                    for (int i = 1; i <= photos.Count; i++)
                    {
                        CloudBlockBlob blockBlobDownload = container.GetBlockBlobReference("thumbnail-" + photos[i].Id + ".png");
                        MemoryStream memoryStream = new MemoryStream();
                        Image thumbnails = Image.FromStream(memoryStream);
                        blockBlobDownload.DownloadToStream(memoryStream);
                        g.DrawImage(thumbnails, 10*i + (i-1) * 150, 450, 150, 150);
                    }
             


                    DocumentDb posterdb = new DocumentDb("MyTripDb", "poster");
                    DocumentClient posterDBClient = posterdb.Client;
                    Poster p = new Poster();
                    p.Id = Guid.NewGuid().ToString();
                    p.Url = "https://filmsphotos.blob.core.windows.net/photo/poster-" + p.Id;
                    p.TripId = trip.Id;
                    p.PosterStatus = PosterStatus.Generated;
                    p.CreationDate = DateTime.Now;

                    MemoryStream memoryStreamPoster = new MemoryStream();
                    b.Save(memoryStreamPoster, System.Drawing.Imaging.ImageFormat.Png);
                    CloudBlockBlob blockBlobUpload = container.GetBlockBlobReference("poster-" + p.Id);
                    blockBlobUpload.UploadFromByteArray(memoryStreamPoster.ToArray(), 0, (int)memoryStreamPoster.Length);

                    posterDBClient.CreateDocumentAsync(posterdb.Collection.SelfLink, p);
                }
               

            }
           
        }

        public string GenerateMap(Route r)
        {
            var point = "";
            for(int i=1;i<= r.points.Count;i++)
            {
                point += @"markers=size:mid%7Ccolor:0xff0000%7Clabel:"+i+ "%7C"+r.points[i].city + "&";
            }
            string url =
               mapGoogle
               + @"scale =false&size=600x300&maptype=roadmap&format=png&visual_refresh=true&"+ point +"key=" + APIKey;
            return "";
        }
    }
}
