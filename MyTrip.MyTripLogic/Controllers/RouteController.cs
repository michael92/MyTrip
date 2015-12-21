using Microsoft.AspNet.Identity;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MyTrip.MyTripLogic.Controllers
{
    public class RouteController : ApiController
    {
        private readonly RouteRepository _repo;

        public RouteController()
        {
            _repo = new RouteRepository();
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> create([FromUri] string name, [FromUri] string description)
        {
            var userName = User.Identity.Name;
            var userId = User.Identity.GetUserId();

            var tripId = Guid.NewGuid().ToString();
            _repo.CreateTrip(userId, tripId, name, description);

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(hpf.InputStream))
                        {
                            string line = sr.ReadToEnd();
                            await _repo.Create(line, tripId);
                            return Ok(tripId);
                        }
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                }
            }

            return InternalServerError();
        }

      


    }






}
