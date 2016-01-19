using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Drawing;

using System.IO;
using System.Linq;

namespace MediaGenerator.Generators
{
    public class GeneratePoster : IGenerate
    {
        public void GenerateData(QueueMessage msg)
        {
            DocumentDb tripdb = new DocumentDb("MyTripDb", "trip");
            DocumentClient tripDBClient = tripdb.getClient();

            var trip = tripDBClient.CreateDocumentQuery<Media>(new Uri(tripdb.getCollection().SelfLink)).Where(t => t.Id == msg.tripId).FirstOrDefault();
            //todo: dodać generowanie plakatu



            tripDBClient.ReplaceDocumentAsync(new Uri(tripdb.getCollection().SelfLink), trip);
        }
    }
}
