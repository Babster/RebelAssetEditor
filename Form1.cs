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

namespace AssetEditor
{
    public partial class Form1 : Form
    {
        private bool NoEvents;

        public Form1()
        {
            NoEvents = false;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoadScenes();

            FillImages();

        }

        #region "Сцены"

        TreeNode nodeScenes;

        private void LoadScenes()
        {
            nodeScenes = treeScenes.Nodes.Add("Сцены");
            SceneNodeTag tScenes = new SceneNodeTag();
            tScenes.NodeType = "Сцены";
            nodeScenes.Tag = tScenes;

            SqlDataReader r = DataConnection.GetReader(
                @"SELECT 
                    id, 
                    name,
                    active, 
                    ISNULL(backgound_image_id, '') AS backgound_image_id
                FROM 
                    story_scenes");
            if (r.HasRows)
            {
                while (r.Read())
                {
                    SceneNodeTag nTag = new SceneNodeTag(ref r);
                    
                    TreeNode nNode = nodeScenes.Nodes.Add(nTag.Name);
                    nNode.Tag = nTag;
                    nTag.Node = nNode;

                }
            }
            r.Close();
        }

        private void treeScenes_AfterSelect(object sender, TreeViewEventArgs e)
        {

            ClearScene();

            if (e.Node == null)
            {
                return;
            }

            SceneNodeTag tTag = (SceneNodeTag)e.Node.Tag;

            if (tTag.NodeType == "Сцены")
            {
            }

            if (tTag.NodeType == "Сцена")
            {
                NoEvents = true;
                textSceneId.Text = tTag.Id.ToString();
                textSceneName.Text = tTag.Name;
                textSceneBackgroundId.Text = tTag.BackgroundImageId.ToString();
                groupBox1.Enabled = true;

                foreach(SceneElement curScene in tTag.Elements )
                {
                    listSceneElements.Items.Add(curScene);
                }

                NoEvents = false;

            }
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

 

        private class SceneNodeTag : RebelSceneWithSql
        { 
            public string NodeType { get; set; }
            public TreeNode Node { get; set; }

            public SceneNodeTag() : base()
            {
                this.NodeType = "Сцена";
            }

            public SceneNodeTag(ref SqlDataReader r) : base(ref r)
            {
                this.NodeType = "Сцена";
            }

        }

        private void buttonAddNode_Click(object sender, EventArgs e)
        {
            SceneNodeTag nTag = new SceneNodeTag();

            TreeNode nNode = nodeScenes.Nodes.Add(nTag.Name);
            nNode.Tag = nTag;
            nTag.Node = nNode;
            treeScenes.SelectedNode = nNode;

        }

        private void buttonSaveScene_Click(object sender, EventArgs e)
        {
            if (treeScenes.SelectedNode == null)
            {
                MessageBox.Show("Не выбрана сцена для сохранения");
                return;
            }

            SceneNodeTag tTag = (SceneNodeTag)treeScenes.SelectedNode.Tag;
            if (tTag.NodeType == "Сцены")
            {
                MessageBox.Show("Не выбрана сцена для сохранения");
                return;
            }

            tTag.SaveData();

        }

        private void textSceneName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            if (treeScenes.SelectedNode == null)
            {
                MessageBox.Show("Не выбрана сцена для ввода наименования");
                return;
            }

            SceneNodeTag tTag = (SceneNodeTag)treeScenes.SelectedNode.Tag;
            if (tTag.NodeType == "Сцены")
            {
                MessageBox.Show("Не выбрана сцена для ввода наименования");
                return;
            }

            tTag.Node.Text = textSceneName.Text;
            tTag.Name = textSceneName.Text;

        }


        private void textSceneBackgroundId_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            if (treeScenes.SelectedNode == null)
            {
                MessageBox.Show("Не выбрана сцена");
                return;
            }

            SceneNodeTag tTag = (SceneNodeTag)treeScenes.SelectedNode.Tag;
            if (tTag.NodeType == "Сцены")
            {
                MessageBox.Show("Не выбрана сцена");
                return;
            }
            int tId = 0;
            if (Int32.TryParse(textSceneBackgroundId.Text, out tId) == true)
                tTag.BackgroundImageId = tId;
        }

        private void buttonAddSceneElement_Click(object sender, EventArgs e)
        {
            SceneNodeTag tTag = (SceneNodeTag)treeScenes.SelectedNode.Tag;
            listSceneElements.Items.Add(tTag.AddElement());
            listSceneElements.SelectedIndex = listSceneElements.Items.Count - 1;
        }

        private void listSceneElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;

            ClearSceneElement();

            if (listSceneElements.SelectedItem == null)
                return;

            SceneElement curElement = (SceneElement ) listSceneElements.SelectedItem;

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
            curElement.TextRussian = textSceneRussian.Text ;
            NoEvents = true;
            listSceneElements.Items[listSceneElements.SelectedIndex] = curElement ;
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
            curElement.NextScreen  = checkNextScreen.Checked;
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
            while(r.Read())
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
                if(treeImages.SelectedNode.Tag == null)
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
                if (cTag.IsPartition )
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

            if(tTag.IsPartition )
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

            if (e.KeyData == Keys.Enter )
            {
                treeImages.SelectedNode.Text = textImagePartition.Text;
                ImageTag tTag = (ImageTag) treeImages.SelectedNode.Tag;
                tTag.Name = textImagePartition.Text;

                if(treeImages.SelectedNode.Nodes.Count > 0)
                {
                    string ids = "";
                    foreach(TreeNode curNode in treeImages.SelectedNode.Nodes )
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

            openFileDialog1.Filter = "*.jpg|*.jpg";
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

        #endregion

        #region Статы адмиралов

        private int statStartPointsId;

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private bool StatsFilled = false;

        private void tabPage5_Enter(object sender, EventArgs e)
        {

            if (StatsFilled)
                return;

            string q;
            SqlDataReader r;

            q = @"SELECT 
                    id,
                    ISNULL(skill_group, '') AS skill_group,
                    name,
                    base_value,
                    description_english,
                    description_russian,
                    order_idx
                FROM
                    admiral_stat_types
                ORDER BY
                    order_idx";
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    PlayerStatNode tStat = new PlayerStatNode(ref r);
                    TreeNode tNode = treeStats.Nodes.Add(tStat.Name);
                    
                    tNode.Tag = tStat;
                    tStat.Node = tNode;
                }
            }
            r.Close();

            q = "SELECT id, value_int FROM s_common_values WHERE name = 'start_stat_points'";
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                r.Read();
                NoEvents = true;
                textStatRegistrationPoints.Text = Convert.ToString(r["value_int"]);
                NoEvents = false;
                statStartPointsId = Convert.ToInt32(r["id"]);
            }
            r.Close();

            if (statStartPointsId == 0)
            {
                q = @"INSERT INTO s_common_values(name, value_int) VALUES ('start_stat_points', 0)
                    SELECT @@IDENTITY AS field0";
                statStartPointsId = Convert.ToInt32(DataConnection.GetResult(q));
                NoEvents = true;
                textStatRegistrationPoints.Text = "0";
                NoEvents = false;
            }
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

        private void treeStats_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearStat();
            if (treeStats.SelectedNode == null)
                return;

            NoEvents = true;

            PlayerStatNode tNode = (PlayerStatNode) treeStats.SelectedNode.Tag;
            textStatId.Text = tNode.Id.ToString();
            if(!string.IsNullOrEmpty(tNode.SkillGroup))
            {
                comboSkillGroup.SelectedItem = tNode.SkillGroup;
            }
            
            textStatName.Text = tNode.Name;
            textStatBaseValue.Text = tNode.Value.ToString();
            textStatDescriptionEnglish.Text = tNode.DescriptionEnglish;
            textStatDescriptionRussian.Text = tNode.DescriptionRussian;
            textStatSortIdx.Text = tNode.OrderIdx.ToString();

            NoEvents = false;

        }

        private void buttonCreateStat_Click(object sender, EventArgs e)
        {
            TreeNode tNode = treeStats.Nodes.Add("new stat");
            PlayerStatNode tTag = new PlayerStatNode();
            tNode.Tag = tTag;
            tTag.Name = "new stat";
            tTag.Node = tNode;
            treeStats.SelectedNode = tNode;
        }

        private void textStatName_TextChanged(object sender, EventArgs e)
        {
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
            treeStats.SelectedNode.Text = textStatName.Text;
            tNode.Name = textStatName.Text;
        }

        private void comboSkillGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
            if(comboSkillGroup.SelectedItem == null)
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
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
            try
            {
                tNode.Value = Convert.ToInt32(textStatBaseValue.Text);
            }
            catch
            {
                tNode.Value = 0;
            }
        }

        private void textStatSortIdx_TextChanged(object sender, EventArgs e)
        {
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
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
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
            tNode.DescriptionEnglish = textStatDescriptionEnglish.Text;
        }

        private void textStatDescriptionRussian_TextChanged(object sender, EventArgs e)
        {
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
            tNode.DescriptionRussian = textStatDescriptionRussian.Text;
        }

        private void buttonSaveStat_Click(object sender, EventArgs e)
        {
            if (treeStats.SelectedNode == null)
                return;
            if (NoEvents)
                return;
            PlayerStatNode tNode = (PlayerStatNode)treeStats.SelectedNode.Tag;
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
            q = "UPDATE s_common_values SET value_int = " + NewValue.ToString() + " WHERE id = " + statStartPointsId.ToString();
            DataConnection.Execute(q);

        }

        private class PlayerStatNode : PlayerStat
        {

            public TreeNode Node { get; set; }

            public PlayerStatNode()
            {

            }

            public PlayerStatNode(ref SqlDataReader r) : base(ref r)
            {

            }


        }


        #endregion

        #region "Тестирование"

        private void buttonDeleteAccount_Click(object sender, EventArgs e)
        {
            string Query = @"SELECT TOP 1 id, steam_account_id, pwd FROM admirals";
            SqlDataReader r = DataConnection.GetReader(Query);
            if (r.HasRows == false)
            {
                r.Close();
                MessageBox.Show("Нечего удалять");
                return;
            }

            r.Read();

            AccountData tUser = new AccountData();
            tUser.Id = Convert.ToInt32(r["id"]);
            tUser.Name = Convert.ToString(r["steam_account_id"]);
            tUser.AdditionalData = Convert.ToString(r["pwd"]);

            r.Close();

            AdmiralsMain.DeleteAccount(ref tUser);

            MessageBox.Show("Аккаунт " + tUser.Name + "(" + tUser.Id.ToString() + ") удален");

        }

        private void buttonRegisterAccount_Click(object sender, EventArgs e)
        {
            AccountData tUser = new AccountData();
            tUser.SteamAccountId = "Babster";
            AdmiralsMain.RegisterAccount(ref tUser);
            MessageBox.Show("Аккаунт " + tUser.Name + "(" + tUser.Id.ToString() + ") зарегистрирован");
        }
        private void buttonTestPlayerStats_Click(object sender, EventArgs e)
        {

            AccountData tData = GetLatestUser();
            AdmiralStats tStats = new AdmiralStats(ref tData);


        }

        private AccountData GetLatestUser()
        {
            string q = "SELECT MAX(id) AS max_id FROM admirals";
            int admiralId = Convert.ToInt32(DataConnection.GetResult(q));

            AccountData tData = new AccountData(admiralId);
            return tData;

        }



        #endregion

        #region История - какой объект за каким следует
        
        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void tabPage7_Enter(object sender, EventArgs e)
        {
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
        }

        private Dictionary<string, string> storyObjectDisct;
        private void GridStoryFlow_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if(storyObjectDisct==null)
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
            if(row.Cells["s_id"].Value!=null)
            {
                if(Convert.ToString(row.Cells["s_id"].Value) != "")
                {
                    id = Convert.ToInt32(row.Cells["s_id"].Value);
                }
            }
            if(id==0)
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
            DataConnection.Execute(q,names);
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
        }

        private void FillModules()
        {
            treeModules.Nodes.Clear();
            string q = ModuleNodeTag.ModuleQuery();

            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();

            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    ModuleNodeTag tag = new ModuleNodeTag(ref r);
                    TreeNode n;
                    if(tag.Parent == 0)
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
            r.Close();

            if (moduleTabDict == null)
            {
                moduleTabDict = new Dictionary<string, TabPage>();
                moduleTabDict.Add("Weapon", tabWeapon);
                moduleTabDict.Add("Defence", tabDefence);
                moduleTabDict.Add("Engine", tabEngine);
                moduleTabDict.Add("Thrusters", tabThrusters);

                moduleTypeDict = new Dictionary<TabPage, string>();
                moduleTypeDict.Add(tabWeapon, "Weapon");
                moduleTypeDict.Add(tabDefence, "Defence");
                moduleTypeDict.Add(tabEngine, "Engine");
                moduleTypeDict.Add(tabThrusters, "Thrusters");

            }

            modulesFilled = true;

        }

        private class ModuleNodeTag
        {
            public int Id { get; set; }
            public int IsCategory { get; set; }
            public int Parent { get; set; }
            public string Name { get; set; }
            public string AssetName { get; set; }
            public string ModuleType { get; set; }
            public int EnergyNeeded { get; set; }
            public int MainScore { get; set; }
            public int SecondaryScore { get; set; }
            public int ThirdScore { get; set; }
            
            public enum EnumModuleType
            {
                None = 0,
                Weapon = 1,
                Defence = 2,
                Engine = 3,
                Thrusters = 4
            }

            public EnumModuleType eType()
            {
                EnumModuleType tType;
                switch (Name)
                {
                    case "Weapon":
                        tType = EnumModuleType.Weapon;
                        break;
                    case "Defence":
                        tType = EnumModuleType.Defence;
                        break;
                    case "Engine":
                        tType = EnumModuleType.Engine;
                        break;
                    case "Thrusters":
                        tType = EnumModuleType.Thrusters;
                        break;
                    default:
                        tType = EnumModuleType.None;
                        break;
                }
                return tType;
            }

            public ModuleNodeTag(int parentId, int isCategory) 
            { 
                this.Parent = parentId;
                this.IsCategory = isCategory;
                if(IsCategory == 1)
                {
                    this.Name = "New category";
                }
                else
                {
                    this.Name = "New module";
                }
            }

            public ModuleNodeTag(ref SqlDataReader r)
            {
                this.Id = Convert.ToInt32(r["id"]);
                this.IsCategory = Convert.ToInt32(r["is_category"]);
                this.Parent = Convert.ToInt32(r["parent"]);
                this.Name = Convert.ToString(r["name"]);
                this.AssetName = Convert.ToString(r["asset_name"]);
                this.ModuleType = Convert.ToString(r["module_type"]);
                this.EnergyNeeded = Convert.ToInt32(r["energy_needed"]);
                this.MainScore = Convert.ToInt32(r["main_score"]);
                this.SecondaryScore = Convert.ToInt32(r["secondary_score"]);
                this.ThirdScore = Convert.ToInt32(r["third_score"]);
            }

            public void SaveData()
            {
                string q;
                if(this.Id==0)
                {
                    q = @"INSERT INTO ss_modules(parent) VALUES(" + this.Parent.ToString() + @")
                            SELECT @@IDENTITY AS Result";
                    this.Id = Convert.ToInt32(DataConnection.GetResult(q));
                        
                }
                q = @"UPDATE ss_modules SET 
                        is_category = " + this.IsCategory.ToString() + @",
                        parent = " + this.Parent.ToString() + @",
                        name = @str1,
                        asset_name = @str2,
                        module_type = @str3,
                        energy_needed = " + this.EnergyNeeded + @",
                        main_score = " + this.MainScore + @",
                        secondary_score = " + this.SecondaryScore + @",
                        third_score = " + this.ThirdScore + @"
                    WHERE id = " + this.Id.ToString();

                List<string> names = new List<string>();
                names.Add(this.Name);
                names.Add(this.AssetName);
                names.Add(this.ModuleType);

                DataConnection.Execute(q, names);

            }

            public static Dictionary<int, string> moduleNames()
            {
                string q;
                SqlDataReader r;
                Dictionary<int, string> ModuleDict = new Dictionary<int, string>();
                q = @"SELECT id, name FROM ss_modules";
                r = DataConnection.GetReader(q);
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        ModuleDict.Add(Convert.ToInt32(r["id"]), Convert.ToString(r["name"]));
                    }
                }
                r.Close();
                return ModuleDict;
            }

            public static Dictionary<int, ModuleNodeTag> CreateModuleDict()
            {

                Dictionary<int, ModuleNodeTag> tags = new Dictionary<int, ModuleNodeTag>();

                string q = ModuleQuery();
                SqlDataReader r = DataConnection.GetReader(q);
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        ModuleNodeTag tag = new ModuleNodeTag(ref r);
                        tags.Add(tag.Id, tag);
                    }
                }
                r.Close();
                return tags;
            }

            public static string ModuleQuery()
            {
                string q = @"
                SELECT 
                    id,
                    is_category,
                    parent,
                    name,
                    asset_name,
                    module_type,
                    energy_needed,
                    main_score,
                    secondary_score,
                    third_score
                FROM
                    ss_modules
                --WHERE
                --    parent = 0
                    ";
                return q;
            }

            #region Specific modules properties

            public enum enumSpecificProperty
            {
                FireRate = 1,
                DeflectorsDamage = 2,
                StructureDamage = 3,
                Deflectors = 4,
                DeflectorsRegen = 5,
                Armor = 6,
                Engine = 7,
                ThrustersSpeed = 8,
                ThrustersDexterity = 9
            }

            public int SpecificPropertiesCount() { return 9; }

            public String PropertyToString(enumSpecificProperty prop)
            {
                switch(prop)
                {
                    case enumSpecificProperty.FireRate:
                        return "Fire rate";
                    case enumSpecificProperty.DeflectorsDamage:
                        return "Deflectors damage";
                    case enumSpecificProperty.StructureDamage:
                        return "Structure damage";
                    case enumSpecificProperty.Deflectors:
                        return "Deflectors";
                    case enumSpecificProperty.DeflectorsRegen:
                        return "Deflectors regen";
                    case enumSpecificProperty.Armor:
                        return "Armor";
                    case enumSpecificProperty.Engine:
                        return "Engine";
                    case enumSpecificProperty.ThrustersSpeed:
                        return "Speed";
                    case enumSpecificProperty.ThrustersDexterity:
                        return "Dexterity";
                    default:
                        return "error: unknown module type";
                }
            }

            public int PropertyValue(enumSpecificProperty prop)
            {
                switch (prop)
                {
                    case enumSpecificProperty.FireRate:
                        return FireRate();
                    case enumSpecificProperty.DeflectorsDamage:
                        return DeflectorsDamage();
                    case enumSpecificProperty.StructureDamage:
                        return StructureDamage();
                    case enumSpecificProperty.Deflectors:
                        return Deflectors();
                    case enumSpecificProperty.DeflectorsRegen:
                        return DeflectorsRegen();
                    case enumSpecificProperty.Armor:
                        return Armor();
                    case enumSpecificProperty.Engine:
                        return Engine();
                    case enumSpecificProperty.ThrustersSpeed:
                        return ThrustersSpeed();
                    case enumSpecificProperty.ThrustersDexterity:
                        return ThrustersDexterity();
                    default:
                        return 0;
                }
            }

            public int FireRate()
            {
                if (eType() == EnumModuleType.Weapon)
                {
                    return this.MainScore;
                }
                else
                {
                    return 0;
                }
            }

            public int DeflectorsDamage()
            {
                if (eType() == EnumModuleType.Weapon)
                {
                    return this.SecondaryScore;
                }
                else
                {
                    return 0;
                }
            }

            public int StructureDamage()
            {
                if (eType() == EnumModuleType.Weapon)
                {
                    return this.ThirdScore;
                }
                else
                {
                    return 0;
                }
            }

            public int Deflectors()
            {
                if (eType() == EnumModuleType.Defence)
                {
                    return this.MainScore;
                }
                else
                {
                    return 0;
                }
            }

            public int DeflectorsRegen()
            {
                if (eType() == EnumModuleType.Defence)
                {
                    return this.SecondaryScore;
                }
                else
                {
                    return 0;
                }
            }

            public int Armor()
            {
                if (eType() == EnumModuleType.Defence)
                {
                    return this.ThirdScore;
                }
                else
                {
                    return 0;
                }
            }

            public int Engine()
            {
                if (eType() == EnumModuleType.Engine)
                {
                    return this.ThirdScore;
                }
                else
                {
                    return 0;
                }
            }

            public int ThrustersSpeed()
            {
                if (eType() == EnumModuleType.Thrusters )
                {
                    return this.MainScore;
                }
                else
                {
                    return 0;
                }
            }

            public int ThrustersDexterity()
            {
                if (eType() == EnumModuleType.Thrusters)
                {
                    return this.SecondaryScore;
                }
                else
                {
                    return 0;
                }
            }

            #endregion 

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
                ModuleNodeTag tTag = (ModuleNodeTag)treeModules.SelectedNode.Tag;
                parentTag = tTag.Id;

            }

            ModuleNodeTag tag = new ModuleNodeTag(parentTag, isCategory);
            newNode = nodesCollection.Add(tag.Name);
            newNode.Tag = tag;
            treeModules.SelectedNode = newNode;
        }

        private Dictionary<string, TabPage> moduleTabDict;
        private Dictionary<TabPage, string> moduleTypeDict;

        private void treeModules_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            ClearModule();

            NoEvents = true;

            textModuleId.Text = tag.Id.ToString();
            textName.Text = tag.Name;
            textModuleUnity.Text = tag.AssetName;
            textModuleType.Text = tag.ModuleType;
            textModuleEnergy.Text = tag.EnergyNeeded.ToString();

            if(tag.IsCategory==1)
            {
                NoEvents = false;
                return;
            }

            if (string.IsNullOrEmpty(tag.ModuleType))
                tag.ModuleType = "Weapon";
            
            NoEvents = false;

            tabControlModule.SelectedTab = moduleTabDict[tag.ModuleType];
            FillModuleTab();
        }

        private void ClearModule()
        {
            NoEvents = true;
            textName.Text = "";
            textModuleUnity.Text = "";
            textModuleEnergy.Text = "";
            textModuleType.Text = "";
            NoEvents = false;
            ClearModuleTabs();
        }

        private void ClearModuleTabs()
        {
            NoEvents = true;

            textModuleFireRate.Text = "";
            textModuleDeflectorDamage.Text = "";
            textModuleStructureDamage.Text = "";

            textModuleDeflectors.Text = "";
            textModuleDeflectorsRegen.Text = "";
            textModuleArmor.Text = "";

            textModuleEngine.Text = "";

            textModuleSpeed.Text = "";
            textDexterity.Text = "";

            NoEvents = false;
        }

        private ModuleNodeTag GetModuleTag()
        {
            if (treeModules.SelectedNode == null)
                return null;

            ModuleNodeTag tag = (ModuleNodeTag)treeModules.SelectedNode.Tag;
            return tag;
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            tag.Name = textName.Text;
            treeModules.SelectedNode.Text = textName.Text;
        }

        private void textModuleUnity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            tag.AssetName  = textModuleUnity.Text;
        }

        private void textModuleEnergy_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleEnergy.Text, out tNumber);
            tag.EnergyNeeded = tNumber;
        }

        private void tabControlModule_Selected(object sender, TabControlEventArgs e)
        {
            FillModuleTab();
        }

        private void FillModuleTab()
        {
            textModuleType.Text = moduleTypeDict[tabControlModule.SelectedTab];
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            tag.ModuleType = textModuleType.Text;

            ClearModuleTabs();

            NoEvents = true;

            switch (tag.ModuleType)
            {
                case "Weapon":
                    textModuleFireRate.Text = tag.MainScore.ToString();
                    textModuleDeflectorDamage.Text = tag.SecondaryScore.ToString();
                    textModuleStructureDamage.Text = tag.ThirdScore.ToString();
                    break;
                case "Defence":
                    textModuleDeflectors.Text = tag.MainScore.ToString();
                    textModuleDeflectorsRegen.Text = tag.SecondaryScore.ToString();
                    textModuleArmor.Text = tag.ThirdScore.ToString();
                    break;
                case "Engine":
                    textModuleEngine.Text = tag.MainScore.ToString();
                    break;
                case "Thrusters":
                    textModuleSpeed.Text = tag.MainScore.ToString();
                    textDexterity.Text = tag.SecondaryScore.ToString();
                    break;
            }
            NoEvents = false;
        }

        private void buttonSaveModule_Click(object sender, EventArgs e)
        {
            textModuleType.Text = moduleTypeDict[tabControlModule.SelectedTab];
            ModuleNodeTag tag = GetModuleTag();
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
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleFireRate.Text, out tNumber);
            tag.MainScore = tNumber;
        }

        private void textModuleDeflectorDamage_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDeflectorDamage.Text, out tNumber);
            tag.SecondaryScore = tNumber;
        }

        private void textModuleStructureDamage_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleStructureDamage.Text, out tNumber);
            tag.ThirdScore = tNumber;
        }

        private void textModuleDeflectors_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDeflectors.Text, out tNumber);
            tag.MainScore = tNumber;
        }

        private void textModuleDeflectorsRegen_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDeflectorsRegen.Text, out tNumber);
            tag.SecondaryScore = tNumber;
        }

        private void textModuleArmor_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleArmor.Text, out tNumber);
            tag.ThirdScore = tNumber;
        }

        private void textModuleEngine_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleEngine.Text, out tNumber);
            tag.MainScore = tNumber;
        }

        private void textModuleSpeed_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleSpeed.Text, out tNumber);
            tag.MainScore = tNumber;
        }

        private void textDexterity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ModuleNodeTag tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textDexterity.Text, out tNumber);
            tag.SecondaryScore = tNumber;
        }



        #endregion

        #endregion


        /// <summary>
        /// Ship designs!!!!
        /// </summary>
        #region Ship designs

        private bool ShipFilled;
        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private void tabPage9_Enter(object sender, EventArgs e)
        {
            if (ShipFilled)
                return;
            FillShips();
        }

        private Dictionary<int, string> ModuleDict;

        private void FillShips()
        {

            treeShips.Nodes.Clear();
            ShipFilled = true;
            string q = @"
                SELECT 
                    id,
                    parent,
                    ISNULL(intensity_amount, 0) AS intensity_amount,
                    ISNULL(base_structure_hp, 0) AS base_structure_hp,
                    name,
                    asset_name
                FROM
                    ss_designs";

            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
                while(r.Read())
                {
                    ShipNodeTag tag = new ShipNodeTag(ref r);
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

        }

        private void buttonReloadModuleDict_Click(object sender, EventArgs e)
        {
            LoadModuleDict();
        }

        private void LoadModuleDict()
        {
            ModuleDict = ModuleNodeTag.moduleNames();
        }

        private void buttonShipAdd_Click(object sender, EventArgs e)
        {
            int parentId;
            TreeNode n;
            if(treeShips.SelectedNode == null)
            {
                parentId = 0;
                n = treeShips.Nodes.Add("");
            }
            else
            {
                ShipNodeTag tTag = (ShipNodeTag)treeShips.SelectedNode.Tag;
                parentId = tTag.Id;
                n = treeShips.SelectedNode.Nodes.Add("");
            }
            ShipNodeTag tag = new ShipNodeTag(parentId);
            n.Text = tag.Name;
            n.Tag = tag;
            treeShips.SelectedNode = n;
        }

        private void treeShips_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearShip();
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            NoEvents = true;
            textShipId.Text = tag.Id.ToString();
            textShipBaseStructure.Text = tag.BaseStructureHp.ToString();
            textBattleIntensity.Text = tag.BattleIntensity.ToString();
            textShipName.Text = tag.Name;
            textShipUnity.Text = tag.AssetName;
            if(tag.slots.Count > 0)
            {
                foreach(ShipNodeTag.Slot slot in tag.slots)
                {
                    listShipSlots.Items.Add(slot);
                }
            }
            NoEvents = false;
            if(listShipSlots.Items.Count > 0)
                listShipSlots.SelectedIndex = 0;
        }

        private void ClearShip()
        {
            NoEvents = true;
            textShipId.Text = "";
            textShipName.Text = "";
            textShipUnity.Text = "";
            listShipSlots.Items.Clear();
            NoEvents = false;
            ClearShipSlot();
        }

        private ShipNodeTag GetCurrentShipTag()
        {
            if (treeShips.SelectedNode == null)
                return null;
            return (ShipNodeTag)treeShips.SelectedNode.Tag;
        }
        private void textShipName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            treeShips.SelectedNode.Text = textShipName.Text;
            tag.Name = textShipName.Text;
        }
        private void textShipUnity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            tag.AssetName  = textShipUnity.Text;
        }
        private void textShipBaseStructure_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipNodeTag tag = GetCurrentShipTag();
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
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            int amount = 0;
            Int32.TryParse(textBattleIntensity.Text, out amount);
            tag.BattleIntensity = amount;
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

            ShipNodeTag.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;

            NoEvents = true;
            textShipSlotId.Text = slot.Id.ToString();
            textShipSlotNumber.Text = slot.SlotNumber.ToString();
            if (!string.IsNullOrEmpty(slot.SlotType))
                comboShipSlotType.SelectedItem = slot.SlotType;
            textShipSlotDefaultModule.Text = slot.DefaultModuleId.ToString();
            ShowDefaultSlotName();
            NoEvents = false;

        }

        private void ClearShipSlot()
        {
            NoEvents = true;
            textShipSlotId.Text = "";
            textShipSlotNumber.Text = "";
            comboShipSlotType.SelectedItem = null;
            textShipSlotDefaultModule.Text = "";
            textShipSlotDefaultModuleName.Text = "";
            NoEvents = false;
        }
        private ShipNodeTag.Slot GetCurrentShipSlot()
        {
            if (listShipSlots.SelectedItem == null)
                return null;
            return (ShipNodeTag.Slot)listShipSlots.SelectedItem;
        }

        private void textShipSlotNumber_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipNodeTag.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            int amount = 0;
            Int32.TryParse(textShipSlotNumber.Text, out amount);
            slot.SlotNumber = amount;
            listShipSlots.Items[listShipSlots.SelectedIndex] = listShipSlots.SelectedItem;
        }
        private void comboShipSlotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipNodeTag.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            string t;
            if(comboShipSlotType.SelectedItem == null)
            {
                t = "";
            }
            else
            {
                t = comboShipSlotType.SelectedItem.ToString();
            }
            slot.SlotType = t;
            listShipSlots.Items[listShipSlots.SelectedIndex] = listShipSlots.SelectedItem;
        }
        private void textShipSlotDefaultModule_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipNodeTag.Slot slot = GetCurrentShipSlot();
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
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            ShipNodeTag.Slot slot = tag.AddSlot();
            listShipSlots.Items.Add(slot);
            listShipSlots.SelectedItem = slot;
        }
        private void buttonRemoveShipPart_Click(object sender, EventArgs e)
        {
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            ShipNodeTag.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            tag.DeleteSlot(ref slot);
            listShipSlots.Items.Remove(slot);
            ClearShipSlot();
        }

        private void buttonSaveShip_Click(object sender, EventArgs e)
        {
            ShipNodeTag tag = GetCurrentShipTag();
            if (tag == null)
                return;
            tag.SaveData();
            textShipId.Text = tag.Id.ToString();
            FillShipSlot();
        }

        private class ShipNodeTag
        {

            public int Id { get; set; }
            public int Parent { get; set; }
            public int BaseStructureHp { get; set; }
            public int BattleIntensity { get; set; }
            public string Name { get; set; }
            public string AssetName { get; set; }
            public List<Slot> slots;
            public List<int> slotsToDelete;

            public ShipNodeTag(int parentId)
            {
                slots = new List<Slot>();
                slotsToDelete = new List<int>();
                this.Parent = parentId;
                this.Name = "New design";
            }

            public ShipNodeTag(ref SqlDataReader  r)
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
                if(this.Id == 0)
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

                if(slots.Count>0)
                {
                    foreach(Slot slot in slots)
                    {
                        if (slot.ShipDesignId == 0)
                            slot.ShipDesignId = this.Id;
                        slot.SaveData();
                    }
                }
                if(slotsToDelete.Count > 0 )
                {
                    q = "DELETE FROM slots WHERE id IN";
                    bool addComma = false;
                    foreach(int slotId in slotsToDelete )
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
                    if(Id == 0)
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

                public ModuleNodeTag module(ref Dictionary<int, ModuleNodeTag> ModuleDict) 
                {
                    if(ModuleDict.ContainsKey(DefaultModuleId))
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

            public enum enumSpecificProperty
            {
                FireRate = 1,
                DeflectorsDamage = 2,
                StructureDamage = 3,
                Deflectors = 4,
                DeflectorsRegen = 5,
                Armor = 6,
                Engine = 7,
                ThrustersSpeed = 8,
                ThrustersDexterity = 9
            }

            public string PropertyString(EnumShipParameter prop)
            {
                switch(prop)
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
                public int Value { get; set; }
                public Parameter(EnumShipParameter p, int value)
                {
                    this.ParamType = p;
                    this.Value = value;
                }
            }

            public class DPSCounter
            {
                private float ShieldDPS;
                private float StructureDPS;
                public DPSCounter()
                {

                }

                public void AddWeapon(ref ModuleNodeTag module)
                {
                    if(module.FireRate() > 0)
                    {
                        if (module.DeflectorsDamage() > 0)
                        {
                            ShieldDPS += (module.DeflectorsDamage() * (module.FireRate() / 60));
                        }
                        if (module.StructureDamage() > 0)
                        {
                            StructureDPS += (module.StructureDamage() * (module.FireRate() / 60));
                        }
                    }

                }

            }

            public class DeflectorsCounter
            {
                public int Points;
                public int Recharge;
                public DeflectorsCounter() { }

                public void AddModule(ref ModuleNodeTag tag)
                {
                    Points += tag.Deflectors();
                    Recharge += tag.DeflectorsRegen();
                }

            }

            public class ThrustersCounter
            {
                public int Speed;
                public int Dexterity;
                public ThrustersCounter() { }

            }

            public List<Parameter> GetParameters()
            {
                Dictionary<int, ModuleNodeTag> mDict = ModuleNodeTag.CreateModuleDict();
                List<Parameter> pms = new List<Parameter>();
                pms.Add(new Parameter(EnumShipParameter.StructurePoints, this.BaseStructureHp));

                DPSCounter dps = new DPSCounter();
                DeflectorsCounter deflect = new DeflectorsCounter();

                foreach(Slot slot in this.slots)
                {
                    ModuleNodeTag tag = slot.module(ref mDict);
                    if(tag != null)
                    {
                        dps.AddWeapon(ref tag);
                        deflect.AddModule(ref tag);

                    }
                }
                return pms;
            }

        }

        private void buttonReloadShipGrid_Click(object sender, EventArgs e)
        {

        }

        #endregion


    }
}
