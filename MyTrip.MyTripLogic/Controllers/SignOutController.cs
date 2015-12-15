using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyTrip.MyTripLogic.Controllers
{
    public class SignOutController : ApiController
    {
        [HttpGet]
        public int SignOut(int i)
        {
            return i + 1;
        }
    }
}
