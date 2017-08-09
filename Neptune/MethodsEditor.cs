using System;
using System.Collections;
using System.Windows.Forms;
using clojure.lang;

namespace Neptune
{
    public partial class MethodsEditor : Form
    {
        const string NEPTUNE_SCRIPT = "neptune-script";
        const string NEPTUNE_SPC = "neptune-spc";

        private string scriptName = "";

        public MethodsEditor()
        {
            InitializeComponent();
            setScriptName(NEPTUNE_SCRIPT);
        }

        private void setScriptName(string name)
        {
            scriptName = name;
            showMethods();
        }

        private void clear()
        {
            this.methodsTB.Text = "";
            this.methodNameTB.Text = "";
            this.methodsLB.Items.Clear();
        }

        private bool isTopLevel()
        {
            return scriptName == NEPTUNE_SPC;
        }

        private void showMethods()
        {
            clear();
            ArrayList methods = RT.var("clojure.core", "method-keys-array").invoke(scriptName) as ArrayList;
            foreach (string method in methods)
                this.methodsLB.Items.Add(method);
        }

        private void onMethodSelected(object sender, EventArgs e)
        {
            string mname = String.Format("{0}", this.methodsLB.SelectedItem);
            this.methodNameTB.Text = mname;
            showMethod(mname);
        }

        private void showMethod(string selector)
        {
            string text = RT.var("clojure.core", "methods-get-selector").invoke(scriptName, selector) as string;
            this.methodsTB.Text = text;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string selector = this.methodNameTB.Text;
            string src = this.methodsTB.Text;
            string reply = RT.var("clojure.core", "methods-save-selector").invoke(scriptName, selector, src) as string;
            if (isTopLevel())
                RT.var("clojure.core", "clear-cache").invoke();
            MessageBox.Show(reply);
            showMethods();
        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            clear();
            this.methodNameTB.Text = "newMethod";
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            string selector = this.methodNameTB.Text;
            string reply = RT.var("clojure.core", "methods-delete-selector").invoke(scriptName, selector) as string;
            MessageBox.Show(reply);
            showMethods();
        }

        private void formatBtn_Click(object sender, EventArgs e)
        {
            string src = this.methodsTB.Text;
            this.methodsTB.Text = RT.var("clojure.core", "methods-format").invoke(src) as string;
        }

        private void loadedCKB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.loadedCKB.Checked)
                setScriptName(NEPTUNE_SPC);
            else
                setScriptName(NEPTUNE_SCRIPT);
        }

    }
}
