using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class BattleSceneType : UnityBattleSceneType
{

    public BattleSceneType() { enemies = new List<BattleSceneTypeEnemy>(); resources = new List<BattleSceneTypeResource>(); }
    public BattleSceneType(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        Name = Convert.ToString(r["name"]);
        MissionObjective = (string)r["mission_objective"];
        ParentId = Convert.ToInt32(r["parent_id"]);
        AssembleShip = Convert.ToInt32(r["assemble_ship"]);
        CycleToComplete = (int)r["cycle_to_complete"];
        LoadEnemies();
        LoadResources();

    }

    private void LoadEnemies()
    {
        enemies = new List<BattleSceneTypeEnemy>();
        string q = $@"
            SELECT
                id,
                battle_scene_id,
                stage_number,
                ship_rig_id,
                count,
                cycle_multiplier,
                base_battle_intensity,
                cycle_intensity_multiplier,
                ISNULL(cycle_from, 1) AS cycle_from,
                ISNULL(cycle_to, 9999) AS cycle_to,
                ISNULL(cycle_periodics, 1) AS cycle_periodics
            FROM
                battle_scenes_enemies
            WHERE
                battle_scene_id = {Id}";
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                enemies.Add(new BattleSceneTypeEnemy(r));
            }
        }
        r.Close();
    }

    private void LoadResources()
    {
        resources = new List<BattleSceneTypeResource>();

        string q = $@"
            SELECT
                id,
                battle_scene_id,
                enemy_id,
                any_enemy,
                resource_id,
                amount_from,
                amount_to,
                variable_chance_from,
                variable_chance_to,
                min_cycle,
                max_cycle,
                blueprint_id,
                blueprint_bonus_points,
                guaranteed_amount
            FROM
                battle_scenes_resources
            WHERE
                battle_scene_id = {Id}";
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                resources.Add(new BattleSceneTypeResource(r));
            }
        }
        r.Close();
    }

    public static List<BattleSceneType> SceneList()
    {
        List<BattleSceneType> sceneList = new List<BattleSceneType>();
        string q = SceneQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                sceneList.Add(new BattleSceneType(r));
            }
        }
        r.Close();

        return sceneList;

    }

    private static void CreateDictionary()
    {
        StaticMembers.sceneDict = new Dictionary<int, BattleSceneType>();
        List<BattleSceneType> lst = SceneList();
        if(lst.Count > 0)
        {
            foreach(BattleSceneType element in lst)
            {
                StaticMembers.sceneDict.Add(element.Id, element);
            }
        }
    }
    public static BattleSceneType SceneById(int Id)
    {
        if (StaticMembers.sceneDict == null)
            CreateDictionary();
        if(StaticMembers.sceneDict.ContainsKey(Id))
        {
            return (StaticMembers.sceneDict[Id]);
        }
        else
        {
            return null;
        }
    }
    private static string SceneQuery()
    {
        string q = $@"
            SELECT
                id,
                name,
                ISNULL(mission_objective, '') AS mission_objective,
                parent_id,
                ISNULL(assemble_ship, 0) AS assemble_ship,
                ISNULL(cycle_to_complete, 0) AS cycle_to_complete
            FROM
                battle_scenes";
        return q;
    }
    public BattleSceneTypeEnemy AddEnemy()
    {
        BattleSceneTypeEnemy enemy = new BattleSceneTypeEnemy();
        enemies.Add(enemy);
        return enemy;
    }

    public void DeleteEnemy(BattleSceneTypeEnemy enemy)
    {
        enemies.Remove(enemy);
    }

    public BattleSceneTypeResource AddResource()
    {
        BattleSceneTypeResource resource = new BattleSceneTypeResource();
        resources.Add(resource);
        return resource;
    }

    public void Save()
    {
        string q;
        if(Id == 0)
        {
            q = $@"
                INSERT INTO battle_scenes(name) VALUES('')
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
            UPDATE battle_scenes SET 
                name = @str1,
                mission_objective = @str2,
                parent_id = {ParentId},
                assemble_ship = {AssembleShip},
                cycle_to_complete = {CycleToComplete}
            WHERE
                id = {Id}";

        List<string> names = new List<string> { Name, MissionObjective };
        DataConnection.Execute(q, names);

        string idsDoNotDelete = "";
        if(enemies.Count > 0)
        {
            foreach(BattleSceneTypeEnemy enemy in enemies)
            {
                enemy.Save(Id);
                if (idsDoNotDelete != "")
                    idsDoNotDelete += ",";
                idsDoNotDelete += enemy.Id;
            }
        }

        q = $"DELETE FROM battle_scenes_enemies WHERE battle_scene_id = {Id}";
        if(idsDoNotDelete != "")
        {
            q += $" AND id NOT IN({idsDoNotDelete})";
        }
        DataConnection.Execute(q);

        idsDoNotDelete = "";
        if(resources.Count > 0)
        {
            foreach(BattleSceneTypeResource resource in resources)
            {
                resource.Save(Id);
                if (idsDoNotDelete != "")
                    idsDoNotDelete += ",";
                idsDoNotDelete += resource.Id;
            }
        }

        q = $"DELETE FROM battle_scenes_resources WHERE battle_scene_id = {Id}";
        if (idsDoNotDelete != "")
        {
            q += $" AND id NOT IN({idsDoNotDelete})";
        }
        DataConnection.Execute(q);

    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q;
        q = $@"
            DELETE FROM battle_scenes WHERE id = {Id};
            DELETE FROM battle_scenes_enemies WHERE battle_scene_id = {Id};
            DELETE FROM battle_scenes_resources WHERE battle_scene_id = {Id};";
        DataConnection.Execute(q);
    }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

}

public class BattleSceneTypeEnemy : UnityBattleSceneTypeEnemy
{

    public BattleSceneTypeEnemy() { }

    public BattleSceneTypeEnemy(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        BattleId = Convert.ToInt32(r["battle_scene_id"]);
        StageNumber = Convert.ToInt32(r["stage_number"]);
        ShipRigId = Convert.ToInt32(r["ship_rig_id"]);
        Count = Convert.ToInt32(r["count"]);
        CycleMultiplier = Convert.ToInt32(r["cycle_multiplier"]);
        BaseBattleIntensity = Convert.ToInt32(r["base_battle_intensity"]);
        CycleIntensityMult = Convert.ToInt32(r["cycle_intensity_multiplier"]);
        CycleFrom = (int)r["cycle_from"];
        CycleTo = (int)r["cycle_to"];
        CyclePeriodics = (int)r["cycle_periodics"];
    }

    public void Save(int battleSceneId)
    {
        string q;
        if (Id == 0)
        {
            q = $@"
                    INSERT INTO battle_scenes_enemies(battle_scene_id) VALUES({battleSceneId})
                    SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
                UPDATE battle_scenes_enemies SET 
                    stage_number = {StageNumber},
                    ship_rig_id = {ShipRigId},
                    count = {Count},
                    cycle_multiplier = {CycleMultiplier},
                    base_battle_intensity = {BaseBattleIntensity},
                    cycle_intensity_multiplier = {CycleIntensityMult},
                    cycle_from = {CycleFrom.ToString()},
                    cycle_to = {CycleTo.ToString()},
                    cycle_periodics = {CyclePeriodics}
                WHERE
                    id = {Id}";
        DataConnection.Execute(q);
    }



}

public class BattleSceneTypeResource : UnityBattleSceneTypeResource
{
     public BattleSceneTypeResource() { }

    public BattleSceneTypeResource(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        BattleSceneId = Convert.ToInt32(r["battle_scene_id"]);
        EnemyId = Convert.ToInt32(r["enemy_id"]);
        this.AnyEnemy = Convert.ToInt32(r["any_enemy"]);
        this.ResourceId = Convert.ToInt32(r["resource_id"]);
        this.AmountFrom = Convert.ToInt32(r["amount_from"]);
        this.AmountTo = Convert.ToInt32(r["amount_to"]);
        this.VariableChanceFrom = Convert.ToInt32(r["variable_chance_from"]);
        this.VariableChanceTo = Convert.ToInt32(r["variable_chance_to"]);
        this.MinimumCycle = Convert.ToInt32(r["min_cycle"]);
        this.MaximumCycle = Convert.ToInt32(r["max_cycle"]);
        this.BlueprintId = Convert.ToInt32(r["blueprint_id"]);
        this.BlueprintBonusPoints = Convert.ToInt32(r["blueprint_bonus_points"]);
        this.GuaranteedAmount = Convert.ToInt32(r["guaranteed_amount"]);
    }

    public void Save(int battleSceneId)
    {
        this.BattleSceneId = battleSceneId;
        string q;
        if (Id == 0)
        {
            q = $@"
                    INSERT INTO battle_scenes_resources(battle_scene_id) VALUES({battleSceneId})
                    SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q);
        }

        q = $@"
                UPDATE battle_scenes_resources SET
                    battle_scene_id = {battleSceneId},
                    enemy_id = {EnemyId},
                    any_enemy = {AnyEnemy},
                    resource_id = {ResourceId},
                    amount_from = {AmountFrom},
                    amount_to = {AmountTo},
                    variable_chance_from = {VariableChanceFrom},
                    variable_chance_to = {VariableChanceTo},
                    min_cycle = {MinimumCycle},
                    max_cycle = {MaximumCycle},
                    blueprint_id = {BlueprintId},
                    blueprint_bonus_points = {BlueprintBonusPoints},
                    guaranteed_amount = {GuaranteedAmount}
                WHERE
                    id = {Id}";
        DataConnection.Execute(q);

    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q = $@"DELETE FROM battle_scenes_resources WHERE id = {Id}";
        DataConnection.Execute(q);
    }

    public override string ToString()
    {
        if (ResourceType == null && BlueprintType == null)
            return "new resource drop";
        else if (this.BlueprintType != null)
            return BlueprintType.ToString();
        else
            return ResourceType.Name;
    }

}

public class UnityBattleSceneType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MissionObjective { get; set; }
    public int CycleToComplete { get; set; }
    public int ParentId { get; set; }
    public int AssembleShip { get; set; }
    public List<BattleSceneTypeEnemy> enemies { get; set; }
    public List<BattleSceneTypeResource> resources { get; set; }
}



public class UnityBattleSceneTypeEnemy
{
    public int Id { get; set; }
    public int BattleId { get; set; }
    public int StageNumber { get; set; } //Номер стадии - враги идут один за другим, но могут вперемешку
    public int ShipRigId
    {
        get
        {
            if (Rig == null)
                return 0;
            else
                return Rig.Id;
        }
        set
        {
            Rig = SpaceshipRig.RigById(value);
        }
    } //Какой именно риг идет против игрока
    public int Count { get; set; } //Количество кораблей в данной стадии
    public int CycleMultiplier { get; set; } //Мультипликатор характеристик врагов
    public int BaseBattleIntensity { get; set; } //Базовая награда за убийство каждого врага
    public int CycleIntensityMult { get; set; } //Мультипликатор награды
    public SpaceshipRig Rig { get; set; }
    public int CycleFrom { get; set; }
    public int CycleTo { get; set; }
    public int CyclePeriodics { get; set; }
    public UnityBattleSceneTypeEnemy() { }

    public override string ToString()
    {
        if (Rig == null)
        {
            return "<none>";
        }
        else
        {
            return $"{Rig.sModel.ToString()} ({Count})";
        }
    }

}

public class UnityBattleSceneTypeResource
{
    public int Id { get; set; }
    public int BattleSceneId { get; set; }
    public int EnemyId { get; set; }
    public int AnyEnemy { get; set; }
    public int ResourceId { get; set; }
    public int AmountFrom { get; set; }
    public int AmountTo { get; set; }
    public int VariableChanceFrom { get; set; } // x 0.01 percent = 10000 for guaranteed drop
    public int VariableChanceTo { get; set; } // x 0.01 percent = 10000 for guaranteed drop
    public int MinimumCycle { get; set; }
    public int MaximumCycle { get; set; } //after it server gives player maximum chance of drop and maximum amount of good
    public int BlueprintId { get; set; }
    public int BlueprintBonusPoints { get; set; }
    public int GuaranteedAmount { get; set; } //this count will drop from ONE random of the enemy in a given cycle range
    public ResourceType ResourceType
    {
        get { return ResourceType.ResourceById(ResourceId); }
    }
    public BlueprintType BlueprintType
    {
        get
        {
            if (BlueprintId == 0)
            {
                return null;
            }
            else
            {
                return BlueprintType.BlueprintById(BlueprintId);
            }
        }

    }

    public UnityBattleSceneTypeResource() { }

}