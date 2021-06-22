﻿using System;
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
    }

    private void LoadResourceTypes()
    {

        resourceTypes = new List<ResourceType>();

        List<ResourceType> tRes = ResourceType.GetResouceList();
        foreach(var res in tRes)
        {
            if(!String.IsNullOrEmpty(res.DescriptionRus))
            {
                resourceTypes.Add(res);
            }
        }
    }

    private void LoadBlueprints()
    {
        blueprintTypes = new List<BlueprintType>();
        List<BlueprintType> tList = BlueprintType.GetList();
        foreach(var bp in tList)
        {
            if(bp.ProductionPoints > 0)
            {
                blueprintTypes.Add(bp);
            }
        }
    }

    private void LoadShipModels()
    {
        shipModels = new List<ShipModel>();
        List<ShipModel> tModels = ShipModel.GetModelList();
        foreach(var model in tModels)
        {
            if(model.BaseStructureHp > 0)
            {
                shipModels.Add(model);
            }
        }
    }

    private void LoadModuleTypes()
    {
        moduleTypes = new List<ShipModuleType>();
        List<ShipModuleType> tModules = ShipModuleType.CreateList(true);
        foreach(var module in tModules)
        {
            if(!string.IsNullOrEmpty(module.AssetName))
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
    public UnityObjectDatabase() { }

    

}
