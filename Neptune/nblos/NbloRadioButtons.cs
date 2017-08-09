using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neptune
{
    public partial class NbloRadioButtons : Form
    {
        private string selection = "";

        public NbloRadioButtons()
        {
            InitializeComponent();
        }

        public string GetSelection()
        {
            return this.selection;
        }

        public void SetQuestion(string text)
        {
            this.questionLabel.Text = text;
        }

        public void SetChoice_1(string text)
        {
            this.choice_1.Text = text;
        }

        public void SetChoice_2(string text)
        {
            this.choice_2.Text = text;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (this.choice_1.Checked)
                this.selection = this.choice_1.Text;
            else
                this.selection = this.choice_2.Text;
            this.Close();
        }

    }
}
