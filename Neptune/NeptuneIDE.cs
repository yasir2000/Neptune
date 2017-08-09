using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using clojure.lang;
using GUI;

namespace Neptune
{
    public partial class NeptuneIDE : Form
    {
        const string NEPTUNE_IDE_VERSION = "1.0";
        const string NEPTUNE_GRAMMAR_VERSION = "2.1";
        private static readonly Symbol CLOJURE_MAIN = Symbol.intern("clojure.main");
        private static readonly Symbol CLOJURE_REFLECT = Symbol.intern("clojure.reflect");
        private static readonly Var REQUIRE = RT.var("clojure.core", "require");
        private static NeptuneIDE instance;
        private static List<string> transcriptQueue;
        private static List<string> outputQueue;
        private static List<string> errorQueue;
        private static List<PddlRunner> pddlRunnerQueue;
        private static bool pddlIsRunning;

        public NeptuneIDE()
        {
            InitializeComponent();
            transcriptQueue = new List<string>();
            outputQueue = new List<string>();
            errorQueue = new List<string>();
            pddlRunnerQueue = new List<PddlRunner>();
            pddlIsRunning = false;
            System.Environment.SetEnvironmentVariable("clojure.load.path", "clojure");
            instance = this;
            String ds = String.Format("{0:yyyy-MMMM-dd}", DateTime.Now);
            printTranscript(ds);
            printTranscript(String.Format("IDE Version {0}", NEPTUNE_IDE_VERSION));
            printTranscript(String.Format("NS Grammar Version {0}", NEPTUNE_GRAMMAR_VERSION));
            printTranscript("Initializing...");
            REQUIRE.invoke(CLOJURE_MAIN);
            REQUIRE.invoke(CLOJURE_REFLECT);
            Compiler.load(new StringReader("(load-file \"../../clojure/init.clj\")"), ".");
            printTranscript("Ready");
        }

        public static string printTranscript(String txt)
        {
            transcriptQueue.Add(txt);
            return txt;
        }

        private void printOnForegroundThread()
        {
            while (transcriptQueue.Count > 0)
            {
                printTextBox(instance.transcriptTB, transcriptQueue[0], true);
                transcriptQueue.RemoveAt(0);
            }
            while (outputQueue.Count > 0)
            {
                printTextBox(instance.outputTB, outputQueue[0], false);
                outputQueue.RemoveAt(0);
            }
            while (errorQueue.Count > 0)
            {
                printTextBox(instance.errorsTB, errorQueue[0], false);
                errorQueue.RemoveAt(0);
            }
        }

        public static void pddlRun(string name, string dir, string dfilename, string pfilename)
        {
            PddlRunner runner = new PddlRunner(name, dir, dfilename, pfilename);
            pddlRunnerQueue.Add(runner);
        }

        private void scheduleNextPddlRun()
        {
            if (pddlIsRunning || pddlRunnerQueue.Count == 0)
                return;
            Thread workerThread = new Thread(delegate()
            {
                PddlRunner runner = pddlRunnerQueue[0];
                printTranscript(String.Format("PDDL process [{0}] is running", runner.Name));
                pddlRunnerQueue.RemoveAt(0);
                runner.Solve();
                printTranscript(String.Format("PDDL process [{0}] has ended", runner.Name));
                printOutput(runner.TraceReport());
                newlineOutput();
                printError(String.Format("PDDL [{0}] Errors Begin ======================", runner.Name));
                string errors = runner.Errors().Trim();
                if (errors.Length > 0)
                    printError(errors);
                printError(String.Format("PDDL [{0}] Errors End ========================", runner.Name));
                newlineError();
                pddlIsRunning = false;
            });
            pddlIsRunning = true;
            workerThread.Start();
        }

        public static string printOutput(String txt)
        {
            txt = txt.Trim();
            if (txt.Length > 0)
                outputQueue.Add(txt);
            return txt;
        }

        public static void newlineOutput()
        {
            outputQueue.Add("");
        }

        public static void newlineError()
        {
            errorQueue.Add("");
        }

        public static string printError(String txt)
        {
            txt = txt.Trim();
            if (txt.Length > 0)
                errorQueue.Add(txt);
            return txt;
        }

        private static string printTextBox(TextBox textBox, String txt, bool withTimeStamp)
        {
            String oldText = textBox.Text;
            String txtLine;
            if (withTimeStamp)
            {
                String ts = String.Format("{0:HH:mm:ss}", DateTime.Now);
                txtLine = String.Format("{0} {1}\r\n", ts, txt);
            }
            else
                txtLine = String.Format("{0}\r\n", txt);
            textBox.Text = oldText + txtLine;
            textBox.Select(instance.transcriptTB.Text.Length, 0);
            textBox.ScrollToCaret();
            return txt;
        }

        private void menuFileExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuScripts(object sender, EventArgs e)
        {
            (new ScriptBrowser()).Show();
        }

        private void menuTlplan(object sender, EventArgs e)
        {
            (new OptionChooser()).Show();
        }

        private void menuActionsCopy(object sender, EventArgs e)
        {
            TextBox textBox = this.tabPanel.SelectedTab.Controls[0] as TextBox;
            if (textBox != null)
                Clipboard.SetText(textBox.Text);
        }

        private void menuActionsClear(object sender, EventArgs e)
        {
            TextBox textBox = this.tabPanel.SelectedTab.Controls[0] as TextBox;
            if (textBox != null)
                textBox.Text = "";
        }

        private void menuActionsRun(object sender, EventArgs e)
        {
            RT.var("clojure.core", "neptune-run").invoke();
        }

        private void menuActionsConsole(object sender, EventArgs e)
        {
            (new Console()).Show();
        }

        private void timerTick(object sender, EventArgs e)
        {
            printOnForegroundThread();
            scheduleNextPddlRun();
        }

        private void pDDLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new PddlBrowser()).Show();
        }


    }
}
