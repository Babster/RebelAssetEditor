using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SpaceshipParameters
{

    public List<ParameterAndValue> ParameterList { get; set; }

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
    }

    public static SpaceshipParameters CreateFromString(string JsonString)
    {
        return JsonConvert.DeserializeObject<SpaceshipParameters>(JsonString);
    }

    public enum SpaceShipParameter
    {
        //Accorded to spaceship 
        StructureHitpoints = 1,
        ShieldDPS = 2,
        StructureDPS = 3,

        //Accorded to both spaceship and modules 
        ArmorPoints = 4,
        DeflectorPoints = 5,
        DeflectorRegen = 6,
        Speed = 7,
        Dexterity = 8,
        Engine = 9,

        //Accorded to module only
        FireRate = 10,
        DeflectorsDamage = 11,
        StructureDamage = 12

    }

    public void SetParameter(SpaceShipParameter parameterType, int Value)
    {
        foreach(ParameterAndValue param in ParameterList)
        {
            if(param.ParamType == parameterType)
            {
                param.Value = Value;
                return;
            }
        }
        ParameterAndValue param2 = new ParameterAndValue(parameterType, Value);
        ParameterList.Add(param2);
    }

    public void AddParameter(SpaceShipParameter parameterType, int Value)
    {
        foreach (ParameterAndValue param in ParameterList)
        {
            if (param.ParamType == parameterType)
            {
                param.Value += Value;
                return;
            }
        }
        ParameterAndValue param2 = new ParameterAndValue(parameterType, Value);
        ParameterList.Add(param2);
    }

    public int ParameterValue(SpaceShipParameter ParamType)
    {
        foreach(ParameterAndValue param in ParameterList)
        {
            if(param.ParamType == ParamType)
            {
                return param.Value;
            }
        }
        return 0;
    }

    public class ParameterAndValue
    {
        public SpaceShipParameter ParamType { get; set; }
        public int Value;

        public ParameterAndValue(SpaceShipParameter paramType, int value)
        {
            this.ParamType = paramType;
            this.Value = value;
        }

        public string TypeToString()
        {
            switch (ParamType)
                {
                case SpaceShipParameter.StructureHitpoints:
                    return "Structure hitpoints";
                case SpaceShipParameter.ShieldDPS:
                    return "Shield DPS";
                case SpaceShipParameter.StructureDPS:
                    return "Structure DPS";
                case SpaceShipParameter.ArmorPoints:
                    return "Armor points";
                case SpaceShipParameter.DeflectorPoints:
                    return "Deflector points";
                case SpaceShipParameter.DeflectorRegen:
                    return "Deflector regen";
                case SpaceShipParameter.Speed:
                    return "Speed";
                case SpaceShipParameter.Dexterity:
                    return "Dexterity";
                case SpaceShipParameter.Engine:
                    return "Engine";
                case SpaceShipParameter.FireRate:
                    return "Fire rate";
                case SpaceShipParameter.DeflectorsDamage:
                    return "Deflectors damage";
                case SpaceShipParameter.StructureDamage:
                    return "Structure damage";
                default:
                    return "error: unknown parameter type";
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
                if(param.ParamType == SpaceShipParameter.DeflectorsDamage)
                {
                    this.AddParameter(SpaceShipParameter.ShieldDPS, CalculateDPS(module.parameters, param));
                }
                if(param.ParamType == SpaceShipParameter.StructureDamage)
                {
                    this.AddParameter(SpaceShipParameter.StructureDPS, CalculateDPS(module.parameters, param));
                }
            }
        }
    }

    private int CalculateDPS(SpaceshipParameters parameters, ParameterAndValue curParameter)
    {
        double DPS = 0;
        double fireRate = parameters.ParameterValue(SpaceShipParameter.FireRate);
        double secondsInMinute = 60; //В редакторе Fire rate указывается как количество выстрелов в минуту
        double deflectorsDamage = curParameter.Value;
        DPS = deflectorsDamage * fireRate / secondsInMinute;
        return Convert.ToInt32(DPS);
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

}
