
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AdmiralNamespace
{


    public class AdmiralsMain
    {

        public static StringAndInt AdmiralNextObject(int AdmiralId, bool doNotLog = false)
        {

            StringAndInt tObject = new StringAndInt();

            string q;
            q = @"SELECT
					admirals_progress.object_type,
					admirals_progress.object_id
				FROM
					admirals_progress 
				INNER JOIN
					(SELECT
						admiral,
						MAX(date_completed) AS max_date_completed
					FROM
						admirals_progress
					GROUP BY
						admiral) AS max_progress ON 
													max_progress.admiral = admirals_progress.admiral
													AND max_progress.max_date_completed = admirals_progress.date_completed 
				WHERE
					admirals_progress.admiral = " + AdmiralId.ToString();

            SqlDataReader r;
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                r.Read();
                tObject.IntValue = Convert.ToInt32(r["object_id"]);
                tObject.StrValue = Convert.ToString(r["object_type"]);
            }
            else
            {
                //tObject = GetCommonValue("start_object");
            }
            r.Close();

            if (doNotLog == false)
            {
                DataConnection.Log(AdmiralId, tObject.StrValue, tObject.IntValue, "request next", "");
            }

            return tObject;

        }

        public static void AdmiralRegisterStepFinished(int AdmiralId)
        {
            StringAndInt thisStep = AdmiralNextObject(AdmiralId, true);

            int Id = 0;

            string q = @"SELECT id FROM admirals_progress WHERE 
			admiral = " + AdmiralId.ToString() + @"
			AND object_type = @str1
			AND object_id = " + thisStep.IntValue.ToString();

            List<string> vs = new List<string>();
            vs.Add(thisStep.StrValue);

            SqlDataReader r = DataConnection.GetReader(q, vs);
            if (r.HasRows)
            {
                r.Read();
                Id = Convert.ToInt32(r["id"]);
            }
            r.Close();

            if (Id == 0)
            {
                q = @"INSERT INTO admirals_progress(admiral, object_type, object_id) 
						VALUES(" + AdmiralId.ToString() + @", @str1, " + thisStep.IntValue.ToString() + @")
						SELECT @@IDENTITY AS field0";
                Id = Convert.ToInt32(DataConnection.GetResult(q, vs));
            }

            q = @"UPDATE admirals_progress SET date_completed = GETDATE() WHERE id = " + Id.ToString();
            DataConnection.Execute(q);

            DataConnection.Log(AdmiralId, thisStep.StrValue, thisStep.IntValue, "completed", "");

        }

        public static void RegisterAccount(ref AccountData userData)
        {
            //Main table record
            string Query;
            List<string> names = new List<string>();
            string newPwd = "temp pwd";
            Query = "INSERT INTO admirals(steam_account_id, pwd, status)  VALUES(@str1, @str2, @str3)" +
                    "SELECT @@IDENTITY AS field0";
            names.Add(userData.SteamAccountId);
            names.Add(newPwd);
            names.Add("active");
            int id = Convert.ToInt32(DataConnection.GetResult(Query, names, 0));
            userData.Id = id;
            userData.AdditionalData = newPwd;
            userData.aResult = AccountData.ActionResult.Success;

            //Logging
            DataConnection.Log(id, "account", 0, "registered", "");

            //Stat points start value
            int startStatPoints = PlayerStatLogic.StartPoints();
            Query = @"INSERT INTO admirals_stats(
						admiral_id, 
						value, 
						date_changed, 
						event_type
					) VALUES (
						<admiral_id>,
						<value>,
						GETDATE(),
						'register account'
					)";
            Query = Query.Replace("<admiral_id>", userData.Id.ToString());
            Query = Query.Replace("<value>", "-" + startStatPoints.ToString());
            DataConnection.Execute(Query);

            //Stat points table filling
            SqlDataReader r;
            Query = @"SELECT
						id,
						name,
						base_value
					FROM
						admiral_stat_types
					ORDER BY
						order_idx";
            r = DataConnection.GetReader(Query);
            Query = @"INSERT INTO admirals_stats(
						admiral_id,
						stat_id,
						value,
						date_changed,
						event_type
					) VALUES (
						" + userData.Id + @",
						<stat_id>,
						<value>,
						GETDATE(),
						'start create')";
            while (r.Read())
            {
                string q = Query.Replace("<stat_id>", Convert.ToInt32(r["id"]).ToString());
                q = q.Replace("<value>", Convert.ToInt32(r["base_value"]).ToString());
                DataConnection.Execute(q);
            }
        }

        public static void DeleteAccount(ref AccountData userData)
        {
            if (userData.CheckPwd() != true)
                return;
            string Query = @"
				DELETE FROM admirals_log WHERE admiral = <admiral_id>;
				DELETE FROM admirals_stats WHERE admiral_id = <admiral_id>;
				DELETE FROM admirals_progress WHERE admiral = <admiral_id>;
				DELETE FROM admirals WHERE id = <admiral_id>;";
            Query = Query.Replace("<admiral_id>", userData.Id.ToString());
            DataConnection.Execute(Query);
        }

    }

    public class AccountData
    {
        public string SteamAccountId { get; set; }
        public string Name { get; set; }

        public int Id { get; set; }
        public string AdditionalData { get; set; }

        public ActionType aType { get; set; }

        public enum ActionType
        {
            CreateUser = 1,
            CheckPwd = 2,
            StepCompleted = 3
        }

        public ActionResult aResult { get; set; }
        public enum ActionResult
        {
            Success = 1,
            PasswordCorrect = 2,
            PasswordIncorrect = 3
        }

        public bool CheckPwd()
        {

            string q;

            if (Id == 0)
            {
                q = "SELECT id FROM admirals WHERE steam_account_id = @str1";
                List<string> names = new List<string>();
                names.Add(this.Name);
                this.Id = Convert.ToInt32(DataConnection.GetResult(q, names, 0));
            }

            q = "SELECT pwd AS field0 FROM admirals WHERE id = " + Id.ToString();
            if (Convert.ToString(DataConnection.GetResult(q, null, "")) == this.AdditionalData)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public AccountData() { }

        public AccountData(int Id)
        {
            string q = @"
            SELECT 
                steam_account_id,
                pwd,
                status
            FROM
                admirals
            WHERE
                id = " + Id.ToString();

            SqlDataReader r = DataConnection.GetReader(q);
            r.Read();
            this.Id = Id;
            this.Name = Convert.ToString(r["steam_account_id"]);
            this.AdditionalData = Convert.ToString(r["pwd"]);
            r.Close();
        }

    }

    public static class PlayerStatLogic
    {

        private static Dictionary<int, PlayerStat> StatCache;

        public static Dictionary<int, PlayerStat> GetStatCache()
        {
            if (StatCache == null)
            {
                StatCache = new Dictionary<int, PlayerStat>();
                List<PlayerStat> tList = GetStatList();
                foreach (PlayerStat tStat in tList)
                {
                    StatCache.Add(tStat.Id, tStat);
                }
            }
            return StatCache;
        }

        public static List<PlayerStat> GetStatList()
        {

            string q;
            SqlDataReader r;

            List<PlayerStat> tStats = new List<PlayerStat>();

            q = @"SELECT 
                    id,
                    name,
                    base_value,
                    description_english,
                    description_russian,
                    order_idx
                FROM
                    admiral_stat_types
                ORDER BY
                    order_idx";

            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    PlayerStat tStat = new PlayerStatWithSql(ref r).Copy();
                    tStats.Add(tStat);
                }
            }
            r.Close();

            return tStats;

        }

        public static int StatPointsLeft(int AdmiralId)
        {
            string q = @"SELECT
							-SUM(value) AS field0
						FROM
							admirals_stats
						WHERE
							admiral_id = " + AdmiralId.ToString();
            return Convert.ToInt32(DataConnection.GetResult(q, null, 0));
        }

        public static int StartPoints()
        {
            string q = @"SELECT SUM(points) AS field0
						FROM
							(SELECT 
								SUM(base_value) AS points
							FROM 
								admiral_stat_types 

							UNION ALL

							SELECT 
								value_int AS points
							FROM
								s_common_values
							WHERE
								name = 'start_stat_points'
							) AS t0";
            return Convert.ToInt32(DataConnection.GetResult(q));
        }

        public static string RegisterStatChange(int AdmiralId, AdmiralStats stats)
        {

            Dictionary<int, PlayerStat> statCache = GetStatCache();

            int totalStatChange = 0;
            foreach (PlayerStat curStat in stats.Stats)
            {
                //Отрицательных статов быть не может
                if (curStat.Value < 0)
                    return "every player stat change must be positive";

                //Каждое изменение должно касаться существующего стата
                if (!statCache.ContainsKey(curStat.Id))
                    return "player stat with id = " + curStat.Id.ToString() + " doesn't exist in the game";

                totalStatChange += curStat.Value;
            }

            //Какие-нибудь изменения должны быть.
            if (totalStatChange == 0)
                return "changes not found";

            //Сумма изменений не может быть больше чем оставшее количество очков игрока
            int statPointsLeft = StatPointsLeft(AdmiralId);
            if (totalStatChange > statPointsLeft)
                return "not enough player points";

            string qTemplate = @"
                INSERT INTO admirals_stats(
                    admiral_id,
                    stat_id,
                    value,
                    date_changed,
                    event_type
                ) VALUES (
                    " + AdmiralId.ToString() + @",
                    <stat_id>,
                    <value>,
                    GETDATE(),
                    'from_client'
                )";


            //Регистрация изменений
            foreach (PlayerStat curStat in stats.Stats)
            {
                if (curStat.Value > 0)
                {
                    string q = qTemplate.Replace("<stat_id>", curStat.Id.ToString());
                    q = q.Replace("<value>", curStat.Value.ToString());
                    DataConnection.Execute(q);
                }
            }

            return "";
        }

    }

    public class PlayerStat
    {
        public int Id { get; set; }
        public string SkillGroup { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string DescriptionEnglish { get; set; }
        public string DescriptionRussian { get; set; }
        public int OrderIdx { get; set; }

        public PlayerStat() { }
        public PlayerStat(ref SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            this.SkillGroup = Convert.ToString(r["skill_group"]);
            this.Name = Convert.ToString(r["name"]);
            this.OrderIdx = Convert.ToInt32(r["order_idx"]);
            this.Value = Convert.ToInt32(r["base_value"]);
            this.DescriptionEnglish = Convert.ToString(r["description_english"]);
            this.DescriptionRussian = Convert.ToString(r["description_russian"]);
        }

        public PlayerStat Copy()
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

        }

        public PlayerStatWithSql CopyAsSql()
        {
            PlayerStatWithSql newElement = new PlayerStatWithSql();
            newElement.Id = this.Id;
            newElement.SkillGroup = this.SkillGroup;
            newElement.Name = this.Name;
            newElement.Value = this.Value;
            newElement.DescriptionEnglish = this.DescriptionEnglish;
            newElement.DescriptionRussian = this.DescriptionRussian;
            newElement.OrderIdx = this.OrderIdx;

            return newElement;

        }
        
        public void SaveData()
        {

            string q;

            if (this.Id == 0)
            {
                q = @"INSERT INTO admiral_stat_types(name) VALUES('')
                         SELECT @@IDENTITY AS field0";
                this.Id = Convert.ToInt32(DataConnection.GetResult(q));
            }

            List<String> names = new List<string>();
            names.Add(this.SkillGroup);
            names.Add(this.Name);
            names.Add(DescriptionEnglish);
            names.Add(DescriptionRussian);
            q = @"UPDATE admiral_stat_types SET
                        skill_group = @str1,
                        name = @str2,
                        base_value = " + this.Value + @",
                        description_english = @str3,
                        description_russian = @str4,
						order_idx = " + this.OrderIdx + @"
                    WHERE id = " + Id.ToString();
            DataConnection.Execute(q, names);
        }

    }

    public class PlayerStatWithSql : PlayerStat
    {

        public PlayerStatWithSql()
        {

        }

        public PlayerStatWithSql(ref SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            this.SkillGroup = Convert.ToString(r["skill_group"]);
            this.Name = Convert.ToString(r["name"]);
            this.OrderIdx = Convert.ToInt32(r["order_idx"]);
            this.Value = Convert.ToInt32(r["base_value"]);
            this.DescriptionEnglish = Convert.ToString(r["description_english"]);
            this.DescriptionRussian = Convert.ToString(r["description_russian"]);
        }

      //  public void SaveData()
      //  {

      //      string q;

      //      if (this.Id == 0)
      //      {
      //          q = @"INSERT INTO admiral_stat_types(name) VALUES('')
      //                   SELECT @@IDENTITY AS field0";
      //          this.Id = Convert.ToInt32(DataConnection.GetResult(q));
      //      }

      //      List<String> names = new List<string>();
      //      names.Add(this.Name);
      //      names.Add(DescriptionEnglish);
      //      names.Add(DescriptionRussian);
      //      q = @"UPDATE admiral_stat_types SET
      //                  name = @str1,
      //                  base_value = " + this.Value + @",
      //                  description_english = @str2,
      //                  description_russian = @str3,
						//order_idx = " + this.OrderIdx + @"
      //              WHERE id = " + Id.ToString();
      //      DataConnection.Execute(q, names);
      //  }



    }

    public class AdmiralStats
    {
        public List<PlayerStat> Stats { get; set; }
        public int StatPointsLeft { get; set; }
        public AdmiralStats() { }

        public AdmiralStats(ref AccountData userData)
        {

            if (userData.CheckPwd() == false)
                return;

            //Список статов игрока
            Dictionary<int, PlayerStat> statCache = PlayerStatLogic.GetStatCache();
            Stats = new List<PlayerStat>();
            string q = @"
				SELECT
					--admirals_stats.admiral_id,
					admirals_stats.stat_id,
					SUM(admirals_stats.value) AS stat_score
				FROM
					admirals_stats
				WHERE
					admirals_stats.admiral_id = " + userData.Id + @"
					AND admirals_stats.value > 0
				GROUP BY
					--admirals_stats.admiral_id,
					admirals_stats.stat_id";
            SqlDataReader r = DataConnection.GetReader(q);
            while (r.Read())
            {
                PlayerStat templateStat = statCache[Convert.ToInt32(r["stat_id"])];
                PlayerStat tStat = templateStat.Copy();
                tStat.Value = Convert.ToInt32(r["stat_score"]);
                Stats.Add(tStat);
            }

            //Сколько у игрока осталось свободных очков для распределения
            q = @"
				SELECT
					-SUM(value) AS value_sum
				FROM
					admirals_stats
				WHERE
					admiral_id = " + userData.Id;

            this.StatPointsLeft = Convert.ToInt32(DataConnection.GetResult(q));

        }

    }

    public class AdmiralStatsWithSql : AdmiralStats
    {

        public AdmiralStatsWithSql() { }

        public AdmiralStatsWithSql(int AdmiralId)
        {

            //Список статов игрока
            Dictionary<int, PlayerStat> statCache = PlayerStatLogic.GetStatCache();
            Stats = new List<PlayerStat>();
            string q = @"
				SELECT
					--admirals_stats.admiral_id,
					admirals_stats.stat_id,
					SUM(admirals_stats.value) AS stat_score
				FROM
					admirals_stats
				WHERE
					admirals_stats.admiral_id = " + AdmiralId + @"
					AND admirals_stats.value > 0
				GROUP BY
					--admirals_stats.admiral_id,
					admirals_stats.stat_id";
            SqlDataReader r = DataConnection.GetReader(q);
            while (r.Read())
            {
                PlayerStat templateStat = statCache[Convert.ToInt32(r["stat_id"])];
                PlayerStat tStat = templateStat.Copy();
                tStat.Value = Convert.ToInt32(r["stat_score"]);
                Stats.Add(tStat);
            }

            //Сколько у игрока осталось свободных очков для распределения
            q = @"
				SELECT
					-SUM(value) AS value_sum
				FROM
					admirals_stats
				WHERE
					admiral_id = " + AdmiralId;

            this.StatPointsLeft = Convert.ToInt32(DataConnection.GetResult(q));

        }

    }

}


