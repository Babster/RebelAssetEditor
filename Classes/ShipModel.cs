using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class ShipModel : UnityShipModel
{

    public ShipModel() { }

    public ShipModel(int parentId)
    {
        slots = new List<ShipModelSlot>();
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
        this.ImgId = (int)r["img_id"];
        LoadSlots();

    }

    private void LoadSlots()
    {
        slots = new List<ShipModelSlot>();
        string q;
        q = @"
                SELECT
                    id,
                    ss_design_id,
                    slot_number,
                    slot_type,
                    ISNULL(slot_control, '') AS slot_control, 
                    ISNULL(size, 1) AS size,
                    default_module_id,
                    ISNULL(is_main_cabin, 0) AS is_main_cabin
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
                slots.Add(new ShipModelSlot(ref r));
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

        q = $@"
                UPDATE ss_designs SET
                    parent = {this.Parent.ToString()},
                    base_structure_hp = {this.BaseStructureHp.ToString()},
                    intensity_amount= {this.BattleIntensity.ToString()},
                    name = @str1,
                    asset_name = @str2,
                    img_id = {ImgId}
                WHERE
                    id = " + this.Id.ToString();

        List<string> names = new List<string>();
        names.Add(this.Name);
        names.Add(this.AssetName);

        DataConnection.Execute(q, names);

        if (slots.Count > 0)
        {
            foreach (ShipModelSlot slot in slots)
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

    public ShipModelSlot AddSlot()
    {
        ShipModelSlot slot = new ShipModelSlot(this.Id);
        slots.Add(slot);
        return slot;
    }

    public void DeleteSlot(ref ShipModelSlot slot)
    {
        slots.Remove(slot);
        slotsToDelete.Add(slot.Id);
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
                    asset_name,
                    ISNULL(img_id, 0) AS img_id
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

    public static Dictionary<int, ShipModel> GetModelDict()
    {
        List<ShipModel> list = GetModelList();
        Dictionary<int, ShipModel> mDict = new Dictionary<int, ShipModel>();
        foreach(ShipModel model in list)
        {
            mDict.Add(model.Id, model);
        }
        return mDict;
    }


    public static ShipModel ModelById(int id)
    {
        if(StaticMembers.pModels == null)
        {
            StaticMembers.pModels = GetModelDict();
        }
        if(StaticMembers.pModels.ContainsKey(id))
        {
            return StaticMembers.pModels[id];
        }
        else
        {
            return null;
        }
    }

    public override string ToString()
    {
        return this.Name + ", " + this.Id;
    }

}

public class ShipModelSlot : UnityShipModelSlot
{


    public ShipModelSlot() { }
    public ShipModelSlot(int SlotNumber)
    {
        this.SlotNumber = SlotNumber;
        this.Size = 1;
    }

    public ShipModelSlot(ref SqlDataReader r)
    {
        Id = Convert.ToInt32(r["Id"]);
        ShipDesignId = Convert.ToInt32(r["ss_design_id"]);
        SlotNumber = Convert.ToInt32(r["slot_number"]);
        SlotTypeStr = Convert.ToString(r["slot_type"]);
        Size = Convert.ToInt32(r["size"]);
        SlotControl = Convert.ToString(r["slot_control"]);
        DefaultModuleId = Convert.ToInt32(r["default_module_id"]);
        MainCabin = (int)r["is_main_cabin"];
        
        if (Size < 1)
            Size = 1;
    }

    public override string ToString()
    {
        return SlotTypeStr + " (" + SlotNumber + "/" + Id + ")";
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

        q = $@"
                    UPDATE ss_designs_slots SET
                        slot_number = {SlotNumber}, 
                        slot_type = @str1,
                        size = {Size},
                        slot_control = @str2,
                        default_module_id = {DefaultModuleId},
                        is_main_cabin = {MainCabin.ToString()}
                    WHERE id = {Id}";
        List<string> names = new List<string> { SlotTypeStr, SlotControl };
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

    public bool SlotForModule
    {
        get
        {
            List<SlotType> forModules = new List<SlotType>();
            forModules.Add(SlotType.Armor);
            forModules.Add(SlotType.Engine);
            forModules.Add(SlotType.Misc);
            forModules.Add(SlotType.Thrusters);
            forModules.Add(SlotType.Weapon);
            return forModules.Contains(sType);
        }
    }

    public bool SlotForOfficer
    {
        get
        {
            List<SlotType> forOfficers = new List<SlotType>();
            forOfficers.Add(SlotType.Cabin);
            forOfficers.Add(SlotType.ExtendedCabin);
            forOfficers.Add(SlotType.ControlRoom);
            forOfficers.Add(SlotType.ExtendedControlRoom);
            return forOfficers.Contains(sType);
        }
    }

    public bool SlotForCrew
    {
        get
        {
            List<SlotType> forCrew = new List<SlotType>();
            forCrew.Add(SlotType.ExtendedCabin);
            forCrew.Add(SlotType.ExtendedControlRoom);
            return forCrew.Contains(sType);
        }
    }

    public string ModuleFitsSlot(ShipModuleType module)
    {
        if ((int)module.ModuleTypeFromStr() != (int)this.sType)
            return "Slot type doesn't fit module type";
        if (module.Size != this.Size)
            return "Slot size is too small";
        return "";
    }

}

public class UnityShipModel
{
    public int Id { get; set; }
    public int Parent { get; set; }
    public int BaseStructureHp { get; set; }
    public int BattleIntensity { get; set; }
    public string Name { get; set; }
    public string AssetName { get; set; }
    public List<ShipModelSlot> slots;
    public List<int> slotsToDelete;
    public int ImgId { get; set; }

    public int WeaponSlotCount
    {
        get
        {
            int count = 0;
            foreach(ShipModelSlot slot in slots)
            {
                if (slot.sType == ShipModelSlot.SlotType.Weapon)
                    count += 1;
            }
            return count;
        }
    }
    public void ClearSlotDuplicates()
    {
        List<int> usedIds = new List<int>();
        int idx = 0;
        for(int i = 0; i<slots.Count; i++)
        {
            if(usedIds.Contains(slots[i].Id))
            {
                idx = i;
                return;
            }
        }

        if(idx > 0)
        {
            for(int i = slots.Count - 1; i >= idx; i--)
            {
                slots.RemoveAt(i);
            }
        }
    }
}

public class UnityShipModelSlot
{
    public int Id { get; set; }
    public int ShipDesignId { get; set; }
    public int SlotNumber { get; set; }
    public string SlotTypeStr { get; set; }
    public int Size { get; set; }
    public string SlotControl { get; set; }
    public int DefaultModuleId { get; set; }
    public int MainCabin { get; set; }

    public enum SlotType
    {
        None = 0,
        Weapon = 1,
        Armor = 2,
        Engine = 3,
        Thrusters = 4,
        Misc = 5,
        Cabin = 20,
        Cabin3 = 24,
        ExtendedCabin = 21,
        ControlRoom = 22,
        ExtendedControlRoom = 23
    }

    private static Dictionary<string, SlotType> StringToSlotDict;
    private static Dictionary<SlotType, string> SlotToStringDict;
    private void FillDicts()
    {
        StringToSlotDict = new Dictionary<string, SlotType>();
        StringToSlotDict.Add("None", SlotType.None);
        StringToSlotDict.Add("Weapon", SlotType.Weapon);
        StringToSlotDict.Add("Armor", SlotType.Armor);
        StringToSlotDict.Add("Engine", SlotType.Engine);
        StringToSlotDict.Add("Thrusters", SlotType.Thrusters);
        StringToSlotDict.Add("Misc", SlotType.Misc);
        StringToSlotDict.Add("Cabin", SlotType.Cabin);
        StringToSlotDict.Add("ExtendedCabin", SlotType.ExtendedCabin);
        StringToSlotDict.Add("ControlRoom", SlotType.ControlRoom);
        StringToSlotDict.Add("ExtendedControlRoom", SlotType.ExtendedControlRoom);

        SlotToStringDict = new Dictionary<SlotType, string>();
        SlotToStringDict.Add(SlotType.None, "None");
        SlotToStringDict.Add(SlotType.Weapon, "Weapon");
        SlotToStringDict.Add(SlotType.Armor, "Armor");
        SlotToStringDict.Add(SlotType.Engine, "Engine");
        SlotToStringDict.Add(SlotType.Thrusters, "Thrusters");
        SlotToStringDict.Add(SlotType.Misc, "Misc");
        SlotToStringDict.Add(SlotType.Cabin, "Cabin");
        SlotToStringDict.Add(SlotType.ExtendedCabin, "ExtendedCabin");
        SlotToStringDict.Add(SlotType.ControlRoom, "ControlRoom");
        SlotToStringDict.Add(SlotType.ExtendedControlRoom, "ExtendedControlRoom");

    }
    public SlotType sType
    {
        get
        {
            if (StringToSlotDict == null)
                FillDicts();
            return StringToSlotDict[SlotTypeStr];
        }
        set
        {
            if (StringToSlotDict == null)
                FillDicts();
            SlotTypeStr = SlotToStringDict[value];
        }
    }

}