using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using MyTrip.MyTripLogic.Repositories;

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
        public IHttpActionResult Get([FromUri] int limit, [FromUri] int offset)
        {
            var name = User.Identity.Name;
            var userId = User.Identity.GetUserId();
            return Ok(_repo.GetTrips(limit, offset, userId));
        }

        [Route("getTripInfo")]
        public IHttpActionResult GetTrip([FromUri] string id)
        {
            return Ok(_repo.GetTrip(id));
        }

        [Route("getMediaList")]
        public IHttpActionResult GetPhotosAndMovies([FromUri] string tripId)
        {
            return Ok(_repo.GetPhotosAndMovies(tripId));
        }

    }
}
