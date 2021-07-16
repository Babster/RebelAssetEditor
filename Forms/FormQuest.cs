using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetEditor.Forms
{
    public partial class FormQuest : Form
    {

        private bool noEvents;

        public FormQuest()
        {
            InitializeComponent();
        }

        private void FormQuest_Load(object sender, EventArgs e)
        {
            List<EventManager.EventType> tList2 = Enum.GetValues(typeof(EventManager.EventType)).Cast<EventManager.EventType>().ToList();
            foreach (var element in tList2)
            {
                comboAppearType.Items.Add(element);
            }
            FillForm();
        }

        private void FillForm()
        {

            TreeNode mainNode = treeQuest.Nodes.Add("Main node");

            List<Quest> quests = Quest.QuestList();
            Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
            foreach(var item in quests)
            {
                if(item.ParentId > 0)
                {
                    TreeNode newQuest = nodes[item.ParentId].Nodes.Add(item.Name);
                    newQuest.Tag = item;
                    nodes.Add(item.Id, newQuest);
                }
                else
                {
                    TreeNode newQuest = mainNode.Nodes.Add(item.Name);
                    newQuest.Tag = item;
                    nodes.Add(item.Id, newQuest);
                }
            }
            mainNode.Expand();
        }

        private void ClearQuest()
        {
            noEvents = true;
            textId.Text = "";
            textName.Text = "";
            textQuestText.Text = "";
            checkAvailableAtStart.Checked = false;
            textPredecessorQuests.Text = "";
            textScenesOnStart.Text = "";
            textScenesOnEnd.Text = "";
            textBattleTypeId.Text = "";
            textEventType.Text = "";
            comboAppearType.SelectedItem = null;
            textVariety.Text = "";
            noEvents = false;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            TreeNode parentNode;
            if(treeQuest.SelectedNode == null)
            {
                parentNode = treeQuest.Nodes[0];
            }
            else
            {
                parentNode = treeQuest.SelectedNode;
            }

            TreeNode newNode = parentNode.Nodes.Add("New quest");
            Quest newQuest = new Quest();
            newQuest.Name = "New quest";
            newNode.Tag = new Quest();
            treeQuest.SelectedNode = newNode;
        }
    }
}
