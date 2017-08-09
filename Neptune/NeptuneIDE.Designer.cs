namespace Neptune
{
    partial class NeptuneIDE
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.consoleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectPanel = new System.Windows.Forms.Panel();
            this.tabPanel = new System.Windows.Forms.TabControl();
            this.transcriptTab = new System.Windows.Forms.TabPage();
            this.transcriptTB = new System.Windows.Forms.TextBox();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.outputTB = new System.Windows.Forms.TextBox();
            this.errorsTab = new System.Windows.Forms.TabPage();
            this.errorsTB = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pDDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.projectPanel.SuspendLayout();
            this.tabPanel.SuspendLayout();
            this.transcriptTab.SuspendLayout();
            this.outputTab.SuspendLayout();
            this.errorsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionsToolStripMenuItem,
            this.scriptsToolStripMenuItem,
            this.pDDLToolStripMenuItem,
            this.webToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(829, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.menuFileExit);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripMenuItem1,
            this.consoleToolStripMenuItem1});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.menuActionsCopy);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.menuActionsClear);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(114, 6);
            // 
            // consoleToolStripMenuItem1
            // 
            this.consoleToolStripMenuItem1.Name = "consoleToolStripMenuItem1";
            this.consoleToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.consoleToolStripMenuItem1.Text = "Console";
            this.consoleToolStripMenuItem1.Click += new System.EventHandler(this.menuActionsConsole);
            // 
            // scriptsToolStripMenuItem
            // 
            this.scriptsToolStripMenuItem.Name = "scriptsToolStripMenuItem";
            this.scriptsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.scriptsToolStripMenuItem.Text = "Scripts";
            this.scriptsToolStripMenuItem.Click += new System.EventHandler(this.menuScripts);
            // 
            // webToolStripMenuItem
            // 
            this.webToolStripMenuItem.Name = "webToolStripMenuItem";
            this.webToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.webToolStripMenuItem.Text = "Planner";
            this.webToolStripMenuItem.Click += new System.EventHandler(this.menuTlplan);
            // 
            // projectPanel
            // 
            this.projectPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.projectPanel.Controls.Add(this.tabPanel);
            this.projectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectPanel.Location = new System.Drawing.Point(0, 24);
            this.projectPanel.Name = "projectPanel";
            this.projectPanel.Size = new System.Drawing.Size(829, 432);
            this.projectPanel.TabIndex = 1;
            // 
            // tabPanel
            // 
            this.tabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPanel.Controls.Add(this.transcriptTab);
            this.tabPanel.Controls.Add(this.outputTab);
            this.tabPanel.Controls.Add(this.errorsTab);
            this.tabPanel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPanel.Location = new System.Drawing.Point(12, 15);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.SelectedIndex = 0;
            this.tabPanel.Size = new System.Drawing.Size(805, 405);
            this.tabPanel.TabIndex = 1;
            // 
            // transcriptTab
            // 
            this.transcriptTab.BackColor = System.Drawing.Color.Transparent;
            this.transcriptTab.Controls.Add(this.transcriptTB);
            this.transcriptTab.Location = new System.Drawing.Point(4, 25);
            this.transcriptTab.Name = "transcriptTab";
            this.transcriptTab.Padding = new System.Windows.Forms.Padding(3);
            this.transcriptTab.Size = new System.Drawing.Size(797, 376);
            this.transcriptTab.TabIndex = 0;
            this.transcriptTab.Text = "Transcript";
            // 
            // transcriptTB
            // 
            this.transcriptTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.transcriptTB.Cursor = System.Windows.Forms.Cursors.Default;
            this.transcriptTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transcriptTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transcriptTB.ForeColor = System.Drawing.Color.Honeydew;
            this.transcriptTB.Location = new System.Drawing.Point(3, 3);
            this.transcriptTB.Margin = new System.Windows.Forms.Padding(20);
            this.transcriptTB.Multiline = true;
            this.transcriptTB.Name = "transcriptTB";
            this.transcriptTB.ReadOnly = true;
            this.transcriptTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.transcriptTB.Size = new System.Drawing.Size(791, 370);
            this.transcriptTB.TabIndex = 0;
            this.transcriptTB.WordWrap = false;
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.outputTB);
            this.outputTab.Location = new System.Drawing.Point(4, 25);
            this.outputTab.Name = "outputTab";
            this.outputTab.Size = new System.Drawing.Size(797, 376);
            this.outputTab.TabIndex = 2;
            this.outputTab.Text = "Output";
            this.outputTab.UseVisualStyleBackColor = true;
            // 
            // outputTB
            // 
            this.outputTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.outputTB.Cursor = System.Windows.Forms.Cursors.Default;
            this.outputTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTB.ForeColor = System.Drawing.Color.Yellow;
            this.outputTB.Location = new System.Drawing.Point(0, 0);
            this.outputTB.Multiline = true;
            this.outputTB.Name = "outputTB";
            this.outputTB.ReadOnly = true;
            this.outputTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTB.Size = new System.Drawing.Size(797, 376);
            this.outputTB.TabIndex = 0;
            this.outputTB.WordWrap = false;
            // 
            // errorsTab
            // 
            this.errorsTab.Controls.Add(this.errorsTB);
            this.errorsTab.Location = new System.Drawing.Point(4, 25);
            this.errorsTab.Name = "errorsTab";
            this.errorsTab.Size = new System.Drawing.Size(797, 376);
            this.errorsTab.TabIndex = 3;
            this.errorsTab.Text = "Errors";
            this.errorsTab.UseVisualStyleBackColor = true;
            // 
            // errorsTB
            // 
            this.errorsTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.errorsTB.Cursor = System.Windows.Forms.Cursors.Default;
            this.errorsTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorsTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorsTB.ForeColor = System.Drawing.Color.SeaShell;
            this.errorsTB.Location = new System.Drawing.Point(0, 0);
            this.errorsTB.Multiline = true;
            this.errorsTB.Name = "errorsTB";
            this.errorsTB.ReadOnly = true;
            this.errorsTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorsTB.Size = new System.Drawing.Size(797, 376);
            this.errorsTB.TabIndex = 0;
            this.errorsTB.WordWrap = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timerTick);
            // 
            // pDDLToolStripMenuItem
            // 
            this.pDDLToolStripMenuItem.Name = "pDDLToolStripMenuItem";
            this.pDDLToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.pDDLToolStripMenuItem.Text = "PDDL";
            this.pDDLToolStripMenuItem.Click += new System.EventHandler(this.pDDLToolStripMenuItem_Click);
            // 
            // NeptuneIDE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 456);
            this.Controls.Add(this.projectPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "NeptuneIDE";
            this.Text = "Neptune 2.0 IDE";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.projectPanel.ResumeLayout(false);
            this.tabPanel.ResumeLayout(false);
            this.transcriptTab.ResumeLayout(false);
            this.transcriptTab.PerformLayout();
            this.outputTab.ResumeLayout(false);
            this.outputTab.PerformLayout();
            this.errorsTab.ResumeLayout(false);
            this.errorsTab.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel projectPanel;
        private System.Windows.Forms.TextBox transcriptTB;
        private System.Windows.Forms.ToolStripMenuItem scriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webToolStripMenuItem;
        private System.Windows.Forms.TabControl tabPanel;
        private System.Windows.Forms.TabPage transcriptTab;
        private System.Windows.Forms.TabPage outputTab;
        private System.Windows.Forms.TabPage errorsTab;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.TextBox outputTB;
        private System.Windows.Forms.TextBox errorsTB;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem pDDLToolStripMenuItem;
    }
}