using System;
using System.Windows.Forms;
using clojure.lang;

namespace Neptune
{
    public partial class Console : Form
    {

        public Console()
        {
            InitializeComponent();
        }

        private object evalStr(String s)
        {
            try
            {
                return RT.var("clojure.core", "eval-string").invoke(s);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private void evalBtn_Click(object sender, EventArgs e)
        {
            String script = this.cinText.Text;
            Object result = evalStr(script);
            this.coutText.Text = result == null ? "nil" : result.ToString();
        }

        private void clearInBtn_Click(object sender, EventArgs e)
        {
            this.cinText.Text = "";
        }

        private void clearOutBtn_Click(object sender, EventArgs e)
        {
            this.coutText.Text = "";
        }

    }

}
