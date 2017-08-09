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
using PDDLParser.Exp.Formula;

namespace PDDLParser.Exception
{
  /// <summary>
  /// A CycleException is thrown to avoid infinite recursion, in the case when an attempt to 
  /// evaluate a defined function results in a cycle.
  /// </summary>
  public class CycleException : System.Exception, ISerializable
  {
    /// <summary>
    /// The defined formula application that caused the exception.
    /// </summary>
    private DefinedFormulaApplication m_formula;

    /// <summary>
    /// Creates a new CycleException with a specified defined formula application.
    /// </summary>
    /// <param name="formula">The formula whose evaluation caused the cycle.</param>
    public CycleException(DefinedFormulaApplication formula)
    {
      this.m_formula = formula;
    }

    /// <summary>
    /// The message of this exception.
    /// </summary>
    public override string Message
    {
      get
      {
        return "A cycle was detected: evaluating the defined formula " + this.m_formula.ToString() + 
               " caused a cycle";
      }
    }
  }
}
