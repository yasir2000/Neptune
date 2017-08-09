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
// Please note that this file was inspired in part by the PDDL4J library:
// http://www.math-info.univ-paris5.fr/~pellier/software/software.php 
//
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.IO;
using System.Runtime.Serialization;

namespace PDDLParser.Exception
{
  /// <summary>
  /// A ParserException is thrown if an internal error occurs in parser.
  /// </summary>
  public class ParserException : System.Exception, ISerializable
  {
    /// <summary>
    /// The line of the error. 
    /// </summary>
    private int m_line;

    /// <summary>
    /// The column of the error.
    /// </summary>
    private int m_column;

    /// <summary>
    /// The file where the error was detected.
    /// </summary>
    private string m_file;

    /// <summary>
    /// Creates a new ParserException with a specific message and cause.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="cause">The exception cause.</param>
    public ParserException(string message, System.Exception cause)
        : base(message, cause)
    {
      this.m_line = -1;
      this.m_column = -1;
      this.m_file = null;
    }

    /// <summary>
    /// Creates a new ParserException with a specific message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ParserException(string message)
        : base(message)
    {
      this.m_line = -1;
      this.m_column = -1;
      this.m_file = null;
    }

    /// <summary>
    /// Creates a new ParserException with a specific message and location.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="file">The file where the error was detected.</param>
    /// <param name="line">The line where the error was detected.</param>
    /// <param name="column">The column where the error was detected.</param>
    public ParserException(string message, string file, int line, int column)
        : base(message)
    {
      this.m_file = file;
      this.m_line = line;
      this.m_column = column;
    }

    /// <summary>
    /// Returns the file where the error was detected.
    /// </summary>
    /// <returns>The file where the error was detected.</returns>
    public string GetFile() 
    {
      return this.m_file;
    }

    /// <summary>
    /// Returns the column where the error was detected.
    /// </summary>
    /// <returns>The column where the error was detected.</returns>
    public int GetColumn()
    {
      return this.m_column;
    }

    /// <summary>
    /// Returns the line where the error was detected.
    /// </summary>
    /// <returns>The line where the error was detected.</returns>
    public int GetLine()
    {
      return this.m_line;
    }

  }
}