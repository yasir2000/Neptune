namespace Neptune
{
    partial class NblosEditor
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
            this.nblosButtonPanel = new System.Windows.Forms.Panel();
            this.compileBtn = new System.Windows.Forms.Button();
            this.nbloNameTB = new System.Windows.Forms.TextBox();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.newBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.nblosSplitPanel = new System.Windows.Forms.SplitContainer();
            this.nblosLB = new System.Windows.Forms.ListBox();
            this.nblosTB = new System.Windows.Forms.TextBox();
            this.nblosButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nblosSplitPanel)).BeginInit();
            this.nblosSplitPanel.Panel1.SuspendLayout();
            this.nblosSplitPanel.Panel2.SuspendLayout();
            this.nblosSplitPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nblosButtonPanel
            // 
            this.nblosButtonPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.nblosButtonPanel.Controls.Add(this.compileBtn);
            this.nblosButtonPanel.Controls.Add(this.nbloNameTB);
            this.nblosButtonPanel.Controls.Add(this.deleteBtn);
            this.nblosButtonPanel.Controls.Add(this.newBtn);
            this.nblosButtonPanel.Controls.Add(this.saveBtn);
            this.nblosButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.nblosButtonPanel.Location = new System.Drawing.Point(0, 309);
            this.nblosButtonPanel.Name = "nblosButtonPanel";
            this.nblosButtonPanel.Size = new System.Drawing.Size(580, 30);
            this.nblosButtonPanel.TabIndex = 3;
            // 
            // compileBtn
            // 
            this.compileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.compileBtn.Location = new System.Drawing.Point(242, 4);
            this.compileBtn.Name = "compileBtn";
            this.compileBtn.Size = new System.Drawing.Size(75, 23);
            this.compileBtn.TabIndex = 5;
            this.compileBtn.Text = "Compile";
            this.compileBtn.UseVisualStyleBackColor = true;
            this.compileBtn.Click += new System.EventHandler(this.compileBtn_Click);
            // 
            // nbloNameTB
            // 
            this.nbloNameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nbloNameTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.nbloNameTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nbloNameTB.ForeColor = System.Drawing.Color.Honeydew;
            this.nbloNameTB.Location = new System.Drawing.Point(3, 3);
            this.nbloNameTB.Name = "nbloNameTB";
            this.nbloNameTB.Size = new System.Drawing.Size(223, 24);
            this.nbloNameTB.TabIndex = 4;
            // 
            // deleteBtn
            // 
            this.deleteBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteBtn.Location = new System.Drawing.Point(493, 4);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(73, 23);
            this.deleteBtn.TabIndex = 3;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // newBtn
            // 
            this.newBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newBtn.Location = new System.Drawing.Point(410, 4);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(73, 23);
            this.newBtn.TabIndex = 2;
            this.newBtn.Text = "New";
            this.newBtn.UseVisualStyleBackColor = true;
            this.newBtn.Click += new System.EventHandler(this.newBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Location = new System.Drawing.Point(327, 4);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(73, 23);
            this.saveBtn.TabIndex = 1;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // nblosSplitPanel
            // 
            this.nblosSplitPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nblosSplitPanel.Location = new System.Drawing.Point(7, 2);
            this.nblosSplitPanel.Name = "nblosSplitPanel";
            // 
            // nblosSplitPanel.Panel1
            // 
            this.nblosSplitPanel.Panel1.Controls.Add(this.nblosLB);
            // 
            // nblosSplitPanel.Panel2
            // 
            this.nblosSplitPanel.Panel2.Controls.Add(this.nblosTB);
            this.nblosSplitPanel.Size = new System.Drawing.Size(572, 305);
            this.nblosSplitPanel.SplitterDistance = 121;
            this.nblosSplitPanel.TabIndex = 4;
            // 
            // nblosLB
            // 
            this.nblosLB.BackColor = System.Drawing.Color.LightSlateGray;
            this.nblosLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nblosLB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nblosLB.ForeColor = System.Drawing.Color.Honeydew;
            this.nblosLB.FormattingEnabled = true;
            this.nblosLB.IntegralHeight = false;
            this.nblosLB.ItemHeight = 17;
            this.nblosLB.Location = new System.Drawing.Point(0, 0);
            this.nblosLB.Name = "nblosLB";
            this.nblosLB.Size = new System.Drawing.Size(121, 305);
            this.nblosLB.TabIndex = 0;
            this.nblosLB.SelectedIndexChanged += new System.EventHandler(this.onNbloSelected);
            // 
            // nblosTB
            // 
            this.nblosTB.BackColor = System.Drawing.Color.DarkSlateGray;
            this.nblosTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nblosTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nblosTB.ForeColor = System.Drawing.Color.White;
            this.nblosTB.Location = new System.Drawing.Point(0, 0);
            this.nblosTB.Multiline = true;
            this.nblosTB.Name = "nblosTB";
            this.nblosTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.nblosTB.Size = new System.Drawing.Size(447, 305);
            this.nblosTB.TabIndex = 0;
            this.nblosTB.WordWrap = false;
            // 
            // NblosEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 339);
            this.Controls.Add(this.nblosSplitPanel);
            this.Controls.Add(this.nblosButtonPanel);
            this.Name = "NblosEditor";
            this.Text = "NBLO Editor";
            this.nblosButtonPanel.ResumeLayout(false);
            this.nblosButtonPanel.PerformLayout();
            this.nblosSplitPanel.Panel1.ResumeLayout(false);
            this.nblosSplitPanel.Panel2.ResumeLayout(false);
            this.nblosSplitPanel.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nblosSplitPanel)).EndInit();
            this.nblosSplitPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel nblosButtonPanel;
        private System.Windows.Forms.Button compileBtn;
        private System.Windows.Forms.TextBox nbloNameTB;
        private System.Windows.Forms.Button deleteBtn;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.SplitContainer nblosSplitPanel;
        private System.Windows.Forms.ListBox nblosLB;
        private System.Windows.Forms.TextBox nblosTB;

    }
}