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

        private readonly DocumentDb db;

        public RouteRepository()
        {
            db = new DocumentDb("MyTripDb", "unformattedroutes");
        }

        public async Task Create(int id, String line, int tripId)
        {
            UnformattedRoute ur = new UnformattedRoute();
            ur.Id = id;
            ur.Route = line;
            ur.TripId = tripId;
            DocumentClient dc = db.getClient();
            
             var doc = await dc.CreateDocumentAsync(db.getCollection().SelfLink, ur);
        }


    }
}