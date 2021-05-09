using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class StaticMembers
{
    public static Dictionary<int, SpaceshipRig> rigsDict;
    public static Dictionary<int, ShipModuleType> pModuleDict;
    public static Dictionary<int, ResourceType> resDict;
    public static Dictionary<int, GameEvent> EventDict;
    public static Dictionary<int, BlueprintType> BlueprintDict;
    public static Dictionary<int, Crew.CrewOfficerType> OfficerTypeDict;
    public static Dictionary<int, Crew.CrewOfficer> OfficerDict;
    public static Dictionary<int, Crew.OfficerTypeStat> statDict;
    public static Dictionary<int, BattleSceneType> sceneDict;

    public static Dictionary<int, ShipModel> pModels;
}

