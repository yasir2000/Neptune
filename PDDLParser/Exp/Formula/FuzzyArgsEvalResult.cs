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
  /// A FuzzyArgsEvalResult represents a set of evaluated arguments, or an undefined or
  /// unknown value if the evaluation was not successful.
  /// </summary>
  public struct FuzzyArgsEvalResult
  {
    /// <summary>
    /// FuzzyArgsEvalResult Unknown value.
    /// </summary>
    public static readonly FuzzyArgsEvalResult Unknown = new FuzzyArgsEvalResult(State.Unknown);

    /// <summary>
    /// FuzzyArgsEvalResult Undefined value.
    /// </summary>
    public static readonly FuzzyArgsEvalResult Undefined = new FuzzyArgsEvalResult(State.Undefined);

    /// <summary>
    /// Internal status of this FuzzyArgsEvalResult which indicates whether it is defined.
    /// </summary>
    public enum State
    {
      /// <summary>
      /// Indicates that all the evaluated arguments are defined.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that at least one evaluated argument is unknown.
      /// </summary>
      Unknown = 1,
      /// <summary>
      /// Indicates that at least one evaluated argument is undefined.
      /// Note that undefined has precedence over unknown.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The inner formula application (with evaluated arguments) stored in this FuzzyArgsEvalResult.
    /// </summary>
    internal FormulaApplication m_value;
    /// <summary>
    /// The status of this FuzzyArgsEvalResult.
    /// </summary>
    internal State m_status;

    /// <summary>
    /// Creates a new FuzzyArgsEvalResult with the given formula application with evaluated arguments.
    /// </summary>
    /// <param name="value">A formula application with evaluated arguments.</param>
    public FuzzyArgsEvalResult(FormulaApplication value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new FuzzyArgsEvalResult with the given status.
    /// </summary>
    /// <param name="status">The status of the new FuzzyArgsEvalResult.</param>
    private FuzzyArgsEvalResult(State status)
    {
      this.m_value = null;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the status of this FuzzyArgsEvalResult.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the inner formula application stored inside this FuzzyArgsEvalResult.
    /// If this FuzzyArgsEvalResult is undefined, this call throws an UndefinedExpException.
    /// If this FuzzyArgsEvalResult is unknown, this call throws an UnknownEvaluationResultException.
    /// </summary>
    public FormulaApplication Value
    {
      get
      {
        switch (this.m_status)
        {
          case State.Unknown:
            throw new UnknownExpException("Value is unknown!");
          case State.Undefined:
            throw new UndefinedExpException("Value is undefined!");
          default:
            return this.m_value;
        }
      }
    }

    /// <summary>
    /// Returns true if this FuzzyArgsEvalResult is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this FuzzyArgsEvalResult is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      FuzzyArgsEvalResult other = (FuzzyArgsEvalResult)obj;
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
    /// Returns the hash code of this FuzzyArgsEvalResult.
    /// </summary>
    /// <returns>The hash code of this FuzzyArgsEvalResult.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ?
              this.m_value.GetHashCode() : this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns the string representation of this FuzzyArgsEvalResult.
    /// </summary>
    /// <returns>The string representation of this FuzzyArgsEvalResult.</returns>
    public override string ToString()
    {
      switch (this.m_status)
      {
        case State.Defined:
          return this.m_value.ToString();
        case State.Unknown:
          return "Unknown";
        case State.Undefined:
          return "Undefined";
        default:
          throw new System.Exception("Invalid EvalStatus: " + this.m_status);
      }
    }
  }
}
