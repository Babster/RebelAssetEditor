using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

class BattleSceneType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ParentId { get; set; }
    public int AssembleShip { get; set; }
    public List<Enemy> enemies { get; set; }

    public BattleSceneType() { enemies = new List<Enemy>(); }
    public BattleSceneType(SqlDataReader r)
    {
        Id = Convert.ToInt32(r["id"]);
        Name = Convert.ToString(r["name"]);
        ParentId = Convert.ToInt32(r["parent_id"]);
        AssembleShip = Convert.ToInt32(r["assemble_ship"]);

        LoadEnemies();

    }

    private void LoadEnemies()
    {
        enemies = new List<Enemy>();
        string q = $@"
            SELECT
                id
                battle_scene_id,
                stage_number,
                ship_rig_id,
                count,
                cycle_multiplier,
                base_battle_intensity,
                cycle_intensity_multiplier
            FROM
                battle_scenes_enemies
            WHERE
                battle_scene_id = {Id}";
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                enemies.Add(new Enemy(r));
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
    private static Dictionary<int, BattleSceneType> sceneDict;
    private static void CreateDictionary()
    {
        sceneDict = new Dictionary<int, BattleSceneType>();
        List<BattleSceneType> lst = SceneList();
        if(lst.Count > 0)
        {
            foreach(BattleSceneType element in lst)
            {
                sceneDict.Add(element.Id, element);
            }
        }
    }
    public static BattleSceneType SceneById(int Id)
    {
        if (sceneDict == null)
            CreateDictionary();
        if(sceneDict.ContainsKey(Id))
        {
            return (sceneDict[Id]);
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
                parent_id,
                ISNULL(assemble_ship, 0) AS assemble_ship
            FROM
                battle_scenes";
        return q;
    }

    public class Enemy
    {
        public int Id { get; set; }
        public int BattleId { get; set; }
        public int StageNumber { get; set; } //Номер стадии - враги идут один за другим, но могут вперемешку
        public int ShipRigId { get; set; } //Какой именно риг идет против игрока
        public int Count { get; set; } //Количество кораблей в данной стадии
        public int CycleMultiplier { get; set; } //Мультипликатор характеристик врагов
        public int BaseBattleIntensity { get; set; } //Базовая награда за убийство каждого врага
        public int CycleIntensityMult { get; set; } //Мультипликатор награды

        public SpaceshipRig Rig
        {
            get
            {
                if (ShipRigId == 0)
                {
                    return null;
                }
                else
                {
                    return SpaceshipRig.RigById(ShipRigId);
                }
            }
        }

        public Enemy() { }

        public Enemy(SqlDataReader r)
        {
            Id = Convert.ToInt32(r["id"]);
            BattleId = Convert.ToInt32(r["battle_scene_id"]);
            StageNumber = Convert.ToInt32(r["stage_number"]);
            ShipRigId = Convert.ToInt32(r["ship_rig_id"]);
            Count = Convert.ToInt32(r["count"]);
            CycleMultiplier = Convert.ToInt32(r["cycle_multiplier"]);
            BaseBattleIntensity = Convert.ToInt32(r["base_battle_intensity"]);
            CycleIntensityMult = Convert.ToInt32(r["cycle_intensity_multiplier"]);
        }

        public void Save(int battleSceneId)
        {
            string q;
            if(Id == 0)
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
                    cycle_intensity_multiplier = {CycleIntensityMult}
                WHERE
                    id = {Id}";
            DataConnection.Execute(q);
        }

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

    public Enemy AddEnemy()
    {
        Enemy enemy = new Enemy();
        enemies.Add(enemy);
        return enemy;
    }

    public void DeleteEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
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
                parent_id = {ParentId},
                assemble_ship = {AssembleShip}
            WHERE
                id = {Id}";

        List<string> names = new List<string> { Name };
        DataConnection.Execute(q, names);

        string idsDoNotDelete = "";
        if(enemies.Count > 0)
        {
            foreach(Enemy enemy in enemies)
            {
                enemy.Save(Id);
                if (idsDoNotDelete != "")
                    idsDoNotDelete += ",";
                idsDoNotDelete += enemy.Id;
            }
        }

        q = $"DELETE FROM battle_scenes_enemies WHERE battle_scene_id = {Id}";
        if(idsDoNotDelete == "")
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
        q = $"DELETE FROM battle_scenes WHERE id = {Id}";
        DataConnection.Execute(q);
    }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

}
