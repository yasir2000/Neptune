namespace Neptune
{
    partial class PddlBrowser
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
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.newBtn = new System.Windows.Forms.Button();
            this.fileNameTB = new System.Windows.Forms.TextBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.sourceTab = new System.Windows.Forms.TabPage();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.sourceTB = new System.Windows.Forms.TextBox();
            this.tabPanel = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.hSplitPanel)).BeginInit();
            this.hSplitPanel.Panel1.SuspendLayout();
            this.hSplitPanel.Panel2.SuspendLayout();
            this.hSplitPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.sourceTab.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.tabPanel.SuspendLayout();
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
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.LightSteelBlue;
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
            // tabPanel
            // 
            this.tabPanel.Controls.Add(this.sourceTab);
            this.tabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPanel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPanel.Location = new System.Drawing.Point(0, 0);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.SelectedIndex = 0;
            this.tabPanel.Size = new System.Drawing.Size(543, 398);
            this.tabPanel.TabIndex = 1;
            // 
            // PddlBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(679, 430);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.hSplitPanel);
            this.Name = "PddlBrowser";
            this.Text = "Neptune 2.0 PDDL Browser";
            this.hSplitPanel.Panel1.ResumeLayout(false);
            this.hSplitPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hSplitPanel)).EndInit();
            this.hSplitPanel.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.sourceTab.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            this.tabPanel.ResumeLayout(false);
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
        private System.Windows.Forms.TabControl tabPanel;
        private System.Windows.Forms.TabPage sourceTab;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.TextBox sourceTB;
    }
}