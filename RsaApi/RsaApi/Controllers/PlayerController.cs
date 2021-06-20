using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace RsaApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Player")]
    public class PlayerController : ApiController
    {

        [HttpGet]
        [Route("GetNextStoryFlowObject")]
        public HttpResponseMessage GetNextStoryFlowObject()
        {
            string steamId = User.Identity.Name;
            StringAndInt currentElement = PlayerDataSql.NextStoryObject(steamId);
            string serializedElement = JsonConvert.SerializeObject(currentElement);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [Route("RegisterStoryElementCompleted")]
        public HttpResponseMessage RegisterStoryElementCompleted()
        {
            string steamId = User.Identity.Name;
            StringAndInt currentElement = PlayerDataSql.RegisterStoryElementCompleted(steamId);
            string serializedElement = JsonConvert.SerializeObject(currentElement);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

    }
}
