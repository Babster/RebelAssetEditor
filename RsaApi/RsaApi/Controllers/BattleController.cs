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
    [RoutePrefix("api/Battle")]
    public class BattleController : ApiController
    {

        [Route("StartBattle")]
        public HttpResponseMessage StartBattle(int battleSetId)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            PlayerAsset playerAsset = PlayerDataSql.GetPlayerAsset(playerId);
            string serializedElement = JsonConvert.SerializeObject(playerAsset);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

    }
}
