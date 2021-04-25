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

        #region Служебная
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
        public static AccountData GetLatestUser()
        {
            string q = "SELECT MAX(id) AS max_id FROM admirals";
            int admiralId = Convert.ToInt32(DataConnection.GetResult(q));

            AccountData tData = new AccountData(admiralId);
            return tData;

        }
        private void buttonClearPlayerProgress_Click(object sender, EventArgs e)
        {
            string q;
            q = "DELETE FROM [admirals_log];";
            DataConnection.Execute(q);
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
            FillModuleSizeDict();
            FillModuleWeaponType();
        }

        private void FillModules()
        {
            treeModules.Nodes.Clear();
            string q = ShipModuleType.ModuleQuery(true);

            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();

            SqlDataReader r = DataConnection.GetReader(q);
            if(r.HasRows)
            {
                while(r.Read())
                {
                    ShipModuleType tag = new ShipModuleType(ref r);
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
                moduleTabDict.Add("Armor", tabArmor);
                moduleTabDict.Add("Engine", tabEngine);
                moduleTabDict.Add("Thrusters", tabThrusters);
                moduleTabDict.Add("Misc", tabMisc);
                
                moduleTypeDict = new Dictionary<TabPage, string>();
                moduleTypeDict.Add(tabWeapon, "Weapon");
                moduleTypeDict.Add(tabArmor, "Armor");
                moduleTypeDict.Add(tabEngine, "Engine");
                moduleTypeDict.Add(tabThrusters, "Thrusters");
                moduleTypeDict.Add(tabMisc, "Misc");

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

        private Dictionary<ShipModuleType.WeaponType, RadioButton> ModuleWeaponTypeToRadioDict;
        private Dictionary<RadioButton, ShipModuleType.WeaponType> ModuleRadioToWeaponTypeDict;
        private void FillModuleWeaponType()
        {
            ModuleWeaponTypeToRadioDict = new Dictionary<ShipModuleType.WeaponType, RadioButton>();
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponType.Energy, radioEnergy);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponType.Kinetic, radioKinetic);
            ModuleWeaponTypeToRadioDict.Add(ShipModuleType.WeaponType.Rocket, radioRocket);
            ModuleRadioToWeaponTypeDict = new Dictionary<RadioButton, ShipModuleType.WeaponType>();
            ModuleRadioToWeaponTypeDict.Add(radioEnergy, ShipModuleType.WeaponType.Energy);
            ModuleRadioToWeaponTypeDict.Add(radioKinetic, ShipModuleType.WeaponType.Kinetic);
            ModuleRadioToWeaponTypeDict.Add(radioRocket, ShipModuleType.WeaponType.Rocket);
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

            ShipModuleType tag = new ShipModuleType(parentTag, isCategory);
            newNode = nodesCollection.Add(tag.Name);
            newNode.Tag = tag;
            treeModules.SelectedNode = newNode;
        }

        private Dictionary<string, TabPage> moduleTabDict;
        private Dictionary<TabPage, string> moduleTypeDict;

        private void treeModules_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            ClearModule();

            NoEvents = true;

            textModuleId.Text = tag.Id.ToString();
            textName.Text = tag.Name;
            textModuleUnity.Text = tag.AssetName;
            textModuleType.Text = tag.ModuleTypeStr;
            textModuleEnergy.Text = tag.EnergyNeeded.ToString();

            if(tag.IsCategory==1)
            {
                NoEvents = false;
                return;
            }

            if (string.IsNullOrEmpty(tag.ModuleTypeStr))
                tag.ModuleTypeStr = "Weapon";

            ModuleSizeToRadioDict[tag.Size].Checked = true;
            if (ModuleWeaponTypeToRadioDict.ContainsKey(tag.wType))
                ModuleWeaponTypeToRadioDict[tag.wType].Checked = true;

            NoEvents = false;

            tabControlModule.SelectedTab = moduleTabDict[tag.ModuleTypeStr];
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
            tag.AssetName  = textModuleUnity.Text;
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
            tag.EnergyNeeded = tNumber;
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
            foreach(RadioButton key in ModuleRadioToSizeDict.Keys)
            {
                if(key.Checked)
                {
                    tag.Size = ModuleRadioToSizeDict[key];
                    return;
                }
            }
        }

        private void tabControlModule_Selected(object sender, TabControlEventArgs e)
        {
            FillModuleTab();
        }

        private void FillModuleTab()
        {
            textModuleType.Text = moduleTypeDict[tabControlModule.SelectedTab];
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            tag.ModuleTypeStr = textModuleType.Text;

            ClearModuleTabs();

            NoEvents = true;

            switch (tag.ModuleTypeStr)
            {
                case "Weapon":
                    textModuleFireRate.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.FireRate).ToString();
                    textModuleDeflectorDamage.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.ShieldDamage).ToString();
                    textModuleStructureDamage.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.StructureDamage).ToString();
                    break;
                case "Armor":
                    textModuleArmor.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.ArmorPoints).ToString();
                    break;
                case "Engine":
                    textModuleEngine.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.Engine).ToString();
                    break;
                case "Thrusters":
                    textModuleSpeed.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.Speed).ToString();
                    textDexterity.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.Dexterity).ToString();
                    break;
                case "Misc":
                    textModuleDeflectors.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.ShieldPoints).ToString();
                    textModuleDeflectorsRegen.Text = tag.parameters.ParameterValue(SpaceshipParameters.SpaceShipParameter.ShieldRegen).ToString();
                    break;
            }
            NoEvents = false;
        }

        private void buttonSaveModule_Click(object sender, EventArgs e)
        {
            textModuleType.Text = moduleTypeDict[tabControlModule.SelectedTab];
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
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.FireRate, tNumber);
        }

        private void textModuleDeflectorDamage_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDeflectorDamage.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.ShieldDamage, tNumber);
        }

        private void textModuleStructureDamage_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleStructureDamage.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.StructureDamage, tNumber);
        }

        private void textModuleDeflectors_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDeflectors.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.ShieldPoints, tNumber);
        }

        private void textModuleDeflectorsRegen_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleDeflectorsRegen.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.ShieldRegen, tNumber);
        }

        private void radioEnergy_CheckedChanged(object sender, EventArgs e)
        {
            SetWeaponType();
        }
        private void radioKinetic_CheckedChanged(object sender, EventArgs e)
        {
            SetWeaponType();
        }
        private void radioRocket_CheckedChanged(object sender, EventArgs e)
        {
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
                    tag.wType = ModuleRadioToWeaponTypeDict[key];
                    return;
                }
            }
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
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.ArmorPoints, tNumber);
        }

        private void textModuleEngine_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleEngine.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.Engine, tNumber);
        }

        private void textModuleSpeed_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textModuleSpeed.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.Speed, tNumber);
        }

        private void textDexterity_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModuleType tag = GetModuleTag();
            if (tag == null)
                return;
            int tNumber = 0;
            Int32.TryParse(textDexterity.Text, out tNumber);
            tag.SetParameter(SpaceshipParameters.SpaceShipParameter.Dexterity, tNumber);
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

        private Dictionary<int, RadioButton> SlotSizeIntToRadioDict;
        private Dictionary<RadioButton, int> SlotSizeRadioToIntDict;

        private void FillShips()
        {

            treeShips.Nodes.Clear();
            ShipFilled = true;
            string q = ShipModel.ShipModelQuery();

            SqlDataReader r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
                while(r.Read())
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
            if(treeShips.SelectedNode == null)
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
            if(tag.slots.Count > 0)
            {
                foreach(ShipModel.Slot slot in tag.slots)
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
            tag.AssetName  = textShipUnity.Text;
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
        private void listShipParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            FillShipSlot();
            FillShipParameters();
        }
        private void FillShipSlot()
        {
            ClearShipSlot();

            ShipModel.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;

            NoEvents = true;
            textShipSlotId.Text = slot.Id.ToString();
            textShipSlotNumber.Text = slot.SlotNumber.ToString();
            if (!string.IsNullOrEmpty(slot.SlotTypeStr))
                comboShipSlotType.SelectedItem = slot.SlotTypeStr;
            textShipSlotDefaultModule.Text = slot.DefaultModuleId.ToString();
            textShipSlotControl.Text = slot.SlotControl;
            SlotSizeIntToRadioDict[slot.Size].Checked = true;
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
            ShipModel.Slot slot = GetCurrentShipSlot();
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
            NoEvents = false;
        }
        private ShipModel.Slot GetCurrentShipSlot()
        {
            if (listShipSlots.SelectedItem == null)
                return null;
            return (ShipModel.Slot)listShipSlots.SelectedItem;
        }
        private void textShipSlotNumber_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            int amount = 0;
            Int32.TryParse(textShipSlotNumber.Text, out amount);
            slot.SlotNumber = amount;
            listShipSlots.Items[listShipSlots.SelectedIndex] = listShipSlots.SelectedItem;
        }
        private void textShipSlotControl_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            slot.SlotControl = textShipSlotControl.Text;
        }
        private void comboShipSlotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel.Slot slot = GetCurrentShipSlot();
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
            slot.SlotTypeStr = t;
            listShipSlots.Items[listShipSlots.SelectedIndex] = listShipSlots.SelectedItem;
        }
        private void textShipSlotDefaultModule_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            ShipModel.Slot slot = GetCurrentShipSlot();
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
            ShipModel.Slot slot = tag.AddSlot();
            listShipSlots.Items.Add(slot);
            listShipSlots.SelectedItem = slot;
        }
        private void buttonRemoveShipPart_Click(object sender, EventArgs e)
        {
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            ShipModel.Slot slot = GetCurrentShipSlot();
            if (slot == null)
                return;
            tag.DeleteSlot(ref slot);
            listShipSlots.Items.Remove(slot);
            ClearShipSlot();
        }
        private void buttonReloadShipGrid_Click(object sender, EventArgs e)
        {
            FillShipParameters();
        }

        private void FillShipParameters()
        {
            ShipModel tag = GetCurrentShipTag();
            if (tag == null)
                return;
            SpaceshipParameters curParams = new SpaceshipParameters();
            curParams.AddShipModelParameters(tag);
            Dictionary<int, ShipModuleType> moduleDict = ShipModuleType.CreateModuleDict();
            foreach (ShipModel.Slot slot in tag.slots)
            {
                if(moduleDict.ContainsKey(slot.DefaultModuleId))
                {
                    ShipModuleType mType = moduleDict[slot.DefaultModuleId];
                    if (mType != null)
                    {
                        curParams.AddModuleParameters(mType);
                    }
                }
            }
            gridShipParameters.Rows.Clear();
            if (curParams.Count > 0)
            {
                foreach(SpaceshipParameters.ParameterAndValue param in curParams.ParameterList )
                {
                    DataGridViewRow row;
                    gridShipParameters.Rows.Add();
                    row = gridShipParameters.Rows[gridShipParameters.Rows.Count - 1];
                    row.Cells["sp_name"].Value = param.tString; 
                    row.Cells["sp_value"].Value = param.Value;
                }
            }
           
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

        private bool SaModuleFilled;
        private SpaceshipRig saRig;

        private void tabPage14_Click(object sender, EventArgs e)
        {

        }
        private void tabPage14_Enter(object sender, EventArgs e)
        {
            if (SaModuleFilled)
                return;
            FillSaModule();
        }

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
            foreach (SpaceshipRig.RigSlot rigSlot in saRig.Slots)
            {
                DataGridViewRow row;
                gridSaSlots.Rows.Add();
                row = gridSaSlots.Rows[gridSaSlots.Rows.Count - 1];
                row.Cells["sas_object"].Value = rigSlot;
                row.Cells["sas_name"].Value = rigSlot.Slot.SlotTypeStr;
                row.Cells["sas_content"].Value = rigSlot;
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

            //Module types grid
            gridSaModules.Rows.Clear();
            List<ShipModuleType> moduleTypes = ShipModuleType.CreateModuleList();
            if(moduleTypes.Count > 0)
            {
                foreach(ShipModuleType moduleType in moduleTypes)
                {
                    DataGridViewRow row;
                    gridSaModules.Rows.Add();
                    row = gridSaModules.Rows[gridSaModules.Rows.Count - 1];
                    row.Cells["sam_module"].Value = moduleType;
                    row.Cells["sam_type"].Value = moduleType.ModuleTypeStr;
                    row.Cells["sam_energy_needed"].Value = moduleType.EnergyNeeded;
                }
            }

            //Officer type combo
            comboSaOfficers.Items.Clear();
            List<CrewOfficerType> tOff = CrewOfficerType.GetTypeList();
            foreach(CrewOfficerType curType in tOff)
            {
                comboSaOfficers.Items.Add(curType);
            }

            SaModuleFilled = true;
        }

        private ShipModel GetCurrentModel()
        {
            if(comboSaShip.SelectedItem == null)
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
        }

        private void gridSaModules_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
            ShipModuleType module = (ShipModuleType)moduleRow.Cells["sam_module"].Value;

            SpaceshipRig.RigSlot slot;
            slot = (SpaceshipRig.RigSlot)slotRow.Cells["sas_object"].Value;
            string fitMsg = slot.Slot.ModuleFitsSlot(module);

            if (fitMsg != "")
            {
                MessageBox.Show(fitMsg);
                return;
            }

            slot.LoadModuleType(module);
            slotRow.Cells["sas_content"].Value = null;
            slotRow.Cells["sas_content"].Value = slot;

            saRig.RecalculateParameters();
            textSaBottomLine.Text = saRig.Params.BottomlineString();

        }
        private void listSaOfficers_DoubleClick(object sender, EventArgs e)
        {
            if (gridSaSlots.SelectedCells.Count == 0)
                return;
            DataGridViewRow slotRow = gridSaSlots.Rows[gridSaSlots.SelectedCells[0].RowIndex];

            CrewOfficer officer = (CrewOfficer)listSaOfficers.SelectedItem;

            SpaceshipRig.RigSlot slot;
            slot = (SpaceshipRig.RigSlot)slotRow.Cells["sas_object"].Value;

            string fitMsg = slot.LoadOfficer(officer);
            if(fitMsg != "")
            {
                MessageBox.Show(fitMsg);
                return;
            }

            slotRow.Cells["sas_content"].Value = null;
            slotRow.Cells["sas_content"].Value = slot;

            saRig.RecalculateParameters();
            textSaBottomLine.Text = saRig.Params.BottomlineString();

        }
        private void gridSaSlots_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void gridSaSlots_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            string columnName = gridSaSlots.Columns[e.ColumnIndex].Name;
            DataGridViewRow slotRow = gridSaSlots.Rows[e.RowIndex];
            if (columnName == "sas_content")
            {
                SpaceshipRig.RigSlot slot;
                slot = (SpaceshipRig.RigSlot)slotRow.Cells["sas_object"].Value;
                slot.ClearSlot();
                slotRow.Cells["sas_content"].Value = null;
                slotRow.Cells["sas_content"].Value = slot;
            }

            saRig.RecalculateParameters();
            textSaBottomLine.Text = saRig.Params.BottomlineString();

        }
        private void textSaBottomLine_TextChanged(object sender, EventArgs e)
        {

        }
        private void textSaBottomLine_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            saRig.RecalculateParameters();
            textSaBottomLine.Text = saRig.Params.BottomlineString();
        }
        private void buttonSaCreateOfficer_Click(object sender, EventArgs e)
        {
            if (comboSaOfficers.SelectedItem == null)
                return;
            CrewOfficerType ofType = (CrewOfficerType)comboSaOfficers.SelectedItem;
            int playerId = GetLatestUser().Id;
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

            foreach(CrewOfficer.Stat stat in ofType.Stats)
            {
                DataGridViewRow row;
                gridSaOfficer.Rows.Add();
                row = gridSaOfficer.Rows[gridSaOfficer.Rows.Count - 1];
                row.Cells["sao_stat_name"].Value = stat;
                row.Cells["sao_value"].Value = stat.Value;
            }

        }

        private bool playersOfficersFilled;
        private void tabPage16_Click(object sender, EventArgs e)
        {

        }
        private void tabPage16_Enter(object sender, EventArgs e)
        {
            if (playersOfficersFilled)
                return;
            FillPlayersOfficers();
        }

        private void FillPlayersOfficers()
        {
            AccountData playerAcc = GetLatestUser();

            //Player's officers. 
            listSaOfficers.Items.Clear();
            CrewOfficer playerOfficer = new CrewOfficer(playerAcc);
            listSaOfficers.Items.Add(playerOfficer);
            List<CrewOfficer> officers = CrewOfficer.OfficersForPlayer(playerAcc.Id);
            if (officers.Count > 0)
            {
                foreach (CrewOfficer off in officers)
                {
                    listSaOfficers.Items.Add(off);
                }
            }

            playersOfficersFilled = true;
        }

        private void buttonSaSave_Click(object sender, EventArgs e)
        {
            int playerId = 0;
            string tg = "";
            if(checkSaForPlayer.Checked )
            {
                playerId = GetLatestUser().Id;
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

        private void tabPage18_Click(object sender, EventArgs e)
        {

        }
        private void tabPage18_Enter(object sender, EventArgs e)
        {
            FillRigTree();
        }

        private void FillRigTree()
        {
            treeSaRigs.Nodes.Clear();
            string rigQuery;
            rigQuery = SpaceshipRig.SpaceshipRigQuery(0, 0, "");
            SqlDataReader r = DataConnection.GetReader(rigQuery);
            if(r.HasRows)
            {
                while(r.Read())
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
            saRig.RecalculateParameters();
            textSaBottomLine.Text = saRig.Params.BottomlineString();
        }

        private void buttonSaDeleteRig_Click(object sender, EventArgs e)
        {
            TreeNode n = treeSaRigs.SelectedNode;
            if (n == null)
                return;
            SpaceshipRig rig = (SpaceshipRig)n.Tag;

            if (MessageBox.Show(null, "Delete rig?","", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            rig.Delete();
            treeSaRigs.Nodes.Remove(n);

        }

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
            if(r.HasRows)
            {
                while(r.Read())
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
            CrewOfficerType curOfficer = new CrewOfficerType(true);
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
            checkOfficerAvailableAtStart.Checked = curOfficerType.AvailableAtStart != 0;
            textOfficerPortraitId.Text = curOfficerType.PortraitId.ToString();
            textOfficerTypeBonusPoints.Text = curOfficerType.BonusPoints.ToString();
            gridOfficerType.Rows.Clear();

            List<CrewOfficerType.OfficerStat> stats = curOfficerType.Stats;
            if(stats.Count > 0)
            {
                foreach(CrewOfficerType.OfficerStat stat in stats)
                {
                    DataGridViewRow row;
                    gridOfficerType.Rows.Add();
                    row = gridOfficerType.Rows[gridOfficerType.Rows.Count - 1];
                    row.Cells["ot_name"].Value = stat.Name;
                    row.Cells["ot_score"].Value = stat.PointsBase;
                    
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
        
        private void checkOfficerAvailableAtStart_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            CrewOfficerType curOfficerType = GetCurrentOfficerType();
            if (curOfficerType == null)
                return;
            curOfficerType.AvailableAtStart = checkOfficerAvailableAtStart.Checked ? 1 : 0;

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

            curOfficerType.SetStatValue(Convert.ToString(row.Cells["ot_name"].Value), baseValue);

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
            foreach(GameEvent gameEvent in events)
            {
                TreeNode n;
                if(gameEvent.ParentId == 0)
                {
                    n = mainNode.Nodes.Add(gameEvent.Name);
                }
                else
                {
                    n = nodeDict[gameEvent.ParentId].Nodes.Add(gameEvent.Name);
                }

                n.Tag = gameEvent;

            }

        }

        private void buttonEventAdd_Click(object sender, EventArgs e)
        {
            if (treeEvents.SelectedNode == null)
                return;
            TreeNode n;
            int parentId = 0;
            GameEvent newEvent = new GameEvent();
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
            
        }

        private GameEvent GetCurrentEvent()
        {
            if(treeEvents.SelectedNode == null)
            {
                return null;
            }
            if(treeEvents.SelectedNode.Tag == null)
            {
                return null;
            }

            GameEvent gEvent = (GameEvent)treeEvents.SelectedNode.Tag;
            return gEvent;

        }

        private void textEventName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            GameEvent gEvent = GetCurrentEvent();
            if (gEvent == null)
                return;
            gEvent.Name = textEventName.Text;
            treeEvents.SelectedNode.Text = textEventName.Text;
        }

        private void buttonSaveEvent_Click(object sender, EventArgs e)
        {

        }

        #endregion


    }
}
