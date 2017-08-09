namespace GUI
{
  partial class ViewerForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewerForm));
        this.toolStripButton = new System.Windows.Forms.ToolStripButton();
        this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
        this.statusStrip = new System.Windows.Forms.StatusStrip();
        this.tsslblState = new System.Windows.Forms.ToolStripStatusLabel();
        this.tsslblStatistics = new System.Windows.Forms.ToolStripStatusLabel();
        this.tspbWorking = new System.Windows.Forms.ToolStripProgressBar();
        this.splitContainer = new System.Windows.Forms.SplitContainer();
        this.outputRichTextBox = new System.Windows.Forms.RichTextBox();
        this.outputContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
        this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.commandTextBox = new System.Windows.Forms.TextBox();
        this.propertyGrid = new System.Windows.Forms.PropertyGrid();
        this.mainToolStrip = new System.Windows.Forms.ToolStrip();
        this.tsbtnSave = new System.Windows.Forms.ToolStripButton();
        this.tsbtnCopy = new System.Windows.Forms.ToolStripButton();
        this.tsbtnClear = new System.Windows.Forms.ToolStripButton();
        this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        this.tsbtnStart = new System.Windows.Forms.ToolStripButton();
        this.tsbtnStep = new System.Windows.Forms.ToolStripButton();
        this.tsbtnPause = new System.Windows.Forms.ToolStripButton();
        this.tsbtnStop = new System.Windows.Forms.ToolStripButton();
        this.tsbtnRestart = new System.Windows.Forms.ToolStripButton();
        this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        this.tsbtnPauseInitial = new System.Windows.Forms.ToolStripButton();
        this.tsbtnPauseGoal = new System.Windows.Forms.ToolStripButton();
        this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
        this.tssbtnPrintWorld = new System.Windows.Forms.ToolStripSplitButton();
        this.printWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.printAllWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.tsbtnPrintStats = new System.Windows.Forms.ToolStripButton();
        this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
        this.toolStripContainer.ContentPanel.SuspendLayout();
        this.toolStripContainer.TopToolStripPanel.SuspendLayout();
        this.toolStripContainer.SuspendLayout();
        this.statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
        this.splitContainer.Panel1.SuspendLayout();
        this.splitContainer.Panel2.SuspendLayout();
        this.splitContainer.SuspendLayout();
        this.outputContextMenu.SuspendLayout();
        this.mainToolStrip.SuspendLayout();
        this.SuspendLayout();
        // 
        // toolStripButton
        // 
        this.toolStripButton.Checked = true;
        this.toolStripButton.CheckOnClick = true;
        this.toolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
        this.toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.toolStripButton.Name = "toolStripButton";
        this.toolStripButton.Size = new System.Drawing.Size(85, 24);
        this.toolStripButton.Text = "Show Options";
        this.toolStripButton.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
        this.toolStripButton.CheckedChanged += new System.EventHandler(this.toolStripButton_CheckedChanged);
        // 
        // toolStripContainer
        // 
        // 
        // toolStripContainer.BottomToolStripPanel
        // 
        this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
        // 
        // toolStripContainer.ContentPanel
        // 
        this.toolStripContainer.ContentPanel.Controls.Add(this.splitContainer);
        this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(663, 315);
        this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.toolStripContainer.LeftToolStripPanelVisible = false;
        this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
        this.toolStripContainer.Name = "toolStripContainer";
        this.toolStripContainer.RightToolStripPanelVisible = false;
        this.toolStripContainer.Size = new System.Drawing.Size(663, 364);
        this.toolStripContainer.TabIndex = 1;
        this.toolStripContainer.Text = "toolStripContainer1";
        // 
        // toolStripContainer.TopToolStripPanel
        // 
        this.toolStripContainer.TopToolStripPanel.Controls.Add(this.mainToolStrip);
        // 
        // statusStrip
        // 
        this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
        this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslblState,
            this.tsslblStatistics,
            this.tspbWorking});
        this.statusStrip.Location = new System.Drawing.Point(0, 0);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Size = new System.Drawing.Size(663, 22);
        this.statusStrip.SizingGrip = false;
        this.statusStrip.TabIndex = 0;
        // 
        // tsslblState
        // 
        this.tsslblState.Name = "tsslblState";
        this.tsslblState.Size = new System.Drawing.Size(595, 17);
        this.tsslblState.Spring = true;
        this.tsslblState.Text = "Ready";
        this.tsslblState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // tsslblStatistics
        // 
        this.tsslblStatistics.Name = "tsslblStatistics";
        this.tsslblStatistics.Size = new System.Drawing.Size(53, 17);
        this.tsslblStatistics.Text = "Statistics";
        // 
        // tspbWorking
        // 
        this.tspbWorking.Maximum = 1000;
        this.tspbWorking.Name = "tspbWorking";
        this.tspbWorking.Size = new System.Drawing.Size(50, 16);
        this.tspbWorking.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
        this.tspbWorking.Visible = false;
        // 
        // splitContainer
        // 
        this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer.Location = new System.Drawing.Point(0, 0);
        this.splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        this.splitContainer.Panel1.Controls.Add(this.outputRichTextBox);
        this.splitContainer.Panel1.Controls.Add(this.commandTextBox);
        // 
        // splitContainer.Panel2
        // 
        this.splitContainer.Panel2.Controls.Add(this.propertyGrid);
        this.splitContainer.Size = new System.Drawing.Size(663, 315);
        this.splitContainer.SplitterDistance = 399;
        this.splitContainer.TabIndex = 0;
        // 
        // outputRichTextBox
        // 
        this.outputRichTextBox.BackColor = System.Drawing.SystemColors.Window;
        this.outputRichTextBox.ContextMenuStrip = this.outputContextMenu;
        this.outputRichTextBox.DetectUrls = false;
        this.outputRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.outputRichTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.outputRichTextBox.Location = new System.Drawing.Point(0, 0);
        this.outputRichTextBox.Name = "outputRichTextBox";
        this.outputRichTextBox.ReadOnly = true;
        this.outputRichTextBox.Size = new System.Drawing.Size(399, 295);
        this.outputRichTextBox.TabIndex = 0;
        this.outputRichTextBox.Text = "";
        this.outputRichTextBox.WordWrap = false;
        // 
        // outputContextMenu
        // 
        this.outputContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator4,
            this.copyToolStripMenuItem});
        this.outputContextMenu.Name = "outputContextMenu";
        this.outputContextMenu.Size = new System.Drawing.Size(165, 76);
        // 
        // clearToolStripMenuItem
        // 
        this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
        this.clearToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
        this.clearToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
        this.clearToolStripMenuItem.Text = "C&lear";
        this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
        // 
        // selectAllToolStripMenuItem
        // 
        this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
        this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
        this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
        this.selectAllToolStripMenuItem.Text = "Select &All";
        this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
        // 
        // toolStripSeparator4
        // 
        this.toolStripSeparator4.Name = "toolStripSeparator4";
        this.toolStripSeparator4.Size = new System.Drawing.Size(161, 6);
        // 
        // copyToolStripMenuItem
        // 
        this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
        this.copyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
        this.copyToolStripMenuItem.Text = "&Copy";
        this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
        // 
        // commandTextBox
        // 
        this.commandTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
        this.commandTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
        this.commandTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.commandTextBox.Enabled = false;
        this.commandTextBox.ForeColor = System.Drawing.Color.DarkGray;
        this.commandTextBox.Location = new System.Drawing.Point(0, 295);
        this.commandTextBox.Name = "commandTextBox";
        this.commandTextBox.Size = new System.Drawing.Size(399, 20);
        this.commandTextBox.TabIndex = 1;
        this.commandTextBox.Text = "Enter commands here";
        this.commandTextBox.WordWrap = false;
        this.commandTextBox.Enter += new System.EventHandler(this.commandTextBox_Enter);
        this.commandTextBox.Leave += new System.EventHandler(this.commandTextBox_Leave);
        this.commandTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.commandTextBox_PreviewKeyDown);
        // 
        // propertyGrid
        // 
        this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid.HelpVisible = false;
        this.propertyGrid.Location = new System.Drawing.Point(0, 0);
        this.propertyGrid.Name = "propertyGrid";
        this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
        this.propertyGrid.Size = new System.Drawing.Size(260, 315);
        this.propertyGrid.TabIndex = 0;
        this.propertyGrid.ToolbarVisible = false;
        // 
        // mainToolStrip
        // 
        this.mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
        this.mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
        this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnSave,
            this.tsbtnCopy,
            this.tsbtnClear,
            this.toolStripSeparator1,
            this.tsbtnStart,
            this.tsbtnStep,
            this.tsbtnPause,
            this.tsbtnStop,
            this.tsbtnRestart,
            this.toolStripSeparator2,
            this.tsbtnPauseInitial,
            this.tsbtnPauseGoal,
            this.toolStripSeparator5,
            this.tssbtnPrintWorld,
            this.tsbtnPrintStats,
            this.toolStripSeparator3,
            this.toolStripButton});
        this.mainToolStrip.Location = new System.Drawing.Point(3, 0);
        this.mainToolStrip.Name = "mainToolStrip";
        this.mainToolStrip.Size = new System.Drawing.Size(612, 27);
        this.mainToolStrip.TabIndex = 2;
        // 
        // tsbtnSave
        // 
        this.tsbtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this.tsbtnSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnSave.Image")));
        this.tsbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnSave.Name = "tsbtnSave";
        this.tsbtnSave.Size = new System.Drawing.Size(23, 24);
        this.tsbtnSave.Text = "&Save";
        this.tsbtnSave.Click += new System.EventHandler(this.tsbtnSave_Click);
        // 
        // tsbtnCopy
        // 
        this.tsbtnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this.tsbtnCopy.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnCopy.Image")));
        this.tsbtnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnCopy.Name = "tsbtnCopy";
        this.tsbtnCopy.Size = new System.Drawing.Size(23, 24);
        this.tsbtnCopy.Text = "&Copy";
        this.tsbtnCopy.Click += new System.EventHandler(this.tsbtnCopy_Click);
        // 
        // tsbtnClear
        // 
        this.tsbtnClear.AutoSize = false;
        this.tsbtnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnClear.Font = new System.Drawing.Font("Wingdings 2", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.tsbtnClear.ForeColor = System.Drawing.Color.Red;
        this.tsbtnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnClear.Name = "tsbtnClear";
        this.tsbtnClear.Size = new System.Drawing.Size(23, 22);
        this.tsbtnClear.Text = "O";
        this.tsbtnClear.ToolTipText = "Clear\r\nClear the output buffer.";
        this.tsbtnClear.Click += new System.EventHandler(this.tsbtnClear_Click);
        // 
        // toolStripSeparator1
        // 
        this.toolStripSeparator1.Name = "toolStripSeparator1";
        this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
        // 
        // tsbtnStart
        // 
        this.tsbtnStart.AutoSize = false;
        this.tsbtnStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnStart.Enabled = false;
        this.tsbtnStart.Font = new System.Drawing.Font("Webdings", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.tsbtnStart.ForeColor = System.Drawing.Color.ForestGreen;
        this.tsbtnStart.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnStart.Image")));
        this.tsbtnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnStart.Name = "tsbtnStart";
        this.tsbtnStart.Size = new System.Drawing.Size(24, 24);
        this.tsbtnStart.Text = "4";
        this.tsbtnStart.ToolTipText = "Start/continue\r\nStart or continue solving the problem, without parsing the files " +
            "again.";
        this.tsbtnStart.Click += new System.EventHandler(this.tsbtnStart_Click);
        // 
        // tsbtnStep
        // 
        this.tsbtnStep.AutoSize = false;
        this.tsbtnStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnStep.Enabled = false;
        this.tsbtnStep.Font = new System.Drawing.Font("Webdings", 11.25F);
        this.tsbtnStep.ForeColor = System.Drawing.Color.ForestGreen;
        this.tsbtnStep.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnStep.Image")));
        this.tsbtnStep.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnStep.Name = "tsbtnStep";
        this.tsbtnStep.Size = new System.Drawing.Size(24, 24);
        this.tsbtnStep.Text = ":";
        this.tsbtnStep.ToolTipText = "Step\r\nExamine a single node, then pause.";
        this.tsbtnStep.Click += new System.EventHandler(this.tsbtnStep_Click);
        // 
        // tsbtnPause
        // 
        this.tsbtnPause.AutoSize = false;
        this.tsbtnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnPause.Font = new System.Drawing.Font("Webdings", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.tsbtnPause.ForeColor = System.Drawing.Color.ForestGreen;
        this.tsbtnPause.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPause.Image")));
        this.tsbtnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnPause.Name = "tsbtnPause";
        this.tsbtnPause.Size = new System.Drawing.Size(24, 24);
        this.tsbtnPause.Text = ";";
        this.tsbtnPause.ToolTipText = "Pause\r\nPause solving the problem.";
        this.tsbtnPause.Click += new System.EventHandler(this.tsbtnPause_Click);
        // 
        // tsbtnStop
        // 
        this.tsbtnStop.AutoSize = false;
        this.tsbtnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnStop.Font = new System.Drawing.Font("Webdings", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.tsbtnStop.ForeColor = System.Drawing.Color.Red;
        this.tsbtnStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnStop.Image")));
        this.tsbtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnStop.Name = "tsbtnStop";
        this.tsbtnStop.Size = new System.Drawing.Size(24, 24);
        this.tsbtnStop.Text = "<";
        this.tsbtnStop.ToolTipText = "Stop\r\nStop solving the problem. You can then start solving from scratch.";
        this.tsbtnStop.Click += new System.EventHandler(this.tsbtnStop_Click);
        // 
        // tsbtnRestart
        // 
        this.tsbtnRestart.AutoSize = false;
        this.tsbtnRestart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnRestart.Font = new System.Drawing.Font("Wingdings 3", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.tsbtnRestart.ForeColor = System.Drawing.Color.DarkBlue;
        this.tsbtnRestart.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnRestart.Image")));
        this.tsbtnRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnRestart.Name = "tsbtnRestart";
        this.tsbtnRestart.Size = new System.Drawing.Size(24, 24);
        this.tsbtnRestart.Text = "O";
        this.tsbtnRestart.ToolTipText = "Restart\r\nStop solving the problem, parse the files again and restart solving.";
        this.tsbtnRestart.Click += new System.EventHandler(this.tsbtnRestart_Click);
        // 
        // toolStripSeparator2
        // 
        this.toolStripSeparator2.Name = "toolStripSeparator2";
        this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
        // 
        // tsbtnPauseInitial
        // 
        this.tsbtnPauseInitial.CheckOnClick = true;
        this.tsbtnPauseInitial.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnPauseInitial.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPauseInitial.Image")));
        this.tsbtnPauseInitial.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnPauseInitial.Name = "tsbtnPauseInitial";
        this.tsbtnPauseInitial.Size = new System.Drawing.Size(74, 24);
        this.tsbtnPauseInitial.Text = "Pause Initial";
        this.tsbtnPauseInitial.ToolTipText = "Pause on the initial world";
        this.tsbtnPauseInitial.Click += new System.EventHandler(this.tsbtnPauseInitial_Click);
        // 
        // tsbtnPauseGoal
        // 
        this.tsbtnPauseGoal.CheckOnClick = true;
        this.tsbtnPauseGoal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnPauseGoal.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPauseGoal.Image")));
        this.tsbtnPauseGoal.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnPauseGoal.Name = "tsbtnPauseGoal";
        this.tsbtnPauseGoal.Size = new System.Drawing.Size(69, 24);
        this.tsbtnPauseGoal.Text = "Pause Goal";
        this.tsbtnPauseGoal.ToolTipText = "Pause on goal world";
        this.tsbtnPauseGoal.Click += new System.EventHandler(this.tsbtnPauseGoal_Click);
        // 
        // toolStripSeparator5
        // 
        this.toolStripSeparator5.Name = "toolStripSeparator5";
        this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
        // 
        // tssbtnPrintWorld
        // 
        this.tssbtnPrintWorld.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tssbtnPrintWorld.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printWorldToolStripMenuItem,
            this.printAllWorldToolStripMenuItem});
        this.tssbtnPrintWorld.Image = ((System.Drawing.Image)(resources.GetObject("tssbtnPrintWorld.Image")));
        this.tssbtnPrintWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tssbtnPrintWorld.Name = "tssbtnPrintWorld";
        this.tssbtnPrintWorld.Size = new System.Drawing.Size(83, 24);
        this.tssbtnPrintWorld.Tag = "";
        this.tssbtnPrintWorld.Text = "Print World";
        this.tssbtnPrintWorld.ToolTipText = "Print World\r\nPrint only the predicates that are true and the fluents that are def" +
            "ined.";
        this.tssbtnPrintWorld.ButtonClick += new System.EventHandler(this.tssbtnPrintWorld_ButtonClick);
        // 
        // printWorldToolStripMenuItem
        // 
        this.printWorldToolStripMenuItem.Name = "printWorldToolStripMenuItem";
        this.printWorldToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
        this.printWorldToolStripMenuItem.Text = "Print World";
        this.printWorldToolStripMenuItem.ToolTipText = "Print World\r\nPrint only the predicates that are true and the fluents that are def" +
            "ined.";
        this.printWorldToolStripMenuItem.Click += new System.EventHandler(this.printWorldToolStripMenuItem_Click);
        // 
        // printAllWorldToolStripMenuItem
        // 
        this.printAllWorldToolStripMenuItem.Name = "printAllWorldToolStripMenuItem";
        this.printAllWorldToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
        this.printAllWorldToolStripMenuItem.Text = "Print All World";
        this.printAllWorldToolStripMenuItem.ToolTipText = "Print All World\r\nPrint all predicates and fluents, independently of their values." +
            "";
        this.printAllWorldToolStripMenuItem.Click += new System.EventHandler(this.printAllWorldToolStripMenuItem_Click);
        // 
        // tsbtnPrintStats
        // 
        this.tsbtnPrintStats.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnPrintStats.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnPrintStats.Name = "tsbtnPrintStats";
        this.tsbtnPrintStats.Size = new System.Drawing.Size(85, 24);
        this.tsbtnPrintStats.Text = "Print Statistics";
        this.tsbtnPrintStats.ToolTipText = "Print Statistics\r\nPrint the current planning statistics (these may not be accurat" +
            "e).";
        this.tsbtnPrintStats.Click += new System.EventHandler(this.tsbtnPrintStats_Click);
        // 
        // toolStripSeparator3
        // 
        this.toolStripSeparator3.Name = "toolStripSeparator3";
        this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
        // 
        // ViewerForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(663, 364);
        this.Controls.Add(this.toolStripContainer);
        this.Name = "ViewerForm";
        this.Text = "Results";
        this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ViewerForm_FormClosed);
        this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
        this.toolStripContainer.BottomToolStripPanel.PerformLayout();
        this.toolStripContainer.ContentPanel.ResumeLayout(false);
        this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
        this.toolStripContainer.TopToolStripPanel.PerformLayout();
        this.toolStripContainer.ResumeLayout(false);
        this.toolStripContainer.PerformLayout();
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.splitContainer.Panel1.ResumeLayout(false);
        this.splitContainer.Panel1.PerformLayout();
        this.splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
        this.splitContainer.ResumeLayout(false);
        this.outputContextMenu.ResumeLayout(false);
        this.mainToolStrip.ResumeLayout(false);
        this.mainToolStrip.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolStripButton toolStripButton;
    private System.Windows.Forms.ToolStripContainer toolStripContainer;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.PropertyGrid propertyGrid;
    private System.Windows.Forms.RichTextBox outputRichTextBox;
    private System.Windows.Forms.ToolStripStatusLabel tsslblState;
    private System.Windows.Forms.ToolStripButton tsbtnStart;
    private System.Windows.Forms.ToolStripButton tsbtnPause;
    private System.Windows.Forms.ToolStripButton tsbtnStop;
    private System.Windows.Forms.ToolStripButton tsbtnRestart;
    private System.Windows.Forms.ToolStripButton tsbtnStep;
    private System.Windows.Forms.ToolStrip mainToolStrip;
    private System.Windows.Forms.ToolStripButton tsbtnSave;
    private System.Windows.Forms.ToolStripButton tsbtnCopy;
    private System.Windows.Forms.ToolStripButton tsbtnClear;
    private System.Windows.Forms.TextBox commandTextBox;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ContextMenuStrip outputContextMenu;
    private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
    private System.Windows.Forms.ToolStripStatusLabel tsslblStatistics;
    private System.Windows.Forms.ToolStripProgressBar tspbWorking;
    private System.Windows.Forms.ToolStripButton tsbtnPauseInitial;
    private System.Windows.Forms.ToolStripButton tsbtnPauseGoal;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripSplitButton tssbtnPrintWorld;
    private System.Windows.Forms.ToolStripMenuItem printWorldToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem printAllWorldToolStripMenuItem;
    private System.Windows.Forms.ToolStripButton tsbtnPrintStats;

  }
}