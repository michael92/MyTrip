using Microsoft.Azure.Documents.Client;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

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

            foreach (var point in points)
            {
                Trace.TraceInformation("Converting point {0} ", point);

                var data = point.Split(' ');

                double latitude = Convert.ToDouble(data[0]);
                double longitutde = Convert.ToDouble(data[1]);

                route.points.Add(new Point {
                    city = this.GetPlaceName(latitude,longitutde),
                    latitude = latitude,
                    longitude = longitutde
                });
                
            }

            return route ;
        }

        private string GetPlaceName(double latitude, double longitutde)
        {
            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key=AIzaSyC0sQ1pLDtQM7atcDpfJfmjD2TWLYf-jn0", latitude,longitutde);
            string result = String.Empty;

            using (WebClient wc = new WebClient())
            {
                result = wc.DownloadString(url);
            }
            

            try
            {
                var googleResult = Rootobject.Deserialize(result);

                if (googleResult != null && googleResult.results.Count > 0)
                {
                    result = googleResult.results[0].formatted_address;
                }
            }
            catch(Exception)
            {
                Trace.TraceInformation("GoogleMaps Revert Geocoding exception");
            }

            return result;
        }
    }
}
