﻿using MyTrip.MyTripLogic.Models;
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

        public async Task CreateTrip(string userId, string tripId, string name, string description)
        {
            Trip trip = new Trip
            {
                Id = tripId,
                Date = DateTime.Now.ToLocalTime(),
                IsPublic = false,
                RouteStatus = RouteStatus.Formatting,
                UserId = userId,
                Name = name,
                Description = description
            };

            DocumentClient dc = tripDb.getClient();
            var doc = await dc.CreateDocumentAsync(tripDb.getCollection().SelfLink, trip);
        }

        public async Task Create(string line, string tripId)
        {
            UnformattedRoute ur = new UnformattedRoute();
            ur.Id = Guid.NewGuid().ToString();
            ur.Route = line;
            ur.TripId = tripId;
            DocumentClient dc = unformattedRouteDb.getClient();
            
            var doc = await dc.CreateDocumentAsync(unformattedRouteDb.getCollection().SelfLink, ur);
        }


    }
}