using Microsoft.Azure.Documents.Client;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Linq;

namespace MediaGenerator.Generators
{
    public class GeneratePoster : IGenerate
    {
        public void GenerateData(QueueMessage msg)
        {
            DocumentDb tripdb = new DocumentDb("MyTripDb", "trip");
            DocumentClient tripDBClient = tripdb.Client;
            
            var trip = tripDBClient.CreateDocumentQuery<Media>(new Uri(tripdb.Collection.SelfLink)).Where(t => t.Id == msg.tripId).FirstOrDefault();
            //todo: dodać generowanie plakatu



            tripDBClient.ReplaceDocumentAsync(new Uri(tripdb.Collection.SelfLink), trip);
        }
    }
}
