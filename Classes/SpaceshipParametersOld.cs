using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Crew;

/*public class SpaceshipParameters
{

    public Ship ship { get; set; }

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
        if (ParameterDict == null)
        {
            CreateDict();
        }
        else if (ParameterDict.Count == 0)
        {
            CreateDict();
        }
        if (ParameterDict.ContainsKey(paramType))
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

        private static Dictionary<SpaceshipParameters.SpaceShipParameter, string> pTypeDict;
        private static void FillTypeDict()
        {
            pTypeDict = new Dictionary<SpaceShipParameter, string>();
            pTypeDict.Add(SpaceShipParameter.StructureHitpoints, "Structure hitpoints");
            pTypeDict.Add(SpaceShipParameter.ShieldDPS, "Shield DPS");
            pTypeDict.Add(SpaceShipParameter.StructureDPS, "Structure DPS");
            pTypeDict.Add(SpaceShipParameter.ArmorPoints, "Armor points");
            pTypeDict.Add(SpaceShipParameter.ShieldPoints, "Shield points");
            pTypeDict.Add(SpaceShipParameter.ShieldRegen, "Shield regen");
            pTypeDict.Add(SpaceShipParameter.Speed, "Speed");
            pTypeDict.Add(SpaceShipParameter.Dexterity, "Dexterity");
            pTypeDict.Add(SpaceShipParameter.Engine, "Engine");
            pTypeDict.Add(SpaceShipParameter.FireRate, "Fire rate");
            pTypeDict.Add(SpaceShipParameter.ShieldDamage, "Shield damage");
            pTypeDict.Add(SpaceShipParameter.StructureDamage, "Structure damage");
            pTypeDict.Add(SpaceShipParameter.EnergyDamage, "Energy damage");
            pTypeDict.Add(SpaceShipParameter.KineticDamage, "Kinetic damage");
            pTypeDict.Add(SpaceShipParameter.RocketDamage, "Rocket damage");
        }
        
        public string tString
        {
            get
            {
                if (pTypeDict == null)
                    FillTypeDict();
                return pTypeDict[ParamType];
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
        //foreach(ParameterAndValue param in module.parameters.ParameterList)
        //{
        //    if(param.Value > 0)
        //    {
        //        this.AddParameter(param.ParamType, param.Value);
        //    }
        //}
    }

    public void AddWeapon(ShipModuleType weapon)
    {
        Weapons.Add(new WeaponParameters(weapon));
    }

    public void AddOfficersParameters(RigSlotOfficerTeam team)
    {
        if (team.OfficerList.Count == 0)
            return;

        foreach(CrewOfficer curOfficer in team.OfficerList)
        {

            //Умножение обычных параметров на параметры офицера
            double EngineBoost = curOfficer.StatValue(OfficerTypeStat.StatType.EngineBoost);
            EngineBoost = (1 + (EngineBoost / 100));
            AdjustParameter(SpaceShipParameter.Engine, EngineBoost);

            double ThrustersBoost = curOfficer.StatValue(OfficerTypeStat.StatType.ThrustersBoost);
            ThrustersBoost = (1 + (ThrustersBoost / 100));
            AdjustParameter(SpaceShipParameter.Speed, ThrustersBoost);
            AdjustParameter(SpaceShipParameter.Dexterity, ThrustersBoost);

            double ShieldsBoost = curOfficer.StatValue(OfficerTypeStat.StatType.ShieldsBoost);
            ShieldsBoost = (1 + (ShieldsBoost / 5));
            AdjustParameter(SpaceShipParameter.ShieldPoints, ShieldsBoost);
            AdjustParameter(SpaceShipParameter.ShieldRegen, ShieldsBoost);

            double ArmorBoost = curOfficer.StatValue(OfficerTypeStat.StatType.ArmorBoost);
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

            switch(Weapon.WeaponType)
            {
                case ShipModuleType.WeaponTypes.Laser:
                    Bonus = officer.StatValue(OfficerTypeStat.StatType.EnergyWeapons);
                    break;
                case ShipModuleType.WeaponTypes.Kinetic:
                    Bonus = officer.StatValue(OfficerTypeStat.StatType.KineticWeapons);
                    break;
                case ShipModuleType.WeaponTypes.Explosive:
                    Bonus = officer.StatValue(OfficerTypeStat.StatType.RocketWeapons);
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

        if (ship != null)
        {
            t.AppendLine("Ship: " + ship.ToString());
            t.AppendLine("");
        }

        
        foreach(ParameterAndValue param in ParameterList)
        {
            t.AppendLine(param.tString + ": " + param.Value);
        }
        t.AppendLine("");

        if (Weapons.Count > 0)
        {
            foreach (WeaponParameters weapon in Weapons)
            {
                t.AppendLine($"Weapon: {weapon.Weapon.Name}");
                t.AppendLine($"{weapon.FireRate}/{weapon.ShieldDamage}/{weapon.StructureDamage} : {weapon.ShieldDPS}/{weapon.StructureDPS}");
            }
        }
        else
        {
            t.AppendLine("Weapons: none");
        }

        return t.ToString();
    }

    public string ParameterNamesString()
    {
        StringBuilder t = new StringBuilder();
        foreach (ParameterAndValue param in ParameterList)
        {
            t.AppendLine(param.tString);
        }

        if (Weapons.Count > 0)
        {
            foreach (WeaponParameters weapon in Weapons)
            {
                t.AppendLine("Weapon: " + weapon.Weapon.Name);
                t.AppendLine("Fire rate");
                t.AppendLine("Shield damage");
                t.AppendLine("Structure damage");
            }
        }

        return t.ToString();
    }

    public string ParameterValuesString()
    {
        StringBuilder t = new StringBuilder();
        foreach (ParameterAndValue param in ParameterList)
        {
            t.AppendLine(param.tString);
        }


        if (Weapons.Count > 0)
        {
            foreach (WeaponParameters weapon in Weapons)
            {
                t.AppendLine("");
                t.AppendLine(weapon.FireRate.ToString());
                t.AppendLine(weapon.ShieldDamage.ToString());
                t.AppendLine(weapon.StructureDamage.ToString());
            }
        }

        return t.ToString();

    }

}*/
