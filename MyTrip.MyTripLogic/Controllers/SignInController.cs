using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using JF.AspNet.Identity.DocumentDB;
using Microsoft.Build.Utilities;

namespace MyTrip.MyTripLogic.Controllers
{
    public class SignInController : ApiController
    {
        [HttpGet]
        public int SignIn(int i)
        {
            return i + 1;
        }
    }
}
