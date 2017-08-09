namespace Neptune
{
    partial class MethodsEditor
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
            this.methodsSplitPanel = new System.Windows.Forms.SplitContainer();
            this.methodsLB = new System.Windows.Forms.ListBox();
            this.methodsTB = new System.Windows.Forms.TextBox();
            this.methodsButtonPanel = new System.Windows.Forms.Panel();
            this.formatBtn = new System.Windows.Forms.Button();
            this.methodNameTB = new System.Windows.Forms.TextBox();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.newBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.loadedCKB = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.methodsSplitPanel)).BeginInit();
            this.methodsSplitPanel.Panel1.SuspendLayout();
            this.methodsSplitPanel.Panel2.SuspendLayout();
            this.methodsSplitPanel.SuspendLayout();
            this.methodsButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // methodsSplitPanel
            // 
            this.methodsSplitPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.methodsSplitPanel.Location = new System.Drawing.Point(0, 0);
            this.methodsSplitPanel.Name = "methodsSplitPanel";
            // 
            // methodsSplitPanel.Panel1
            // 
            this.methodsSplitPanel.Panel1.Controls.Add(this.methodsLB);
            // 
            // methodsSplitPanel.Panel2
            // 
            this.methodsSplitPanel.Panel2.Controls.Add(this.methodsTB);
            this.methodsSplitPanel.Size = new System.Drawing.Size(567, 335);
            this.methodsSplitPanel.SplitterDistance = 120;
            this.methodsSplitPanel.TabIndex = 1;
            // 
            // methodsLB
            // 
            this.methodsLB.BackColor = System.Drawing.Color.LightSlateGray;
            this.methodsLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodsLB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.methodsLB.ForeColor = System.Drawing.Color.Honeydew;
            this.methodsLB.FormattingEnabled = true;
            this.methodsLB.IntegralHeight = false;
            this.methodsLB.ItemHeight = 17;
            this.methodsLB.Location = new System.Drawing.Point(0, 0);
            this.methodsLB.Name = "methodsLB";
            this.methodsLB.Size = new System.Drawing.Size(120, 335);
            this.methodsLB.TabIndex = 0;
            this.methodsLB.SelectedIndexChanged += new System.EventHandler(this.onMethodSelected);
            // 
            // methodsTB
            // 
            this.methodsTB.BackColor = System.Drawing.Color.DarkSlateGray;
            this.methodsTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodsTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.methodsTB.ForeColor = System.Drawing.Color.White;
            this.methodsTB.Location = new System.Drawing.Point(0, 0);
            this.methodsTB.Multiline = true;
            this.methodsTB.Name = "methodsTB";
            this.methodsTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.methodsTB.Size = new System.Drawing.Size(443, 335);
            this.methodsTB.TabIndex = 0;
            this.methodsTB.WordWrap = false;
            // 
            // methodsButtonPanel
            // 
            this.methodsButtonPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.methodsButtonPanel.Controls.Add(this.loadedCKB);
            this.methodsButtonPanel.Controls.Add(this.formatBtn);
            this.methodsButtonPanel.Controls.Add(this.methodNameTB);
            this.methodsButtonPanel.Controls.Add(this.deleteBtn);
            this.methodsButtonPanel.Controls.Add(this.newBtn);
            this.methodsButtonPanel.Controls.Add(this.saveBtn);
            this.methodsButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.methodsButtonPanel.Location = new System.Drawing.Point(0, 337);
            this.methodsButtonPanel.Name = "methodsButtonPanel";
            this.methodsButtonPanel.Size = new System.Drawing.Size(567, 30);
            this.methodsButtonPanel.TabIndex = 2;
            // 
            // formatBtn
            // 
            this.formatBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.formatBtn.Location = new System.Drawing.Point(255, 4);
            this.formatBtn.Name = "formatBtn";
            this.formatBtn.Size = new System.Drawing.Size(63, 23);
            this.formatBtn.TabIndex = 5;
            this.formatBtn.Text = "Format";
            this.formatBtn.UseVisualStyleBackColor = true;
            this.formatBtn.Click += new System.EventHandler(this.formatBtn_Click);
            // 
            // methodNameTB
            // 
            this.methodNameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.methodNameTB.BackColor = System.Drawing.Color.LightSlateGray;
            this.methodNameTB.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.methodNameTB.ForeColor = System.Drawing.Color.Honeydew;
            this.methodNameTB.Location = new System.Drawing.Point(3, 3);
            this.methodNameTB.Name = "methodNameTB";
            this.methodNameTB.Size = new System.Drawing.Size(159, 24);
            this.methodNameTB.TabIndex = 4;
            // 
            // deleteBtn
            // 
            this.deleteBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteBtn.Location = new System.Drawing.Point(492, 4);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(63, 23);
            this.deleteBtn.TabIndex = 3;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // newBtn
            // 
            this.newBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newBtn.Location = new System.Drawing.Point(413, 4);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(63, 23);
            this.newBtn.TabIndex = 2;
            this.newBtn.Text = "New";
            this.newBtn.UseVisualStyleBackColor = true;
            this.newBtn.Click += new System.EventHandler(this.newBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Location = new System.Drawing.Point(334, 4);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(63, 23);
            this.saveBtn.TabIndex = 1;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // loadedCKB
            // 
            this.loadedCKB.AutoSize = true;
            this.loadedCKB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadedCKB.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.loadedCKB.Location = new System.Drawing.Point(168, 5);
            this.loadedCKB.Name = "loadedCKB";
            this.loadedCKB.Size = new System.Drawing.Size(75, 20);
            this.loadedCKB.TabIndex = 6;
            this.loadedCKB.Text = "Loaded";
            this.loadedCKB.UseVisualStyleBackColor = true;
            this.loadedCKB.CheckedChanged += new System.EventHandler(this.loadedCKB_CheckedChanged);
            // 
            // MethodsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 367);
            this.Controls.Add(this.methodsButtonPanel);
            this.Controls.Add(this.methodsSplitPanel);
            this.Name = "MethodsEditor";
            this.Text = "Methods Editor";
            this.methodsSplitPanel.Panel1.ResumeLayout(false);
            this.methodsSplitPanel.Panel2.ResumeLayout(false);
            this.methodsSplitPanel.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.methodsSplitPanel)).EndInit();
            this.methodsSplitPanel.ResumeLayout(false);
            this.methodsButtonPanel.ResumeLayout(false);
            this.methodsButtonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer methodsSplitPanel;
        private System.Windows.Forms.ListBox methodsLB;
        private System.Windows.Forms.TextBox methodsTB;
        private System.Windows.Forms.Panel methodsButtonPanel;
        private System.Windows.Forms.TextBox methodNameTB;
        private System.Windows.Forms.Button deleteBtn;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button formatBtn;
        private System.Windows.Forms.CheckBox loadedCKB;
    }
}