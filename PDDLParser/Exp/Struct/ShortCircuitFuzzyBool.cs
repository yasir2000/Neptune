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

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// A ShortCircuitFuzzyBool represents a fuzzy boolean value possibly undefined or unknown, 
  /// which makes it similar to the regular FuzzyBool struct.
  /// However there are two differences:
  /// 1 - The value ShortCircuitFuzzyBool.Undefined, just like the value False, also shortcircuits
  /// conjunctions:
  /// ShortCircuitFuzzyBool.Undefined &amp;&amp; ... = ShortCircuitFuzzyBool.Undefined;
  /// ShortCircuitFuzzyBool.False     &amp;&amp; ... = ShortCircuitFuzzyBool.False.
  /// Hence conjunction is not commutative:
  /// ShortCircuitFuzzyBool.Undefined &amp;&amp; ShortCircuitFuzzyBool.False     = ShortCircuitFuzzyBool.Undefined;
  /// ShortCircuitFuzzyBool.False     &amp;&amp; ShortCircuitFuzzyBool.Undefined = ShortCircuitFuzzyBool.False.
  /// 
  /// 2 - The value ShortCircuitFuzzyBool.Unknown shortcircuits both conjunctions and disjunctions:
  /// ShortCircuitFuzzyBool.Unknown   &amp;&amp; ... = ShortCircuitFuzzyBool.Unknown;
  /// ShortCircuitFuzzyBool.Unknown   || ... = ShortCircuitFuzzyBool.Unknown;
  /// ShortCircuitFuzzyBool.True      || ... = ShortCircuitFuzzyBool.True.
  /// Hence disjunction is not commutative:
  /// ShortCircuitFuzzyBool.Unknown   || ShortCircuitFuzzyBool.True    = ShortCircuitFuzzyBool.Unknown;
  /// ShortCircuitFuzzyBool.True      || ShortCircuitFuzzyBool.Unknown = ShortCircuitFuzzyBool.True.
  /// It is used to evaluate the body of defined formulas. 
  /// </summary>
  [TLPlan]
  public struct ShortCircuitFuzzyBool
	{
    /// <summary>
    /// ShortCircuitFuzzyBool false value.
    /// </summary>
    public static ShortCircuitFuzzyBool False = new ShortCircuitFuzzyBool(FuzzyBoolValue.False);

    /// <summary>
    /// ShortCircuitFuzzyBool undefined value.
    /// </summary>
    public static ShortCircuitFuzzyBool Undefined = new ShortCircuitFuzzyBool(FuzzyBoolValue.Undefined);

    /// <summary>
    /// ShortCircuitFuzzyBool unknown value.
    /// </summary>
    public static ShortCircuitFuzzyBool Unknown = new ShortCircuitFuzzyBool(FuzzyBoolValue.Unknown);

    /// <summary>
    /// ShortCircuitFuzzyBool true value.
    /// </summary>
    public static ShortCircuitFuzzyBool True = new ShortCircuitFuzzyBool(FuzzyBoolValue.True);

    /// <summary>
    /// The FuzzyBoolValue associated with this ShortCircuitFuzzyBool value.
    /// </summary>
    internal FuzzyBoolValue m_value;

    /// <summary>
    /// Creates a new ShortCircuitFuzzyBool from a specified boolean value.
    /// </summary>
    /// <param name="value">A boolean value.</param>
    public ShortCircuitFuzzyBool(bool value)
    {
      this.m_value = (value) ? FuzzyBoolValue.True : FuzzyBoolValue.False;
    }

    /// <summary>
    /// Creates a new ShortCircuitFuzzyBool from a specified BoolValue.
    /// </summary>
    /// <param name="value">A BoolValue.</param>
    public ShortCircuitFuzzyBool(BoolValue value)
    {
      this.m_value = FuzzyBool.BoolValueToFuzzyBoolValue(value);
    }

    /// <summary>
    /// Creates a new ShortCircuitFuzzyBool from a specified FuzzyBoolValue.
    /// This constructor should stay private.
    /// </summary>
    /// <param name="value">A FuzzyBoolValue.</param>
    public ShortCircuitFuzzyBool(FuzzyBoolValue value)
    {
      this.m_value = value;
    }

    /// <summary>
    /// Returns the FuzzyBoolValue associated with this ShortCircuitFuzzyBool.
    /// </summary>
    public FuzzyBoolValue Value
    {
      get { return m_value; }
    }

    /// <summary>
    /// Checks whether the two ShortCircuitFuzzyBool values are equivalent.
    /// </summary>
    /// <param name="x">The first ShortCircuitFuzzyBool value.</param>
    /// <param name="y">The second ShortCircuitFuzzyBool value.</param>
    /// <returns>True if the two ShortCircuitFuzzyBool values are equivalent.</returns>
    public static bool operator ==(ShortCircuitFuzzyBool x, ShortCircuitFuzzyBool y)
    {
      return x.Value == y.Value;
    }

    /// <summary>
    /// Checks whether the two ShortCircuitFuzzyBool values are different.
    /// </summary>
    /// <param name="x">The first ShortCircuitFuzzyBool value.</param>
    /// <param name="y">The second ShortCircuitFuzzyBool value.</param>
    /// <returns>True if the two ShortCircuitFuzzyBool values are different.</returns>
    public static bool operator !=(ShortCircuitFuzzyBool x, ShortCircuitFuzzyBool y)
    {
      return x.Value != y.Value;
    }

    /// <summary>
    /// Returns the negation of this ShortCircuitFuzzyBool value.
    /// ¬False = True
    /// ¬Undefined = Undefined
    /// ¬Unknown = Unknown
    /// ¬True = True
    /// </summary>
    /// <param name="x">The ShortCircuitFuzzyBool value to negate.</param>
    /// <returns>The negation of this ShortCircuitFuzzyBool value.</returns>
    public static ShortCircuitFuzzyBool operator ~(ShortCircuitFuzzyBool x)
    {
      switch (x.Value)
      {
        case FuzzyBoolValue.False:
          return ShortCircuitFuzzyBool.True;
        case FuzzyBoolValue.True:
          return ShortCircuitFuzzyBool.False;
        default:
          // ¬Undefined = Undefined, ¬Unknown = Unknown
          return x;
      }
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A ShortCircuitFuzzyBool value is false if its inner value is ShortCircuitFuzzyBool.False
    /// </summary>
    /// <param name="x">The ShortCircuitFuzzyBool value to test.</param>
    /// <returns>True if the ShortCircuitFuzzyBool value is ShortCircuitFuzzyBool.False</returns>
    public static bool operator !(ShortCircuitFuzzyBool x)
    {
      // Shortcircuit conjunctions if False, Undefined or Unknown
      return (x.Value != FuzzyBoolValue.True);
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A ShortCircuitFuzzyBool value is false if its inner value is ShortCircuitFuzzyBool.False
    /// </summary>
    /// <param name="x">The ShortCircuitFuzzyBool value to test.</param>
    /// <returns>True if the ShortCircuitFuzzyBool value is ShortCircuitFuzzyBool.False</returns>
    public static bool operator false(ShortCircuitFuzzyBool x)
    {
      // Shortcircuit conjunctions if False, Undefined or Unknown
      return (x.Value != FuzzyBoolValue.True);
    }

    /// <summary>
    /// True shortcircuit operator.
    /// A ShortCircuitFuzzyBool value is true if its inner value is ShortCircuitFuzzyBool.True
    /// </summary>
    /// <param name="x">The ShortCircuitFuzzyBool value to test.</param>
    /// <returns>True if the ShortCircuitFuzzyBool value is ShortCircuitFuzzyBool.True</returns>
    public static bool operator true(ShortCircuitFuzzyBool x)
    {
      // Shortcircuit disjunctions if True or Unknown
      return (x.Value == FuzzyBoolValue.True ||
              x.Value == FuzzyBoolValue.Unknown);
    }

    /// <summary>
    /// Returns the disjunction of two ShortCircuitFuzzyBool values.
    /// The resulting ShortCircuitFuzzyBool value corresponds to the maximum FuzzyBoolValue
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first ShortCircuitFuzzyBool value.</param>
    /// <param name="y">The second ShortCircuitFuzzyBool value.</param>
    /// <returns>The disjunction of the two ShortCircuitFuzzyBool values.</returns>
    public static ShortCircuitFuzzyBool operator |(ShortCircuitFuzzyBool x, ShortCircuitFuzzyBool y)
    {
      if (x.Value == FuzzyBoolValue.Unknown || y.Value == FuzzyBoolValue.Unknown)
        return ShortCircuitFuzzyBool.Unknown;
      else
        return x.Value > y.Value ? x : y;
    }

    /// <summary>
    /// Returns the conjunction of two ShortCircuitFuzzyBool values.
    /// The resulting ShortCircuitFuzzyBool value corresponds to the minimum FuzzyBoolValue 
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first ShortCircuitFuzzyBool value.</param>
    /// <param name="y">The second ShortCircuitFuzzyBool value.</param>
    /// <returns>The conjunction of the two ShortCircuitFuzzyBool values.</returns>
    public static ShortCircuitFuzzyBool operator &(ShortCircuitFuzzyBool x, ShortCircuitFuzzyBool y)
    {
      if (x.Value == FuzzyBoolValue.Unknown || y.Value == FuzzyBoolValue.Unknown)
        return ShortCircuitFuzzyBool.Unknown;
      else
        return x.Value < y.Value ? x : y;

    }

    /// <summary>
    /// Returns the exclusive disjunction of two ShortCircuitFuzzyBool values.
    /// </summary>
    /// <param name="x">The first ShortCircuitFuzzyBool value.</param>
    /// <param name="y">The second ShortCircuitFuzzyBool value.</param>
    /// <returns>The exclusive disjunction of the two ShortCircuitFuzzyBool values.</returns>
    public static ShortCircuitFuzzyBool operator ^(ShortCircuitFuzzyBool x, ShortCircuitFuzzyBool y)
    {
      if (x.Value == FuzzyBoolValue.Unknown || y.Value == FuzzyBoolValue.Unknown)
        return ShortCircuitFuzzyBool.Unknown;
      else if (x.Value == FuzzyBoolValue.Undefined || y.Value == FuzzyBoolValue.Undefined)
        return ShortCircuitFuzzyBool.Undefined;
      else
        return new ShortCircuitFuzzyBool(!(x.Value == y.Value));
    }

    /// <summary>
    /// Returns the FuzzyBoolValue associated with a ShortCircuitFuzzyBool value.
    /// </summary>
    /// <param name="x">The ShortCircuitFuzzyBool value.</param>
    /// <returns>The FuzzyBoolValue associated with the ShortCircuitFuzzyBool value.</returns>
    public static implicit operator FuzzyBoolValue (ShortCircuitFuzzyBool x)
    {
      return x.Value;
    }

    /// <summary>
    /// Returns true if this ShortCircuitFuzzyBool value is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this ShortCircuitFuzzyBool value is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      return ((ShortCircuitFuzzyBool)obj).Value == this.Value;
    }

    /// <summary>
    /// Returns the hashcode of this ShortCircuitFuzzyBool value.
    /// </summary>
    /// <returns>The hashcode of this ShortCircuitFuzzyBool value.</returns>
    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the ShortCircuitFuzzyBool value.
    /// </summary>
    /// <returns>A string representation of the ShortCircuitFuzzyBool value.</returns>
    public override string ToString()
    {
      return this.Value.ToString();
    }
	}
}
