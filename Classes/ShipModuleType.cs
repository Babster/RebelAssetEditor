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

        parameters = new SpaceshipParameters();
        Size = 1;
    }

    public ShipModuleType(ref SqlDataReader r)
    {
        this.Id = Convert.ToInt32(r["id"]);
        this.IsCategory = Convert.ToInt32(r["is_category"]);
        this.Parent = Convert.ToInt32(r["parent"]);
        this.Name = Convert.ToString(r["name"]);
        this.ModuleTypeStr = Convert.ToString(r["module_type"]);
        this.wType = (WeaponType)Convert.ToInt32(r["weapon_type"]);
        this.Size = Convert.ToInt32(r["size"]);
        this.AssetName = Convert.ToString(r["asset_name"]);
        this.EnergyNeeded = Convert.ToInt32(r["energy_needed"]);
        this.ImgId = (int)r["img_id"];

        string paramStr = Convert.ToString(r["params_structure"]);

        if(!String.IsNullOrEmpty(paramStr))
            this.parameters = SpaceshipParameters.CreateFromString(paramStr);
        if (parameters == null)
            parameters = new SpaceshipParameters();
        if (Size < 1)
            Size = 1;
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
        q = $@"UPDATE ss_modules SET 
                        is_category = {IsCategory},
                        parent = {Parent},
                        name = @str1,
                        asset_name = @str2,
                        module_type = @str3,
                        weapon_type = {(int)wType},
                        size = {Size},
                        energy_needed = {EnergyNeeded},
                        params_structure = @str4,
                        img_id = {ImgId}
                    WHERE id = " + this.Id.ToString();

        List<string> names = new List<string>();
        names.Add(this.Name);
        names.Add(this.AssetName);
        names.Add(this.ModuleTypeStr);
        names.Add(this.parameters.ToDbString());

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

        List<ShipModuleType> lst = GetModuleList();
        foreach(ShipModuleType module in lst)
        {
            tags.Add(module.Id, module);
        }

        return tags;
       
    }

    public static List<ShipModuleType> GetModuleList()
    {
        List<ShipModuleType> lst = new List<ShipModuleType>();
        string q = ModuleQuery(false);
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                ShipModuleType tag = new ShipModuleType(ref r);
                lst.Add(tag);
            }
        }
        r.Close();
        return lst;
    }

   
    public static ShipModuleType ModuleById(int ShipModuleId)
    {
        if (StaticMembers.pModuleDict == null)
            StaticMembers.pModuleDict = CreateModuleDict();

        if(StaticMembers.pModuleDict.ContainsKey(ShipModuleId))
        {
            return StaticMembers.pModuleDict[ShipModuleId];
        }
        else
        {
            return null;
        }

    }

    public static List<ShipModuleType> CreateModuleList()
    {
        List<ShipModuleType> moduleTypes = new List<ShipModuleType>();

        string q = ModuleQuery(false);
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                ShipModuleType moduleType = new ShipModuleType(ref r);
                moduleTypes.Add(moduleType);
            }
        }
        r.Close();
        return moduleTypes;
    }

    public static string ModuleQuery(bool includeGroups)
    {
        string q = @"
                SELECT 
                    id,
                    is_category,
                    parent,
                    name,
                    asset_name,
                    module_type,
                    ISNULL(weapon_type, 0) AS weapon_type,
                    ISNULL(size, 1) AS size,
                    energy_needed,
                    ISNULL(params_structure, '') AS params_structure,
                    ISNULL(img_id, 0) AS img_id
                FROM
                    ss_modules
                    ";

        if(!includeGroups)
        {
            q = q + @"
                WHERE
                    is_category = 0";
        }

        return q;
    }
 
    public override string ToString()
    {
        return this.Name + " " + this.parameters.ToString() ;
    }

}

public class UnityShipModuleType
{

    public int Id { get; set; }
    public int IsCategory { get; set; }
    public int Parent { get; set; }
    public string Name { get; set; }
    public string ModuleTypeStr { get; set; }
    public WeaponType wType { get; set; }
    public int Size { get; set; }
    public string AssetName { get; set; }
    public int EnergyNeeded { get; set; }
    public int ImgId { get; set; }
    public SpaceshipParameters parameters { get; set; }

    public enum WeaponType
    {
        None = 0,
        Energy = 10,
        Kinetic = 11,
        Rocket = 12
    }

    public enum ModuleType
    {
        None = 0,
        Weapon = 1,
        Armor = 2,
        Engine = 3,
        Thrusters = 4,
        Misc = 5
    }

    public string wTypeStr
    {
        get
        {
            switch (wType)
            {
                case WeaponType.Energy:
                    return "Energy";
                case WeaponType.Kinetic:
                    return "Kinetic";
                case WeaponType.Rocket:
                    return "Rocket";
                default:
                    return "";
            }
        }
    }

    public ModuleType ModuleTypeFromStr()
    {
        ModuleType tType;
        switch (ModuleTypeStr)
        {
            case "Weapon":
                tType = ModuleType.Weapon;
                break;
            case "Armor":
                tType = ModuleType.Armor;
                break;
            case "Engine":
                tType = ModuleType.Engine;
                break;
            case "Thrusters":
                tType = ModuleType.Thrusters;
                break;
            case "Misc":
                tType = ModuleType.Misc;
                break;
            default:
                tType = ModuleType.None;
                break;
        }
        return tType;
    }

    public double GetParameter(SpaceshipParameters.SpaceShipParameter paramType)
    {
        return parameters.ParameterValue(paramType);
    }
    public void SetParameter(SpaceshipParameters.SpaceShipParameter paramType, int Value)
    {
        parameters.SetParameter(paramType, Value);
    }

}