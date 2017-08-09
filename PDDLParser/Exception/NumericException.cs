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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PDDLParser.Exp;

namespace PDDLParser.Exception
{
  /// <summary>
  /// A NumericException is thrown if an attempt is made to perform an illegal
  /// arithmetic operation (like a division by zero).
  /// </summary>
  public class NumericException : System.Exception, ISerializable
  {
    /// <summary>
    /// The expression that caused the exception.
    /// </summary>
    private IExp m_cause;

    /// <summary>
    /// The arguments the function was evaluated with.
    /// </summary>
    private IEnumerable<double> m_arguments;

    /// <summary>
    /// Creates a new NumericException with a specific message and cause.
    /// </summary>
    /// <param name="cause">The numeric expression that caused the exception.</param>
    /// <param name="arguments">The arguments the function was evaluated with.</param>
    public NumericException(IExp cause, IEnumerable<double> arguments)
      : base()
    {
      this.m_cause = cause;
      this.m_arguments = arguments;
    }

    /// <summary>
    /// The exception's message.
    /// </summary>
    public override string Message
    {
      get
      {
        return "Numeric error while evaluating " + this.m_cause.ToString()
             + " with arguments " + m_arguments.Aggregate("", (string i, double j) => i + " " + j);
      }
    }
  }
}
