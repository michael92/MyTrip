using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyTrip.MyTripLogic.Repositories;

namespace MyTrip.MyTripLogic.Controllers
{
    [RoutePrefix("api/Trips")]
    [Authorize]
    public class TripController : ApiController
    {
        private readonly TripRepository _repo;

        public TripController()
        {
            _repo = new TripRepository();
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_repo.GetTrips());
        }
    }
}
