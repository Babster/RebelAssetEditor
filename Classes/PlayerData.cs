using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerDataSql : PlayerData
{

}

public class PlayerData
{
    public int Id { get; set; }
    public string SteamId { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }



}

