using System;
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
using System.Net.Http.Formatting;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

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
            _repo.EditTrip(id, name, description, isPublic);
            return Ok();
        }
        
        [Route("editRoute")]
        public IHttpActionResult EditRoute([FromUri]string id, [FromBody]Route route)
        {
            if (route != null)
            {
                _repo.EditRoute(id, route);
                return Ok();
            }
            else
            {
                return InternalServerError(new Exception("Invalid body. Parameter route is required."));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getPoster")]
        public async Task<IHttpActionResult> GetPoster([FromUri] string tripId)
        {
            var poster = await _repo.GetPosterByTripId(tripId);
            return Ok(poster);
        }

    }
}
