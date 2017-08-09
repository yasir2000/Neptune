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
using System.IO;
using System.Linq;
using System.Text;
using PDDLParser.Parser;
using TLPlan.World;

namespace TLPlan.Utils
{
  /// <summary>
  /// Represents a class which can write traces to a given <see cref="System.IO.TextWriter"/> and print worlds to it.
  /// </summary>
  public class TraceWriter
  {
    #region Private Fields

    /// <summary>
    /// The current PDDL problem being solved.
    /// </summary>
    PDDLObject m_fullProblem;
    /// <summary>
    /// The stream to which traces are to be written.
    /// </summary>
    TextWriter m_textWriter;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the current PDDL problem being solved.
    /// </summary>
    public PDDLObject FullProblem
    {
      get { return m_fullProblem; }
      set { m_fullProblem = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new trace writer with the given trace stream.
    /// </summary>
    /// <param name="writer">The stream to which traces are to be written.</param>
    public TraceWriter(TextWriter writer)
    {
      System.Diagnostics.Debug.Assert(writer != null);

      this.m_fullProblem = null;
      this.m_textWriter = writer;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Logs a string to the trace stream.
    /// </summary>
    /// <param name="str">The string to be logged.</param>
    public void WriteLine(string str)
    {
      m_textWriter.WriteLine(str);
    }

    /// <summary>
    /// Logs a formatted string to the trace stream.
    /// </summary>
    /// <param name="format">The formatted string.</param>
    /// <param name="args">The parameters of the formatted string.</param>
    public void WriteLine(string format, params object[] args)
    {
      m_textWriter.WriteLine(format, args);
    }

    /// <summary>
    /// Prints a world to the trace stream.
    /// </summary>
    /// <param name="node">The node containing the world to print.</param>
    /// <param name="worldParts">The parts of the world to print.</param>
    public void PrintWorld(Node node, WorldUtils.PrintWorldPart worldParts)
    {
      WorldUtils.PrintWorld(m_fullProblem, node, worldParts, false, false, m_textWriter, m_textWriter);
    }

    #endregion
  }
}
