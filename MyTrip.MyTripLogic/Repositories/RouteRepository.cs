using MyTrip.MyTripLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JF.AspNet.Identity.DocumentDB;
using Microsoft.AspNet.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MyTrip.MyTripLogic.DB;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyTrip.MyTripLogic.Repositories
{
    public class RouteRepository
    {

        private DocumentDb unformattedRouteDb;
        private DocumentDb tripDb;

        public RouteRepository()
        {
            unformattedRouteDb = new DocumentDb("MyTripDb", "unformattedroutes");
            tripDb = new DocumentDb("MyTripDb", "trip");
        }

        public async Task<Document> CreateTrip(string userId, string name, string description, bool isPublic)
        {
            Trip trip = new Trip
            {
                Date = DateTime.Now.ToLocalTime(),
                IsPublic = isPublic,
                RouteStatus = RouteStatus.Formatting,
                UserId = userId,
                Name = name,
                Description = description
            };

            tripDb = new DocumentDb("MyTripDb", "trip");
            DocumentClient dc = tripDb.Client;
            return await dc.CreateDocumentAsync(tripDb.Collection.SelfLink, trip);
        }

        public async Task<Document> CreateUnformattedRoute(string line, string tripId)
        {
            UnformattedRoute ur = new UnformattedRoute();
            ur.Route = line;
            ur.TripId = tripId;

            unformattedRouteDb = new DocumentDb("MyTripDb", "unformattedroutes");
            DocumentClient dc = unformattedRouteDb.Client;
            
            return await dc.CreateDocumentAsync(unformattedRouteDb.Collection.SelfLink, ur);
        }


    }
}