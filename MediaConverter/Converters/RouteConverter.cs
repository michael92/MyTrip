using MediaConverter.Models;
using Microsoft.Azure.Documents.Client;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Linq;

namespace MediaConverter.Converters
{
    public class RouteConverter : IConverter
    {
        public void ConvertData(QueueMessage msg)
        {
            //TODO: switch to repo
            DocumentDb unfrouteDB = new DocumentDb("MyTripDb", "unformattedroute");
            DocumentClient dc = unfrouteDB.getClient();
            UnformattedRoute unfroute = dc.CreateDocumentQuery<UnformattedRoute>(new Uri(unfrouteDB.getCollection().SelfLink))
                .Where(t => t.Id == msg.routeId).FirstOrDefault();
           
            if(unfroute != null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
