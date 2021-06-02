using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;

public class ShipModuleType : UnityShipModuleType
{

    public ShipModuleType() { }

    public ShipModuleType(SqlDataReader r)
    {
        Id = (int)r["id"];
        IsCategory = (int)r["is_category"];
        Parent = (int)r["parent"];
        Name = (string)r["name"];
        AssetName = (string)r["asset_name"];
        if (IsCategory == 0)
        {
            ModuleType = (ModuleTypes)r["module_type_int"];
            Size = (int)r["size"];
            EnergyNeed = (int)r["energy_needed"];
            ImgId = (int)r["img_id"];
            WeaponType = (WeaponTypes)r["weapon_type"];
            FireRate = (int)r["fire_rate"];
            DamageAmount = (int)r["damage_amount"];
            ShieldEffectiveness = (int)r["shield_effectiveness"];
            ArmorEffectiveness = (int)r["armor_effectiveness"];
            StructureEffectiveness = (int)r["structure_effectiveness"];
            IgnoreShield = (int)r["ignore_shield"];
            CriticalChance = (int)r["critical_chance"];
            CriticalStrength = (int)r["critical_strength"];
            ArmorPoints = (int)r["armor_points"];
            ReactorPoints = (int)r["reactor_points"];
            ThrustStrength = (int)r["thrust_strength"];
            Dexterity = (int)r["dexterity"];
            ShieldPoints = (int)r["shield_points"];
            ShieldRegen = (int)r["shield_regen"];
        }
    }

    /*public ShipModuleType(ShipModuleType module)
    {
        Id = module.Id;
        IsCategory = module.IsCategory;
        Parent = module.Parent;
        Name = module.Name;
        AssetName = module.AssetName;
        ModuleType = ConvertModuleType(module.ModuleTypeStr);
        Size = module.Size;
        EnergyNeed = module.EnergyNeeded;
        ImgId = module.ImgId;

        foreach (var oldParameter in module.parameters.ParameterList)
        {
            switch (oldParameter.ParamType)
            {
                case SpaceshipParameters.SpaceShipParameter.ArmorPoints:
                    this.ArmorPoints = (int)oldParameter.Value;
                    break;
                case SpaceshipParameters.SpaceShipParameter.Dexterity:
                    this.Dexterity = (int)oldParameter.Value;
                    break;
                case SpaceshipParameters.SpaceShipParameter.Engine:
                    this.ReactorPoints = (int)oldParameter.Value;
                    break;
                case SpaceshipParameters.SpaceShipParameter.FireRate:
                    this.FireRate = (int)oldParameter.Value;
                    break;
                case SpaceshipParameters.SpaceShipParameter.ShieldPoints:
                    this.ShieldPoints = (int)oldParameter.Value;
                    break;
                case SpaceshipParameters.SpaceShipParameter.ShieldRegen:
                    this.ShieldRegen = (int)oldParameter.Value;
                    break;
                case SpaceshipParameters.SpaceShipParameter.Speed:
                    this.ThrustStrength = (int)oldParameter.Value;
                    break;
            }
        }
    }*/

    public void SaveData()
    {
        string q = "";
        if (Id == 0)
        {
            q = $@"
                INSERT INTO ss_modules(name) VALUES('')
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE ss_modules SET
                is_category = {IsCategory},
                parent = {Parent},
                name = @str1,
                asset_name = @str2,
                module_type_int = {(int)ModuleType},
                weapon_type = {(int)WeaponType},
                size = {Size.ToString()},
                energy_needed = {EnergyNeed},
                img_id = {ImgId},
                fire_rate = {FireRate},
                damage_amount = {DamageAmount},
                shield_effectiveness = {ShieldEffectiveness},
                armor_effectiveness = {ArmorEffectiveness},
                structure_effectiveness = {StructureEffectiveness},
                ignore_shield = {IgnoreShield},
                critical_chance = {CriticalChance},
                critical_strength = {CriticalStrength},
                armor_points = {ArmorPoints},
                reactor_points = {ReactorPoints},
                thrust_strength = {ThrustStrength},
                dexterity = {Dexterity},
                shield_points = {ShieldPoints},
                shield_regen = {ShieldRegen}
            WHERE
                id = {Id}";
        List<string> names = new List<string> { Name, AssetName };
        DataConnection.Execute(q, names);

    }

    private ModuleTypes ConvertModuleType(string ModuleTypeStr)
    {
        switch (ModuleTypeStr)
        {
            case "Weapon":
                return ModuleTypes.Weapon;
            case "Armor":
                return ModuleTypes.Armor;
            case "Reactor":
                return ModuleTypes.Reactor;
            case "Thrusters":
                return ModuleTypes.Thrusters;
            case "Misc":
                return ModuleTypes.Shield;
            default:
                return ModuleTypes.None;
        }
    }

    private static string Query()
    {
        string q = $@"
            SELECT
                id,
                is_category,
                parent,
                name,
                asset_name,
                ISNULL(module_type_int, 0) AS module_type_int,
                weapon_type,
                size,
                energy_needed,
                img_id,
                fire_rate,
                ISNULL(damage_amount, 0) AS damage_amount,
                ISNULL(shield_effectiveness, 0) AS shield_effectiveness,
                ISNULL(armor_effectiveness, 0) AS armor_effectiveness,
                ISNULL(structure_effectiveness, 0) AS structure_effectiveness,
                ISNULL(ignore_shield, 0) AS ignore_shield,
                ISNULL(critical_chance, 0) AS critical_chance,
                ISNULL(critical_strength, 0) AS critical_strength,
                armor_points,
                reactor_points,
                thrust_strength,
                dexterity,
                shield_points,
                shield_regen
            FROM
                ss_modules";
        return q;
    }

    public static List<ShipModuleType> CreateList(bool onlyModules)
    {
        List<ShipModuleType> tList = new List<ShipModuleType>();
        SqlDataReader r = DataConnection.GetReader(Query());
        if (r.HasRows)
        {
            while (r.Read())
            {
                var curModule = new ShipModuleType(r);
                if(!onlyModules || (onlyModules && curModule.IsCategory == 0))
                {
                    tList.Add(curModule);
                }
                
            }
        }
        return tList;
    }

    private static Dictionary<int, ShipModuleType> mDict;
    private static void CreateDictionary()
    {
        if (mDict != null)
        {
            return;
        }
        mDict = new Dictionary<int, ShipModuleType>();
        var tList = CreateList(true);
        if (tList.Count > 0)
        {
            foreach (var module in tList)
            {
                mDict.Add(module.Id, module);
            }
        }

    }
    public static ShipModuleType ModuleById(int id)
    {
        CreateDictionary();
        if (mDict.ContainsKey(id))
        {
            return mDict[id];
        }
        else
        {
            return null;
        }
    }
    public static Dictionary<int, string> moduleNames()
    {
        string q;
        SqlDataReader r;
        Dictionary<int, string> ModuleDict = new Dictionary<int, string>();
        q = @"SELECT id, name FROM ss_modules";
        r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                ModuleDict.Add(Convert.ToInt32(r["id"]), Convert.ToString(r["name"]));
            }
        }
        r.Close();
        return ModuleDict;
    }
    public static Dictionary<int, ShipModuleType> CreateModuleDict()
    {

        Dictionary<int, ShipModuleType> tags = new Dictionary<int, ShipModuleType>();

        List<ShipModuleType> lst = CreateList(true);
        foreach (ShipModuleType module in lst)
        {
            tags.Add(module.Id, module);
        }

        return tags;

    }
    private static Dictionary<int, ShipModuleType> moduleDict;
    public static void AddModuleType(ShipModuleType mType)
    {
        if (moduleDict == null)
            moduleDict = new Dictionary<int, ShipModuleType>();
        if (!moduleDict.ContainsKey(mType.Id))
            moduleDict.Add(mType.Id, mType);
    }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

}

public class UnityShipModuleType
{
    public enum WeaponTypes
    {
        Laser = 1,
        Explosive = 2,
        Kinetic = 3,
        Ray = 4,
        Plasma = 5,
        Gravity = 6,
        Doom = 7
    }

    public enum ModuleTypes
    {
        None = 0,
        Weapon = 1,
        Armor = 2,
        Reactor = 3,
        Thrusters = 4,
        Shield = 5
    }

    public UnityShipModuleType() { }

    public int Id { get; set; }
    public int IsCategory { get; set; }
    public int Parent { get; set; }
    public string Name { get; set; }
    public string AssetName { get; set; }
    public ModuleTypes ModuleType { get; set; }
    public int Size { get; set; }
    public int EnergyNeed { get; set; }
    public int ImgId { get; set; }

    //Related to weapon type modules
    public WeaponTypes WeaponType { get; set; }
    public int FireRate { get; set; }
    public int DamageAmount { get; set; }
    public int ShieldEffectiveness { get; set; }
    public int ArmorEffectiveness { get; set; }
    public int StructureEffectiveness { get; set; }
    public int IgnoreShield { get; set; }
    public int CriticalChance { get; set; }
    public int CriticalStrength { get; set; }

    //Related to armor. Here can be some more characteristics to defend from specific types of damage
    public int ArmorPoints { get; set; }

    //Reactor
    public int ReactorPoints { get; set; }

    //Thrusters
    public int ThrustStrength { get; set; }
    public int Dexterity { get; set; }

    //Shields
    public int ShieldPoints { get; set; }
    public int ShieldRegen { get; set; }

}


