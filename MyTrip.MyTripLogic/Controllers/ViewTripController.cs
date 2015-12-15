using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyTrip.MyTripLogic.Controllers
{
    public class ViewTripController : ApiController
    {
        [HttpGet]
        public int ViewTrip(int i)
        {
            return i + 1;
        }
    }
}
