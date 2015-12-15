using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyTrip.ClientApp.Controllers
{
    public class InsertPhotoOrFilmController : ApiController
    {
        [HttpGet]
        public int InsertTripRoute(int i)
        {
            return i + 1;
        }
    }
}
