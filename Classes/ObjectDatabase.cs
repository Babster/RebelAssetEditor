using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ObjectDatabase : UnityObjectDatabase
{
    public ObjectDatabase() { }

    public void SaveDataToStringFile()
    {
        string objectData = JsonConvert.SerializeObject(this);
        objectData = CommonFunctions.Compress(objectData);
        System.IO.File.WriteAllText("object database.dat", objectData);
    }

    /// <summary>
    /// Загрузка данных на основе того, что содержится в классах Asset editor
    /// </summary>
    public void LoadDataAssetEditor()
    {
        LoadResourceTypes();
        LoadBlueprints();
        LoadShipModels();
        LoadModuleTypes();
        LoadRigs();
        LoadSkillsets();
        LoadSkillTypes();
        LoadStatTypes();
        LoadStoryScenes();
        LoadOfficerTypes();
        LoadReactorTypes();
        LoadDictionaries();
    }

    private void LoadResourceTypes()
    {

        resourceTypes = new List<ResourceType>();

        List<ResourceType> tRes = ResourceType.GetResouceList();
        foreach (var res in tRes)
        {
            if (!String.IsNullOrEmpty(res.DescriptionRus))
            {
                resourceTypes.Add(res);
            }
        }
    }

    private void LoadBlueprints()
    {
        blueprintTypes = new List<BlueprintType>();
        List<BlueprintType> tList = BlueprintType.GetList();
        foreach (var bp in tList)
        {
            if (bp.ProductionPoints > 0)
            {
                blueprintTypes.Add(bp);
            }
        }
    }

    private void LoadShipModels()
    {
        shipModels = new List<ShipModel>();
        List<ShipModel> tModels = ShipModel.GetModelList();
        foreach (var model in tModels)
        {
            if (model.BaseStructureHp > 0)
            {
                shipModels.Add(model);
            }
        }
    }

    private void LoadModuleTypes()
    {
        moduleTypes = new List<ShipModuleType>();
        List<ShipModuleType> tModules = ShipModuleType.CreateList(true);
        foreach (var module in tModules)
        {
            if (!string.IsNullOrEmpty(module.AssetName))
            {
                moduleTypes.Add(module);
            }
        }
    }

    private void LoadRigs()
    {
        rigList = SpaceshipRig.BuiltInRigs();
    }

    private void LoadSkillsets()
    {
        skillSets = SkillSetSql.GetSkillsetList();
    }

    private void LoadSkillTypes()
    {
        skillTypes = SkillTypeSql.SkillTypeList();
    }

    private void LoadStoryScenes()
    {
        storyScenes = Story.RebelSceneWithSql.GetSceneList();
    }

    private void LoadStatTypes()
    {
        statTypes = Crew.OfficerStatTypeSql.GetStatTypeList();
    }

    private void LoadOfficerTypes()
    {
        officerTypes = Crew.CrewOfficerType.GetTypeList();
    }
    public void LoadReactorTypes()
    {
        reactorTypes = new List<Station.ReactorType>();
        reactorTypes.Add(new Station.ReactorType(1, "Fusion", 200, 3, 1));
        reactorTypes.Add(new Station.ReactorType(2, "Neutronium", 500, 20, 1));
        reactorTypes.Add(new Station.ReactorType(3, "Vacuum", 800, 21, 1));
        reactorTypes.Add(new Station.ReactorType(4, "Time-space", 1500, 22, 1));
    }

    /// <summary>
    /// Отключено, потому что вся необходимая информация должна быть упакована в объект конкретной боевой сцены
    /// </summary>
    private void LoadBattleSceneTypes()
    {
        battleSceneTypes = BattleSceneType.SceneList();
    }

    private static ObjectDatabase pStaticDatabase;
    public static ObjectDatabase staticDatabase
    {
        get
        {
            if(pStaticDatabase == null)
            {
                pStaticDatabase = new ObjectDatabase();
                pStaticDatabase.LoadDataAssetEditor();
            }
            return pStaticDatabase;
        }
    }

    #region dictionary-linked procedures

    [JsonIgnore]
    public static Dictionary<int, Station.ReactorType> reactorTypeDictionary;
    private void LoadDictionaries()
    {
        reactorTypeDictionary = new Dictionary<int, Station.ReactorType>();
        foreach (var item in reactorTypes)
        {
            reactorTypeDictionary.Add(item.Id, item);
        }
    }

    public static Station.ReactorType ReactorTypeById(int id)
    {
        if (reactorTypeDictionary.ContainsKey(id))
        {
            return reactorTypeDictionary[id];
        }
        else
        {
            return null;
        }
    }

    #endregion

}

/// <summary>
/// Класс, который будет являться словарем для проекта Unity. После загрузки всех данных на основе
/// того, какие объекты введены пользователем и сохранены в БД, он создает собственные списки и словари, 
/// чтобы затем сохранить их в текстовый формат. После того, как стартанется проект Unity, все данные
/// будут загружены и распакованы из строки и затем этот класс будет предоставлять столь необходимые процедуры
/// ModuleById, ShipTypeById и так далее
/// </summary>
public class UnityObjectDatabase
{
    public List<ResourceType> resourceTypes { get; set; }
    public List<BlueprintType> blueprintTypes { get; set; }
    public List<ShipModel> shipModels { get; set; }
    public List<ShipModuleType> moduleTypes { get; set; }
    public List<SpaceshipRig> rigList { get; set; }
    public List<SkillSetSql> skillSets { get; set; }
    public List<SkillTypeSql> skillTypes { get; set; }
    public List<Story.RebelSceneWithSql> storyScenes { get; set; }
    public List<Crew.OfficerStatTypeSql> statTypes { get; set; }
    public List<Crew.CrewOfficerType> officerTypes { get; set; }
    public List<BattleSceneType> battleSceneTypes { get; set; }
    public List<Station.ReactorType> reactorTypes { get; set; }
    public UnityObjectDatabase() { }


    

}
