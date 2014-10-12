using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ImageServer.Controllers
{
    [RoutePrefix("v1/Event")]
    public class EventController : ApiController
    {
        [Route("FindEvents/{limit?}")]
        [HttpGet]
        public HttpResponseMessage FindEvents(int limit = 100)
        {
            bool res = EventProcessor.GetSongkickEvents(limit);
            HttpResponseMessage response = new HttpResponseMessage();
            if (res)
                response.StatusCode = HttpStatusCode.Found;
            else
                response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }
    }
}