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

        [HttpGet]
        [Route("BattleSceneList")]
        public HttpResponseMessage BattleSceneList()
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            List<Battle> playerBattles = Battle.BattlesForPlayer(playerId, true);
            string serializedElement = JsonConvert.SerializeObject(playerBattles);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }
        
        [Route("StartBattle")]
        public HttpResponseMessage StartBattle(SpaceshipRig rig, int battleSceneId)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            Battle battle = null;
            if(rig != null)
            {
                if (rig.Id == 0)
                {
                    rig.SaveData(playerId, "");
                    battle = Battle.CreateBattle(playerId, battleSceneId, rig.Id, true);
                    battle.Rig = rig;
                }
                else
                {

                }
            }

            string serializedElement = JsonConvert.SerializeObject(battle);
            serializedElement = CommonFunctions.Compress(serializedElement);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [Route("ContinueBattle")]
        public HttpResponseMessage ContinueBattle(int battleSceneId)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            Battle battle = Battle.BattleByTypeId(playerId, battleSceneId);
            BattleScene tBattle = new BattleScene(BattleSceneType.SceneById(battleSceneId));
            battle.battleScene = new UnityBattleScene(tBattle);
            battle.Rig = SpaceshipRig.RigById(battle.RigId);

            string serializedElement = JsonConvert.SerializeObject(battle);
            serializedElement = CommonFunctions.Compress(serializedElement);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [Route ("RegisterStageCompleted")]
        public HttpResponseMessage RegisterStageCompleted()
        {

            return null;
        }

    }
}
