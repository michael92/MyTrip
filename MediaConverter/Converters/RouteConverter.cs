using MediaConverter.Models;
using Microsoft.Azure.Documents.Client;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace MediaConverter.Converters
{
    public class RouteConverter : IConverter
    {
        public void ConvertData(QueueMessage msg)
        {
            DocumentDb unfrouteDB = new DocumentDb("MyTripDb", "unformattedroute");
            DocumentClient unfrouteClient = unfrouteDB.getClient();

            DocumentDb tripDB = new DocumentDb("MyTripDb", "trip");
            DocumentClient tripDBClient = tripDB.getClient();

            var trip = tripDBClient.CreateDocumentQuery<Trip>(new Uri(tripDB.getCollection().SelfLink)).Where(t => t.Id == msg.tripId).FirstOrDefault();
            
            try
            {
                UnformattedRoute unfroute = unfrouteClient.CreateDocumentQuery<UnformattedRoute>(new Uri(unfrouteDB.getCollection().SelfLink))
                    .Where(t => t.Id == msg.routeId).FirstOrDefault();

                if (unfroute != null)
                {
                    Route route = this.ParseRoute(unfroute);
                    trip.Route = route;
                    tripDBClient.ReplaceDocumentAsync(new Uri(tripDB.getCollection().SelfLink), trip);
                }
            }
            catch(Exception e)
            {
                Trace.TraceInformation("Failed to proccess unformattedroute {0} {1}", msg.routeId,e.ToString());
            }
        }

        private Route ParseRoute(UnformattedRoute unfroute)
        {
            var points = unfroute.Route.Split(';');
            Route route = new Route {id = unfroute.Id };
            int i = 0;

            foreach (var point in points)
            {
                Trace.TraceInformation("Converting point {0} ", point);

                var data = point.Split(' ');

                double latitude = Convert.ToDouble(data[0]);
                double longitutde = Convert.ToDouble(data[1]);

                route.points.Add(new Point { city = data[2],
                    latitude = latitude,
                    longitude = longitutde
                });

                i++;
            }

            return route ;
        }
    }
}
