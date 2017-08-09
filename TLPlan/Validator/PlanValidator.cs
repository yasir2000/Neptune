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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TLPlan.Validator
{
  /// <summary>
  /// Represents a wrapper for the PDDL3.0 plan validator. The validator is an external executable.
  /// </summary>
  public class PlanValidator
  {
    #region Private Fields

    /// <summary>
    /// The validator's options.
    /// </summary>
    private ValidatorOptions m_options;

    /// <summary>
    /// The output stream to which informative validation messages will be written. Defaults to Console.Out.
    /// </summary>
    private TextWriter m_stdOut;

    /// <summary>
    /// The output stream to which validation error messages will be written. Defaults to Console.Error.
    /// </summary>
    private TextWriter m_errorOut;

    #endregion

    #region Properties

    /// <summary>
    /// Returns the validator's options.
    /// </summary>
    public ValidatorOptions Options { get { return m_options; } }

    /// <summary>
    /// Returns the stream to which informative validation messages will be written.
    /// </summary>
    public TextWriter Out { get { return m_stdOut; } }

    /// <summary>
    /// Returns the stream to which validation error messages will be written.
    /// </summary>
    public TextWriter Error { get { return m_errorOut; } }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an instance of the validator with default values.
    /// </summary>
    public PlanValidator()
      : this(new ValidatorOptions(), Console.Out, Console.Error)
    { }

    /// <summary>
    /// Creates an instance of the validator.
    /// </summary>
    /// <param name="options">The validator's options.</param>
    /// <param name="outStream">The stream to which informative validation messages will be written.</param>
    /// <param name="errorStream">The stream to which validation error messages will be written.</param>
    public PlanValidator(ValidatorOptions options, TextWriter outStream, TextWriter errorStream)
    {
      this.m_options = options;
      this.m_stdOut = outStream;
      this.m_errorOut = errorStream;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Validates the plan found to a problem and a domain.
    /// Note: The plan has to be saved to a temporary file which can the be input the the validator.
    /// </summary>
    /// <param name="domain">The PDDL3.0-compliant domain used for validation.</param>
    /// <param name="problem">The PDDL3.0-compliant problem used for validation.</param>
    /// <param name="plan">The plan found for the given domain and problem.</param>
    /// <returns>Whether the plan is valid.</returns>
    public bool Validate(string domain, string problem, Plan plan)
    {
      string planFile = Path.GetTempFileName();
      
      // Write the plan to a temporary file
      using (StreamWriter fileWriter = new StreamWriter(new FileStream(planFile, FileMode.Create, FileAccess.Write), Encoding.UTF8))
      {
        plan.PrintPlan(fileWriter);
        fileWriter.Close();
      }

      // Make sure the file is kept open, and shared for reading, so that other validators cannot overwrite it.

      ProcessStartInfo info = new ProcessStartInfo(m_options.ValidatorPath, string.Format("{3} {0} {1} {2}", domain, problem, planFile, m_options.ToString()));
      info.WorkingDirectory = System.Environment.CurrentDirectory;
      info.CreateNoWindow = true;
      info.UseShellExecute = false;
      info.RedirectStandardError = true;
      info.RedirectStandardOutput = true;

      Process validator = new Process();
      StringBuilder sb = new StringBuilder();
      validator.OutputDataReceived += (sender, e) => { m_stdOut.WriteLine(e.Data); sb.AppendLine(e.Data); };
      validator.StartInfo = info;

      validator.Start();
      validator.BeginOutputReadLine();
      validator.WaitForExit();

      // Unfortunately, the validator's exit code merely represents the number of errors that occurred (e.g. unreadable domain),
      // instead of indicating if some plan being validated failed. Therefore, we have to search through the output to find out
      // if a plan was indeed valid.
      bool returnValue = validator.ExitCode == 0;

      if (!returnValue)
        m_errorOut.Write(validator.StandardError.ReadToEnd());

      validator.Close();

      if (returnValue)
      {
        // HACK: This has to rely on output since the ExitCode is not worthwhile... therefore, 
        // there may be failure cases that are missed, and if the validator is upgraded, this may
        // not work properly anymore.

        // Match a line starting with "Plan valid"
        MatchCollection matches = Regex.Matches(sb.ToString(), "^Plan valid.*$", RegexOptions.Multiline);

        // In some cases, more text may be appended after "Plan valid"; these will be considered as errors for now.
        if (matches.Count != 1 || matches[0].Value.Trim() != "Plan valid")
          returnValue = false;
      }

      try
      {
        File.Delete(planFile);
      }
      catch (System.Exception e)
      {
        m_errorOut.WriteLine("Could not delete temporary plan file: " + e.Message);
      }

      return returnValue;
    }

    #endregion
  }
}
