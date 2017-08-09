namespace Neptune
{
    partial class Console
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.clearOutBtn = new System.Windows.Forms.Button();
            this.clearInBtn = new System.Windows.Forms.Button();
            this.evalBtn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.coutText = new System.Windows.Forms.TextBox();
            this.cinText = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel1.Controls.Add(this.clearOutBtn);
            this.panel1.Controls.Add(this.clearInBtn);
            this.panel1.Controls.Add(this.evalBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 332);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(583, 30);
            this.panel1.TabIndex = 0;
            // 
            // clearOutBtn
            // 
            this.clearOutBtn.Location = new System.Drawing.Point(221, 4);
            this.clearOutBtn.Name = "clearOutBtn";
            this.clearOutBtn.Size = new System.Drawing.Size(75, 23);
            this.clearOutBtn.TabIndex = 2;
            this.clearOutBtn.Text = "Clear Out";
            this.clearOutBtn.UseVisualStyleBackColor = true;
            this.clearOutBtn.Click += new System.EventHandler(this.clearOutBtn_Click);
            // 
            // clearInBtn
            // 
            this.clearInBtn.Location = new System.Drawing.Point(122, 4);
            this.clearInBtn.Name = "clearInBtn";
            this.clearInBtn.Size = new System.Drawing.Size(75, 23);
            this.clearInBtn.TabIndex = 1;
            this.clearInBtn.Text = "Clear In";
            this.clearInBtn.UseVisualStyleBackColor = true;
            this.clearInBtn.Click += new System.EventHandler(this.clearInBtn_Click);
            // 
            // evalBtn
            // 
            this.evalBtn.Location = new System.Drawing.Point(23, 4);
            this.evalBtn.Name = "evalBtn";
            this.evalBtn.Size = new System.Drawing.Size(75, 23);
            this.evalBtn.TabIndex = 0;
            this.evalBtn.Text = "Eval";
            this.evalBtn.UseVisualStyleBackColor = true;
            this.evalBtn.Click += new System.EventHandler(this.evalBtn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.coutText);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cinText);
            this.splitContainer1.Size = new System.Drawing.Size(583, 332);
            this.splitContainer1.SplitterDistance = 194;
            this.splitContainer1.TabIndex = 1;
            // 
            // coutText
            // 
            this.coutText.BackColor = System.Drawing.Color.Black;
            this.coutText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coutText.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coutText.ForeColor = System.Drawing.Color.Honeydew;
            this.coutText.Location = new System.Drawing.Point(0, 0);
            this.coutText.Multiline = true;
            this.coutText.Name = "coutText";
            this.coutText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.coutText.Size = new System.Drawing.Size(583, 194);
            this.coutText.TabIndex = 0;
            this.coutText.WordWrap = false;
            // 
            // cinText
            // 
            this.cinText.BackColor = System.Drawing.Color.Black;
            this.cinText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cinText.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cinText.ForeColor = System.Drawing.Color.Yellow;
            this.cinText.Location = new System.Drawing.Point(0, 0);
            this.cinText.Multiline = true;
            this.cinText.Name = "cinText";
            this.cinText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.cinText.Size = new System.Drawing.Size(583, 134);
            this.cinText.TabIndex = 0;
            this.cinText.WordWrap = false;
            // 
            // Console
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 362);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "Console";
            this.Text = "Clojure Console";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button clearOutBtn;
        private System.Windows.Forms.Button clearInBtn;
        private System.Windows.Forms.Button evalBtn;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox coutText;
        private System.Windows.Forms.TextBox cinText;
    }
}

