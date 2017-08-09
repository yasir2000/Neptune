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

namespace PDDLParser
{
  /// <summary>
  /// An EvaluableValue represents an evaluable expression or an evaluation result.
  /// </summary>
	public struct EvaluableValue
	{
    /// <summary>
    /// The evaluation result associated with this EvaluableValue.
    /// Note that this value is not defined if a corresponding expression is defined.
    /// </summary>
    private object value;
    /// <summary>
    /// The evaluable expression associated with this EvaluableValue.
    /// </summary>
    private IEvaluableExp exp;

    /// <summary>
    /// Creates a new EvaluableValue from a specified value.
    /// </summary>
    /// <param name="value">A value.</param>
    public EvaluableValue(object value)
    {
      this.value = value;
      this.exp = null;
    }

    /// <summary>
    /// Creates a new EvaluableValue from a specified evaluable expression.
    /// </summary>
    /// <param name="exp">A evaluable expression.</param>
    public EvaluableValue(IEvaluableExp exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.value = null;
      this.exp = exp;
    }

    /// <summary>
    /// Creates a new EvaluableValue from a specified evaluable expression and 
    /// evaluation result.
    /// </summary>
    /// <param name="exp">A evaluable expression.</param>
    /// <param name="value">The value of this EvaluableValue.</param>
    internal EvaluableValue(IEvaluableExp exp, object value)
    {
      this.value = value;
      this.exp = exp;
    }

    /// <summary>
    /// Returns the evaluable expression stored in this EvaluableValue.
    /// Note that this evaluable expression may be null.
    /// </summary>
    public IEvaluableExp Exp
    {
      get { return exp; }
    }

    /// <summary>
    /// Returns the value stored in this EvaluableValue.
    /// If an evaluable expression is stored in this EvaluableValue, this call throws
    /// an exception.
    /// </summary>
    public object Value
    {
      get
      {
        if (exp != null) throw new Exception("Value is not accessible.");
        else return value;
      }
    }

    /// <summary>
    /// Returns true if this EvaluableValue is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this EvaluableValue is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      EvaluableValue other = (EvaluableValue)obj;
      if (this.exp != null)
        return other.exp != null && this.exp.Equals(other.exp);
      else
        return other.exp == null && this.value.Equals(other.value);
    }

    /// <summary>
    /// Returns the hashcode of this EvaluableValue.
    /// </summary>
    /// <returns>The hashcode of this EvaluableValue.</returns>
    public override int GetHashCode()
    {
      return (this.exp != null) ? this.exp.GetHashCode() : 
                                  this.value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this EvaluableValue.
    /// </summary>
    /// <returns>A string representation of this EvaluableValue.</returns>
    public override string ToString()
    {
      if (exp != null)
      {
        return exp.ToString();
      }
      else
      {
        return value.ToString();
      }
    }
	}
}
