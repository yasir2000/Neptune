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
using PDDLParser.Exp.Logical;

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// A LogicalValue represents a logical expression, a boolean value, or an undefined value.
  /// </summary>
  public struct LogicalValue
  {
    /// <summary>
    /// LogicalValue true value.
    /// </summary>
    public static readonly LogicalValue True = new LogicalValue(Bool.True);

    /// <summary>
    /// LogicalValue false value.
    /// </summary>
    public static readonly LogicalValue False = new LogicalValue(Bool.False);

    /// <summary>
    /// LogicalValue undefined value.
    /// </summary>
    public static readonly LogicalValue Undefined = new LogicalValue(Bool.Undefined);

    /// <summary>
    /// The Bool value associated with this LogicalValue.
    /// Note that this value is not defined if a corresponding logical expression is defined.
    /// </summary>
    internal Bool m_value;
    /// <summary>
    /// The logical expression associated with this LogicalValue.
    /// </summary>
    internal ILogicalExp m_exp;

    /// <summary>
    /// Creates a new LogicalValue from a specified bool value.
    /// </summary>
    /// <param name="value">A bool value.</param>
    public LogicalValue(bool value)
      : this(new Bool(value))
    {
    }

    /// <summary>
    /// Creates a new LogicalValue from a specified Bool value.
    /// </summary>
    /// <param name="value">A Bool value.</param>
    public LogicalValue(Bool value)
    {
      this.m_value = value;
      this.m_exp = null;
    }

    /// <summary>
    /// Creates a new LogicalValue from a specified logical expression.
    /// </summary>
    /// <param name="exp">A logical expression.</param>
    public LogicalValue(ILogicalExp exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_value = Bool.Undefined;
      this.m_exp = exp;
    }

    /// <summary>
    /// Creates a new LogicalValue from a specified result and logical expression.
    /// Note that this constructor should stay private.
    /// </summary>
    /// <param name="value">A Bool value.</param>
    /// <param name="exp">A logical expression.</param>
    private LogicalValue(Bool value, ILogicalExp exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_value = value;
      this.m_exp = exp;
    }

    /// <summary>
    /// Returns the logical expression stored in this LogicalValue.
    /// Note that this logical expression may be null.
    /// </summary>
    public ILogicalExp Exp
    {
      get { return m_exp; }
    }

    /// <summary>
    /// Returns the Bool value stored in this LogicalValue.
    /// This value should only be accessed if the expression is null.
    /// </summary>
    public Bool Value
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
    /// Returns a logical expression equivalent to this LogicalValue.
    /// If this LogicalValue corresponds to a Bool value, an equivalent constant expression
    /// is returned (false -> FalseExp, Undefined -> UndefinedExp, True -> TrueExp).
    /// </summary>
    /// <returns>A logical expression equivalent to this LogicalValue.</returns>
    public ILogicalExp GetEquivalentExp()
    {
      if (this.m_exp != null)
        return this.m_exp;
      else
      {
        switch ((BoolValue)m_value)
        {
          case BoolValue.False:
            return FalseExp.False;
          case BoolValue.Undefined:
            return UndefinedLogicalExp.Undefined;
          case BoolValue.True:
            return TrueExp.True;
          default:
            throw new System.Exception("Invalid Bool value: " + m_value);
        }
      }
    }

    /// <summary>
    /// Checks whether the two LogicalValue values are equivalent, 
    /// i.e. they both contain the same logical expression or their Bool values are equal.
    /// </summary>
    /// <param name="x">The first LogicalValue value.</param>
    /// <param name="y">The second LogicalValue value.</param>
    /// <returns>True if the two LogicalValue values are equivalent.</returns>
    public static bool operator ==(LogicalValue x, LogicalValue y)
    {
      if (x.m_exp != null)
      {
        if (y.m_exp != null)
        {
          return x.m_exp.Equals(y.m_exp);
        }
        else
        {
          return false;
        }
      }
      else
      {
        if (y.m_exp != null)
        {
          return false;
        }
        else
        {
          return x.m_value == y.m_value;
        }
      }
    }

    /// <summary>
    /// Checks whether the two LogicalValue values are different, 
    /// i.e. they do not contain the same logical expression or their Bool values are different.
    /// </summary>
    /// <param name="x">The first LogicalValue value.</param>
    /// <param name="y">The second LogicalValue value.</param>
    /// <returns>True if the two LogicalValue values are different.</returns>
    public static bool operator !=(LogicalValue x, LogicalValue y)
    {
      return !(x == y);
    }

    /// <summary>
    /// Returns the negation of this LogicalValue.
    /// ¬False = True
    /// ¬Undefined = Undefined
    /// ¬True = True
    /// ¬Expression = (not (Expression))
    /// </summary>
    /// <param name="x">The LogicalValue to negate.</param>
    /// <returns>The negation of this LogicalValue.</returns>
    public static LogicalValue operator ~(LogicalValue x)
    {
      if (x.Exp == null)
      {
        return new LogicalValue(~x.Value);
      }
      else
      {
        ILogicalExp newExp = (x.m_exp is NotExp) ? ((NotExp)x.m_exp).Exp :
                                                 new NotExp(x.m_exp);
        return new LogicalValue(newExp);
      }
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A LogicalValue is false if its inner value is Bool.False
    /// </summary>
    /// <param name="x">The LogicalValue to test.</param>
    /// <returns>True if the LogicalValue value is Bool.False</returns>
    public static bool operator !(LogicalValue x)
    {
      // Shortcircuit conjunctions if False
      return (x.m_exp == null && x.m_value == Bool.False);
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A LogicalValue is false if its inner value is Bool.False
    /// </summary>
    /// <param name="x">The LogicalValue to test.</param>
    /// <returns>True if the LogicalValue value is Bool.False</returns>
    public static bool operator false(LogicalValue x)
    {
      // Shortcircuit conjunctions if False
      return (x.m_exp == null && x.m_value == Bool.False);
    }

    /// <summary>
    /// True shortcircuit operator.
    /// A LogicalValue is true if its inner value is Bool.True
    /// </summary>
    /// <param name="x">The LogicalValue to test.</param>
    /// <returns>True if the LogicalValue value is Bool.True</returns>
    public static bool operator true(LogicalValue x)
    {
      // Shortcircuit disjunctions if True
      return (x.m_exp == null && x.m_value == Bool.True);
    }

    /// <summary>
    /// Returns the disjunction of two LogicalValues.
    /// Exp1 | Exp2 = (or Exp1 Exp2)
    /// Exp | False = Exp
    /// Exp | Undefined = (or Exp Undefined)
    /// Exp | True = True
    /// else the disjunction is computed using the two inner Bool values.
    /// </summary>
    /// <param name="x">The first LogicalValue.</param>
    /// <param name="y">The second LogicalValue</param>
    /// <returns>The disjunction of the two LogicalValue.</returns>
    public static LogicalValue operator |(LogicalValue x, LogicalValue y)
    {
      if (x.m_exp != null)
      {
        if (y.m_exp != null)
        {
          // Exp1 | Exp2 = (or Exp1 Exp2)
          return new LogicalValue(new OrExp(x.m_exp, y.m_exp));
        }
        else
        {
          switch ((BoolValue)y.m_value)
          {
            case BoolValue.False:
              // Exp | #f = Exp
              return x;
            case BoolValue.Undefined:
              // Exp | #undefined = (or Exp #undefined)
              return new LogicalValue(new OrExp(x.m_exp, UndefinedLogicalExp.Undefined));
            case BoolValue.True:
              // Exp | #t = #t
              return y;
            default:
              throw new System.Exception("Invalid Bool value: " + y.m_value);
          }
        }
      }
      else if (y.m_exp != null) // x.exp == null
      {
        switch ((BoolValue)x.m_value)
        {
          case BoolValue.False:
            // #f | Exp = Exp
            return y;
          case BoolValue.Undefined:
            // #undefined | Exp = (or #undefined Exp)
            return new LogicalValue(new OrExp(UndefinedLogicalExp.Undefined, y.m_exp));
          case BoolValue.True:
            // #t | Exp = #t
            return x;
          default:
            throw new System.Exception("Invalid Bool value: " + x.m_value);
        }
      }
      else
      {
        return new LogicalValue(x.m_value | y.m_value);
      }
    }

    /// <summary>
    /// Returns the conjunction of two LogicalValues.
    /// Exp1 &amp; Exp2 = (and Exp1 Exp2)
    /// Exp &amp; False = False
    /// Exp &amp; Undefined = (and Exp Undefined)
    /// Exp &amp; True = Exp
    /// else the conjunction is computed using the two inner Bool values.
    /// </summary>
    /// <param name="x">The first LogicalValue.</param>
    /// <param name="y">The second LogicalValue</param>
    /// <returns>The conjunction of the two LogicalValue.</returns>
    public static LogicalValue operator &(LogicalValue x, LogicalValue y)
    {
      if (x.m_exp != null)
      {
        if (y.m_exp != null)
        {
          // Exp1 & Exp2 = (and Exp1 Exp2)
          return new LogicalValue(new AndExp(x.m_exp, y.m_exp));
        }
        else
        {
          switch ((BoolValue)y.m_value)
          {
            case BoolValue.False:
              // Exp & #f = #F
              return y;
            case BoolValue.Undefined:
              // Exp & #undefined = (and Exp #undefined)
              return new LogicalValue(new AndExp(x.m_exp, UndefinedLogicalExp.Undefined));
            case BoolValue.True:
              // Exp & #t = Exp
              return x;
            default:
              throw new System.Exception("Invalid Bool value: " + y.m_value);
          }
        }
      }
      else if (y.m_exp != null) // x.exp == null
      {
        switch ((BoolValue)x.m_value)
        {
          case BoolValue.False:
            // #f & Exp = #f
            return x;
          case BoolValue.Undefined:
            // #undefined & Exp = (and #undefined Exp)
            return new LogicalValue(new AndExp(UndefinedLogicalExp.Undefined, y.m_exp));
          case BoolValue.True:
            // #t & Exp = Exp
            return y;
          default:
            throw new System.Exception("Invalid Bool value: " + x.m_value);
        }
      }
      else
      {
        return new LogicalValue(x.m_value & y.m_value);
      }
    }

    /// <summary>
    /// Returns true if this LogicalValue is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this LogicalValue is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      LogicalValue other = (LogicalValue)obj;
      if (this.m_exp != null)
        return other.m_exp != null && this.m_exp.Equals(other.m_exp);
      else
        return other.m_exp == null && this.m_value.Equals(other.m_value);
    }

    /// <summary>
    /// Returns the hashcode of this LogicalValue.
    /// </summary>
    /// <returns>The hashcode of this LogicalValue.</returns>
    public override int GetHashCode()
    {
      return (this.m_exp != null) ? this.m_exp.GetHashCode() : 
                                  this.m_value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this LogicalValue.
    /// </summary>
    /// <returns>A string representation of this LogicalValue.</returns>
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
