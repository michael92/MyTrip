using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyTrip.ClientApp.MyTripWebApi.EndPoints
{
    public class CreateTripPosterController : ApiController
    {
        [HttpGet]
        public int CreateTripPoster(int i)
        {
            return i + 1;
        }
    }
}
