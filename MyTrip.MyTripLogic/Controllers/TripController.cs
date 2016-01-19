﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using MyTrip.MyTripLogic.Repositories;
using MyTrip.MyTripLogic.Models;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MyTrip.MyTripLogic.Controllers
{
    [RoutePrefix("api/Trip")]
    [Authorize]
    public class TripController : ApiController
    {
        private readonly TripRepository _repo;

        public TripController()
        {
            _repo = new TripRepository();
        }

        [Route("")]
        public IHttpActionResult Get([FromUri] int limit, [FromUri] int offset, [FromUri] bool isPublic)
        {
            var name = User.Identity.Name;
            var userId = User.Identity.GetUserId();
            return Ok(_repo.GetTrips(limit, offset, userId, isPublic));
        }

        [Route("getTripInfo")]
        public IHttpActionResult GetTrip([FromUri] string id)
        {
            return Ok(_repo.GetTrip(id));
        }

        [Route("editTrip")]
        public IHttpActionResult EditTrip([FromUri] string id, [FromUri] string name = null, [FromUri] string description = null, [FromUri] bool? isPublic = null)
        {
            //TODO
            return Ok();
        }

        [Route("generatePoster")]
        public IHttpActionResult GeneratePoster([FromUri] string tripId)
        {
            QueueMessage qm = new QueueMessage();
            qm.tripId = tripId;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QueueConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("posterQueue");
            queue.CreateIfNotExists();
            CloudQueueMessage message = QueueMessage.SerializeMessage(qm);
            queue.AddMessage(message);
            return Ok();
        }

    }
}
