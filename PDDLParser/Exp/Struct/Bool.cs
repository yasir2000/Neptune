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
  /// All possible Bool values.
  /// </summary>
  public enum BoolValue : int
  {
    /// <summary>
    /// The false BoolValue.
    /// </summary>
    False = 0,
    /// <summary>
    /// The undefined BoolValue.
    /// </summary>
    Undefined = 2,
    /// <summary>
    /// The true BoolValue.
    /// </summary>
    True = 4
  }

  /// <summary>
  /// A Bool wraps a BoolValue and thus represents a possibly undefined boolean value.
  /// Its domain is { False, Undefined, True }.
  /// Note that ¬Undefined = Undefined.
  /// Bool.False shortcircuits conjunctions and Bool.True shortcircuits disjunctions.
  /// Bool.Undefined never shortcircuits the evaluation.
  /// </summary>
  public struct Bool
  {
    /// <summary>
    /// Bool false value.
    /// </summary>
    public static Bool False = new Bool(BoolValue.False);

    /// <summary>
    /// Bool undefined value.
    /// </summary>
    public static Bool Undefined = new Bool(BoolValue.Undefined);

    /// <summary>
    ///  Bool true value.
    /// </summary>
    public static Bool True = new Bool(BoolValue.True);
    
    /// <summary>
    /// Inner BoolValue of this Bool.
    /// </summary>
    internal BoolValue m_value;

    /// <summary>
    /// Creates a new Bool from a specified boolean value.
    /// </summary>
    /// <param name="value">A boolean value.</param>
    public Bool(bool value)
    {
      this.m_value = (value) ? BoolValue.True : BoolValue.False;
    }

    /// <summary>
    /// Creates a new Bool from a specified BoolValue.
    /// This constructor should stay private.
    /// </summary>
    /// <param name="value">A BoolValue.</param>
    public Bool(BoolValue value)
    {
      this.m_value = value;
    }

    /// <summary>
    /// Returns the BoolValue associated with this Bool.
    /// </summary>
    public BoolValue Value
    {
      get { return m_value; }
    }

    /// <summary>
    /// Returns the corresponding truth value of this Bool.
    /// Bool.True yields true,
    /// Bool.False and Bool.Undefined both yield false.
    /// </summary>
    /// <returns>The boolean value of this Bool.</returns>
    public bool ToBool()
    {
      return this.m_value == BoolValue.True;
    }

    /// <summary>
    /// Checks whether the two Bool values are equivalent.
    /// </summary>
    /// <param name="x">The first Bool value.</param>
    /// <param name="y">The second Bool value.</param>
    /// <returns>True if the two Bool values are equivalent.</returns>
    public static bool operator ==(Bool x, Bool y)
    {
      return x.Value == y.Value;
    }

    /// <summary>
    /// Checks whether the two Bool values are different.
    /// </summary>
    /// <param name="x">The first Bool value.</param>
    /// <param name="y">The second Bool value.</param>
    /// <returns>True if the two Bool values are different.</returns>
    public static bool operator !=(Bool x, Bool y)
    {
      return x.Value != y.Value;
    }

    /// <summary>
    /// Returns the negation of this Bool value.
    /// ¬False = True
    /// ¬Undefined = Undefined
    /// ¬True = True
    /// </summary>
    /// <param name="x">The Bool value to negate.</param>
    /// <returns>The negation of this Bool value.</returns>
    public static Bool operator ~(Bool x)
    {
      return new Bool((BoolValue)(BoolValue.True - x.Value));
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A Bool value is false if its inner value is Bool.False
    /// </summary>
    /// <param name="x">The Bool value to test.</param>
    /// <returns>True if the Bool value is Bool.False</returns>
    public static bool operator !(Bool x)
    {
      // Shortcircuit conjunctions if False
      return (x.Value == BoolValue.False);
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A Bool value is false if its inner value is Bool.False
    /// </summary>
    /// <param name="x">The Bool value to test.</param>
    /// <returns>True if the Bool value is Bool.False</returns>
    public static bool operator false(Bool x)
    {
      // Shortcircuit conjunctions if False
      return (x.Value == BoolValue.False);
    }

    /// <summary>
    /// True shortcircuit operator.
    /// A Bool value is true if its inner value is Bool.True
    /// </summary>
    /// <param name="x">The Bool value to test.</param>
    /// <returns>True if the Bool value is Bool.True</returns>
    public static bool operator true(Bool x)
    {
      // Shortcircuit disjunctions if True
      return (x.Value == BoolValue.True);
    }

    /// <summary>
    /// Returns the disjunction of two Bool values.
    /// The resulting Bool value corresponds to the maximum BoolValue 
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first Bool value.</param>
    /// <param name="y">The second Bool value.</param>
    /// <returns>The disjunction of the two Bool values.</returns>
    public static Bool operator |(Bool x, Bool y)
    {
      // Max(x, y)
      return x.Value > y.Value ? x : y;
    }

    /// <summary>
    /// Returns the conjunction of two Bool values.
    /// The resulting Bool value corresponds to the minimum BoolValue 
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first Bool value.</param>
    /// <param name="y">The second Bool value.</param>
    /// <returns>The conjunction of the two Bool values.</returns>
    public static Bool operator &(Bool x, Bool y)
    {
      // Min(x, y)
      return x.Value < y.Value ? x : y;
    }

    /// <summary>
    /// Returns the exclusive disjunction of two Bool values.
    /// </summary>
    /// <param name="x">The first Bool value.</param>
    /// <param name="y">The second Bool value.</param>
    /// <returns>The exclusive disjunction of the two Bool values.</returns>
    public static Bool operator ^(Bool x, Bool y)
    {
      if (x.Value == BoolValue.Undefined || y.Value == BoolValue.Undefined)
        return Bool.Undefined;
      else
        return new Bool(!(x.Value == y.Value));
    }

    /// <summary>
    /// Returns the BoolValue associated with a Bool.
    /// </summary>
    /// <param name="x">The Bool value.</param>
    /// <returns>The BoolValue associated with the Bool.</returns>
    public static implicit operator BoolValue (Bool x)
    {
      return x.Value;
    }

    /// <summary>
    /// Returns true if this Bool value is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this Bool value is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      // Assume obj is Bool
      return ((Bool)obj).Value == this.Value;
    }

    /// <summary>
    /// Returns the hashcode of this Bool value.
    /// </summary>
    /// <returns>The hashcode of this Bool value.</returns>
    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the Bool value.
    /// </summary>
    /// <returns>A string representation of the Bool value.</returns>
    public override string ToString()
    {
      return Value.ToString();
    }
  }
}
