using System;
using System.Collections;
using System.Windows.Forms;

namespace Neptune
{
    public partial class NbloListSelect : Form
    {
        private string selection = "";

        public NbloListSelect()
        {
            InitializeComponent();
        }

        public void SetList(ArrayList arrayList)
        {
            this.listBox.Items.Clear();
            foreach (string selection in arrayList)
                this.listBox.Items.Add(selection);
        }

        public string GetSelection()
        {
            return this.selection;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            this.selection = this.listBox.SelectedItem as string;
            this.Close();
        }
    }
}
