using AdmiralNamespace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Crew
{

    #region Crew officer and his stats

    public class CrewOfficer : UnityCrewOfficer
    {

        public CrewOfficer() { }

        /// <summary>
        /// Generates new officer from template (officerType)
        /// </summary>
        /// <param name="officerType"></param>
        public CrewOfficer(CrewOfficerType officerType, int PlayerId)
        {
            this.OfficerTypeId = officerType.Id;
            this.PlayerId = PlayerId;
            this.SkillSetPoints = 1;
            Stats = new List<CrewOfficerStat>();
            foreach (OfficerTypeStat curStat in OfficerType.Stats)
            {
                CrewOfficerStat newStat = new CrewOfficerStat();
                newStat.OfficerStatTypeId = curStat.OfficerStatTypeId;
                newStat.Value = curStat.PointsBase;
                Stats.Add(newStat);
            }

            Random rnd = new Random();
            int maxRange = Stats.Count;
            for (int i = 0; i < officerType.BonusPoints; i++)
            {
                Stats[rnd.Next(0, maxRange)].AddStat();
            }

            //Провека того, есть ли запись в таблице crew_officers, которая соответствует
            //данному игроку
            if (Id == 0)
            {
                Save();
            }
        }

        public CrewOfficer(SqlDataReader r)
        {
            LoadOfficerByReader(r);
        }

        /// <summary>
        /// Создание или загрузка офицера, который создан из персонажа игрока
        /// </summary>
        /// <param name="acc"></param>
        public CrewOfficer(int playerId)
        {
            //Это идёт блок когда на основе игрока ещё не был создан офицер и нужно его создать
            //При этом сразу идёт запись в БД
            string q = CrewOfficer.OfficerQuery() + $@" WHERE player_id = {playerId} AND is_player = 1";
            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                r.Read();
                LoadOfficerByReader(r);
                r.Close();
                return;
            }
            r.Close();

            Stats = new List<CrewOfficerStat>();
            this.PlayerId = playerId;
            this.IsPlayer = 1;
            this.SkillSetPoints = 1; //Это сколько можно открыть (не относится к статам самого офицера)
            this.StatPointsLeft = CommonFunctions.GetCommonValue("start_stat_points").IntValue;
            var statTypeList = OfficerStatTypeSql.GetStatTypeList();
            Stats = new List<CrewOfficerStat>();
            foreach(var statTemplate in statTypeList)
            {
                var curStat = new CrewOfficerStat();
                curStat.OfficerStatTypeId = statTemplate.Id;
                curStat.Value = statTemplate.BaseValue;
                Stats.Add(curStat);
            }

            Save();

        }

        private void LoadOfficerByReader(SqlDataReader r)
        {
            Id = (int)r["id"];
            PlayerId = (int)r["player_id"];
            OfficerTypeId = (int)r["officer_type_id"];
            RigId = (int)r["rig_id"];
            IsPlayer = (int)r["is_player"];
            SkillSetPoints = (int)r["skill_set_points"];
            OfficerGuid = (Guid)r["unique_code"];
            StatPointsLeft = (int)r["stat_points_left"];
            LoadStats();
            LoadSkillsets();
        }

        private Dictionary<int, CrewOfficerStat> officerStatDict;
        private void LoadOfficerStatDict()
        {
            if(officerStatDict != null)
            {
                return;
            }
            officerStatDict = new Dictionary<int, CrewOfficerStat>();
            foreach(var item in Stats)
            {
                officerStatDict.Add(item.OfficerStatTypeId, item);
            }
        }
        public CrewOfficerStat StatById(int id)
        {
            LoadOfficerStatDict();
            if(officerStatDict.ContainsKey(id))
            {
                return officerStatDict[id];
            }
            else
            {
                return null;
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
                    stat_type_id,
                    value
                FROM
                    crew_officers_stats
                WHERE
                    crew_officer_id = {Id}";
            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
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
            skillSetDict = new Dictionary<int, CrewOfficerSkillSet>();

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
            if (r.HasRows)
            {
                while (r.Read())
                {
                    CrewOfficerSkillSet curSkillSet = new CrewOfficerSkillSet(r);
                    SkillSets.Add(curSkillSet);
                    skillSetDict.Add(curSkillSet.SkillSetId, curSkillSet);
                }
            }
            r.Close();
        }
        public void Save()
        {
            if (StaticMembers.OfficerDict == null)
                CrewOfficer.CreateDictionary();

            string q;

            //Создание и обновление основной записи в таблице офицеров
            if (Id == 0)
            {
                this.OfficerGuid = Guid.NewGuid();
                q = $@"
                    INSERT INTO crew_officers(player_id, unique_code) VALUES({PlayerId}, CAST('{this.OfficerGuid.ToString()}' AS uniqueidentifier))
                    SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
                StaticMembers.OfficerDict.Add(Id, this);
            }

            q = $@"
                UPDATE crew_officers SET
                    player_id = {PlayerId},
                    officer_type_id = {OfficerTypeId},
                    rig_id = {RigId},
                    is_player = {IsPlayer},
                    skill_set_points = {SkillSetPoints},
                    stat_points_left = {StatPointsLeft}
                WHERE
                    id = {Id}";
            DataConnection.Execute(q);

            string IdsDoNotDelete = "";

            //Сохранение статов офицера
            foreach (CrewOfficerStat stat in Stats)
            {
                stat.SaveData(Id);
                if (IdsDoNotDelete != "")
                    IdsDoNotDelete += ",";
                IdsDoNotDelete += stat.Id.ToString();
            }
            q = $@"DELETE FROM crew_officers_stats WHERE crew_officer_id = {Id}";
            if(IdsDoNotDelete != "")
            {
                q = q + $" AND id NOT IN({IdsDoNotDelete})";
            }
            DataConnection.Execute(q);

            //Сохранение наборов скиллов офицера


        }
        public override string ToString()
        {
            if(IsPlayer == 0)
            {
                return $"{OfficerType.Name} ({Id})";
            }
            else
            {
                return PlayerDataSql.GetPlayerData(PlayerId).DisplayName;
            }
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
                while (r.Read())
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

            if (StaticMembers.OfficerDict.ContainsKey(Id))
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
            bool playerLoaded = false;
            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    int id = Convert.ToInt32(r["id"]);
                    var curOfficer = OfficerById(id);
                    if(curOfficer.IsPlayer == 1)
                    {
                        playerLoaded = true;
                    }
                    tList.Add(curOfficer);
                    
                }
            }
            r.Close();
            if(!playerLoaded)
            {
                tList.Add(new CrewOfficer(playerId));
            }
            return tList;
        }
        public static string RegisterOfficerStatsChanged(int playerId, CrewOfficer changedOfficer)
        {

            var officers = OfficersForPlayer(playerId);
            CrewOfficer curOfficer = null;
            foreach(var officer in officers)
            {
                if(officer.OfficerGuid == changedOfficer.OfficerGuid)
                {
                    curOfficer = officer;
                    break;
                }
            }

            if(curOfficer == null)
            {
                return "changing officer not found";
            }

            PlayerData curPlayer = PlayerDataSql.GetPlayerData(playerId);

            foreach(var stat in curOfficer.Stats)
            {
                CrewOfficerStat changedStat = changedOfficer.StatById(stat.OfficerStatTypeId);
                if(changedStat != null)
                {
                    int pointsAdded = changedStat.Value - stat.Value;
                    if(pointsAdded < 0)
                    {
                        return $"stat value can't be decreased: {changedStat.OfficerStatType.Name}";
                    }
                    if(pointsAdded > curOfficer.StatPointsLeft)
                    {
                        return $"not enough skill points";
                    }

                    curOfficer.StatPointsLeft -= pointsAdded;
                    stat.Value = changedStat.Value;

                }
            }

            curOfficer.Save();
            return "";
        }

        private static Dictionary<Guid, int> officerCodesDict;
        public static CrewOfficer OfficerByCode(Guid code)
        {
            if(officerCodesDict == null)
            {
                officerCodesDict = new Dictionary<Guid, int>();
            }
            if(officerCodesDict.ContainsKey(code))
            {
                return OfficerById(officerCodesDict[code]);
            }
            else
            {
                int id = DataConnection.GetResultInt($"SELECT id FROM crew_officers WHERE unique_code = CAST('{code.ToString()}' AS uniqueidentifier)");
                officerCodesDict.Add(code, id);
                return OfficerById(id);
            }
        }
        private static string OfficerQuery()
        {
            string q;
            q = $@"
                SELECT
                    id,
                    player_id,
                    officer_type_id,
                    rig_id,
                    is_player,
                    skill_set_points,
                    unique_code,
                    stat_points_left
                FROM
                    crew_officers";
            return q;
        }

        /// <summary>
        /// Словарь идентификаторов офицеров, которые созданы на основе игроков. Ключом является идентификатор игрока,
        /// значением - идентификатором офицера в таблице officers
        /// </summary>
        private static Dictionary<int, int> playerOfficersDict;
        public static int PlayerOfficerId(int playerId)
        {
            if(playerOfficersDict == null)
            {
                playerOfficersDict = new Dictionary<int, int>();
            }
            if(playerOfficersDict.ContainsKey(playerId))
            {
                return playerOfficersDict[playerId];
            }
            else
            {
                string q = $@"SELECT id FROM crew_officers WHERE player_id = {playerId} AND is_player = 1";
                int id = DataConnection.GetResultInt(q,null, 0);
                if(id == 0)
                {
                    var newOfficer = new CrewOfficer(playerId);
                    officerCodesDict.Add(newOfficer.OfficerGuid, newOfficer.Id);
                    StaticMembers.OfficerDict.Add(newOfficer.Id, newOfficer);
                    id = newOfficer.Id;
                }
                playerOfficersDict.Add(playerId, id);
                return id;
            }
        }

        /// <summary>
        /// Ключом является идентификатор данного скиллсета в таблице skill_sets!!!
        /// </summary>
        private Dictionary<int, CrewOfficerSkillSet> skillSetDict;
        public void RegisterSkillSetChanges(OfficerExperience officerExperience)
        {
            foreach(OfficerExperience.SkillSet expSkillSet in officerExperience.SkillSetList)
            {
                CrewOfficerSkillSet curSkillSet = null;
                if(skillSetDict.ContainsKey(expSkillSet.SkillSetId))
                {
                    curSkillSet = skillSetDict[expSkillSet.SkillSetId];
                }
                if(curSkillSet == null)
                {
                    curSkillSet = new CrewOfficerSkillSet();
                    curSkillSet.CrewOfficerId = Id;
                    curSkillSet.SkillSetId = expSkillSet.SkillSetId;
                    skillSetDict.Add(expSkillSet.SkillSetId, curSkillSet);
                    SkillSets.Add(curSkillSet);
                }
                curSkillSet.Experience = expSkillSet.Experience;
                curSkillSet.SkillPointsTotal = expSkillSet.SkillPointsTotal;
                curSkillSet.SkillPointsLeft = expSkillSet.SkillPointsLeft;
                curSkillSet.OpenedSkills = expSkillSet.OpenedSkills;
                curSkillSet.SaveData(Id);
            }
        }

    }
    public class UnityCrewOfficer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int OfficerTypeId { get; set; }
        public int RigId { get; set; }
        public int IsPlayer { get; set; }
        public int SkillSetPoints { get; set; }        //Сколько всего веток скиллов может открыть данный офицер
        public Guid OfficerGuid { get; set; }
        public int StatPointsLeft { get; set; } //Сколько очков скиллов офицера еще осталось (сколько можно очков еще вложить в public List<CrewOfficerStat> Stats)
        public List<CrewOfficerStat> Stats { get; set; } //Список статов офицера
        public List<UnityCrewOfficerSkillSet> SkillSets { get; set; } //Ветки скиллов, которые открыты у данного офицера

        [JsonIgnore]
        public CrewOfficerType OfficerType
        {
            get
            {
                return CrewOfficerType.OfficerTypeById(OfficerTypeId);
            }
        }

        [JsonIgnore]
        private Dictionary<OfficerStatType.StatType, CrewOfficerStat> statTypeDict;
        private void CreateStatTypeDict()
        {
            if (statTypeDict != null)
            {
                return;
            }
            statTypeDict = new Dictionary<OfficerStatType.StatType, CrewOfficerStat>();
            foreach(var stat in Stats)
            {
                if(stat.OfficerStatType.statType != OfficerStatType.StatType.None && !statTypeDict.ContainsKey(stat.OfficerStatType.statType))
                {
                    statTypeDict.Add(stat.OfficerStatType.statType, stat);
                }
            }
        }
        //Это получение значения скилла офицера по его встроенному типу
        public int StatValue(OfficerStatType.StatType statType)
        {
            CreateStatTypeDict();
            if(statTypeDict.ContainsKey(statType))
            {
                return statTypeDict[statType].Value;
            }
            else
            {
                return 0;
            }
        }

        public int WeaponStatSum()
        {
            List<OfficerStatType.StatType> weaponStatTypes = new List<OfficerStatType.StatType>();
            weaponStatTypes.Add(OfficerStatType.StatType.LaserWeapons);
            weaponStatTypes.Add(OfficerStatType.StatType.KineticWeapons);
            weaponStatTypes.Add(OfficerStatType.StatType.ExplosiveWeapons);

            int sum = 0;
            foreach(var stat in weaponStatTypes)
            {
                sum += StatValue(stat);
            }

            return sum;

        }

    }
    public class CrewOfficerStat : UnityCrewOfficerStat
    {

        public CrewOfficerStat() { }
        public CrewOfficerStat(SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            OfficerStatTypeId = Convert.ToInt32(r["stat_type_id"]);
            Value = Convert.ToInt32(r["value"]);
        }
        public void AddStat() { Value += 1; }

        public override string ToString()
        {
            return $"{OfficerStatType.Name} : {Value}";
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
                        stat_type_id = {OfficerStatTypeId}, 
                        value = {Value}
                    WHERE
                        id = {Id}";
            DataConnection.Execute(q);
        }

    }
    public class UnityCrewOfficerStat
    {
        public int Id { get; set; } //Таблица crew_officers_stats
        public int OfficerStatTypeId { get; set; }
        public int Value { get; set; }

        //Сделано для того, чтобы игрок не мог уменьшать значение стата, если он уже выбран.
        [JsonIgnore]
        public int MinimalValue { get; set; }

        [JsonIgnore]
        public OfficerStatType OfficerStatType 
        { 
            get
            {
                return OfficerStatTypeSql.OfficerStatTypeById(OfficerStatTypeId);
            }
        }

        public override string ToString()
        {
            return $"{OfficerStatType.Name} : {Value}";
        }
    }

    #endregion


    #region Crew officer type and its stats

    public class CrewOfficerType : UnityCrewOfficerType
    {

        public CrewOfficerType() 
        {
            CreateStats();
        }

        public CrewOfficerType(ref SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            Name = Convert.ToString(r["name"]);
            BonusPoints = Convert.ToInt32(r["bonus_points"]);
            PortraitId = Convert.ToInt32(r["portrait_id"]);
            AvailableSkillSetsStr = (string)r["crew_officers_skill_sets"];
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
                while (r.Read())
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
                stat_type_id,
                points_base
            FROM
                crew_officers_types_stats";
            return q;
        }

        private void CreateStats()
        {
            Stats = new List<OfficerTypeStat>();
            var statTemplates = OfficerStatTypeSql.GetStatTypeList();
            foreach(var statTemplate in statTemplates)
            {
                var curStat = new OfficerTypeStat();
                curStat.OfficerStatTypeId = statTemplate.Id;
                curStat.PointsBase = 0;
                Stats.Add(curStat);
            }
        }

        public void SaveData()
        {

            string q;
            if (this.Id == 0)
            {
                q = @"INSERT INTO crew_officers_types(name) VALUES('')
                        SELECT @@IDENTITY AS Result";
                this.Id = DataConnection.GetResultInt(q);
            }

            q = $@"
                UPDATE crew_officers_types SET
                    name = @str1,
                    bonus_points = {BonusPoints},
                    portrait_id = {PortraitId},
                    crew_officers_skill_sets = @str2
                WHERE 
                    id = {Id}";

            List<string> names = new List<string> { this.Name, this.AvailableSkillSetsStr };
            DataConnection.Execute(q, names);

            //Saving stats
            foreach (OfficerTypeStat stat in Stats)
            {
                stat.OfficerTypeId = Id;
                stat.Save();
            }

        }

         public override string ToString()
        {
            return $"{Name}: {BonusPoints}";
        }

         public static string OfficerTypeQuery()
        {
            string q = @"
                SELECT
                    id,
                    name,
                    bonus_points,
                    portrait_id,
                    ISNULL(crew_officers_skill_sets, '') AS crew_officers_skill_sets
                FROM
                    crew_officers_types";
            return q;
        }

        public static List<CrewOfficerType> GetTypeList()
        {
            List<CrewOfficerType> res = new List<CrewOfficerType>();
            string q = OfficerTypeQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
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
            foreach (CrewOfficerType t in lst)
            {
                StaticMembers.OfficerTypeDict.Add(t.Id, t);
            }
        }
        public static CrewOfficerType OfficerTypeById(int Id)
        {
            if (StaticMembers.OfficerTypeDict == null)
                CreateDict();
            if (StaticMembers.OfficerTypeDict.ContainsKey(Id))
            {
                return StaticMembers.OfficerTypeDict[Id];
            }
            else
            {
                return null;
            }
        }

    }

    public class OfficerTypeStat : UnityOfficerTypeStat
    {

        public OfficerTypeStat() { }

        public OfficerTypeStat(ref SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            OfficerTypeId = (int)r["crew_officer_type_id"];
            OfficerStatTypeId = Convert.ToInt32(r["stat_type_id"]);
            PointsBase = Convert.ToInt32(r["points_base"]);
        }

        public void Save()
        {
            string q;
            List<string> names = new List<string> { OfficerStatType.Name };
            if (Id == 0)
            {
                q = $@"INSERT INTO crew_officers_types_stats
                    (
                        crew_officer_type_id, 
                        stat_type_id, 
                        points_base
                    ) VALUES (
                        {OfficerTypeId},
                        {OfficerStatTypeId},
                        {PointsBase}
                    )
                    SELECT @@IDENTITY AS Result";
                this.Id = DataConnection.GetResultInt(q, names);
            }
            else
            {
                q = $@"UPDATE crew_officers_types_stats SET points_base = {PointsBase} WHERE id = {Id}";
                DataConnection.Execute(q, names);
            }

        }

        public override string ToString()
        {
            return $"{OfficerStatType.Name} ({PointsBase})";
        }

    }
    
    public class UnityCrewOfficerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public int BonusPoints { get; set; }
        public int PortraitId { get; set; }
        public string AvailableSkillSetsStr { get; set; }
        [JsonIgnore]
        public List<OfficerTypeStat> Stats { get; set; }
    }

    public class UnityOfficerTypeStat
    {
        public int Id { get; set; }
        public int OfficerTypeId { get; set; }
        public int OfficerStatTypeId { get; set; }
        public int PointsBase { get; set; }
 
        [JsonIgnore]
        public OfficerStatType OfficerStatType 
        { 
            get
            {
                return OfficerStatTypeSql.OfficerStatTypeById(OfficerStatTypeId);
            }
        }

   }

    #endregion

    #region Officer stat type

    public class OfficerStatTypeSql : OfficerStatType
    {
        public OfficerStatTypeSql(SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            this.SkillGroup = Convert.ToString(r["skill_group"]);
            this.Name = Convert.ToString(r["name"]);
            this.OrderIdx = Convert.ToInt32(r["order_idx"]);
            this.BaseValue = Convert.ToInt32(r["base_value"]);
            this.DescriptionEnglish = Convert.ToString(r["description_english"]);
            this.DescriptionRussian = Convert.ToString(r["description_russian"]);
            this.statType = (StatType)r["stat_type"];
        }

        public void SaveData()
        {

            string q;

            if (this.Id == 0)
            {
                q = @"INSERT INTO crew_officer_stat_types(name) VALUES('')
                         SELECT @@IDENTITY AS field0";
                this.Id = Convert.ToInt32(DataConnection.GetResult(q));
            }

            List<String> names = new List<string>();
            names.Add(this.SkillGroup);
            names.Add(this.Name);
            names.Add(DescriptionEnglish);
            names.Add(DescriptionRussian);
            q = $@"UPDATE crew_officer_stat_types SET
                        skill_group = @str1,
                        name = @str2,
                        base_value = {BaseValue},
                        description_english = @str3,
                        description_russian = @str4,
						order_idx = {OrderIdx},
                        stat_type = {(int)statType}
                    WHERE id = " + Id.ToString();
            DataConnection.Execute(q, names);
        }

        public static List<OfficerStatTypeSql> GetStatTypeList()
        {
            List<OfficerStatTypeSql> stats = new List<OfficerStatTypeSql>();

            string q;
            SqlDataReader r;

            q = @"SELECT 
                    id,
                    ISNULL(skill_group, '') AS skill_group,
                    name,
                    base_value,
                    description_english,
                    description_russian,
                    order_idx,
                    ISNULL(stat_type, 0) AS stat_type
                FROM
                    crew_officer_stat_types
                ORDER BY
                    order_idx";
            r = DataConnection.GetReader(q);
            while (r.Read())
            {
                stats.Add(new OfficerStatTypeSql(r));
            }
            r.Close();

            return stats;

        }

        private static Dictionary<int, OfficerStatTypeSql> officerStatTypeDict;
        private static void LoadOfficerDict()
        {
            if(officerStatTypeDict != null)
            {
                return;
            }
            officerStatTypeDict = new Dictionary<int, OfficerStatTypeSql>();
            List<OfficerStatTypeSql> statList = GetStatTypeList();
            foreach(var stat in statList)
            {
                officerStatTypeDict.Add(stat.Id, stat);
            }

        }
        public static OfficerStatTypeSql OfficerStatTypeById(int id)
        {
            LoadOfficerDict();
            if(officerStatTypeDict.ContainsKey(id))
            {
                return officerStatTypeDict[id];
            }
            else
            {
                return null;
            }
        }

    }


    public class OfficerStatType
    {
        public int Id { get; set; }
        public string SkillGroup { get; set; }
        public string Name { get; set; }
        public int BaseValue { get; set; }
        public string DescriptionEnglish { get; set; }
        public string DescriptionRussian { get; set; }
        public int OrderIdx { get; set; }
        public StatType statType { get; set; }

        public OfficerStatType() { }

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

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }

    }

    #endregion

    #region Officer skillset and experiencing

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
            if (Id == 0)
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

    public class OfficerExperience
    {
        public Guid OfficerCode { get; set; }
        public List<SkillSet> SkillSetList { get; set; }

        public OfficerExperience() { }

        public class SkillSet
        {
            public int SkillSetId { get; set; }
            public int Experience { get; set; }
            public int SkillPointsTotal { get; set; }
            public int SkillPointsLeft { get; set; }
            public string OpenedSkills { get; set; }
        }

        public void SaveData()
        {
            CrewOfficer curOfficer = CrewOfficer.OfficerByCode(OfficerCode);
            curOfficer.RegisterSkillSetChanges(this);
        }

    }

    #endregion




}