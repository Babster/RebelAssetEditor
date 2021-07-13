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
    [RoutePrefix("api/Station")]
    public class StationController : ApiController
    {

        [HttpGet]
        [Route("GetPlayerResources")]
        public HttpResponseMessage GetPlayerResources()
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            PlayerResources playerResources = new PlayerResources();
            playerResources.LoadData(PlayerResources.StorageType.MainWarehouse, 0, playerId);
            string serializedElement = JsonConvert.SerializeObject(playerResources);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [HttpGet]
        [Route("GetPlayerModules")]
        public HttpResponseMessage GetPlayerModules()
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            List<ShipModule> modules = ShipModule.PlayerModules(playerId);
            string serializedElement = JsonConvert.SerializeObject(modules);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

    }
}
