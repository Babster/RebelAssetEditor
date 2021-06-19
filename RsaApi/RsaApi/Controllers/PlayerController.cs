using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;

namespace RsaApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Player")]
    public class PlayerController : Controller
    {
        /*[HttpGet]
        [Route("GetStats")]
        public HttpResponseMessage GetStats()
        {

            if (!User.Identity.IsAuthenticated)
                return null;

            string userName = User.Identity.Name;
            int userId = CommonFunctions.UserId(User);

            //AdmiralStats admStats = new AdmiralStatsWithSql(userId);

            //Console.Wri

            var json = JsonConvert.SerializeObject(admStats);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json);
            return response;

        }*/
    }

}