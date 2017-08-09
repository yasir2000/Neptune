using System;
using System.Collections;
using System.Windows.Forms;
using clojure.lang;

namespace Neptune
{
    public partial class NblosEditor : Form
    {
        public NblosEditor()
        {
            InitializeComponent();
            showNblos();
        }

        private void clear()
        {
            this.nblosTB.Text = "";
            this.nbloNameTB.Text = "";
            this.nblosLB.Items.Clear();
        }

        private void showNblos()
        {
            clear();
            ArrayList nblos = RT.var("clojure.core", "nblo-keys-array").invoke() as ArrayList;
            foreach (string nblo in nblos)
                this.nblosLB.Items.Add(nblo);
        }

        private void onNbloSelected(object sender, EventArgs e)
        {
            string nbloName = String.Format("{0}", this.nblosLB.SelectedItem);
            this.nbloNameTB.Text = nbloName;
            showNblo(nbloName);
        }

        private void showNblo(string nbloName)
        {
            string text = RT.var("clojure.core", "nblo-get-name").invoke(nbloName) as string;
            this.nblosTB.Text = text;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string nbloName = this.nbloNameTB.Text;
            string src = this.nblosTB.Text;
            string reply = RT.var("clojure.core", "nblo-save-src").invoke(nbloName, src) as string;
            MessageBox.Show(reply);
            showNblos();
        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            clear();
            this.nbloNameTB.Text = "newNBLO";
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            string nbloName = this.nbloNameTB.Text;
            string reply = RT.var("clojure.core", "nblo-delete-src").invoke(nbloName) as string;
            MessageBox.Show(reply);
            showNblos();
        }

        private void compileBtn_Click(object sender, EventArgs e)
        {

        }

    }
}
