namespace GUI
{
  partial class TLPlan
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
      this.grpOutput = new System.Windows.Forms.GroupBox();
      this.rtxtOutput = new System.Windows.Forms.RichTextBox();
      this.grpFiles = new System.Windows.Forms.GroupBox();
      this.btnProblem = new System.Windows.Forms.Button();
      this.btnDomain = new System.Windows.Forms.Button();
      this.lblProblem = new System.Windows.Forms.Label();
      this.lblDomain = new System.Windows.Forms.Label();
      this.txtProblem = new System.Windows.Forms.TextBox();
      this.txtDomain = new System.Windows.Forms.TextBox();
      this.grpOptions = new System.Windows.Forms.GroupBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tbctlOutput = new System.Windows.Forms.TabControl();
      this.tabParsing = new System.Windows.Forms.TabPage();
      this.rtxtParsing = new System.Windows.Forms.RichTextBox();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.tabPage4 = new System.Windows.Forms.TabPage();
      this.tabPage5 = new System.Windows.Forms.TabPage();
      this.tabPage6 = new System.Windows.Forms.TabPage();
      this.tabPage7 = new System.Windows.Forms.TabPage();
      this.tabPage8 = new System.Windows.Forms.TabPage();
      this.tabPage9 = new System.Windows.Forms.TabPage();
      this.btnSolve = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.connardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.autreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.autre2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ahhhLalalalalalalalalalalaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.propertyCombobox1 = new CustomControls.PropertyCombobox();
      this.fcDomain = new CustomControls.FileChooser();
      this.grpOutput.SuspendLayout();
      this.grpFiles.SuspendLayout();
      this.grpOptions.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tbctlOutput.SuspendLayout();
      this.tabParsing.SuspendLayout();
      this.contextMenuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // grpOutput
      // 
      this.grpOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpOutput.Controls.Add(this.rtxtOutput);
      this.grpOutput.Location = new System.Drawing.Point(12, 229);
      this.grpOutput.Name = "grpOutput";
      this.grpOutput.Size = new System.Drawing.Size(556, 174);
      this.grpOutput.TabIndex = 0;
      this.grpOutput.TabStop = false;
      this.grpOutput.Text = "Output";
      // 
      // rtxtOutput
      // 
      this.rtxtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtxtOutput.Location = new System.Drawing.Point(3, 16);
      this.rtxtOutput.Name = "rtxtOutput";
      this.rtxtOutput.Size = new System.Drawing.Size(550, 155);
      this.rtxtOutput.TabIndex = 0;
      this.rtxtOutput.Text = "";
      this.rtxtOutput.WordWrap = false;
      // 
      // grpFiles
      // 
      this.grpFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpFiles.Controls.Add(this.btnProblem);
      this.grpFiles.Controls.Add(this.btnDomain);
      this.grpFiles.Controls.Add(this.lblProblem);
      this.grpFiles.Controls.Add(this.lblDomain);
      this.grpFiles.Controls.Add(this.txtProblem);
      this.grpFiles.Controls.Add(this.txtDomain);
      this.grpFiles.Location = new System.Drawing.Point(12, 12);
      this.grpFiles.Name = "grpFiles";
      this.grpFiles.Size = new System.Drawing.Size(556, 98);
      this.grpFiles.TabIndex = 1;
      this.grpFiles.TabStop = false;
      this.grpFiles.Text = "Domain and problem";
      // 
      // btnProblem
      // 
      this.btnProblem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnProblem.Enabled = false;
      this.btnProblem.Location = new System.Drawing.Point(530, 70);
      this.btnProblem.Name = "btnProblem";
      this.btnProblem.Size = new System.Drawing.Size(20, 20);
      this.btnProblem.TabIndex = 2;
      this.btnProblem.Text = "…";
      this.btnProblem.UseVisualStyleBackColor = true;
      this.btnProblem.Click += new System.EventHandler(this.btnProblem_Click);
      // 
      // btnDomain
      // 
      this.btnDomain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDomain.Location = new System.Drawing.Point(530, 32);
      this.btnDomain.Name = "btnDomain";
      this.btnDomain.Size = new System.Drawing.Size(20, 20);
      this.btnDomain.TabIndex = 2;
      this.btnDomain.Text = "…";
      this.btnDomain.UseVisualStyleBackColor = true;
      this.btnDomain.Click += new System.EventHandler(this.btnDomain_Click);
      // 
      // lblProblem
      // 
      this.lblProblem.AutoSize = true;
      this.lblProblem.Location = new System.Drawing.Point(6, 55);
      this.lblProblem.Name = "lblProblem";
      this.lblProblem.Size = new System.Drawing.Size(64, 13);
      this.lblProblem.TabIndex = 1;
      this.lblProblem.Text = "Problem file:";
      // 
      // lblDomain
      // 
      this.lblDomain.AutoSize = true;
      this.lblDomain.Location = new System.Drawing.Point(6, 16);
      this.lblDomain.Name = "lblDomain";
      this.lblDomain.Size = new System.Drawing.Size(62, 13);
      this.lblDomain.TabIndex = 1;
      this.lblDomain.Text = "Domain file:";
      // 
      // txtProblem
      // 
      this.txtProblem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtProblem.Enabled = false;
      this.txtProblem.Location = new System.Drawing.Point(9, 71);
      this.txtProblem.Name = "txtProblem";
      this.txtProblem.Size = new System.Drawing.Size(515, 20);
      this.txtProblem.TabIndex = 0;
      // 
      // txtDomain
      // 
      this.txtDomain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtDomain.Location = new System.Drawing.Point(9, 32);
      this.txtDomain.Name = "txtDomain";
      this.txtDomain.Size = new System.Drawing.Size(515, 20);
      this.txtDomain.TabIndex = 0;
      // 
      // grpOptions
      // 
      this.grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpOptions.Controls.Add(this.propertyCombobox1);
      this.grpOptions.Controls.Add(this.button1);
      this.grpOptions.Controls.Add(this.fcDomain);
      this.grpOptions.Location = new System.Drawing.Point(12, 116);
      this.grpOptions.Name = "grpOptions";
      this.grpOptions.Size = new System.Drawing.Size(556, 107);
      this.grpOptions.TabIndex = 2;
      this.grpOptions.TabStop = false;
      this.grpOptions.Text = "Options";
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.tbctlOutput);
      this.groupBox1.Location = new System.Drawing.Point(12, 409);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(556, 139);
      this.groupBox1.TabIndex = 3;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Output";
      // 
      // tbctlOutput
      // 
      this.tbctlOutput.Alignment = System.Windows.Forms.TabAlignment.Bottom;
      this.tbctlOutput.Controls.Add(this.tabParsing);
      this.tbctlOutput.Controls.Add(this.tabPage1);
      this.tbctlOutput.Controls.Add(this.tabPage2);
      this.tbctlOutput.Controls.Add(this.tabPage3);
      this.tbctlOutput.Controls.Add(this.tabPage4);
      this.tbctlOutput.Controls.Add(this.tabPage5);
      this.tbctlOutput.Controls.Add(this.tabPage6);
      this.tbctlOutput.Controls.Add(this.tabPage7);
      this.tbctlOutput.Controls.Add(this.tabPage8);
      this.tbctlOutput.Controls.Add(this.tabPage9);
      this.tbctlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbctlOutput.Location = new System.Drawing.Point(3, 16);
      this.tbctlOutput.Name = "tbctlOutput";
      this.tbctlOutput.SelectedIndex = 0;
      this.tbctlOutput.Size = new System.Drawing.Size(550, 120);
      this.tbctlOutput.TabIndex = 0;
      // 
      // tabParsing
      // 
      this.tabParsing.Controls.Add(this.rtxtParsing);
      this.tabParsing.Location = new System.Drawing.Point(4, 4);
      this.tabParsing.Name = "tabParsing";
      this.tabParsing.Padding = new System.Windows.Forms.Padding(3);
      this.tabParsing.Size = new System.Drawing.Size(542, 94);
      this.tabParsing.TabIndex = 0;
      this.tabParsing.Text = "Parsing";
      this.tabParsing.UseVisualStyleBackColor = true;
      // 
      // rtxtParsing
      // 
      this.rtxtParsing.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtxtParsing.Location = new System.Drawing.Point(3, 3);
      this.rtxtParsing.Name = "rtxtParsing";
      this.rtxtParsing.ReadOnly = true;
      this.rtxtParsing.Size = new System.Drawing.Size(536, 88);
      this.rtxtParsing.TabIndex = 0;
      this.rtxtParsing.Text = "";
      // 
      // tabPage1
      // 
      this.tabPage1.Location = new System.Drawing.Point(4, 4);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(542, 94);
      this.tabPage1.TabIndex = 1;
      this.tabPage1.Text = "tabPage1";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Location = new System.Drawing.Point(4, 4);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(542, 94);
      this.tabPage2.TabIndex = 2;
      this.tabPage2.Text = "tabPage2";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // tabPage3
      // 
      this.tabPage3.Location = new System.Drawing.Point(4, 4);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage3.Size = new System.Drawing.Size(542, 94);
      this.tabPage3.TabIndex = 3;
      this.tabPage3.Text = "tabPage3";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // tabPage4
      // 
      this.tabPage4.Location = new System.Drawing.Point(4, 4);
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage4.Size = new System.Drawing.Size(542, 94);
      this.tabPage4.TabIndex = 4;
      this.tabPage4.Text = "tabPage4";
      this.tabPage4.UseVisualStyleBackColor = true;
      // 
      // tabPage5
      // 
      this.tabPage5.Location = new System.Drawing.Point(4, 4);
      this.tabPage5.Name = "tabPage5";
      this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage5.Size = new System.Drawing.Size(542, 94);
      this.tabPage5.TabIndex = 5;
      this.tabPage5.Text = "tabPage5";
      this.tabPage5.UseVisualStyleBackColor = true;
      // 
      // tabPage6
      // 
      this.tabPage6.Location = new System.Drawing.Point(4, 4);
      this.tabPage6.Name = "tabPage6";
      this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage6.Size = new System.Drawing.Size(542, 94);
      this.tabPage6.TabIndex = 6;
      this.tabPage6.Text = "tabPage6";
      this.tabPage6.UseVisualStyleBackColor = true;
      // 
      // tabPage7
      // 
      this.tabPage7.Location = new System.Drawing.Point(4, 4);
      this.tabPage7.Name = "tabPage7";
      this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage7.Size = new System.Drawing.Size(542, 94);
      this.tabPage7.TabIndex = 7;
      this.tabPage7.Text = "tabPage7";
      this.tabPage7.UseVisualStyleBackColor = true;
      // 
      // tabPage8
      // 
      this.tabPage8.Location = new System.Drawing.Point(4, 4);
      this.tabPage8.Name = "tabPage8";
      this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage8.Size = new System.Drawing.Size(542, 94);
      this.tabPage8.TabIndex = 8;
      this.tabPage8.Text = "tabPage8";
      this.tabPage8.UseVisualStyleBackColor = true;
      // 
      // tabPage9
      // 
      this.tabPage9.Location = new System.Drawing.Point(4, 4);
      this.tabPage9.Name = "tabPage9";
      this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage9.Size = new System.Drawing.Size(542, 94);
      this.tabPage9.TabIndex = 9;
      this.tabPage9.Text = "tabPage9";
      this.tabPage9.UseVisualStyleBackColor = true;
      // 
      // btnSolve
      // 
      this.btnSolve.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSolve.Enabled = false;
      this.btnSolve.Location = new System.Drawing.Point(12, 554);
      this.btnSolve.Name = "btnSolve";
      this.btnSolve.Size = new System.Drawing.Size(556, 38);
      this.btnSolve.TabIndex = 4;
      this.btnSolve.Text = "Solve current problem";
      this.btnSolve.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Font = new System.Drawing.Font("Wingdings 3", 6F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Pixel);
      this.button1.Location = new System.Drawing.Point(98, 42);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(21, 21);
      this.button1.TabIndex = 3;
      this.button1.Text = "q";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.DropShadowEnabled = false;
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connardToolStripMenuItem,
            this.autreToolStripMenuItem,
            this.autre2ToolStripMenuItem,
            this.ahhhLalalalalalalalalalalaToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.ShowImageMargin = false;
      this.contextMenuStrip1.Size = new System.Drawing.Size(159, 92);
      // 
      // connardToolStripMenuItem
      // 
      this.connardToolStripMenuItem.Name = "connardToolStripMenuItem";
      this.connardToolStripMenuItem.ShortcutKeyDisplayString = "";
      this.connardToolStripMenuItem.ShowShortcutKeys = false;
      this.connardToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
      this.connardToolStripMenuItem.Text = "Connard";
      // 
      // autreToolStripMenuItem
      // 
      this.autreToolStripMenuItem.Name = "autreToolStripMenuItem";
      this.autreToolStripMenuItem.ShowShortcutKeys = false;
      this.autreToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
      this.autreToolStripMenuItem.Text = "Autre";
      // 
      // autre2ToolStripMenuItem
      // 
      this.autre2ToolStripMenuItem.Name = "autre2ToolStripMenuItem";
      this.autre2ToolStripMenuItem.ShowShortcutKeys = false;
      this.autre2ToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
      this.autre2ToolStripMenuItem.Text = "Autre2";
      // 
      // ahhhLalalalalalalalalalalaToolStripMenuItem
      // 
      this.ahhhLalalalalalalalalalalaToolStripMenuItem.Name = "ahhhLalalalalalalalalalalaToolStripMenuItem";
      this.ahhhLalalalalalalalalalalaToolStripMenuItem.ShowShortcutKeys = false;
      this.ahhhLalalalalalalalalalalaToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
      this.ahhhLalalalalalalalalalalaToolStripMenuItem.Text = "Ahhh lalalalalalalalalalala";
      // 
      // propertyCombobox1
      // 
      this.propertyCombobox1.BackColor = System.Drawing.SystemColors.Window;
      this.propertyCombobox1.Items = new string[] {
        "Allo",
        "Les",
        "Amis"};
      this.propertyCombobox1.Location = new System.Drawing.Point(27, 73);
      this.propertyCombobox1.MinimumSize = new System.Drawing.Size(18, 18);
      this.propertyCombobox1.Name = "propertyCombobox1";
      this.propertyCombobox1.Size = new System.Drawing.Size(101, 18);
      this.propertyCombobox1.TabIndex = 4;
      // 
      // fcDomain
      // 
      this.fcDomain.BackColor = System.Drawing.SystemColors.Window;
      this.fcDomain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.fcDomain.FileName = "";
      this.fcDomain.Location = new System.Drawing.Point(9, 19);
      this.fcDomain.MinimumSize = new System.Drawing.Size(19, 19);
      this.fcDomain.Name = "fcDomain";
      this.fcDomain.Size = new System.Drawing.Size(194, 19);
      this.fcDomain.TabIndex = 0;
      this.fcDomain.VerifyFileChanges = true;
      this.fcDomain.FileChosenNotOk += new System.EventHandler(this.fcDomain_FileChosenNotOk);
      this.fcDomain.FileRenamed += new System.IO.RenamedEventHandler(this.fcDomain_FileRenamed);
      this.fcDomain.FileChosenOk += new System.EventHandler(this.fcDomain_FileChosenOk);
      this.fcDomain.FileChanged += new System.IO.FileSystemEventHandler(this.fcDomain_FileChanged);
      this.fcDomain.FileDeleted += new System.IO.FileSystemEventHandler(this.fcDomain_FileDeleted);
      this.fcDomain.FileCreated += new System.IO.FileSystemEventHandler(this.fcDomain_FileCreated);
      // 
      // TLPlan
      // 
      this.AcceptButton = this.btnSolve;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(580, 598);
      this.Controls.Add(this.btnSolve);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.grpOptions);
      this.Controls.Add(this.grpFiles);
      this.Controls.Add(this.grpOutput);
      this.Name = "TLPlan";
      this.Text = "TLPlan";
      this.grpOutput.ResumeLayout(false);
      this.grpFiles.ResumeLayout(false);
      this.grpFiles.PerformLayout();
      this.grpOptions.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.tbctlOutput.ResumeLayout(false);
      this.tabParsing.ResumeLayout(false);
      this.contextMenuStrip1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox grpOutput;
    private System.Windows.Forms.RichTextBox rtxtOutput;
    private System.Windows.Forms.GroupBox grpFiles;
    private System.Windows.Forms.Label lblProblem;
    private System.Windows.Forms.Label lblDomain;
    private System.Windows.Forms.TextBox txtProblem;
    private System.Windows.Forms.TextBox txtDomain;
    private System.Windows.Forms.Button btnDomain;
    private System.Windows.Forms.Button btnProblem;
    private System.Windows.Forms.GroupBox grpOptions;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TabControl tbctlOutput;
    private System.Windows.Forms.TabPage tabParsing;
    private System.Windows.Forms.Button btnSolve;
    private System.Windows.Forms.RichTextBox rtxtParsing;
    private CustomControls.FileChooser fcDomain;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage tabPage4;
    private System.Windows.Forms.TabPage tabPage5;
    private System.Windows.Forms.TabPage tabPage6;
    private System.Windows.Forms.TabPage tabPage7;
    private System.Windows.Forms.TabPage tabPage8;
    private System.Windows.Forms.TabPage tabPage9;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem connardToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem autreToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem autre2ToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ahhhLalalalalalalalalalalaToolStripMenuItem;
    private CustomControls.PropertyCombobox propertyCombobox1;
  }
}

