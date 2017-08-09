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
  /// A Double represents a double value which may be undefined.
  /// Note that all arithmetic comparisons performed on an undefined Double value
  /// always yield an undefined (Bool.Undefined) result.
  /// </summary>
  public struct Double : IComparable<Double>
  {
    /// <summary>
    /// Double undefined value.
    /// </summary>
    public static Double Undefined = new Double(State.Undefined, 0.0);

    /// <summary>
    /// Internal Double status, which indicates whether the Double is defined or not.
    /// </summary>
    public enum State : byte
    {
      /// <summary>
      /// Indicates that this Double is defined, and thus its internal double value
      /// can be accessed.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that this Double is undefined.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The value of this Double.
    /// </summary>
    internal double m_value;
    /// <summary>
    /// The status of this Double.
    /// </summary>
    internal State m_status;

    /// <summary>
    /// Creates a new Double with the specified double value.
    /// </summary>
    /// <param name="value">The new Double's value.</param>
    public Double(double value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new Double with the specified status and double value.
    /// </summary>
    /// <param name="status">The new Double's status.</param>
    /// <param name="value">The new Double's value.</param>
    internal Double(State status, double value)
    {
      this.m_value = value;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the status of this Double.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the value of this Double.
    /// The value should only be accessed if this Double is defined.
    /// </summary>
    public double Value
    {
      get 
      {
        //if (m_status != DoubleStatus.Defined)
        //  throw new UndefinedExpException("Value is undefined!");

        System.Diagnostics.Debug.Assert(m_status == State.Defined);

        return this.m_value;
      }
    }

    /// <summary>
    /// Checks whether a given Double is strictly less than another given Double.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is strictly less than the second operand.</returns>
    public static Bool operator <(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ? 
              Bool.Undefined : new Bool(d1.m_value < d2.m_value);
    }

    /// <summary>
    /// Checks whether a given Double is less than or equal to another given Double.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is less than or equal to the second operand.</returns>
    public static Bool operator <=(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ?
              Bool.Undefined : new Bool(d1.m_value <= d2.m_value);
    }

    /// <summary>
    /// Checks whether a given Double is strictly greater than another given Double.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is strictly greater than the second operand.</returns>
    public static Bool operator >(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ? 
              Bool.Undefined : new Bool(d1.m_value > d2.m_value);
    }

    /// <summary>
    /// Checks whether a given Double is greater than or equal to another given Double.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is greater than or equal to the second operand.</returns>
    public static Bool operator >=(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ? 
              Bool.Undefined : new Bool(d1.m_value >= d2.m_value);
    }

    /// <summary>
    /// Checks whether a given Double is equal to another given Double.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is equal to the second operand.</returns>
    public static Bool operator ==(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ? 
              Bool.Undefined : new Bool(d1.m_value == d2.m_value);
    }

    /// <summary>
    /// Checks whether a given Double is different from another given Double.
    /// This comparison returns Bool.Undefined if at least one of two operands is undefined.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is different from the second operand.</returns>
    public static Bool operator !=(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ? 
              Bool.Undefined : new Bool(d1.m_value != d2.m_value);
    }

    /// <summary>
    /// Performs the negation of a given double.
    /// This operation returns an undefined value if the given Double is undefined.
    /// </summary>
    /// <param name="d">The value to negate.</param>
    /// <returns>Double.Undefined if the operand is undefined, or else the negated value.</returns>
    public static Double operator -(Double d)
    {
      return (d.m_status == State.Undefined ? Double.Undefined : new Double(-d.m_value));
    }

    //public static Double operator -(Double d1, Double d2)
    //{
    //  return (d1.status == DoubleStatus.Undefined || d2.status == DoubleStatus.Undefined) ? Double.Undefined :
    //                                          new Double(d1.value - d2.value);
    //}

    /// <summary>
    /// Adds two <see cref="PDDLParser.Exp.Struct.Double"/> instances together.
    /// </summary>
    /// <param name="d1">The first operand.</param>
    /// <param name="d2">The second operand.</param>
    /// <returns><see cref="PDDLParser.Exp.Struct.Double.Undefined"/> if one operand is undefined, or a new <see cref="PDDLParser.Exp.Struct.Double"/>
    /// whose value is the sum of the operands' values.</returns>
    public static Double operator +(Double d1, Double d2)
    {
      return (d1.m_status == State.Undefined || d2.m_status == State.Undefined) ? Double.Undefined :
                                              new Double(d1.m_value + d2.m_value);
    }

    //public static Double operator *(Double d1, Double d2)
    //{
    //  return (d1.status == DoubleStatus.Undefined || d2.status == DoubleStatus.Undefined) ? Double.Undefined :
    //                                          new Double(d1.value * d2.value);
    //}

    //public static Double operator /(Double d1, Double d2)
    //{
    //  return (d1.status == DoubleStatus.Undefined || d2.status == DoubleStatus.Undefined) ? Double.Undefined :
    //                                          new Double(d1.value / d2.value);
    //}

    /// <summary>
    /// Returns true if this Double is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this Double is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      Double other = (Double)obj;
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
    /// Returns the hashcode of this Double.
    /// </summary>
    /// <returns>The hashcode of this Double.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ? this.m_value.GetHashCode() : 
                                                     this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this Double.
    /// </summary>
    /// <returns>A string representation of this Double.</returns>
    public override string ToString()
    {
      if (this.m_status == State.Defined)
      {
        return this.m_value.ToString();
      }
      else
      {
        return "Undefined";
      }
    }

    #region IComparable<Double> Members

    /// <summary>
    /// Compares this Double with another Double.
    /// </summary>
    /// <param name="other">The other Double to compare this Double to.
    /// </param>
    /// <returns>An integer representing the total order relation between the two Doubles.
    /// </returns>
    public int CompareTo(Double other)
    {
      int value = this.m_status.CompareTo(other.m_status);
      if (value != 0)
        return value;
      else
        return this.m_value.CompareTo(other.m_value);
    }

    #endregion
  }
}
