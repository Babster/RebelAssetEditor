using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;

public  class ShipModuleType
{
    public int Id { get; set; }
    public int IsCategory { get; set; }
    public int Parent { get; set; }
    public string Name { get; set; }
    public string ModuleTypeStr { get; set; }
    public int Size { get; set; }
    public string AssetName { get; set; }
    public int EnergyNeeded { get; set; }
    public SpaceshipParameters parameters { get; set; }

    public enum ModuleType
    {
        None = 0,
        Weapon = 1,
        Armor = 2,
        Engine = 3,
        Thrusters = 4,
        Misc = 5
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
        this.Size = Convert.ToInt32(r["size"]);
        this.AssetName = Convert.ToString(r["asset_name"]);
        this.EnergyNeeded = Convert.ToInt32(r["energy_needed"]);

        string paramStr = Convert.ToString(r["params_structure"]);

        if(!String.IsNullOrEmpty(paramStr))
            this.parameters = SpaceshipParameters.CreateFromString(paramStr);
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
                        size = {Size},
                        energy_needed = {EnergyNeeded},
                        params_structure = @str4
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

        string q = ModuleQuery(false);
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

    private static Dictionary<int, ShipModuleType> pModuleDict;
    public static ShipModuleType ModuleById(int ShipModuleId)
    {
        if (pModuleDict == null)
            pModuleDict = CreateModuleDict();

        if(pModuleDict.ContainsKey(ShipModuleId))
        {
            return pModuleDict[ShipModuleId];
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
                    ISNULL(size, 1) AS size,
                    energy_needed,
                    ISNULL(params_structure, '') AS params_structure
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

    public int GetParameter(SpaceshipParameters.SpaceShipParameter paramType)
    {
        return parameters.ParameterValue(paramType);
    }
    public void SetParameter(SpaceshipParameters.SpaceShipParameter paramType, int Value)
    {
        parameters.SetParameter(paramType, Value);
    }

}
