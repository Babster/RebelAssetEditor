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

        public bool ModuleFitsSlot(ShipModuleType module)
        {
            return true;
        }

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
