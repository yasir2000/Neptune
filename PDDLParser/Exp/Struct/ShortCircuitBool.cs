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
  /// A ShortCircuitBool represents a boolean value possibly undefined, which makes it similar
  /// to the regular Bool struct.
  /// The only difference is that the value ShortCircuitBool.Undefined, just like the value False,
  /// also shortcircuits conjunctions:
  /// ShortCircuitBool.Undefined &amp;&amp; ... = ShortCircuitBool.Undefined;
  /// ShortCircuitBool.False     &amp;&amp; ... = ShortCircuitBool.False.
  /// Hence conjunction is not commutative:
  /// ShortCircuitBool.Undefined &amp;&amp; ShortCircuitBool.False     = ShortCircuitBool.Undefined;
  /// ShortCircuitBool.False     &amp;&amp; ShortCircuitBool.Undefined = ShortCircuitBool.False.
  /// It is used to evaluate the body of defined formulas.
  /// </summary>
  [TLPlan]
  public struct ShortCircuitBool
	{
    /// <summary>
    /// ShortCircuitBool false value.
    /// </summary>
    public static ShortCircuitBool False = new ShortCircuitBool(BoolValue.False);

    /// <summary>
    /// ShortCircuitBool undefined value.
    /// </summary>
    public static ShortCircuitBool Undefined = new ShortCircuitBool(BoolValue.Undefined);

    /// <summary>
    ///  ShortCircuitBool true value.
    /// </summary>
    public static ShortCircuitBool True = new ShortCircuitBool(BoolValue.True);
    
    /// <summary>
    /// BoolValue associated with this ShortCircuitBool.
    /// </summary>
    internal BoolValue m_value;

    /// <summary>
    /// Creates a new ShortCircuitBool from a specified boolean value.
    /// </summary>
    /// <param name="value">A boolean value.</param>
    public ShortCircuitBool(bool value)
    {
      this.m_value = (value) ? BoolValue.True : BoolValue.False;
    }

    /// <summary>
    /// Creates a new ShortCircuitBool from a specified BoolValue.
    /// This constructor should stay private.
    /// </summary>
    /// <param name="value">A BoolValue.</param>
    public ShortCircuitBool(BoolValue value)
    {
      this.m_value = value;
    }

    /// <summary>
    /// Gets the BoolValue associated with this ShortCircuitBool. 
    /// </summary>
    public BoolValue Value
    {
      get { return m_value; }
    }

    /// <summary>
    /// Returns the corresponding boolean value of this ShortCircuitBool.
    /// ShortCircuitBool.True yields true,
    /// ShortCircuitBool.False and ShortCircuitBool.Undefined both yield false.
    /// </summary>
    /// <returns>The boolean value of this ShortCircuitBool.</returns>
    public bool ToBool()
    {
      return (Value == BoolValue.True);
    }

    /// <summary>
    /// Checks whether the two ShortCircuitBool values are equivalent.
    /// </summary>
    /// <param name="x">The first ShortCircuitBool value.</param>
    /// <param name="y">The second ShortCircuitBool value.</param>
    /// <returns>True if the two ShortCircuitBool values are equivalent.</returns>
    public static bool operator ==(ShortCircuitBool x, ShortCircuitBool y)
    {
      return x.Value == y.Value;
    }

    /// <summary>
    /// Checks whether the two ShortCircuitBool values are different.
    /// </summary>
    /// <param name="x">The first ShortCircuitBool value.</param>
    /// <param name="y">The second ShortCircuitBool value.</param>
    /// <returns>True if the two ShortCircuitBool values are different.</returns>
    public static bool operator !=(ShortCircuitBool x, ShortCircuitBool y)
    {
      return x.Value != y.Value;
    }

    /// <summary>
    /// Returns the negation of this ShortCircuitBool value.
    /// ¬False = True
    /// ¬Undefined = Undefined
    /// ¬True = True
    /// </summary>
    /// <param name="x">The ShortCircuitBool value to negate.</param>
    /// <returns>The negation of this ShortCircuitBool value.</returns>
    public static ShortCircuitBool operator ~(ShortCircuitBool x)
    {
      return new ShortCircuitBool((BoolValue)(BoolValue.True - x.Value));
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A ShortCircuitBool value is false if its inner value is ShortCircuitBool.False
    /// or ShortCircuitBool.Undefined
    /// </summary>
    /// <param name="x">The ShortCircuitBool value to test.</param>
    /// <returns>True if the ShortCircuitBool value is ShortCircuitBool.False or
    /// ShortCircuitBool.Undefined</returns>
    public static bool operator !(ShortCircuitBool x)
    {
      // Shortcircuit conjunctions if False or Undefined
      return (x.Value != BoolValue.True);
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A ShortCircuitBool value is false if its inner value is ShortCircuitBool.False
    /// or ShortCircuitBool.Undefined
    /// </summary>
    /// <param name="x">The ShortCircuitBool value to test.</param>
    /// <returns>True if the ShortCircuitBool value is ShortCircuitBool.False or
    /// ShortCircuitBool.Undefined</returns>
    public static bool operator false(ShortCircuitBool x)
    {
      // Shortcircuit conjunctions if False or Undefined
      return (x.Value != BoolValue.True);
    }

    /// <summary>
    /// True shortcircuit operator.
    /// A ShortCircuitBool value is true if its inner value is ShortCircuitBool.True
    /// </summary>
    /// <param name="x">The ShortCircuitBool value to test.</param>
    /// <returns>True if the ShortCircuitBool value is ShortCircuitBool.True</returns>
    public static bool operator true(ShortCircuitBool x)
    {
      // Shortcircuit disjunctions if True
      return (x.Value == BoolValue.True);
    }

    /// <summary>
    /// Returns the disjunction of two ShortCircuitBool values.
    /// The resulting ShortCircuitBool value corresponds to the maximum BoolValue 
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first ShortCircuitBool value.</param>
    /// <param name="y">The second ShortCircuitBool value.</param>
    /// <returns>The disjunction of the two ShortCircuitBool values.</returns>
    public static ShortCircuitBool operator |(ShortCircuitBool x, ShortCircuitBool y)
    {
      // Max(x, y)
      return x.Value > y.Value ? x : y;
    }

    /// <summary>
    /// Returns the conjunction of two ShortCircuitBool values.
    /// The resulting ShortCircuitBool value corresponds to the minimum BoolValue
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first ShortCircuitBool value.</param>
    /// <param name="y">The second ShortCircuitBool value.</param>
    /// <returns>The conjunction of the two ShortCircuitBool values.</returns>
    public static ShortCircuitBool operator &(ShortCircuitBool x, ShortCircuitBool y)
    {
      // Min(x, y)
      return x.Value < y.Value ? x : y;
    }

    /// <summary>
    /// Returns the exclusive disjunction of two ShortCircuitBool values.
    /// </summary>
    /// <param name="x">The first ShortCircuitBool value.</param>
    /// <param name="y">The second ShortCircuitBool value.</param>
    /// <returns>The exclusive disjunction of the two ShortCircuitBool values.</returns>
    public static ShortCircuitBool operator ^(ShortCircuitBool x, ShortCircuitBool y)
    {
      if (x.Value == BoolValue.Undefined || y.Value == BoolValue.Undefined)
        return ShortCircuitBool.Undefined;
      else
        return new ShortCircuitBool(!(x.Value == y.Value));
    }

    /// <summary>
    /// Returns the BoolValue associated with a ShortCircuitBool value.
    /// </summary>
    /// <param name="x">The ShortCircuitBool value.</param>
    /// <returns>The BoolValue associated with the ShortCircuitBool value.</returns>
    public static implicit operator BoolValue (ShortCircuitBool x)
    {
      return x.Value;
    }

    /// <summary>
    /// Returns true if this ShortCircuitBool value is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this ShortCircuitBool value is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      return ((ShortCircuitBool)obj).Value == this.Value;
    }

    /// <summary>
    /// Returns the hashcode of this ShortCircuitBool value.
    /// </summary>
    /// <returns>The hashcode of this ShortCircuitBool value.</returns>
    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the ShortCircuitBool value.
    /// </summary>
    /// <returns>A string representation of the ShortCircuitBool value.</returns>
    public override string ToString()
    {
      return this.Value.ToString();
    }
	}
}
