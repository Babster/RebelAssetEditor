namespace AssetEditor
{
    partial class Form1
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
            this.treeScenes = new System.Windows.Forms.TreeView();
            this.buttonAddNode = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textSceneName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkNextScreen = new System.Windows.Forms.CheckBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textSceneRussian = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.textSceneEnglish = new System.Windows.Forms.TextBox();
            this.textSceneElementImageId = new System.Windows.Forms.TextBox();
            this.comboSceneElementType = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.listSceneElements = new System.Windows.Forms.ListBox();
            this.buttonAddSceneElement = new System.Windows.Forms.Button();
            this.textSceneBackgroundId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonSaveScene = new System.Windows.Forms.Button();
            this.textSceneId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textImageId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonImageLoad = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textImageName = new System.Windows.Forms.TextBox();
            this.textImagePartition = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonAddPartition = new System.Windows.Forms.Button();
            this.treeImages = new System.Windows.Forms.TreeView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.textStatRegistrationPoints = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textStatBaseValue = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.buttonSaveStat = new System.Windows.Forms.Button();
            this.textStatSortIdx = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textStatDescriptionRussian = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textStatDescriptionEnglish = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textStatName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textStatId = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.treeStats = new System.Windows.Forms.TreeView();
            this.buttonCreateStat = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.buttonTestPlayerStats = new System.Windows.Forms.Button();
            this.buttonRegisterAccount = new System.Windows.Forms.Button();
            this.buttonDeleteAccount = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeScenes
            // 
            this.treeScenes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeScenes.Location = new System.Drawing.Point(7, 53);
            this.treeScenes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeScenes.Name = "treeScenes";
            this.treeScenes.Size = new System.Drawing.Size(329, 453);
            this.treeScenes.TabIndex = 0;
            this.treeScenes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeScenes_AfterSelect);
            // 
            // buttonAddNode
            // 
            this.buttonAddNode.Location = new System.Drawing.Point(7, 8);
            this.buttonAddNode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonAddNode.Name = "buttonAddNode";
            this.buttonAddNode.Size = new System.Drawing.Size(111, 35);
            this.buttonAddNode.TabIndex = 1;
            this.buttonAddNode.Text = "Добавить";
            this.buttonAddNode.UseVisualStyleBackColor = true;
            this.buttonAddNode.Click += new System.EventHandler(this.buttonAddNode_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(511, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Имя:";
            // 
            // textSceneName
            // 
            this.textSceneName.Location = new System.Drawing.Point(562, 53);
            this.textSceneName.MaxLength = 50;
            this.textSceneName.Name = "textSceneName";
            this.textSceneName.Size = new System.Drawing.Size(568, 26);
            this.textSceneName.TabIndex = 3;
            this.textSceneName.TextChanged += new System.EventHandler(this.textSceneName_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1129, 547);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.textSceneBackgroundId);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.buttonSaveScene);
            this.tabPage1.Controls.Add(this.textSceneId);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.buttonAddNode);
            this.tabPage1.Controls.Add(this.textSceneName);
            this.tabPage1.Controls.Add(this.treeScenes);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1121, 514);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scene editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkNextScreen);
            this.groupBox1.Controls.Add(this.tabControl2);
            this.groupBox1.Controls.Add(this.textSceneElementImageId);
            this.groupBox1.Controls.Add(this.comboSceneElementType);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.listSceneElements);
            this.groupBox1.Controls.Add(this.buttonAddSceneElement);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(349, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(764, 327);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Элементы сцены";
            // 
            // checkNextScreen
            // 
            this.checkNextScreen.AutoSize = true;
            this.checkNextScreen.Location = new System.Drawing.Point(652, 69);
            this.checkNextScreen.Name = "checkNextScreen";
            this.checkNextScreen.Size = new System.Drawing.Size(94, 24);
            this.checkNextScreen.TabIndex = 17;
            this.checkNextScreen.Text = "Переход";
            this.checkNextScreen.UseVisualStyleBackColor = true;
            this.checkNextScreen.CheckedChanged += new System.EventHandler(this.checkNextScreen_CheckedChanged);
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(296, 133);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(462, 188);
            this.tabControl2.TabIndex = 16;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textSceneRussian);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(454, 155);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Русский";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textSceneRussian
            // 
            this.textSceneRussian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSceneRussian.Location = new System.Drawing.Point(3, 3);
            this.textSceneRussian.MaxLength = 200;
            this.textSceneRussian.Multiline = true;
            this.textSceneRussian.Name = "textSceneRussian";
            this.textSceneRussian.ReadOnly = true;
            this.textSceneRussian.Size = new System.Drawing.Size(448, 149);
            this.textSceneRussian.TabIndex = 0;
            this.textSceneRussian.TextChanged += new System.EventHandler(this.textSceneRussian_TextChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.textSceneEnglish);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(454, 88);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "English";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // textSceneEnglish
            // 
            this.textSceneEnglish.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSceneEnglish.Location = new System.Drawing.Point(3, 3);
            this.textSceneEnglish.MaxLength = 200;
            this.textSceneEnglish.Multiline = true;
            this.textSceneEnglish.Name = "textSceneEnglish";
            this.textSceneEnglish.ReadOnly = true;
            this.textSceneEnglish.Size = new System.Drawing.Size(448, 82);
            this.textSceneEnglish.TabIndex = 1;
            this.textSceneEnglish.TextChanged += new System.EventHandler(this.textSceneEnglish_TextChanged);
            // 
            // textSceneElementImageId
            // 
            this.textSceneElementImageId.Location = new System.Drawing.Point(445, 101);
            this.textSceneElementImageId.Name = "textSceneElementImageId";
            this.textSceneElementImageId.ReadOnly = true;
            this.textSceneElementImageId.Size = new System.Drawing.Size(100, 26);
            this.textSceneElementImageId.TabIndex = 15;
            this.textSceneElementImageId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textSceneElementImageId.TextChanged += new System.EventHandler(this.textSceneElementImageId_TextChanged);
            // 
            // comboSceneElementType
            // 
            this.comboSceneElementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSceneElementType.Enabled = false;
            this.comboSceneElementType.FormattingEnabled = true;
            this.comboSceneElementType.Items.AddRange(new object[] {
            "Не выбран",
            "Текст",
            "Реплика",
            "Досье"});
            this.comboSceneElementType.Location = new System.Drawing.Point(445, 67);
            this.comboSceneElementType.Name = "comboSceneElementType";
            this.comboSceneElementType.Size = new System.Drawing.Size(189, 28);
            this.comboSceneElementType.TabIndex = 9;
            this.comboSceneElementType.SelectedIndexChanged += new System.EventHandler(this.comboSceneElementType_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(292, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(147, 20);
            this.label11.TabIndex = 5;
            this.label11.Text = "Код изображения:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(292, 70);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(123, 20);
            this.label10.TabIndex = 4;
            this.label10.Text = "Вид элемента:";
            // 
            // listSceneElements
            // 
            this.listSceneElements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listSceneElements.FormattingEnabled = true;
            this.listSceneElements.ItemHeight = 20;
            this.listSceneElements.Location = new System.Drawing.Point(11, 70);
            this.listSceneElements.Name = "listSceneElements";
            this.listSceneElements.Size = new System.Drawing.Size(279, 244);
            this.listSceneElements.TabIndex = 3;
            this.listSceneElements.SelectedIndexChanged += new System.EventHandler(this.listSceneElements_SelectedIndexChanged);
            // 
            // buttonAddSceneElement
            // 
            this.buttonAddSceneElement.Location = new System.Drawing.Point(7, 27);
            this.buttonAddSceneElement.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonAddSceneElement.Name = "buttonAddSceneElement";
            this.buttonAddSceneElement.Size = new System.Drawing.Size(111, 35);
            this.buttonAddSceneElement.TabIndex = 2;
            this.buttonAddSceneElement.Text = "Добавить";
            this.buttonAddSceneElement.UseVisualStyleBackColor = true;
            this.buttonAddSceneElement.Click += new System.EventHandler(this.buttonAddSceneElement_Click);
            // 
            // textSceneBackgroundId
            // 
            this.textSceneBackgroundId.Location = new System.Drawing.Point(562, 94);
            this.textSceneBackgroundId.Name = "textSceneBackgroundId";
            this.textSceneBackgroundId.Size = new System.Drawing.Size(100, 26);
            this.textSceneBackgroundId.TabIndex = 13;
            this.textSceneBackgroundId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textSceneBackgroundId.TextChanged += new System.EventHandler(this.textSceneBackgroundId_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(363, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(193, 20);
            this.label8.TabIndex = 12;
            this.label8.Text = "Код изображения фона:";
            // 
            // buttonSaveScene
            // 
            this.buttonSaveScene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveScene.Location = new System.Drawing.Point(968, 466);
            this.buttonSaveScene.Name = "buttonSaveScene";
            this.buttonSaveScene.Size = new System.Drawing.Size(145, 40);
            this.buttonSaveScene.TabIndex = 9;
            this.buttonSaveScene.Text = "Сохранить сцену";
            this.buttonSaveScene.UseVisualStyleBackColor = true;
            this.buttonSaveScene.Click += new System.EventHandler(this.buttonSaveScene_Click);
            // 
            // textSceneId
            // 
            this.textSceneId.Location = new System.Drawing.Point(396, 53);
            this.textSceneId.MaxLength = 50;
            this.textSceneId.Name = "textSceneId";
            this.textSceneId.ReadOnly = true;
            this.textSceneId.Size = new System.Drawing.Size(103, 26);
            this.textSceneId.TabIndex = 6;
            this.textSceneId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(345, 56);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Код:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(356, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "RebelSpaceGeneral.story_scenes";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textImageId);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.buttonImageLoad);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Controls.Add(this.textImageName);
            this.tabPage2.Controls.Add(this.textImagePartition);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.buttonAddPartition);
            this.tabPage2.Controls.Add(this.treeImages);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1121, 514);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Картинки";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textImageId
            // 
            this.textImageId.Location = new System.Drawing.Point(935, 80);
            this.textImageId.MaxLength = 50;
            this.textImageId.Name = "textImageId";
            this.textImageId.ReadOnly = true;
            this.textImageId.Size = new System.Drawing.Size(90, 26);
            this.textImageId.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(886, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 20);
            this.label9.TabIndex = 8;
            this.label9.Text = "Код:";
            // 
            // buttonImageLoad
            // 
            this.buttonImageLoad.Enabled = false;
            this.buttonImageLoad.Location = new System.Drawing.Point(311, 126);
            this.buttonImageLoad.Name = "buttonImageLoad";
            this.buttonImageLoad.Size = new System.Drawing.Size(129, 34);
            this.buttonImageLoad.TabIndex = 7;
            this.buttonImageLoad.Text = "Загрузить";
            this.buttonImageLoad.UseVisualStyleBackColor = true;
            this.buttonImageLoad.Click += new System.EventHandler(this.buttonImageLoad_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(311, 166);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(797, 357);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // textImageName
            // 
            this.textImageName.Location = new System.Drawing.Point(400, 80);
            this.textImageName.MaxLength = 50;
            this.textImageName.Name = "textImageName";
            this.textImageName.ReadOnly = true;
            this.textImageName.Size = new System.Drawing.Size(460, 26);
            this.textImageName.TabIndex = 5;
            this.textImageName.TextChanged += new System.EventHandler(this.textImageName_TextChanged);
            this.textImageName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textImageName_KeyUp);
            // 
            // textImagePartition
            // 
            this.textImagePartition.Location = new System.Drawing.Point(400, 40);
            this.textImagePartition.MaxLength = 50;
            this.textImagePartition.Name = "textImagePartition";
            this.textImagePartition.ReadOnly = true;
            this.textImagePartition.Size = new System.Drawing.Size(460, 26);
            this.textImagePartition.TabIndex = 4;
            this.textImagePartition.TextChanged += new System.EventHandler(this.textImagePartition_TextChanged);
            this.textImagePartition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textImagePartition_KeyUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(324, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Раздел:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(324, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Имя:";
            // 
            // buttonAddPartition
            // 
            this.buttonAddPartition.Location = new System.Drawing.Point(8, 6);
            this.buttonAddPartition.Name = "buttonAddPartition";
            this.buttonAddPartition.Size = new System.Drawing.Size(129, 34);
            this.buttonAddPartition.TabIndex = 1;
            this.buttonAddPartition.Text = "Добавить";
            this.buttonAddPartition.UseVisualStyleBackColor = true;
            this.buttonAddPartition.Click += new System.EventHandler(this.buttonAddPartition_Click);
            // 
            // treeImages
            // 
            this.treeImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeImages.HideSelection = false;
            this.treeImages.Location = new System.Drawing.Point(8, 46);
            this.treeImages.Name = "treeImages";
            this.treeImages.Size = new System.Drawing.Size(287, 477);
            this.treeImages.TabIndex = 0;
            this.treeImages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeImages_AfterSelect);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.textStatRegistrationPoints);
            this.tabPage5.Controls.Add(this.label18);
            this.tabPage5.Controls.Add(this.groupBox2);
            this.tabPage5.Controls.Add(this.treeStats);
            this.tabPage5.Controls.Add(this.buttonCreateStat);
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1121, 514);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Статы";
            this.tabPage5.UseVisualStyleBackColor = true;
            this.tabPage5.Click += new System.EventHandler(this.tabPage5_Click);
            this.tabPage5.Enter += new System.EventHandler(this.tabPage5_Enter);
            // 
            // textStatRegistrationPoints
            // 
            this.textStatRegistrationPoints.Location = new System.Drawing.Point(549, 45);
            this.textStatRegistrationPoints.Name = "textStatRegistrationPoints";
            this.textStatRegistrationPoints.Size = new System.Drawing.Size(100, 26);
            this.textStatRegistrationPoints.TabIndex = 4;
            this.textStatRegistrationPoints.TextChanged += new System.EventHandler(this.textStatRegistrationPoints_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(333, 48);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(210, 20);
            this.label18.TabIndex = 3;
            this.label18.Text = "Поинтов при регистрации:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textStatBaseValue);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.buttonSaveStat);
            this.groupBox2.Controls.Add(this.textStatSortIdx);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.textStatDescriptionRussian);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.textStatDescriptionEnglish);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.textStatName);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textStatId);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Location = new System.Drawing.Point(337, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 225);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Для записи кликай \"Сохранить\"";
            // 
            // textStatBaseValue
            // 
            this.textStatBaseValue.Location = new System.Drawing.Point(181, 70);
            this.textStatBaseValue.Name = "textStatBaseValue";
            this.textStatBaseValue.Size = new System.Drawing.Size(100, 26);
            this.textStatBaseValue.TabIndex = 12;
            this.textStatBaseValue.TextChanged += new System.EventHandler(this.textStatBaseValue_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(152, 20);
            this.label17.TabIndex = 11;
            this.label17.Text = "Базовое значение:";
            // 
            // buttonSaveStat
            // 
            this.buttonSaveStat.Location = new System.Drawing.Point(623, 181);
            this.buttonSaveStat.Name = "buttonSaveStat";
            this.buttonSaveStat.Size = new System.Drawing.Size(147, 36);
            this.buttonSaveStat.TabIndex = 100;
            this.buttonSaveStat.Text = "Сохранить";
            this.buttonSaveStat.UseVisualStyleBackColor = true;
            this.buttonSaveStat.Click += new System.EventHandler(this.buttonSaveStat_Click);
            // 
            // textStatSortIdx
            // 
            this.textStatSortIdx.Location = new System.Drawing.Point(465, 70);
            this.textStatSortIdx.Name = "textStatSortIdx";
            this.textStatSortIdx.Size = new System.Drawing.Size(100, 26);
            this.textStatSortIdx.TabIndex = 15;
            this.textStatSortIdx.TextChanged += new System.EventHandler(this.textStatSortIdx_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(287, 73);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(172, 20);
            this.label16.TabIndex = 8;
            this.label16.Text = "Порядок сортировки:";
            // 
            // textStatDescriptionRussian
            // 
            this.textStatDescriptionRussian.Location = new System.Drawing.Point(210, 140);
            this.textStatDescriptionRussian.MaxLength = 50;
            this.textStatDescriptionRussian.Name = "textStatDescriptionRussian";
            this.textStatDescriptionRussian.Size = new System.Drawing.Size(560, 26);
            this.textStatDescriptionRussian.TabIndex = 25;
            this.textStatDescriptionRussian.TextChanged += new System.EventHandler(this.textStatDescriptionRussian_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 143);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(159, 20);
            this.label15.TabIndex = 6;
            this.label15.Text = "Описание (русский):";
            // 
            // textStatDescriptionEnglish
            // 
            this.textStatDescriptionEnglish.Location = new System.Drawing.Point(210, 105);
            this.textStatDescriptionEnglish.MaxLength = 50;
            this.textStatDescriptionEnglish.Name = "textStatDescriptionEnglish";
            this.textStatDescriptionEnglish.Size = new System.Drawing.Size(560, 26);
            this.textStatDescriptionEnglish.TabIndex = 20;
            this.textStatDescriptionEnglish.TextChanged += new System.EventHandler(this.textStatDescriptionEnglish_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 108);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(188, 20);
            this.label14.TabIndex = 4;
            this.label14.Text = "Описание (английский):";
            // 
            // textStatName
            // 
            this.textStatName.Location = new System.Drawing.Point(233, 35);
            this.textStatName.MaxLength = 50;
            this.textStatName.Name = "textStatName";
            this.textStatName.Size = new System.Drawing.Size(332, 26);
            this.textStatName.TabIndex = 3;
            this.textStatName.TextChanged += new System.EventHandler(this.textStatName_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(182, 38);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 20);
            this.label13.TabIndex = 2;
            this.label13.Text = "Имя:";
            // 
            // textStatId
            // 
            this.textStatId.Location = new System.Drawing.Point(65, 35);
            this.textStatId.Name = "textStatId";
            this.textStatId.ReadOnly = true;
            this.textStatId.Size = new System.Drawing.Size(100, 26);
            this.textStatId.TabIndex = 1;
            this.textStatId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 38);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 20);
            this.label12.TabIndex = 0;
            this.label12.Text = "Код:";
            // 
            // treeStats
            // 
            this.treeStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeStats.HideSelection = false;
            this.treeStats.Location = new System.Drawing.Point(6, 48);
            this.treeStats.Name = "treeStats";
            this.treeStats.Size = new System.Drawing.Size(307, 460);
            this.treeStats.TabIndex = 1;
            this.treeStats.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeStats_AfterSelect);
            // 
            // buttonCreateStat
            // 
            this.buttonCreateStat.Location = new System.Drawing.Point(6, 6);
            this.buttonCreateStat.Name = "buttonCreateStat";
            this.buttonCreateStat.Size = new System.Drawing.Size(147, 36);
            this.buttonCreateStat.TabIndex = 0;
            this.buttonCreateStat.Text = "Добавить ";
            this.buttonCreateStat.UseVisualStyleBackColor = true;
            this.buttonCreateStat.Click += new System.EventHandler(this.buttonCreateStat_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.buttonTestPlayerStats);
            this.tabPage6.Controls.Add(this.buttonRegisterAccount);
            this.tabPage6.Controls.Add(this.buttonDeleteAccount);
            this.tabPage6.Location = new System.Drawing.Point(4, 29);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1121, 514);
            this.tabPage6.TabIndex = 3;
            this.tabPage6.Text = "Тесты API";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // buttonTestPlayerStats
            // 
            this.buttonTestPlayerStats.Location = new System.Drawing.Point(8, 80);
            this.buttonTestPlayerStats.Name = "buttonTestPlayerStats";
            this.buttonTestPlayerStats.Size = new System.Drawing.Size(169, 31);
            this.buttonTestPlayerStats.TabIndex = 2;
            this.buttonTestPlayerStats.Text = "Скачать статы";
            this.buttonTestPlayerStats.UseVisualStyleBackColor = true;
            this.buttonTestPlayerStats.Click += new System.EventHandler(this.buttonTestPlayerStats_Click);
            // 
            // buttonRegisterAccount
            // 
            this.buttonRegisterAccount.Location = new System.Drawing.Point(8, 43);
            this.buttonRegisterAccount.Name = "buttonRegisterAccount";
            this.buttonRegisterAccount.Size = new System.Drawing.Size(169, 31);
            this.buttonRegisterAccount.TabIndex = 1;
            this.buttonRegisterAccount.Text = "Зарегать аккаунт";
            this.buttonRegisterAccount.UseVisualStyleBackColor = true;
            this.buttonRegisterAccount.Click += new System.EventHandler(this.buttonRegisterAccount_Click);
            // 
            // buttonDeleteAccount
            // 
            this.buttonDeleteAccount.Location = new System.Drawing.Point(8, 6);
            this.buttonDeleteAccount.Name = "buttonDeleteAccount";
            this.buttonDeleteAccount.Size = new System.Drawing.Size(169, 31);
            this.buttonDeleteAccount.TabIndex = 0;
            this.buttonDeleteAccount.Text = "Удалить аккаунт";
            this.buttonDeleteAccount.UseVisualStyleBackColor = true;
            this.buttonDeleteAccount.Click += new System.EventHandler(this.buttonDeleteAccount_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1129, 547);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Asset editor for Rebel space general";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeScenes;
        private System.Windows.Forms.Button buttonAddNode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textSceneName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textSceneId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSaveScene;
        private System.Windows.Forms.Button buttonAddPartition;
        private System.Windows.Forms.TreeView treeImages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textImageName;
        private System.Windows.Forms.TextBox textImagePartition;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonImageLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textSceneBackgroundId;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textImageId;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox listSceneElements;
        private System.Windows.Forms.Button buttonAddSceneElement;
        private System.Windows.Forms.TextBox textSceneElementImageId;
        private System.Windows.Forms.ComboBox comboSceneElementType;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox textSceneRussian;
        private System.Windows.Forms.TextBox textSceneEnglish;
        private System.Windows.Forms.CheckBox checkNextScreen;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button buttonCreateStat;
        private System.Windows.Forms.TreeView treeStats;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textStatId;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textStatSortIdx;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textStatDescriptionRussian;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textStatDescriptionEnglish;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textStatName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonSaveStat;
        private System.Windows.Forms.TextBox textStatBaseValue;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textStatRegistrationPoints;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button buttonDeleteAccount;
        private System.Windows.Forms.Button buttonRegisterAccount;
        private System.Windows.Forms.Button buttonTestPlayerStats;
    }
}

