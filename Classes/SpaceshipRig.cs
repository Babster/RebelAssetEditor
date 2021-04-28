using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Crew;
using AdmiralNamespace;

public class SpaceshipRig
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Tag { get; set; }
    public ShipModel sModel { get; set; }

    public List<RigSlot> Slots { get; set; }

    public SpaceshipParameters Params { get; set; }

    public SpaceshipRig() { }

    public SpaceshipRig(int id)
    {
        this.Id = id;

        SqlDataReader r = DataConnection.GetReader(SpaceshipRigQuery(id, 0, "", false));
        r.Read();
        LoadRigInner(r);
        r.Close();
    }

    public SpaceshipRig(SqlDataReader r)
    {
        LoadRigInner(r);
    }

    private void LoadRigInner(SqlDataReader r)
    {
        Id = Convert.ToInt32( r["id"]);
        PlayerId = Convert.ToInt32(r["player_id"]);
        Tag = Convert.ToString(r["tag"]);
        int ShipModelId = Convert.ToInt32(r["ship_model_id"]);
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
                module_type_id,
                officer_ids,
                crew_id
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

    public static string SpaceshipRigQuery(int id, int playerId, string tag, bool builtIn)
    {
        string q;
        q = $@"
        SELECT
            id,
            ship_model_id,
            player_id,
            tag
        FROM
            ss_rigs ";
        if (id > 0)
        {
            q += $@" 
            WHERE
                ss_rigs.id = {id}
            ";
        }
        else if(playerId > 0)
        {
            q += $@" 
            WHERE
                ss_rigs.player_id = {playerId}
            ";
        }
        else if(tag != "")
        {
            q += $@" 
            WHERE
                ss_rigs.player_id = '{tag}'
            ";
        }
        else if(builtIn)
        {
            q += $@" 
            WHERE
                ISNULL(ss_rigs.tag, '') <> ''
            ";
        }
        return q;
    }

    private static Dictionary<int, SpaceshipRig> rigsDict;
    private static void UpdateRigDict(SpaceshipRig rig)
    {
        if (rigsDict == null)
            rigsDict = new Dictionary<int, SpaceshipRig>();
        if (!rigsDict.ContainsKey(rig.Id))
            rigsDict.Add(rig.Id, rig);
    }
    public static SpaceshipRig RigById(int id)
    {
        if (rigsDict == null)
            rigsDict = new Dictionary<int, SpaceshipRig>();
        if (!rigsDict.ContainsKey(id))
        {
            SpaceshipRig newRig = new SpaceshipRig(id);
            rigsDict.Add(newRig.Id, newRig);
        }
            
        return rigsDict[id];

    }
    public static List<SpaceshipRig> BuiltInRigs()
    {
        List<SpaceshipRig> rigs = new List<SpaceshipRig>();
        string q = SpaceshipRigQuery(0, 0, "", true);
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            while(r.Read())
            {
                rigs.Add(new SpaceshipRig(r));
            }
        }
        r.Close();

        return rigs;
        
    }

    public class RigSlot
    {
        public int Id { get; set; }
        public ShipModel.Slot Slot { get; set; }
        public ShipModuleType ModuleType { get; set; }
        public OfficerTeam team { get; set; }
        public RigSlot()
        {
            team = new OfficerTeam();
        }
        public RigSlot(ShipModel.Slot modelSlot)
        {
            team = new OfficerTeam();
            this.Slot = modelSlot;
        }
        public void LoadRig(SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            int moduleTypeId = Convert.ToInt32(r["module_type_id"]);
            this.ModuleType = ShipModuleType.ModuleById(moduleTypeId);
            team = new OfficerTeam(Convert.ToString(r["officer_ids"]));
        }

        /// <summary>
        /// Если в слот помещается подходящий модуль, то возвращается пустая строка.
        /// Если не подходит - то программа возвращается строку - описание ошибки
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string LoadModuleType(ShipModuleType moduleType)
        {
            if (!Slot.SlotForModule)
                return "Slot type doesn't fit module";

            string compareResult = Slot.ModuleFitsSlot(moduleType);
            if(compareResult != "")
                return compareResult;

            this.ModuleType = moduleType;
            return "";
        }
        public string LoadOfficer(CrewOfficer officer)
        {
            if (!Slot.SlotForOfficer)
                return "Slot can't contain officer";
            return team.AddOfficer(officer);
        }

        public void ClearSlot()
        {
            team = new OfficerTeam();
            ModuleType = null;
        }

        public class OfficerTeam
        {
            private const int MaxOfficers = 3;

            public List<CrewOfficer> OfficerList;

            public OfficerTeam() { OfficerList = new List<CrewOfficer>(); }
            public OfficerTeam(string list)
            {
                OfficerList = new List<CrewOfficer>();
                if (String.IsNullOrEmpty(list))
                    return;
                string[] ofString = list.Split(',');
                foreach(string s in ofString)
                {
                    string s2 = s.Trim();
                    if (s2 != "")
                    {
                        if (s2 == "player")
                        {
                            AccountData playerAcc = AssetEditor.FormMain.GetLatestUser();
                            OfficerList.Add(new CrewOfficer(playerAcc));
                        }
                        else
                        {
                            OfficerList.Add(CrewOfficer.OfficerById(Convert.ToInt32(s2)));
                        }
                    }

                }
            }

            public string GetOfficerString()
            {
                if (OfficerList.Count == 0)
                    return "";
                string tStr = "";
                foreach(CrewOfficer of in OfficerList)
                {
                    if (of.Id == 0)
                        of.Save();
                    if (tStr != "")
                        tStr += ",";
                    if(of.IsPlayer)
                    {
                        tStr += "player";
                    }
                    else
                    {
                        tStr += of.Id.ToString();
                    }
                    
                }
                return tStr;
            }
            
            /// <summary>
            /// Возвращает пустую строку если всё ОК или описание ошибки если что-то пошло не так
            /// Что может пойти не так в данном случае? Только что превышено количество офицеров
            /// (может быть не более трех) или попытка добавить офицера, который уже есть в списке
            /// </summary>
            /// <param name="officer"></param>
            /// <returns></returns>
            public string AddOfficer(CrewOfficer officer)
            {
                if (OfficerList.Count >= MaxOfficers)
                    return "Too mane officers in team";

                int id = officer.Id;
                if (id > 0 && OfficerList.Count > 0)
                {
                    foreach(CrewOfficer of in OfficerList)
                    {
                        if (of.Id == id)
                            return "Duplicate officer in team";
                    }
                }

                OfficerList.Add(officer);
                return "";

            }

            public override string ToString()
            {
                StringBuilder s = new StringBuilder();
                s.Append("Team ");
                if (OfficerList.Count == 0)
                {
                    s.Append(" (empty)");
                }
                else if(OfficerList.Count == 1)
                {
                    s.Append(OfficerList[0].ToString());
                }
                else
                {
                    s.Append(" (" + OfficerList.Count + " members)");
                }
                return s.ToString();
            }

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
                module_type_id = {moduleTypeId},
                officer_ids = @str1
            WHERE
                id = {Id}";

            List<string> names = new List<string> { team.GetOfficerString() };

            DataConnection.Execute(q, names);
        }

        public bool IsModule
        {
            get
            {
                List<ShipModuleType.ModuleType> weaponTypes = new List<ShipModuleType.ModuleType>();
                weaponTypes.Add(ShipModuleType.ModuleType.Weapon);
                if(ModuleType == null)
                {
                    return false;
                }
                else
                {
                    if(ModuleType.ModuleTypeFromStr() == ShipModuleType.ModuleType.Weapon)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                
            }
        }
        public bool IsWeapon
        {
            get 
            {
                if (ModuleType == null)
                { 
                    return false;
                }
                else
                {
                    return ModuleType.ModuleTypeFromStr() == ShipModuleType.ModuleType.Weapon;
                }
            }
        }
        public bool IsOfficers
        {
            get
            {
                return team.OfficerList.Count > 0;
            }
        }
        public bool IsEmpty
        {
            get
            {
                if (team.OfficerList.Count > 0)
                {
                    return false;
                }
                else if (ModuleType != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public override string ToString()
        {
            if(team.OfficerList.Count > 0)
            {
                return team.ToString();
            }
            else if(ModuleType != null)
            {
                return ModuleType.ToString();
            }
            else
            {
                return "Empty";
            }
        }

    }

    public void RecalculateParameters()
    {
        Params = new SpaceshipParameters();

        //Загрузка параметров модели корабля
        Params.AddShipModelParameters(sModel);

        //Загрузка параметров просто типов модулей в слотах
        foreach(RigSlot slot in Slots)
        {
            if(!slot.IsEmpty)
            {
                if (slot.IsModule)
                    Params.AddModuleParameters(slot.ModuleType);
                if (slot.IsWeapon)
                    Params.AddWeapon(slot.ModuleType);
            }
        }

        //Корректировка по параметрам офицеров
        foreach(RigSlot slot in Slots)
        {
            if(slot.IsOfficers)
            {
                Params.AddOfficersParameters(slot.team);
            }
        }

    }

    public void SaveData(int playerId, string tag)
    {
        string q;
        List<string> names = new List<string> { tag };
        if (Id == 0)
        {
            q = $@"
                INSERT INTO ss_rigs(ship_model_id, player_id, tag) VALUES({sModel.Id}, {playerId}, @str1)
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q, names);

        }
        else
        {
            q = $@"
                UPDATE ss_rigs SET
                    ship_model_id = {sModel.Id},
                    player_id = {playerId},
                    tag = @str1
                WHERE
                    id = {Id}";
            DataConnection.Execute(q, names);
        }

        string idsDoNotDelete = "";
        if(Slots.Count > 0)
        {
            foreach(RigSlot slot in Slots)
            {
                slot.SaveData(Id);
                if (idsDoNotDelete != "")
                    idsDoNotDelete += ",";
                idsDoNotDelete += slot.Id;
            }
        }

        q = $"DELETE FROM ss_rigs_slots WHERE ss_rig_id = {Id} ";
        if (idsDoNotDelete != "")
            q += " AND id NOT IN (" + idsDoNotDelete + ")";
        DataConnection.Execute(q);

        UpdateRigDict(this);

    }

    public void Delete()
    {
        if (Id == 0)
            return;
        string q = $@"
            DELETE FROM ss_rigs WHERE id = {Id};
            DELETE FROM ss_rigs_slots WHERE ss_rig_id = {Id}";
        DataConnection.Execute(q);
    }

    public override string ToString()
    {
        if(PlayerId > 0)
        {
            return "player " + PlayerId;
        }
        else if(Tag != "")
        {
            return Tag;
        }
        else
        {
            return sModel.Name;
        }
    }

}

