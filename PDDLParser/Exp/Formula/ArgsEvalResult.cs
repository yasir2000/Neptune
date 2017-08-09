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
using PDDLParser.Exception;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// An ArgsEvalResult represents a set of evaluated arguments, or an undefined value if 
  /// the evaluation was not successful.
  /// </summary>
  public struct ArgsEvalResult
  {
    /// <summary>
    /// ArgsEvalResult Undefined value.
    /// </summary>
    public static readonly ArgsEvalResult Undefined = new ArgsEvalResult(State.Undefined);

    /// <summary>
    /// Internal status of this ArgsEvalResult which indicates whether it is defined.
    /// </summary>
    public enum State
    {
      /// <summary>
      /// Indicates that all the evaluated arguments are defined.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that their is at least one argument which evaluates to undefined.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The inner formula application (with evaluated arguments) stored in this ArgsEvalResult.
    /// </summary>
    internal FormulaApplication m_value;
    /// <summary>
    /// The status of this ArgsEvalResult.
    /// </summary>
    internal State m_status;
    
    /// <summary>
    /// Creates a new ArgsEvalResult with the given formula application with evaluated arguments.
    /// </summary>
    /// <param name="value">A formula application with evaluated arguments.</param>
    public ArgsEvalResult(FormulaApplication value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new ArgsEvalResult with the given status.
    /// </summary>
    /// <param name="status">The status of the new ArgsEvalResult.</param>
    private ArgsEvalResult(State status)
    {
      this.m_value = null;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the status of this ArgsEvalResult.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the inner formula application stored inside this ArgsEvalResult.
    /// If this ArgsEvalResult is undefined, this call throws an UndefinedExpException.
    /// </summary>
    public FormulaApplication Value
    {
      get
      {
        if (this.m_status != State.Defined)
          throw new UndefinedExpException("Value is undefined!");
        return this.m_value;
      }
    }

    /// <summary>
    /// Returns true if this ArgsEvalResult is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this ArgsEvalResult is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      ArgsEvalResult other = (ArgsEvalResult)obj;
      if (this.m_status == State.Defined)
      {
        return this.m_status == other.m_status &&
               this.m_value.Equals(other.m_value);
      }
      else
      {
        return this.m_status == other.m_status;
      }
    }

    /// <summary>
    /// Returns the hash code of this ArgsEvalResult.
    /// </summary>
    /// <returns>The hash code of this ArgsEvalResult.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ?
              this.m_value.GetHashCode() : this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this ArgsEvalResult.
    /// </summary>
    /// <returns>A string representation of this ArgsEvalResult.</returns>
    public override string ToString()
    {
      if (this.m_status == State.Defined)
      {
        return this.m_value.ToString();
      }
      else
      {
        return "Undefined";
      }
    }
  }
}
