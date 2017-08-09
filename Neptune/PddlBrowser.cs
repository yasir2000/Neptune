using System;
using System.IO;
using System.Windows.Forms;
using clojure.lang;

namespace Neptune
{
    public partial class PddlBrowser : Form
    {
        const string dirpath = "../../pddl/petshop";
        private string dirtemplate;

        public PddlBrowser()
        {
            InitializeComponent();
            dirtemplate = String.Format("{0}/{1}.pddl", dirpath, "{0}");
            refresh();
        }

        private void clear()
        {
            this.fileNameTB.Text = "";
            this.sourceTB.Text = "";
        }

        private void refresh()
        {
            clear();
            int dirlen = dirpath.Length;
            string[] files = Directory.GetFiles(dirpath);
            this.fileList.Items.Clear();
            foreach (string fname in files)
                this.fileList.Items.Add(fname.Substring(dirpath.Length + 1, fname.Length - dirlen - 6));
        }

        private void onScriptSelected(object sender, EventArgs e)
        {
            string fname = this.fileList.SelectedItem as string;
            readScript(fname);
        }

        private void deleteScript()
        {
            try
            {
                File.Delete(String.Format(dirtemplate, this.fileNameTB.Text));
            }
            catch (Exception)
            {
            }
            refresh();
        }

        private void newScript()
        {
            refresh();
            this.fileNameTB.Text = "new-pddl";
            this.sourceTB.Text = "";
        }

        private void readScript(string fname)
        {
            this.fileNameTB.Text = fname;
            string path = String.Format(dirtemplate, this.fileNameTB.Text);
            if (File.Exists(path))
                this.sourceTB.Text = File.ReadAllText(path);
        }

        private void writeScript(string fname)
        {
            File.WriteAllText(String.Format(dirtemplate, this.fileNameTB.Text), this.sourceTB.Text);
            NeptuneIDE.printTranscript(String.Format("Updated script [{0}]", fname));
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string fname = this.fileNameTB.Text.Trim();
            if (fname.Length > 0)
                writeScript(fname);
            refresh();
        }

         private void newBtn_Click(object sender, EventArgs e)
        {
            newScript();
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            deleteScript();
        }

        private void editMethodsBtn_Click(object sender, EventArgs e)
        {
            (new MethodsEditor()).Show();
        }

        private void editNblosBtn_Click(object sender, EventArgs e)
        {
            (new NblosEditor()).Show();
        }

     }
}
