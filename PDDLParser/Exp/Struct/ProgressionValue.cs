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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Constraint;
using PDDLParser.Exp.Logical;

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// A ProgressionValue represents a constraint expression, a boolean value, or an undefined value.
  /// </summary>
  public struct ProgressionValue
  {
    /// <summary>
    /// ProgressionValue true value.
    /// </summary>
    public static readonly ProgressionValue True;

    /// <summary>
    /// ProgressionValue false value.
    /// </summary>
    public static readonly ProgressionValue False;

    /// <summary>
    /// ProgressionValue undefined value.
    /// </summary>
    public static readonly ProgressionValue Undefined;

    /// <summary>
    /// A timestamp value meaning no event of interest has been created by the progression.
    /// </summary>
    public static readonly TimeValue NoTimestamp;

    /// <summary>
    /// The Bool value associated with this ProgressionValue.
    /// Note that this value is not defined if a corresponding constraint expression is defined.
    /// </summary>
    private Bool m_value;
    /// <summary>
    /// The constraint expression associated with this ProgressionValue.
    /// </summary>
    private IConstraintExp m_exp;

    /// <summary>
    /// The next "interesting" timestamp, i.e. the timestamp at which the next change occurs.
    /// </summary>
    private TimeValue m_nextAbsoluteTimestamp;

    /// <summary>
    /// This static constructor initializes static fields.
    /// </summary>
    static ProgressionValue()
    {
      // This needs to be initialized before any ProgressionValue struct is built!
      NoTimestamp = new TimeValue(double.PositiveInfinity, true, false);

      True = new ProgressionValue(Bool.True);
      False = new ProgressionValue(Bool.False);
      Undefined = new ProgressionValue(Bool.Undefined);
    }

    /// <summary>
    /// Creates a new ProgressionValue from a specified bool value.
    /// </summary>
    /// <param name="value">A bool value.</param>
    public ProgressionValue(bool value)
      : this(new Bool(value))
    {
    }

    /// <summary>
    /// Creates a new ProgressionValue from a specified Bool value.
    /// </summary>
    /// <param name="value">A Bool value.</param>
    public ProgressionValue(Bool value)
    {
      this.m_value = value;
      this.m_exp = null;
      this.m_nextAbsoluteTimestamp = ProgressionValue.NoTimestamp;
    }

    /// <summary>
    /// Creates a new ProgressionValue from a specified constraint expression.
    /// </summary>
    /// <param name="exp">A constraint expression.</param>
    /// <param name="nextAbsoluteTimestamp">The next smallest absolute timestamp at which a change occurs.</param>
    public ProgressionValue(IConstraintExp exp, TimeValue nextAbsoluteTimestamp)
      : this(Bool.Undefined, exp, nextAbsoluteTimestamp)
    {
    }

    /// <summary>
    /// Creates a new ProgressionValue from a specified result and constraint expression.
    /// Note that this constructor should stay private.
    /// </summary>
    /// <param name="value">A Bool value.</param>
    /// <param name="exp">A constraint expression.</param>
    /// <param name="nextAbsoluteTimestamp">The next smallest absolute timestamp at which a change occurs.</param>
    private ProgressionValue(Bool value, IConstraintExp exp, TimeValue nextAbsoluteTimestamp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_value = value;
      this.m_exp = exp;
      this.m_nextAbsoluteTimestamp = nextAbsoluteTimestamp;
    }

    /// <summary>
    /// Returns the constraint expression stored in this ProgressionValue.
    /// Note that this constraint expression may be null.
    /// </summary>
    public IConstraintExp Exp
    {
      get { return m_exp; }
    }

    /// <summary>
    /// Returns the Bool value stored in this ProgressionValue.
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
    /// Returns the next (smallest) absolute timestamp at which a change will occur.
    /// </summary>
    public TimeValue NextAbsoluteTimestamp
    {
      get
      {
        return m_nextAbsoluteTimestamp;
      }
    }

    /// <summary>
    /// Returns a constraint expression equivalent to this ProgressionValue.
    /// If this ProgressionValue corresponds to a Bool value, an equivalent constant expression
    /// is returned (false -> FalseExp, Undefined -> UndefinedExp, True -> TrueExp).
    /// </summary>
    /// <returns>A constraint expression equivalent to this ProgressionValue.</returns>
    public IConstraintExp GetEquivalentExp()
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
    /// Returns whether this progression value evaluates to false, i.e. if its value
    /// is Bool.False or Bool.Undefined
    /// </summary>
    /// <returns>Whether this progression value evaluates to false.</returns>
    public bool IsFalse()
    {
      return this.m_exp == null && (this.m_value == Bool.False ||
                                    this.m_value == Bool.Undefined);
    }

    /// <summary>
    /// Returns whether this progression value evaluates to true, i.e. if its value
    /// is Bool.True
    /// </summary>
    /// <returns>Whether this progression value evaluates to true.</returns>
    public bool IsTrue()
    {
      return this.m_exp == null && (this.m_value == Bool.True);
    }

    /// <summary>
    /// Checks whether the two ProgressionValue values are equivalent, i.e. they both contain
    /// the same constraint expression and timestamp or their Bool values are equal.
    /// </summary>
    /// <param name="x">The first ProgressionValue value.</param>
    /// <param name="y">The second ProgressionValue value.</param>
    /// <returns>True if the two ProgressionValue values are equivalent.</returns>
    public static bool operator ==(ProgressionValue x, ProgressionValue y)
    {
      if (x.m_exp != null)
      {
        if (y.m_exp != null)
        {
          return (x.m_exp.Equals(y.m_exp) &&
                  x.NextAbsoluteTimestamp.Equals(y.NextAbsoluteTimestamp));
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
    /// Checks whether the two ProgressionValue values are different, i.e. they do not contain
    /// the same constraint expression and timestamp or their Bool values are different.
    /// </summary>
    /// <param name="x">The first ProgressionValue value.</param>
    /// <param name="y">The second ProgressionValue value.</param>
    /// <returns>True if the two ProgressionValue values are different.</returns>
    public static bool operator !=(ProgressionValue x, ProgressionValue y)
    {
      return !(x == y);
    }

    /// <summary>
    /// Returns the negation of this ProgressionValue.
    /// ¬False = True
    /// ¬Undefined = Undefined
    /// ¬True = False
    /// ¬Expression = (not (Expression))
    /// </summary>
    /// <param name="x">The ProgressionValue to negate.</param>
    /// <returns>The negation of this ProgressionValue.</returns>
    public static ProgressionValue operator ~(ProgressionValue x)
    {
      if (x.Exp == null)
      {
        return new ProgressionValue(~x.Value);
      }
      else
      {
        IConstraintExp newExp = (x.m_exp is NotConstraintExp) ? 
                                        ((NotConstraintExp)x.m_exp).Exp :
                                        new NotConstraintExp(x.m_exp);
        return new ProgressionValue(newExp, x.NextAbsoluteTimestamp);
      }
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A ProgressionValue is false if its inner value is Bool.False
    /// </summary>
    /// <param name="x">The ProgressionValue to test.</param>
    /// <returns>True if the ProgressionValue value is Bool.False</returns>
    public static bool operator !(ProgressionValue x)
    {
      // Shortcircuit conjunctions if False
      return (x.m_exp == null && x.m_value == Bool.False);
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A ProgressionValue is false if its inner value is Bool.False
    /// </summary>
    /// <param name="x">The ProgressionValue to test.</param>
    /// <returns>True if the ProgressionValue value is Bool.False</returns>
    public static bool operator false(ProgressionValue x)
    {
      // Shortcircuit conjunctions if False
      return (x.m_exp == null && x.m_value == Bool.False);
    }

    /// <summary>
    /// True shortcircuit operator.
    /// A ProgressionValue is true if its inner value is Bool.True
    /// </summary>
    /// <param name="x">The ProgressionValue to test.</param>
    /// <returns>True if the ProgressionValue value is Bool.True</returns>
    public static bool operator true(ProgressionValue x)
    {
      // Shortcircuit disjunctions if True
      return (x.m_exp == null && x.m_value == Bool.True);
    }

    /// <summary>
    /// Returns the disjunction of two ProgressionValues.
    /// Exp1 | Exp2 = (or Exp1 Exp2)
    /// Exp | False = Exp
    /// Exp | Undefined = (or Exp Undefined)
    /// Exp | True = True
    /// else the disjunction is computed using the two inner Bool values.
    /// </summary>
    /// <param name="x">The first ProgressionValue.</param>
    /// <param name="y">The second ProgressionValue</param>
    /// <returns>The disjunction of the two ProgressionValue.</returns>
    public static ProgressionValue operator |(ProgressionValue x, ProgressionValue y)
    {
      if (x.m_exp != null)
      {
        if (y.m_exp != null)
        {
          // Exp1 | Exp2 = (or Exp1 Exp2)
          return new ProgressionValue(new OrConstraintExp(x.m_exp, y.m_exp),
                                      TimeValue.Min(x.NextAbsoluteTimestamp, y.NextAbsoluteTimestamp));
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
              return new ProgressionValue(new OrConstraintExp(x.m_exp, UndefinedLogicalExp.Undefined),
                                          x.NextAbsoluteTimestamp);
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
            return new ProgressionValue(new OrConstraintExp(UndefinedLogicalExp.Undefined, y.m_exp),
                                        y.NextAbsoluteTimestamp);
          case BoolValue.True:
            // #t | Exp = #t
            return x;
          default:
            throw new System.Exception("Invalid Bool value: " + x.m_value);
        }
      }
      else
      {
        return new ProgressionValue(x.m_value | y.m_value);
      }
    }

    /// <summary>
    /// Returns the conjunction of two ProgressionValues.
    /// Exp1 &amp; Exp2 = (and Exp1 Exp2)
    /// Exp &amp; False = False
    /// Exp &amp; Undefined = (and Exp Undefined)
    /// Exp &amp; True = Exp
    /// else the conjunction is computed using the two inner Bool values.
    /// </summary>
    /// <param name="x">The first ProgressionValue.</param>
    /// <param name="y">The second ProgressionValue</param>
    /// <returns>The conjunction of the two ProgressionValue.</returns>
    public static ProgressionValue operator &(ProgressionValue x, ProgressionValue y)
    {
      if (x.m_exp != null)
      {
        if (y.m_exp != null)
        {
          // Exp1 & Exp2 = (and Exp1 Exp2)
          return new ProgressionValue(new AndConstraintExp(x.m_exp, y.m_exp),
                                      TimeValue.Min(x.NextAbsoluteTimestamp, y.NextAbsoluteTimestamp));
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
              return new ProgressionValue(new AndConstraintExp(x.m_exp, UndefinedLogicalExp.Undefined),
                                          x.NextAbsoluteTimestamp);
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
            return new ProgressionValue(new AndConstraintExp(UndefinedLogicalExp.Undefined, y.m_exp),
                                        y.NextAbsoluteTimestamp);
          case BoolValue.True:
            // #t & Exp = Exp
            return y;
          default:
            throw new System.Exception("Invalid Bool value: " + x.m_value);
        }
      }
      else
      {
        return new ProgressionValue(x.m_value & y.m_value);
      }
    }

    /// <summary>
    /// Returns true if this ProgressionValue is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this ProgressionValue is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      ProgressionValue other = (ProgressionValue)obj;
      if (this.m_exp != null)
        return other.m_exp != null && this.m_exp.Equals(other.m_exp)
                                   && this.NextAbsoluteTimestamp.Equals(other.NextAbsoluteTimestamp);
      else
        return other.m_exp == null && this.m_value.Equals(other.m_value);
    }

    /// <summary>
    /// Returns the hashcode of this ProgressionValue.
    /// </summary>
    /// <returns>The hashcode of this ProgressionValue.</returns>
    public override int GetHashCode()
    {
      return (this.m_exp != null) ? (this.m_exp.GetHashCode() + 17 * this.NextAbsoluteTimestamp.GetHashCode()) :
                                    this.m_value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this ProgressionValue.
    /// </summary>
    /// <returns>A string representation of this ProgressionValue.</returns>
    public override string ToString()
    {
      if (m_exp != null)
      {
        if (!this.NextAbsoluteTimestamp.Equals(ProgressionValue.NoTimestamp))
          return string.Format("{0} ({1})", m_exp, this.NextAbsoluteTimestamp);
        else
          return m_exp.ToString();
      }
      else
      {
        return m_value.ToString();
      }
    }
  }
}
