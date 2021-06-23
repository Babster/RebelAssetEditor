using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;

public class Battle : UnityBattle
{

    public Battle() { }

    public Battle(SqlDataReader r)
    {
        Id = (int)r["id"];
        BattleCode = (Guid)r["battle_code"];
        PlayerId = (int)r["player_id"];
        Ongoing = (int)r["ongoing"];
        BattleSceneTypeId = (int)r["battle_scene_type_id"];
        RigId = (int)r["rig_id"];
        DateStart = (DateTime)r["date_start"];
        if(r["date_complete"] != DBNull.Value)
        {
            DateComplete = (DateTime)r["date_complete"];
        }
        Successfull = (int)r["successfull"];
        CurrentCycle = (int)r["current_cycle"];
        CurrentStage = (int)r["current_stage"];
        MaxOpenedCycle = (int)r["max_opened_cycle"];
        MaxOpenedStage = (int)r["max_opened_stage"];
        ShipExperience = (int)r["ship_experience"];
        ShipOpenedSkills = (string)r["ship_opened_skills"];
    }

    public void SaveData()
    {
        string q;
        if(Id == 0)
        {
            BattleCode = Guid.NewGuid();
            q = $@"INSERT INTO battles(battle_code) VALUES(CAST('{BattleCode.ToString()}' AS uniqueidentifier))
                    SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"UPDATE battles SET 
                battle_code = {BattleCode},
                player_id = {PlayerId},
                ongoing = {Ongoing},
                battle_scene_type_id = {BattleSceneTypeId},
                rig_id = {RigId},
                date_start = {DataConnection.DateToSqlString(DateStart)},
                date_complete = {DataConnection.DateToSqlString(DateComplete)},
                successfull = {Successfull},
                current_cycle = {CurrentCycle},
                current_stage = {CurrentStage},
                max_opened_cycle = {MaxOpenedCycle},
                max_opened_stage = {MaxOpenedStage},
                ship_experience = {ShipExperience},
                ship_opened_skills = @str1
            WHERE
                id = {Id}";
        List<string> names = new List<string> { ShipOpenedSkills };
        DataConnection.Execute(q, names);
    }

    private static string BattleQuery()
    {
        return $@"
            SELECT
                id,
                battle_code,
                player_id,
                ongoing int,
                battle_scene_type_id,
                rig_id,
                date_start,
                date_complete,
                successfull,
                current_cycle,
                current_stage,
                max_opened_cycle,
                max_opened_stage,
                ship_experience,
                ship_opened_skills
            FROM
                battles";
    }

    private static Dictionary<Guid, Battle> BattleCache;
    public static List<Battle> BattlesForPlayer(int PlayerId, bool OnlyOngoing)
    {
        if(BattleCache == null)
        {
            BattleCache = new Dictionary<Guid, Battle>();
        }
        List<Battle> playerBattles = new List<Battle>();
        string q = BattleQuery() + $" WHERE player_id = {PlayerId}";
        if(OnlyOngoing)
        {
            q += " AND ongoing = 1";
        }
        SqlDataReader r = DataConnection.GetReader(q);
        while(r.Read())
        {
            Battle curBattle = new Battle(r);
            if(!BattleCache.ContainsKey(curBattle.BattleCode))
            {
                BattleCache.Add(curBattle.BattleCode, curBattle);
            }
            playerBattles.Add(curBattle);
        }
        return playerBattles;
    }
    public static Battle BattleByCode(Guid code)
    {
        if(BattleCache.ContainsKey(code))
        {
            return BattleCache[code];
        }
        else
        {
            Battle curBattle = null;
            string q = BattleQuery() + $@" WHERE battle_code = CAST('{code.ToString()}' AS uniqueidentifier)";
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                r.Read();
                curBattle = new Battle(r);
                BattleCache.Add(curBattle.BattleCode, curBattle);
            }
            r.Close();
            return curBattle;
        }
    }
    public static Battle BattleByTypeId(int playerId, int battleSceneId)
    {
        List<Battle> battlesForPlayer = BattlesForPlayer(playerId, true);
        foreach(var battle in battlesForPlayer)
        {
            if(battle.BattleSceneTypeId == battleSceneId)
            {
                return battle;
            }
        }
        return null;
    }

    /// <summary>
    /// Запуск нового экземпляра сражения
    /// </summary>
    public static Battle CreateBattle(int playerId, int battleSceneId, int rigId, bool forceCreate)
    {

        List<Battle> playerBattles = BattlesForPlayer(playerId, true);
        Battle curBattle = new Battle();
        curBattle.PlayerId = playerId;
        curBattle.Ongoing = 1;
        curBattle.BattleSceneTypeId = battleSceneId;
        curBattle.RigId = rigId;
        curBattle.DateStart = DateTime.Now;
        curBattle.Successfull = 0;
        curBattle.CurrentCycle = 1;
        curBattle.CurrentStage = 1;
        curBattle.MaxOpenedCycle = 1;
        curBattle.MaxOpenedCycle = 1;
        curBattle.ShipExperience = 0;
        curBattle.ShipOpenedSkills = "";
        curBattle.battleScene = new BattleScene(BattleSceneType.SceneById(curBattle.BattleSceneTypeId));
        curBattle.SaveData();
        return curBattle;
    }

    public static bool BattleCanBeStarted(string steamId, int battleSceneId)
    {
        StringAndInt nextObject = PlayerDataSql.NextStoryObject(steamId);
        return nextObject.StrValue == "battle" && nextObject.IntValue == battleSceneId;
    }

}

public class UnityBattle
{
    public int Id { get; set; }
    public Guid BattleCode { get; set; }
    public int PlayerId { get; set; }
    public int Ongoing { get; set; }
    public int BattleSceneTypeId { get; set; }
    public int RigId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateComplete { get; set; }
    public int Successfull { get; set; }
    public int CurrentCycle { get; set; }
    public int CurrentStage { get; set; }
    public int MaxOpenedCycle { get; set; }
    public int MaxOpenedStage { get; set; }
    public int ShipExperience { get; set; }
    public string ShipOpenedSkills { get; set; }
    public BattleScene battleScene { get; set; }
}