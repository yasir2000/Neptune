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
  /// A FuzzyDouble represents a double value which may be undefined or unknown.
  /// </summary>
  public struct FuzzyDouble
  {
    /// <summary>
    /// FuzzyDouble unknown value.
    /// </summary>
    public static FuzzyDouble Unknown = new FuzzyDouble(State.Unknown, 0.0);

    /// <summary>
    /// FuzzyDouble undefined value.
    /// </summary>
    public static FuzzyDouble Undefined = new FuzzyDouble(State.Undefined, 0.0);

    /// <summary>
    /// The internal status of this FuzzyDouble, which indicates whether it is defined,
    /// unknown, or undefined.
    /// </summary>
    public enum State : byte
    {
      /// <summary>
      /// Indicates that this FuzzyDouble is defined, and thus its internal double value
      /// can be accessed.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that this FuzzyDouble is unknown.
      /// </summary>
      Unknown = 1,
      /// <summary>
      /// Indicates that this FuzzyDouble is undefined.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The value of this FuzzyDouble.
    /// </summary>
    internal double m_value;
    /// <summary>
    /// Indicates the status of this Double.
    /// </summary>
    internal State m_status;

    /// <summary>
    /// Creates a new FuzzyDouble with the specified double value.
    /// </summary>
    /// <param name="value">The new FuzzyDouble's value.</param>
    public FuzzyDouble(double value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new FuzzyDouble with the specified Double value.
    /// </summary>
    /// <param name="value">The new FuzzyDouble's value.</param>
    public FuzzyDouble(Double value)
    {
      this.m_value = value.m_value;
      this.m_status = (State)value.m_status;
    }

    /// <summary>
    /// Creates a new FuzzyDouble with the specified status and double value.
    /// </summary>
    /// <param name="status">The new Double's status.</param>
    /// <param name="value">The new Double's value.</param>
    internal FuzzyDouble(State status, double value)
    {
      this.m_value = value;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the Double value corresponding to the specified FuzzyDouble value.
    /// </summary>
    /// <param name="value">A FuzzyDouble value.</param>
    /// <returns>The Double value corresponding to the specified FuzzyDouble value.</returns>
    public static Double FuzzyDoubleToDouble(FuzzyDouble value)
    {
      if (value.m_status == State.Unknown)
        throw new System.Exception("Cannot convert Unknown FuzzyDouble to Double!");
      return new Double((Double.State)value.m_status, value.m_value);
    }

    /// <summary>
    /// Returns the status of this FuzzyDouble.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the inner double value of this FuzzyDouble.
    /// The value should only be accessed if this FuzzyDouble is defined.
    /// </summary>
    public double Value
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
    /// Returns the corresponding Double value of this FuzzyDouble.
    /// Note that FuzzyDouble.Unknown cannot be converted to Double and thus throws 
    /// an UnknownEvaluationResultException.
    /// </summary>
    /// <returns>The corresponding Double value of this FuzzyDouble.</returns>
    public Double ToDoubleValue()
    {
      return FuzzyDoubleToDouble(this);
    }

    /// <summary>
    /// Checks whether a given FuzzyDouble is strictly less than another given FuzzyDouble.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is strictly less than the second operand.</returns>
    public static FuzzyBool operator <(FuzzyDouble d1, FuzzyDouble d2)
    {
      State max = (State)Math.Max((byte)d1.m_status, (byte)d2.m_status);
      switch (max)
      {
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(d1.m_value < d2.m_value);
      }
    }

    /// <summary>
    /// Checks whether a given FuzzyDouble is less than or equal to another given FuzzyDouble.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is less than or equal to the second operand.</returns>
    public static FuzzyBool operator <=(FuzzyDouble d1, FuzzyDouble d2)
    {
      State max = (State)Math.Max((byte)d1.m_status, (byte)d2.m_status);
      switch (max)
      {
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(d1.m_value <= d2.m_value);
      }
    }

    /// <summary>
    /// Checks whether a given FuzzyDouble is strictly greater than another given FuzzyDouble.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is strictly greater than the second operand.</returns>
    public static FuzzyBool operator >(FuzzyDouble d1, FuzzyDouble d2)
    {
      State max = (State)Math.Max((byte)d1.m_status, (byte)d2.m_status);
      switch (max)
      {
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(d1.m_value > d2.m_value);
      }
    }

    /// <summary>
    /// Checks whether a given FuzzyDouble is greater than or equal to another given FuzzyDouble.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is greater than or equal to the second operand.</returns>
    public static FuzzyBool operator >=(FuzzyDouble d1, FuzzyDouble d2)
    {
      State max = (State)Math.Max((byte)d1.m_status, (byte)d2.m_status);
      switch (max)
      {         
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(d1.m_value >= d2.m_value);
      }
    }

    /// <summary>
    /// Checks whether a given FuzzyDouble is equal to another given FuzzyDouble.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is equal to the second operand.</returns>
    public static FuzzyBool operator ==(FuzzyDouble d1, FuzzyDouble d2)
    {
      State max = (State)Math.Max((byte)d1.m_status, (byte)d2.m_status);
      switch (max)
      {         
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(d1.m_value == d2.m_value);
      }
    }

    /// <summary>
    /// Checks whether a given FuzzyDouble is different from another given FuzzyDouble.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is different from the second operand.</returns>
    public static FuzzyBool operator !=(FuzzyDouble d1, FuzzyDouble d2)
    {
      State max = (State)Math.Max((byte)d1.m_status, (byte)d2.m_status);
      switch (max)
      {
        case State.Unknown:
          return FuzzyBool.Unknown;
        case State.Undefined:
          return FuzzyBool.Undefined;
        default:
          return new FuzzyBool(d1.m_value != d2.m_value);
      }
    }

    /// <summary>
    /// Performs the negation of a given FuzzyDouble.
    /// This operation returns an undefined value if the given FuzzyDouble is undefined.
    /// </summary>
    /// <param name="d">The value to negate.</param>
    /// <returns>Double.Undefined if the operand is undefined, or else the negated value.</returns>
    public static FuzzyDouble operator -(FuzzyDouble d)
    {
      if (d.m_status == State.Defined)
      {
        return new FuzzyDouble(-d.m_value);
      }
      else
      {
        return d;
      }
    }

    //public static Double operator -(FuzzyDouble d1, FuzzyDouble d2)
    //{
    //  return (d1.status == Status.Undefined || d2.status == Status.Undefined) ? Double.Undefined :
    //                                          new Double(d1.value - d2.value);
    //}

    /// <summary>
    /// Adds two <see cref="FuzzyDouble"/> instances together.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns><see cref="FuzzyDouble.Undefined"/> if one operand is undefined, <see cref="FuzzyDouble.Unknown"/> if
    /// one operand is unknown, or a new <see cref="FuzzyDouble"/> whose value is the sum of the operands' values.</returns>
    public static FuzzyDouble operator +(FuzzyDouble d1, FuzzyDouble d2)
    {
      if (d1.m_status == State.Undefined || d2.m_status == State.Undefined)
        return FuzzyDouble.Undefined;
      
      if (d1.m_status == State.Unknown   || d2.m_status == State.Unknown)
        return FuzzyDouble.Unknown;

      return new FuzzyDouble(d1.m_value + d2.m_value);
    }

    //public static Double operator *(FuzzyDouble d1, FuzzyDouble d2)
    //{
    //  return (d1.status == Status.Undefined || d2.status == Status.Undefined) ? Double.Undefined :
    //                                          new Double(d1.value * d2.value);
    //}

    //public static Double operator /(FuzzyDouble d1, FuzzyDouble d2)
    //{
    //  return (d1.status == Status.Undefined || d2.status == Status.Undefined) ? Double.Undefined :
    //                                          new Double(d1.value / d2.value);
    //}

    /// <summary>
    /// Returns true if this FuzzyDouble is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this FuzzyDouble is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      FuzzyDouble other = (FuzzyDouble)obj;
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
    /// Returns the hashcode of this FuzzyDouble.
    /// </summary>
    /// <returns>The hashcode of this FuzzyDouble.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ? 
              this.m_value.GetHashCode() : this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the FuzzyDouble value.
    /// </summary>
    /// <returns>A string representation of the FuzzyDouble value.</returns>
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
          throw new System.Exception("Invalid FuzzyDoubleStatus: " + this.m_status);
      }
    }
  }
}
