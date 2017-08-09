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
  /// A ConstantExp represents a constant value which may also be undefined.
  /// </summary>
  public struct ConstantExp : IComparable<ConstantExp>
  {
    /// <summary>
    /// The undefined ConstantExp.
    /// </summary>
    public static ConstantExp Undefined = new ConstantExp(State.Undefined, null);

    /// <summary>
    /// Internal ConstantExp status, which indicates whether the ConstantExp 
    /// is defined or not.
    /// </summary>
    public enum State : byte
    {
      /// <summary>
      /// Indicates that this ConstantExp is defined, and thus its internal Constant
      /// value can be accessed.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that this ConstantExp is undefined.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The constant object of this ConstantExp.
    /// </summary>
    internal Constant m_value;
    /// <summary>
    /// The status of this ConstantExp.
    /// </summary>
    internal State m_status;

    /// <summary>
    /// Creates a new ConstantExp with the specified constant object.
    /// </summary>
    /// <param name="value">The new ConstantExp's constant object.</param>
    public ConstantExp(Constant value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new ConstantExp with the specified status and constant object.
    /// </summary>
    /// <param name="status">The new ConstantExp's status.</param>
    /// <param name="value">The new ConstantExp's constant object.</param>
    internal ConstantExp(State status, Constant value)
    {
      this.m_value = value;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the status of this ConstantExp.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the inner Constant value of this ConstantExp.
    /// The value should only be accessed if this ConstantExp is defined.
    /// </summary>
    public Constant Value
    {
      get
      {
        //if (m_status != State.Defined)
        //  throw new UndefinedExpException("Value is undefined!");

        System.Diagnostics.Debug.Assert(m_status == State.Defined);

        return this.m_value;
      }
    }

    /// <summary>
    /// Returns whether two constant values are equal. If one of them is undefined, this
    /// function returns undefined.
    /// </summary>
    /// <param name="x">The first constant value.</param>
    /// <param name="y">The second constant value.</param>
    /// <returns>Bool.Undefined if at least one of two operands is undefined, else
    /// Bool.True if the first operand is equal to the second operand.</returns>
    public static Bool ConstantEquals(ConstantExp x, ConstantExp y)
    {
      if (x.Status == State.Undefined ||
          y.Status == State.Undefined)
      {
        return Bool.Undefined;
      }
      else
      {
        return new Bool(x.Value.Equals(y.Value));
      }
    }

    ///// <summary>
    ///// Returns whether two constant values are equal. If one of them is undefined, this
    ///// function returns undefined.
    ///// </summary>
    ///// <param name="d1">The first operand.</param>
    ///// <param name="d2">The second operand.</param>
    ///// <returns>Bool.Undefined if at least one of two operands is undefined, else
    ///// Bool.True if the first operand is equal to the second operand.</returns>
    //public static bool operator ==(ConstantExp x, ConstantExp y)
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
    ///// Returns whether two constant values are different. If one of them is undefined, this
    ///// function returns undefined.
    ///// </summary>
    ///// <param name="d1">The first operand.</param>
    ///// <param name="d2">The second operand.</param>
    ///// <returns>Bool.Undefined if at least one of two operands is undefined, else
    ///// Bool.True if the first operand is different from the second operand.</returns>
    //public static bool operator !=(ConstantExp x, ConstantExp y)
    //{
    //  return !(x == y);
    //}

    /// <summary>
    /// Returns true if this ConstantExp is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this ConstantExp is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      ConstantExp other = (ConstantExp)obj;
      if (this.m_status == State.Defined)
      {
        return this.m_status == other.m_status &&
               this.m_value.Equals(other.m_value);
      }
      else
      {
        return this.m_status == other.m_status;
      }
    }

    /// <summary>
    /// Returns the hashcode of this ConstantExp.
    /// </summary>
    /// <returns>The hashcode of this ConstantExp.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ? 
              this.m_value.GetHashCode() : this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this ConstantExp.
    /// </summary>
    /// <returns>A string representation of this ConstantExp.</returns>
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

    /// <summary>
    /// Returns a typed string representation of this ConstantExp.
    /// </summary>
    /// <returns>A typed string representation of this ConstantExp.</returns>
    public string ToTypedString()
    {
      if (this.m_status == State.Defined)
      {
        return this.m_value.ToTypedString();
      }
      else
      {
        return "Undefined";
      }
    }

    #region IComparable<ConstantExp> Members

    /// <summary>
    /// Compares this ConstantExp with another ConstantExp.
    /// </summary>
    /// <param name="other">The other ConstantExp to compare this ConstantExp to.
    /// </param>
    /// <returns>An integer representing the total order relation between the two ConstantExps.
    /// </returns>
    public int CompareTo(ConstantExp other)
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
