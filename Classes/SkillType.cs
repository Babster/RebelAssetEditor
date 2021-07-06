﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class SkillTypeSql : SkillType
{
    public SkillTypeSql() { }

    public SkillTypeSql(SqlDataReader r)
    {

        Id = (int)r["id"];
        ParentId = (int)r["parent_id"];
        Grade = (SkillGrages)r["grade"];
        Name = (string)r["name"];
        IconId = (int)r["icon_id"];
        OpenSkillpointsCost = (int)r["open_skillpoints_cost"];
        ActivationEnergyCost = (int)r["activation_energy_cost"];
        DurationMilliseconds = (int)r["duration_milliseconds"];
        EffectType = (SkillEffectTypes)r["effect_type"];
        EffectPower = (int)r["effect_power"];
        CooldownMilliseconds = (int)r["cooldown_milliseconds"];
    }

    public void SaveData()
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO skill_types(name) VALUES('')
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE skill_types SET 
                parent_id = {ParentId},
                grade = {(int)Grade},
                name = @str1,
                icon_id = {IconId},
                open_skillpoints_cost = {OpenSkillpointsCost},
                activation_energy_cost = {ActivationEnergyCost},
                duration_milliseconds = {DurationMilliseconds},
                effect_type = {(int)EffectType},
                effect_power = {EffectPower},
                cooldown_milliseconds = {CooldownMilliseconds}
            WHERE
                id = {Id}";

        List<string> names = new List<string> { Name };
        DataConnection.Execute(q, names);

    }

    public static List<SkillTypeSql> SkillTypeList()
    {
        List<SkillTypeSql> tList = new List<SkillTypeSql>();
        string q = SkillTypeQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                tList.Add(new SkillTypeSql(r));
            }
        }
        return tList;
    }

    public static string SkillTypeQuery()
    {
        return $@"
            SELECT
                id,
                parent_id,
                grade,
                name,
                icon_id,
                open_skillpoints_cost,
                activation_energy_cost,
                duration_milliseconds,
                effect_type,
                effect_power,
                ISNULL(cooldown_milliseconds, 0) AS cooldown_milliseconds
            FROM
                skill_types
            ORDER BY
                parent_id, id";
    }

    private static Dictionary<int, SkillType> skillTypeDict;
    public static void LoadSkillTypeDict(bool forceUpdate)
    {
        if(skillTypeDict != null && !forceUpdate)
        {
            return;
        }
        skillTypeDict = new Dictionary<int, SkillType>();
        List<SkillTypeSql> tList = SkillTypeList();
        foreach(var element in tList)
        {
            skillTypeDict.Add(element.Id, element);
        }
    }
    public static SkillType SkillTypeById(int id)
    {
        LoadSkillTypeDict(false);
        if(skillTypeDict.ContainsKey(id))
        {
            return skillTypeDict[id];
        }
        else
        {
            return null;
        }
    }

}

public class SkillType
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public SkillGrages Grade { get; set; }
    public string Name { get; set; }
    public int IconId { get; set; }
    public int OpenSkillpointsCost { get; set; }
    public int ActivationEnergyCost { get; set; }
    public int DurationMilliseconds { get; set; }
    public SkillEffectTypes EffectType { get; set; }
    public int EffectPower { get; set; }
    public int CooldownMilliseconds { get; set; }

    public enum SkillGrages
    {
        None = 0,
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5
    }
    
    public enum SkillEffectTypes
    {
        None = 0,

        //Active skills
        RepairArmor = 1,
        RepairStructure = 2,
        RestoreShield = 3,
        ActiveDexterity = 4,

        Invincibility = 10,
        DamageAllEnemies = 20,


        //Passive skils - defence and energy
        AddArmorPercent = 30,
        AddStructurePercent = 31,
        AddShieldPercent = 32,
        AddShieldRechargeRate = 33,
        AddEnergy = 34,
        RepairArmorContinuosly = 35,
        RepairStructureContinuosly = 36,
        PassiveDexterity = 37,
        AddEnergyPercent = 38,

        //Passive skills - weapons
        AddDamagePercent = 40,
        AddFireRatePercent = 41
        
    }

    public SkillType() { }

}

