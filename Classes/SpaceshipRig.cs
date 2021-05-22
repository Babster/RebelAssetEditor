using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Crew;
using AdmiralNamespace;
using Newtonsoft.Json;

public class SpaceshipRig : UnitySpaceshipRig
{
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

    public void LoadModules(List<ShipModule> moduleList)
    {
        ShipModule.ModuleList mList = new ShipModule.ModuleList(moduleList);
        foreach(var slot in Slots)
        {
            ShipModelSlot slot2 = slot.Slot;
            if(slot2.sType == ShipModelSlot.SlotType.Weapon)
            {
                var lst = mList.GetModules(ShipModuleType.ModuleType.Weapon);
                if (lst.Count > 0)
                    slot.LoadModule(lst[0]);
            }
            else if(slot2.sType == ShipModelSlot.SlotType.Engine)
            {
                var lst = mList.GetModules(ShipModuleType.ModuleType.Engine);
                if (lst.Count > 0)
                    slot.LoadModule(lst[0]);
            }
            else if(slot2.sType == ShipModelSlot.SlotType.Armor)
            {
                var lst = mList.GetModules(ShipModuleType.ModuleType.Armor);
                if (lst.Count > 0)
                    slot.LoadModule(lst[0]);
            }
            else if(slot2.sType == ShipModelSlot.SlotType.Thrusters)
            {
                var lst = mList.GetModules(ShipModuleType.ModuleType.Thrusters);
                if (lst.Count > 0)
                    slot.LoadModule(lst[0]);
            }
            else if(slot2.sType == ShipModelSlot.SlotType.Misc)
            {
                var lst = mList.GetModules(ShipModuleType.ModuleType.Misc);
                if (lst.Count > 0)
                    slot.LoadModule(lst[0]);
            }
        }
    }

    private void LoadSlots()
    {

        if (sModel == null)
            return;

        //Загрузка слотов - сначала подкачиваются все слоты которые есть в модели корабля,
        //а затем на них накатываются модули, которые установлены в эти слоты согласно 
        Slots = new List<RigSlot>();

        Dictionary<int, RigSlot> slotDict = new Dictionary<int, RigSlot>();

        sModel.ClearSlotDuplicates();

        foreach(ShipModelSlot modelSlot in sModel.slots)
        {
            RigSlot curSlot = new RigSlot(modelSlot);
            Slots.Add(curSlot);
            if(!slotDict.ContainsKey(curSlot.Slot.Id))
            {
                slotDict.Add(curSlot.Slot.Id, curSlot);
            }
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
                crew_id,
                ISNULL(module_id, 0) AS module_id
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
                    slotDict[SlotId].LoadRig(r, PlayerId);
                }
            }
        }

    }

    private void LoadOfficers(List<CrewOfficer> officers)
    {
        foreach(var slot in Slots)
        {
            if(slot.Slot.SlotForOfficer)
            {
                foreach (var officer in officers)
                    slot.LoadOfficer(officer);
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

    
    private static void UpdateRigDict(SpaceshipRig rig)
    {
        if (StaticMembers.rigsDict == null)
            StaticMembers.rigsDict = new Dictionary<int, SpaceshipRig>();
        if (!StaticMembers.rigsDict.ContainsKey(rig.Id))
            StaticMembers.rigsDict.Add(rig.Id, rig);
    }
    public static SpaceshipRig RigById(int id)
    {
        if (StaticMembers.rigsDict == null)
            StaticMembers.rigsDict = new Dictionary<int, SpaceshipRig>();
        if (!StaticMembers.rigsDict.ContainsKey(id))
        {
            SpaceshipRig newRig = new SpaceshipRig(id);
            StaticMembers.rigsDict.Add(newRig.Id, newRig);
        }
            
        return StaticMembers.rigsDict[id];

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

    public void SaveData(int playerId, string tag)
    {
        string q;
        List<string> names = new List<string> { tag };
        int shipId = 0;
        if (Ship != null)
        {
            shipId = Ship.Id;
        }
            
        if (Id == 0)
        {
            q = $@"
                INSERT INTO ss_rigs(ship_model_id, ship_id, player_id, tag) VALUES({sModel.Id}, {shipId}, {playerId}, @str1)
                SELECT @@IDENTITY AS Result";
            Id = DataConnection.GetResultInt(q, names);

        }
        else
        {
            q = $@"
                UPDATE ss_rigs SET
                    ship_model_id = {sModel.Id},
                    ship_id = {shipId},
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

        if (Ship != null)
        {
            Ship.RigId = Id;
            Ship.Save();
        }
            


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

    public static List<SpaceshipRig> PlayerRigs(int playerId)
    {
        List<SpaceshipRig> rigList = new List<SpaceshipRig>();
        string q = SpaceshipRigQuery(0, playerId, "", false);
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            while(r.Read())
            {
                rigList.Add(new SpaceshipRig(r));
            }
        }
        r.Close();
        return rigList;
    }

    public static SpaceshipRig RigForPlayer(int playerId)
    {
        SpaceshipRig tRig = null;
        string q = SpaceshipRigQuery(0, playerId, "", false);
        SqlDataReader r = DataConnection.GetReader(q);
        if(r.HasRows)
        {
            r.Read();
            tRig = new SpaceshipRig(r);
        }
        r.Close();
        if(tRig != null)
            return tRig;


        List<Ship> ships = Ship.PlayerShips(playerId);
        if (ships.Count == 0)
            return null;
        tRig = new SpaceshipRig();
        tRig.LoadShip(ships[0]);

        var moduleList = ShipModule.PlayerModules(playerId);
        tRig.LoadModules(moduleList);

        List<CrewOfficer> officers = CrewOfficer.OfficersForPlayer(playerId, true);
        officers.Add(new CrewOfficer(playerId));
        tRig.LoadOfficers(officers);
        return tRig;
    }

}

public class RigSlot : UnityRigSlot
{

    public RigSlot()
    {
        team = new RigSlotOfficerTeam();
    }
    public RigSlot(ShipModelSlot modelSlot)
    {
        team = new RigSlotOfficerTeam();
        this.Slot = modelSlot;
    }
    public void LoadRig(SqlDataReader r, int playerId)
    {
        this.Id = Convert.ToInt32(r["id"]);
        int moduleTypeId = Convert.ToInt32(r["module_type_id"]);
        this.ModuleType = ShipModuleType.ModuleById(moduleTypeId);
        if (playerId > 0)
        {
            team = new RigSlotOfficerTeam(Convert.ToString(r["officer_ids"]), playerId);
        }
        else
        {
            team = new RigSlotOfficerTeam();
        }
        int moduleId = (int)r["module_id"];
        if (moduleId > 0)
            Module = new ShipModule(moduleId);
    }

    public void SaveData(int rigId)
    {
        string q;
        if (Id == 0)
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

        int moduleId = 0;
        if (Module != null)
        {
            moduleId = Module.Id;
            Module.RigSlotId = Id;
            Module.Save();
        }


        q = $@"
            UPDATE ss_rigs_slots SET 
                slot_id = {slotId},
                module_type_id = {moduleTypeId},
                module_id = {moduleId},
                officer_ids = @str1
            WHERE
                id = {Id}";

        List<string> names = new List<string> { team.GetOfficerString() };

        DataConnection.Execute(q, names);
    }

}

public class RigSlotOfficerTeam : UnityRigSlotOfficerTeam
{
    private const int MaxOfficers = 3;

    public RigSlotOfficerTeam() { OfficerList = new List<CrewOfficer>(); }
    public RigSlotOfficerTeam(string list, int playerId)
    {
        OfficerList = new List<CrewOfficer>();
        if (String.IsNullOrEmpty(list))
            return;
        string[] ofString = list.Split(',');
        foreach (string s in ofString)
        {
            string s2 = s.Trim();
            if (s2 != "")
            {
                if (s2 == "player")
                {
                    //AccountData playerAcc = AssetEditor.FormMain.GetLatestUser();
                    OfficerList.Add(new CrewOfficer(playerId));
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
        foreach (CrewOfficer of in OfficerList)
        {
            if (of.Id == 0)
                of.Save();
            if (tStr != "")
                tStr += ",";
            if (of.IsPlayer)
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



    public override string ToString()
    {
        StringBuilder s = new StringBuilder();
        s.Append("Team ");
        if (OfficerList.Count == 0)
        {
            s.Append(" (empty)");
        }
        else if (OfficerList.Count == 1)
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

public class UnityRigSlot
{
    public int Id { get; set; }
    public ShipModelSlot Slot { get; set; }
    public ShipModuleType ModuleType { get; set; }
    public ShipModule Module { get; set; }
    public RigSlotOfficerTeam team { get; set; }

    public UnityRigSlot() { team = new RigSlotOfficerTeam(); }

    public UnityRigSlot(ShipModelSlot modelSlot)
    {
        team = new RigSlotOfficerTeam();
        team.SlotType = modelSlot.sType;
        this.Slot = modelSlot;
    }

    public bool IsModule
    {
        get
        {
            List<ShipModuleType.ModuleType> weaponTypes = new List<ShipModuleType.ModuleType>();
            weaponTypes.Add(ShipModuleType.ModuleType.Weapon);
            if (ModuleType == null)
            {
                return false;
            }
            else
            {
                if (ModuleType.ModuleTypeFromStr() == ShipModuleType.ModuleType.Weapon)
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
        if (team.OfficerList.Count > 0)
        {
            return team.ToString();
        }
        else if (ModuleType != null)
        {
            return ModuleType.ToString();
        }
        else
        {
            return "Empty";
        }
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
        if (compareResult != "")
            return compareResult;

        this.ModuleType = moduleType;
        return "";
    }

    public void LoadModule(ShipModule module)
    {
        this.Module = module;
        this.ModuleType = module.ModuleType;
        module.Reserve = true;

    }

    public string LoadOfficer(CrewOfficer officer)
    {
        if (!Slot.SlotForOfficer)
            return "Slot can't contain officer";
        return team.AddOfficer(officer);
    }

    public void ClearSlot()
    {
        team = new RigSlotOfficerTeam();
        ModuleType = null;
    }

}

public class UnitySpaceshipRig
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Tag { get; set; }
    public ShipModel sModel { get; set; }
    public Ship Ship { get; set; }
    public List<RigSlot> Slots { get; set; }
    public SpaceshipParameters Params { get; set; }

    public void RecalculateParameters()
    {
        
        Params = new SpaceshipParameters();
        Params.ship = Ship;
        Params.AddShipModelParameters(sModel);

        //Загрузка параметров просто типов модулей в слотах
        foreach (RigSlot slot in Slots)
        {
            if (!slot.IsEmpty)
            {
                if (slot.IsModule)
                    Params.AddModuleParameters(slot.ModuleType);
                if (slot.IsWeapon)
                    Params.AddWeapon(slot.ModuleType);
            }
        }

        //Корректировка по параметрам офицеров
        foreach (RigSlot slot in Slots)
        {
            if (slot.IsOfficers)
            {
                Params.AddOfficersParameters(slot.team);
            }
        }

    }

    private void LoadSlots()
    {
        if (sModel == null)
            return;

        //Загрузка слотов - сначала подкачиваются все слоты которые есть в модели корабля,
        //а затем на них накатываются модули, которые установлены в эти слоты согласно 
        Slots = new List<RigSlot>();

        Dictionary<int, RigSlot> slotDict = new Dictionary<int, RigSlot>();

        sModel.ClearSlotDuplicates();

        foreach (ShipModelSlot modelSlot in sModel.slots)
        {
            RigSlot curSlot = new RigSlot(modelSlot);
            Slots.Add(curSlot);
            if (!slotDict.ContainsKey(curSlot.Slot.Id))
            {
                slotDict.Add(curSlot.Slot.Id, curSlot);
            }
        }
    }

    public void LoadShip(Ship ship)
    {

        this.Ship = ship;
        this.sModel = ship.Model;
        this.PlayerId = ship.PlayerId;
        LoadSlots();

        Params = new SpaceshipParameters();
        Params.ship = this.Ship;

        //Загрузка параметров модели корабля
        Params.AddShipModelParameters(sModel);

    }

    private Dictionary<int, RigSlot> rigSlotDict;
    public RigSlot GetSlotByShipModuleSlotId(int id)
    {
        if (rigSlotDict == null)
        {
            rigSlotDict = new Dictionary<int, RigSlot>();
            foreach (RigSlot slot in Slots)
            {
                rigSlotDict.Add(slot.Slot.Id, slot);
            }
        }

        return rigSlotDict[id];

    }



    public class ShipIsReadyResult
    {
        public string Need { get; set; }
        public bool Ready { get; set; }
    }
    public ShipIsReadyResult ShipIsReady()
    {
        StringBuilder t = new StringBuilder();

        //Проверка экипажа. В зависимости от настроек может понадобится либо полностью весь экипаж
        //либо как минимум один офицер управления
        foreach (var rigSlot in Slots)
        {
            if (rigSlot.IsOfficers)
            {
                if (RigConfiguration.NeedFullCrew)
                {
                    if (rigSlot.team.OfficerList.Count < rigSlot.team.MaxOfficers)
                    {
                        t.AppendLine("- Fill every crew slot");
                        break;
                    }
                }
                else
                {
                    if (rigSlot.Slot.MainCabin == 1) //В главной кабине кто-то должен быть. В остальных - не обязательно
                    {
                        if (rigSlot.team.OfficerList.Count == 0)
                        {
                            t.AppendLine("- Add a ship captain to the main cabin");
                            break;
                        }
                    }
                }
            }
        }

        //Проверка на то, что все необходимые модули установлены. Нужны: силовая установка, ускорители, оружие
        //Второй вариант - когда нужно чтобы были установлены вообще все модули (это когда корабль отправляется по сюжетным миссиям)
        //Возможно, в будущем появится такой вариант, когда должен быть установлен какой-нибудь определенный модуль, но пока два варианта
        bool reactorInstalled = false, thrustersInstalled = false, weaponInstalled = false;
        bool unfilledSlot = false;
        foreach (var slot in Slots)
        {
            if (slot.Module != null)
            {
                if (slot.ModuleType.ModuleTypeFromStr() == ShipModuleType.ModuleType.Engine)
                {
                    reactorInstalled = true;
                }
                else if (slot.ModuleType.ModuleTypeFromStr() == ShipModuleType.ModuleType.Weapon)
                {
                    weaponInstalled = true;
                }
                else if (slot.ModuleType.ModuleTypeFromStr() == ShipModuleType.ModuleType.Thrusters)
                {
                    thrustersInstalled = true;
                }
            }
            else
            {
                if (RigConfiguration.NeedEverySlotFilled)
                {
                    unfilledSlot = true;
                }
            }
        }
        if (RigConfiguration.NeedEverySlotFilled)
        {
            if (unfilledSlot)
            {
                t.AppendLine("- Fill every slot in the ship");
            }
        }
        else
        {
            if (!reactorInstalled)
            {
                t.AppendLine("- Install a reactor");
            }
            if (!thrustersInstalled)
            {
                t.AppendLine("- Install a thrusters");
            }
            if (!weaponInstalled)
            {
                t.AppendLine("- Install a weapon");
            }
        }

        ShipIsReadyResult res = new ShipIsReadyResult();
        res.Need = t.ToString();
        if (string.IsNullOrEmpty(res.Need))
        {
            res.Ready = true;
            res.Need = "The ship is ready to launch";
        }

        return res;

    }

    public static class RigConfiguration
    {
        public static bool NeedEnergyBalance { get; set; }
        public static bool NeedEverySlotFilled { get; set; }
        public static bool NeedFullCrew { get; set; }
    }

}

public class UnityRigSlotOfficerTeam
{

    public ShipModelSlot.SlotType  SlotType { get; set; }

    public int MaxOfficers
    {
        get
        {
            switch(SlotType)
            {
                case UnityShipModelSlot.SlotType.Cabin:
                    return 1;
                case UnityShipModelSlot.SlotType.Cabin3:
                    return 3;
                case UnityShipModelSlot.SlotType.ControlRoom:
                    return 1;
                default:
                    return 0;
            }
        }
    }

    public List<CrewOfficer> OfficerList;

    public UnityRigSlotOfficerTeam()
    {
        OfficerList = new List<CrewOfficer>();
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
            return "Too many officers in team";

        int id = officer.Id;
        if (id > 0 && OfficerList.Count > 0)
        {
            foreach (CrewOfficer of in OfficerList)
            {
                if (of.Id == id)
                    return "Duplicate officer in team";
            }
        }

        OfficerList.Add(officer);
        return "";

    }

}