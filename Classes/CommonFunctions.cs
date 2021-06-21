using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;

public struct StringAndInt
{
    public string StrValue { get; set; }
    public int IntValue { get; set; }
}

public static class CommonFunctions
{

    public static StringAndInt GetCommonValue(string name)
    {

        StringAndInt tValue = new StringAndInt();

        string q;
        q = "SELECT ISNULL(value_str, '') AS value_str, ISNULL(value_int, 0) AS value_int FROM s_common_values WHERE name = @str1 ";
        List<string> names = new List<string>();
        names.Add(name);
        SqlDataReader r = DataConnection.GetReader(q, names);
        if (r.HasRows == true)
        {
            r.Read();
            tValue.StrValue = Convert.ToString(r["value_str"]);
            tValue.IntValue = Convert.ToInt32(r["value_int"]);
        }
        r.Close();

        return tValue;

    }

    public static void SetCommonvalue(string name, StringAndInt value)
    {
        int id;
        string q = "SELECT id AS Result FROM s_common_values WHERE name = @str1";
        List<string> names = new List<string> { name };

        id = DataConnection.GetResultInt(q, names, 0);
        if(id == 0)
        {
            q = $@"INSERT INTO s_common_values(name, value_str, value_int) VALUES (@str1, @str2, {value.IntValue})";
            names.Add(value.StrValue);
        }
        else
        {
            names[0] = value.StrValue;
            q = $@"UPDATE s_common_values SET value_str = @str1, value_int = {value.IntValue} WHERE id = {id}";
        }
        DataConnection.Execute(q, names);
    }

    private static Dictionary<string, int> userIds;
    public static int UserId(System.Security.Principal.IPrincipal user)
    {

        if (userIds == null)
        {
            userIds = new Dictionary<string, int>();
        }

        string name = user.Identity.Name;

        if (userIds.ContainsKey(name) == false)
        {
            string q = @"SELECT
	            players.id AS field0
            FROM
	            players
            WHERE
	            players.steam_id = @str1
            ";

            List<string> names = new List<string>();
            names.Add(name);

            int curId = Convert.ToInt32(DataConnection.GetResult(q, names));
            userIds.Add(name, curId);
        }

        return userIds[user.Identity.Name];

    }

    public static string Compress(string s)
    {
        var bytes = Encoding.Unicode.GetBytes(s);
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                msi.CopyTo(gs);
            }
            return Convert.ToBase64String(mso.ToArray());
        }
    }

    public static string Decompress(string s)
    {
        var bytes = Convert.FromBase64String(s);
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                gs.CopyTo(mso);
            }
            return Encoding.Unicode.GetString(mso.ToArray());
        }

    }
}
