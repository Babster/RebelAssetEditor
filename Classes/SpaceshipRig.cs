using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class SpaceshipRig
{
    public int Id { get; set; }
    public ShipModel sModel { get; set; }

    public List<RigSlot> Slots { get; set; }

    public SpaceshipRig() { }

    public SpaceshipRig(int id)
    {
        this.Id = id;

        SqlDataReader r = DataConnection.GetReader(SpaceshipRigQuery(id));
        r.Read();
        int ShipModelId = Convert.ToInt32(r["ship_model_id"]);
        r.Close();

        sModel = ShipModel.ModelById(ShipModelId);

        LoadSlots();

    }

    public void LoadShipModel(ShipModel sModel)
    {
        this.sModel = sModel;
        LoadSlots();
    }

    private void LoadSlots()
    {

        if (sModel == null)
            return;

        //Загрузка слотов - сначала подкачиваются все слоты которые есть в модели корабля,
        //а затем на них накатываются модули, которые установлены в эти слоты согласно 
        Slots = new List<RigSlot>();

        Dictionary<int, RigSlot> slotDict = new Dictionary<int, RigSlot>();

        foreach(ShipModel.Slot modelSlot in sModel.slots)
        {
            RigSlot curSlot = new RigSlot(modelSlot);
            Slots.Add(curSlot);
            slotDict.Add(curSlot.Slot.Id, curSlot);
        }

        
        if(Id > 0)
        {
            string q;
            q = $@"
            SELECT
                id,
                ss_rig_id,
                slot_id,
                module_type_id
            FROM
                ss_rigs_slots
            WHERE
                ss_rig_id = {this.Id}";

            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    int SlotId = Convert.ToInt32(r["slot_id"]);
                    slotDict[SlotId].LoadRig(r);
                }
            }
        }

    }

    private string SpaceshipRigQuery(int id)
    {
        string q;
        q = $@"
        SELECT
            ss_rigs.ship_model_id
        FROM
            ss_rigs
        WHERE
            ss_rigs.id = {id}
            ";
        return q;
    }

    public class RigSlot
    {
        public int Id { get; set; }
        public ShipModel.Slot Slot { get; set; }
        public ShipModuleType ModuleType { get; set; }
        
        public RigSlot(ShipModel.Slot modelSlot)
        {
            this.Slot = modelSlot;
        }
        
        /// <summary>
        /// Если в слот помещается подходящий модуль, то возвращается пустая строка.
        /// Если не подходит - то программа возвращается строку - описание ошибки
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string LoadModuleType(ShipModuleType moduleType)
        {
            string compareResult = Slot.ModuleFitsSlot(moduleType);
            if(compareResult != "")
                return compareResult;

            this.ModuleType = moduleType;
            return "";
        }

        public void LoadRig(SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            int moduleTypeId = Convert.ToInt32(r["module_type_id"]);
            this.ModuleType = ShipModuleType.ModuleById(moduleTypeId);
        }

        public void SaveData(int rigId)
        {
            string q;
            if(Id == 0)
            {
                q = $@"
                    INSERT INTO ss_rigs_slots(ss_rig_id) VALUES({rigId})
                    SELECT @@IDENTITY AS Result";
                this.Id = DataConnection.GetResultInt(q);
            }

            int slotId = 0;
            if (Slot != null)
                slotId = Slot.Id;
            int moduleTypeId = 0;
            if (ModuleType != null)
                moduleTypeId = ModuleType.Id;

            q = $@"
            UPDATE ss_rigs_slots SET 
                slot_id = {slotId},
                module_type_id = {moduleTypeId}
            WHERE
                id = {Id}";

            DataConnection.Execute(q);
        }

    }

}

