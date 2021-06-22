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

        [Route("GetOfficerList")]
        public HttpResponseMessage GetOfficerList()
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            List<Crew.CrewOfficer> officers = Crew.CrewOfficer.OfficersForPlayer(playerId);
            string serializedElement = JsonConvert.SerializeObject(officers);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [Route("RegisterOfficerChanges")]
        public HttpResponseMessage RegisterOfficerChanges(Crew.CrewOfficer officer)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            string statsEditingResult = Crew.CrewOfficer.RegisterOfficerStatsChanged(playerId, officer);

            if(string.IsNullOrEmpty(statsEditingResult))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                return response;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.ReasonPhrase = statsEditingResult;
                return response;
            }
            
        }

        [Route("GetPlayerAsset")]
        public HttpResponseMessage GetPlayerAsset()
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
