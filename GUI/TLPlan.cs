using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PDDLParser;
using TLPlan;

namespace GUI
{
  public partial class TLPlan : Form
  {
    #region Private Fields

    //private Parser m_parser;

    //private PDDLObject m_domain;
    //private PDDLObject m_currentProblem;

    #endregion

    #region Properties

    public Parser Parser { get; set; }
    public ErrorManager ErrorManager { get; set; }
    public StringWriter ErrorStream { get; set; }
    public PDDLObject Domain { get; set; }
    public PDDLObject CurrentProblem { get; set; }

    //public Parser Parser
    //{
    //  get { return m_parser; }
    //  set { m_parser = value; }
    //}

    //public PDDLObject Domain
    //{
    //  get { return m_domain; }
    //  set { m_domain = value; }
    //}

    //public PDDLObject CurrentProblem
    //{
    //  get { return m_currentProblem; }
    //  set { m_currentProblem = value; }
    //}

    #endregion

    #region Constructors

    public TLPlan()
    {
      InitializeComponent();

      Domain = null;
      CurrentProblem = null;

      ErrorStream = new StringWriter();
      ErrorManager = new ErrorManager(ErrorStream, ErrorStream);
      Parser = new Parser(Planner.GetDefaultParserOptions(), ErrorManager);
    }

    #endregion

    #region Event Handlers

    private void btnDomain_Click(object sender, EventArgs e)
    {
      //dlgParse.Title = "Domain file";
      //dlgParse.FileName = txtDomain.Text;

      //if (dlgParse.ShowDialog() != DialogResult.Cancel)
      //{
      //  ParseDomain(dlgParse.FileName);
      //}
    }

    private void btnProblem_Click(object sender, EventArgs e)
    {
      //dlgParse.Title = "Problem file";
      //dlgParse.FileName = txtProblem.Text;

      //if (dlgParse.ShowDialog() != DialogResult.Cancel)
      //{
      //  ParseProblem(dlgParse.FileName);
      //}
    }

    #endregion

    #region Private Methods

    private bool VerifyErrors()
    {
      if (ErrorManager.Contains(ErrorManager.Message.ERROR))
      {
        PrintParseMessages(ErrorManager.Message.ALL);
        return false;
      }

      PrintParseMessages(ErrorManager.Message.WARNING);
      return true;
    }

    private PDDLObject Parse(string fileName)
    {
      PDDLObject obj = Parser.parse(fileName);

      return (VerifyErrors() ? obj : null);
    }

    private PDDLObject Link()
    {
      PDDLObject obj = Parser.link(Domain, CurrentProblem);

      return (VerifyErrors() ? obj : null);
    }

    private void ParseDomain(string fileName)
    {
      bool enableControls = false;

      Domain = Parse(fileName);

      if (Domain != null)
      {
        if (Domain.Content != PDDLObject.PDDLContent.DOMAIN)
        {
          PrintParseError(string.Format("The file \"{0}\" is not a domain file.\n", fileName));
          Domain = null;
        }
        else
          enableControls = true;
      }

      txtProblem.Enabled = btnProblem.Enabled = enableControls;
      btnSolve.Enabled = false;
    }

    private void ParseProblem(string fileName)
    {
      bool enableControls = false;

      CurrentProblem = Parse(fileName);

      if (CurrentProblem != null)
      {
        if (CurrentProblem.Content != PDDLObject.PDDLContent.PARTIAL_PROBLEM)
        {
          PrintParseError(string.Format("The file \"{0}\" is not a problem file.\n", fileName));
          CurrentProblem = null;
        }
        else if ((CurrentProblem = Link()) != null)
          enableControls = true;
      }

      btnSolve.Enabled = enableControls;
    }

    private void PrintParseMessages(ErrorManager.Message type)
    {
      ErrorManager.print(type);

      foreach (string str in ErrorStream.ToString().Split('\n').Select(s => s + "\n"))
      {

        if (str.IndexOf("warning", StringComparison.CurrentCultureIgnoreCase) != -1)
          PrintParseError(str);
        else if (str.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) != -1)
          PrintParseWarning(str);
        else
          PrintParseInfo(str);
      }
    }

    private void PrintParseError(string error)
    {
      rtxtParsing.ForeColor = Color.OrangeRed;
      rtxtParsing.AppendText(error);
    }

    private void PrintParseWarning(string warning)
    {
      rtxtParsing.ForeColor = Color.DarkRed;
      rtxtParsing.AppendText(warning);
    }

    private void PrintParseInfo(string info)
    {
      rtxtParsing.ForeColor = Color.Black;
      rtxtParsing.AppendText(info);
    }

    #endregion

    private void fcDomain_FileChosenOk(object sender, EventArgs e)
    {
      //MessageBox.Show("File ok: " + fcDomain.FileName);
    }

    private void fcDomain_FileChosenNotOk(object sender, EventArgs e)
    {
      //MessageBox.Show("File not ok: " + fcDomain.FileName);

    }

    private void fcDomain_FileChanged(object sender, FileSystemEventArgs e)
    {
      //MessageBox.Show("File changed");
    }

    private void fcDomain_FileCreated(object sender, FileSystemEventArgs e)
    {
      //MessageBox.Show("File created");
    }

    private void fcDomain_FileDeleted(object sender, FileSystemEventArgs e)
    {
      //MessageBox.Show("File deleted");

    }

    private void fcDomain_FileRenamed(object sender, RenamedEventArgs e)
    {
      //MessageBox.Show("File renamed");
    }

    private void button1_Click(object sender, EventArgs e)
    {
      contextMenuStrip1.Show(button1, new Point(button1.Width, button1.Height), ToolStripDropDownDirection.Left);
    }
  }
}
