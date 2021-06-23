using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

public static class DataConnection
{

    public static SqlConnection conn;

    public static void ConnectSQL()
    {
        string connString = @"server=localhost\SQLEXPRESS;Database=RebelSpaceAdmiral;User Id=sa;Password=123123;MultipleActiveResultSets=True";
        conn = new SqlConnection(connString);
        conn.Open();
    }

    public static SqlDataReader GetReader(string Query, List<string> strParameters = null)
    {

        if (strParameters == null)
        {
            strParameters = new List<string>();
        }

        if (conn == null)
        {
            ConnectSQL();
        }

        SqlDataReader r;
        SqlCommand c = new SqlCommand(Query, conn);
        if (strParameters.Count > 0)
        {
            for (int i = 0; i < strParameters.Count; i++)
            {
                c.Parameters.AddWithValue("@str" + (i + 1).ToString(), strParameters[i]);
            }
        }
        r = c.ExecuteReader();

        return r;

    }

    public static object GetResult(string Query, List<string> strParameters = null, object defaultValue = null)
    {
        if (strParameters == null)
        {
            strParameters = new List<string>();
        }

        if (conn == null)
        {
            ConnectSQL();
        }

        //conn.

        SqlDataReader r;
        SqlCommand c = new SqlCommand(Query, conn);
        if (strParameters.Count > 0)
        {
            for (int i = 0; i < strParameters.Count; i++)
            {
                c.Parameters.AddWithValue("@str" + (i + 1).ToString(), strParameters[i]);
            }
        }

        object tempRes;
        r = c.ExecuteReader();
        if (r.HasRows)
        {
            //123
            r.Read();
            tempRes = r[0];
        }
        else
        {
            tempRes = defaultValue;
        }
        r.Close();

        return tempRes;
    }

    public static int GetResultInt(string Query, List<string> strParameters = null, object defaultValue = null)
    {
        return Convert.ToInt32(GetResult(Query, strParameters, defaultValue));
    }

    public static string GetResultStr(string Query, List<string> strParameters = null, object defaultValue = null)
    {
        return Convert.ToString(GetResult(Query, strParameters, defaultValue));
    }

    public static void Execute(string Query, List<string> strParameters = null, List<byte[]> files = null)
    {

        if (strParameters == null)
        {
            strParameters = new List<string>();
        }

        if (files == null)
        {
            files = new List<byte[]>();
        }

        if (conn == null)
        {
            ConnectSQL();
        }

        bool replaceText = false;

        SqlCommand c = new SqlCommand(Query, conn);
        if (strParameters.Count > 0)
        {
            for (int i = 0; i < strParameters.Count; i++)
            {
                if (String.IsNullOrEmpty(strParameters[i]))
                {
                    Query = Query.Replace("@str" + (i + 1).ToString(), "''");
                    replaceText = true;
                }
                else
                {
                    c.Parameters.AddWithValue("@str" + (i + 1).ToString(), strParameters[i]);
                }

            }
        }

        if (replaceText)
            c.CommandText = Query;

        if (files.Count > 0)
        {
            for (int i = 0; i < files.Count; i++)
            {
                c.Parameters.AddWithValue("@data" + (i + 1).ToString(), files[i]);
            }
        }

        c.ExecuteNonQuery();


    }

    public static void Log(int admiralId, string objectType, int objectId, string actionType, string ipAddress)
    {
        string q;
        List<string> vs = new List<string>();
        vs.Add(objectType);
        vs.Add(actionType);
        vs.Add(ipAddress);
        q = @"INSERT INTO admirals_log(admiral, log_date, object_type, object_id, action, ip_address) VALUES
					(" + admiralId.ToString() + @",
					GETDATE(),
					@str1,
					" + objectId.ToString() + @",
					@str2,
                    @str3)";
        DataConnection.Execute(q, vs);
    }

    public static string DateToSqlString(DateTime date)
    {
        if(date == null)
        {
            return "NULL";
        }
        else if(date == new DateTime(0))
        {
            return "NULL";
        }
        else
        {
            return "CAST('" + date.Year + "-" + date.Day + "-" + date.Month + " " + 
                              date.Hour + ":" + date.Minute + ":" + date.Second + ":" + date.Millisecond + "' AS DateTime)";
        }
    }

}