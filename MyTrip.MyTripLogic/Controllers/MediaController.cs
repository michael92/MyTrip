using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyTrip.MyTripLogic.Controllers
{
    [RoutePrefix("api/Media")]
    [Authorize]
    public class MediaController : ApiController
    {
        private readonly MediaRepository _repo;

        public MediaController()
        {
            _repo = new MediaRepository();
        }

        [HttpPost]
        [Route("addPhoto")]
        public async Task<IHttpActionResult> addPhoto([FromUri] string tripId)
        {

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    string id = Guid.NewGuid().ToString();
                    var document = await _repo.CreatePhoto(id, "https://mytripblob.blob.core.windows.net/photo/" + id, tripId, "https://mytripblob.blob.core.windows.net/photo/" + id);
                    _repo.CreatePhotoInBlob(id, hpf.InputStream);
                    _repo.SendPhotoToQueue(id, tripId);
                    
                    return Ok(id);
                }
            }

            return InternalServerError();
        }

        [HttpPost]
        [Route("addMovie")]
        public async Task<IHttpActionResult> addMovie([FromUri] string tripId)
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
                        string id = Guid.NewGuid().ToString();
                        var document = await _repo.CreateMovie(id, "https://mytripblob.blob.core.windows.net/movie/" + id, tripId, "https://mytripblob.blob.core.windows.net/movie/" + id);
                        _repo.CreateMovieInBlob(id, hpf.InputStream);
                        _repo.SendMovieToQueue(id, tripId);
                        
                        return Ok(id);
                    }
                }
            }

            return InternalServerError();
        }

        [HttpDelete]
        [Route("deletePhoto")]
        public async Task<IHttpActionResult> deletePhoto([FromUri] string photoId)
        {
            _repo.DeletePhoto(photoId);
            return Ok();
        }

        [HttpDelete]
        [Route("deleteMovie")]
        public async Task<IHttpActionResult> deleteMovie([FromUri] string movieId)
        {
            _repo.DeleteMovie(movieId);
            return Ok();
        }

        [HttpGet]
        [Route("getPhotos")]
        public IHttpActionResult GetPhotos([FromUri] string tripId)
        {
            return Ok(_repo.GetPhotos(tripId));
        }

        [HttpGet]
        [Route("getMovies")]
        public IHttpActionResult GetMovies([FromUri] string tripId)
        {
            return Ok(_repo.GetMovies(tripId));
        }

    }
}
