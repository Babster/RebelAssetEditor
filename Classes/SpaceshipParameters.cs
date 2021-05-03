using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Crew;

public class SpaceshipParameters
{


    public List<ParameterAndValue> ParameterList { get; set; }

    [NonSerialized()]
    private Dictionary<SpaceShipParameter, ParameterAndValue> ParameterDict;

    [NonSerialized()]
    public List<WeaponParameters> Weapons;

    public int Count
    {
        get
        {
            return ParameterList.Count;
        }
    }

    public SpaceshipParameters() 
    {
        ParameterList = new List<ParameterAndValue>();
        ParameterDict = new Dictionary<SpaceShipParameter, ParameterAndValue>();
        Weapons = new List<WeaponParameters>();
    }

    public static SpaceshipParameters CreateFromString(string JsonString)
    {
        SpaceshipParameters tParam = JsonConvert.DeserializeObject<SpaceshipParameters>(JsonString);
        if (tParam.ParameterList == null)
            tParam.ParameterList = new List<ParameterAndValue>();
        tParam.CreateDict();
        return tParam;
    }

    public void CreateDict()
    {
        ParameterDict = new Dictionary<SpaceShipParameter, ParameterAndValue>();
        foreach(ParameterAndValue param in ParameterList)
        {
            ParameterDict.Add(param.ParamType, param);
        }
    }

    public enum SpaceShipParameter
    {
        //Accorded to spaceship 
        StructureHitpoints = 1,
        ShieldDPS = 2,
        StructureDPS = 3,

        //Accorded to both spaceship and modules 
        ArmorPoints = 4,
        ShieldPoints = 5,
        ShieldRegen = 6,
        Speed = 7,
        Dexterity = 8,
        Engine = 9,

        //Accorded to module only
        EnergyDamage = 10,
        KineticDamage = 11,
        RocketDamage = 12,

        //For a weapon after all calculations
        FireRate = 50,
        ShieldDamage = 51,
        StructureDamage = 52
    }

    public void SetParameter(SpaceShipParameter parameterType, double Value)
    {
        if (ParameterDict.ContainsKey(parameterType))
        {
            ParameterDict[parameterType].Value = Value;
        }
        else
        {
            ParameterAndValue param2 = new ParameterAndValue(parameterType, Value);
            ParameterList.Add(param2);
            ParameterDict.Add(param2.ParamType, param2);
        }
    }

    public void AddParameter(SpaceShipParameter parameterType, double Value)
    {
        if (ParameterDict.ContainsKey(parameterType))
        {
            ParameterDict[parameterType].Value += Value;
        }
        else
        {
            ParameterAndValue param2 = new ParameterAndValue(parameterType, Value);
            ParameterList.Add(param2);
            ParameterDict.Add(param2.ParamType, param2);
        }
    }

    public double ParameterValue(SpaceShipParameter paramType)
    {
        if(ParameterDict.ContainsKey(paramType))
        {
            return ParameterDict[paramType].Value;
        }
        else
        {
            return 0;
        }
    }

    public void AdjustParameter(SpaceShipParameter paramType, double multiplier)
    {
        if(ParameterDict.ContainsKey(paramType))
        {
            ParameterDict[paramType].Value = (int)(ParameterDict[paramType].Value * multiplier);
        }
        
    }

    public class ParameterAndValue
    {
        public SpaceShipParameter ParamType { get; set; }
        public double Value;

        public ParameterAndValue(SpaceShipParameter paramType, double value)
        {
            this.ParamType = paramType;
            this.Value = value;
        }


        private static void FillTypeDict()
        {
            StaticMembers.pTypeDict = new Dictionary<SpaceShipParameter, string>();
            StaticMembers.pTypeDict.Add(SpaceShipParameter.StructureHitpoints, "Structure hitpoints");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.ShieldDPS, "Shield DPS");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.StructureDPS, "Structure DPS");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.ArmorPoints, "Armor points");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.ShieldPoints, "Shield points");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.ShieldRegen, "Shield regen");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.Speed, "Speed");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.Dexterity, "Dexterity");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.Engine, "Engine");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.FireRate, "Fire rate");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.ShieldDamage, "Shield damage");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.StructureDamage, "Structure damage");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.EnergyDamage, "Energy damage");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.KineticDamage, "Kinetic damage");
            StaticMembers.pTypeDict.Add(SpaceShipParameter.RocketDamage, "Rocket damage");
        }
        
        public string tString
        {
            get
            {
                if (StaticMembers.pTypeDict == null)
                    FillTypeDict();
                return StaticMembers.pTypeDict[ParamType];
            }
        }
    }
    
    public string ToDbString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public void AddShipModelParameters(ShipModel model)
    {
        AddParameter(SpaceShipParameter.StructureHitpoints, model.BaseStructureHp);
    }

    public void AddModuleParameters(ShipModuleType module)
    {
        foreach(ParameterAndValue param in module.parameters.ParameterList)
        {
            if(param.Value > 0)
            {
                this.AddParameter(param.ParamType, param.Value);
            }
        }
    }

    public void AddWeapon(ShipModuleType weapon)
    {
        Weapons.Add(new WeaponParameters(weapon));
    }

    public void AddOfficersParameters(SpaceshipRig.RigSlot.OfficerTeam team)
    {
        if (team.OfficerList.Count == 0)
            return;

        foreach(CrewOfficer curOfficer in team.OfficerList)
        {

            //Умножение обычных параметров на параметры офицера
            double EngineBoost = curOfficer.StatValue(CrewOfficerType.OfficerStat.StatType.EngineBoost);
            EngineBoost = (1 + (EngineBoost / 100));
            AdjustParameter(SpaceShipParameter.Engine, EngineBoost);

            double ThrustersBoost = curOfficer.StatValue(CrewOfficerType.OfficerStat.StatType.ThrustersBoost);
            ThrustersBoost = (1 + (ThrustersBoost / 100));
            AdjustParameter(SpaceShipParameter.Speed, ThrustersBoost);
            AdjustParameter(SpaceShipParameter.Dexterity, ThrustersBoost);

            double ShieldsBoost = curOfficer.StatValue(CrewOfficerType.OfficerStat.StatType.ShieldsBoost);
            ShieldsBoost = (1 + (ShieldsBoost / 5));
            AdjustParameter(SpaceShipParameter.ShieldPoints, ShieldsBoost);
            AdjustParameter(SpaceShipParameter.ShieldRegen, ShieldsBoost);

            double ArmorBoost = curOfficer.StatValue(CrewOfficerType.OfficerStat.StatType.ArmorBoost);
            ArmorBoost = (1 + (ArmorBoost / 5));
            AdjustParameter(SpaceShipParameter.ArmorPoints, ArmorBoost);

            //Добавление параметров вооружения
            if(Weapons.Count > 0)
            {
                foreach(WeaponParameters weapon in Weapons)
                {
                    weapon.LoadOfficer(curOfficer);
                }
            }
        }
    }

    public override string ToString()
    {
        StringBuilder t = new StringBuilder();
        bool stringStarted = false;
        foreach(ParameterAndValue param in this.ParameterList)
        {

            if(param.Value > 0)
            {
                if (stringStarted)
                    t.Append("/");
                t.Append(param.Value.ToString());
                stringStarted = true;
            }
        }
        return t.ToString();
    }

    public class WeaponParameters
    {

        public double ShieldDPS { get; set; }
        public double StructureDPS { get; set; }
        public int FireRate { get; set; }
        public double ShieldDamage { get; set; }
        public double StructureDamage { get; set; }

        public ShipModuleType Weapon { get; set; }

        public WeaponParameters(ShipModuleType Weapon)  
        {

            this.Weapon = Weapon;
            FireRate = (int)Weapon.GetParameter(SpaceShipParameter.FireRate);
            ShieldDamage = Weapon.GetParameter(SpaceShipParameter.ShieldDamage);
            StructureDamage = Weapon.GetParameter(SpaceShipParameter.StructureDamage);

            CalculateDPS();
        }

        public void LoadOfficer(CrewOfficer officer)
        {
            double Bonus = 0;

            switch(Weapon.wType)
            {
                case ShipModuleType.WeaponType.Energy:
                    Bonus = officer.StatValue(CrewOfficerType.OfficerStat.StatType.EnergyWeapons);
                    break;
                case ShipModuleType.WeaponType.Kinetic:
                    Bonus = officer.StatValue(CrewOfficerType.OfficerStat.StatType.KineticWeapons);
                    break;
                case ShipModuleType.WeaponType.Rocket:
                    Bonus = officer.StatValue(CrewOfficerType.OfficerStat.StatType.RocketWeapons);
                    break;
                default:
                    Bonus = 0;
                    break;
            }

            if (Bonus < 1)
                return;

            ShieldDamage = (ShieldDamage * (1 + (Bonus / 5)));
            StructureDamage = (StructureDamage * (1 + (Bonus / 5)));

            CalculateDPS();

        }

        private void CalculateDPS()
        {
            this.ShieldDPS = (double)ShieldDamage * FireRate / 60;
            this.StructureDPS = (double)StructureDamage * FireRate / 60;
        }

    }

    public string BottomlineString()
    {
        StringBuilder t = new StringBuilder();
        foreach(ParameterAndValue param in ParameterList)
        {
            t.AppendLine(param.tString + ": " + param.Value);
        }
        t.AppendLine("");

        if(Weapons.Count > 0)
        {
            foreach(WeaponParameters weapon in Weapons)
            {
                t.AppendLine("****Weapon****");
                t.AppendLine($"{weapon.FireRate}/{weapon.ShieldDamage}/{weapon.StructureDamage} : {weapon.ShieldDPS}/{weapon.StructureDPS}");
            }
        }

        return t.ToString();
    }

}
