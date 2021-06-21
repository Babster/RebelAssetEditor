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
using System.Diagnostics.Tracing;
using System.Linq.Expressions;
using System.IO;
using System.Reflection.Emit;
using Story;
using Crew;
using AdmiralNamespace;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AssetEditor
{
    public partial class FormMain : Form
    {
        private bool NoEvents;

        public FormMain()
        {
            NoEvents = false;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoadScenes();

            FillImages();

        }

        #region Starting buttons

        
        private void buttonOpenSkills_Click(object sender, EventArgs e)
        {
            AssetEditor.Forms.FormSkills f = new AssetEditor.Forms.FormSkills();
            f.Show();

        }

        private void buttonOpenSpaceshipsModels_Click(object sender, EventArgs e)
        {

            AssetEditor.Forms.FormShipDesigns f = new Forms.FormShipDesigns();
            f.Show();

        }

        #endregion

        #region "Сцены"

        TreeNode nodeScenes;

        private void LoadScenes()
        {
            nodeScenes = treeScenes.Nodes.Add("Сцены");

            var sceneList = RebelSceneWithSql.GetSceneList();
            foreach(var scene in sceneList)
            {
                TreeNode nNode = nodeScenes.Nodes.Add(scene.Name);
                nNode.Tag = scene;
            }

        }

        private void treeScenes_AfterSelect(object sender, TreeViewEventArgs e)
        {

            ClearScene();

            if (e.Node == null)
            {
                return;
            }

            if(e.Node.Tag == null)
            {
                return;
            }

            RebelSceneWithSql curScene = (RebelSceneWithSql)e.Node.Tag;
            NoEvents = true;
            textSceneId.Text = curScene.Id.ToString();
            textSceneName.Text = curScene.Name;
            textSceneBackgroundId.Text = curScene.BackgroundImageId.ToString();
            groupBox1.Enabled = true;
            foreach (SceneElement curSceneElement in curScene.Elements)
            {
                listSceneElements.Items.Add(curSceneElement);
            }

            NoEvents = false;

        }

        private void ClearScene()
        {
            NoEvents = true;
            textSceneId.Text = "";
            textSceneName.Text = "";
            textSceneBackgroundId.Text = "";
            listSceneElements.Items.Clear();
            groupBox1.Enabled = false;
            NoEvents = false;
            ClearSceneElement();

        }

        private void ClearSceneElement()
        {

            NoEvents = true;

            textSceneEnglish.Text = "";
            comboSceneElementType.SelectedItem = null;
            comboSceneElementType.Enabled = false;
            textSceneElementImageId.Text = "";
            textSceneElementImageId.ReadOnly = true;
            textSceneRussian.Text = "";
            textSceneRussian.ReadOnly = true;
            textSceneEnglish.Text = "";
            textSceneEnglish.ReadOnly = true;
            checkNextScreen.Checked = false;
            checkNextScreen.Enabled = false;
            NoEvents = false;
        }


        private void buttonAddNode_Click(object sender, EventArgs e)
        {
            RebelSceneWithSql newScene = new RebelSceneWithSql();

            TreeNode nNode = nodeScenes.Nodes.Add(newScene.Name);
            nNode.Tag = newScene;
            treeScenes.SelectedNode = nNode;
        }

        private RebelSceneWithSql GetCurrentScene()
        {
            if(treeScenes.SelectedNode == null)
            {
                return null;
            }
            if(treeScenes.SelectedNode.Tag == null)
            {
                return null;
            }
            return (RebelSceneWithSql)treeScenes.SelectedNode.Tag;
        }

        private void buttonSaveScene_Click(object sender, EventArgs e)
        {
            RebelSceneWithSql curScene = GetCurrentScene();
            if(curScene == null)
            {
                MessageBox.Show("Не выбрана сцена для сохранения");
                return;
            }
            curScene.SaveData();

        }

        private void textSceneName_TextChanged(object sender, EventArgs e)
        {
            RebelSceneWithSql curScene = GetCurrentScene();
            if (curScene == null)
            {
                return;
            }

            curScene.Name = textSceneName.Text;
            treeScenes.SelectedNode.Text = curScene.Name;

        }


        private void textSceneBackgroundId_TextChanged(object sender, EventArgs e)
        {
            RebelSceneWithSql curScene = GetCurrentScene();
            if (curScene == null)
            {
                return;
            }
            int tId = 0;
            if (Int32.TryParse(textSceneBackgroundId.Text, out tId) == true)
                curScene.BackgroundImageId = tId;
        }

        private void buttonAddSceneElement_Click(object sender, EventArgs e)
        {
            RebelSceneWithSql curScene = GetCurrentScene();
            if (curScene == null)
            {
                return;
            }
            listSceneElements.Items.Add(curScene.AddElement());
            listSceneElements.SelectedIndex = listSceneElements.Items.Count - 1;
        }

        private void listSceneElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ClearSceneElement();

            if (listSceneElements.SelectedItem == null)
                return;

            SceneElement curElement = (SceneElement)listSceneElements.SelectedItem;

            NoEvents = true;
            comboSceneElementType.SelectedItem = curElement.Type;
            comboSceneElementType.Enabled = true;
            textSceneElementImageId.Text = curElement.ImageId.ToString();
            textSceneElementImageId.ReadOnly = false;
            textSceneRussian.Text = curElement.TextRussian;
            textSceneRussian.ReadOnly = false;
            textSceneEnglish.Text = curElement.TextEnglish;
            textSceneEnglish.ReadOnly = false;
            checkNextScreen.Enabled = true;
            checkNextScreen.Checked = curElement.NextScreen;
            NoEvents = false;

        }

        private void comboSceneElementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            SceneElement curElement = (SceneElement)listSceneElements.SelectedItem;
            curElement.Type = Convert.ToString(comboSceneElementType.SelectedItem);
            NoEvents = true;
            listSceneElements.Items[listSceneElements.SelectedIndex] = curElement;
            NoEvents = false;
        }

        private void textSceneElementImageId_TextChanged(object sender, EventArgs e)
        {

            if (NoEvents)
                return;

            SceneElement curElement = (SceneElement)listSceneElements.SelectedItem;

            int imgId = 0;
            Int32.TryParse(textSceneElementImageId.Text, out imgId);
            curElement.ImageId = imgId;

        }

        private void textSceneRussian_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            SceneElement curElement = (SceneElement)listSceneElements.SelectedItem;
            curElement.TextRussian = textSceneRussian.Text;
            NoEvents = true;
            listSceneElements.Items[listSceneElements.SelectedIndex] = curElement;
            NoEvents = false;
        }

        private void textSceneEnglish_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            SceneElement curElement = (SceneElement)listSceneElements.SelectedItem;
            curElement.TextEnglish = textSceneEnglish.Text;

        }

        private void checkNextScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            SceneElement curElement = (SceneElement)listSceneElements.SelectedItem;
            curElement.NextScreen = checkNextScreen.Checked;
        }

        #endregion

        #region "Изображения"

        private class ImageTag : RebelImageWithSql
        {
            public TreeNode Node { get; set; }

            public bool IsPartition { get; set; }

            public ImageTag()
            {

            }

            public ImageTag(ref SqlDataReader r) : base(ref r)
            {

            }

        }

        private void FillImages()
        {

            TreeNode ImagesMainNode = treeImages.Nodes.Add("Изображения");
            ImagesMainNode.Expand();

            string q;
            q = "SELECT DISTINCT partition FROM images ORDER BY partition";
            SqlDataReader r;
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    TreeNode tNode = ImagesMainNode.Nodes.Add(Convert.ToString(r["partition"]));
                    ImageTag tTag = new ImageTag();
                    tTag.IsPartition = true;
                    tTag.Name = tNode.Text;
                    tNode.Tag = tTag;
                    tTag.Node = tNode;

                    FillImagePartition(tNode);

                }
            }
            r.Close();

        }

        private void FillImagePartition(TreeNode partitionNode)
        {

            string q;
            q = "SELECT id, partition, name, file_inner FROM images WHERE partition = @str1";
            List<string> names = new List<string>();
            names.Add(partitionNode.Text);
            SqlDataReader r = DataConnection.GetReader(q, names);
            while (r.Read())
            {
                ImageTag tTag = new ImageTag(ref r);
                TreeNode tNode = partitionNode.Nodes.Add(tTag.Name);
                tNode.Tag = tTag;
                tTag.Node = tNode;
            }
        }

        private void ClearImages()
        {
            NoEvents = true;
            textImagePartition.Text = "";
            textImagePartition.ReadOnly = true;
            textImageName.Text = "";
            textImageName.ReadOnly = true;
            pictureBox1.Image = null;
            buttonImageLoad.Enabled = false;
            textImageId.Text = "";
            NoEvents = false;
        }

        private void buttonAddPartition_Click(object sender, EventArgs e)
        {

            string addType = "";
            if (treeImages.SelectedNode == null)
            {
                addType = "partition";
            }

            if (treeImages.SelectedNode != null)
            {
                if (treeImages.SelectedNode.Tag == null)
                {
                    addType = "partition";
                }
                else
                {
                    addType = "image";
                }
            }

            if (addType == "partition")
            {

                TreeNode tNode = treeImages.Nodes[0].Nodes.Add("Новый раздел");
                ImageTag tTag = new ImageTag();
                tTag.IsPartition = true;
                tTag.Name = tNode.Text;
                tNode.Tag = tTag;
                tTag.Node = tNode;
                treeImages.SelectedNode = tNode;
            }

            if (addType == "image")
            {
                TreeNode tNode;
                ImageTag cTag = (ImageTag)treeImages.SelectedNode.Tag;
                if (cTag.IsPartition)
                {
                    tNode = treeImages.SelectedNode.Nodes.Add("Новое изображение");
                }
                else
                {
                    tNode = treeImages.SelectedNode.Parent.Nodes.Add("Новое изображение");
                }

                ImageTag tTag = new ImageTag();
                tTag.IsPartition = false;
                tTag.Name = tNode.Text;
                tNode.Tag = tTag;
                tTag.Node = tNode;
                tTag.Partition = tNode.Parent.Text;
                treeImages.SelectedNode = tNode;
            }

        }

        private void treeImages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearImages();

            if (e.Node == null)
                return;

            if (e.Node.Tag == null)
                return;

            ImageTag tTag = (ImageTag)e.Node.Tag;

            if (tTag.IsPartition)
            {
                textImagePartition.Text = tTag.Name;
                textImagePartition.ReadOnly = false;
            }
            else
            {
                NoEvents = true;
                textImagePartition.Text = tTag.Node.Parent.Text;
                textImageName.Text = tTag.Name;
                textImageName.ReadOnly = false;
                buttonImageLoad.Enabled = true;
                pictureBox1.Image = tTag.Img;
                textImageId.Text = tTag.Id.ToString();
                NoEvents = false;
            }
        }

        private void textImagePartition_TextChanged(object sender, EventArgs e)
        {

        }
        private void textImagePartition_KeyUp(object sender, KeyEventArgs e)
        {

            if (textImagePartition.ReadOnly == true)
                return;

            if (e.KeyData == Keys.Enter)
            {
                treeImages.SelectedNode.Text = textImagePartition.Text;
                ImageTag tTag = (ImageTag)treeImages.SelectedNode.Tag;
                tTag.Name = textImagePartition.Text;

                if (treeImages.SelectedNode.Nodes.Count > 0)
                {
                    string ids = "";
                    foreach (TreeNode curNode in treeImages.SelectedNode.Nodes)
                    {
                        ImageTag curTag = (ImageTag)curNode.Tag;
                        if (ids != "")
                            ids = ids + ",";
                        ids = ids + curTag.Id.ToString();

                        curTag.Partition = textImagePartition.Text;

                    }

                    string q;
                    q = "UPDATE images SET partition = @str1 WHERE id IN(" + ids + ")";
                    List<string> names = new List<string>();
                    names.Add(textImagePartition.Text);
                    DataConnection.Execute(q, names);

                }

            }
        }

        private void textImageName_TextChanged(object sender, EventArgs e)
        {

        }
        private void textImageName_KeyUp(object sender, KeyEventArgs e)
        {

            if (textImageName.ReadOnly == true)
                return;

            if (e.KeyData == Keys.Enter)
            {
                ImageTag tTag = (ImageTag)treeImages.SelectedNode.Tag;
                tTag.Name = textImageName.Text;
                treeImages.SelectedNode.Text = textImageName.Text;
                tTag.Savedata();
                textImageId.Text = tTag.Id.ToString();
            }


        }

        private void buttonImageLoad_Click(object sender, EventArgs e)
        {

            ImageTag tTag = (ImageTag)treeImages.SelectedNode.Tag;

            openFileDialog1.Filter = "*.jpg|*.jpg|*.png|*.png";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            byte[] fileInner;
            fileInner = System.IO.File.ReadAllBytes(openFileDialog1.FileName);

            tTag.SetImage(ref fileInner);
            pictureBox1.Image = tTag.Img;

            tTag.Savedata();

            textImageId.Text = tTag.Id.ToString();

        }

        private void buttonImageSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Switched off");

            if (treeImages.SelectedNode == null)
                return;

            if (treeImages.SelectedNode.Tag == null)
                return;

            ImageTag tTag = (ImageTag)treeImages.SelectedNode.Tag;

            if (tTag.IsPartition)
            {
                MessageBox.Show("Разделы сохранять нельзя");
                return;
            }
            else
            {
                
                /*NoEvents = true;
                textImagePartition.Text = tTag.Node.Parent.Text;
                textImageName.Text = tTag.Name;
                textImageName.ReadOnly = false;
                buttonImageLoad.Enabled = true;
                pictureBox1.Image = tTag.Img;
                textImageId.Text = tTag.Id.ToString();
                NoEvents = false;*/
            }
        }

        #endregion

        #region Статы адмиралов

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private bool StatsFilled = false;

        private void tabPage5_Enter(object sender, EventArgs e)
        {

            if (StatsFilled)
                return;

            List<OfficerStatTypeSql> stats = OfficerStatTypeSql.GetStatTypeList();
            
            foreach(var stat in stats)
            {
                TreeNode tNode = treeStats.Nodes.Add(stat.Name);
                tNode.Tag = stat;
            }

            NoEvents = true;

            textStatRegistrationPoints.Text = CommonFunctions.GetCommonValue("start_stat_points").IntValue.ToString();

            comboStatBaseType.Items.Clear();
            List<Crew.OfficerStatType.StatType> tList2 = Enum.GetValues(typeof(Crew.OfficerStatType.StatType)).Cast<Crew.OfficerStatType.StatType>().ToList();
            foreach(var item in tList2)
            {
                comboStatBaseType.Items.Add(item.ToString());
            }

            NoEvents = false;

            

            StatsFilled = true;

        }

        private void ClearStat()
        {
            NoEvents = true;
            textStatId.Text = "";
            textStatBaseValue.Text = "";
            textStatName.Text = "";
            textStatDescriptionEnglish.Text = "";
            textStatDescriptionRussian.Text = "";
            textStatSortIdx.Text = "";
            comboSkillGroup.SelectedItem = null;
            NoEvents = false;
        }

        private OfficerStatTypeSql GetCurrentStatType()
        {
            if (treeStats.SelectedNode == null)
                return null;
            if (treeStats.SelectedNode.Tag == null)
                return null;
            return (OfficerStatTypeSql)treeStats.SelectedNode.Tag;
        }


        private void treeStats_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearStat();
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }

            NoEvents = true;

            
            textStatId.Text = tNode.Id.ToString();
            if (!string.IsNullOrEmpty(tNode.SkillGroup))
            {
                comboSkillGroup.SelectedItem = tNode.SkillGroup;
            }

            textStatName.Text = tNode.Name;
            textStatBaseValue.Text = tNode.BaseValue.ToString();
            textStatDescriptionEnglish.Text = tNode.DescriptionEnglish;
            textStatDescriptionRussian.Text = tNode.DescriptionRussian;
            textStatSortIdx.Text = tNode.OrderIdx.ToString();
            foreach(var item in comboStatBaseType.Items)
            {
                if((string)item == tNode.statType.ToString())
                {
                    comboStatBaseType.SelectedItem = item;
                    break;
                }
            }
            NoEvents = false;

        }

        private void buttonCreateStat_Click(object sender, EventArgs e)
        {
            TreeNode tNode = treeStats.Nodes.Add("new stat");
            OfficerStatType tTag = new OfficerStatType();
            tNode.Tag = tTag;
            tTag.Name = "new stat";
            treeStats.SelectedNode = tNode;
        }

        private void textStatName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            treeStats.SelectedNode.Text = textStatName.Text;
            tNode.Name = textStatName.Text;
        }

        private void comboSkillGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            if (comboSkillGroup.SelectedItem == null)
            {
                tNode.SkillGroup = "";
            }
            else
            {
                tNode.SkillGroup = (string)comboSkillGroup.SelectedItem;
            }


        }

        private void textStatBaseValue_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            try
            {
                tNode.BaseValue = Convert.ToInt32(textStatBaseValue.Text);
            }
            catch
            {
                tNode.BaseValue = 0;
            }
        }

        private void textStatSortIdx_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            try
            {
                tNode.OrderIdx = Convert.ToInt32(textStatSortIdx.Text);
            }
            catch
            {
                tNode.OrderIdx = 0;
            }
        }

        private void textStatDescriptionEnglish_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            tNode.DescriptionEnglish = textStatDescriptionEnglish.Text;
        }

        private void textStatDescriptionRussian_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            tNode.DescriptionRussian = textStatDescriptionRussian.Text;
        }
        private void comboStatBaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatType tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            if(comboStatBaseType.SelectedItem == null)
            {
                tNode.statType = OfficerStatType.StatType.None;
            }
            else
            {
                List<Crew.OfficerStatType.StatType> tList2 = Enum.GetValues(typeof(Crew.OfficerStatType.StatType)).Cast<Crew.OfficerStatType.StatType>().ToList();
                foreach (var item in tList2)
                {
                    if (item.ToString() == (string)comboStatBaseType.SelectedItem)
                    {
                        tNode.statType = item;
                        break;
                    }
                }
            }

        }

        private void buttonSaveStat_Click(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            OfficerStatTypeSql tNode = GetCurrentStatType();
            if (tNode == null)
            {
                return;
            }
            tNode.SaveData();
            textStatId.Text = tNode.Id.ToString();
        }

        private void textStatRegistrationPoints_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            int NewValue = 0;
            Int32.TryParse(textStatRegistrationPoints.Text, out NewValue);
            string q;
            StringAndInt newValue = new StringAndInt();
            newValue.StrValue = "";
            newValue.IntValue = NewValue;
            CommonFunctions.SetCommonvalue("start_stat_points", newValue);
        }

        #endregion

        #region Tests

        private bool playersFilled;

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }
        private void tabPage6_Enter(object sender, EventArgs e)
        {
            if (playersFilled)
                return;
            //FillPlayers();
        }

        private void buttonTestUpdatePlayerList_Click(object sender, EventArgs e)
        {
            //FillPlayers();
        }

        /*private void FillPlayers()
        {
            
            treePlayers.Nodes.Clear();

            List<AccountData> list = AccountData.GetAccounts();

            string q = $@"
                SELECT
                    id
                FROM
                    admirals";
            SqlDataReader r = DataConnection.GetReader(q);
            if(list.Count > 0)
            {
                foreach(AccountData curData in list)
                {
                    TreeNode node = treePlayers.Nodes.Add(curData.Name);
                    node.Tag = curData;
                }
            }
            playersFilled = true;
        }
        */
        private void buttonDeleteAccount_Click(object sender, EventArgs e)
        {
            /*
            if (treePlayers.SelectedNode == null)
                return;
            AccountData tData = (AccountData)treePlayers.SelectedNode.Tag;
            AdmiralsMain.DeleteAccount(ref tData);
            MessageBox.Show("Аккаунт " + tData.Name + "(" + tData.Id.ToString() + ") удален");
            treePlayers.Nodes.Remove(treePlayers.SelectedNode);
            */
        }
        private void buttonRegisterAccount_Click(object sender, EventArgs e)
        {
            /*
            AccountData tUser = new AccountData();
            tUser.SteamAccountId = "Babster";
            AdmiralsMain.RegisterAccount(ref tUser);
            MessageBox.Show("Аккаунт " + tUser.Name + "(" + tUser.Id.ToString() + ") зарегистрирован");
            */
        }
        private void buttonTestPlayerStats_Click(object sender, EventArgs e)
        {
            /*
            MessageBox.Show("Не запрограммировано");
            if (treePlayers.SelectedNode == null)
                return;
            AccountData tData = (AccountData)treePlayers.SelectedNode.Tag;
            AdmiralStats tStats = new AdmiralStats(ref tData);
            */
        }
        /*public static AccountData GetLatestUser()
        {
            string q = "SELECT MAX(id) AS max_id FROM admirals";
            int admiralId = Convert.ToInt32(DataConnection.GetResult(q));

            AccountData tData = new AccountData(admiralId);
            return tData;

        }*/
        private void buttonClearPlayerProgress_Click(object sender, EventArgs e)
        {
            /*
            if (treePlayers.SelectedNode == null)
                return;
            AccountData tData = (AccountData)treePlayers.SelectedNode.Tag;
            int Id = tData.Id;
            string q;
            q = $@" 
                DELETE FROM [admirals_log] WHERE admiral = {Id};
                DELETE FROM [admirals_ships] WHERE player_id = {Id};
                DELETE FROM [admirals_modules] WHERE player_id = {Id};
                DELETE FROM game_events_log WHERE player_id = {Id};
                DELETE FROM crew_officers_stats WHERE crew_officer_id IN (SELECT id FROM crew_officers WHERE player_id = {Id});
                DELETE FROM crew_officers WHERE player_id = {Id};
                DELETE FROM ss_rigs_slots WHERE ss_rig_id IN(SELECT id FROM ss_rigs WHERE player_id = {Id});
                DELETE FROM ss_rigs WHERE player_id = {Id}; ";
            DataConnection.Execute(q);
            MessageBox.Show("Стерто");
            */
        }

        private void buttonSavePlayerAssets_Click(object sender, EventArgs e)
        {
            /*
            
            //Сохраняется игрок в виде офицера, список кораблей и модулей.
            //Каждый из видов ассетов сохраняется в отдельный файл, в папку с игрой.
            //Потом переносим код в обратной последовательности, большинство из
            //того что будет здесь написано до/после файловых функций может быть
            //использовано в реальном сетевом коде готового продукта
            if (treePlayers.SelectedNode == null)
            {
                MessageBox.Show("Выберите игрока, ассет которого нужно сохранить в файл");
                return;
            }
                
            AccountData tData = (AccountData)treePlayers.SelectedNode.Tag;
            PlayerAsset asset = new PlayerAsset(tData.Id);
            string textAsset = JsonConvert.SerializeObject(asset);
            textAsset = CommonFunctions.Compress(textAsset);
            System.IO.File.WriteAllText("player assets.txt", textAsset);

            textAsset = CommonFunctions.Decompress(textAsset);
            PlayerAsset asset2 = JsonConvert.DeserializeObject<PlayerAsset>(textAsset);
            asset2.ClearDeserializationDuplicates();

            Process.Start(Directory.GetCurrentDirectory());
            */
        }
        private void buttonSaveDataset_Click(object sender, EventArgs e)
        {
            ObjectDatabase od = new ObjectDatabase();
            od.LoadDataAssetEditor();
            od.SaveDataToStringFile();
            //MessageBox.Show("Completed");
            Process.Start(Directory.GetCurrentDirectory());
        }

        private void buttonSerializationTests_Click(object sender, EventArgs e)
        {
            /*
            if (treePlayers.SelectedNode == null)
                return;
            AccountData tData = (AccountData)treePlayers.SelectedNode.Tag;
            PlayerAsset asset = new PlayerAsset(tData.Id);
            string textAsset = JsonConvert.SerializeObject(asset.Ships);

            List<Ship> ships2 = JsonConvert.DeserializeObject<List<Ship>>(textAsset);
            */
        }

        private void buttonConvertModulesFormat_Click(object sender, EventArgs e)
        {
            /*List<ShipModuleType> oldModules = ShipModuleType.CreateList();
            foreach(var oldModule in oldModules)
            {
                ShipModuleType newModule = new ShipModuleType(oldModule);
                newModule.SaveData();
            }*/
        }

        #endregion

        #region История - какой объект за каким следует

        private bool historyFilled;

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void tabPage7_Enter(object sender, EventArgs e)
        {

            if (historyFilled)
                return;

            SqlDataReader r = DataConnection.GetReader(
                @"SELECT 
                    id,
                    previous_object_type,
                    previous_object_id,
                    object_type,
                    object_id
                FROM 
                    story_object_flow");
            if (r.HasRows)
            {
                while (r.Read())
                {
                    GridStoryFlow.Rows.Add();
                    DataGridViewRow row = GridStoryFlow.Rows[GridStoryFlow.Rows.Count - 2];
                    row.Cells["s_id"].Value = Convert.ToString(r["id"]);
                    row.Cells["s_prev_name"].Value = Convert.ToString(r["previous_object_type"]);
                    row.Cells["s_prev"].Value = Convert.ToString(r["previous_object_id"]);
                    row.Cells["s_name"].Value = Convert.ToString(r["object_type"]);
                    row.Cells["s_code"].Value = Convert.ToString(r["object_id"]);
                }
            }
            r.Close();

            historyFilled = true;

        }

        private Dictionary<string, string> storyObjectDisct;
        private void GridStoryFlow_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (storyObjectDisct == null)
            {
                storyObjectDisct = new Dictionary<string, string>();
                storyObjectDisct.Add("s_prev_name", "previous_object_type");
                storyObjectDisct.Add("s_prev", "previous_object_id");
                storyObjectDisct.Add("s_name", "object_type");
                storyObjectDisct.Add("s_code", "object_id");
            }

            string q;
            DataGridViewRow row = GridStoryFlow.Rows[e.RowIndex];
            int id = 0;
            if (row.Cells["s_id"].Value != null)
            {
                if (Convert.ToString(row.Cells["s_id"].Value) != "")
                {
                    id = Convert.ToInt32(row.Cells["s_id"].Value);
                }
            }
            if (id == 0)
            {
                q = @"
                     INSERT INTO story_object_flow(previous_object_type) VALUES('')
                     SELECT @@IDENTITY AS Result";
                id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));
            }

            string columnName = GridStoryFlow.Columns[e.ColumnIndex].Name;
            string Value = Convert.ToString(row.Cells[e.ColumnIndex].Value);

            List<String> names = new List<string> { Value };


            q = $@"UPDATE story_object_flow SET " + storyObjectDisct[columnName] + " = @str1 WHERE id = " + id;
            DataConnection.Execute(q, names);
        }


        #endregion

        #region SS modules

        private bool modulesFilled;

        private void tabPage8_Click(object sender, EventArgs e)
        {

        }

        private void tabPage8_Enter(object sender, EventArgs e)
        {
            if (modulesFilled)
                return;
            FillModules();
            FillModuleSizeDict();
            FillModuleWeaponType();
        }

        private void FillModules()
        {
            treeModules.Nodes.Clear();
            List<ShipModuleType> tModules = ShipModuleType.CreateList(false);

            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();

            if(tModules.Count > 0)
            {
                foreach(var tag in tModules)
                {
                    TreeNode n;
                    if (tag.Parent == 0)
                    {
                        n = treeModules.Nodes.Add(tag.Name);
                    }
                    else
                    {
                        n = nodeDict[tag.Parent].Nodes.Add(tag.Name);
                    }
                    n.Tag = tag;
                    nodeDict.Add(tag.Id, n);
                }
            }

            if (moduleTabDict == null)
            {
                moduleTabDict = new Dictionary<ShipModuleType.ModuleTypes, TabPage>();
                moduleTabDict.Add(ShipModuleType.ModuleTypes.Weapon, tabWeapon);
                moduleTabDict.Add(ShipModuleType.ModuleTypes.Armor, tabArmor);
                moduleTabDict.Add(ShipModuleType.ModuleTypes.Reactor, tabReactor);
                moduleTabDict.Add(ShipModuleType.ModuleTypes.Thrusters, tabThrusters);
                moduleTabDict.Add(ShipModuleType.ModuleTypes.Shield, tabShield);

                moduleTypeDict = new Dictionary<TabPage, ShipModuleType.ModuleTypes>();
                moduleTypeDict.Add(tabWeapon, ShipModuleType.ModuleTypes.Weapon);
                moduleTypeDict.Add(tabArmor, ShipModuleType.ModuleTypes.Armor);
                moduleTypeDict.Add(tabReactor, ShipModuleType.ModuleTypes.Reactor);
                moduleTypeDict.Add(tabThrusters, ShipModuleType.ModuleTypes.Thrusters);
                moduleTypeDict.Add(tabShield, ShipModuleType.ModuleTypes.Shield);

            }

            modulesFilled = true;

        }

        private Dictionary<int, RadioButton> ModuleSizeToRadioDict;
        private Dictionary<RadioButton, int> ModuleRadioToSizeDict;
        private void FillModuleSizeDict()
        {
            ModuleSizeToRadioDict = new Dictionary<int, RadioButton>();
            ModuleSizeToRadioDict.Add(1, radioModule1);
            ModuleSizeToRadioDict.Add(2, radioModule2);
            ModuleSizeToRadioDict.Add(3, radioModule3);
            ModuleRadioToSizeDict = new Dictionary<RadioButton, int>();
            ModuleRadioToSizeDict.Add(radioModule1, 1);
            ModuleRadioToSizeDict.Add(radioModule2, 2);
            ModuleRadioToSizeDict.Add(radioModule3, 3);
        }

        private Dictionary<ShipModuleType.WeaponTypes, RadioButton> ModuleWeaponTypeToRadioDict;
        private Dictionary<RadioButton, ShipModuleType.WeaponTypes> ModuleRadioToWeaponTypeDict;
        
        private void FillModuleWeaponType()
        {
            ModuleWeaponTypeToRadioDict = new Dictionary<ShipModuleType.WeaponTypes, RadioButton>();
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Laser, radioLaser);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Kinetic, radioKinetic);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Explosive, radioExplosive);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Ray, radioRay);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Plasma, radioPlasma);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Gravity, radioGravity);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponTypes.Doom, radioDoom);
            ModuleRadioToWeaponTypeDict = new Dictionary<RadioButton, ShipModuleType.WeaponTypes>();
            ModuleRadioToWeaponTypeDict.Add(radioLaser, ShipModuleType.WeaponTypes.Laser);
            ModuleRadioToWeaponTypeDict.Add(radioKinetic, ShipModuleType.WeaponTypes.Kinetic);
            ModuleRadioToWeaponTypeDict.Add(radioExplosive, ShipModuleType.WeaponTypes.Explosive);
            ModuleRadioToWeaponTypeDict.Add(radioRay, ShipModuleType.WeaponTypes.Ray);
            ModuleRadioToWeaponTypeDict.Add(radioPlasma, ShipModuleType.WeaponTypes.Plasma);
            ModuleRadioToWeaponTypeDict.Add(radioGravity, ShipModuleType.WeaponTypes.Gravity);
            ModuleRadioToWeaponTypeDict.Add(radioDoom, ShipModuleType.WeaponTypes.Doom);
        }

        private void buttonSsAddCategory_Click(object sender, EventArgs e)
        {
            AddModuleNode(1);
        }
        private void buttonSsAddItem_Click(object sender, EventArgs e)
        {
            AddModuleNode(0);
        }

        private void AddModuleNode(int isCategory)
        {
            TreeNode newNode;
            TreeNodeCollection nodesCollection;
            int parentTag;
            if (treeModules.SelectedNode == null)
            {
                nodesCollection = treeModules.Nodes;
                parentTag = 0;
            }
            else
            {
                nodesCollection = treeModules.SelectedNode.Nodes;
                ShipModuleType tTag = (ShipModuleType)treeModules.SelectedNode.Tag;
                parentTag = tTag.Id;

            }

            ShipModuleType tag = new ShipModuleType();
            tag.Parent = parentTag;
            tag.IsCategory = isCategory;
            newNode = nodesCollection.Add(tag.Name);
            newNode.Tag = tag;
            treeModules.SelectedNode = newNode;
        }

        private Dictionary<ShipModuleType.ModuleTypes, TabPage> moduleTabDict;
        private Dictionary<TabPage, ShipModuleType.ModuleTypes> moduleTypeDict;

        private void treeModules_AfterSelect(object sender, TreeViewEventArgs e)
        {

            ClearModule();

            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            
            NoEvents = true;

            textModuleId.Text = tag.Id.ToString();
            textName.Text = tag.Name;
            textModuleUnity.Text = tag.AssetName;
            textModuleType.Text = tag.ModuleType.ToString();
            textModuleEnergy.Text = tag.EnergyNeed.ToString();
            textModuleImgId.Text = tag.ImgId.ToString();
            if (tag.IsCategory == 1)
            {
                NoEvents = false;
                return;
            }

            ModuleSizeToRadioDict[tag.Size].Checked = true;

            ModuleSizeToRadioDict[tag.Size].Checked = true;

            //Weapon module type filling
            if((int)tag.WeaponType > 0)
            {
                ModuleWeaponTypeToRadioDict[tag.WeaponType].Checked = true;
            }
            textModuleFireRate.Text = tag.FireRate.ToString();
            textModuleDamageAmount.Text = tag.DamageAmount.ToString();
            textModuleShieldEffectiveness.Text = tag.ShieldEffectiveness.ToString();
            textModuleArmorEffectiveness.Text = tag.ArmorEffectiveness.ToString();
            textModuleStructureEffectiveness.Text = tag.StructureEffectiveness.ToString();
            checkModuleIgnoreShield.Checked = tag.IgnoreShield == 1;
            textModuleCriticalChance.Text = tag.CriticalChance.ToString();
            textModuleCriticalStrength.Text = tag.CriticalStrength.ToString();

            //Armor
            textModuleArmor.Text = tag.ArmorPoints.ToString();

            //Reactor
            textModuleReactor.Text = tag.ReactorPoints.ToString();

            //Thrusters
            textModuleThrustStrength.Text = tag.ThrustStrength.ToString();
            textModuleDexterity.Text = tag.Dexterity.ToString();

            //Shields
            textModuleShieldPoints.Text = tag.ShieldPoints.ToString();
            textModuleShieldRegen.Text = tag.ShieldRegen.ToString();

            if (moduleTabDict.ContainsKey(tag.ModuleType))
            {
                tabControlModule.SelectedTab = moduleTabDict[tag.ModuleType];
            }
            else
            {
                tabControlModule.SelectedTab = tabControlModule.TabPages[0];
            }


            NoEvents = false;

            
        }

        private void ClearModule()
        {
            NoEvents = true;
            textName.Text = "";
            textModuleUnity.Text = "";
            textModuleEnergy.Text = "";
            textModuleType.Text = "";
            textModuleImgId.Text = "";
            foreach (RadioButton key in ModuleRadioToSizeDict.Keys)
            {
                key.Checked = false;
            }

            foreach (RadioButton key in ModuleRadioToWeaponTypeDict.Keys)
            {
                key.Checked = false;
            }

            textModuleFireRate.Text = "";
            textModuleDamageAmount.Text = "";
            textModuleShieldEffectiveness.Text = "";
            textModuleCriticalChance.Text = "";
            textModuleCriticalStrength.Text = "";

            //Armor
            textModuleArmor.Text = "";

            //Reactor
            textModuleReactor.Text = "";

            //Thrusters
            textModuleThrustStrength.Text = "";
            textModuleDexterity.Text = "";

            //Shields
            textModuleShieldPoints.Text = "";
            textModuleShieldRegen.Text = "";

            NoEvents = false;

        }

        private ShipModuleType GetModuleTag()
        {
            if (treeModules.SelectedNode == null)
                return null;

            ShipModuleType tag = (ShipModuleType)treeModules.SelectedNode.Tag;
            return tag;
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            tag.Name = textName.Text;
            treeModules.SelectedNode.Text = textName.Text;
        }

        private void textModuleUnity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            tag.AssetName = textModuleUnity.Text;
        }

        private void textModuleEnergy_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleEnergy.Text, out tNumber);
            tag.EnergyNeed = tNumber;
        }
        private void textModuleImgId_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleImgId.Text, out tNumber);
            tag.ImgId = tNumber;

        }
        private void radioModule1_CheckedChanged(object sender, EventArgs e)
        {
            SetModuleSize();
        }
        private void radioModule2_CheckedChanged(object sender, EventArgs e)
        {
            SetModuleSize();
        }
        private void radioModule3_CheckedChanged(object sender, EventArgs e)
        {
            SetModuleSize();
        }
        private void SetModuleSize()
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            foreach (RadioButton key in ModuleRadioToSizeDict.Keys)
            {
                if (key.Checked)
                {
                    tag.Size = ModuleRadioToSizeDict[key];
                    return;
                }
            }
        }

        private void buttonSaveModule_Click(object sender, EventArgs e)
        {
            textModuleType.Text = moduleTypeDict[tabControlModule.SelectedTab].ToString();
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            tag.SaveData();
            textModuleId.Text = tag.Id.ToString();
        }

        #region Module tab scores editing

        private void textModuleFireRate_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleFireRate.Text, out tNumber);
            tag.FireRate = tNumber;
        }

        private void textModuleDamageAmount_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDamageAmount.Text, out tNumber);
            tag.DamageAmount = tNumber;

        }

        private void textModuleShieldEffectiveness_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleShieldEffectiveness.Text, out tNumber);
            tag.ShieldEffectiveness = tNumber;
        }

        private void textModuleArmorEffectiveness_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleArmorEffectiveness.Text, out tNumber);
            tag.ArmorEffectiveness = tNumber;
        }

        private void textModuleStructureEffectiveness_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleStructureEffectiveness.Text, out tNumber);
            tag.StructureEffectiveness = tNumber;
        }

        private void checkModuleIgnoreShield_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            if(checkModuleIgnoreShield.Checked)
            {
                tag.IgnoreShield = 1;
            }
            else
            {
                tag.IgnoreShield = 0;
            }
        }

        private void textModuleCriticalChance_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleCriticalChance.Text, out tNumber);
            tag.CriticalChance = tNumber;
        }

        private void textModuleCriticalStrength_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleCriticalStrength.Text, out tNumber);
            tag.CriticalStrength = tNumber;
        }

        private void textModuleArmor_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleArmor.Text, out tNumber);
            tag.ArmorPoints = tNumber;
        }

        private void textModuleEngine_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleReactor.Text, out tNumber);
            tag.ReactorPoints = tNumber;
        }

        private void textModuleSpeed_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleThrustStrength.Text, out tNumber);
            tag.ThrustStrength = tNumber;
        }

        private void textDexterity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDexterity.Text, out tNumber);
            tag.Dexterity = tNumber;
        }

        private void textModuleDeflectors_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleShieldPoints.Text, out tNumber);
            tag.ShieldPoints = tNumber;
        }

        private void textModuleDeflectorsRegen_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleShieldRegen.Text, out tNumber);
            tag.ShieldRegen = tNumber;
        }

        private void radioLaser_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void radioExplosive_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void radioKinetic_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void radioRay_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void radioPlasma_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void radioGravity_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void radioDoom_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SetWeaponType();
        }

        private void SetWeaponType()
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            foreach (RadioButton key in ModuleRadioToWeaponTypeDict.Keys)
            {
                if (key.Checked)
                {
                    tag.WeaponType = ModuleRadioToWeaponTypeDict[key];
                    return;
                }
            }
        }

        private void tabWeapon_Enter(object sender, EventArgs e)
        {
            SetModuleType();
        }
        private void tabArmor_Enter(object sender, EventArgs e)
        {
            SetModuleType();
        }
        private void tabReactor_Enter(object sender, EventArgs e)
        {
            SetModuleType();
        }
        private void tabThrusters_Enter(object sender, EventArgs e)
        {
            SetModuleType();
        }
        private void tabShield_Enter(object sender, EventArgs e)
        {
            SetModuleType();
        }

        private void SetModuleType()
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;

            tag.ModuleType = moduleTypeDict[tabControlModule.SelectedTab];
            textModuleType.Text = tag.ModuleType.ToString();

        }

        #endregion

        #endregion

        #region Officer types

        private bool officerTypesFilled;

        private void tabPage11_Click(object sender, EventArgs e)
        {

        }
        private void tabPage11_Enter(object sender, EventArgs e)
        {
            if (officerTypesFilled)
                return;
            FillOfficerTypes();
        }

        private void tabPage10_Click(object sender, EventArgs e)
        {

        }
        private void tabPage10_Enter(object sender, EventArgs e)
        {
            if (officerTypesFilled)
                return;
            FillOfficerTypes();
        }

        private void FillOfficerTypes()
        {

            string q;
            q = CrewOfficerType.OfficerTypeQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    CrewOfficerType curOfficer = new CrewOfficerType(ref r);
                    TreeNode n = treeOfficerTypes.Nodes.Add(curOfficer.Name);
                    n.Tag = curOfficer;
                }
            }
            r.Close();

            officerTypesFilled = true;
        }

        private void buttonAddOfficerType_Click(object sender, EventArgs e)
        {
            CrewOfficerType curOfficer = new CrewOfficerType();
            TreeNode n = treeOfficerTypes.Nodes.Add(curOfficer.Name);
            n.Tag = curOfficer;
            treeOfficerTypes.SelectedNode = n;
        }

        private void ClearOfficerType()
        {
            NoEvents = true;
            textOfficerTypeName.Text = "";
            checkOfficerAvailableAtStart.Checked = false;
            textOfficerPortraitId.Text = "";
            textOfficerTypeBonusPoints.Text = "";
            gridOfficerType.Rows.Clear();
            NoEvents = false;
        }

        private CrewOfficerType GetCurrentOfficerType()
        {
            if (treeOfficerTypes.SelectedNode == null)
                return null;
            return (CrewOfficerType)treeOfficerTypes.SelectedNode.Tag;
        }


        private void treeOfficerTypes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearOfficerType();
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;

            NoEvents = true;
            textOfficerTypeName.Text = curOfficerType.Name;
            textOfficerPortraitId.Text = curOfficerType.PortraitId.ToString();
            textOfficerTypeBonusPoints.Text = curOfficerType.BonusPoints.ToString();
            gridOfficerType.Rows.Clear();

            List<OfficerTypeStat> stats = curOfficerType.Stats;
            if (stats.Count > 0)
            {
                foreach (OfficerTypeStat stat in stats)
                {
                    DataGridViewRow row;
                    gridOfficerType.Rows.Add();
                    row = gridOfficerType.Rows[gridOfficerType.Rows.Count - 1];
                    row.Cells["ot_name"].Value = stat.ToString();
                    row.Cells["ot_score"].Value = stat.PointsBase;
                    row.Cells["ot_stat_object"].Value = stat;
                }
            }

            NoEvents = false;

        }
        private void textOfficerTypeName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;
            treeOfficerTypes.SelectedNode.Text = textOfficerTypeName.Text;
            curOfficerType.Name = textOfficerTypeName.Text;
        }

        private void textOfficerPortraitId_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;
            int tVal = 0;
            Int32.TryParse(textOfficerPortraitId.Text, out tVal);
            curOfficerType.PortraitId = tVal;
        }

        private void textOfficerTypeBonusPoints_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;
            int tVal = 0;
            Int32.TryParse(textOfficerTypeBonusPoints.Text, out tVal);
            curOfficerType.BonusPoints = tVal;
        }


        private void gridOfficerType_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gridOfficerType_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (NoEvents)
                return;
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;
            DataGridViewRow row = gridOfficerType.Rows[e.RowIndex];
            int baseValue = 0;
            Int32.TryParse(Convert.ToString(row.Cells["ot_score"].Value), out baseValue);

            Crew.OfficerTypeStat stat = (Crew.OfficerTypeStat)row.Cells["ot_stat_object"].Value;
            stat.PointsBase = baseValue;
            //curOfficerType.SetStatValue(Convert.ToString(row.Cells["ot_name"].Value), baseValue);

        }

        private void buttonSaveOfficerType_Click(object sender, EventArgs e)
        {
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;
            curOfficerType.SaveData();
        }


        #endregion

        #region Игровые события

        private bool eventsFilled;

        private void tabPage19_Click(object sender, EventArgs e)
        {

        }
        private void tabPage19_Enter(object sender, EventArgs e)
        {
            if (eventsFilled)
                return;
            FillEvents();
        }
        private void buttonEventUpdate_Click(object sender, EventArgs e)
        {
            FillEvents();
        }

        private void FillEvents()
        {
            treeEvents.Nodes.Clear();
            TreeNode mainNode = treeEvents.Nodes.Add("События в игре");

            List<GameEvent> events = GameEvent.EventList();
            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();
            foreach (GameEvent gameEvent in events)
            {
                TreeNode n;
                if (gameEvent.ParentId == 0)
                {
                    n = mainNode.Nodes.Add(gameEvent.Name);
                }
                else
                {
                    n = nodeDict[gameEvent.ParentId].Nodes.Add(gameEvent.Name);
                }

                n.Tag = gameEvent;
                nodeDict.Add(gameEvent.Id, n);
            }

            comboEventSpaceship.Items.Clear();
            List<ShipModel> ships = ShipModel.GetModelList();
            foreach (ShipModel model in ships)
            {
                comboEventSpaceship.Items.Add(model);
            }

            comboEventModule.Items.Clear();
            List<ShipModuleType> modules = ShipModuleType.CreateList(false);
            foreach (ShipModuleType module in modules)
            {
                comboEventModule.Items.Add(module);
            }

            comboEventOfficer.Items.Clear();
            List<CrewOfficerType> officerTypes = CrewOfficerType.GetTypeList();
            foreach (CrewOfficerType ofType in officerTypes)
            {
                comboEventOfficer.Items.Add(ofType);
            }

            mainNode.Expand();

            FillEventDictionaries();

            eventsFilled = true;

        }

        private Dictionary<string, TabPage> eventStringToTabDict;
        private Dictionary<TabPage, string> eventTabToStringDict;

        private void FillEventDictionaries()
        {
            if (eventTabToStringDict != null)
                return;

            eventStringToTabDict = new Dictionary<string, TabPage>();
            eventStringToTabDict.Add("Give spaceship", tabGiveSpaceShip);
            eventStringToTabDict.Add("Give module", tabGiveModule);
            eventStringToTabDict.Add("Give resources", tabGiveResources);
            eventStringToTabDict.Add("Create officer", tabCreateOfficer);

            eventTabToStringDict = new Dictionary<TabPage, string>();
            eventTabToStringDict.Add(tabGiveSpaceShip, "Give spaceship");
            eventTabToStringDict.Add(tabGiveModule, "Give module");
            eventTabToStringDict.Add(tabGiveResources, "Give resources");
            eventTabToStringDict.Add(tabCreateOfficer, "Create officer");

        }

        private void buttonEventAdd_Click(object sender, EventArgs e)
        {
            if (treeEvents.SelectedNode == null)
                return;
            TreeNode n;
            int parentId = 0;
            GameEvent newEvent = new GameEvent();
            newEvent.Name = "New game event";
            if (treeEvents.SelectedNode.Tag != null)
            {
                GameEvent gEvent = (GameEvent)treeEvents.SelectedNode.Tag;
                parentId = gEvent.Id;
                n = treeEvents.SelectedNode.Nodes.Add(newEvent.Name);
            }
            else
            {
                parentId = 0;
                n = treeEvents.Nodes[0].Nodes.Add(newEvent.Name);
            }

            newEvent.ParentId = parentId;
            n.Tag = newEvent;
            treeEvents.SelectedNode = n;
        }

        private GameEvent GetCurrentEvent()
        {
            if (treeEvents.SelectedNode == null)
            {
                return null;
            }
            if (treeEvents.SelectedNode.Tag == null)
            {
                return null;
            }

            GameEvent gEvent = (GameEvent)treeEvents.SelectedNode.Tag;
            return gEvent;

        }

        private void treeEvents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearEvent();
            if (NoEvents)
                return;
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            NoEvents = true;
            textEventId.Text = curEvent.Id.ToString();
            textEventName.Text = curEvent.Name;
            checkEventRepeatable.Checked = curEvent.Repeatable == 1;

            listEventElements.Items.Clear();
            if (curEvent.Elements.Count > 0)
            {
                foreach (GameEvent.EventElement eventElement in curEvent.Elements)
                {
                    listEventElements.Items.Add(eventElement);
                }
            }
            NoEvents = false;
        }

        private void ClearEvent()
        {
            NoEvents = true;
            textEventId.Text = "";
            textEventName.Text = "";
            NoEvents = false;
        }

        private void textEventName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            curEvent.Name = textEventName.Text;
            treeEvents.SelectedNode.Text = textEventName.Text;
        }

        private void checkEventRepeatable_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            curEvent.Repeatable = checkEventRepeatable.Checked ? 1 : 0;
        }

        private void buttonSaveEvent_Click(object sender, EventArgs e)
        {
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            curEvent.Save();
            textEventId.Text = curEvent.Id.ToString();
        }

        private void buttonEventDelete_Click(object sender, EventArgs e)
        {
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            if (treeEvents.SelectedNode.Nodes.Count > 0)
            {
                MessageBox.Show("Нельзя удалять события, если у них есть подчиненные");
                return;
            }
            if (MessageBox.Show("Удалить событие?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            curEvent.Delete();
            treeEvents.SelectedNode.Parent.Nodes.Remove(treeEvents.SelectedNode);
        }

        private void buttonAddEventElement_Click(object sender, EventArgs e)
        {
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            GameEvent.EventElement elem = curEvent.AddElement();
            listEventElements.Items.Add(elem);
            listEventElements.SelectedItem = elem;
        }

        private void buttonDeleteEventElement_Click(object sender, EventArgs e)
        {

            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            curElement.Delete();
            listEventElements.Items.Remove(listEventElements.SelectedItem);

            //GameEvent curEvent = GetCurrentEvent();
            //if (curEvent == null)
            //    return;
            //curEvent.Delete();
            //listEventElements.Items.Remove(curEvent);
        }

        private void listEventElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ClearEventElement();
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            NoEvents = true;
            if (String.IsNullOrEmpty(curElement.ElementType))
            {
                tabControlEventElements.SelectedTab = tabGiveSpaceShip;
            }
            else
            {
                tabControlEventElements.SelectedTab = eventStringToTabDict[curElement.ElementType];
            }
            textEventSpaceshipExperience.Text = curElement.Experience.ToString();
            textEventModuleExperience.Text = curElement.Experience.ToString();
            textEventOfficerExperience.Text = curElement.Experience.ToString();
            SetEventElementCombos(curElement);
            NoEvents = false;
        }

        private void SetEventElementCombos(GameEvent.EventElement element)
        {
            if (element.ShipModel != null)
            {
                foreach (ShipModel item in comboEventSpaceship.Items)
                {
                    if (item.Id == element.ShipModel.Id)
                    {
                        comboEventSpaceship.SelectedItem = item;
                        break;
                    }
                }
            }

            if (element.ModuleType != null)
            {
                foreach (ShipModuleType item in comboEventModule.Items)
                {
                    if (item.Id == element.ModuleType.Id)
                    {
                        comboEventModule.SelectedItem = item;
                        break;
                    }
                }
            }

            if (element.Officer != null)
            {
                foreach (CrewOfficerType item in comboEventOfficer.Items)
                {
                    if (item.Id == element.Officer.Id)
                    {
                        comboEventOfficer.SelectedItem = item;
                        break;
                    }
                }
            }

        }

        private GameEvent.EventElement GetCurrentEventElement()
        {
            if (listEventElements.SelectedItem == null)
                return null;
            return (GameEvent.EventElement)listEventElements.SelectedItem;
        }

        private void ClearEventElement()
        {
            NoEvents = true;
            comboEventModule.SelectedItem = null;
            comboEventOfficer.SelectedItem = null;
            comboEventSpaceship.SelectedItem = null;
            textEventSpaceshipExperience.Text = "";
            textEventModuleExperience.Text = "";
            textEventOfficerExperience.Text = "";
            NoEvents = false;
        }
        private void tabControlEventElements_Selected(object sender, TabControlEventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            curElement.ElementType = eventTabToStringDict[tabControlEventElements.SelectedTab];
            NoEvents = true;
            listEventElements.Items[listEventElements.SelectedIndex] = listEventElements.SelectedItem;
            comboEventSpaceship.SelectedItem = null;
            comboEventModule.SelectedItem = null;
            comboEventOfficer.SelectedItem = null;
            NoEvents = false;
        }

        private void comboEventSpaceship_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            curElement.ShipModel = (ShipModel)comboEventSpaceship.SelectedItem;
        }

        private void textEventSpaceshipExperience_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            int value = 0;
            Int32.TryParse(textEventSpaceshipExperience.Text, out value);
            curElement.Experience = value;
            textEventModuleExperience.Text = curElement.Experience.ToString();
            textEventOfficerExperience.Text = curElement.Experience.ToString();
        }

        private void comboEventModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            curElement.ModuleType = (ShipModuleType)comboEventModule.SelectedItem;
        }

        private void textEventModuleExperience_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            int value = 0;
            Int32.TryParse(textEventModuleExperience.Text, out value);
            curElement.Experience = value;
            textEventSpaceshipExperience.Text = curElement.Experience.ToString();
            textEventOfficerExperience.Text = curElement.Experience.ToString();
        }

        private void comboEventOfficer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            curElement.Officer = (CrewOfficerType)comboEventOfficer.SelectedItem;
        }

        private void textEventOfficerExperience_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent.EventElement curElement = GetCurrentEventElement();
            if (curElement == null)
                return;
            int value = 0;
            Int32.TryParse(textEventOfficerExperience.Text, out value);
            curElement.Experience = value;
            textEventSpaceshipExperience.Text = curElement.Experience.ToString();
            textEventModuleExperience.Text = curElement.Experience.ToString();
        }

        private void buttonEventTest_Click(object sender, EventArgs e)
        {
            /*AccountData curPlayer = GetLatestUser();
            GameEvent curEvent = GetCurrentEvent();
            if (curEvent == null)
                return;
            curEvent.ExecuteEvent(curPlayer.Id);
            MessageBox.Show("Успешно");
            */
        }

        #endregion

        #region Battle scenes

        private bool battleScenesFilled;

        private void tabPage20_Click(object sender, EventArgs e)
        {

        }

        private void tabPage20_Enter(object sender, EventArgs e)
        {
            if (battleScenesFilled)
                return;
            FillBattleScenes();
        }

        

        private void buttonBsUpdate_Click(object sender, EventArgs e)
        {
            FillBattleScenes();
            ClearBs();
            ClearBsEnemy();
            ClearBsResource();
        }

        private void FillBattleScenes()
        {
            treeBs.Nodes.Clear();
            TreeNode mainNode = treeBs.Nodes.Add("Battle scenes");
            Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
            List<BattleSceneType> scenes = BattleSceneType.SceneList();
            if (scenes.Count > 0)
            {
                foreach (BattleSceneType scene in scenes)
                {
                    TreeNode n;
                    if (scene.ParentId > 0)
                    {
                        n = nodes[scene.ParentId].Nodes.Add(scene.ToString());

                    }
                    else
                    {
                        n = mainNode.Nodes.Add(scene.ToString());
                    }
                    n.Tag = scene;
                    nodes.Add(scene.Id, n);
                }
            }

            comboBsEnemy.Items.Clear();
            List<SpaceshipRig> rigs = SpaceshipRig.BuiltInRigs();
            if (rigs.Count > 0)
            {
                foreach (SpaceshipRig rig in rigs)
                {
                    comboBsEnemy.Items.Add(rig);
                }
            }


            comboBsResourceType.Items.Clear();
            List<ResourceType> resources = ResourceType.GetResouceList();
            if(resources.Count > 0)
            {
                foreach(ResourceType resource in resources)
                {
                    comboBsResourceType.Items.Add(resource);
                }
            }

            comboBsBlueprint.Items.Clear();
            var BpList = BlueprintType.GetList();
            if(BpList.Count > 0)
            {
                foreach(var bp in BpList)
                {
                    comboBsBlueprint.Items.Add(bp);
                }
            }


            mainNode.Expand();
            battleScenesFilled = true;
        }

        private void treeBs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearBs();

            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            NoEvents = true;
            textBsId.Text = sceneType.Id.ToString();
            textBsName.Text = sceneType.Name;
            textBsObjectives.Text = sceneType.MissionObjective;
            textBsCycleToComplete.Text = sceneType.CycleToComplete.ToString();
            checkBsAssembleShip.Checked = sceneType.AssembleShip == 1;
            listBsEnemies.Items.Clear();
            ClearBsEnemy();
            if (sceneType.enemies.Count > 0)
            {
                foreach(BattleSceneTypeEnemy enemy in sceneType.enemies)
                {
                    listBsEnemies.Items.Add(enemy);
                }
            }

            listBsResources.Items.Clear();
            List<BattleSceneTypeResource> list = sceneType.resources;
            if(list.Count > 0)
            {
                foreach(BattleSceneTypeResource resource in list)
                {
                    listBsResources.Items.Add(resource);
                }
            }

            NoEvents = false;

            FillBsEnemyInResource();


        }

        private BattleSceneType GetCurrentBattleScene()
        {
            if (treeBs.SelectedNode == null)
                return null;
            if (treeBs.SelectedNode.Tag == null)
                return null;
            return (BattleSceneType)treeBs.SelectedNode.Tag;
        }

        private void ClearBs()
        {
            NoEvents = true;
            textBsId.Text = "";
            textBsName.Text = "";
            textBsObjectives.Text = "";
            textBsCycleToComplete.Text = "";
            checkBsAssembleShip.Checked = false;
            listBsEnemies.Items.Clear();
            listBsResources.Items.Clear();
            NoEvents = false;
        }

        private void buttonBsAdd_Click(object sender, EventArgs e)
        {
            TreeNodeCollection hostNodes;
            int parentId = 0;
            if (treeBs.SelectedNode == null)
            {
                hostNodes = treeBs.Nodes[0].Nodes;
            }
            else if(treeBs.SelectedNode.Tag == null)
            {
                hostNodes = treeBs.Nodes[0].Nodes;
            }
            else
            {
                hostNodes = treeBs.SelectedNode.Nodes;
                BattleSceneType bsToAttach = (BattleSceneType)treeBs.SelectedNode.Tag;
                parentId = bsToAttach.Id;
            }

            BattleSceneType bs = new BattleSceneType();
            bs.ParentId = parentId;
            bs.Name = "new battle scene";
            TreeNode n = hostNodes.Add(bs.Name);
            n.Tag = bs;
            treeBs.SelectedNode = n;
        }

        private void buttonBsSave_Click(object sender, EventArgs e)
        {
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            sceneType.Save();
            textBsId.Text = sceneType.Id.ToString();

            BattleSceneTypeEnemy curEnemy = GetCurrentBsEnemy();
            if (curEnemy != null)
            {
                textBsEnemyId.Text = curEnemy.Id.ToString();
            }

        }

        private void buttonBsDelete_Click(object sender, EventArgs e)
        {
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            if (treeBs.SelectedNode.Nodes.Count > 0)
            {
                MessageBox.Show("Нельзя удалять событие, у которого есть подчиненные события");
                return;
            }

            if (MessageBox.Show("Удалить событие?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            sceneType.Delete();
            treeBs.SelectedNode.Parent.Nodes.Remove(treeBs.SelectedNode);
            ClearBs();
        }

        private void textBsName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            sceneType.Name = textBsName.Text;
            treeBs.SelectedNode.Text = sceneType.Name;
        }

        private void textBsObjectives_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            sceneType.MissionObjective = textBsObjectives.Text;
        }

        private void textBsCycleToComplete_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            int value = 0;
            Int32.TryParse(textBsCycleToComplete.Text, out value);
            sceneType.CycleToComplete = value;
        }

        private void checkBsAssembleShip_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            sceneType.AssembleShip = checkBsAssembleShip.Checked ? 1 : 0;
        }

        private void buttonBsAddEnemy_Click(object sender, EventArgs e)
        {            
            if (NoEvents)
                return;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            BattleSceneTypeEnemy enemy = sceneType.AddEnemy();
            listBsEnemies.Items.Add(enemy);
            listBsEnemies.SelectedItem = enemy;
        }

        private void buttonBsDeleteEnemy_Click(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            BattleSceneTypeEnemy curEnemy = GetCurrentBsEnemy();
            if (curEnemy == null)
                return;
            sceneType.DeleteEnemy(curEnemy);
            listBsEnemies.Items.Remove(curEnemy);
        }

        private BattleSceneTypeEnemy GetCurrentBsEnemy()
        {
            if (listBsEnemies.SelectedItem == null)
                return null;
            return (BattleSceneTypeEnemy)listBsEnemies.SelectedItem;
        }

        private void ClearBsEnemy()
        {
            NoEvents = true;
            textBsEnemyId.Text = "";
            comboBsEnemy.SelectedItem = null;
            textBsStageNumber.Text = "";
            textBsEnemyCount.Text = "";
            textBsCycleMultiplier.Text = "";
            textBsBattleIntensity.Text = "";
            textBsIntensityMultiplier.Text = "";
            NoEvents = false;
        }

        private void listBsEnemies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ClearBsEnemy();
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;

            NoEvents = true;
            if(enemy.Rig != null)
            {
                foreach (var item in comboBsEnemy.Items)
                {
                    SpaceshipRig rig = (SpaceshipRig)item;
                    if (rig.Id == enemy.Rig.Id)
                    {
                        comboBsEnemy.SelectedItem = item;
                        break;
                    }
                }
            }

            textBsEnemyId.Text = enemy.Id.ToString();
            textBsStageNumber.Text = enemy.StageNumber.ToString() ;
            textBsEnemyCount.Text = enemy.Count.ToString();
            textBsCycleMultiplier.Text = enemy.CycleMultiplier.ToString();
            textBsBattleIntensity.Text = enemy.BaseBattleIntensity.ToString();
            textBsIntensityMultiplier.Text = enemy.CycleIntensityMult.ToString();
            textBsMinimumCycle.Text = enemy.CycleFrom.ToString();
            textBsMaximumCycle.Text = enemy.CycleTo.ToString();
            NoEvents = false;

        }

        private void comboBsEnemy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            if(comboBsEnemy.SelectedItem == null)
            {
                enemy.ShipRigId = 0;
            }
            else
            {
                SpaceshipRig rig = (SpaceshipRig)comboBsEnemy.SelectedItem;
                enemy.ShipRigId = rig.Id;
            }
            NoEvents = true;
            listBsEnemies.Items[listBsEnemies.SelectedIndex] = listBsEnemies.SelectedItem;
            NoEvents = false;

        }
        private void textBsStageNumber_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsStageNumber.Text, out value);
            enemy.StageNumber = value;
        }
        private void textBsEnemyCount_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsEnemyCount.Text, out value);
            enemy.Count = value;
        }
        private void textBsCycleMultiplier_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsCycleMultiplier.Text, out value);
            enemy.CycleMultiplier = value;
        }
        private void textBsBattleIntensity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsBattleIntensity.Text, out value);
            enemy.BaseBattleIntensity = value;
        }
        private void textBsIntensityMultiplier_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsIntensityMultiplier.Text, out value);
            enemy.CycleIntensityMult = value;
        }

        private void textBsMinimumCycle_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsMinimumCycle.Text, out value);
            enemy.CycleFrom = value;
        }

        private void textBsMaximumCycle_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeEnemy enemy = GetCurrentBsEnemy();
            if (enemy == null)
                return;
            int value = 0;
            Int32.TryParse(textBsMaximumCycle.Text, out value);
            enemy.CycleTo = value;
        }

        private void FillBsEnemyInResource()
        {
            NoEvents = true;
            comboBsEnemyInResource.Items.Clear();
            NoEvents = false;
            BattleSceneType sceneType = GetCurrentBattleScene();
            if (sceneType == null)
                return;
            if (sceneType.enemies.Count == 0)
                return;
            NoEvents = true;
            foreach(BattleSceneTypeEnemy enemy in sceneType.enemies)
            {
                comboBsEnemyInResource.Items.Add(enemy);
            }
            NoEvents = false;
        }
        private void buttonBsRefreshEnemy_Click(object sender, EventArgs e)
        {
            FillBsEnemyInResource();
        }

        private void buttonBsAddResource_Click(object sender, EventArgs e)
        {
            BattleSceneType bs = GetCurrentBattleScene();
            if (bs == null)
                return;
            BattleSceneTypeResource res = bs.AddResource();
            listBsResources.Items.Add(res);
            listBsResources.SelectedItem = res;

        } 
        private void buttonBsDeleteResource_Click(object sender, EventArgs e)
        {
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            if (MessageBox.Show("Удалить данный ресурс?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            res.Delete();
            listBsResources.Items.Remove(listBsResources.SelectedItem);
            ClearBsResource();
        }
        private void listBsResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearBsResource();
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;

            NoEvents = true;
            checkBsAnyEnemy.Checked = res.AnyEnemy == 1;
            
            comboBsResourceType.SelectedItem = null;
            if (res.EnemyId > 0)
            {
                foreach (var item in comboBsEnemyInResource.Items)
                {
                    BattleSceneTypeEnemy enemy = (BattleSceneTypeEnemy)item;
                    if (enemy.Id == res.EnemyId)
                    {
                        comboBsEnemyInResource.SelectedItem = item;
                        break;
                    }
                }
            }
            if (res.ResourceId > 0)
            {
                foreach (var item in comboBsResourceType.Items)
                {
                    ResourceType resType = (ResourceType)item;
                    if (resType.Id == res.ResourceId)
                    {
                        comboBsResourceType.SelectedItem = item;
                        break;
                    }
                }
            }
            
            comboBsBlueprint.SelectedItem = null;
            if(res.BlueprintId > 0)
            {
                foreach(var item in comboBsBlueprint.Items)
                {
                    BlueprintType bt = (BlueprintType)item;
                    if(bt.Id == res.BlueprintId)
                    {
                        comboBsBlueprint.SelectedItem = item;
                        break;
                    }
                }
            }

            textBsBlueprintBonus.Text = res.BlueprintBonusPoints.ToString(); ;
            textBsAmountFrom.Text = res.AmountFrom.ToString();
            textBsAmountTo.Text = res.AmountTo.ToString();
            textBsChanceFrom.Text = res.VariableChanceFrom.ToString();
            textBsChanceTo.Text = res.VariableChanceTo.ToString();
            textBsCycleFrom.Text = res.MinimumCycle.ToString();
            textBsCycleTo.Text = res.MaximumCycle.ToString();
            textBsGuaranteedAmount.Text = res.GuaranteedAmount.ToString();
            NoEvents = false;

        }

        private BattleSceneTypeResource GetBsCurrentResource()
        {
            if (listBsResources.SelectedItem == null)
                return null;
            return (BattleSceneTypeResource)listBsResources.SelectedItem;
        }
        private void ClearBsResource()
        {
            NoEvents = true;
            comboBsEnemyInResource.SelectedItem = null;
            checkBsAnyEnemy.Checked = false;
            comboBsResourceType.SelectedItem = null;
            comboBsBlueprint.SelectedItem = null;
            textBsBlueprintBonus.Text = "";
            textBsAmountFrom.Text = "";
            textBsAmountTo.Text = "";
            textBsChanceFrom.Text = "";
            textBsChanceTo.Text = "";
            textBsCycleFrom.Text = "";
            textBsCycleTo.Text = "";
            textBsGuaranteedAmount.Text = "";
            NoEvents = false;
        }

        private void comboBsEnemyInResource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            if(comboBsEnemyInResource.SelectedItem == null)
            {
                res.EnemyId = 0;
            }
            else
            {
                BattleSceneTypeEnemy enemy = (BattleSceneTypeEnemy)comboBsEnemyInResource.SelectedItem;
                res.EnemyId = enemy.Id;
            }
        }
        private void checkBsAnyEnemy_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            res.AnyEnemy = checkBsAnyEnemy.Checked ? 1 : 0;
        }
        private void comboBsResourceType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            if (comboBsResourceType.SelectedItem == null)
            {
                res.ResourceId = 0;
            }
            else
            {
                ResourceType resType = (ResourceType)comboBsResourceType.SelectedItem;
                res.ResourceId = resType.Id;
            }
            NoEvents = true;
            listBsResources.Items[listBsResources.SelectedIndex] = res;
            NoEvents = false;
        }

        private void comboBsBlueprint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            if (comboBsBlueprint.SelectedItem == null)
            {
                res.BlueprintId = 0;
            }
            else
            {
                BlueprintType  resType = (BlueprintType)comboBsBlueprint.SelectedItem;
                res.BlueprintId = resType.Id;
            }
            NoEvents = true;
            listBsResources.Items[listBsResources.SelectedIndex] = res;
            NoEvents = false;
        }
        private void textBsBlueprintBonus_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsBlueprintBonus.Text, out value);
            res.BlueprintBonusPoints = value;
        }
        private void textBsAmountFrom_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsAmountFrom.Text, out value);
            res.AmountFrom = value;
        }
        private void textBsAmountTo_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsAmountTo.Text, out value);
            res.AmountTo = value;
        }
        private void textBsChanceFrom_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsChanceFrom.Text, out value);
            res.VariableChanceFrom = value;
        }
        private void textBsChanceTo_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsChanceTo.Text, out value);
            res.VariableChanceTo = value;
        }
        private void textBsCycleFrom_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsCycleFrom.Text, out value);
            res.MinimumCycle = value;
        }
        private void textBsCycleTo_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsCycleTo.Text, out value);
            res.MaximumCycle = value;
        }
        private void textBsGuaranteedAmount_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            BattleSceneTypeResource res = GetBsCurrentResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBsGuaranteedAmount.Text, out value);
            res.GuaranteedAmount = value;
        }

        #endregion

        #region Resource types

        private bool resourcesFilled;

        private void tabPage23_Enter(object sender, EventArgs e)
        {
            if (resourcesFilled)
                return;
            FillResources();
        }

        private void buttonResRefresh_Click(object sender, EventArgs e)
        {
            FillResources();
        }

        private void FillResources()
        {
            treeRes.Nodes.Clear();
            TreeNode mainNode = treeRes.Nodes.Add("Ресурсы");
            
            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();
            List<ResourceType> resList = ResourceType.GetResouceList();
            if(resList.Count > 0)
            {
                foreach(ResourceType res in resList)
                {
                    TreeNodeCollection hostNodes;
                    if(res.ParentId == 0)
                    {
                        hostNodes = mainNode.Nodes;
                    }    
                    else
                    {
                        hostNodes = nodeDict[res.ParentId].Nodes;
                    }
                    TreeNode n = hostNodes.Add(res.Name);
                    n.Tag = res;

                    nodeDict.Add(res.Id, n);

                }
            }


            mainNode.Expand();
            resourcesFilled = true;
        }


        private void buttonResAdd_Click(object sender, EventArgs e)
        {
            int parentId = 0;
            ResourceType res = new ResourceType();
            TreeNodeCollection hostNodes;
            if(treeRes.SelectedNode == null)
            {
                hostNodes = treeRes.Nodes[0].Nodes;
            }
            else if(treeRes.SelectedNode.Tag == null)
            {
                hostNodes = treeRes.Nodes[0].Nodes;
            }
            else
            {
                ResourceType hostRes = (ResourceType)treeRes.SelectedNode.Tag;
                parentId = hostRes.Id;
                hostNodes = treeRes.SelectedNode.Nodes;
            }
            res.Name = "new resource";
            res.ParentId = parentId;
            TreeNode n = hostNodes.Add(res.Name);
            n.Tag = res;
            treeRes.SelectedNode = n;
            
        }

        private void buttonResDelete_Click(object sender, EventArgs e)
        {
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            if(treeRes.SelectedNode.Nodes.Count > 0)
            {
                MessageBox.Show("Нельзя удалять узел, на котором есть другие узлы");
                return;
            }
            if(MessageBox.Show("Удалить данный ресурс?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            res.Delete();
            treeRes.SelectedNode.Parent.Nodes.Remove(treeRes.SelectedNode);
        }

        private ResourceType CurrentRes()
        {
            if (treeRes.SelectedNode == null)
                return null;
            if (treeRes.SelectedNode.Tag == null)
                return null;
            return (ResourceType)treeRes.SelectedNode.Tag;
        }

        private void ClearResource()
        {
            NoEvents = true;
            textResId.Text = "";
            textResName.Text = "";
            textResDecriptionEng.Text = "";
            textResDescriptionRus.Text = "";
            textResImg.Text = "";
            textResUnityName.Text = "";
            NoEvents = false;
        }

        private void treeRes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearResource();
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            NoEvents = true;
            textResId.Text = res.Id.ToString();
            textResName.Text = res.Name;
            textResDecriptionEng.Text = res.DescriptionEng;
            textResDescriptionRus.Text = res.DescriptionRus;
            textResImg.Text = res.ImgId.ToString();
            textResUnityName.Text = res.UnityName;
            NoEvents = false;
        }

        private void textResName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            res.Name = textResName.Text;
            treeRes.SelectedNode.Text = res.Name;
        }
        private void textResDecriptionEng_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            res.DescriptionEng = textResDecriptionEng.Text;
        }
        private void textResDescriptionRus_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            res.DescriptionRus = textResDescriptionRus.Text;
        }
        private void textResImg_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textResImg.Text, out value);
            res.ImgId = value;
        }

        private void textResUnityName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            res.UnityName = textResUnityName.Text;
        }

        private void buttonResSave_Click(object sender, EventArgs e)
        {
            ResourceType res = CurrentRes();
            if (res == null)
                return;
            res.Save();
            textResId.Text = res.Id.ToString();

        }






        #endregion

        #region Blueprints

        private bool BpFilled;

        private void tabPage25_Enter(object sender, EventArgs e)
        {
            if (BpFilled)
                return;
            FillBlueprints();
        }

        private void buttonRefreshBlueprint_Click(object sender, EventArgs e)
        {
            FillBlueprints();
        }

        private void buttonAddBlueprint_Click(object sender, EventArgs e)
        {
            BlueprintType bp = new BlueprintType();
            bp.Name = "new blueprint";
            TreeNode n;
            if(treeBlueprint.SelectedNode == null)
            {
                n = treeBlueprint.Nodes[0].Nodes.Add(bp.Name);
            }
            else if(treeBlueprint.SelectedNode.Tag == null)
            {
                n = treeBlueprint.Nodes[0].Nodes.Add(bp.Name);
            }
            else
            {
                n = treeBlueprint.SelectedNode.Nodes.Add(bp.Name);
                BlueprintType parentBp = (BlueprintType)treeBlueprint.SelectedNode.Tag;
                bp.ParentId = parentBp.Id;
            }
            n.Tag = bp;
            treeBlueprint.SelectedNode = n;
        }

        private void buttonDeleteBlueprint_Click(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            bp.Delete();
            treeBlueprint.SelectedNode.Parent.Nodes.Remove(treeBlueprint.SelectedNode);
        }

        private BlueprintType GetCurrentBlueprint()
        {
            if (treeBlueprint.SelectedNode == null)
                return null;
            if (treeBlueprint.SelectedNode.Tag == null)
                return null;
            return (BlueprintType)treeBlueprint.SelectedNode.Tag;
        }

        private void FillBlueprints()
        {
            treeBlueprint.Nodes.Clear();
            TreeNode mainNode = treeBlueprint.Nodes.Add("Blueprint list");

            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();
            var bpList = BlueprintType.GetList();
            foreach(var bp in bpList)
            {
                TreeNode n;
                if(bp.ParentId > 0)
                {
                    n = nodeDict[bp.ParentId].Nodes.Add(bp.ToString());
                }
                else
                {
                    n = mainNode.Nodes.Add(bp.ToString());
                }
                n.Tag = bp;
                nodeDict.Add(bp.Id, n);
            }

            mainNode.Expand();

            NoEvents = true;
            comboBpResource.Items.Clear();
            var resList = ResourceType.GetResouceList();
            foreach(var res in resList)
            {
                comboBpResource.Items.Add(res);
            }
            NoEvents = false;

            BpFilled = true;
        }

        private void ClearBp()
        {
            NoEvents = true;
            textBpId.Text = "";
            textBpName.Text = "";
            comboBpProductType.SelectedItem = null;
            comboBpProduct.Items.Clear();
            textBpBonus.Text = "";
            textBpFailChance.Text = "";
            textBpProductionPoints.Text = "";
            listBpResources.Items.Clear();
            comboBpResource.SelectedItem = null;
            textBpResourceAmount.Text = "";
            textBpUnityName.Text = "";
            NoEvents = false;
        }

        private void treeBlueprint_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            NoEvents = true;
            textBpId.Text = bp.Id.ToString();
            textBpName.Text = bp.Name;
            comboBpProductType.SelectedIndex = (int)bp.ProductType;
            FillBpProductCombo();
            SetBpComboValue(bp.ProductId);
            textBpBonus.Text = bp.BaseBonus.ToString();
            textBpFailChance.Text = bp.FailChance.ToString();
            textBpProductionPoints.Text = bp.ProductionPoints.ToString();
            textBpUnityName.Text = bp.UnityName;
            listBpResources.Items.Clear();
            if (bp.ResourceList.Count > 0)
            {
                foreach(var res in bp.ResourceList)
                {
                    listBpResources.Items.Add(res);
                }
            }

            NoEvents = false;
        }

        private void textBpName_TextChanged(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            bp.Name = textBpName.Text;
            treeBlueprint.SelectedNode.Text = bp.Name;
        }

        private void comboBpProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillBpProductCombo();
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            if (comboBpProductType.SelectedItem == null)
            {
                bp.ProductType = BlueprintType.BlueprintProductType.None;
                return;
            }
            bp.ProductType = (BlueprintType.BlueprintProductType)comboBpProductType.SelectedIndex;
        }

        private void FillBpProductCombo()
        {
            //None
            //Make spaceship module
            //Make spaceship
            //Make spacestation module
            //Improve officer
            //Improve spaceship module
            //Improve spaceship
            //Improve playerstat
            NoEvents = true;
            comboBpProduct.Items.Clear();
            NoEvents = false;
            if (comboBpProductType.SelectedItem == null)
                return;
            if (comboBpProductType.SelectedIndex == 0)
                return;

            if((string)comboBpProductType.SelectedItem == "Make spaceship module")
            {
                List<ShipModuleType> mTypes = ShipModuleType.CreateList(true);
                if(mTypes.Count > 0)
                {
                    foreach(var mType in mTypes)
                    {
                        comboBpProduct.Items.Add(mType);
                    }
                }
            }

            if((string)comboBpProductType.SelectedItem == "Make spaceship")
            {
                var sModels = ShipModuleType.CreateList(true);
                foreach(var sModel in sModels)
                {
                    comboBpProduct.Items.Add(sModel);
                }
            }

            if((string)comboBpProductType.SelectedItem == "Make spacestation module")
            {

            }

        }

        private void SetBpComboValue(int value)
        {
            if (value == 0)
                return;
            if(comboBpProduct.Items.Count > 0)
            {
                Type type = comboBpProduct.Items[0].GetType();
                PropertyInfo propInfo = type.GetProperty("Id");
                foreach (var item in comboBpProduct.Items)
                {
                    int id = (int)propInfo.GetValue(item);
                    if (id == value)
                    {
                        comboBpProduct.SelectedItem = item;
                        return;
                    }
                }
            }
        }

        private void comboBpProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            if (comboBpProduct.SelectedItem == null)
            {
                bp.ProductId = 0;
                return;
            }
            Type type = comboBpProduct.SelectedItem.GetType();
            PropertyInfo propInfo = type.GetProperty("Id");
            bp.ProductId = (int)propInfo.GetValue(comboBpProduct.SelectedItem);
        }

        private void textBpBonus_TextChanged(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            int value = 0;
            Int32.TryParse(textBpBonus.Text, out value);
            bp.BaseBonus = value;
        }
        private void textBpFailChance_TextChanged(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            int value = 0;
            Int32.TryParse(textBpFailChance.Text, out value);
            bp.FailChance = value;
        }

        private void textBpProductionPoints_TextChanged(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            int value = 0;
            Int32.TryParse(textBpProductionPoints.Text, out value);
            bp.ProductionPoints = value;
        }

        private void textBpUnityName_TextChanged(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            bp.UnityName = textBpUnityName.Text;
        }

        private void buttonBpSave_Click(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            bp.Save();
            textBpId.Text = bp.Id.ToString();
        }

        private void buttonAddBpResource_Click(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            var curResource = bp.AddResource();
            listBpResources.Items.Add(curResource);
            listBpResources.SelectedItem = curResource;
        }

        private void buttonDeleteBpResource_Click(object sender, EventArgs e)
        {
            BlueprintType bp = GetCurrentBlueprint();
            if (bp == null)
                return;
            var res = GetCurrentBpResource();
            if (res == null)
                return;
            res.Delete();
            listBpResources.Items.Remove(listBpResources.SelectedItem);
        }

        private ResourceForBlueprint GetCurrentBpResource()
        {
            if (listBpResources.SelectedItem == null)
                return null;
            return (ResourceForBlueprint)listBpResources.SelectedItem;
        }

        private void listBpResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            NoEvents = true;
            comboBpResource.SelectedItem = null;
            textBpResourceAmount.Text = "";
            NoEvents = false;

            var res = GetCurrentBpResource();
            if (res == null)
                return;

            if(res.ResourceTypeId > 0)
            { 
                foreach(var item in comboBpResource.Items)
                {
                    var curResource = (ResourceType)item;
                    if(curResource.Id == res.ResourceTypeId)
                    {
                        comboBpResource.SelectedItem = item;
                        break;
                    }
                }
            }
            textBpResourceAmount.Text = res.Amount.ToString();

        }

        private void comboBpResource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            var res = GetCurrentBpResource();
            if (res == null)
                return;
            if(comboBpResource.SelectedItem == null)
            {
                res.ResourceTypeId = 0;
                return;
            }
            var bpResType = (ResourceType)comboBpResource.SelectedItem;
            res.ResourceTypeId = bpResType.Id;
            NoEvents = true;
            listBpResources.Items[listBpResources.SelectedIndex] = res;
            NoEvents = false;
        }
        private void textBpResourceAmount_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            var res = GetCurrentBpResource();
            if (res == null)
                return;
            int value = 0;
            Int32.TryParse(textBpResourceAmount.Text, out value);
            res.Amount = value;
            NoEvents = true;
            listBpResources.Items[listBpResources.SelectedIndex] = res;
            NoEvents = false;
        }

        #endregion

        #region "Battle scene test"

        private bool BstFilled;

        private void tabPage27_Click(object sender, EventArgs e)
        {

        }
        private void tabPage27_Enter(object sender, EventArgs e)
        {
            if (BstFilled)
                return;
            FillBst();
        }
        private void buttonBstRefresh_Click(object sender, EventArgs e)
        {
            FillBst();
        }
        private void FillBst()
        {
            comboBst.Items.Clear();
            var bsList = BattleSceneType.SceneList();
            if(bsList.Count > 0 )
            {
                foreach(var bs in bsList)
                {
                    comboBst.Items.Add(bs);
                }
            }
            BstFilled = true;
        }
        private void buttonBstCreate_Click(object sender, EventArgs e)
        {
            treeBst.Nodes.Clear();
            if (comboBst.SelectedItem == null)
                return;
            BattleSceneType sType = (BattleSceneType)comboBst.SelectedItem;
            BattleScene scene = new BattleScene(sType);
            foreach(var cycle in scene.Cycles)
            {
                TreeNode cNode = treeBst.Nodes.Add("Cycle " + cycle.Number.ToString());
                cNode.Tag = cycle;
                foreach(var stage in cycle.Stages)
                {
                    TreeNode sNode = cNode.Nodes.Add("Stage " + stage.StageNumber.ToString());
                    sNode.Tag = stage;
                    foreach(var enemy in stage.Enemy)
                    {
                        TreeNode eNode = sNode.Nodes.Add(enemy.ToString());
                        eNode.Tag = enemy;
                    }
                }
            }
        }

        private void buttonBstTestCompressed_Click(object sender, EventArgs e)
        {
            treeBst.Nodes.Clear();
            if (comboBst.SelectedItem == null)
                return;
            BattleSceneType sType = (BattleSceneType)comboBst.SelectedItem;
            BattleScene scene = new BattleScene(sType);

        }

        private void treeBst_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TextBsEnemies.Text = "";
            textBsResources.Text = "";

            if (treeBst.SelectedNode == null)
                return;

            if (treeBst.SelectedNode.Tag == null)
                return;

            if(treeBst.SelectedNode.Tag.GetType() == typeof(BattleScene.Stage))
            {
                BattleScene.Stage curStage = (BattleScene.Stage)treeBst.SelectedNode.Tag;
                BattleScene.BsStatictics stat = new BattleScene.BsStatictics();
                stat.AddStage(curStage);
                TextBsEnemies.Text = stat.EnemiesString();
                textBsResources.Text = stat.ResourcesString();
            }

            if(treeBst.SelectedNode.Tag.GetType() == typeof(BattleScene.Cycle))
            {
                BattleScene.Cycle cycle = (BattleScene.Cycle)treeBst.SelectedNode.Tag;
                BattleScene.BsStatictics stat = new BattleScene.BsStatictics();
                foreach(var stage in cycle.Stages)
                {
                    stat.AddStage(stage);
                }
                TextBsEnemies.Text = stat.EnemiesString();
                textBsResources.Text = stat.ResourcesString();
            }

            if (treeBst.SelectedNode.Tag.GetType() == typeof(StageEnemy))
            {
                StageEnemy enemy = (StageEnemy)treeBst.SelectedNode.Tag;
                BattleScene.BsStatictics stat = new BattleScene.BsStatictics();
                stat.AddEnemy(enemy);
                TextBsEnemies.Text = stat.EnemiesString();
                textBsResources.Text = stat.ResourcesString();
            }

        }


        /*private void button1_Click(object sender, EventArgs e)
        {
            AccountData player = GetLatestUser();
            var bst = BattleSceneType.SceneById(4);
            var bs = new BattleScene(bst, player);
            var cbs = new CompressedBattleScene(bs);
            string ser = JsonConvert.SerializeObject(cbs);
            ser = CommonFunctions.Compress(ser);
            TextBsEnemies.Text = $@"Lenght: {ser.Length.ToString()}
{ser}";

            string deser = CommonFunctions.Decompress(ser);


            var ex2 = JsonConvert.DeserializeObject<CompressedBattleScene>(deser);


        }*/

            private void buttonBstSave_Click(object sender, EventArgs e)
        {

            BattleSceneType sType = (BattleSceneType)comboBst.SelectedItem;
            BattleScene scene = new BattleScene(sType);
            UnityBattleScene Bst = new UnityBattleScene(scene);
            string strCbs = JsonConvert.SerializeObject(Bst);
            strCbs = CommonFunctions.Compress(strCbs);
            System.IO.File.WriteAllText("battle scene.dat", strCbs);
            Process.Start(Directory.GetCurrentDirectory());
        }














        #endregion

    }
}
