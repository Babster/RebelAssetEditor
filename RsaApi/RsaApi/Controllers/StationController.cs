﻿using System;
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

        [HttpGet]
        [Route("GetPlayerShips")]
        public HttpResponseMessage GetPlayerShips()
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            List<Ship> ships = Ship.PlayerShips(playerId);
            string serializedElement = JsonConvert.SerializeObject(ships);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [HttpGet]
        [Route("GetStationInfo")]
        public HttpResponseMessage GetStationInfo()
        {
            string steamId = User.Identity.Name;
            PlayerData playerData = PlayerDataSql.GetPlayerData(steamId);
            if(playerData.StationOpened == 0)
            {
                var negativeResponse =  new HttpResponseMessage(HttpStatusCode.BadRequest);
                negativeResponse.ReasonPhrase = "Station is not opened yet";
                return negativeResponse;
            }
            int playerId = playerData.Id;
            Station station = new Station(playerId);
            string serializedElement = JsonConvert.SerializeObject(station);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

    }
}
