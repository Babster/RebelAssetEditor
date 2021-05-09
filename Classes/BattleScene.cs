using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmiralNamespace;
using System.Data.SqlClient;

class BattleScene
{

    public BattleSceneType SceneType { get; set; }
    public SpaceshipRig Rig { get; set; }

    public List<Cycle> Cycles { get; set; }

    public BattleScene(BattleSceneType stype, AccountData player)
    {
        SceneType = stype;
        Rig = SpaceshipRig.RigForPlayer(player.Id);
        Rig.SaveData(player.Id, "");

        //Starting with 5 cycles and then add some more
        Cycles = new List<Cycle>();
        for(int i=1; i<=5; i++)
        {
            Cycle curCycle = new Cycle(stype, i);
            Cycles.Add(curCycle);
        }
    }


    /// <summary>
    /// Battle scene -> cycle -> stage
    /// </summary>
    public class Cycle
    {

        private const double AmountVariability = 0.2;
        public int Number { get; set; }
        public List<Stage> Stages { get; set; }

        private List<ResourceInCycle> crList;

        public Cycle() { }
        public Cycle(BattleSceneType stype, int cycleNumber)
        {

            Random rnd = new Random();

            crList = new List<ResourceInCycle>();
            foreach (BattleSceneType.Resource res in stype.resources)
            {
                ResourceInCycle curRc = new ResourceInCycle(res, cycleNumber);
                if (curRc.CanBeUsed)
                    crList.Add(curRc);
            }

            Stages = new List<Stage>();
            Number = cycleNumber;
            Stage curStage = null;
            foreach (var enemy in stype.enemies)
            {
                if(curStage == null)
                {
                    curStage = new Stage(enemy, cycleNumber, crList, stype);
                    Stages.Add(curStage);
                }
                else
                {
                    if(enemy.StageNumber == curStage.StageNumber)
                    {
                        curStage.AddEnemy(enemy);
                    }
                    else
                    {
                        curStage = new Stage(enemy, cycleNumber, crList, stype);
                        Stages.Add(curStage);
                    }
                }
                
            }

            foreach (var res in crList)
            {
                res.ProcessGuaranteedAmount();
            }

        }

        public class ResourceInCycle
        {

            private Random rnd;

            private double probability;
            private double amount;
            private ResourceType resType;
            private BlueprintType bpType;

            private int guaranteedAmount;
            

            private List<StageEnemy> enemies;
            private int EnemyId;

            

            public bool CanBeUsed
            {
                get
                {
                    if (probability > 0 && amount > 0)
                        return true;
                    if (guaranteedAmount > 0)
                        return true;
                    return false;
                }
            }

            public ResourceInCycle(BattleSceneType.Resource res, int cycleNumber)
            {

                rnd = new Random();
                probability = 0;
                amount = 0;
                resType = res.ResourceType;
                bpType = res.BlueprintType;

                //Пока что гарантированный дроп падает в тот цикл, который минимален для данного ресурса
                if(res.GuaranteedAmount > 0)
                {
                    if(res.MinimumCycle == cycleNumber)
                    {
                        guaranteedAmount = res.GuaranteedAmount;
                    }
                }
                

                if(res.AnyEnemy == 0)
                {
                    EnemyId = res.EnemyId;
                }
                else
                {
                    EnemyId = 0;
                }

                if (guaranteedAmount > 0)
                    enemies = new List<StageEnemy>();

                //Это дроп ресурса, который падает с определенной вероятностью
                if (cycleNumber >= res.MinimumCycle && res.AmountFrom > 0 && res.VariableChanceFrom > 0)
                {

                    this.resType = res.ResourceType;
                    this.bpType = res.BlueprintType;

                    if (res.MaximumCycle <= cycleNumber)
                    {
                        probability = res.VariableChanceTo / 10000;
                        amount = res.AmountTo;
                    }
                    else
                    {
                        probability = (double)res.VariableChanceFrom / 10000 + (double)(cycleNumber - res.MinimumCycle) / (double)(res.MaximumCycle - res.MinimumCycle) * (double)((res.VariableChanceTo - res.VariableChanceFrom) / 1000);
                        amount = (double)res.AmountFrom + (double)(cycleNumber - res.MinimumCycle) / (double)(res.MaximumCycle - res.MinimumCycle) * (double)(res.AmountTo - res.AmountFrom);
                    }



                }
            }

            public void ProcessEnemy(StageEnemy enemy)
            {
                if (EnemyId > 0 && enemy.EnemyType.Id != EnemyId)
                    return;

                if (guaranteedAmount > 0)
                    enemies.Add(enemy);

                if (probability <= 0 || amount <= 0)
                    return;

                if(probability > rnd.NextDouble())
                {
                    //Additionally amount of resources variates in +-20% range
                    amount = amount * (1 + (rnd.NextDouble() - 0.5) * AmountVariability * 2);
                    amount = Math.Round(amount);

                    if(resType != null)
                    {
                        enemy.Resources.Add(new EnemyResource(resType, (int)amount));
                    }
                    
                    if(bpType != null)
                    {
                        enemy.Resources.Add(new EnemyResource(bpType, (int)amount));
                    }


                }
            }

            public void ProcessGuaranteedAmount()
            {
                if (guaranteedAmount <= 0)
                    return;

                if (enemies.Count == 0)
                    return;

                if(resType != null)
                    enemies[rnd.Next(enemies.Count)].Resources.Add(new EnemyResource(resType, (int)amount));


                if(bpType != null)
                {
                    for (int i = 1; i <= guaranteedAmount; i++)
                    {
                        enemies[rnd.Next(enemies.Count)].Resources.Add(new EnemyResource(bpType, 1));
                    }
                }
                
            }

        }

    }

    public class Stage
    {
        
        public int StageNumber { get; set; }
        public int CycleNumber { get; set; }
        public List<StageEnemy> Enemy { get; set; }
        public List<Cycle.ResourceInCycle> Cr { get; set; }

        public Stage() { }

        public Stage(BattleSceneType.Enemy enemy, int cycleNumber, List<Cycle.ResourceInCycle> cr, BattleSceneType stype)
        {
            Enemy = new List<StageEnemy>();
            StageNumber = enemy.StageNumber;
            CycleNumber = cycleNumber;
            Cr = cr;
            AddEnemy(enemy);
        }

        public void AddEnemy(BattleSceneType.Enemy enemy)
        {

            for (int i = 0; i < enemy.Count; i++)
            {
                StageEnemy curEnemy = new StageEnemy();
                curEnemy.StageNumber = StageNumber;
                curEnemy.CycleNumber = CycleNumber;
                curEnemy.EnemyType = enemy;
                double intensityMult = 1;
                double cycleMultiplier = 1;
                if (CycleNumber > 1)
                {
                    intensityMult = Math.Pow((1 + (double)enemy.CycleIntensityMult / 100), CycleNumber - 1);
                    cycleMultiplier = Math.Pow((1 + (double)enemy.CycleMultiplier / 100), CycleNumber - 1);
                }
                    
                curEnemy.BattleIntensity = (int)Math.Round(enemy.Rig.sModel.BattleIntensity * intensityMult);
                curEnemy.EnemyStatsMultiplier = cycleMultiplier;
                curEnemy.CalculateParameters();
                foreach (var c in Cr)
                {
                    c.ProcessEnemy(curEnemy);
                }
                Enemy.Add(curEnemy);
            }

        }

    }


    public class StageEnemy
    {

        public int StageNumber { get; set; }
        public int CycleNumber { get; set; }

        public BattleSceneType.Enemy EnemyType { get; set; }
        public int BattleIntensity { get; set; }
        public double EnemyStatsMultiplier { get; set; }
        public List<EnemyResource> Resources { get; set; }
        public StageEnemy() { Resources = new List<EnemyResource>(); }

        public int EnemyStructurePoints { get; set; }
        public int EnemyShieldPoints { get; set; }
        public int EnemyShieldRegen { get; set; }

        public void CalculateParameters()
        {
            EnemyType.Rig.RecalculateParameters();
            EnemyStructurePoints = (int)Math.Round(EnemyStatsMultiplier * EnemyType.Rig.Params.ParameterValue(SpaceshipParameters.SpaceShipParameter.StructureHitpoints));
            EnemyShieldPoints = (int)Math.Round(EnemyStatsMultiplier * EnemyType.Rig.Params.ParameterValue(SpaceshipParameters.SpaceShipParameter.ShieldPoints));
            EnemyShieldRegen = (int)Math.Round(EnemyStatsMultiplier * EnemyType.Rig.Params.ParameterValue(SpaceshipParameters.SpaceShipParameter.ShieldRegen));
        }


        public override string ToString()
        {
            return $@"{EnemyType.ToString()} struct: {EnemyStructurePoints}, shield: {EnemyShieldPoints}, regen: {EnemyShieldRegen}";
        }

    }

    public class EnemyResource
    {
        public ResourceType ResType { get; set; }
        public BlueprintType BpType { get; set; }
        public int Count { get; set; }
        public EnemyResource() { }
        public EnemyResource(ResourceType rt, int count)
        {
            ResType = rt;
            Count = count;
        }
        public EnemyResource(BlueprintType bp, int count)
        {
            BpType = bp;
            Count = count;
        }
    }

    public class BsStatictics
    {

        //public List<Stage> Stages { get; set; }
        public List<EnemyResource> Res { get; set; }

        public List<StageEnemy> Enemies { get; set; }

        public BsStatictics() 
        {
            Res = new List<EnemyResource>();
            Enemies = new List<StageEnemy>();
        }

        public void AddStage(BattleScene.Stage stage)
        {
            //Stages.Add(stage);
            foreach(var enemy in stage.Enemy)
            {
                AddEnemy(enemy);
            }
        }

        public void AddEnemy(BattleScene.StageEnemy enemy)
        {
            Enemies.Add(enemy);
            if (enemy.Resources != null)
                Res.AddRange(enemy.Resources);
        }

        public string EnemiesString()
        {
            StringBuilder b = new StringBuilder();
            StageEnemy curEnemy = null;
            int curCount = 0;
            foreach(var enemy in Enemies)
            {
                if(curEnemy == null)
                {
                    curCount = 1;
                    curEnemy = enemy;
                }
                else if(enemy.EnemyType.Id != curEnemy.EnemyType.Id || enemy.CycleNumber != curEnemy.CycleNumber || enemy.StageNumber != curEnemy.StageNumber)
                {
                    b.AppendLine(curEnemy.ToString() + ": " + curCount.ToString());
                    curCount = 1;
                    curEnemy = enemy;
                }
                else
                {
                    curCount += 1;
                }
            }
            if(curCount > 0)
            {
                b.AppendLine(curEnemy.ToString() + ": " + curCount.ToString());
            }
            return b.ToString();
        }

        public string ResourcesString()
        {

            StringBuilder b = new StringBuilder();

            Dictionary<ResourceType, int> ResDict = new Dictionary<ResourceType, int>();
            foreach(EnemyResource res in Res)
            {
                if(res.ResType != null)
                {
                    if (ResDict.ContainsKey(res.ResType))
                    {
                        ResDict[res.ResType] += res.Count;
                    }
                    else
                    {
                        ResDict.Add(res.ResType, res.Count);
                    }
                }
                if(res.BpType != null)
                {
                    b.AppendLine("(blueprint) " + res.BpType.ToString() + ": " + res.Count);
                }
            }

            
            foreach(ResourceType rType in ResDict.Keys)
            {
                b.AppendLine(rType.ToString() + ": " + ResDict[rType].ToString());
            }
            return b.ToString();

        }

    }

    public void SaveData()
    {

    }

}

class CompressedBattleScene
{
    int BattleSceneTypeId { get; set; }

    public List<Cycle> Cycles { get; set; }

    public SpaceshipRig PlayerRig { get; set; }

    public Dictionary<int, BattleSceneType.Enemy> EnemyDict;
    public Dictionary<int, ResourceType> ResourceDict;
    public Dictionary<int, BlueprintType> BlueprintDict;


    public CompressedBattleScene()
    {

    }

    public CompressedBattleScene(BattleScene createFrom)
    {
        BattleSceneTypeId = createFrom.SceneType.Id;
        PlayerRig = createFrom.Rig;

        EnemyDict = new Dictionary<int, BattleSceneType.Enemy>();
        ResourceDict = new Dictionary<int, ResourceType>();
        BlueprintDict = new Dictionary<int, BlueprintType>();

        Cycles = new List<Cycle>();
        foreach (var cycle in createFrom.Cycles)
        {
            Cycles.Add(new Cycle(cycle, EnemyDict, ResourceDict, BlueprintDict));
        }

    }

    public class Cycle
    {
        public int Number { get; set; }
        public List<Stage> Stages { get; set; }

        public Cycle() { }
        public Cycle(BattleScene.Cycle cycleFrom, 
                     Dictionary<int, BattleSceneType.Enemy> EnemyDict,
                     Dictionary<int, ResourceType> ResourceDict,
                     Dictionary<int, BlueprintType> BlueprintDict)
        {
            Number = cycleFrom.Number;
            Stages = new List<Stage>();
            foreach(var stage in cycleFrom.Stages)
            {
                Stages.Add(new Stage(stage, EnemyDict, ResourceDict, BlueprintDict));
            }
        }

    }

    public class StageEnemyTemplate
    {
        public int StageNumber { get; set; }
        public int CycleNumber { get; set; }
        public int EnemyTypeId { get; set; }
        public int BattleIntensity { get; set; }
        public int EnemyStructurePoints { get; set; }
        public int EnemyShieldPoints { get; set; }
        public int EnemyShieldRegen { get; set; }
        public int Count { get; set; }
        public StageEnemyTemplate() { }

        public StageEnemyTemplate(BattleScene.StageEnemy enemy)
        {
            StageNumber = enemy.StageNumber;
            CycleNumber = enemy.CycleNumber;
            EnemyTypeId = enemy.EnemyType.Id;
            BattleIntensity = enemy.BattleIntensity;
            EnemyStructurePoints = enemy.EnemyStructurePoints;
            EnemyShieldPoints = enemy.EnemyShieldPoints;
            EnemyShieldRegen = enemy.EnemyShieldRegen;
            Count += 1;
        }

    }

    public class EnemyResource
    {
        public int ResTypeId { get; set; }
        public int BpTypeId { get; set; }
        public int Count { get; set; }
        public int EnemyIdx { get; set; }
        public EnemyResource() { }
        public EnemyResource(BattleScene.EnemyResource res, int idx)
        {
            if(res.ResType != null)
                ResTypeId = res.ResType.Id;
            if(res.BpType != null)
                BpTypeId = res.BpType.Id;
            Count = res.Count;
            EnemyIdx = idx;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            if (ResTypeId > 0)
                b.Append("res: " + ResTypeId);
            if (BpTypeId > 0)
                b.Append(" bp: " + BpTypeId);
            return b.ToString();
        }

    }

    public class Stage
    {
        public int CycleNumber { get; set; }
        public int StageNumber { get; set; }
        public List<StageEnemyTemplate> EnemyTemplates { get; set; }
        public List<EnemyResource> enmResource { get; set; }

        public Stage() { }
        public Stage(BattleScene.Stage stageFrom,
                     Dictionary<int, BattleSceneType.Enemy> EnemyDict,
                     Dictionary<int, ResourceType> ResourceDict,
                     Dictionary<int, BlueprintType> BlueprintDict)
        {
            CycleNumber = stageFrom.CycleNumber;
            StageNumber = stageFrom.StageNumber;
            int curEnemyId = 0;
            EnemyTemplates = new List<StageEnemyTemplate>();
            enmResource = new List<EnemyResource>();
            int idx = 0;
            foreach(var enemy in stageFrom.Enemy)
            {
                if(enemy.EnemyType.Id != curEnemyId)
                {
                    if (!EnemyDict.ContainsKey(enemy.EnemyType.Id))
                        EnemyDict.Add(enemy.EnemyType.Id, enemy.EnemyType);
                    EnemyTemplates.Add(new StageEnemyTemplate(enemy));
                    curEnemyId = enemy.EnemyType.Id;
                }
                else
                {
                    EnemyTemplates[EnemyTemplates.Count - 1].Count += 1;
                }
                if (enemy.Resources.Count > 0)
                {
                    foreach (var res in enemy.Resources)
                    {
                        enmResource.Add(new EnemyResource(res, idx));
                        if (res.ResType != null)
                        {
                            if (!ResourceDict.ContainsKey(res.ResType.Id))
                                ResourceDict.Add(res.ResType.Id, res.ResType);
                        }
                        if (res.BpType != null)
                        {
                            if (!BlueprintDict.ContainsKey(res.BpType.Id))
                                BlueprintDict.Add(res.BpType.Id, res.BpType);
                        }
                    }
                }
                idx = idx + 1;
            }
        }
    }
}
