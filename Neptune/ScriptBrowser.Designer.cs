namespace Neptune
{
    partial class ScriptBrowser
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
            this.hSplitPanel = new System.Windows.Forms.SplitContainer();
            this.fileList = new System.Windows.Forms.ListBox();
            this.tabPanel = new System.Windows.Forms.TabControl();
            this.sourceTab = new System.Windows.Forms.TabPage();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.sourceTB = new System.Windows.Forms.TextBox();
            this.toolsTab = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.syncAmazonBtn = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.backupManagerBtn = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dependenciesBtn = new System.Windows.Forms.Button();
            this.scriptHierarchyBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.editNblosBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.editMethodsBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.editAspectsBtn = new System.Windows.Forms.Button();
            this.editGoalsBtn = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.runBtn = new System.Windows.Forms.Button();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.newBtn = new System.Windows.Forms.Button();
            this.fileNameTB = new System.Windows.Forms.TextBox();
            this.saveBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hSplitPanel)).BeginInit();
            this.hSplitPanel.Panel1.SuspendLayout();
            this.hSplitPanel.Panel2.SuspendLayout();
            this.hSplitPanel.SuspendLayout();
            this.tabPanel.SuspendLayout();
            this.sourceTab.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.toolsTab.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // hSplitPanel
            // 
            this.hSplitPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hSplitPanel.Location = new System.Drawing.Point(0, 0);
            this.hSplitPanel.Name = "hSplitPanel";
            // 
            // hSplitPanel.Panel1
            // 
            this.hSplitPanel.Panel1.Controls.Add(this.fileList);
            // 
            // hSplitPanel.Panel2
            // 
            this.hSplitPanel.Panel2.Controls.Add(this.tabPanel);
            this.hSplitPanel.Panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.hSplitPanel.Size = new System.Drawing.Size(679, 398);
            this.hSplitPanel.SplitterDistance = 132;
            this.hSplitPanel.TabIndex = 0;
            // 
            // fileList
            // 
            this.fileList.BackColor = System.Drawing.Color.LightSlateGray;
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileList.ForeColor = System.Drawing.Color.Honeydew;
            this.fileList.FormattingEnabled = true;
            this.fileList.HorizontalScrollbar = true;
            this.fileList.IntegralHeight = false;
            this.fileList.ItemHeight = 17;
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(132, 398);
            this.fileList.TabIndex = 0;
            this.fileList.SelectedIndexChanged += new System.EventHandler(this.onScriptSelected);
            // 
            // tabPanel
            // 
            this.tabPanel.Controls.Add(this.sourceTab);
            this.tabPanel.Controls.Add(this.toolsTab);
            this.tabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPanel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPanel.Location = new System.Drawing.Point(0, 0);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.SelectedIndex = 0;
            this.tabPanel.Size = new System.Drawing.Size(543, 398);
            this.tabPanel.TabIndex = 1;
            // 
            // sourceTab
            // 
            this.sourceTab.BackColor = System.Drawing.Color.Transparent;
            this.sourceTab.Controls.Add(this.infoPanel);
            this.sourceTab.ForeColor = System.Drawing.Color.Gray;
            this.sourceTab.Location = new System.Drawing.Point(4, 25);
            this.sourceTab.Name = "sourceTab";
            this.sourceTab.Padding = new System.Windows.Forms.Padding(3);
            this.sourceTab.Size = new System.Drawing.Size(535, 369);
            this.sourceTab.TabIndex = 0;
            this.sourceTab.Text = "Source";
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.infoPanel.Controls.Add(this.sourceTB);
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPanel.Location = new System.Drawing.Point(3, 3);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(529, 363);
            this.infoPanel.TabIndex = 0;
            // 
            // sourceTB
            // 
            this.sourceTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.sourceTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceTB.ForeColor = System.Drawing.Color.White;
            this.sourceTB.Location = new System.Drawing.Point(0, 0);
            this.sourceTB.Multiline = true;
            this.sourceTB.Name = "sourceTB";
            this.sourceTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sourceTB.Size = new System.Drawing.Size(529, 363);
            this.sourceTB.TabIndex = 7;
            // 
            // toolsTab
            // 
            this.toolsTab.BackColor = System.Drawing.Color.LightSteelBlue;
            this.toolsTab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolsTab.Controls.Add(this.groupBox6);
            this.toolsTab.Controls.Add(this.groupBox5);
            this.toolsTab.Controls.Add(this.groupBox4);
            this.toolsTab.Controls.Add(this.groupBox3);
            this.toolsTab.Controls.Add(this.groupBox2);
            this.toolsTab.Controls.Add(this.groupBox1);
            this.toolsTab.Location = new System.Drawing.Point(4, 25);
            this.toolsTab.Name = "toolsTab";
            this.toolsTab.Size = new System.Drawing.Size(535, 369);
            this.toolsTab.TabIndex = 3;
            this.toolsTab.Text = "Tools";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.syncAmazonBtn);
            this.groupBox6.Location = new System.Drawing.Point(280, 257);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(227, 82);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Cloud";
            // 
            // syncAmazonBtn
            // 
            this.syncAmazonBtn.Location = new System.Drawing.Point(49, 32);
            this.syncAmazonBtn.Name = "syncAmazonBtn";
            this.syncAmazonBtn.Size = new System.Drawing.Size(129, 23);
            this.syncAmazonBtn.TabIndex = 0;
            this.syncAmazonBtn.Text = "Sync AWS EC2";
            this.syncAmazonBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.backupManagerBtn);
            this.groupBox5.Location = new System.Drawing.Point(280, 146);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(227, 91);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Repository";
            // 
            // backupManagerBtn
            // 
            this.backupManagerBtn.Location = new System.Drawing.Point(49, 39);
            this.backupManagerBtn.Name = "backupManagerBtn";
            this.backupManagerBtn.Size = new System.Drawing.Size(129, 23);
            this.backupManagerBtn.TabIndex = 0;
            this.backupManagerBtn.Text = "Backup Manager";
            this.backupManagerBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dependenciesBtn);
            this.groupBox4.Controls.Add(this.scriptHierarchyBtn);
            this.groupBox4.Location = new System.Drawing.Point(280, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 119);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Structure";
            // 
            // dependenciesBtn
            // 
            this.dependenciesBtn.Location = new System.Drawing.Point(49, 71);
            this.dependenciesBtn.Name = "dependenciesBtn";
            this.dependenciesBtn.Size = new System.Drawing.Size(129, 23);
            this.dependenciesBtn.TabIndex = 1;
            this.dependenciesBtn.Text = "Dependencies";
            this.dependenciesBtn.UseVisualStyleBackColor = true;
            // 
            // scriptHierarchyBtn
            // 
            this.scriptHierarchyBtn.Location = new System.Drawing.Point(49, 29);
            this.scriptHierarchyBtn.Name = "scriptHierarchyBtn";
            this.scriptHierarchyBtn.Size = new System.Drawing.Size(129, 23);
            this.scriptHierarchyBtn.TabIndex = 0;
            this.scriptHierarchyBtn.Text = "Hierarchy";
            this.scriptHierarchyBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.editNblosBtn);
            this.groupBox3.Location = new System.Drawing.Point(24, 257);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(227, 82);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Base Language";
            // 
            // editNblosBtn
            // 
            this.editNblosBtn.Location = new System.Drawing.Point(48, 32);
            this.editNblosBtn.Name = "editNblosBtn";
            this.editNblosBtn.Size = new System.Drawing.Size(129, 23);
            this.editNblosBtn.TabIndex = 0;
            this.editNblosBtn.Text = "Edit NBLOs";
            this.editNblosBtn.UseVisualStyleBackColor = true;
            this.editNblosBtn.Click += new System.EventHandler(this.editNblosBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.editMethodsBtn);
            this.groupBox2.Location = new System.Drawing.Point(24, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 91);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Actions";
            // 
            // editMethodsBtn
            // 
            this.editMethodsBtn.Location = new System.Drawing.Point(48, 38);
            this.editMethodsBtn.Name = "editMethodsBtn";
            this.editMethodsBtn.Size = new System.Drawing.Size(129, 23);
            this.editMethodsBtn.TabIndex = 1;
            this.editMethodsBtn.Text = "Edit Methods";
            this.editMethodsBtn.UseVisualStyleBackColor = true;
            this.editMethodsBtn.Click += new System.EventHandler(this.editMethodsBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.editAspectsBtn);
            this.groupBox1.Controls.Add(this.editGoalsBtn);
            this.groupBox1.Location = new System.Drawing.Point(24, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 119);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Meta Data";
            // 
            // editAspectsBtn
            // 
            this.editAspectsBtn.Location = new System.Drawing.Point(48, 71);
            this.editAspectsBtn.Name = "editAspectsBtn";
            this.editAspectsBtn.Size = new System.Drawing.Size(129, 23);
            this.editAspectsBtn.TabIndex = 1;
            this.editAspectsBtn.Text = "Edit Aspects";
            this.editAspectsBtn.UseVisualStyleBackColor = true;
            this.editAspectsBtn.Click += new System.EventHandler(this.editAspectsBtn_Click);
            // 
            // editGoalsBtn
            // 
            this.editGoalsBtn.Location = new System.Drawing.Point(48, 29);
            this.editGoalsBtn.Name = "editGoalsBtn";
            this.editGoalsBtn.Size = new System.Drawing.Size(129, 23);
            this.editGoalsBtn.TabIndex = 0;
            this.editGoalsBtn.Text = "Edit Goals";
            this.editGoalsBtn.UseVisualStyleBackColor = true;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPanel.Controls.Add(this.runBtn);
            this.buttonPanel.Controls.Add(this.deleteBtn);
            this.buttonPanel.Controls.Add(this.newBtn);
            this.buttonPanel.Controls.Add(this.fileNameTB);
            this.buttonPanel.Controls.Add(this.saveBtn);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 400);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(679, 30);
            this.buttonPanel.TabIndex = 1;
            // 
            // runBtn
            // 
            this.runBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runBtn.Location = new System.Drawing.Point(314, 3);
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(75, 23);
            this.runBtn.TabIndex = 4;
            this.runBtn.Text = "Run";
            this.runBtn.UseVisualStyleBackColor = true;
            this.runBtn.Click += new System.EventHandler(this.runBtn_Click);
            // 
            // deleteBtn
            // 
            this.deleteBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteBtn.Location = new System.Drawing.Point(578, 3);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(75, 23);
            this.deleteBtn.TabIndex = 3;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // newBtn
            // 
            this.newBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newBtn.Location = new System.Drawing.Point(490, 3);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(75, 23);
            this.newBtn.TabIndex = 2;
            this.newBtn.Text = "New";
            this.newBtn.UseVisualStyleBackColor = true;
            this.newBtn.Click += new System.EventHandler(this.newBtn_Click);
            // 
            // fileNameTB
            // 
            this.fileNameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.fileNameTB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileNameTB.ForeColor = System.Drawing.Color.White;
            this.fileNameTB.Location = new System.Drawing.Point(5, 4);
            this.fileNameTB.Name = "fileNameTB";
            this.fileNameTB.Size = new System.Drawing.Size(279, 22);
            this.fileNameTB.TabIndex = 1;
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Location = new System.Drawing.Point(402, 3);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 0;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // ScriptBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(679, 430);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.hSplitPanel);
            this.Name = "ScriptBrowser";
            this.Text = "Neptune 2.0 Script Browser";
            this.hSplitPanel.Panel1.ResumeLayout(false);
            this.hSplitPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hSplitPanel)).EndInit();
            this.hSplitPanel.ResumeLayout(false);
            this.tabPanel.ResumeLayout(false);
            this.sourceTab.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            this.toolsTab.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer hSplitPanel;
        private System.Windows.Forms.ListBox fileList;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button deleteBtn;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.TextBox fileNameTB;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button runBtn;
        private System.Windows.Forms.TabControl tabPanel;
        private System.Windows.Forms.TabPage sourceTab;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.TextBox sourceTB;
        private System.Windows.Forms.TabPage toolsTab;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button editGoalsBtn;
        private System.Windows.Forms.Button editMethodsBtn;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button syncAmazonBtn;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button backupManagerBtn;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button dependenciesBtn;
        private System.Windows.Forms.Button scriptHierarchyBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button editNblosBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button editAspectsBtn;
    }
}