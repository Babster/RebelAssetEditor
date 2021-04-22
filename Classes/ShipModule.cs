using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public  class ShipModuleType
{
    public int Id { get; set; }
    public int IsCategory { get; set; }
    public int Parent { get; set; }
    public string Name { get; set; }
    public string AssetName { get; set; }
    public string ModuleType { get; set; }
    public int EnergyNeeded { get; set; }
    public int MainScore { get; set; }
    public int SecondaryScore { get; set; }
    public int ThirdScore { get; set; }

    public enum EnumModuleType
    {
        None = 0,
        Weapon = 1,
        Defence = 2,
        Engine = 3,
        Thrusters = 4
    }

    public EnumModuleType eType()
    {
        EnumModuleType tType;
        switch (Name)
        {
            case "Weapon":
                tType = EnumModuleType.Weapon;
                break;
            case "Defence":
                tType = EnumModuleType.Defence;
                break;
            case "Engine":
                tType = EnumModuleType.Engine;
                break;
            case "Thrusters":
                tType = EnumModuleType.Thrusters;
                break;
            default:
                tType = EnumModuleType.None;
                break;
        }
        return tType;
    }

    public ShipModuleType(int parentId, int isCategory)
    {
        this.Parent = parentId;
        this.IsCategory = isCategory;
        if (IsCategory == 1)
        {
            this.Name = "New category";
        }
        else
        {
            this.Name = "New module";
        }
    }

    public ShipModuleType(ref SqlDataReader r)
    {
        this.Id = Convert.ToInt32(r["id"]);
        this.IsCategory = Convert.ToInt32(r["is_category"]);
        this.Parent = Convert.ToInt32(r["parent"]);
        this.Name = Convert.ToString(r["name"]);
        this.AssetName = Convert.ToString(r["asset_name"]);
        this.ModuleType = Convert.ToString(r["module_type"]);
        this.EnergyNeeded = Convert.ToInt32(r["energy_needed"]);
        this.MainScore = Convert.ToInt32(r["main_score"]);
        this.SecondaryScore = Convert.ToInt32(r["secondary_score"]);
        this.ThirdScore = Convert.ToInt32(r["third_score"]);
    }

    public void SaveData()
    {
        string q;
        if (this.Id == 0)
        {
            q = @"INSERT INTO ss_modules(parent) VALUES(" + this.Parent.ToString() + @")
                            SELECT @@IDENTITY AS Result";
            this.Id = Convert.ToInt32(DataConnection.GetResult(q));

        }
        q = @"UPDATE ss_modules SET 
                        is_category = " + this.IsCategory.ToString() + @",
                        parent = " + this.Parent.ToString() + @",
                        name = @str1,
                        asset_name = @str2,
                        module_type = @str3,
                        energy_needed = " + this.EnergyNeeded + @",
                        main_score = " + this.MainScore + @",
                        secondary_score = " + this.SecondaryScore + @",
                        third_score = " + this.ThirdScore + @"
                    WHERE id = " + this.Id.ToString();

        List<string> names = new List<string>();
        names.Add(this.Name);
        names.Add(this.AssetName);
        names.Add(this.ModuleType);

        DataConnection.Execute(q, names);

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

        string q = ModuleQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                ShipModuleType tag = new ShipModuleType(ref r);
                tags.Add(tag.Id, tag);
            }
        }
        r.Close();
        return tags;
    }

    public static string ModuleQuery()
    {
        string q = @"
                SELECT 
                    id,
                    is_category,
                    parent,
                    name,
                    asset_name,
                    module_type,
                    energy_needed,
                    main_score,
                    secondary_score,
                    third_score
                FROM
                    ss_modules
                --WHERE
                --    parent = 0
                    ";
        return q;
    }

    #region Specific modules properties

    public enum enumSpecificProperty
    {
        FireRate = 1,
        DeflectorsDamage = 2,
        StructureDamage = 3,
        Deflectors = 4,
        DeflectorsRegen = 5,
        Armor = 6,
        Engine = 7,
        ThrustersSpeed = 8,
        ThrustersDexterity = 9
    }

    public int SpecificPropertiesCount() { return 9; }

    public String PropertyToString(enumSpecificProperty prop)
    {
        switch (prop)
        {
            case enumSpecificProperty.FireRate:
                return "Fire rate";
            case enumSpecificProperty.DeflectorsDamage:
                return "Deflectors damage";
            case enumSpecificProperty.StructureDamage:
                return "Structure damage";
            case enumSpecificProperty.Deflectors:
                return "Deflectors";
            case enumSpecificProperty.DeflectorsRegen:
                return "Deflectors regen";
            case enumSpecificProperty.Armor:
                return "Armor";
            case enumSpecificProperty.Engine:
                return "Engine";
            case enumSpecificProperty.ThrustersSpeed:
                return "Speed";
            case enumSpecificProperty.ThrustersDexterity:
                return "Dexterity";
            default:
                return "error: unknown module type";
        }
    }

    public int PropertyValue(enumSpecificProperty prop)
    {
        switch (prop)
        {
            case enumSpecificProperty.FireRate:
                return FireRate();
            case enumSpecificProperty.DeflectorsDamage:
                return DeflectorsDamage();
            case enumSpecificProperty.StructureDamage:
                return StructureDamage();
            case enumSpecificProperty.Deflectors:
                return Deflectors();
            case enumSpecificProperty.DeflectorsRegen:
                return DeflectorsRegen();
            case enumSpecificProperty.Armor:
                return Armor();
            case enumSpecificProperty.Engine:
                return Engine();
            case enumSpecificProperty.ThrustersSpeed:
                return ThrustersSpeed();
            case enumSpecificProperty.ThrustersDexterity:
                return ThrustersDexterity();
            default:
                return 0;
        }
    }

    public int FireRate()
    {
        if (eType() == EnumModuleType.Weapon)
        {
            return this.MainScore;
        }
        else
        {
            return 0;
        }
    }

    public int DeflectorsDamage()
    {
        if (eType() == EnumModuleType.Weapon)
        {
            return this.SecondaryScore;
        }
        else
        {
            return 0;
        }
    }

    public int StructureDamage()
    {
        if (eType() == EnumModuleType.Weapon)
        {
            return this.ThirdScore;
        }
        else
        {
            return 0;
        }
    }

    public int Deflectors()
    {
        if (eType() == EnumModuleType.Defence)
        {
            return this.MainScore;
        }
        else
        {
            return 0;
        }
    }

    public int DeflectorsRegen()
    {
        if (eType() == EnumModuleType.Defence)
        {
            return this.SecondaryScore;
        }
        else
        {
            return 0;
        }
    }

    public int Armor()
    {
        if (eType() == EnumModuleType.Defence)
        {
            return this.ThirdScore;
        }
        else
        {
            return 0;
        }
    }

    public int Engine()
    {
        if (eType() == EnumModuleType.Engine)
        {
            return this.ThirdScore;
        }
        else
        {
            return 0;
        }
    }

    public int ThrustersSpeed()
    {
        if (eType() == EnumModuleType.Thrusters)
        {
            return this.MainScore;
        }
        else
        {
            return 0;
        }
    }

    public int ThrustersDexterity()
    {
        if (eType() == EnumModuleType.Thrusters)
        {
            return this.SecondaryScore;
        }
        else
        {
            return 0;
        }
    }

    #endregion

    public override string ToString()
    {
        return this.Name + " " + this.MainScore + "/" + this.SecondaryScore + "/" + this.ThirdScore;
    }

}
