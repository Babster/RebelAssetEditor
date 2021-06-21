using AdmiralNamespace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Crew
{

    public class CrewOfficerType : UnityCrewOfficerType
    {

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
            Stats = new List<OfficerTypeStat>();
            string q;
            q = StatsQuery() + $@"
                WHERE
                    crew_officer_type_id = {Id}";

            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while(r.Read())
                {
                    OfficerTypeStat stat = new OfficerTypeStat(ref r);
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

        public static List<OfficerTypeStat> CreateStats()
        {
            List<OfficerTypeStat> Stats = new List<OfficerTypeStat>();
            List<OfficerTypeStat.StatType> statTypeList = Enum.GetValues(typeof(OfficerTypeStat.StatType))
                    .Cast<OfficerTypeStat.StatType>()
                    .ToList();
            foreach(OfficerTypeStat.StatType statType in statTypeList)
            {
                if (statType != OfficerTypeStat.StatType.None )
                {
                    OfficerTypeStat curStat = new OfficerTypeStat();
                    curStat.Name = OfficerTypeStat.StatTypeToString(statType);
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
            foreach(OfficerTypeStat stat in Stats)
            {
                stat.TypeId = this.Id;
                stat.Save();
            }

        }

        private static void CreateStatDict()
        {
            StaticMembers.statDict = new Dictionary<int, OfficerTypeStat>();
            string q;
            q = StatsQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    OfficerTypeStat tStat = new OfficerTypeStat(ref r);
                    StaticMembers.statDict.Add(tStat.Id, tStat);
                }
            }
        }
        public static OfficerTypeStat OfficerStatById(int Id)
        {
            if (StaticMembers.statDict == null)
                CreateStatDict();
            if(StaticMembers.statDict.ContainsKey(Id))
            {
                return StaticMembers.statDict[Id];
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
            foreach(OfficerTypeStat stat in Stats )
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
            StaticMembers.OfficerTypeDict = new Dictionary<int, CrewOfficerType>();
            List<CrewOfficerType> lst = GetTypeList();
            foreach(CrewOfficerType t in lst)
            {
                StaticMembers.OfficerTypeDict.Add(t.Id, t);
            }
        }
        public static CrewOfficerType OfficerById(int Id)
        {
            if (StaticMembers.OfficerTypeDict == null)
                CreateDict();
            if(StaticMembers.OfficerTypeDict.ContainsKey(Id))
            {
                return StaticMembers.OfficerTypeDict[Id];
            }
            else
            {
                return null;
            }
        }

    }

    public class CrewOfficer : UnityCrewOfficer
    {


        public CrewOfficer() { }

        /// <summary>
        /// Generates new officer from template (officerType)
        /// </summary>
        /// <param name="officerType"></param>
        public CrewOfficer(CrewOfficerType officerType, int PlayerId)
        {
            this.OfficerType = officerType;
            this.PlayerId = PlayerId;
            this.SkillSetPoints = 1;
            Stats = new List<CrewOfficerStat>();
            foreach (OfficerTypeStat curStat in OfficerType.Stats)
            {
                CrewOfficerStat newStat = new CrewOfficerStat();
                newStat.BaseStat = CrewOfficerType.OfficerStatById(curStat.Id);
                Stats.Add(newStat);
            }

            Random rnd = new Random();
            int maxRange = Stats.Count;
            for(int i=0; i < officerType.BonusPoints; i++)
            {
                Stats[rnd.Next(0, maxRange)].AddStat();
            }

            //Провека того, есть ли запись в таблице crew_officers, которая соответствует
            //данному игроку
            if(Id == 0)
            {
                Save();
            }

            LoadSkillsets();

        }

        public CrewOfficer(SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            OfficerType = CrewOfficerType.OfficerById(Convert.ToInt32(r["officer_type_id"]));
            LoadStats();
            LoadSkillsets();
        }

        /// <summary>
        /// Создание офицера из персонажа игрока.
        /// Главная фишка процедуры - трансляция параметров игрока в параметры офицера
        /// </summary>
        /// <param name="acc"></param>
        public CrewOfficer(int playerId)
        {
            Stats = new List<CrewOfficerStat>();
            this.PlayerId = playerId;
            /*acc = new AccountData(playerId);
            this.playerStats = new AdmiralStats(ref acc);*/
            this.SkillSetPoints = 1;
            List<OfficerTypeStat> statTypes = CrewOfficerType.CreateStats();
            foreach(OfficerTypeStat statType in statTypes)
            {
                /*int score = 0;
                switch(statType.StatTypeFromString)
                {
                    case OfficerTypeStat.StatType.ArmorBoost:
                        score = playerStats.GetSpecificStat("Armor mastery").Value;
                        break;
                    case OfficerTypeStat.StatType.Dexterity:
                        score = playerStats.GetSpecificStat("Thrusters").Value;
                        break;
                    case OfficerTypeStat.StatType.ReactorBoost:
                        score = playerStats.GetSpecificStat("Reactors").Value;
                        break;
                    case OfficerTypeStat.StatType.LaserWeapons:
                        score = playerStats.GetSpecificStat("Laser weapons").Value;
                        break;
                    case OfficerTypeStat.StatType.ExplosiveWeapons:
                        score = playerStats.GetSpecificStat("Explosive weapons").Value;
                        break;
                    case OfficerTypeStat.StatType.KineticWeapons:
                        score = playerStats.GetSpecificStat("Kinetic weapons").Value;
                        break;
                    case OfficerTypeStat.StatType.ShieldsBoost:
                        score = playerStats.GetSpecificStat("Shield mastery").Value;
                        break;

                    default:
                        break;
                    
                }
                if (score > 0)
                {
                    CrewOfficerStat stat = new CrewOfficerStat();
                    stat.BaseStat = statType;
                    stat.BonusPoints = Convert.ToInt32(score * 3.14);
                    Stats.Add(stat);
                }
                    
                */
            }
        }

        private void LoadStats()
        {
            Stats = new List<CrewOfficerStat>();
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
                    CrewOfficerStat stat = new CrewOfficerStat(r);
                    Stats.Add(stat);
                }
            }
            r.Close();
        }

        //Загрузка наборов скиллов, которые открыты у данного офицера
        private void LoadSkillsets()
        {

            SkillSets = new List<UnityCrewOfficerSkillSet>();

            string q = $@"
                SELECT
                    id,
                    crew_officer_id,
                    skillset_id,
                    experience,
                    ISNULL(skill_points_total, 0) AS skill_points_total,
                    ISNULL(skill_points_left, 0) AS skill_points_left,
                    opened_skills
                FROM
                    crew_officers_skillsets
                WHERE
                    crew_officer_id = {Id}";
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    SkillSets.Add(new CrewOfficerSkillSet(r));
                }
            }
        }

        public void Save()
        {
            //Теперь офицера из игрока сохраняем точно так же, но без статов
            //if (IsPlayer)
            //    return;

            if (StaticMembers.OfficerDict == null)
                CrewOfficer.CreateDictionary();

            string q;
            if(Id == 0)
            {
                q = $@"
                    INSERT INTO crew_officers(player_id) VALUES({PlayerId})
                    SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
                StaticMembers.OfficerDict.Add(Id, this);
            }

            q = $@"
                UPDATE crew_officers SET
                    officer_type_id = {OfficerType.Id},
                    skill_set_points = {SkillSetPoints}
                WHERE
                    id = {Id}";
            DataConnection.Execute(q);

            //Статы офицера сохраняются только в том случае, если он создан не на основе персонажа игрока
            if(PlayerId == 0)
            {
                string IdsDoNotDelete = "";
                foreach (CrewOfficerStat stat in Stats)
                {
                    stat.SaveData(Id);
                    if (IdsDoNotDelete != "")
                        IdsDoNotDelete += ",";
                    IdsDoNotDelete += stat.Id.ToString();
                }

                q = $@"DELETE FROM crew_officers_stats WHERE crew_officer_id = {Id} AND id NOT IN({IdsDoNotDelete})";
                DataConnection.Execute(q);
            }


        }

        public bool IsPlayer
        {
            get
            {
                //return acc != null;
                return false;
            }
        }

        public override string ToString()
        {
            /*if(acc == null)
            {
                return $"{OfficerType.Name} ({OfficerType.Id})";
            }
            else
            {
                return acc.Name;
            }
            */
            return $"{OfficerType.Name} ({OfficerType.Id})";
        }

        public void Delete()
        {
            if (Id == 0) //Если еще не записан то удалять нечего
                return;
            /*if (acc != null)//Если создан из офицера, то тоже (это лишняя проверка, т.к. идентификатора всё равно не будет, но пусть)
            {
                return;
            }*/
                
            string q;
            q = $@"
                DELETE FROM crew_officers WHERE id = {Id};
                DELETE FROM crew_officers_stats WHERE crew_officer_id = {Id}";

            DataConnection.Execute(q);
        }


        private static void CreateDictionary()
        {
            StaticMembers.OfficerDict = new Dictionary<int, CrewOfficer>();
            string q = OfficerQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while(r.Read())
                {
                    CrewOfficer curOfficer = new CrewOfficer(r);
                    StaticMembers.OfficerDict.Add(curOfficer.Id, curOfficer);
                }
            }
            r.Close();
        }
        public static CrewOfficer OfficerById(int Id)
        {
            if (StaticMembers.OfficerDict == null)
                CreateDictionary();

            if(StaticMembers.OfficerDict.ContainsKey(Id))
            {
                return StaticMembers.OfficerDict[Id];
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
            if (onlyFree)
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
            tList.Add(new CrewOfficer(playerId));
            return tList;
        }
        private static string OfficerQuery()
        {
            string q;
            q = $@"
                SELECT
                    id,
                    player_id,
                    officer_type_id,
                    ISNULL(skill_set_points, 0) AS skill_set_points
                FROM
                    crew_officers";
            return q;
        }
    }

    public class OfficerTypeStat : Crew.UnityOfficerTypeStat
    {

        public OfficerTypeStat() { }

        public OfficerTypeStat(ref SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            TypeId = Convert.ToInt32(r["crew_officer_type_id"]);
            Name = Convert.ToString(r["stat_name"]);
            PointsBase = Convert.ToInt32(r["points_base"]);
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

    public class CrewOfficerStat : UnityCrewOfficerStat
    {

        public CrewOfficerStat() { }
        public CrewOfficerStat(SqlDataReader r)
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
            if (Id == 0)
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

    public class CrewOfficerSkillSet : UnityCrewOfficerSkillSet
    {
        public CrewOfficerSkillSet() { }
        public CrewOfficerSkillSet(SqlDataReader r)
        {
            Id = (int)r["id"];
            CrewOfficerId = (int)r["crew_officer_id"];
            SkillSetId = (int)r["skillset_id"];
            Experience = (int)r["experience"];
            SkillPointsTotal = (int)r["skill_points_total"];
            SkillPointsLeft = (int)r["skill_points_left"];
            OpenedSkills = (string)r["opened_skills"];
        }

        public void SaveData(int CrewOfficerId)
        {
            string q;
            if(Id == 0)
            {
                q = $@"
                    INSERT INTO crew_officers_skillsets(crew_officer_id) VALUES ({CrewOfficerId})
                    SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
            }

            q = $@"UPDATE crew_officers_skillsets SET 
                    crew_officer_id = {CrewOfficerId},
                    skillset_id = {SkillSetId},
                    experience = {Experience},
                    skill_points_total = {SkillPointsTotal},
                    skill_points_left = {SkillPointsLeft},
                    opened_skills = @str1
                WHERE
                    id = {Id}";
            List<string> names = new List<string> { OpenedSkills };
            DataConnection.Execute(q, names);
        }

    }

    public class UnityCrewOfficerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AvailableAtStart { get; set; }
        public int BonusPoints { get; set; }
        public int PortraitId { get; set; }

        public List<OfficerTypeStat> Stats { get; set; }
    }

    public class UnityOfficerTypeStat
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public int PointsBase { get; set; }

        public enum StatType
        {
            None = 0,
            LaserWeapons = 1,
            ExplosiveWeapons = 2,
            KineticWeapons = 3,
            ReactorBoost = 21,
            ThrustersBoost = 22,
            ShieldsBoost = 31,
            ArmorBoost = 32,
            Repair = 41,
            Dexterity = 42,
            LootBoost = 51
        }

        public StatType StatTypeFromString
        {
            get
            {
                switch (Name)
                {
                    case "Laser weapons":
                        return StatType.LaserWeapons;
                    case "Explosive weapons":
                        return StatType.ExplosiveWeapons;
                    case "Kinetic weapons":
                        return StatType.KineticWeapons;
                    case "Reactors":
                        return StatType.ReactorBoost;
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
            switch (sType)
            {
                case StatType.LaserWeapons:
                    return "Laser weapons";
                case StatType.ExplosiveWeapons:
                    return "Explosive weapons";
                case StatType.KineticWeapons:
                    return "Kinetic weapons";
                case StatType.ReactorBoost:
                    return "Reactors";
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

    }

    public class UnityCrewOfficer
    {
        public int Id { get; set; }

        public CrewOfficerType OfficerType { get; set; }

        public List<CrewOfficerStat> Stats { get; set; }
        
        //public AccountData acc;

        public int PlayerId { get; set; }

        //Сколько всего веток скиллов может открыть данный офицер
        public int SkillSetPoints { get; set; }

        public List<UnityCrewOfficerSkillSet> SkillSets { get; set; }

        //Для тех объектов, которые созданы на основе статов игрока
        //public AdmiralStats playerStats;

        public int StatValue(OfficerTypeStat.StatType param)
        {
            if (Stats.Count == 0)
                return 0;
            foreach (CrewOfficerStat stat in Stats)
            {
                if (stat.BaseStat.StatTypeFromString == param)
                {
                    return stat.Value;
                }
            }
            return 0;
        }

        public int WeaponStatSum()
        {
            int sum = 0;
            List<string> weaponStatList = new List<string> {"Laser weapons" , "Explosive weapons", "Kinetic weapons"};
            foreach(var stat in Stats)
            {
                if(weaponStatList.Contains(stat.BaseStat.Name))
                {
                    sum += stat.BonusPoints + stat.Value;
                }
            }
            return sum;
        }

    }

    public class UnityCrewOfficerStat
    {
        public int Id { get; set; } //Таблица crew_officers_stats
        public int BonusPoints { get; set; }
        public OfficerTypeStat BaseStat { get; set; }
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

        public override string ToString()
        {
            return $"{BaseStat.Name} : {Value}";
        }
    }

    public class UnityCrewOfficerSkillSet
    {
        public int Id { get; set; }
        public int CrewOfficerId { get; set; }
        public int SkillSetId { get; set; }
        public int Experience { get; set; }
        public int SkillPointsTotal { get; set; }
        public int SkillPointsLeft { get; set; }
        public string OpenedSkills { get; set; }

        public UnityCrewOfficerSkillSet() { }

    }

}