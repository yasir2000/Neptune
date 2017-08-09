//
// Copyright (c) 2009 Froduald Kabanza and the Université de Sherbrooke.
// Use of this software is permitted for non-commercial research purposes, and
// it may be copied or applied only for that use. All copies must include this
// copyright message.
// 
// This is a research prototype and it has not gone through intensive tests and
// is delivered as is. It may still contain bugs. Froduald Kabanza and the
// Université de Sherbrooke disclaim any responsibility for damage that may be
// caused by using it.
// 
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PDDLParser.Exp;
using PDDLParser.Exp.Struct;
using PDDLParser.Parser;
using TLPlan;
using TLPlan.Validator;
using TLPlan.World;

namespace GUI
{
  /// <summary>
  /// Represents a planning GUI window for the TLPlan planner. It displays parsing and planning
  /// information, and allows limited queries on worlds.
  /// </summary>
  public partial class ViewerForm : Form
  {
    #region Nested Classes

    /// <summary>
    /// Represents a <see cref="System.IO.TextWriter"/> which processes the text written to it
    /// using an <see cref="System.Action"/> delegate.
    /// </summary>
    private class EventWriter : TextWriter
    {
      /// <summary>
      /// The <see cref="System.Action"/> delegate used to process all text written to this object.
      /// </summary>
      Action<string> m_writeString;
      
      /// <summary>
      /// Creates a new event writer with a given delegate.
      /// </summary>
      /// <param name="writeStringAction">The delegate which will process the text written to this object.</param>
      public EventWriter(Action<string> writeStringAction)
      {
        m_writeString = writeStringAction;
      }

      /// <summary>
      /// Writes a string to the text stream. This string is processed using this object <see cref="System.Action"/> delegate.
      /// </summary>
      /// <param name="value">The string to write.</param>
      public override void Write(string value)
      {
        m_writeString(value);
      }

      /// <summary>
      /// Writes a character to the text stream.
      /// This character is processed using this object <see cref="System.Action"/> delegate.
      /// </summary>
      /// <param name="value">The character to write to the text stream.</param>
      public override void Write(char value)
      {
        Write(value.ToString());
      }

      /// <summary>
      /// Writes a subarray of characters to the text stream.
      /// This subarray of characters is processed using this object <see cref="System.Action"/> delegate.
      /// </summary>
      /// <param name="buffer">The character array to write data from.</param>
      /// <param name="index">Starting index in the buffer.</param>
      /// <param name="count">The number of characters to write.</param>
      public override void Write(char[] buffer, int index, int count)
      {
        Write(new string(buffer, index, count));
      }

      /// <summary>
      /// Returns <see cref="System.Text.Encoding.Default"/>.
      /// </summary>
      public override Encoding Encoding
      {
        get { return Encoding.Default; }
      }
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The asynchronous wrapper around the planner.
    /// </summary>
    AsyncPlannerWrapper m_asyncPlanner;
    /// <summary>
    /// The last plan found.
    /// </summary>
    Plan m_lastPlan;

    /// <summary>
    /// Whether to parse and then immediately solve.
    /// </summary>
    bool m_parseAndSolve;
    /// <summary>
    /// Whether to restart upon stopping.
    /// </summary>
    bool m_restart;

    /// <summary>
    /// The writer to which normal, informative messages are written.
    /// </summary>
    TextWriter m_infoWriter;
    /// <summary>
    /// The writer to which warning or important messages are written.
    /// </summary>
    TextWriter m_warningWriter;
    /// <summary>
    /// The writer to which errors are written.
    /// </summary>
    TextWriter m_errorWriter;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new viewer form with the given TLPlan options.
    /// </summary>
    /// <param name="options">The TLPlan options to use.</param>
    public ViewerForm(TLPlanOptions options)
      : this(new GUITLPlanOptions(options))
    { }

    /// <summary>
    /// Creates a new viewer form with the given GUI TLPlan options.
    /// </summary>
    /// <param name="guiOptions">The GUI TLPlan options to use.</param>
    public ViewerForm(GUITLPlanOptions guiOptions)
    {
      InitializeComponent();

      guiOptions.MakeReadOnly();
      guiOptions.HideNonTLPlanOptions();
      propertyGrid.SelectedObject = guiOptions;

      TLPlanOptions options = guiOptions.ConvertToTLPlanOptions();

      Statistics stats = new Statistics();
      stats.Options = options;

      m_warningWriter = new EventWriter(PrintWarning);
      m_errorWriter = new EventWriter(PrintError);
      m_infoWriter = new EventWriter(PrintInfo);

      Planner planner = new Planner(options, stats, m_errorWriter, m_infoWriter);
      m_lastPlan = null;

     Parser parser = new Parser(new ErrorManager(m_warningWriter, m_errorWriter));

      m_asyncPlanner = new AsyncPlannerWrapper(parser, planner);

      m_parseAndSolve = false;
      m_restart = false;

      m_asyncPlanner.StateChanged += new EventHandler<PlannerStateChangedEventArgs>(AsyncPlanner_StateChanged);
      m_asyncPlanner.Parsed += new EventHandler<ParsedEventArgs>(AsyncPlanner_Parsed);
      m_asyncPlanner.PlanFound += new EventHandler<PlanFoundEventArgs>(AsyncPlanner_PlanFound);
      m_asyncPlanner.PlanValidated += new EventHandler<PlanValidatedEventArgs>(AsyncPlanner_PlanValidated);
      m_asyncPlanner.ExceptionRaised += new EventHandler<ExceptionEventArgs>(AsyncPlanner_ExceptionRaised);

      tsbtnPauseInitial.Checked = m_asyncPlanner.PauseOnInitialWorld = guiOptions.PauseOnInitialWorld;
      tsbtnPauseGoal.Checked = planner.PauseOnGoalWorld = guiOptions.PauseOnGoalWorld;
      tssbtnPrintWorld.Tag = false;

      this.UpdateStatistics();
    }

    /// <summary>
    /// Prints a message describing the given state of the asynchronous planner.
    /// </summary>
    /// <param name="state">The state to describe.</param>
    private void PrintStateMessage(AsyncPlannerWrapper.State state)
    {
      string strPrint = string.Empty;
      string strStatus = string.Empty;
      switch (state)
      {
        case AsyncPlannerWrapper.State.None:       break; // Never happens
        case AsyncPlannerWrapper.State.Parsing:    strStatus = strPrint = "Parsing..."; break;
        case AsyncPlannerWrapper.State.Parsed:     strStatus = "Parsed."; break; // strPrint handled elsewhere
        case AsyncPlannerWrapper.State.Solving:    strStatus = strPrint = "Solving..."; break;
        case AsyncPlannerWrapper.State.Solved:     strStatus = (m_lastPlan != null ? "Solved." : "Not solved."); break; // strPrint handled elsewhere
        case AsyncPlannerWrapper.State.Validating: strStatus = strPrint = "Validating..."; break;
        case AsyncPlannerWrapper.State.Validated:  strStatus = "Validated."; break; // strPrint handled elsewhere
        case AsyncPlannerWrapper.State.Stepping:   strStatus = strPrint = "Stepping..."; break;
        case AsyncPlannerWrapper.State.Pausing:    strStatus = strPrint = "Pausing..."; break;
        case AsyncPlannerWrapper.State.Paused:     strStatus = strPrint = "Paused."; break;
        case AsyncPlannerWrapper.State.Unpausing:  strStatus = strPrint = "Solving..."; break;
        case AsyncPlannerWrapper.State.Unpaused:   break; // No message.
        case AsyncPlannerWrapper.State.Stopping:   strStatus = strPrint = "Stopping..."; break;
        case AsyncPlannerWrapper.State.Stopped:    strStatus = strPrint = "Stopped.";  break;
      }

      if (strPrint != string.Empty)
        PrintWarning(strPrint + "\n");
      if (strStatus != string.Empty)
        statusStrip.Invoke(new EventHandler((sender, e) => tsslblState.Text = strStatus));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Parses and solves the domain and problem specified in the planner's options.
    /// </summary>
    /// <remarks>
    /// This should only be called right after the form has been created; after that,
    /// the user will want full control over what is happening...
    /// </remarks>
    public void Start()
    {
      // TODO: Move this in AsyncPlannerWrapper?
      m_parseAndSolve = true;
      m_asyncPlanner.AsynchronousParse();
    }

    /// <summary>
    /// Updates the statistics in the status bar.
    /// </summary>
    public void UpdateStatistics()
    {
      if (this.m_asyncPlanner != null)
      {
        Statistics stats = this.m_asyncPlanner.Planner.Statistics;
        string strStats = string.Format("{0:0.00000} secs. Nodes examined: {1}",
                                        stats.ParseTime + stats.SolveTime, stats.ExaminedNodeCount);

        if (statusStrip.InvokeRequired)
          statusStrip.Invoke(new EventHandler((s, e) => tsslblStatistics.Text = strStats));
        else
          tsslblStatistics.Text = strStats;
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the form's controls so that they are appropriately enabled or disabled,
    /// depending on the given asynchronous planner's state.
    /// </summary>
    /// <param name="state">The asynchronous planner's state.</param>
    private void UpdateControls(AsyncPlannerWrapper.State state)
    {
      bool validating = state == AsyncPlannerWrapper.State.Validating;
      bool parsing = state == AsyncPlannerWrapper.State.Parsing;
      bool stopDisabled = false;
      bool startPauseStepEnabled = false;
      bool paused = (state == AsyncPlannerWrapper.State.Paused);
      
      if (!validating)
      {
        stopDisabled = (state == AsyncPlannerWrapper.State.None || state == AsyncPlannerWrapper.State.Parsed ||
                        state == AsyncPlannerWrapper.State.Solved || state == AsyncPlannerWrapper.State.Stopped ||
                        state == AsyncPlannerWrapper.State.Validated);
        startPauseStepEnabled = (stopDisabled || state == AsyncPlannerWrapper.State.Paused);
      }

      this.Invoke(new EventHandler((s, args) =>
        {
          tsbtnStart.Enabled = startPauseStepEnabled;
          tsbtnPause.Enabled = !startPauseStepEnabled && !parsing;
          tsbtnStep.Enabled = startPauseStepEnabled;
          tsbtnStop.Enabled = !stopDisabled && !parsing;
          tsbtnRestart.Enabled = !validating && !parsing;

          commandTextBox.Enabled = paused;
          tssbtnPrintWorld.Enabled = paused;

          tspbWorking.Visible = !startPauseStepEnabled;
        }));
    }

    /// <summary>
    /// Prints an error to the main text box.
    /// </summary>
    /// <param name="s">The error to print.</param>
    private void PrintError(string s)
    {
      outputRichTextBox.Invoke(new EventHandler((sender, e) =>
      {
        outputRichTextBox.SelectionStart = outputRichTextBox.Text.Length;
        outputRichTextBox.SelectionColor = Color.OrangeRed;
        outputRichTextBox.SelectedText = s;
      }));
    }

    /// <summary>
    /// Prints a warning to the main text box.
    /// </summary>
    /// <param name="s">The warning to print.</param>
    private void PrintWarning(string s)
    {
      outputRichTextBox.Invoke(new EventHandler((sender, e) =>
      {
        outputRichTextBox.SelectionStart = outputRichTextBox.Text.Length;
        outputRichTextBox.SelectionColor = Color.DarkRed;
        outputRichTextBox.SelectedText = s;
      }));
    }

    /// <summary>
    /// Print a normal, informative string to the main text box.
    /// </summary>
    /// <param name="s">The string to print.</param>
    private void PrintInfo(string s)
    {
      outputRichTextBox.Invoke(new EventHandler((sender, e) =>
      {
        outputRichTextBox.SelectionStart = outputRichTextBox.Text.Length;
        outputRichTextBox.SelectionColor = Color.Black;
        outputRichTextBox.SelectedText = s;
      }));
    }

    /// <summary>
    /// Prints a command or a command result to the main text box.
    /// </summary>
    /// <param name="s">The string to print.</param>
    private void PrintCommand(string s)
    {
      outputRichTextBox.Invoke(new EventHandler((sender, e) =>
      {
        outputRichTextBox.SelectionStart = outputRichTextBox.Text.Length;
        outputRichTextBox.SelectionColor = Color.DarkBlue;
        outputRichTextBox.SelectedText = s;
      }));
    }

    /// <summary>
    /// Prints the current world to the main text box.
    /// </summary>
    /// <remarks>This assumes a paused planner state.</remarks>
    /// <param name="printAllPredicates">Whether to show all predicates, or only true and defined ones.</param>
    private void PrintWorld(bool printAllPredicates)
    {
      if (printAllPredicates)
        m_errorWriter.WriteLine("Printing all world predicates and fluents...");
      else
        m_errorWriter.WriteLine("Printing the world's true predicates and defined fluents...");

      Node node = m_asyncPlanner.Planner.CurrentNode;
      WorldUtils.PrintWorld(m_asyncPlanner.Problem, node, WorldUtils.PrintWorldPart.All, printAllPredicates, true, m_warningWriter, m_infoWriter);
    }

    #endregion

    #region Event Handlers

    #region Toolbar and Menu Events

    #region Toolbar buttons

    private void toolStripButton_CheckedChanged(object sender, EventArgs e)
    {
      splitContainer.Panel2Collapsed = !toolStripButton.Checked;
    }

    private void tsbtnStart_Click(object sender, EventArgs e)
    {
      if (m_asyncPlanner.CurrentState == AsyncPlannerWrapper.State.Paused)
      {
        m_asyncPlanner.AsynchronousUnpause();
      }
      else
      {
        m_asyncPlanner.AsynchronousSolve();
      }
    }

    private void tsbtnStep_Click(object sender, EventArgs e)
    {
      m_asyncPlanner.AsynchronousStep();
    }

    private void tsbtnPause_Click(object sender, EventArgs e)
    {
      m_asyncPlanner.AsynchronousPause();
    }

    private void tsbtnStop_Click(object sender, EventArgs e)
    {
      m_asyncPlanner.AsynchronousStop();
    }

    private void tsbtnRestart_Click(object sender, EventArgs e)
    {
      if (!m_asyncPlanner.IsStopped)
      {
        m_restart = true;
        m_asyncPlanner.AsynchronousStop();
      }
      else
      {
        m_parseAndSolve = true;
        m_asyncPlanner.AsynchronousParse();
      }
      
    }

    private void tsbtnSave_Click(object sender, EventArgs e)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      saveFileDialog.Filter = "RTF Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        string fileName = saveFileDialog.FileName;
        bool textFile = Path.GetExtension(fileName) == ".txt";
        outputRichTextBox.SaveFile(fileName, textFile ? RichTextBoxStreamType.PlainText : RichTextBoxStreamType.RichText);
      }
    }

    private void tsbtnCopy_Click(object sender, EventArgs e)
    {
      outputRichTextBox.Copy();
    }

    private void tsbtnClear_Click(object sender, EventArgs e)
    {
      outputRichTextBox.Clear();
    }

    private void tssbtnPrintWorld_ButtonClick(object sender, EventArgs e)
    {
      PrintWorld((bool)tssbtnPrintWorld.Tag);
    }

    private void printWorldToolStripMenuItem_Click(object sender, EventArgs e)
    {
      tssbtnPrintWorld.Text = printWorldToolStripMenuItem.Text;
      tssbtnPrintWorld.ToolTipText = printWorldToolStripMenuItem.ToolTipText;
      tssbtnPrintWorld.Tag = false;
      PrintWorld(false);
    }

    private void printAllWorldToolStripMenuItem_Click(object sender, EventArgs e)
    {
      tssbtnPrintWorld.Text = printAllWorldToolStripMenuItem.Text;
      tssbtnPrintWorld.ToolTipText = printAllWorldToolStripMenuItem.ToolTipText;
      tssbtnPrintWorld.Tag = true;
      PrintWorld(true);
    }

    private void tsbtnPauseInitial_Click(object sender, EventArgs e)
    {
      m_asyncPlanner.PauseOnInitialWorld = tsbtnPauseInitial.Checked;
    }
    
    private void tsbtnPauseGoal_Click(object sender, EventArgs e)
    {
      m_asyncPlanner.Planner.PauseOnGoalWorld = tsbtnPauseGoal.Checked;
    }

    private void tsbtnPrintStats_Click(object sender, EventArgs e)
    {
      m_warningWriter.WriteLine(m_asyncPlanner.Planner.Statistics.ToString());
    }

    #endregion

    #region Context Menu
    
    private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      outputRichTextBox.SelectAll();
    }

    private void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      outputRichTextBox.Copy();
    }

    private void clearToolStripMenuItem_Click(object sender, EventArgs e)
    {
      outputRichTextBox.Clear();
    }

    #endregion

    #endregion

    #region Textbox Events

    private void commandTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      if (e.KeyCode == Keys.Enter && commandTextBox.Text != string.Empty)
      {
        try
        {
          PrintCommand(string.Format("Evaluating {0}\n", commandTextBox.Text));
          // Process command! :D
          IEvaluableExp exp = m_asyncPlanner.Parser.parseEvaluableExp(commandTextBox.Text);
          string result = string.Empty;
          TLPlanReadOnlyDurativeClosedWorld world = m_asyncPlanner.Planner.CurrentNode.World;

          if (exp is ILogicalExp)
          {
            ILogicalExp logicalExp = (ILogicalExp)exp;

            Bool value = logicalExp.Evaluate(world, LocalBindings.EmptyBindings);
            result = value.ToString();
          }
          else if (exp is INumericExp)
          {
            INumericExp numericExp = (INumericExp)exp;
            PDDLParser.Exp.Struct.Double value = numericExp.Evaluate(world, LocalBindings.EmptyBindings);
            result = value.ToString();
          }
          else if (exp is ITerm)
          {
            ITerm term = (ITerm)exp;
            ConstantExp value = term.Evaluate(world, LocalBindings.EmptyBindings);
            result = value.ToString();
          }
          else
          {
            throw new InvalidOperationException(string.Format("{0} is not a recognized evaluable expression.", commandTextBox.Text));
          }

          PrintCommand(string.Format("Result: {0}\n\n", result));
          commandTextBox.AutoCompleteCustomSource.Add(commandTextBox.Text);
          commandTextBox.Clear();
        }
        catch (System.Exception exception)
        {
          PrintWarning(exception.Message + "\n");
          commandTextBox.SelectAll();
        }
      }
    }

    private void commandTextBox_Enter(object sender, EventArgs e)
    {
      if (commandTextBox.ForeColor == Color.DarkGray) // No text entered
      {
        commandTextBox.ForeColor = SystemColors.WindowText;
        commandTextBox.Text = string.Empty;
      }
    }

    private void commandTextBox_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(commandTextBox.Text))
      {
        commandTextBox.ForeColor = Color.DarkGray;
        commandTextBox.Text = "Enter commands here";
      }
    }

    #endregion

    #region Other Interface Events

    private void ViewerForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      m_asyncPlanner.Dispose();
    }

    #endregion

    #endregion

    #region Parsing and Planning Events

    private void AsyncPlanner_PlanValidated(object sender, PlanValidatedEventArgs e)
    {
      if (e.PlanValid)
      {
        PrintWarning("Plan successfully validated!\n");
      }
      else
      {
        PrintError("Plan invalid!\n\n");
        PrintWarning("Validator output:\n");
        if (!string.IsNullOrEmpty(e.Error.Trim()))
          PrintError(e.Error.Trim());
        else
          PrintInfo(e.Out);
        PrintInfo("\n");
      }
    }

    private void AsyncPlanner_PlanFound(object sender, PlanFoundEventArgs e)
    {
      m_lastPlan = e.Plan;
      UpdateStatistics();
    }

    private void AsyncPlanner_Parsed(object sender, ParsedEventArgs e)
    {
      if (e.ParsedSuccessfully)
      {
        m_asyncPlanner.Parser.getErrorManager().print(ErrorManager.Message.WARNING);

        m_infoWriter.WriteLine("\nparsing domain \"" + m_asyncPlanner.Problem.DomainName + "\" done successfully ...");
        m_infoWriter.WriteLine("parsing problem \"" + m_asyncPlanner.Problem.ProblemName + "\" done successfully ...\n");

        if (m_parseAndSolve)
        {
          m_parseAndSolve = false;
          m_asyncPlanner.AsynchronousSolve();
        }
      }
      else
      {
        m_asyncPlanner.Parser.getErrorManager().print(ErrorManager.Message.ALL);
      }
    }

    private void AsyncPlanner_StateChanged(object sender, PlannerStateChangedEventArgs e)
    {
      UpdateControls(e.NewState);
      PrintStateMessage(e.NewState);

      switch (e.NewState)
      {
        //case AsyncPlannerWrapper.State.Parsed:
        //  if (m_parseAndSolve)
        //  {
        //    m_parseAndSolve = false;
        //    m_asyncPlanner.AsynchronousSolve();
        //  }
        //  break;
        case AsyncPlannerWrapper.State.Paused:
          UpdateStatistics();
          break;
        //case AsyncPlannerWrapper.State.Stopped:
        //  if (m_restart)
        //  {
        //    m_restart = false;
        //    m_asyncPlanner.AsynchronousSolve();
        //  }
        //  break;
        case AsyncPlannerWrapper.State.Solved:
          if (!m_restart)
          {
            m_infoWriter.WriteLine(m_asyncPlanner.Planner.Statistics);
          }

          if (m_lastPlan != null)
          {
            m_warningWriter.WriteLine("Operators:");
            m_lastPlan.PrintOperators(m_infoWriter);

            m_warningWriter.WriteLine("\nOrder:");
            m_lastPlan.PrintOrder(m_infoWriter);

            m_warningWriter.WriteLine("\nPlan:");
            m_lastPlan.PrintPlan(m_infoWriter);

            m_warningWriter.Write("Plan cost: ");
            m_lastPlan.PrintMetric(m_infoWriter);
          }
          else
          {
            if (m_restart)
            {
              m_restart = false;
              m_parseAndSolve = true;
              m_asyncPlanner.AsynchronousParse();
            }
            else
            {
              m_infoWriter.WriteLine("No plan found!");
            }
          }

          m_lastPlan = null;

          break;
      }
    }

    private void AsyncPlanner_ExceptionRaised(object sender, ExceptionEventArgs e)  
    {
      m_errorWriter.WriteLine("An exception was thrown!");
      m_errorWriter.WriteLine(e.Exception);
    }

    #endregion
  }

  #region Private Classes

  /// <summary>
  /// Represents an asynchronous wrapper over the TLPlan planner that uses threads to parse and solve problems.
  /// </summary>
  sealed class AsyncPlannerWrapper : IDisposable
  {
    #region Private Fields

    /// <summary>
    /// The worker thread used to parse and solve problems.
    /// </summary>
    /// <remarks>
    /// For now, a new thread is created for each operation. Therefore, parsing and planning use two threads consecutively.
    /// This member may be null, for example, when the planning is aborted. If it is not null, it is still doing some work
    /// (either parsing or solving).
    /// </remarks>
    private Thread m_workerThread;

    /// <summary>
    /// Whether to examine only one world at a time.
    /// </summary>
    private bool m_stepOnce;

    #endregion

    #region Enumerations

    /// <summary>
    /// The different states the planner can be in.
    /// </summary>
    public enum State
    {
      /// <summary>
      /// The state in which the planner is right after its creation.
      /// </summary>
      /// <remarks>Once the planner changes states, there is no way to come back to the <see cref="State.None"/> state.</remarks>
      None,
      /// <summary>
      /// The planner is currently parsing or linking domain and problem files.
      /// </summary>
      Parsing,
      /// <summary>
      /// The planner has finished parsing or linking domain and problem files.
      /// </summary>
      Parsed,
      /// <summary>
      /// The planner is currently solving the given problem.
      /// </summary>
      Solving,
      /// <summary>
      /// The planner has finished solving the given problem.
      /// </summary>
      Solved,
      /// <summary>
      /// The planner is currently validating the plan found for the given problem.
      /// </summary>
      Validating,
      /// <summary>
      /// The planner has finished validating the plan found for the given problem.
      /// </summary>
      Validated,
      /// <summary>
      /// The planner is currently stepping to the next world in the current problem.
      /// </summary>
      Stepping,
      /// <summary>
      /// The planner is currently pausing the solving.
      /// </summary>
      Pausing,
      /// <summary>
      /// The planner has paused the solving.
      /// </summary>
      Paused,
      /// <summary>
      /// The planner is currently resuming the solving.
      /// </summary>
      Unpausing,
      /// <summary>
      /// The planner has resumed the solving.
      /// </summary>
      Unpaused,
      /// <summary>
      /// The planner is currently stopping the solving.
      /// </summary>
      Stopping,
      /// <summary>
      /// The planner has stopped the solving.
      /// </summary>
      Stopped,
    }

    #endregion

    #region Properties

    /// <summary>
    /// The wrapped planner.
    /// </summary>
    public Planner Planner { get; private set; }
    /// <summary>
    /// The parser used.
    /// </summary>
    public Parser Parser { get; private set; }
    /// <summary>
    /// The problem that is being solved.
    /// </summary>
    /// <remarks>
    /// This can be null if the problem was not parsed correctly, or if the parsing is still in progress.
    /// </remarks>
    public PDDLObject Problem { get; private set; }

    /// <summary>
    /// The current state of the planner.
    /// </summary>
    public State CurrentState { get; private set; }

    /// <summary>
    /// Whether the planner is in a state where the problem has been parsed correctly.
    /// </summary>
    public bool IsParsed
    {
      get { return CurrentState != State.None && CurrentState != State.Parsing && Problem != null; }
    }

    /// <summary>
    /// Whether the planner is in a stopped state.
    /// </summary>
    public bool IsStopped
    {
      get { return CurrentState == State.Parsed || CurrentState == State.Solved ||
                   CurrentState == State.Stopped || CurrentState == State.None ||
                   CurrentState == State.Validated; }
    }

    /// <summary>
    /// Gets or sets whether to pause on the initial world (i.e. at the start of the search).
    /// </summary>
    public bool PauseOnInitialWorld { get; set; }

    #endregion

    #region Events and Delegates

    /// <summary>
    /// An event fired when the planner's state changes.
    /// </summary>
    public event EventHandler<PlannerStateChangedEventArgs> StateChanged;
    /// <summary>
    /// An event fired when the parsing and linking of a domain and problem are finished.
    /// </summary>
    /// <remarks>
    /// The <see cref="ParsedEventArgs.ParsedSuccessfully"/> property indicates whether the
    /// parsing was successful.
    /// Also, the <see cref="AsyncPlannerWrapper.StateChanged"/> event is fired just prior to this event,
    /// changing the state to <see cref="AsyncPlannerWrapper.State.Parsed"/>.
    /// </remarks>
    public event EventHandler<ParsedEventArgs> Parsed;
    /// <summary>
    /// An event fired when a plan is found for a given problem.
    /// </summary>
    /// <remarks>
    /// As of now, this is only called once, but it could ultmately be called more than once for a single
    /// problem (e.g. when using iterative deepening)
    /// </remarks>
    public event EventHandler<PlanFoundEventArgs> PlanFound;
    /// <summary>
    /// An event fired when the validation of a plan has finished.
    /// </summary>
    /// <remarks>
    /// The <see cref="AsyncPlannerWrapper.StateChanged"/> event is fired just after to this event,
    /// changing the state to <see cref="AsyncPlannerWrapper.State.Validated"/>.
    /// </remarks>
    public event EventHandler<PlanValidatedEventArgs> PlanValidated;
    /// <summary>
    /// An event fired when an exception occurs while parsing or planning.
    /// </summary>
    public event EventHandler<ExceptionEventArgs> ExceptionRaised;

    #endregion

    #region Constructors/Destructors

    /// <summary>
    /// Creates a new asynchronous wrapper for the given planner.
    /// </summary>
    /// <param name="parser">The parser used to parse domains and problems.</param>
    /// <param name="planner">The planner to wrap.</param>
    public AsyncPlannerWrapper(Parser parser, Planner planner)
    {
      this.Parser = parser;
      this.Planner = planner;
      this.Problem = null;
      this.CurrentState = State.None;
      
      this.m_workerThread = null;

      this.Planner.Paused += new EventHandler(Planner_Paused);
      this.Planner.Unpaused += new EventHandler(Planner_Unpaused);
      this.Planner.Started += new EventHandler(Planner_Started);
      this.Planner.Stopped += new EventHandler(Planner_Stopped);
      this.Planner.PlanningFinished += new EventHandler<Planner.PlanningFinishedEventArgs>(Planner_PlanningFinished);

      this.m_stepOnce = false;
    }

    /// <summary>
    /// Disposes of the running thread, if any.
    /// </summary>
    ~AsyncPlannerWrapper()
    {
      Dispose(true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Asynchronously parses the domain and problem found in the planner's options.
    /// </summary>
    /// <remarks>
    /// This assumes a stopped state.
    /// </remarks>
    public void AsynchronousParse()
    {
      System.Diagnostics.Debug.Assert(IsStopped);
      System.Diagnostics.Debug.Assert(m_workerThread == null);

      this.Planner.Statistics.Reset();

      FireStateChanged(State.Parsing);

      m_workerThread = new Thread(delegate()
      {
        // TODO: Try/catch this.
        // Gets the error manager of the pddl compiler
        ErrorManager mgr = Parser.getErrorManager();
        mgr.clear();

        bool parsed = false;

        Planner.Statistics.Reset();
        Planner.Statistics.StartParse();
        Problem = null;

        try
        {
          PDDLObject domain = Parser.parse(Planner.Options.Domain);
          PDDLObject problem = Parser.parse(Planner.Options.Problem);
          if (domain != null && problem != null)
          {
            Problem = Parser.link(domain, problem);
          }
        }
        catch { } // Errors have been logged in the error manager.

        Planner.Statistics.StopParse();

        // If the compilation produces errors we stop
        if (mgr.Contains(ErrorManager.Message.ERROR))
        {
          Problem = null;
        }
        // else parsing worked
        else
        {
          parsed = (Problem != null);
        }

        m_workerThread = null;
        FireParsed(parsed);
      });

      m_workerThread.Start();
    }

    /// <summary>
    /// Asynchronously solves the parsed problem.
    /// </summary>
    /// <remarks>
    /// <para>This assumes a parsed and stopped state.</para>
    /// <para>Also, this might only step to the first (inital) world, depending on the current options.</para>
    /// </remarks>
    public void AsynchronousSolve()
    {
      System.Diagnostics.Debug.Assert((IsParsed && IsStopped) || CurrentState == State.Stepping);
      System.Diagnostics.Debug.Assert(m_workerThread == null);

      this.Planner.Statistics.ResetSolve();

      if (!m_stepOnce)
        FireStateChanged(State.Solving);

      if (m_stepOnce || PauseOnInitialWorld)
      {
        m_stepOnce = false;
        Planner.Pause();
      }

      m_workerThread = new Thread(delegate()
      {
        try
        {
          // Rely on events to get the plan(s).
          Plan lastPlan = Planner.Solve(Problem);
          m_workerThread = null;
          FireStateChanged(State.Solved);

          if (Planner.Options.ValidatePlan && lastPlan != null)
            AsynchronousValidate(lastPlan);
        }
        catch (System.Exception e)
        {
          if (!(e is ThreadAbortException))
          {
            FireExceptionRaised(e);
            m_workerThread = null;
            FireStateChanged(State.Stopped);
          }
        }
      });

      m_workerThread.Start();
    }

    /// <summary>
    /// Asynchronously steps to the next world in the current planning.
    /// </summary>
    /// <remarks>
    /// This assumes a paused state, or a parsed and stopped state.
    /// </remarks>
    public void AsynchronousStep()
    {
      System.Diagnostics.Debug.Assert(CurrentState == State.Paused || (IsParsed && IsStopped));

      State oldState = CurrentState;
      FireStateChanged(State.Stepping);
      m_stepOnce = true;

      if (oldState == State.Paused)
      {
        Planner.Unpause();
      }
      else
      {
        AsynchronousSolve();
      }
    }

    /// <summary>
    /// Asynchronously pauses the current planning session.
    /// </summary>
    /// <remarks>
    /// This assumes a running state.
    /// </remarks>
    public void AsynchronousPause()
    {
      if (CurrentState != State.Stopping)
      {
        System.Diagnostics.Debug.Assert(CurrentState == State.Parsing || CurrentState == State.Solving || CurrentState == State.Unpaused);

        FireStateChanged(State.Pausing);
        Planner.Pause();
      }
    }

    /// <summary>
    /// Asynchronously resumes the current planning session.
    /// </summary>
    /// <remarks>
    /// This assumes a paused state.
    /// </remarks>
    public void AsynchronousUnpause()
    {
      System.Diagnostics.Debug.Assert(CurrentState == State.Paused);

      FireStateChanged(State.Unpausing);
      Planner.Unpause();
    }

    /// <summary>
    /// Asynchronously stops a planning session.
    /// </summary>
    /// <remarks>
    /// This assumes a running state.
    /// </remarks>
    public void AsynchronousStop()
    {
      System.Diagnostics.Debug.Assert(!(IsParsed && IsStopped));

      FireStateChanged(State.Stopping);
      Planner.Stop();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Asynchronously validates the given plan.
    /// </summary>
    /// <param name="plan">The plan to validate.</param>
    /// <remarks>This assumes a solved state.</remarks>
    private void AsynchronousValidate(Plan plan)
    {
      System.Diagnostics.Debug.Assert(CurrentState == State.Solved);
      System.Diagnostics.Debug.Assert(plan != null);
      System.Diagnostics.Debug.Assert(Planner.Options.ValidatePlan);

      FireStateChanged(State.Validating);

      m_workerThread = new Thread(delegate()
      {
        try
        {
          TLPlanOptions options = Planner.Options;
          PlanValidator planValidator = new PlanValidator(new ValidatorOptions(), new StringWriter(), new StringWriter());
          bool planValid = planValidator.Validate(options.ValidationDomain, options.Problem, plan);
          m_workerThread = null;
          FirePlanValidated(planValid, planValidator.Out.ToString(), planValidator.Error.ToString());
          FireStateChanged(State.Validated);
        }
        catch (System.Exception e)
        {
          FireExceptionRaised(e);
          m_workerThread = null;
          FireStateChanged(State.Stopped);
        }
      });

      m_workerThread.Start();
    }

    /// <summary>
    /// Disposes of the worker thread, if it is still running.
    /// </summary>
    /// <param name="inDestructor">Whether this is called from the destructor.</param>
    private void Dispose(bool inDestructor)
    {
      Thread threadToKill = m_workerThread; // Saves multithreading work (as it may be nulled anytime)
      m_workerThread = null;
      if (threadToKill != null)
      {
        threadToKill.Abort();
        threadToKill.Join(); // TODO: Do something else?
      }
    }

    #region Event Handlers

    /// <summary>
    /// Handles the <see cref="TLPlan.Planner.PlanningFinished"/> event by forwarding the event.
    /// </summary>
    /// <param name="sender">The planner.</param>
    /// <param name="e">The event's argument.</param>
    private void Planner_PlanningFinished(object sender, Planner.PlanningFinishedEventArgs e)
    {
      if (e.ProblemSolved)
        FirePlanFound(e.Plan);
    }

    /// <summary>
    /// Handles tje <see cref="TLPlan.Planner.Stopped"/> event by forwarding the event.
    /// </summary>
    /// <param name="sender">The planner.</param>
    /// <param name="e">The event's argument.</param>
    private void Planner_Stopped(object sender, EventArgs e)
    {
      FireStateChanged(State.Stopped);
    }

    /// <summary>
    /// Handles the <see cref="TLPlan.Planner.Started"/> event. This currently does nothing.
    /// </summary>
    /// <param name="sender">The planner.</param>
    /// <param name="e">The event's argument.</param>
    private void Planner_Started(object sender, EventArgs e)
    {
      //FireStateChanged(State.Solving);
    }

    /// <summary>
    /// Handles the <see cref="TLPlan.Planner.Unpaused"/> event by forwarding the event.
    /// </summary>
    /// <remarks>This also handles the stepping logic.</remarks>
    /// <param name="sender">The planner.</param>
    /// <param name="e">The event's argument.</param>
    private void Planner_Unpaused(object sender, EventArgs e)
    {
      if (m_stepOnce)
      {
        m_stepOnce = false;
        Planner.Pause();
      }
      else
      {
        FireStateChanged(State.Unpaused);
      }
    }

    /// <summary>
    /// Handles the <see cref="TLPlan.Planner.Paused"/> event by forwarding the event.
    /// </summary>
    /// <param name="sender">The planner.</param>
    /// <param name="e">The event's argument.</param>
    private void Planner_Paused(object sender, EventArgs e)
    {
      FireStateChanged(State.Paused);
    }

    #endregion

    #region Fire Events

    /// <summary>
    /// Fires the <see cref="StateChanged"/> event.
    /// </summary>
    /// <remarks>
    /// This also performs the change from the old state to the new one.
    /// </remarks>
    /// <param name="newState">The new state.</param>
    private void FireStateChanged(State newState)
    {
      State oldState = CurrentState;
      CurrentState = newState;

      if (StateChanged != null)
        StateChanged(this, new PlannerStateChangedEventArgs(oldState, newState));
    }

    /// <summary>
    /// Fire the <see cref="Parsed"/> event.
    /// </summary>
    /// <param name="success">Whether parsing was successful.</param>
    private void FireParsed(bool success)
    {
      FireStateChanged(State.Parsed);
      if (Parsed != null)
        Parsed(this, new ParsedEventArgs(success));
    }

    /// <summary>
    /// Fires the <see cref="PlanFound"/> event.
    /// </summary>
    /// <param name="plan">The plan which was found.</param>
    private void FirePlanFound(Plan plan)
    {
      if (PlanFound != null)
        PlanFound(this, new PlanFoundEventArgs(plan));
    }

    /// <summary>
    /// Fires the <see cref="PlanValidated"/> event.
    /// </summary>
    /// <param name="planValid">Whether the plan was valid.</param>
    /// <param name="output">The standard output of the validator.</param>
    /// <param name="errorOutput">The error output of the validator.</param>
    private void FirePlanValidated(bool planValid, string output, string errorOutput)
    {
      if (PlanValidated != null)
        PlanValidated(this, new PlanValidatedEventArgs(planValid, output, errorOutput));
    }

    /// <summary>
    /// Fires the <see cref="ExceptionRaised"/> event.
    /// </summary>
    /// <param name="e">The exception that was thrown.</param>
    private void FireExceptionRaised(System.Exception e)
    {
      if (ExceptionRaised != null)
        ExceptionRaised(this, new ExceptionEventArgs(e));
    }

    #endregion

    #endregion

    #region IDisposable Interface

    /// <summary>
    /// Disposes of the worker thread properly.
    /// </summary>
    public void Dispose()
    {
      Dispose(false);
      GC.SuppressFinalize(this);
    }

    #endregion
  }

  #endregion

  #region Private EventArgs Classes

  /// <summary>
  /// Represents a class containing event data for the <see cref="AsyncPlannerWrapper.StateChanged"/> event.
  /// </summary>
  class PlannerStateChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Represents the previous state in which the planner was before this event was triggered.
    /// </summary>
    public AsyncPlannerWrapper.State OldState { get; private set; }
    /// <summary>
    /// Represents the new state in which the planner is now.
    /// </summary>
    public AsyncPlannerWrapper.State NewState { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="PlannerStateChangedEventArgs"/>.
    /// </summary>
    /// <param name="oldState">The previous state in which the planner was before this event was triggered.</param>
    /// <param name="newState">The new state in which the planner is now.</param>
    public PlannerStateChangedEventArgs(AsyncPlannerWrapper.State oldState, AsyncPlannerWrapper.State newState)
    {
      OldState = oldState;
      NewState = newState;
    }
  }

  /// <summary>
  /// Represents a class containing event data for the <see cref="AsyncPlannerWrapper.Parsed"/> event.
  /// </summary>
  class ParsedEventArgs : EventArgs
  {
    /// <summary>
    /// Whether the domain and problem were parsed and linked successfully.
    /// </summary>
    public bool ParsedSuccessfully { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="ParsedEventArgs"/>.
    /// </summary>
    /// <param name="parsedSuccessfully">Whether the domain and problem were parsed and linked successfully.</param>
    public ParsedEventArgs(bool parsedSuccessfully)
    {
      ParsedSuccessfully = parsedSuccessfully;
    }
  }

  /// <summary>
  /// Represents a class containing event data for the <see cref="AsyncPlannerWrapper.PlanFound"/> event.
  /// </summary>
  class PlanFoundEventArgs : EventArgs
  {
    /// <summary>
    /// The plan which was found by the planner.
    /// </summary>
    public Plan Plan { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="PlanFoundEventArgs"/>.
    /// </summary>
    /// <param name="plan">The plan which was found by the planner.</param>
    public PlanFoundEventArgs(Plan plan)
    {
      Plan = plan;
    }
  }

  /// <summary>
  /// Represents a class containing event data for the <see cref="AsyncPlannerWrapper.PlanValidated"/> event.
  /// </summary>
  class PlanValidatedEventArgs : EventArgs
  {
    /// <summary>
    /// Whether the plan was valid.
    /// </summary>
    public bool PlanValid { get; private set; }
    /// <summary>
    /// The standard output of the validator.
    /// </summary>
    public string Out { get; private set; }
    /// <summary>
    /// The error output of the validator.
    /// </summary>
    public string Error { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="PlanValidatedEventArgs"/>.
    /// </summary>
    /// <param name="planValid">Whether the plan was valid.</param>
    /// <param name="output">The standard output of the validator.</param>
    /// <param name="errorOutput">The error output of the validator.</param>
    public PlanValidatedEventArgs(bool planValid, string output, string errorOutput)
    {
      PlanValid = planValid;
      Out = output;
      Error = errorOutput;
    }
  }

  /// <summary>
  /// Represents a class containing event data for the <see cref="AsyncPlannerWrapper.ExceptionRaised"/> event.
  /// </summary>
  class ExceptionEventArgs : EventArgs
  {
    /// <summary>
    /// The exception which was raised.
    /// </summary>
    public System.Exception Exception { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="ExceptionEventArgs"/>.
    /// </summary>
    /// <param name="e">The exception which was raised.</param>
    public ExceptionEventArgs(System.Exception e)
    {
      Exception = e;
    }
  }

  #endregion
}
