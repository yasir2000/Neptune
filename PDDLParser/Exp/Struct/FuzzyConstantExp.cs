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
using PDDLParser.Exp.Term;

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// A FuzzyConstantExp represents a constant which may be undefined or unknown.
  /// </summary>
  public struct FuzzyConstantExp
  {
    /// <summary>
    /// FuzzyConstantExp unknown value.
    /// </summary>
    public static FuzzyConstantExp Unknown = new FuzzyConstantExp(State.Unknown);

    /// <summary>
    /// FuzzyConstantExp undefined value.
    /// </summary>
    public static FuzzyConstantExp Undefined = new FuzzyConstantExp(State.Undefined);

    /// <summary>
    /// The internal status of this FuzzyConstantExp, which indicates whether it is defined,
    /// unknown, or undefined.
    /// </summary>
    public enum State : byte
    {
      /// <summary>
      /// Indicates that this FuzzyConstantExp is defined, and thus its internal constant
      /// value can be accessed.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that this FuzzyConstantExp is unknown.
      /// </summary>
      Unknown = 1,
      /// <summary>
      /// Indicates that this FuzzyConstantExp is undefined.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The constant value of this FuzzyConstantExp.
    /// </summary>
    private Constant m_value;
    /// <summary>
    /// The status of this FuzzyConstantExp.
    /// </summary>
    private State m_status;

    /// <summary>
    /// Creates a new FuzzyConstantExp with the specified constant object.
    /// </summary>
    /// <param name="value">The new FuzzyConstantExp's constant object.</param>
    public FuzzyConstantExp(Constant value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new FuzzyConstantExp with the specified constant value.
    /// </summary>
    /// <param name="value">The new FuzzyConstantExp's constant value.</param>
    public FuzzyConstantExp(ConstantExp value)
    {
      this.m_value = value.m_value;
      this.m_status = (State)value.m_status;
    }

    /// <summary>
    /// Creates a new FuzzyConstantExp with the specified status.
    /// </summary>
    /// <param name="status">The new FuzzyConstantExp's status.</param>
    public FuzzyConstantExp(State status)
    {
      this.m_value = null;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the ConstantExp value corresponding to the FuzzyConstantExp value.
    /// </summary>
    /// <param name="value">A FuzzyConstantExp value.</param>
    /// <returns>The ConstantExp value corresponding to the FuzzyConstantExp value.</returns>
    public static ConstantExp FuzzyConstantExpToConstantExp(FuzzyConstantExp value)
    {
      if (value.m_status == FuzzyConstantExp.State.Unknown)
        throw new System.Exception("Cannot convert Unknown FuzzyConstantExp to ConstantExp!");
      else
        return new ConstantExp((ConstantExp.State)value.m_status, value.m_value);
    }

    /// <summary>
    /// Returns the status of this FuzzyConstantExp.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the inner constant value of this FuzzyConstantExp.
    /// The value should only be accessed if this FuzzyConstantExp is defined.
    /// </summary>
    public Constant Value
    {
      get 
      {
        //switch (m_status)
        //{
        //  case State.Undefined:
        //    throw new UndefinedExpException("Value is undefined!");
        //  case State.Unknown:
        //    throw new UnknownExpException("Value is unknown!");
        //  default:
        //    return this.m_value;
        //}

        System.Diagnostics.Debug.Assert(m_status == State.Defined);

        return this.m_value;
      }
    }

    /// <summary>
    /// Returns the corresponding ConstantExp value of this FuzzyConstantExp.
    /// Note that FuzzyConstantExp.Unknown cannot be converted to ConstantExp and thus throws 
    /// an UnknownEvaluationResultException.
    /// </summary>
    /// <returns>The corresponding ConstantExp value of this FuzzyConstantExp.</returns>
    public ConstantExp ToConstantValue()
    {
      return FuzzyConstantExpToConstantExp(this);
    }

    /// <summary>
    /// Returns whether two fuzzy constant values are equal. 
    /// If one of them is undefined, this function returns undefined.
    /// Else if one of them is unknown, this function returns unknown.
    /// </summary>
    /// <param name="x">The first fuzzy constant value.</param>
    /// <param name="y">The second fuzzy constant value.</param>
    /// <returns>Whether two fuzzy constant values are equal.</returns>
    public static FuzzyBool ConstantEquals(FuzzyConstantExp x, FuzzyConstantExp y)
    {
      State max = (State) Math.Max((byte)x.Status, (byte)y.Status);
      switch (max)
      {
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(x.Value.Equals(y.Value));
      }
    }

    ///// <summary>
    ///// Checks whether a given FuzzyConstantExp is equal to another given FuzzyConstantExp.
    ///// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    ///// </summary>
    ///// <param name="d1">The first operand.</param>
    ///// <param name="d2">The second operand.</param>
    ///// <returns>Bool.Undefined if at least one of two operands is undefined, else
    ///// Bool.True if the first operand is equal to the second operand.</returns>
    //public static bool operator ==(FuzzyConstantExp x, FuzzyConstantExp y)
    //{
    //  if (x.status == y.status)
    //  {
    //    if (x.status == ConstantValueStatus.Defined)
    //    {
    //      return x.value.Equals(y.value);
    //    }
    //    else
    //    {
    //      return true;
    //    }
    //  }
    //  else
    //  {
    //    return false;
    //  }
    //}

    ///// <summary>
    ///// Checks whether a given FuzzyConstantExp is different from another given FuzzyConstantExp.
    ///// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    ///// </summary>
    ///// <param name="d1">The first operand.</param>
    ///// <param name="d2">The second operand.</param>
    ///// <returns>Bool.Undefined if at least one of two operands is undefined, else
    ///// Bool.True if the first operand is different from the second operand.</returns>
    //public static bool operator !=(FuzzyConstantExp x, FuzzyConstantExp y)
    //{
    //  return !(x == y);
    //}

    /// <summary>
    /// Returns true if this FuzzyConstantExp is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this FuzzyConstantExp is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      FuzzyConstantExp other = (FuzzyConstantExp)obj;
      if (this.m_status == State.Defined)
      {
        return this.m_status == other.m_status &&
               this.m_value == other.m_value;
      }
      else
      {
        return this.m_status == other.m_status;
      }
    }

    /// <summary>
    /// Returns the hashcode of this FuzzyConstantExp.
    /// </summary>
    /// <returns>The hashcode of this FuzzyConstantExp.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ? 
              this.m_value.GetHashCode() : this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this FuzzyConstantExp.
    /// </summary>
    /// <returns>A string representation of this FuzzyConstantExp.</returns>
    public override string ToString()
    {
      switch (this.m_status)
      {
        case State.Unknown:
          return "Unknown";
        case State.Undefined:
          return "Undefined";
        default:
          return this.m_value.ToString();
      }
    }
  }
}
