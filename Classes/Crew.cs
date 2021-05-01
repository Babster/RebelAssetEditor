using AdmiralNamespace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Crew
{

    public class CrewOfficerType
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int AvailableAtStart { get; set; }
        public int BonusPoints { get; set; }
        public int PortraitId { get; set; }

        public List<OfficerStat> Stats { get; set; }

        public CrewOfficerType() { }

        public CrewOfficerType(Boolean fillObject)
        {
            if(fillObject)
            {
                this.Name = "New officer type";
                Stats = CreateStats();
            }
        }

        public CrewOfficerType(ref SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            Name = Convert.ToString(r["name"]);
            AvailableAtStart = Convert.ToInt32(r["available_at_start"]);
            BonusPoints = Convert.ToInt32(r["bonus_points"]);
            PortraitId = Convert.ToInt32(r["portrait_id"]);

            LoadStats();

        }

        private void LoadStats()
        {
            Stats = new List<OfficerStat>();
            string q;
            q = StatsQuery() + $@"
                WHERE
                    crew_officer_type_id = {Id}";

            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while(r.Read())
                {
                    OfficerStat stat = new OfficerStat(ref r);
                    Stats.Add(stat);
                }
            }
            r.Close();

        }

        private static string StatsQuery()
        {
            string q = $@"
            SELECT
                id,
                crew_officer_type_id,
                stat_name,
                points_base
            FROM
                crew_officers_types_stats";
            return q;
        }

        public static List<OfficerStat> CreateStats()
        {
            List<OfficerStat> Stats = new List<OfficerStat>();
            List<OfficerStat.StatType> statTypeList = Enum.GetValues(typeof(OfficerStat.StatType))
                    .Cast<OfficerStat.StatType>()
                    .ToList();
            foreach(OfficerStat.StatType statType in statTypeList)
            {
                if (statType != OfficerStat.StatType.None )
                {
                    OfficerStat curStat = new OfficerStat();
                    curStat.Name = OfficerStat.StatTypeToString(statType);
                    curStat.PointsBase = 0;
                    Stats.Add(curStat);
                }
            }
            return Stats;
        }

        public void SaveData()
        {

            string q;
            if(this.Id == 0)
            {
                q = @"INSERT INTO crew_officers_types(name) VALUES('')
                        SELECT @@IDENTITY AS Result";
                this.Id = DataConnection.GetResultInt(q);
            }

            q = @"
                UPDATE crew_officers_types SET
                    name = @str1,
                    available_at_start = " + this.AvailableAtStart + @",
                    bonus_points = " + this.BonusPoints + @",
                    portrait_id = " + this.PortraitId + @"
                WHERE 
                    id = " + this.Id;

            List<string> names = new List<string> { this.Name };
            DataConnection.Execute(q, names);

            //Saving stats
            foreach(OfficerStat stat in Stats)
            {
                stat.TypeId = this.Id;
                stat.Save();
            }

        }

        public class OfficerStat
        {
            public int Id { get; set; }
            public int TypeId { get; set; }
            public string Name { get; set; }
            public int PointsBase { get; set; }

            public OfficerStat() { }

            public OfficerStat(ref SqlDataReader r)
            {
                Id = Convert.ToInt32(r["id"]);
                TypeId = Convert.ToInt32(r["crew_officer_type_id"]);
                Name = Convert.ToString(r["stat_name"]);
                PointsBase = Convert.ToInt32(r["points_base"]);
            }

            public enum StatType
            {
                None = 0,
                EnergyWeapons = 1,
                KineticWeapons = 2,
                RocketWeapons = 3,
                EngineBoost = 4,
                ThrustersBoost = 5,
                ShieldsBoost = 6,
                ArmorBoost = 7,
                Repair = 8,
                Dexterity = 9,
                LootBoost = 10
            }

            public StatType StatTypeFromString
            {
                get
                {
                    switch (Name)
                    {
                        case "Energy weapons":
                            return StatType.EnergyWeapons;
                        case "Kinetic weapons":
                            return StatType.KineticWeapons;
                        case "Rocket weapons":
                            return StatType.RocketWeapons;
                        case "Engine boost":
                            return StatType.EngineBoost;
                        case "Thrusters boost":
                            return StatType.ThrustersBoost;
                        case "Shields boost":
                            return StatType.ShieldsBoost;
                        case "Armor boost":
                            return StatType.ArmorBoost;
                        case "Repair":
                            return StatType.Repair;
                        case "Dexterity":
                            return StatType.Dexterity;
                        case "Loot boost":
                            return StatType.LootBoost;
                        default:
                            return StatType.None;
                    }
                }
            }

            public static string StatTypeToString(StatType sType)
            {
                switch(sType)
                {
                    case StatType.EnergyWeapons:
                        return "Energy weapons";
                    case StatType.KineticWeapons:
                        return "Kinetic weapons";
                    case StatType.RocketWeapons:
                        return "Rocket weapons";
                    case StatType.EngineBoost:
                        return "Engine boost";
                    case StatType.ThrustersBoost:
                        return "Thrusters boost";
                    case StatType.ShieldsBoost:
                        return "Shields boost";
                    case StatType.ArmorBoost:
                        return "Armor boost";
                    case StatType.Repair:
                        return "Repair";
                    case StatType.Dexterity:
                        return "Dexterity";
                    case StatType.LootBoost:
                        return "Loot boost";
                    default:
                        return "None";
                }
                    
            }

            public void Save()
            {
                string q;
                List<string> names = new List<string> { this.Name };
                if (Id == 0)
                {
                    q = @"INSERT INTO crew_officers_types_stats(crew_officer_type_id, stat_name, points_base) VALUES (" + TypeId + ", @str1, " + PointsBase + @")
                            SELECT @@IDENTITY AS Result";
                    this.Id = DataConnection.GetResultInt(q, names);
                }
                else
                {
                    q = "UPDATE crew_officers_types_stats SET points_base = " + PointsBase + " WHERE id = " + this.Id;
                    DataConnection.Execute(q, names);
                }

            }

            public override string ToString()
            {
                return $"{Name} ({PointsBase})";
            }

        }

        private static Dictionary<int, OfficerStat> statDict;
        private static void CreateStatDict()
        {
            statDict = new Dictionary<int, OfficerStat>();
            string q;
            q = StatsQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    OfficerStat tStat = new OfficerStat(ref r);
                    statDict.Add(tStat.Id, tStat);
                }
            }
        }
        public static OfficerStat OfficerStatById(int Id)
        {
            if (statDict == null)
                CreateStatDict();
            if(statDict.ContainsKey(Id))
            {
                return statDict[Id];
            }
            else
            {
                return null;
            }
            
        }

        public override string ToString()
        {
            return $"{Name}: {BonusPoints}";
        }

        public void SetStatValue(string ScoreName, int Score)
        {
            foreach(OfficerStat stat in Stats )
            {
                if(stat.Name == ScoreName)
                {
                    stat.PointsBase = Score;
                    return;
                }
            }
            throw new Exception("Неизвестный скилл офицера: " + ScoreName);

        }

        public static string OfficerTypeQuery()
        {
            string q = @"
                SELECT
                    id,
                    name,
                    available_at_start,
                    bonus_points,
                    portrait_id
                FROM
                    crew_officers_types";
            return q;
        }

        public static List<CrewOfficerType> GetTypeList()
        {
            List<CrewOfficerType> res = new List<CrewOfficerType>();
            string q = OfficerTypeQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    CrewOfficerType cType = new CrewOfficerType(ref r);
                    res.Add(cType);
                }
            }
            return res;
        }

        private static void CreateDict()
        {
            OfficerDict = new Dictionary<int, CrewOfficerType>();
            List<CrewOfficerType> lst = GetTypeList();
            foreach(CrewOfficerType t in lst)
            {
                OfficerDict.Add(t.Id, t);
            }
        }

        private static Dictionary<int, CrewOfficerType> OfficerDict;
        public static CrewOfficerType OfficerById(int Id)
        {
            if (OfficerDict == null)
                CreateDict();
            if(OfficerDict.ContainsKey(Id))
            {
                return OfficerDict[Id];
            }
            else
            {
                return null;
            }
        }

    }

    public class CrewOfficer
    {
        public int Id { get; set; }

        public CrewOfficerType OfficerType { get; set; }

        public List<Stat> Stats { get; set; }

        public int PlayerId { get; set; }

        //Для тех объектов, которые созданы на основе статов игрока
        private AccountData acc;
        private AdmiralStats playerStats;

        public CrewOfficer() { }

        /// <summary>
        /// Generates new officer from template (officerType)
        /// </summary>
        /// <param name="officerType"></param>
        public CrewOfficer(CrewOfficerType officerType, int PlayerId)
        {
            this.OfficerType = officerType;
            this.PlayerId = PlayerId;
            Stats = new List<Stat>();
            foreach (CrewOfficerType.OfficerStat curStat in OfficerType.Stats)
            {
                Stat newStat = new Stat();
                newStat.BaseStat = CrewOfficerType.OfficerStatById(curStat.Id);
                Stats.Add(newStat);
            }

            Random rnd = new Random();
            int maxRange = Stats.Count;
            for(int i=0; i < officerType.BonusPoints; i++)
            {
                Stats[rnd.Next(0, maxRange)].AddStat();
            }

        }

        public CrewOfficer(SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            OfficerType = CrewOfficerType.OfficerById(Convert.ToInt32(r["officer_type_id"]));
            LoadStats();
        }

        /// <summary>
        /// Создание офицера из персонажа игрока.
        /// Главная фишка процедуры - трансляция параметров игрока в параметры офицера
        /// </summary>
        /// <param name="acc"></param>
        public CrewOfficer(AccountData acc)
        {
            Stats = new List<Stat>();
            this.acc = acc;
            this.playerStats = new AdmiralStats(ref acc);
            PlayerId = acc.Id;// Не нужно, но пусть будет
            List<CrewOfficerType.OfficerStat> statTypes = CrewOfficerType.CreateStats();
            foreach(CrewOfficerType.OfficerStat statType in statTypes)
            {
                int score = 0;
                switch(statType.StatTypeFromString)
                {
                    case CrewOfficerType.OfficerStat.StatType.ArmorBoost:
                        score = playerStats.GetSpecificStat("Armor mastery").Value;
                        break;
                    case CrewOfficerType.OfficerStat.StatType.Dexterity:
                        score = playerStats.GetSpecificStat("Thrusters").Value;
                        break;
                    case CrewOfficerType.OfficerStat.StatType.EngineBoost:
                        score = playerStats.GetSpecificStat("Engines").Value;
                        break;
                    case CrewOfficerType.OfficerStat.StatType.EnergyWeapons:
                        score = playerStats.GetSpecificStat("Energy weapons").Value;
                        break;
                    case CrewOfficerType.OfficerStat.StatType.KineticWeapons:
                        score = playerStats.GetSpecificStat("Kinetic weapons").Value;
                        break;
                    case CrewOfficerType.OfficerStat.StatType.RocketWeapons:
                        score = playerStats.GetSpecificStat("Rocket weapons").Value;
                        break;
                    case CrewOfficerType.OfficerStat.StatType.ShieldsBoost:
                        score = playerStats.GetSpecificStat("Shield mastery").Value;
                        break;

                    default:
                        break;
                }
                if (score > 0)
                {
                    Stat stat = new Stat();
                    stat.BaseStat = statType;
                    stat.BonusPoints = Convert.ToInt32(score * 3.14);
                    Stats.Add(stat);
                }
                    

            }
        }

        private void LoadStats()
        {
            Stats = new List<Stat>();
            string q;
            q = $@"
                SELECT
                    id,
                    crew_officer_id,
                    stat_id,
                    bonus_points
                FROM
                    crew_officers_stats
                WHERE
                    crew_officer_id = {Id}";
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    Stat stat = new Stat(r);
                    Stats.Add(stat);
                }
            }
            r.Close();
        }

        public class Stat
        {
            public int Id { get; set; } //Таблица crew_officers_stats
            public int BonusPoints { get; set; }
            public CrewOfficerType.OfficerStat BaseStat { get; set; }
            public int BaseStatId
            {
                get
                {
                    return BaseStat.Id;
                }
            } //Read-only

            public int Value //Read-only
            {
                get
                {
                    return BaseStat.PointsBase + BonusPoints;
                }
            }

            public Stat() { }
            public Stat(SqlDataReader r)
            {
                Id = Convert.ToInt32(r["id"]);
                BonusPoints = Convert.ToInt32(r["bonus_points"]);
                int bsId = Convert.ToInt32(r["stat_id"]);
                BaseStat = CrewOfficerType.OfficerStatById(bsId);
            }
            public void AddStat() { BonusPoints += 1; }

            public override string ToString()
            {
                return BaseStat.ToString();
            }

            public void SaveData(int OfficerId)
            {
                string q;
                if(Id == 0)
                {
                    q = $@"
                        INSERT INTO crew_officers_stats(crew_officer_id) VALUES({OfficerId})
                        SELECT @@IDENTITY AS Result";
                    Id = DataConnection.GetResultInt(q);
                }

                q = $@"
                    UPDATE crew_officers_stats SET 
                        stat_id = {BaseStatId}, 
                        bonus_points = {BonusPoints}
                    WHERE
                        id = {Id}";
                DataConnection.Execute(q);
            }

        }

        public void Save()
        {
            if (IsPlayer)//Из игрока офицера сохранять не нужно
                return;

            if (CrewOfficer.OfficerDict == null)
                CrewOfficer.CreateDictionary();

            string q;
            if(Id == 0)
            {
                q = $@"
                    INSERT INTO crew_officers(player_id) VALUES({PlayerId})
                    SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
                CrewOfficer.OfficerDict.Add(Id, this);
            }
            q = $@"
                UPDATE crew_officers SET
                    officer_type_id = {OfficerType.Id}
                WHERE
                    id = {Id}";
            DataConnection.Execute(q);

            string IdsDoNotDelete = "";
            foreach(Stat stat in Stats)
            {
                stat.SaveData(Id);
                if (IdsDoNotDelete != "")
                    IdsDoNotDelete += ",";
                IdsDoNotDelete += stat.Id.ToString();
            }

            q = $@"DELETE FROM crew_officers_stats WHERE crew_officer_id = {Id} AND id NOT IN({IdsDoNotDelete})";
            DataConnection.Execute(q);

        }

        public bool IsPlayer
        {
            get
            {
                return acc != null;
            }
        }

        public override string ToString()
        {
            if(acc == null)
            {
                return $"{OfficerType.Name} ({OfficerType.Id})";
            }
            else
            {
                return acc.Name;
            }
            
        }

        public void Delete()
        {
            if (Id == 0) //Если еще не записан то удалять нечего
                return;
            if (acc != null)//Если создан из офицера, то тоже (это лишняя проверка, т.к. идентификатора всё равно не будет, но пусть)
                return;
            string q;
            q = $@"
                DELETE FROM crew_officers WHERE id = {Id};
                DELETE FROM crew_officers_stats WHERE crew_officer_id = {Id}";

            DataConnection.Execute(q);
        }

        private static Dictionary<int, CrewOfficer> OfficerDict;
        private static void CreateDictionary()
        {
            OfficerDict = new Dictionary<int, CrewOfficer>();
            string q = OfficerQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while(r.Read())
                {
                    CrewOfficer curOfficer = new CrewOfficer(r);
                    OfficerDict.Add(curOfficer.Id, curOfficer);
                }
            }
            r.Close();
        }
        public static CrewOfficer OfficerById(int Id)
        {
            if (OfficerDict == null)
                CreateDictionary();

            if(OfficerDict.ContainsKey(Id))
            {
                return OfficerDict[Id];
            }
            else
            {
                return null;
            }
        }
        public static List<CrewOfficer> OfficersForPlayer(int playerId, bool onlyFree = false)
        {
            List<CrewOfficer> tList = new List<CrewOfficer>();
            string q = $"SELECT id FROM crew_officers WHERE player_id = {playerId}";
            if (!onlyFree)
                q += $@" AND ISNULL(rig_id, 0) = 0";
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    int id = Convert.ToInt32(r["id"]);
                    tList.Add(OfficerById(id));
                }
            }
            r.Close();
            return tList;
        }
        private static string OfficerQuery()
        {
            string q;
            q = $@"
                SELECT
                    id,
                    player_id,
                    officer_type_id
                FROM
                    crew_officers";
            return q;
        }
        public int StatValue(CrewOfficerType.OfficerStat.StatType param)
        {
            if (Stats.Count == 0)
                return 0;
            foreach(Stat stat in Stats)
            {
                if(stat.BaseStat.StatTypeFromString == param)
                {
                    return stat.Value;
                }
            }
            return 0;
        }

    }

}