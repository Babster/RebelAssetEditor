
namespace AssetEditor.Forms
{
    partial class FormQuest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeQuest = new System.Windows.Forms.TreeView();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textQuestText = new System.Windows.Forms.RichTextBox();
            this.checkAvailableAtStart = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textPredecessorQuests = new System.Windows.Forms.TextBox();
            this.textScenesOnStart = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textScenesOnEnd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBattleTypeId = new System.Windows.Forms.TextBox();
            this.textEventType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboAppearType = new System.Windows.Forms.ComboBox();
            this.textVariety = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeQuest
            // 
            this.treeQuest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeQuest.HideSelection = false;
            this.treeQuest.Location = new System.Drawing.Point(12, 46);
            this.treeQuest.Name = "treeQuest";
            this.treeQuest.Size = new System.Drawing.Size(252, 413);
            this.treeQuest.TabIndex = 8;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(169, 12);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(95, 28);
            this.buttonDelete.TabIndex = 10;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(12, 12);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(95, 28);
            this.buttonAdd.TabIndex = 9;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(279, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 28);
            this.button1.TabIndex = 11;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(275, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "Id:";
            // 
            // textId
            // 
            this.textId.Location = new System.Drawing.Point(308, 49);
            this.textId.Name = "textId";
            this.textId.ReadOnly = true;
            this.textId.Size = new System.Drawing.Size(97, 26);
            this.textId.TabIndex = 13;
            this.textId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(411, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "Name:";
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(472, 49);
            this.textName.MaxLength = 50;
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(459, 26);
            this.textName.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(275, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "Text:";
            // 
            // textQuestText
            // 
            this.textQuestText.Location = new System.Drawing.Point(324, 87);
            this.textQuestText.Name = "textQuestText";
            this.textQuestText.Size = new System.Drawing.Size(607, 96);
            this.textQuestText.TabIndex = 17;
            this.textQuestText.Text = "";
            // 
            // checkAvailableAtStart
            // 
            this.checkAvailableAtStart.AutoSize = true;
            this.checkAvailableAtStart.Location = new System.Drawing.Point(324, 198);
            this.checkAvailableAtStart.Name = "checkAvailableAtStart";
            this.checkAvailableAtStart.Size = new System.Drawing.Size(145, 24);
            this.checkAvailableAtStart.TabIndex = 18;
            this.checkAvailableAtStart.Text = "Available at start";
            this.checkAvailableAtStart.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(275, 236);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 20);
            this.label4.TabIndex = 19;
            this.label4.Text = "Predecessor quests:";
            // 
            // textPredecessorQuests
            // 
            this.textPredecessorQuests.Location = new System.Drawing.Point(435, 233);
            this.textPredecessorQuests.MaxLength = 20;
            this.textPredecessorQuests.Name = "textPredecessorQuests";
            this.textPredecessorQuests.Size = new System.Drawing.Size(496, 26);
            this.textPredecessorQuests.TabIndex = 20;
            // 
            // textScenesOnStart
            // 
            this.textScenesOnStart.Location = new System.Drawing.Point(406, 265);
            this.textScenesOnStart.MaxLength = 20;
            this.textScenesOnStart.Name = "textScenesOnStart";
            this.textScenesOnStart.Size = new System.Drawing.Size(525, 26);
            this.textScenesOnStart.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(275, 268);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 20);
            this.label5.TabIndex = 21;
            this.label5.Text = "Scenes on start:";
            // 
            // textScenesOnEnd
            // 
            this.textScenesOnEnd.Location = new System.Drawing.Point(406, 297);
            this.textScenesOnEnd.MaxLength = 20;
            this.textScenesOnEnd.Name = "textScenesOnEnd";
            this.textScenesOnEnd.Size = new System.Drawing.Size(525, 26);
            this.textScenesOnEnd.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(275, 300);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 20);
            this.label6.TabIndex = 23;
            this.label6.Text = "Scenes on end:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(275, 332);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 20);
            this.label7.TabIndex = 25;
            this.label7.Text = "Battle type Id:";
            // 
            // textBattleTypeId
            // 
            this.textBattleTypeId.Location = new System.Drawing.Point(388, 329);
            this.textBattleTypeId.MaxLength = 10;
            this.textBattleTypeId.Name = "textBattleTypeId";
            this.textBattleTypeId.Size = new System.Drawing.Size(100, 26);
            this.textBattleTypeId.TabIndex = 26;
            this.textBattleTypeId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textEventType
            // 
            this.textEventType.Location = new System.Drawing.Point(674, 329);
            this.textEventType.MaxLength = 10;
            this.textEventType.Name = "textEventType";
            this.textEventType.Size = new System.Drawing.Size(100, 26);
            this.textEventType.TabIndex = 28;
            this.textEventType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(497, 332);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(171, 20);
            this.label8.TabIndex = 27;
            this.label8.Text = "Game event Id on end:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(275, 366);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 20);
            this.label9.TabIndex = 29;
            this.label9.Text = "Quest appear type:";
            // 
            // comboAppearType
            // 
            this.comboAppearType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAppearType.FormattingEnabled = true;
            this.comboAppearType.Location = new System.Drawing.Point(425, 363);
            this.comboAppearType.Name = "comboAppearType";
            this.comboAppearType.Size = new System.Drawing.Size(180, 28);
            this.comboAppearType.TabIndex = 30;
            // 
            // textVariety
            // 
            this.textVariety.Location = new System.Drawing.Point(689, 363);
            this.textVariety.MaxLength = 10;
            this.textVariety.Name = "textVariety";
            this.textVariety.Size = new System.Drawing.Size(100, 26);
            this.textVariety.TabIndex = 32;
            this.textVariety.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(621, 366);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 20);
            this.label10.TabIndex = 31;
            this.label10.Text = "Variety:";
            // 
            // FormQuest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 471);
            this.Controls.Add(this.textVariety);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.comboAppearType);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textEventType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBattleTypeId);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textScenesOnEnd);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textScenesOnStart);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textPredecessorQuests);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkAvailableAtStart);
            this.Controls.Add(this.textQuestText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.treeQuest);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormQuest";
            this.Text = "Quests";
            this.Load += new System.EventHandler(this.FormQuest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeQuest;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox textQuestText;
        private System.Windows.Forms.CheckBox checkAvailableAtStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textPredecessorQuests;
        private System.Windows.Forms.TextBox textScenesOnStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textScenesOnEnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBattleTypeId;
        private System.Windows.Forms.TextBox textEventType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboAppearType;
        private System.Windows.Forms.TextBox textVariety;
        private System.Windows.Forms.Label label10;
    }
}