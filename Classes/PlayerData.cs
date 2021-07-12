using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;

public class PlayerDataSql : PlayerData
{
    
    /// <summary>
    /// Возвращает пустую строку если регистрация успешна и описание ошибки если что-то пошло не так
    /// </summary>
    /// <returns></returns>
    public static string RegisterAccount(string serializedBindingModel)
    {

        RegisterBindingModel curModel = JsonConvert.DeserializeObject<RegisterBindingModel>(serializedBindingModel);

        string q;
        q = $@"SELECT id FROM players WHERE steam_id = @str1";
        List<String> names = new List<string>() { curModel.SteamId };
        int id = DataConnection.GetResultInt(q, names, 0);
        if(id > 0)
        {
            return "Account already been registered";
        }

        //int StatPoints = CommonFunctions.GetCommonValue("start_stat_points").IntValue;

        q = $@"INSERT INTO players(steam_id, display_name, register_date) VALUES(@str1, @str2, GETDATE())";
        names.Add(curModel.DisplayName);
        try
        {
            DataConnection.Execute(q, names);
            DataConnection.Execute(q, names);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "";

    }

    /// <summary>
    /// Процедуры, которые возвращают данные по игроку по его идентификаторам
    /// </summary>
    public static PlayerData GetPlayerData(string steamId)
    {
        return PlayerDataByIdOrSteamId(steamId, 0);
    }
    public static PlayerData GetPlayerData(int id)
    {
        return PlayerDataByIdOrSteamId("", id);
    }
    private static PlayerData PlayerDataByIdOrSteamId(string steamId, int id)
    {
        string q = $@"SELECT 
            steam_id,
            display_name,
            register_date
        FROM
            players
        WHERE
            ";

        bool needAnd = false;

        if(id > 0)
        {
            q = q + $" id = {id}";
            needAnd = true;
        }

        SqlDataReader r;
        if (!String.IsNullOrEmpty(steamId))
        {
            if (needAnd)
            {
                q = q + " AND ";
            }
            q = q + " steam_id = @str1";
            List<string> names = new List<string>() { steamId };
             r = DataConnection.GetReader(q, names);
        }
        else
        {
             r = DataConnection.GetReader(q);
        }
        
        if (!r.HasRows)
        {
            r.Close();
            return null;
        }

        PlayerData curData = new PlayerData();

        r.Read();
        curData.SteamId = (string)r["steam_id"];
        curData.DisplayName = (string)r["display_name"];
        r.Close();

        return curData;
    }
    
    /// <summary>
    /// Получение данных тестового игрока
    /// </summary>
    /// <returns></returns>
    public static PlayerData GetTestPlayerData()
    {
        return GetPlayerData(6);
    }

    /// <summary>
    /// Ключом является SteamId игрока, а значением его идентификатор в таблице players,
    /// который идентификатор используется во всех таблицах, которые ссылаются на данные
    /// игрока (например, его прогресс в прохождении истории)
    /// </summary>
    private static Dictionary<string, int> playerIdsDictionary;
    public static int PlayerId(string steamId)
    {
        if(playerIdsDictionary == null)
        {
            playerIdsDictionary = new Dictionary<string, int>();
        }
        if (!playerIdsDictionary.ContainsKey(steamId))
        {
            string q;
            q = $"SELECT id FROM players WHERE steam_id = @str1";
            List<string> names = new List<string>() { steamId };
            int id = DataConnection.GetResultInt(q, names, 0);
            playerIdsDictionary.Add(steamId, id);
            return id;
        }
        else
        {
            return playerIdsDictionary[steamId];
        }
    }

    public void SaveBaseData()
    {
        string q = $@"UPDATE players SET display_name = @str1 WHERE steam_id = @str2";
        List<string> names = new List<string>() { DisplayName, SteamId };
        DataConnection.Execute(q, names);
    }

    private class RegisterBindingModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SteamId { get; set; }
        public string DisplayName { get; set; }

    }
    
    /// <summary>
    /// Возвращает имя и идентификатор объекта, который в данный момент доступен для игрока.
    /// Начинается всё со сцены истории story 1 потом битва и так далее пока не дойдёт до
    /// базы. Там будем разрабатывать систему миссий которая будет выглядеть несколько 
    /// по-другому.
    /// </summary>
    /// <param name="steamId"></param>
    /// <returns></returns>
    public static StringAndInt NextStoryObject(string steamId)
    {
        int playerId = PlayerId(steamId);
        StringAndInt curData = PlayerStoryFlowHub.CurrentProgressElementForPlayer(playerId).ToStringAndInt();

        //Если битва еще не началась, то игрока надо кинуть на сцену рига корабля
        if(curData.StrValue == "battle")
        {

            int battleId = curData.IntValue;

            //Проверка на то, что битва уже идёт
            Battle curBattle = Battle.BattleByTypeId(playerId, battleId);

            //Если битва не идёт надо проверить нет ли уже готового рига под эту битву
            if(curBattle == null)
            {

                SpaceshipRig rig = SpaceshipRig.RigForBattle(playerId, battleId);
                if(rig == null)
                {
                    curData.StrValue = "rig";
                }
                else
                {
                    curData.StrValue = "launch battle";
                }
            }

        }

        //Если следующий объект истории - станция, тогда нужно поставить специальную пометку в профиль игрока что у него есть станция
        if(curData.StrValue == "station")
        {
            PlayerStoryFlowHub.SetStationAvailable(playerId);
        }

        return curData;

    }
    public static StringAndInt RegisterStoryElementCompleted(string steamId)
    {
        int playerId = PlayerId(steamId);
        return PlayerStoryFlowHub.RegisterPlayerProgress(playerId).ToStringAndInt();
    }

    public static PlayerAsset GetPlayerAsset(int playerId)
    {
        return new PlayerAsset(playerId);
    }

}

public class PlayerData
{
    public string SteamId { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    
    [JsonIgnore]
    public int Id
    {
        get
        {
            return PlayerDataSql.PlayerId(SteamId);
        }
    }

}


