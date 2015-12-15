using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyTrip.ClientApp.Controllers
{
    public class EditRoutePointsController : ApiController
    {
        [HttpGet]
        public int EditRoutePoints(int i)
        {
            return i + 1;
        }
    }
}
