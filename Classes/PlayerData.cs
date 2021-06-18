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

        q = $@"INSERT INTO players(steam_id, display_name, register_date) VALUES(@str1, @str2, GETDATE())";
        names.Add(curModel.DisplayName);
        try
        {
            DataConnection.Execute(q, names);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "";

    }

    public PlayerData GetPlayerData(string steamId)
    {
        string q = $@"SELECT 
            steam_id,
            display_name,
            register_date
        FROM
            players
        WHERE
            steam_id = @str1";
        List<string> names = new List<string>() { steamId };
        SqlDataReader r = DataConnection.GetReader(q);
        if(!r.HasRows)
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

    private class RegisterBindingModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SteamId { get; set; }
        public string DisplayName { get; set; }

    }

}

public class PlayerData
{
    public string SteamId { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    
}

