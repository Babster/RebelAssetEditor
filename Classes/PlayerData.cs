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

        int StatPoints = CommonFunctions.GetCommonValue("start_stat_points").IntValue;

        q = $@"INSERT INTO players(steam_id, display_name, register_date, skill_points_left) VALUES(@str1, @str2, GETDATE(), {StatPoints})";
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

    public static PlayerData GetPlayerData(string steamId)
    {
        string q = $@"SELECT 
            steam_id,
            display_name,
            register_date,
            skill_points_left
            
        FROM
            players
        WHERE
            steam_id = @str1";
        List<string> names = new List<string>() { steamId };
        SqlDataReader r = DataConnection.GetReader(q, names);
        if(!r.HasRows)
        {
            r.Close();
            return null;
        }

        PlayerData curData = new PlayerData();

        r.Read();
        curData.SteamId = (string)r["steam_id"];
        curData.DisplayName = (string)r["display_name"];
        curData.SkillPointsLeft = (int)r["skill_points_left"];
        r.Close();

        return curData;

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
        return PlayerStoryFlowHub.CurrentProgressElementForPlayer(playerId).ToStringAndInt();

    }

    public static StringAndInt RegisterStoryElementCompleted(string steamId)
    {
        int playerId = PlayerId(steamId);
        return PlayerStoryFlowHub.RegisterPlayerProgress(playerId).ToStringAndInt();
    }


}

public class PlayerData
{
    public string SteamId { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    public int SkillPointsLeft { get; set; }
    
}

public class PlayerStatType
{
    public int Id { get; set; }
    public string SkillGroup { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
    public string DescriptionEnglish { get; set; }
    public string DescriptionRussian { get; set; }
    public int OrderIdx { get; set; }

    public PlayerStatType() { }
    public PlayerStatType(SqlDataReader r)
    {
        this.Id = Convert.ToInt32(r["id"]);
        this.SkillGroup = Convert.ToString(r["skill_group"]);
        this.Name = Convert.ToString(r["name"]);
        this.OrderIdx = Convert.ToInt32(r["order_idx"]);
        this.Value = Convert.ToInt32(r["base_value"]);
        this.DescriptionEnglish = Convert.ToString(r["description_english"]);
        this.DescriptionRussian = Convert.ToString(r["description_russian"]);
    }

    /*public PlayerStat Copy()
    {
        PlayerStat newElement = new PlayerStat();
        newElement.SkillGroup = this.SkillGroup;
        newElement.Id = this.Id;
        newElement.Name = this.Name;
        newElement.Value = this.Value;
        newElement.DescriptionEnglish = this.DescriptionEnglish;
        newElement.DescriptionRussian = this.DescriptionRussian;
        newElement.OrderIdx = this.OrderIdx;

        return newElement;

    }*/

    public void SaveData()
    {

        string q;

        if (this.Id == 0)
        {
            q = @"INSERT INTO officer_stat_types(name) VALUES('')
                         SELECT @@IDENTITY AS field0";
            this.Id = Convert.ToInt32(DataConnection.GetResult(q));
        }

        List<String> names = new List<string>();
        names.Add(this.SkillGroup);
        names.Add(this.Name);
        names.Add(DescriptionEnglish);
        names.Add(DescriptionRussian);
        q = @"UPDATE officer_stat_types SET
                        skill_group = @str1,
                        name = @str2,
                        base_value = " + this.Value + @",
                        description_english = @str3,
                        description_russian = @str4,
						order_idx = " + this.OrderIdx + @"
                    WHERE id = " + Id.ToString();
        DataConnection.Execute(q, names);
    }

    public static List<PlayerStatType> GetStatTypeList()
    {
        List<PlayerStatType> stats = new List<PlayerStatType>();

        string q;
        SqlDataReader r;

        q = @"SELECT 
                    id,
                    ISNULL(skill_group, '') AS skill_group,
                    name,
                    base_value,
                    description_english,
                    description_russian,
                    order_idx
                FROM
                    officer_stat_types
                ORDER BY
                    order_idx";
        r = DataConnection.GetReader(q);
        while(r.Read())
        {
            stats.Add(new PlayerStatType(r));
        }
        r.Close();

        return stats;

    }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

}
