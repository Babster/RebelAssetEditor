using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RsaApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Officers")]
    public class OfficerController : ApiController
    {

        [Route("UpdateOfficerSkillsets")]
        public HttpResponseMessage UpdateOfficerSkillsets(Crew.OfficerExperience officerExperience)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            //Внимание! Данные залетают в базу без какой-либо проверки.
            officerExperience.SaveData();

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }

    }
}
