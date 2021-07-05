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
            if (rig != null)
            {
                if (rig.Id == 0)
                {
                    rig.SaveData(playerId, "");
                }
            }
            else
            {
                rig = SpaceshipRig.RigForBattle(playerId, battleSceneId);
            }
            
            battle = Battle.CreateBattle(playerId, battleSceneId, rig.Id, true);
            battle.Rig = rig;


            string serializedElement = JsonConvert.SerializeObject(battle);
            serializedElement = CommonFunctions.Compress(serializedElement);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [Route("StartBattleWithRigInDb")]
        public HttpResponseMessage StartBattleWithRigInDb(int battleSceneId)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            SpaceshipRig rig = SpaceshipRig.RigForBattle(playerId, battleSceneId);

            Battle battle = null;
            if (rig != null)
            {
                if (rig.Id == 0)
                {
                    rig.SaveData(playerId, "");
                }
            }
            else
            {
                rig = SpaceshipRig.RigForBattle(playerId, battleSceneId);
            }

            battle = Battle.CreateBattle(playerId, battleSceneId, rig.Id, true);
            battle.Rig = rig;


            string serializedElement = JsonConvert.SerializeObject(battle);
            serializedElement = CommonFunctions.Compress(serializedElement);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }

        [Route("RegisterRigCompleted")]
        public HttpResponseMessage RegisterRigCompleted(SpaceshipRig rig)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            if(rig != null)
            {
                rig.SaveData(playerId, "");
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }

        [Route("RigForBattle")]
        public HttpResponseMessage RigForBattle(int BattleSceneId)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            var rig = SpaceshipRig.RigForBattle(playerId, BattleSceneId);

            string serializedElement = JsonConvert.SerializeObject(rig);
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

        [Route ("RegisterBattleProgress")]
        public HttpResponseMessage RegisterBattleProgress(BattleProgressRegistration progress)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            return SaveBattleProgress(progress, playerId);

        }

        [Route("RegisterBattleCompleted")]
        public HttpResponseMessage RegisterBattleCompleted(BattleProgressRegistration progress)
        {
            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);

            var response = SaveBattleProgress(progress, playerId);
            if(!response.IsSuccessStatusCode)
            {
                return response;
            }

            Battle currentBattle = Battle.BattleByCode(progress.BattleCode);
            currentBattle.RegisterBattleCompleted(progress);

            return response;
        }

        private HttpResponseMessage SaveBattleProgress(BattleProgressRegistration progress, int playerId)
        {

            Battle currentBattle = Battle.BattleByCode(progress.BattleCode);
            if (currentBattle == null)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
                errorResponse.ReasonPhrase = "Battle not found";
                return errorResponse;
            }
            if (currentBattle.PlayerId != playerId)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.Forbidden);
                errorResponse.ReasonPhrase = "Battle ownership trouble";
                return errorResponse;
            }

            currentBattle.CurrentCycle = progress.CurrentCycle;
            currentBattle.CurrentStage = progress.CurrentStage;
            currentBattle.MaxOpenedCycle = progress.MaxOpenedCycle;
            currentBattle.MaxOpenedStage = progress.MaxOpenedStage;
            currentBattle.ShipOpenedSkills = progress.ShipSkills;
            currentBattle.ShipExperience = progress.battleExperience.Experience;
            currentBattle.ShipSkillPoints = progress.battleExperience.skillPoints;
            currentBattle.ShipNextSkillPointExperience = progress.battleExperience.nextSkillPointExperience;
            currentBattle.ShipTotalSkillPointsReceived = progress.battleExperience.totalSkillPointsReceived;
            currentBattle.ModuleSkillsJsonCompressed = progress.ModuleSkillsSerializedString;
            currentBattle.SaveData();
            currentBattle.GainedResources = progress.GainedResources;
            progress.GainedResources.SaveData(PlayerResources.StorageType.SpaceShip, currentBattle.RigId, playerId);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;

        }

    }
}
