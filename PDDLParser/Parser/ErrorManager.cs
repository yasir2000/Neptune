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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using PDDLParser.Extensions;

namespace PDDLParser.Parser
{
  /// <summary>
  /// The error manager is used to log lexing/parsing errors and warnings.
  /// </summary>
  public class ErrorManager
  {
    /// <summary>
    /// This enumeration defines the type of message logged in the error manager.
    /// </summary>
    [Flags]
    public enum Message
    {
      /// <summary>
      /// No type.
      /// </summary>
      NONE = 0,
      /// <summary>
      /// A parser error.
      /// </summary>
      PARSER_ERROR = 1,
      /// <summary>
      /// A parser warning.
      /// </summary>
      PARSER_WARNING = 2,
      /// <summary>
      /// A lexical error.
      /// </summary>
      LEXICAL_ERROR = 4,
      /// <summary>
      /// A linker error.
      /// </summary>
      LINKER_ERROR = 8,
      /// <summary>
      /// A linker warning.
      /// </summary>
      LINKER_WARNING = 16,
      /// <summary>
      /// All errors and warnings.
      /// </summary>
      ALL = ~NONE,
      /// <summary>
      /// A parser error or a parser warning.
      /// </summary>
      PARSER = PARSER_ERROR | PARSER_WARNING,
      /// <summary>
      /// A linker error or a linker warning.
      /// </summary>
      LINKER = LINKER_ERROR | LINKER_WARNING,
      /// <summary>
      /// All errors.
      /// </summary>
      ERROR = PARSER_ERROR | LINKER_ERROR | LEXICAL_ERROR,
      /// <summary>
      /// All warnings.
      /// </summary>
      WARNING = PARSER_WARNING | LINKER_WARNING,
    }

    /// <summary>
    /// A triplet stores three different values.
    /// </summary>
    /// <typeparam name="TFirst">The type of the first value.</typeparam>
    /// <typeparam name="TSecond">The type of the second value.</typeparam>
    /// <typeparam name="TThird">The type of the third value.</typeparam>
    private struct Triplet<TFirst, TSecond, TThird>
    {
      /// <summary>
      /// The first value.
      /// </summary>
      public TFirst First { get; set; }
      /// <summary>
      /// The second value.
      /// </summary>
      public TSecond Second { get; set; }
      /// <summary>
      /// The third value.
      /// </summary>
      public TThird Third { get; set; }

      /// <summary>
      /// Creates a new triplet with the specified values.
      /// </summary>
      /// <param name="first">The first value.</param>
      /// <param name="second">The second value.</param>
      /// <param name="third">The third value.</param>
      public Triplet(TFirst first, TSecond second, TThird third)
        : this()
      {
        First = first;
        Second = second;
        Third = third;
      }
      
      /// <summary>
      /// Returns a string representation of this triplet.
      /// </summary>
      /// <returns>A string representation of this triplet.</returns>
      public override string ToString()
      {
        return string.Format("({0},{1},{2})", First.ToString(), Second.ToString(), Third.ToString());
      }
    }

    /// <summary>
    /// All messages, kept as triplets (file name, message type, actual message).
    /// </summary>
    private List<Triplet<string, Message, string>> m_msgs;

    /// <summary>
    /// The warning output stream.
    /// </summary>
    private TextWriter m_warningStream;
    /// <summary>
    /// The error output stream.
    /// </summary>
    private TextWriter m_errorStream;

    /// <summary>
    /// Creates a new error manager with the default output streams.
    /// </summary>
    public ErrorManager()
      : this(Console.Out, Console.Out)
    { 
    }

    /// <summary>
    /// Creates a new error manager with the specified output streams.
    /// </summary>
    /// <param name="warningStream">The warning output stream.</param>
    /// <param name="errorStream">The error output stream.</param>
    public ErrorManager(TextWriter warningStream, TextWriter errorStream)
    {
      this.m_msgs = new List<Triplet<string, Message, string>>();
      this.m_errorStream = errorStream;
      this.m_warningStream = warningStream;
    }

    /// <summary>
    /// Prints all messages of the specified type.
    /// </summary>
    /// <param name="type">The type of messages to print.</param>
    public void print(Message type)
    {
      this.print(this.getMessages(type));
    }

    /// <summary>
    /// Prints all the messages concerning a specific file.
    /// </summary>
    /// <param name="file">A file.</param>
    public void print(string file)
    {
      this.print(this.getMessages(Message.ALL, file));
    }

    /// <summary>
    /// Prints all messages of the specified type which concern a specific file.
    /// </summary>
    /// <param name="type">The type of messages to print.</param>
    /// <param name="file">A file.</param>
    public void print(Message type, string file)
    {
      this.print(this.getMessages(type, file));
    }

    /// <summary>
    /// Print the messages to the appropriate output stream.
    /// </summary>
    /// <param name="msgs">The messages to output.</param>
    private void print(IEnumerable<KeyValuePair<Message, string>> msgs)
    {
      if (msgs != null)
      {
        foreach (KeyValuePair<Message, string> pair in msgs)
        {
          if ((pair.Key | Message.ERROR) != 0)
            this.m_errorStream.WriteLine(pair.Value);
          else
            this.m_warningStream.WriteLine(pair.Value);
        }
      }
    }

    /// <summary>
    /// Returns all messages of the specified type.
    /// </summary>
    /// <param name="type">The type of the messages to return.</param>
    /// <returns>All the messages of the specified type.</returns>
    public IEnumerable<KeyValuePair<Message, string>> getMessages(Message type)
    {
      return m_msgs.Where(t => (t.Second & type) != 0)
                   .Select(t => new KeyValuePair<Message, string>(t.Second, t.Third));
    }

    /// <summary>
    /// Returns all messages of the specified type. Note that only the actual messages
    /// are returned (and not their types).
    /// </summary>
    /// <param name="type">The type of the messages to return.</param>
    /// <returns>All messages of the specified type.</returns>
    public IEnumerable<string> getUntypedMessages(Message type)
    {
      return getMessages(type).Select(p => p.Value);
    }

    /// <summary>
    /// Returns all messages concerning a specific file.
    /// </summary>
    /// <param name="file">A file.</param>
    /// <returns>All messages concerning a specific file.</returns>
    public IEnumerable<KeyValuePair<Message, string>> getMessages(string file)
    {
      return m_msgs.Where(t => t.First == file)
                   .Select(t => new KeyValuePair<Message, string>(t.Second, t.Third));
    }

    /// <summary>
    /// Returns all messages of the specified type which concern a specific file.
    /// </summary>
    /// <param name="type">The type of the messages to return.</param>
    /// <param name="file">A file.</param>
    /// <returns>All messages of the specified type which concern a specific file.</returns>
    public IEnumerable<KeyValuePair<Message, string>> getMessages(Message type, string file)
    {
      return m_msgs.Where(t => t.First == file && (t.Second & type) != 0)
                   .Select(t => new KeyValuePair<Message, string>(t.Second, t.Third));
    }

    /// <summary>
    /// Reinitializes the error manager.
    /// </summary>
    public void clear()
    {
      this.m_msgs.Clear();
    }

    /// <summary>
    /// Returns true if the error manager does not contain any message, false otherwise.
    /// </summary>
    /// <returns>True if the error manager does not contain any message, false otherwise.</returns>
    public bool isEmpty()
    {
      return m_msgs.Count == 0;
    }

    /// <summary>
    /// Returns true if the error manager contains at least one message of the specified type.
    /// </summary>
    /// <param name="type">The type of the messages to look for.</param>
    /// <returns>True if at least one corresponding message is found.</returns>
    public bool Contains(Message type)
    {
      return !this.getMessages(type).IsEmpty();
    }

    /// <summary>
    /// Logs a parser error message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    public void logParserError(string msg, string file)
    {
      string error = "Compiler error (" + file + "): " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.PARSER_ERROR, error));
    }

    /// <summary>
    /// Logs a parser error message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    /// <param name="line">The line where the error was encountered.</param>
    /// <param name="column">The column where the error was encountered.</param>
    public void logParserError(string msg, string file, int line, int column)
    {
      string error = "Parser error at line " + line + ", column " + column
      + " file (" + file + ") : " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.PARSER_ERROR, error));
    }

    /// <summary>
    /// Logs a parser warning message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    public void logParserWarning(string msg, string file)
    {
      string warning = "Parser warning (" + file + "): " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.PARSER_WARNING, warning));
    }

    /// <summary>
    /// Logs a parser warning message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    /// <param name="line">The line where the error was encountered.</param>
    /// <param name="column">The column where the error was encountered.</param>
    public void logParserWarning(string msg, string file, int line, int column)
    {
      string warning = "Parser warning at line " + line + ", column " + column
          + " file (" + file + ") : " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.PARSER_WARNING, warning));
    }

    /// <summary>
    /// Logs a lexical error message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    public void logLexicalError(string msg, string file)
    {
      string error = msg + " in file " + file;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.LEXICAL_ERROR, error));
    }

    /// <summary>
    /// Logs a lexical error message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    /// <param name="line">The line where the error was encountered.</param>
    /// <param name="column">The column where the error was encountered.</param>
    public void logLexicalError(string msg, string file, int line, int column)
    {
      string error = "Lexical error at line " + line + ", column " + column + ", file (" + file + ") : " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.LEXICAL_ERROR, error));
    }

    /// <summary>
    /// Logs a linker error message.
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    public void logLinkerError(string msg, string file)
    {
      string error = "Linker error (" + file + "): " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.LINKER_ERROR, error));
    }

    /// <summary>
    /// Logs a linker warning message
    /// </summary>
    /// <param name="msg">The error message.</param>
    /// <param name="file">The file where the error was encountered.</param>
    public void logLinkerWarning(string msg, string file)
    {
      string warning = "Linker warning (" + file + "): " + msg;

      this.m_msgs.Add(new Triplet<string, Message, string>(file, Message.LINKER_WARNING, warning));
    }
  }
}