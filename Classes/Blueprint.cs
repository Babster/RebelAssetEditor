﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class BlueprintType : UnityBlueprintType
{

    public BlueprintType() { ResourceList = new List<ResourceForBlueprint>(); }
    public BlueprintType(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        ParentId = Convert.ToInt32(r["parent_id"]);
        Name = Convert.ToString(r["name"]);
        UnityName = (string)r["unity_name"];
        ProductType = (BlueprintProductType)r["product_type"];
        ProductId = Convert.ToInt32(r["product_id"]);
        BaseBonus = Convert.ToInt32(r["base_bonus_points"]);
        FailChance = Convert.ToInt32(r["fail_chance"]);
        ProductionPoints = Convert.ToInt32(r["production_points"]);

        LoadResources();

    }

    private void LoadResources()
    {
        ResourceList = new List<ResourceForBlueprint>();
        string q;
        q = $@"
            SELECT 
                id,
                blueprint_type,
                resource_type_id,
                amount
            FROM
                blueprint_types_resources
            WHERE
                blueprint_type = {Id}";
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                ResourceList.Add(new ResourceForBlueprint(r));
            }
        }
        r.Close();
    }

    public ResourceForBlueprint AddResource()
    {
        ResourceForBlueprint tRes = new ResourceForBlueprint();
        ResourceList.Add(tRes);
        return tRes;
    }

    public static List<BlueprintType> GetList()
    {
        List<BlueprintType> lBlue = new List<BlueprintType>();
        string q = $@"
            SELECT
                id,
                parent_id,
                name,
                ISNULL(unity_name, '') AS unity_name,
                product_type,
                product_id,
                base_bonus_points,
                fail_chance,
                production_points
            FROM
                blueprint_types
            ";
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                lBlue.Add(new BlueprintType(r));
            }
        }
        r.Close();
        return lBlue;
    }

    private static void CreateDict()
    {
        StaticMembers.BlueprintDict = new Dictionary<int, BlueprintType>();
        List<BlueprintType> lBlue = GetList();
        if(lBlue.Count > 0)
        {
            foreach(var bp in lBlue)
            {
                StaticMembers.BlueprintDict.Add(bp.Id, bp);
            }
        }
    }
    public static BlueprintType BlueprintById(int Id)
    {
        if (StaticMembers.BlueprintDict == null)
            CreateDict();
        if (StaticMembers.BlueprintDict.ContainsKey(Id))
        {
            return StaticMembers.BlueprintDict[Id];
        }
        else
        {
            return null;
        }

    }

    public void Save()
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO blueprint_types(parent_id) VALUES(0)
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE blueprint_types SET
                parent_id = {ParentId},
                name = @str1,
                unity_name = @str2,
                product_type = {(int)ProductType},
                product_id = {ProductId},
                base_bonus_points = {BaseBonus},
                fail_chance = {FailChance},
                production_points = {ProductionPoints}
            WHERE
                id = {Id}";

        List<string> names = new List<string> { Name, UnityName };
        DataConnection.Execute(q, names);

        string idsDoNotDelete = "";
        if(ResourceList.Count > 0)
        {
            foreach (var res in ResourceList)
            {
                res.Save(Id);
                if (idsDoNotDelete != "")
                    idsDoNotDelete += ",";
                idsDoNotDelete += res.Id.ToString();
            }

        }
        q = $@"DELETE FROM blueprint_types_resources WHERE blueprint_type = {Id}";
        if(idsDoNotDelete != "")
        {
            q += $@" AND id NOT IN ({idsDoNotDelete})";
        }
        DataConnection.Execute(q);

    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q = $@"
            DELETE FROM blueprint_types WHERE id = {Id};
            DELETE FROM blueprint_types_resources WHERE blueprint_type = {Id}";
        DataConnection.Execute(q);
    }

    public override string ToString()
    {
        return Name;
    }

}

public class ResourceForBlueprint : UnityResourceForBlueprint
{


    public ResourceType ResType
    {
        get
        {
            return ResourceType.ResourceById(ResourceTypeId);
        }
    }

    public ResourceForBlueprint() { }

    public ResourceForBlueprint(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        BlueprintTypeId = Convert.ToInt32(r["blueprint_type"]);
        ResourceTypeId = Convert.ToInt32(r["resource_type_id"]);
        Amount = Convert.ToInt32(r["amount"]);
    }

    public void Save(int blueprintTypeId)
    {
        this.BlueprintTypeId = blueprintTypeId;
        string q;
        if (Id == 0)
        {
            q = $@"
                    INSERT INTO blueprint_types_resources(blueprint_type) VALUES(0)
                    SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
                UPDATE blueprint_types_resources SET 
                    blueprint_type = {BlueprintTypeId},
                    resource_type_id = {ResourceTypeId},
                    amount = {Amount}
                WHERE
                    id = {Id}";
        DataConnection.Execute(q);
    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q = $@"DELETE FROM blueprint_types_resources WHERE id = {Id}";
        DataConnection.Execute(q);
    }

    public override string ToString()
    {
        if (ResType == null)
        {
            return "-";
        }
        else
        {
            return $@"{ResType.Name} - {Amount}";
        }
    }

}


public class UnityBlueprintType
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public string UnityName { get; set; }
    public BlueprintProductType ProductType { get; set; }
    public int ProductId { get; set; }
    public int BaseBonus { get; set; }
    public int FailChance { get; set; }
    public int ProductionPoints { get; set; }
    public List<ResourceForBlueprint> ResourceList { get; set; }

    public enum BlueprintProductType
    {
        None = 0,
        MakeSpaceshipModule = 1,
        MakeSpaceship = 2,
        MakeSpacestationModule = 3,
        ImproveOfficer = 4,
        ImproveSpaceshipModule = 5,
        ImproveSpaceship = 6,
        ImprovePlayerStat = 7
    }
}

public class UnityResourceForBlueprint
{
    public int Id { get; set; }
    public int BlueprintTypeId { get; set; }
    public int ResourceTypeId { get; set; }
    public int Amount { get; set; }
}

