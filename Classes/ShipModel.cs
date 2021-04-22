using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class ShipModel
{

    public int Id { get; set; }
    public int Parent { get; set; }
    public int BaseStructureHp { get; set; }
    public int BattleIntensity { get; set; }
    public string Name { get; set; }
    public string AssetName { get; set; }
    public List<Slot> slots;
    public List<int> slotsToDelete;

    public ShipModel(int parentId)
    {
        slots = new List<Slot>();
        slotsToDelete = new List<int>();
        this.Parent = parentId;
        this.Name = "New design";
    }

    public ShipModel(ref SqlDataReader r)
    {
        slotsToDelete = new List<int>();

        this.Id = Convert.ToInt32(r["id"]);
        this.Parent = Convert.ToInt32(r["parent"]);
        this.BaseStructureHp = Convert.ToInt32(r["base_structure_hp"]);
        this.BattleIntensity = Convert.ToInt32(r["intensity_amount"]);
        this.Name = Convert.ToString(r["name"]);
        this.AssetName = Convert.ToString(r["asset_name"]);

        LoadSlots();

    }

    private void LoadSlots()
    {
        slots = new List<Slot>();
        string q;
        q = @"
                SELECT
                    id,
                    ss_design_id,
                    slot_number,
                    slot_type,
                    default_module_id
                FROM
                    ss_designs_slots
                WHERE
                    ss_design_id = " + Id;
        SqlDataReader r;
        r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while (r.Read())
            {
                slots.Add(new Slot(ref r));
            }
        }
        r.Close();
    }

    public void SaveData()
    {
        string q;
        if (this.Id == 0)
        {
            q = @"INSERT INTO ss_designs(name) VALUES('')
                        SELECT @@IDENTITY AS Result";
            this.Id = DataConnection.GetResultInt(q);
        }

        q = @"
                UPDATE ss_designs SET
                    parent = " + this.Parent.ToString() + @",
                    base_structure_hp = " + this.BaseStructureHp.ToString() + @",
                    intensity_amount= " + this.BattleIntensity.ToString() + @",
                    name = @str1,
                    asset_name = @str2 
                WHERE
                    id = " + this.Id.ToString();

        List<string> names = new List<string>();
        names.Add(this.Name);
        names.Add(this.AssetName);

        DataConnection.Execute(q, names);

        if (slots.Count > 0)
        {
            foreach (Slot slot in slots)
            {
                if (slot.ShipDesignId == 0)
                    slot.ShipDesignId = this.Id;
                slot.SaveData();
            }
        }
        if (slotsToDelete.Count > 0)
        {
            q = "DELETE FROM slots WHERE id IN";
            bool addComma = false;
            foreach (int slotId in slotsToDelete)
            {
                if (addComma)
                    q = q + ",";
                q = q + slotId;
                addComma = true;
            }
            q = q + ")";
            DataConnection.Execute(q);
            slotsToDelete.Clear();
        }

    }

    public Slot AddSlot()
    {
        Slot slot = new Slot(this.Id);
        slots.Add(slot);
        return slot;
    }

    public void DeleteSlot(ref Slot slot)
    {
        slots.Remove(slot);
        slotsToDelete.Add(slot.Id);
    }

    public class Slot
    {
        public int Id { get; set; }
        public int ShipDesignId { get; set; }
        public int SlotNumber { get; set; }
        public string SlotType { get; set; }
        public int DefaultModuleId { get; set; }

        public Slot(int SlotNumber)
        {
            this.SlotNumber = SlotNumber;
        }

        public Slot(ref SqlDataReader r)
        {
            Id = Convert.ToInt32(r["Id"]);
            ShipDesignId = Convert.ToInt32(r["ss_design_id"]);
            SlotNumber = Convert.ToInt32(r["slot_number"]);
            SlotType = Convert.ToString(r["slot_type"]);
            DefaultModuleId = Convert.ToInt32(r["default_module_id"]);
        }

        public override string ToString()
        {
            return SlotType + " (" + SlotNumber + "/" + Id + ")";
        }

        public void SaveData()
        {
            string q;
            if (Id == 0)
            {
                q = "INSERT INTO ss_designs_slots(ss_design_id) VALUES(" + ShipDesignId + @")
                            SELECT @@IDENTITY AS Result";
                Id = DataConnection.GetResultInt(q);
            }

            q = @"
                    UPDATE ss_designs_slots SET
                        slot_number = " + SlotNumber + @", 
                        slot_type = @str1, 
                        default_module_id = " + DefaultModuleId + @"
                    WHERE id = " + Id.ToString();
            List<string> names = new List<string> { SlotType };
            DataConnection.Execute(q, names);
        }

        public ShipModuleType module(ref Dictionary<int, ShipModuleType> ModuleDict)
        {
            if (ModuleDict.ContainsKey(DefaultModuleId))
            {
                return ModuleDict[DefaultModuleId];
            }
            else
            {
                return null;
            }
        }

    }

    public enum EnumShipParameter
    {
        StructurePoints = 1,
        ArmorPoints = 2,
        DeflectorPoints = 3,
        DeflectorRegen = 4,
        ShieldDPS = 5,
        StructureDPS = 6,
        Speed = 7,
        Dexterity = 8
    }
    public static string ParameterName(EnumShipParameter param)
    {
        switch (param)
        {
            case EnumShipParameter.StructurePoints:
                return "Structure points";
            case EnumShipParameter.ArmorPoints:
                return "Armor points";
            case EnumShipParameter.DeflectorPoints:
                return "Deflector points";
            case EnumShipParameter.DeflectorRegen:
                return "Deflector regen";
            case EnumShipParameter.ShieldDPS:
                return "Shield DPS";
            case EnumShipParameter.StructureDPS:
                return "Structure DPS";
            case EnumShipParameter.Speed:
                return "Speed";
            case EnumShipParameter.Dexterity:
                return "Dexterity";
            default:
                return "";
        }
    }
    public string PropertyString(EnumShipParameter prop)
    {
        switch (prop)
        {
            case EnumShipParameter.StructurePoints:
                return "Fire rate";
            case EnumShipParameter.ArmorPoints:
                return "Armor points";
            case EnumShipParameter.DeflectorPoints:
                return "Deflector points";
            case EnumShipParameter.DeflectorRegen:
                return "Deflector regen";
            case EnumShipParameter.ShieldDPS:
                return "Shield DPS";
            case EnumShipParameter.StructureDPS:
                return "Structure DPS";
            case EnumShipParameter.Speed:
                return "Speed";
            case EnumShipParameter.Dexterity:
                return "Dexterity";
            default:
                return "";

        }
    }

    public class Parameter
    {
        public EnumShipParameter ParamType { get; set; }
        public float Value { get; set; }
        public Parameter(EnumShipParameter p, float value)
        {
            this.ParamType = p;
            this.Value = (float)Math.Round(value, 2);
        }
    }

    public class DPSCounter
    {
        public float ShieldDPS { get; set; }
        public float StructureDPS { get; set; }
        public DPSCounter()
        {

        }

        public void AddWeapon(ref ShipModuleType module)
        {
            if (module.FireRate() > 0)
            {
                if (module.DeflectorsDamage() > 0)
                {
                    ShieldDPS += module.DeflectorsDamage() * ((float)module.FireRate() / 60);
                }
                if (module.StructureDamage() > 0)
                {
                    StructureDPS += module.StructureDamage() * ((float)module.FireRate() / 60);
                }
            }

        }

    }

    public class DeflectorsCounter
    {
        public int Points { get; set; }
        public int Recharge { get; set; }
        public DeflectorsCounter() { }

        public void AddModule(ref ShipModuleType tag)
        {
            Points += tag.Deflectors();
            Recharge += tag.DeflectorsRegen();
        }

    }

    public class ThrustersCounter
    {
        public int Speed { get; set; }
        public int Dexterity { get; set; }
        public ThrustersCounter() { }

        public void AddThruster(ref ShipModuleType tag)
        {
            this.Speed += tag.ThrustersSpeed();
            Dexterity += tag.ThrustersDexterity();
        }

    }

    public List<Parameter> GetParameters()
    {
        Dictionary<int, ShipModuleType> mDict = ShipModuleType.CreateModuleDict();
        List<Parameter> pms = new List<Parameter>();
        pms.Add(new Parameter(EnumShipParameter.StructurePoints, this.BaseStructureHp));

        DPSCounter dps = new DPSCounter();
        DeflectorsCounter deflect = new DeflectorsCounter();
        ThrustersCounter th = new ThrustersCounter();

        int armor = 0;

        foreach (Slot slot in this.slots)
        {
        ShipModuleType tag = slot.module(ref mDict);
            if (tag != null)
            {
                dps.AddWeapon(ref tag);
                deflect.AddModule(ref tag);
                th.AddThruster(ref tag);
                armor += tag.Armor();
            }
        }
        pms.Add(new Parameter(EnumShipParameter.ArmorPoints, armor));
        pms.Add(new Parameter(EnumShipParameter.DeflectorPoints, deflect.Points));
        pms.Add(new Parameter(EnumShipParameter.DeflectorRegen, deflect.Recharge));
        pms.Add(new Parameter(EnumShipParameter.ShieldDPS, dps.ShieldDPS));
        pms.Add(new Parameter(EnumShipParameter.StructureDPS, dps.StructureDPS));
        pms.Add(new Parameter(EnumShipParameter.Speed, th.Speed));
        pms.Add(new Parameter(EnumShipParameter.Dexterity, th.Dexterity));
        return pms;
    }

    public static string ShipModelQuery()
    {
        return @"
                SELECT 
                    id,
                    parent,
                    ISNULL(intensity_amount, 0) AS intensity_amount,
                    ISNULL(base_structure_hp, 0) AS base_structure_hp,
                    name,
                    asset_name
                FROM
                    ss_designs";
    }

    public static List<ShipModel> GetModelList()
    {
        String q;
        List<ShipModel> modelList = new List<ShipModel>();
        q = ShipModelQuery();
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while(r.Read())
            {
                ShipModel curModel = new ShipModel(ref r);
                modelList.Add(curModel);
            }
        }
        r.Close();
        return modelList;
    }

    public override string ToString()
    {
        return this.Name + ", " + this.Id;
    }

}
