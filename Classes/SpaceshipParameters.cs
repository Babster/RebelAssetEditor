using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SpaceshipParameters
{

    public List<ParameterAndValue> ParameterList { get; set; }

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

    public override string ToString()
    {
        StringBuilder t = new StringBuilder();
        bool stringStarted = false;
        foreach(ParameterAndValue param in this.ParameterList)
        {
            if (stringStarted)
                t.Append("/");
            if(param.Value > 0)
            {
                t.Append(param.Value.ToString());
            }
        }
        return t.ToString();
    }

}
