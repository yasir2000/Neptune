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

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// All possible FuzzyBool values.
  /// </summary>
  public enum FuzzyBoolValue : int
  {
    /// <summary>
    /// The false FuzzyBool value.
    /// </summary>
    False = 0,
    /// <summary>
    /// The undefined FuzzyBool value.
    /// </summary>
    Undefined = 2,
    /// <summary>
    /// The unknown FuzzyBool value.
    /// </summary>
    Unknown = 3,
    /// <summary>
    /// The true FuzzyBool value.
    /// </summary>
    True = 4
  }

  /// <summary>
  /// A FuzzyBool represents a fuzzy boolean value possibly undefined or unknown.
  /// It wraps a FuzzyBoolValue.
  /// Hence its domain is { False, Undefined, Unknown, True }.
  /// 
  /// Note that ¬Undefined = Undefined and ¬Unknown = Unknown.
  /// FuzzyBool.False shortcircuits conjunctions and FuzzyBool.True shortcircuits disjunctions.
  /// FuzzyBool.Undefined and FuzzyBool.Unknown do not shortcircuit the evaluation.
  /// </summary>
  public struct FuzzyBool
  {
    /// <summary>
    /// FuzzyBool false value.
    /// </summary>
    public static FuzzyBool False = new FuzzyBool(FuzzyBoolValue.False);

    /// <summary>
    /// FuzzyBool undefined value.
    /// </summary>
    public static FuzzyBool Undefined = new FuzzyBool(FuzzyBoolValue.Undefined);

    /// <summary>
    /// FuzzyBool unknown value.
    /// </summary>
    public static FuzzyBool Unknown = new FuzzyBool(FuzzyBoolValue.Unknown);

    /// <summary>
    /// FuzzyBool true value.
    /// </summary>
    public static FuzzyBool True = new FuzzyBool(FuzzyBoolValue.True);

    /// <summary>
    /// The corresponding FuzzyBoolValue of this FuzzyBool value.
    /// </summary>
    internal FuzzyBoolValue m_value;

    /// <summary>
    /// Creates a new FuzzyBool from a specified boolean value.
    /// </summary>
    /// <param name="value">A boolean value.</param>
    public FuzzyBool(bool value)
    {
      this.m_value = (value) ? FuzzyBoolValue.True : FuzzyBoolValue.False;
    }

    /// <summary>
    /// Creates a new FuzzyBool from a specified FuzzyBoolValue.
    /// This constructor should stay private.
    /// </summary>
    /// <param name="value">A FuzzyBoolValue.</param>
    public FuzzyBool(FuzzyBoolValue value)
    {
      this.m_value = value;
    }

    /// <summary>
    /// Creates a new FuzzyBool from a specified BoolValue.
    /// </summary>
    /// <param name="value">A BoolValue.</param>
    public FuzzyBool(BoolValue value)
    {
      this.m_value = BoolValueToFuzzyBoolValue(value);
    }
    
    /// <summary>
    /// Returns the FuzzyBoolValue associated with this FuzzyBool.
    /// </summary>
    public FuzzyBoolValue Value
    {
      get { return m_value; }
    }

    /// <summary>
    /// Returns the FuzzyBoolValue corresponding to a BoolValue.
    /// </summary>
    /// <param name="value">A BoolValue.</param>
    /// <returns>The FuzzyBoolValue corresponding to the BoolValue.</returns>
    public static FuzzyBoolValue BoolValueToFuzzyBoolValue(BoolValue value)
    {
      return (FuzzyBoolValue)value;
    }

    /// <summary>
    /// Returns the BoolValue value corresponding to a FuzzyBoolValue.
    /// Note that FuzzyBoolValue.Unknown cannot be converted to BoolValue and thus throws
    /// and UnknownEvaluationResultException.
    /// </summary>
    /// <param name="value">A FuzzyBoolValue.</param>
    /// <returns>The FuzzyBoolValue corresponding to the BoolValue.</returns>
    public static BoolValue FuzzyBoolValueToBoolValue(FuzzyBoolValue value)
    {
      if (value == FuzzyBoolValue.Unknown)
        throw new UnknownExpException("Cannot convert FuzzyBoolValue.Unknown to BoolValue.");
      else
        return (BoolValue)value;
    }

    /// <summary>
    /// Returns the corresponding Bool value of this FuzzyBool.
    /// Note that FuzzyBool.Unknown cannot be converted to boolean and thus 
    /// throws an UnknownEvaluationResultException.
    /// </summary>
    /// <returns>The corresponding Bool value of this FuzzyBool.</returns>
    public Bool ToBoolValue()
    {
      return new Bool(FuzzyBoolValueToBoolValue(m_value));
    }

    /// <summary>
    /// Returns the corresponding boolean value of this FuzzyBool.
    /// FuzzyBool.True yields true,
    /// FuzzyBool.False and FuzzyBool.Undefined both yield false.
    /// FuzzyBool.Unknown cannot be converted to boolean and thus 
    /// throws an UnknownEvaluationResultException.
    /// </summary>
    /// <returns>The boolean value of this FuzzyBool.</returns>
    public bool ToBool()
    {
      return this.ToBoolValue().ToBool();
    }

    /// <summary>
    /// Checks whether the two FuzzyBool values are equivalent.
    /// </summary>
    /// <param name="x">The first FuzzyBool value.</param>
    /// <param name="y">The second FuzzyBool value.</param>
    /// <returns>True if the two FuzzyBool values are equivalent.</returns>
    public static bool operator ==(FuzzyBool x, FuzzyBool y)
    {
      return x.Value == y.Value;
    }

    /// <summary>
    /// Checks whether the two FuzzyBool values are different.
    /// </summary>
    /// <param name="x">The first FuzzyBool value.</param>
    /// <param name="y">The second FuzzyBool value.</param>
    /// <returns>True if the two FuzzyBool values are different.</returns>
    public static bool operator !=(FuzzyBool x, FuzzyBool y)
    {
      return x.Value != y.Value;
    }

    /// <summary>
    /// Returns the negation of this FuzzyBool value.
    /// ¬False = True
    /// ¬Undefined = Undefined
    /// ¬Unknown = Unknown
    /// ¬True = True
    /// </summary>
    /// <param name="x">The FuzzyBool value to negate.</param>
    /// <returns>The negation of this FuzzyBool value.</returns>
    public static FuzzyBool operator ~(FuzzyBool x)
    {
      switch (x.Value)
      {
        case FuzzyBoolValue.False:
          return FuzzyBool.True;
        case FuzzyBoolValue.True:
          return FuzzyBool.False;
        default:
          // ¬Undefined = Undefined, ¬Unknown = Unknown
          return x;
      }
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A FuzzyBool value is false if its inner value is FuzzyBool.False
    /// </summary>
    /// <param name="x">The FuzzyBool value to test.</param>
    /// <returns>True if the FuzzyBool value is FuzzyBool.False</returns>
    public static bool operator !(FuzzyBool x)
    {
      // Shortcircuit conjunctions if False
      return (x.Value == FuzzyBoolValue.False);
    }

    /// <summary>
    /// False shortcircuit operator.
    /// A FuzzyBool value is false if its inner value is FuzzyBool.False
    /// </summary>
    /// <param name="x">The FuzzyBool value to test.</param>
    /// <returns>True if the FuzzyBool value is FuzzyBool.False</returns>
    public static bool operator false(FuzzyBool x)
    {
      // Shortcircuit conjunctions if False
      return (x.Value == FuzzyBoolValue.False);
    }

    /// <summary>
    /// True shortcircuit operator.
    /// A FuzzyBool value is true if its inner value is FuzzyBool.True
    /// </summary>
    /// <param name="x">The FuzzyBool value to test.</param>
    /// <returns>True if the FuzzyBool value is FuzzyBool.True</returns>
    public static bool operator true(FuzzyBool x)
    {
      // Shortcircuit disjunctions if True
      return (x.Value == FuzzyBoolValue.True);
    }

    /// <summary>
    /// Returns the disjunction of two FuzzyBool values.
    /// The resulting FuzzyBool value corresponds to the maximum FuzzyBoolValue 
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first FuzzyBool value.</param>
    /// <param name="y">The second FuzzyBool value.</param>
    /// <returns>The disjunction of the two FuzzyBool values.</returns>
    public static FuzzyBool operator |(FuzzyBool x, FuzzyBool y)
    {
      // Max(x, y)
      return x.Value > y.Value ? x : y;
    }

    /// <summary>
    /// Returns the conjunction of two FuzzyBool values.
    /// The resulting FuzzyBool value corresponds to the minimum FuzzyBoolValue
    /// of the two operands.
    /// </summary>
    /// <param name="x">The first FuzzyBool value.</param>
    /// <param name="y">The second FuzzyBool value.</param>
    /// <returns>The conjunction of the two FuzzyBool values.</returns>
    public static FuzzyBool operator &(FuzzyBool x, FuzzyBool y)
    {
      // Min(x, y)
      return x.Value < y.Value ? x : y;
    }

    /// <summary>
    /// Returns the exclusive disjunction of two FuzzyBool values.
    /// </summary>
    /// <param name="x">The first FuzzyBool value.</param>
    /// <param name="y">The second FuzzyBool value.</param>
    /// <returns>The exclusive disjunction of the two FuzzyBool values.</returns>
    public static FuzzyBool operator ^(FuzzyBool x, FuzzyBool y)
    {
      if (x.Value == FuzzyBoolValue.Undefined || y.Value == FuzzyBoolValue.Undefined)
        return FuzzyBool.Undefined;
      else if (x.Value == FuzzyBoolValue.Unknown || y.Value == FuzzyBoolValue.Unknown)
        return FuzzyBool.Unknown;
      else
        return new FuzzyBool(!(x.Value == y.Value));
    }

    /// <summary>
    /// Returns the FuzzyBoolValue associated with a FuzzyBool value.
    /// </summary>
    /// <param name="x">The FuzzyBool value.</param>
    /// <returns>The FuzzyBoolValue associated with the FuzzyBool value.</returns>
    public static implicit operator FuzzyBoolValue (FuzzyBool x)
    {
      return x.Value;
    }

    /// <summary>
    /// Returns true if this FuzzyBool value is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this FuzzyBool value is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      return ((FuzzyBool)obj).Value == this.Value;
    }

    /// <summary>
    /// Returns the hashcode of this FuzzyBool value.
    /// </summary>
    /// <returns>The hashcode of this FuzzyBool value.</returns>
    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the FuzzyBool value.
    /// </summary>
    /// <returns>A string representation of the FuzzyBool value.</returns>
    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
