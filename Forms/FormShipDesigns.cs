using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Crew;
using AdmiralNamespace;

namespace AssetEditor.Forms
{
    public partial class FormShipDesigns : Form
    {

        private bool NoEvents;

        public FormShipDesigns()
        {
            InitializeComponent();
        }

        private void FormShipDesigns_Load(object sender, EventArgs e)
        {
            FillShips();
            FillSaModule();
            FillPlayersOfficers();
        }

        /// <summary>
        /// Ship designs!!!!
        /// </summary>
        #region Ship designs

        private Dictionary<int, string> ModuleDict;

        private Dictionary<int, RadioButton> SlotSizeIntToRadioDict;
        private Dictionary<RadioButton, int> SlotSizeRadioToIntDict;

        private void FillShips()
        {

            treeShips.Nodes.Clear();
            string q = ShipModel.ShipModelQuery();

            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
                while (r.Read())
                {
                    ShipModel tag = new ShipModel(ref r);
                    TreeNode n;
                    if (tag.Parent > 0)
                    {
                        n = nodes[tag.Parent].Nodes.Add(tag.Name);
                    }
                    else
                    {
                        n = treeShips.Nodes.Add(tag.Name);
                    }
                    nodes.Add(tag.Id, n);
                    n.Tag = tag;

                }
            }
            r.Close();

            LoadModuleDict();
            LoadSizeRadioDict();

        }

        private void buttonReloadModuleDict_Click(object sender, EventArgs e)
        {
            LoadModuleDict();
        }

        private void LoadModuleDict()
        {
            ModuleDict = ShipModuleType.moduleNames();
        }

        private void LoadSizeRadioDict()
        {
            SlotSizeIntToRadioDict = new Dictionary<int, RadioButton>();
            SlotSizeIntToRadioDict.Add(1, radioSize1);
            SlotSizeIntToRadioDict.Add(2, radioSize2);
            SlotSizeIntToRadioDict.Add(3, radioSize2);
            SlotSizeRadioToIntDict = new Dictionary<RadioButton, int>();
            SlotSizeRadioToIntDict.Add(radioSize1, 1);
            SlotSizeRadioToIntDict.Add(radioSize2, 2);
            SlotSizeRadioToIntDict.Add(radioSize3, 3);
        }

        private void buttonShipAdd_Click(object sender, EventArgs e)
        {
            int parentId;
            TreeNode n;
            if (treeShips.SelectedNode == null)
            {
                parentId = 0;
                n = treeShips.Nodes.Add("");
            }
            else
            {
                ShipModel tTag = (ShipModel)treeShips.SelectedNode.Tag;
                parentId = tTag.Id;
                n = treeShips.SelectedNode.Nodes.Add("");
            }
            ShipModel tag = new ShipModel(parentId);
            n.Text = tag.Name;
            n.Tag = tag;
            treeShips.SelectedNode = n;
        }

        private void treeShips_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearShip();
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            NoEvents = true;
            textShipId.Text = tag.Id.ToString();
            textShipBaseStructure.Text = tag.BaseStructureHp.ToString();
            textBattleIntensity.Text = tag.BattleIntensity.ToString();
            textShipName.Text = tag.Name;
            textShipUnity.Text = tag.AssetName;
            textShipImageId.Text = tag.ImgId.ToString();
            textShipBaseEnergy.Text = tag.BaseEnergy.ToString();
            if (tag.slots.Count > 0)
            {
                foreach (ShipModelSlot slot in tag.slots)
                {
                    listShipSlots.Items.Add(slot);
                }
            }
            NoEvents = false;
            if (listShipSlots.Items.Count > 0)
                listShipSlots.SelectedIndex = 0;
        }

        private void ClearShip()
        {
            NoEvents = true;
            textShipId.Text = "";
            textShipName.Text = "";
            textShipUnity.Text = "";
            textShipImageId.Text = "";
            listShipSlots.Items.Clear();
            NoEvents = false;
            ClearShipSlot();
        }

        private ShipModel GetCurrentShipTag()
        {
            if (treeShips.SelectedNode == null)
                return null;
            return (ShipModel)treeShips.SelectedNode.Tag;
        }
        private void textShipName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            treeShips.SelectedNode.Text = textShipName.Text;
            tag.Name = textShipName.Text;
        }
        private void textShipUnity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            tag.AssetName = textShipUnity.Text;
        }
        private void textShipBaseEnergy_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            int amount = 0;
            Int32.TryParse(textShipBaseEnergy.Text, out amount);
            tag.BaseEnergy = amount;
        }
        private void textShipBaseStructure_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            int amount = 0;
            Int32.TryParse(textShipBaseStructure.Text, out amount);
            tag.BaseStructureHp = amount;
        }
        private void textBattleIntensity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            int amount = 0;
            Int32.TryParse(textBattleIntensity.Text, out amount);
            tag.BattleIntensity = amount;
        }
        private void textShipImageId_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            int amount = 0;
            Int32.TryParse(textShipImageId.Text, out amount);
            tag.ImgId = amount;
        }
        private void listShipParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            FillShipSlot();
        }
        private void FillShipSlot()
        {
            ClearShipSlot();

            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;

            NoEvents = true;
            textShipSlotId.Text = slot.Id.ToString();
            textShipSlotNumber.Text = slot.SlotNumber.ToString();
            if ((int)slot.SlotType > 0)
                comboShipSlotType.SelectedItem = slot.SlotType.ToString();
            textShipSlotDefaultModule.Text = slot.DefaultModuleId.ToString();
            textShipSlotControl.Text = slot.SlotControl;
            SlotSizeIntToRadioDict[slot.Size].Checked = true;
            checkShipMainCabin.Checked = slot.MainCabin == 1;
            checkShipDoubleWeapon.Checked = slot.DoubleWeapon == 1;
            ShowDefaultSlotName();
            NoEvents = false;

        }
        private void radioSize1_CheckedChanged(object sender, EventArgs e) { SetSlotSize(); }
        private void radioSize2_CheckedChanged(object sender, EventArgs e) { SetSlotSize(); }
        private void radioSize3_CheckedChanged(object sender, EventArgs e) { SetSlotSize(); }
        private void SetSlotSize()
        {
            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            foreach (RadioButton key in SlotSizeRadioToIntDict.Keys)
            {
                if (key.Checked)
                {
                    slot.Size = SlotSizeRadioToIntDict[key];
                    return;
                }


            }
        }
        private void ClearShipSlot()
        {
            NoEvents = true;
            textShipSlotId.Text = "";
            textShipSlotNumber.Text = "";
            comboShipSlotType.SelectedItem = null;
            textShipSlotDefaultModule.Text = "";
            textShipSlotDefaultModuleName.Text = "";
            checkShipMainCabin.Checked = false;
            NoEvents = false;
        }
        private ShipModelSlot GetCurrentShipSlot()
        {
            if (listShipSlots.SelectedItem == null)
                return null;
            return (ShipModelSlot)listShipSlots.SelectedItem;
        }
        private void textShipSlotNumber_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            int amount = 0;
            Int32.TryParse(textShipSlotNumber.Text, out amount);
            slot.SlotNumber = amount;
            listShipSlots.Items[listShipSlots.SelectedIndex] = listShipSlots.SelectedItem;
        }
        private void checkShipMainCabin_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            if (checkShipMainCabin.Checked)
                slot.MainCabin = 1;
            else
                slot.MainCabin = 0;

        }

        private void checkShipDoubleWeapon_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            if (checkShipDoubleWeapon.Checked)
                slot.DoubleWeapon = 1;
            else
                slot.DoubleWeapon = 0;
        }

        private void textShipSlotControl_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            slot.SlotControl = textShipSlotControl.Text;
        }


        private void comboShipSlotType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            ShipModelSlot.SlotTypes t = UnityShipModelSlot.SlotTypes.None;
            if (comboShipSlotType.SelectedItem == null)
            {
                t = UnityShipModelSlot.SlotTypes.None;
            }
            else
            {
                List<ShipModelSlot.SlotTypes> tList = Enum.GetValues(typeof(ShipModelSlot.SlotTypes)).Cast<ShipModelSlot.SlotTypes>().ToList();
                foreach (var element in tList)
                {
                    if (element.ToString() == (string)comboShipSlotType.SelectedItem)
                    {
                        t = element;
                        break;
                    }
                }

            }
            slot.SlotType = t;
            listShipSlots.Items[listShipSlots.SelectedIndex] = listShipSlots.SelectedItem;
        }
        private void textShipSlotDefaultModule_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            int moduleId;
            Int32.TryParse(textShipSlotDefaultModule.Text, out moduleId);
            slot.DefaultModuleId = moduleId;
            ShowDefaultSlotName();
        }
        private void ShowDefaultSlotName()
        {
            int moduleId;
            Int32.TryParse(textShipSlotDefaultModule.Text, out moduleId);
            if (moduleId > 0)
            {
                if (ModuleDict.ContainsKey(moduleId))
                {
                    textShipSlotDefaultModuleName.Text = ModuleDict[moduleId];
                }
                else
                {
                    textShipSlotDefaultModuleName.Text = "";
                }

            }
            else
            {
                textShipSlotDefaultModuleName.Text = "";
            }
        }
        private void buttonAddShipPart_Click(object sender, EventArgs e)
        {
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            ShipModelSlot slot = tag.AddSlot();
            listShipSlots.Items.Add(slot);
            listShipSlots.SelectedItem = slot;
        }
        private void buttonRemoveShipPart_Click(object sender, EventArgs e)
        {
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            ShipModelSlot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            tag.DeleteSlot(ref slot);
            listShipSlots.Items.Remove(slot);
            ClearShipSlot();
        }
        private void buttonSaveShip_Click(object sender, EventArgs e)
        {
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            tag.SaveData();
            textShipId.Text = tag.Id.ToString();
            FillShipSlot();
        }

        #endregion

        #region Ship assemble


        private SpaceshipRig saRig;

        private void buttonCreateRig_Click(object sender, EventArgs e)
        {
            ShipModel curModel = GetCurrentModel();
            if (curModel == null)
                return;

            saRig = new SpaceshipRig();
            saRig.LoadShipModel(curModel);
            FillRig();
        }

        private void FillRig()
        {
            ClearRig();
            foreach (RigSlot rigSlot in saRig.Slots)
            {
                DataGridViewRow row;
                gridSaSlots.Rows.Add();
                row = gridSaSlots.Rows[gridSaSlots.Rows.Count - 1];
                row.Cells["sas_object"].Value = rigSlot;
                row.Cells["sas_name"].Value = rigSlot.Slot.SlotType.ToString();
                row.Cells["sas_content"].Value = rigSlot;
            }

            //Module types grid
            if (saRig.Ship == null)
            {
                //Сюда попадаем если нужно заполнить список модулей их типами
                List<ShipModuleType> moduleTypes = ShipModuleType.CreateList(true);
                if (moduleTypes.Count > 0)
                {
                    foreach (ShipModuleType moduleType in moduleTypes)
                    {
                        DataGridViewRow row;
                        gridSaModules.Rows.Add();
                        row = gridSaModules.Rows[gridSaModules.Rows.Count - 1];
                        row.Cells["sam_module"].Value = moduleType;
                        row.Cells["sam_type"].Value = moduleType.ModuleType.ToString();
                        row.Cells["sam_energy_needed"].Value = moduleType.EnergyNeed;
                    }
                }
            }
            if (saRig.Ship != null)
            {
                //Сюда попадаем если нужно заполнить таблицу теми модулями, которые есть у игрока
                List<ShipModule> modules = ShipModule.PlayerModules(saRig.PlayerId);
                if (modules.Count > 0)
                {
                    foreach (var module in modules)
                    {
                        DataGridViewRow row;
                        gridSaModules.Rows.Add();
                        row = gridSaModules.Rows[gridSaModules.Rows.Count - 1];
                        row.Cells["sam_module"].Value = module;
                        row.Cells["sam_type"].Value = module.ModuleType.ModuleType.ToString();
                        row.Cells["sam_energy_needed"].Value = module.ModuleType.EnergyNeed;
                    }
                }
            }


        }
        private void comboSaShip_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FillSaModule()
        {


            //Spaceship models combo
            comboSaShip.Items.Clear();
            List<ShipModel> models = ShipModel.GetModelList();
            foreach (ShipModel model in models)
            {
                comboSaShip.Items.Add(model);
            }

            //Officer type combo
            comboSaOfficers.Items.Clear();
            List<CrewOfficerType> tOff = CrewOfficerType.GetTypeList();
            foreach (CrewOfficerType curType in tOff)
            {
                comboSaOfficers.Items.Add(curType);
            }
        }

        private ShipModel GetCurrentModel()
        {
            if (comboSaShip.SelectedItem == null)
            {
                return null;
            }
            else
            {
                return (ShipModel)comboSaShip.SelectedItem;
            }
        }

        private void ClearRig()
        {
            gridSaSlots.Rows.Clear();
            textSaBottomLine.Text = "";
            gridSaModules.Rows.Clear();
        }

        private void gridSaModules_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            DataGridViewRow row = gridSaModules.Rows[e.RowIndex];
            PlaceModuleToShipSlot(row);
        }
        private void buttonSaMountModule_Click(object sender, EventArgs e)
        {
            if (gridSaModules.SelectedCells.Count == 0)
                return;
            DataGridViewRow row = gridSaModules.Rows[gridSaModules.SelectedCells[0].RowIndex];
            PlaceModuleToShipSlot(row);
        }
        private void PlaceModuleToShipSlot(DataGridViewRow moduleRow)
        {

            if (gridSaSlots.SelectedCells.Count == 0)
                return;
            DataGridViewRow slotRow = gridSaSlots.Rows[gridSaSlots.SelectedCells[0].RowIndex];

            ShipModuleType moduleType = null;
            ShipModule module = null;
            if (moduleRow.Cells["sam_module"].Value.GetType() == typeof(ShipModuleType))
            {
                moduleType = (ShipModuleType)moduleRow.Cells["sam_module"].Value;
            }
            else if (moduleRow.Cells["sam_module"].Value.GetType() == typeof(ShipModule))
            {
                module = (ShipModule)moduleRow.Cells["sam_module"].Value;
                moduleType = module.ModuleType;
            }


            RigSlot slot;
            slot = (RigSlot)slotRow.Cells["sas_object"].Value;
            string fitMsg = slot.Slot.ModuleFitsSlot(moduleType);

            if (fitMsg != "")
            {
                MessageBox.Show(fitMsg);
                return;
            }

            if (module == null)
            {
                slot.LoadModuleType(moduleType);
            }
            else
            {
                slot.LoadModule(module);
            }
            slotRow.Cells["sas_content"].Value = null;
            slotRow.Cells["sas_content"].Value = slot;

            SpaceShipParameters Params = new SpaceShipParameters();
            Params.rig = saRig;
            Params.CalculateParameters();
            textSaBottomLine.Text = Params.BottomLineString();

        }
        private void listSaOfficers_DoubleClick(object sender, EventArgs e)
        {
            if (gridSaSlots.SelectedCells.Count == 0)
                return;
            DataGridViewRow slotRow = gridSaSlots.Rows[gridSaSlots.SelectedCells[0].RowIndex];

            CrewOfficer officer = (CrewOfficer)listSaOfficers.SelectedItem;

            RigSlot slot;
            slot = (RigSlot)slotRow.Cells["sas_object"].Value;

            string fitMsg = slot.LoadOfficer(officer);
            if (fitMsg != "")
            {
                MessageBox.Show(fitMsg);
                return;
            }

            slotRow.Cells["sas_content"].Value = null;
            slotRow.Cells["sas_content"].Value = slot;

            SpaceShipParameters Params = new SpaceShipParameters();
            Params.rig = saRig;
            Params.CalculateParameters();
            textSaBottomLine.Text = Params.BottomLineString();

        }
        private void gridSaSlots_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            string columnName = gridSaSlots.Columns[e.ColumnIndex].Name;
            DataGridViewRow slotRow = gridSaSlots.Rows[e.RowIndex];
            if (columnName == "sas_content")
            {
                RigSlot slot;
                slot = (RigSlot)slotRow.Cells["sas_object"].Value;
                slot.ClearSlot();
                slotRow.Cells["sas_content"].Value = null;
                slotRow.Cells["sas_content"].Value = slot;
            }

            SpaceShipParameters Params = new SpaceShipParameters();
            Params.rig = saRig;
            Params.CalculateParameters();
            textSaBottomLine.Text = Params.BottomLineString();

        }
        private void buttonSaCreateOfficer_Click(object sender, EventArgs e)
        {
            if (comboSaOfficers.SelectedItem == null)
                return;
            CrewOfficerType ofType = (CrewOfficerType)comboSaOfficers.SelectedItem;
            int playerId = DataConnection.GetResultInt("SELECT MAX(id) AS Result FROM [admirals]");
            CrewOfficer newOfficer = new CrewOfficer(ofType, playerId);
            listSaOfficers.Items.Add(newOfficer);
        }
        private void buttonSaDeleteOfficer_Click(object sender, EventArgs e)
        {
            if (listSaOfficers.SelectedItem == null)
                return;
            CrewOfficer ofType = (CrewOfficer)listSaOfficers.SelectedItem;
            ofType.Delete();
            listSaOfficers.Items.Remove(ofType);

        }
        private void listSaOfficers_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridSaOfficer.Rows.Clear();
            if (listSaOfficers.SelectedItem == null)
                return;
            CrewOfficer ofType = (CrewOfficer)listSaOfficers.SelectedItem;

            foreach (CrewOfficerStat stat in ofType.Stats)
            {
                DataGridViewRow row;
                gridSaOfficer.Rows.Add();
                row = gridSaOfficer.Rows[gridSaOfficer.Rows.Count - 1];
                row.Cells["sao_stat_name"].Value = stat;
                row.Cells["sao_value"].Value = stat.Value;
            }

        }

        private void FillPlayersOfficers()
        {
            AccountData playerAcc = new AccountData(DataConnection.GetResultInt("SELECT MAX(id) AS Result FROM [admirals]"));

            //Player's officers. 
            listSaOfficers.Items.Clear();
            List<CrewOfficer> officers = CrewOfficer.OfficersForPlayer(playerAcc.Id);
            if (officers.Count > 0)
            {
                foreach (CrewOfficer off in officers)
                {
                    listSaOfficers.Items.Add(off);
                }
            }
        }

        private void buttonSaSave_Click(object sender, EventArgs e)
        {

            if (saRig.Ship != null)
            {
                MessageBox.Show("Нельзя сохранять риг, который сделан на основе экземпляра корабля");
                return;
            }

            int playerId = 0;
            string tg = "";
            if (checkSaForPlayer.Checked)
            {
                playerId = DataConnection.GetResultInt("SELECT MAX(id) AS Result FROM [admirals]");
            }
            else
            {
                if (textSaRigTag.Text == "")
                {
                    MessageBox.Show("Не выбран тег и не помечено что это для игрока");
                    return;
                }
                tg = textSaRigTag.Text;
            }

            saRig.SaveData(playerId, tg);
        }


        private void tabPage18_Enter(object sender, EventArgs e)
        {
            FillRigTree();
        }

        private void FillRigTree()
        {
            treeSaRigs.Nodes.Clear();
            string rigQuery;
            rigQuery = SpaceshipRig.SpaceshipRigQuery(0, 0, "", false);
            SqlDataReader r = DataConnection.GetReader(rigQuery);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    SpaceshipRig rig = new SpaceshipRig(r);
                    TreeNode n = treeSaRigs.Nodes.Add(rig.ToString());
                    n.Tag = rig;
                }
            }
            r.Close();
        }

        private void buttonSaLoadRig_Click(object sender, EventArgs e)
        {
            TreeNode n = treeSaRigs.SelectedNode;
            if (n == null)
                return;
            SpaceshipRig rig = (SpaceshipRig)n.Tag;
            saRig = rig;
            textSaRigTag.Text = rig.Tag;
            checkSaForPlayer.Checked = saRig.PlayerId > 0;
            FillRig();
            SpaceShipParameters Params = new SpaceShipParameters();
            Params.rig = saRig;
            Params.CalculateParameters();
            textSaBottomLine.Text = Params.BottomLineString();
        }

        private void buttonSaDeleteRig_Click(object sender, EventArgs e)
        {
            TreeNode n = treeSaRigs.SelectedNode;
            if (n == null)
                return;
            SpaceshipRig rig = (SpaceshipRig)n.Tag;

            if (MessageBox.Show(null, "Delete rig?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            rig.Delete();
            treeSaRigs.Nodes.Remove(n);

        }

        private void buttonLoadPlayerShip_Click(object sender, EventArgs e)
        {

            if (treePlayerShipsRig.SelectedNode == null)
                return;
            Ship ship = (Ship)treePlayerShipsRig.SelectedNode.Tag;
            ship.Model.ClearSlotDuplicates();

            saRig = new SpaceshipRig();
            saRig.LoadShip(ship);
            FillRig();
            SpaceShipParameters Params = new SpaceShipParameters();
            Params.rig = saRig;
            Params.CalculateParameters();
            textSaBottomLine.Text = Params.BottomLineString();
        }

        //Вход на закладку кораблей игрока
        private void tabPage29_Enter(object sender, EventArgs e)
        {

            treePlayerShipsRig.Nodes.Clear();
            List<Ship> ships = Ship.PlayerShips(DataConnection.GetResultInt("SELECT MAX(id) AS Result FROM [admirals]"));
            foreach (Ship ship in ships)
            {
                TreeNode n = treePlayerShipsRig.Nodes.Add(ship.ToString());
                n.Tag = ship;
            }
        }

        #endregion

    }
}
