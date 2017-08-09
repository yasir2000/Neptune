namespace Neptune
{
    partial class NbloRadioButtons
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.choice_1 = new System.Windows.Forms.RadioButton();
            this.choice_2 = new System.Windows.Forms.RadioButton();
            this.questionLabel = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.BackColor = System.Drawing.Color.LightSteelBlue;
            this.groupBox.Controls.Add(this.okBtn);
            this.groupBox.Controls.Add(this.questionLabel);
            this.groupBox.Controls.Add(this.choice_2);
            this.groupBox.Controls.Add(this.choice_1);
            this.groupBox.Location = new System.Drawing.Point(46, 32);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(279, 187);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Please select one";
            // 
            // choice_1
            // 
            this.choice_1.AutoSize = true;
            this.choice_1.BackColor = System.Drawing.Color.Transparent;
            this.choice_1.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.choice_1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.choice_1.Location = new System.Drawing.Point(50, 65);
            this.choice_1.Name = "choice_1";
            this.choice_1.Size = new System.Drawing.Size(98, 21);
            this.choice_1.TabIndex = 0;
            this.choice_1.TabStop = true;
            this.choice_1.Text = "Choice 1";
            this.choice_1.UseVisualStyleBackColor = false;
            // 
            // choice_2
            // 
            this.choice_2.AutoSize = true;
            this.choice_2.BackColor = System.Drawing.Color.Transparent;
            this.choice_2.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.choice_2.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.choice_2.Location = new System.Drawing.Point(50, 94);
            this.choice_2.Name = "choice_2";
            this.choice_2.Size = new System.Drawing.Size(98, 21);
            this.choice_2.TabIndex = 1;
            this.choice_2.TabStop = true;
            this.choice_2.Text = "Choice 2";
            this.choice_2.UseVisualStyleBackColor = false;
            // 
            // questionLabel
            // 
            this.questionLabel.AutoSize = true;
            this.questionLabel.BackColor = System.Drawing.Color.Transparent;
            this.questionLabel.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.questionLabel.ForeColor = System.Drawing.Color.MidnightBlue;
            this.questionLabel.Location = new System.Drawing.Point(35, 33);
            this.questionLabel.Name = "questionLabel";
            this.questionLabel.Size = new System.Drawing.Size(80, 17);
            this.questionLabel.TabIndex = 2;
            this.questionLabel.Text = "Question";
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(38, 138);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(91, 23);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "Ok";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // RadioButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.ClientSize = new System.Drawing.Size(366, 262);
            this.Controls.Add(this.groupBox);
            this.Name = "RadioButtons";
            this.Text = "RadioButtons";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label questionLabel;
        private System.Windows.Forms.RadioButton choice_2;
        private System.Windows.Forms.RadioButton choice_1;
        private System.Windows.Forms.Button okBtn;
    }
}