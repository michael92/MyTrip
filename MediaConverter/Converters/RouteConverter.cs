using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.Repositories;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace MediaConverter.Converters
{
    public class RouteConverter : IConverter
    {
        public static string APIKey = "AIzaSyC0sQ1pLDtQM7atcDpfJfmjD2TWLYf-jn0";
        public void ConvertData(QueueMessage msg)
        {
            Trace.TraceError("Connecting to documentdb unformattedroute ");
            DocumentDb unfrouteDB = new DocumentDb("MyTripDb", "unformattedroutes");
            DocumentClient unfrouteClient = unfrouteDB.Client;

            Trace.TraceError("Connecting to documentdb trip");
            DocumentDb tripDB = new DocumentDb("MyTripDb", "trip");
            DocumentClient tripDBClient = tripDB.Client;
            TripRepository tripRepo = new TripRepository();

            Trace.TraceError("Getting trip {0} ",msg.tripId);
            var trip = tripRepo.GetTrip(msg.tripId);
            Document doc = tripDB.GetDocument(trip.Id);

            try
            {
                Trace.TraceError("Getting unformatted route {0} ", msg.tripId);
                UnformattedRoute unfroute = unfrouteClient.CreateDocumentQuery<UnformattedRoute>(unfrouteDB.Collection.DocumentsLink)
                    .AsEnumerable()
                    .Where(t => t.Id == msg.routeId)
                    .FirstOrDefault();

                if (unfroute != null)
                {
                    Gpx gpx = this.GetGpxRoute(unfroute);
                    Route route = null;

                    if (gpx != null)
                    {
                        Trace.TraceInformation("GPX route format {0})}", msg.routeId);
                        route = this.ParseGpxRoute(gpx,unfroute);
                    }
                    else
                    {
                        route = this.ParseRoute(unfroute);
                    }
                    trip.Route = route;
                    trip.RouteStatus = RouteStatus.Success;
                    tripDBClient.ReplaceDocumentAsync(doc.SelfLink, trip).Wait();
                }
            }
            catch(Exception e)
            {
                Trace.TraceError("Failed to proccess unformattedroute {0} {1}", msg.routeId,e.ToString());
                trip.RouteStatus = RouteStatus.InvalidFormat;
                tripDBClient.ReplaceDocumentAsync(doc.SelfLink, trip).Wait();
            }
        }

        private Gpx GetGpxRoute(UnformattedRoute route)
        {
            try
            {
                return Gpx.DeserializeGPX(route.Route);
            }
            catch(Exception e)
            {
                Trace.TraceError("Failed to parse GPX file {0} for unformattedRoute: {1}", e.ToString(), route.Id);
                return null;
            }
        }

        private Route ParseRoute(UnformattedRoute unfroute)
        {
            var points = unfroute.Route.Split(';');
            Route route = new Route {id = unfroute.Id };

            foreach (var point in points)
            {
                Trace.TraceError("Converting point {0} ", point);

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

        private Route ParseGpxRoute(Gpx gpxRoute, UnformattedRoute unfroute)
        {
            Route route = new Route { id = unfroute.Id };

            foreach (var point in gpxRoute.trk.trkseg)
            {
                Trace.TraceError("Converting GPX point {0} ", point);

                double latitude = Convert.ToDouble(point.lat);
                double longitutde = Convert.ToDouble(point.lon);

                route.points.Add(new Point
                {
                    city = this.GetPlaceName(latitude, longitutde),
                    latitude = latitude,
                    longitude = longitutde
                });
            }

            return route;
        }

        private string GetPlaceName(double latitude, double longitutde)
        {
            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}", latitude,longitutde,APIKey);
            string result = String.Empty;

            Trace.TraceError("Getting data from Google Geocodie GPX point");
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
                Trace.TraceError("GoogleMaps Revert Geocoding exception");
            }

            return result;
        }
    }
}
