﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;

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
	            admirals.id AS field0
            FROM
	            admirals
            WHERE
	            admirals.steam_account_id = @str1
            ";

            List<string> names = new List<string>();
            names.Add(name);

            int curId = Convert.ToInt32(DataConnection.GetResult(q, names));
            userIds.Add(name, curId);
        }

        return userIds[user.Identity.Name];

    }

}
