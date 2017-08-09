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
using PDDLParser.Exp.Numeric;
using Number = PDDLParser.Exp.Numeric.Number;

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// A NumericValue represents a numeric expression, a double value, or an undefined value.
  /// </summary>
  public struct NumericValue
  {
    /// <summary>
    /// NumericValue undefined value.
    /// </summary>
    public static readonly NumericValue Undefined = new NumericValue(Double.Undefined);

    /// <summary>
    /// The Double value associated with this NumericValue.
    /// Note that this value is not defined if a corresponding expression is defined.
    /// </summary>
    internal Double m_value;
    /// <summary>
    /// The numeric expression associated with this ExpressionValue.
    /// </summary>
    internal INumericExp m_exp;

    /// <summary>
    /// Creates a new NumericValue from a specified Double value.
    /// </summary>
    /// <param name="value">A Double value.</param>
    public NumericValue(Double value)
    {
      this.m_value = value;
      this.m_exp = null;
    }

    /// <summary>
    /// Creates a new NumericValue from a specified double value.
    /// </summary>
    /// <param name="value">A double value.</param>
    public NumericValue(double value)
      : this(new Double(value))
    {
    }

    /// <summary>
    /// Creates a new NumericValue from a specified numeric expression.
    /// </summary>
    /// <param name="exp">A numeric expression.</param>
    public NumericValue(INumericExp exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_value = Double.Undefined;
      this.m_exp = exp;
    }

    /// <summary>
    /// Returns the numeric expression stored in this NumericValue.
    /// Note that this numeric expression may be null.
    /// </summary>
    public INumericExp Exp
    {
      get { return m_exp; }
    }

    /// <summary>
    /// Returns the Double value stored in this ExpressionValue.
    /// This value should only be accessed if the expression is null.
    /// </summary>
    public Double Value
    {
      get
      {
        //if (m_exp != null) throw new System.Exception("Value is not accessible.");
        //else return m_value;

        System.Diagnostics.Debug.Assert(m_exp == null);

        return m_value;
      }
    }

    /// <summary>
    /// Returns a numeric expression equivalent to this NumericValue.
    /// If this NumericValue corresponds to a Double value, an equivalent constant expression
    /// is returned (double -> Number, Undefined -> UndefinedExp).
    /// </summary>
    /// <returns>A numeric expression equivalent to this NumericValue.</returns>
    public INumericExp GetEquivalentExp()
    {
      if (this.m_exp != null)
        return this.m_exp;
      else
      {
        switch (m_value.Status)
        {
          case Double.State.Defined:
            return new Number(m_value.Value);
          case Double.State.Undefined:
            return UndefinedNumericExp.Undefined;
          default:
            throw new System.Exception("Invalid DoubleStatus value: " + m_value.Status);
        }
      }
    }

    /// <summary>
    /// Returns true if this NumericValue is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this NumericValue is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      NumericValue other = (NumericValue)obj;
      if (this.m_exp != null)
        return other.m_exp != null && this.m_exp.Equals(other.m_exp);
      else
        return other.m_exp == null && this.m_value.Equals(other.m_value);
    }

    /// <summary>
    /// Returns the hashcode of this NumericValue.
    /// </summary>
    /// <returns>The hashcode of this NumericValue.</returns>
    public override int GetHashCode()
    {
      return (this.m_exp != null) ? this.m_exp.GetHashCode() :
                                  this.m_value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this NumericValue.
    /// </summary>
    /// <returns>A string representation of this NumericValue.</returns>
    public override string ToString()
    {
      if (m_exp != null)
      {
        return m_exp.ToString();
      }
      else
      {
        return m_value.ToString();
      }
    }
  }
}
