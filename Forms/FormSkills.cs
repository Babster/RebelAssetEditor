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
    public partial class FormSkills : Form
    {

        private bool NoEvents;

        public FormSkills()
        {
            InitializeComponent();
        }

        private void FormSkills_Load(object sender, EventArgs e)
        {
            StartSkillsets();
            StartSkillTypes();
        }

 
        #region Skill sets

        private bool skillSetsFilled = false;

        private void StartSkillsets()
        {
            if (skillSetsFilled)
            {
                return;
            }
            treeSkillSets.Nodes.Clear();
            TreeNode mainNode = treeSkillSets.Nodes.Add("Skill set list");
            List<SkillSetSql> sets = SkillSetSql.GetSkillsetList();
            Dictionary<int, TreeNode> nodeDict = new Dictionary<int, TreeNode>();
            if (sets.Count > 0)
            {
                foreach (var set in sets)
                {
                    if (set.ParentId == 0)
                    {
                        TreeNode n = mainNode.Nodes.Add(set.Name);
                        n.Tag = set;
                        nodeDict.Add(set.Id, n);
                    }
                    else
                    {
                        TreeNode n = nodeDict[set.ParentId].Nodes.Add(set.Name);
                        n.Tag = set;
                        nodeDict.Add(set.Id, n);
                    }
                }

            }
            mainNode.Expand();

            skillSetsFilled = true;
        }

        private SkillSetSql GetCurrentSkillSet()
        {
            if (treeSkillSets.SelectedNode == null)
                return null;
            if (treeSkillSets.SelectedNode.Tag == null)
                return null;
            return (SkillSetSql)treeSkillSets.SelectedNode.Tag;
        }

        private void buttonSkillAdd_Click(object sender, EventArgs e)
        {
            SkillSetSql parentSet = GetCurrentSkillSet();

            SkillSetSql newSkillset = new SkillSetSql();
            newSkillset.Name = "New skillset";
            TreeNode n;
            if (parentSet == null)
            {
                n = treeSkillSets.Nodes[0].Nodes.Add(newSkillset.Name);
                n.Tag = newSkillset;
            }
            else
            {
                n = treeSkillSets.SelectedNode.Nodes.Add(newSkillset.Name);
                newSkillset.ParentId = parentSet.Id;
                n.Tag = newSkillset;
            }
            treeSkillSets.SelectedNode = n;
        }

        private void ClearSkillset()
        {
            NoEvents = true;
            textSkillsetId.Text = "";
            textSkillsetName.Text = "";
            comboSkillsetType.SelectedItem = null;
            textSkillsetOpenCost.Text = "";
            NoEvents = false;
        }

        private void treeSkillSets_AfterSelect(object sender, TreeViewEventArgs e)
        {

            ClearSkillset();
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }

            NoEvents = true;
            textSkillsetId.Text = curSet.Id.ToString();
            textSkillsetName.Text = curSet.Name;
            comboSkillsetType.SelectedIndex = (int)curSet.OwnerType;
            textSkillsetOpenCost.Text = curSet.OpenCost.ToString();
            checkSkillSetAvailableForPlayer.Checked = curSet.AvailableForPlayer == 1;

            treeStr.Nodes.Clear();
            if (curSet.Elements.Count > 0)
            {
                foreach (var element in curSet.Elements)
                {
                    TreeNode n = treeStr.Nodes.Add(element.ToString());
                    n.Tag = element;
                }
            }
            if(treeStr.Nodes.Count > 0)
            {
                treeStr.SelectedNode = treeStr.Nodes[0];
            }

            NoEvents = false;


        }

        private void textSkillsetName_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }
            curSet.Name = textSkillsetName.Text;
            treeSkillSets.SelectedNode.Text = curSet.Name;
        }

        private void comboSkillsetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }
            curSet.OwnerType = (SkillSet.SkillsetOwnerTypes)comboSkillsetType.SelectedIndex;
        }

        private void textSkillsetOpenCost_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }
            int value = 0;
            Int32.TryParse(textSkillsetOpenCost.Text, out value);
            curSet.OpenCost = value;
        }

        private void checkSkillSetAvailableForPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }
            if(checkSkillSetAvailableForPlayer.Checked)
            {
                curSet.AvailableForPlayer = 1;
            }
            else
            {
                curSet.AvailableForPlayer = 0;
            }
        }

        private void buttonSaveSkillset_Click(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }
            curSet.SaveData();
            textSkillsetId.Text = curSet.Id.ToString();
        }

        private void buttonAddSkillStructure_Click(object sender, EventArgs e)
        {
            if (NoEvents)
                return;
            SkillSetSql curSet = GetCurrentSkillSet();
            if (curSet == null)
            {
                return;
            }
            SkillSetElementSql newElement = curSet.AddElement();
            TreeNode n = treeStr.Nodes.Add(newElement.ToString());
            n.Tag = newElement;
        }

        private void ClearSkillStructureElement()
        {
            NoEvents = true;
            textStrId.Text = "";
            textStrSkillId.Text = "";
            textStrSkillName.Text = "";
            textStrLevel.Text = "";
            textStrColumn.Text = "";
            checkAvailableAtStart.Checked = false;
            textStrPredecessorId1.Text = "";
            textStrPredecessorName1.Text = "";
            textStrPredecessorId2.Text = "";
            textStrPredecessorName2.Text = "";
            NoEvents = false;
        }

        private SkillSetElementSql GetCurrentSkillElement()
        {
            if(treeStr.SelectedNode == null)
            {
                return null;
            }
            else
            {
                return (SkillSetElementSql)treeStr.SelectedNode.Tag;
            }
        }

        private void treeStr_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearSkillStructureElement();
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if(curElement == null)
            {
                return;
            }
            textStrId.Text = curElement.Id.ToString();
            textStrSkillId.Text = curElement.SkillTypeId.ToString();
            SetSkillName(textStrSkillName, curElement.SkillTypeId);
            textStrLevel.Text = curElement.SkillLevel.ToString();
            textStrColumn.Text = curElement.SkillColumn.ToString();
            checkAvailableAtStart.Checked = curElement.AvailableAtStart == 1;
            textStrPredecessorId1.Text = curElement.Predecessor1.ToString() ;
            SetSkillName(textStrPredecessorName1, curElement.Predecessor1);
            textStrPredecessorId2.Text = curElement.Predecessor2.ToString();
            SetSkillName(textStrPredecessorName2, curElement.Predecessor2);
        }

        private void SetSkillName(TextBox text, int skillId)
        {
            SkillType t = SkillTypeSql.SkillTypeById(skillId);
            if(t == null)
            {
                text.Text = "";
            }
            else
            {
                text.Text = t.Name;
            }
        }

        private void UpdateSkillElementInTree()
        {
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            treeStr.SelectedNode.Text = curElement.ToString();
        }

        private void textStrSkillId_TextChanged(object sender, EventArgs e)
        {
            if(NoEvents)
            {
                return;
            }
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            int value = 0;
            Int32.TryParse(textStrSkillId.Text, out value);
            curElement.SkillTypeId = value;
            SetSkillName(textStrSkillName, value);
            UpdateSkillElementInTree();
        }

        private void textStrLevel_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            int value = 0;
            Int32.TryParse(textStrLevel.Text, out value);
            curElement.SkillLevel = value;
            UpdateSkillElementInTree();
        }

        private void textStrColumn_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            int value = 0;
            Int32.TryParse(textStrColumn.Text, out value);
            curElement.SkillColumn = value;
            UpdateSkillElementInTree();
        }

        private void checkAvailableAtStart_CheckedChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            if(checkAvailableAtStart.Checked)
            {
                curElement.AvailableAtStart = 1;
            }
            else
            {
                curElement.AvailableAtStart = 0;
            }
        }

        private void textStrPredecessorId1_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            int value = 0;
            Int32.TryParse(textStrPredecessorId1.Text, out value);
            curElement.Predecessor1 = value;
            SetSkillName(textStrPredecessorName1, value);
        }

        private void textStrPredecessorId2_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            SkillSetElementSql curElement = GetCurrentSkillElement();
            if (curElement == null)
            {
                return;
            }
            int value = 0;
            Int32.TryParse(textStrPredecessorId2.Text, out value);
            curElement.Predecessor2 = value;
            SetSkillName(textStrPredecessorName2, value);
        }

        #endregion

        #region Skill types

        private void StartSkillTypes()
        {

            treeSkillTypes.Nodes.Clear();
            TreeNode mainNode = treeSkillTypes.Nodes.Add("Skill types");
            Dictionary<int, TreeNode> skillDict = new Dictionary<int, TreeNode>();
            List<SkillTypeSql> sList = SkillTypeSql.SkillTypeList();
            if (sList.Count > 0)
            {
                foreach (var skill in sList)
                {
                    TreeNode n;
                    if (skill.ParentId == 0)
                    {
                        n = mainNode.Nodes.Add(skill.Name);
                        n.Tag = skill;
                    }
                    else
                    {
                        n = skillDict[skill.ParentId].Nodes.Add(skill.Name);
                        n.Tag = skill;
                    }
                    skillDict.Add(skill.Id, n);
                }
            }

            mainNode.Expand();

            List<SkillType.SkillGrages> tList = Enum.GetValues(typeof(SkillType.SkillGrages)).Cast<SkillType.SkillGrages>().ToList();
            foreach (var element in tList)
            {
                comboSkillTypeGrade.Items.Add(element);
            }

            List<SkillType.SkillEffectTypes> tList2 = Enum.GetValues(typeof(SkillType.SkillEffectTypes)).Cast<SkillType.SkillEffectTypes>().ToList();
            foreach(var element in tList2)
            {
                comboSkillTypeEffectType.Items.Add(element);
            }

        }

        private SkillTypeSql GetCurrentSkillType()
        {
            if (treeSkillTypes.SelectedNode == null)
            {
                return null;
            }
            if (treeSkillTypes.SelectedNode.Tag == null)
            {
                return null;
            }
            return (SkillTypeSql)treeSkillTypes.SelectedNode.Tag;
        }

        private void buttonAddSkillType_Click(object sender, EventArgs e)
        {

            SkillTypeSql parentSkill = GetCurrentSkillType();

            SkillTypeSql curSkill = new SkillTypeSql();
            curSkill.Name = "New skill type";
            TreeNode newSkillType;
            if (parentSkill == null)
            {
                newSkillType = treeSkillTypes.Nodes[0].Nodes.Add(curSkill.Name);
            }
            else
            {
                newSkillType = treeSkillTypes.SelectedNode.Nodes.Add(curSkill.Name);
                curSkill.ParentId = parentSkill.Id;
            }
            newSkillType.Tag = curSkill;
            treeSkillTypes.SelectedNode = newSkillType;
        }

        private void treeSkillTypes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearSkillType();
            var curSkill = GetCurrentSkillType();
            if(curSkill == null)
            {
                return;
            }
            NoEvents = true;
            textSkillTypeId.Text = curSkill.Id.ToString();
            textSkillTypeName.Text = curSkill.Name;
            foreach(var item in comboSkillTypeGrade.Items)
            {
                if(item.ToString() == curSkill.Grade.ToString())
                {
                    comboSkillTypeGrade.SelectedItem = item;
                    break;
                }
            }
            
            textSkillTypeIcon.Text = curSkill.IconId.ToString();
            textSkillTypeOpenCost.Text = curSkill.OpenSkillpointsCost.ToString();
            textSkillTypeActivationCost.Text = curSkill.ActivationEnergyCost.ToString();
            textSkillTypeDuration.Text = curSkill.DurationMilliseconds.ToString();
            foreach(var item in comboSkillTypeEffectType.Items)
            {
                if(item.ToString() == curSkill.EffectType.ToString())
                {
                    comboSkillTypeEffectType.SelectedItem = item;
                    break;
                }
            }
            textSkillTypeEffectPower.Text = curSkill.EffectPower.ToString();
            textSkillTypeCooldown.Text = curSkill.CooldownMilliseconds.ToString();
            NoEvents = false;
        }

        private void ClearSkillType()
        {
            NoEvents = true;
            textSkillTypeId.Text = "";
            textSkillTypeName.Text = "";
            comboSkillTypeGrade.SelectedItem = null;
            textSkillTypeIcon.Text = "";
            textSkillTypeOpenCost.Text = "";
            textSkillTypeActivationCost.Text = "";
            textSkillTypeDuration.Text = "";
            comboSkillTypeEffectType.SelectedItem = null;
            textSkillTypeEffectPower.Text = "";
            textSkillTypeCooldown.Text = "";
            NoEvents = false;
        }

       

        private void textSkillTypeName_TextChanged(object sender, EventArgs e)
        {
            if(NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            curSkill.Name = textSkillTypeName.Text;
            treeSkillTypes.SelectedNode.Text = curSkill.Name;
        }

        private void comboSkillTypeGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            if(comboSkillTypeGrade.SelectedItem == null)
            {
                curSkill.Grade = SkillType.SkillGrages.None;
                return;
            }
            List<SkillType.SkillGrages> tList = Enum.GetValues(typeof(SkillType.SkillGrages)).Cast<SkillType.SkillGrages>().ToList();
            foreach (var element in tList)
            {
                if(element.ToString() == comboSkillTypeGrade.SelectedItem.ToString())
                {
                    curSkill.Grade = element;
                    return;
                }
            }
        }

        private void textSkillTypeIcon_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            int val = 0;
            Int32.TryParse(textSkillTypeIcon.Text, out val);
            curSkill.IconId = val;
        }

        private void textSkillTypeOpenCost_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            int val = 0;
            Int32.TryParse(textSkillTypeOpenCost.Text, out val);
            curSkill.OpenSkillpointsCost = val;
        }

        private void textSkillTypeActivationCost_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            int val = 0;
            Int32.TryParse(textSkillTypeActivationCost.Text, out val);
            curSkill.ActivationEnergyCost = val;
        }

        private void textSkillTypeDuration_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            int val = 0;
            Int32.TryParse(textSkillTypeDuration.Text, out val);
            curSkill.DurationMilliseconds = val;
        }

        private void comboSkillTypeEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            if (comboSkillTypeEffectType.SelectedItem == null)
            {
                curSkill.EffectType = SkillType.SkillEffectTypes.None;
                return;
            }
            List<SkillType.SkillEffectTypes> tList2 = Enum.GetValues(typeof(SkillType.SkillEffectTypes)).Cast<SkillType.SkillEffectTypes>().ToList();
            foreach (var element in tList2)
            { 
                if (element.ToString() == comboSkillTypeEffectType.SelectedItem.ToString())
                {
                    curSkill.EffectType = element;
                    return;
                }
            }
        }

        private void textSkillTypeEffectPower_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            int val = 0;
            Int32.TryParse(textSkillTypeEffectPower.Text, out val);
            curSkill.EffectPower = val;
        }

        private void textSkillTypeCooldown_TextChanged(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            int val = 0;
            Int32.TryParse(textSkillTypeCooldown.Text, out val);
            curSkill.CooldownMilliseconds = val;
        }

        private void buttonSaveSkillType_Click(object sender, EventArgs e)
        {
            if (NoEvents)
            {
                return;
            }
            var curSkill = GetCurrentSkillType();
            if (curSkill == null)
            {
                return;
            }
            curSkill.SaveData();
            textSkillTypeId.Text = curSkill.Id.ToString();
        }


        #endregion


    }
}
