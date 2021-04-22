﻿using System;
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
                CreateStats();
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
            q = @"SELECT
                id,
                crew_officer_type_id,
                stat_name,
                points_base
            FROM
                crew_officers_types_stats
            WHERE
                crew_officer_type_id = " + this.Id;

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

        private void CreateStats()
        {
            Stats = new List<OfficerStat>();
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
                FireRate = 1,
                ShieldDamage = 2,
                StructureDamage = 3,
                EngineBoost = 4,
                ThrustersBoost = 5,
                ShieldsBoost = 6,
                ArmorBoost = 7,
                Repair = 8,
                Dexterity = 9,
                LootBoost = 10
            }

            public static StatType StatTypeFromString(string statType)
            {
                switch(statType)
                {
                    case "Fire rate":
                        return StatType.FireRate;
                    case "Shield damage":
                        return StatType.ShieldDamage;
                    case "Structure damage":
                        return StatType.StructureDamage;
                    case "Engine boost":
                        return StatType.EngineBoost;
                    case "Thrusters boost":
                        return StatType.ThrustersBoost;
                    case "Shields boost":
                        return StatType.ShieldsBoost;
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

            public static string StatTypeToString(StatType sType)
            {
                switch(sType)
                {
                    case StatType.FireRate:
                        return "Fire rate";
                    case StatType.ShieldDamage:
                        return "Shield damage";
                    case StatType.StructureDamage:
                        return "Structure damage";
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

    }

}