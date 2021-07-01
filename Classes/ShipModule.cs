using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;

public class ShipModule : UnityShipModule
{
    public ShipModuleType ModuleType
    {
        get
        {
            if (ModuleTypeId == 0)
                return null;
            return ShipModuleType.ModuleById(ModuleTypeId);
        }
    }

    public ShipModule() { }
    public ShipModule(SqlDataReader r)
    {
        LoadFromReader(r);
    }

    public ShipModule(int id)
    {
        string q = ModuleQuery();
        q += $@" WHERE id = {id}";
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            r.Read();
            LoadFromReader(r);
            r.Close();
        }
    }

    private void LoadFromReader(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        PlayerId = Convert.ToInt32(r["player_id"]);
        ModuleTypeId = Convert.ToInt32(r["module_id"]);
        Experience = Convert.ToInt32(r["experience"]);
        ModuleLevel = Convert.ToInt32(r["module_level"]);
        RigSlotId = Convert.ToInt32(r["rig_id"]);

        if (r["module_code"] == DBNull.Value)
        {
            this.ModuleCode = Guid.NewGuid();
            string q = $"UPDATE players_modules SET module_code = CAST('{this.ModuleCode.ToString()}' AS uniqueidentifier) WHERE id = {this.Id}";
            DataConnection.Execute(q);
        }
        else
        {
            this.ModuleCode = (Guid)r["module_code"];
        }

    }

    public static string ModuleQuery()
    {
        string q = $@"
            SELECT 
                id,
                player_id,
                module_id,
                experience,
                module_level,
                ISNULL(rig_id, 0) AS rig_id,
                module_code
            FROM
                players_modules";
        return q;
    }

    public void Save()
    {
        string q;
        if(Id==0)
        {
            if(ModuleCode == Guid.Empty)
            {
                q = $@"
                INSERT INTO players_modules(player_id) VALUES(0)
                SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
            }
            else
            {
                Id = ShipModule.ModuleIdByGuid(ModuleCode);
                if(Id == 0)
                {
                    return;
                }
            }
        }

        q = $@"UPDATE players_modules SET 
                player_id = {PlayerId},
                module_id = {ModuleTypeId},
                experience = {Experience},
                module_level = {ModuleLevel},
                rig_id = {RigSlotId},
                module_code = CAST('{ModuleCode.ToString()}' AS uniqueidentifier)
            WHERE
                id = {Id}";
        DataConnection.Execute(q);
    }

    public static List<ShipModule> PlayerModules(int playerId)
    {
        string q = ModuleQuery();
        q += $@" WHERE player_id = {playerId}";

        List<ShipModule> moduleList = new List<ShipModule>();
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while (r.Read())
            {
                moduleList.Add(new ShipModule(r));
            }
        }
        r.Close();
        return moduleList;
    }

    /// <summary>
    /// Class for creating ship rigs. Loads player's modules and has a procedure
    /// to return lists of specific type modules
    /// </summary>
    public class ModuleList
    {
        
        public List<ShipModule> Modules { get; set; }

        public ModuleList(List<ShipModule> modules)
        {
            this.Modules = modules;
        }

        public List<ShipModule> GetModules(ShipModuleType.ModuleTypes moduleType)
        {
            List<ShipModule> getModules = new List<ShipModule>();
            foreach(var module in this.Modules)
            {
                if(!module.Reserve)
                {
                    if (module.ModuleType.ModuleType == moduleType)
                    {
                        getModules.Add(module);
                    }
                }
            }
            return getModules;
        }

        public void MarkUsed(int Id)
        {
            foreach (var module in this.Modules)
            {
                if (module.Id == Id)
                    module.Reserve = true;
                return;
            }
        }

    }

    private static Dictionary<Guid, int> moduleGuidIdDict;
    public static int ModuleIdByGuid(Guid guid)
    {
        if(moduleGuidIdDict == null)
        {
            moduleGuidIdDict = new Dictionary<Guid, int>();
        }
        if(moduleGuidIdDict.ContainsKey(guid))
        {
            return moduleGuidIdDict[guid];
        }
        else
        {
            string q = $@"SELECT id FROM players_modules WHERE module_code = CAST('{guid.ToString()}' AS uniqueidentifier)";
            return DataConnection.GetResultInt(q,null,0);
        }
    }

}

/// <summary>
/// При переносе в  Unity надо убирать префикс Unity и тогда
/// десериализация будет проходить 100% нормально. 
/// </summary>
public class UnityShipModule
{
    [JsonIgnore]
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int ModuleTypeId { get; set; }
    public int Experience { get; set; }
    public int ModuleLevel { get; set; }
    public int RigSlotId { get; set; }
    public bool Reserve { get; set; }
    public Guid ModuleCode { get; set; }

    public override string ToString()
    {
        ShipModuleType mType = ShipModuleType.ModuleById(ModuleTypeId);
        return $"{mType.Name} ({Id})";
    }

}