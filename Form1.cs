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

        #region "Характеристики адмиралов"

        private int statStartPointsId;

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }
        private void tabPage5_Enter(object sender, EventArgs e)
        {
            string q;
            SqlDataReader r;

            q = @"SELECT 
                    id,
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


    }
}
