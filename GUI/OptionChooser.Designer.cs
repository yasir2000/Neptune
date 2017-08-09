namespace GUI
{
  partial class OptionChooser
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
        this.pgrdOptions = new System.Windows.Forms.PropertyGrid();
        this.toolStrip = new System.Windows.Forms.ToolStrip();
        this.tsbtnStartOne = new System.Windows.Forms.ToolStripButton();
        this.tsbtnStartAll = new System.Windows.Forms.ToolStripButton();
        this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
        this.toolStrip.SuspendLayout();
        this.toolStripContainer1.ContentPanel.SuspendLayout();
        this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
        this.toolStripContainer1.SuspendLayout();
        this.SuspendLayout();
        // 
        // pgrdOptions
        // 
        this.pgrdOptions.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pgrdOptions.Location = new System.Drawing.Point(0, 0);
        this.pgrdOptions.Name = "pgrdOptions";
        this.pgrdOptions.PropertySort = System.Windows.Forms.PropertySort.Categorized;
        this.pgrdOptions.Size = new System.Drawing.Size(686, 323);
        this.pgrdOptions.TabIndex = 0;
        this.pgrdOptions.ToolbarVisible = false;
        // 
        // toolStrip
        // 
        this.toolStrip.AllowMerge = false;
        this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
        this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
        this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnStartOne,
            this.tsbtnStartAll});
        this.toolStrip.Location = new System.Drawing.Point(3, 0);
        this.toolStrip.Name = "toolStrip";
        this.toolStrip.Size = new System.Drawing.Size(47, 25);
        this.toolStrip.TabIndex = 1;
        this.toolStrip.Text = "toolStrip";
        // 
        // tsbtnStartOne
        // 
        this.tsbtnStartOne.AutoSize = false;
        this.tsbtnStartOne.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnStartOne.Font = new System.Drawing.Font("Webdings", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.tsbtnStartOne.ForeColor = System.Drawing.Color.ForestGreen;
        this.tsbtnStartOne.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnStartOne.Name = "tsbtnStartOne";
        this.tsbtnStartOne.Size = new System.Drawing.Size(22, 22);
        this.tsbtnStartOne.Text = "4";
        this.tsbtnStartOne.ToolTipText = "Start First\r\nStart solving using only the first of all possible options.";
        this.tsbtnStartOne.Click += new System.EventHandler(this.tsbtnStartOne_Click);
        // 
        // tsbtnStartAll
        // 
        this.tsbtnStartAll.AutoSize = false;
        this.tsbtnStartAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        this.tsbtnStartAll.Font = new System.Drawing.Font("Webdings", 11F);
        this.tsbtnStartAll.ForeColor = System.Drawing.Color.ForestGreen;
        this.tsbtnStartAll.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.tsbtnStartAll.Name = "tsbtnStartAll";
        this.tsbtnStartAll.Size = new System.Drawing.Size(22, 22);
        this.tsbtnStartAll.Text = "8";
        this.tsbtnStartAll.ToolTipText = "Start All\r\nStart solving using all selected options. If multiple options are set " +
            "to All, this may take several minutes.";
        this.tsbtnStartAll.Click += new System.EventHandler(this.tsbtnStartAll_Click);
        // 
        // toolStripContainer1
        // 
        this.toolStripContainer1.BottomToolStripPanelVisible = false;
        // 
        // toolStripContainer1.ContentPanel
        // 
        this.toolStripContainer1.ContentPanel.Controls.Add(this.pgrdOptions);
        this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(686, 323);
        this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.toolStripContainer1.LeftToolStripPanelVisible = false;
        this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
        this.toolStripContainer1.Name = "toolStripContainer1";
        this.toolStripContainer1.RightToolStripPanelVisible = false;
        this.toolStripContainer1.Size = new System.Drawing.Size(686, 348);
        this.toolStripContainer1.TabIndex = 2;
        this.toolStripContainer1.Text = "toolStripContainer1";
        // 
        // toolStripContainer1.TopToolStripPanel
        // 
        this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
        // 
        // OptionChooser
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(686, 348);
        this.Controls.Add(this.toolStripContainer1);
        this.Name = "OptionChooser";
        this.Text = "Neptune TLPLAN Options";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionChooser_FormClosing);
        this.Load += new System.EventHandler(this.OptionChooser_Load);
        this.toolStrip.ResumeLayout(false);
        this.toolStrip.PerformLayout();
        this.toolStripContainer1.ContentPanel.ResumeLayout(false);
        this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
        this.toolStripContainer1.TopToolStripPanel.PerformLayout();
        this.toolStripContainer1.ResumeLayout(false);
        this.toolStripContainer1.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid pgrdOptions;
    private System.Windows.Forms.ToolStrip toolStrip;
    private System.Windows.Forms.ToolStripButton tsbtnStartOne;
    private System.Windows.Forms.ToolStripButton tsbtnStartAll;
    private System.Windows.Forms.ToolStripContainer toolStripContainer1;
  }
}