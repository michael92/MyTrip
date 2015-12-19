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

namespace MyTrip.MyTripLogic.Controllers
{
    public class MediaController : ApiController
    {
        private readonly MediaRepository _repo;

        public MediaController()
        {
            _repo = new MediaRepository();
        }



        [HttpPost]
        public async Task<IHttpActionResult> addPhoto([FromUri] int id, [FromUri] int tripId)
        {

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    await _repo.CreatePhoto(id, "https://mytripblob.blob.core.windows.net/photo/" + id, tripId, "https://mytripblob.blob.core.windows.net/photo/" + id, hpf.InputStream);
                    return Ok();
                }
            }

            return InternalServerError();
        }

        [HttpPost]
        public async Task<IHttpActionResult> addMovie([FromUri] int id, [FromUri] int tripId)
        {

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {

                    if (hpf.ContentLength > 0)
                    {
                        await _repo.CreateMovie(id, "https://mytripblob.blob.core.windows.net/movie/" + id, tripId, "https://mytripblob.blob.core.windows.net/movie/" + id, hpf.InputStream);
                        return Ok();
                    }
                }
            }

            return InternalServerError();
        }



    }
}
