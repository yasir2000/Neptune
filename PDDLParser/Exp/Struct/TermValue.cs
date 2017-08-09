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
  /// A TermValue represents a term (possibly constant) or an undefined value.
  /// Note that a TermValue behaves exactly like a FuzzyConstantExp; the only difference is 
  /// that a TermValue stores the actual unknown expression instead of the 
  /// FuzzyConstantExp.Unknown value.
  /// </summary>
  public struct TermValue
  {
    /// <summary>
    /// TermValue undefined value.
    /// </summary>
    public static TermValue Undefined = new TermValue(State.Undefined, null);

    /// <summary>
    /// Internal status of this TermValue which indicates whether it is defined.
    /// </summary>
    public enum State : byte
    {
      /// <summary>
      /// Indicates that this TermValue is defined, and thus its internal term value
      /// can be accessed.
      /// </summary>
      Defined = 0,
      /// <summary>
      /// Indicates that this TermValue is undefined.
      /// </summary>
      Undefined = 2
    }

    /// <summary>
    /// The inner term stored inside this TermValue.
    /// </summary>
    internal ITerm m_value;
    /// <summary>
    /// The status of this TermValue.
    /// </summary>
    internal State m_status;

    /// <summary>
    /// Creates a new TermValue with the specified term.
    /// </summary>
    /// <param name="value">The new TermValue's term.</param>
    public TermValue(ITerm value)
    {
      this.m_value = value;
      this.m_status = State.Defined;
    }

    /// <summary>
    /// Creates a new TermValue with the specified constant value.
    /// </summary>
    /// <param name="value">The new TermValue's constant value.</param>
    public TermValue(ConstantExp value)
    {
      if (value.Status == ConstantExp.State.Defined)
      {
        this.m_value = value.m_value;
        this.m_status = State.Defined;
      }
      else
      {
        this.m_value = null;
        this.m_status = State.Undefined;
      }
    }

    /// <summary>
    /// Creates a new TermValue with the specified status and term.
    /// </summary>
    /// <param name="status">The new TermValue's status.</param>
    /// <param name="value">The new TermValue's term.</param>
    internal TermValue(State status, ITerm value)
    {
      this.m_value = value;
      this.m_status = status;
    }

    /// <summary>
    /// Returns the status of this TermValue.
    /// </summary>
    public State Status
    {
      get { return this.m_status; }
    }

    /// <summary>
    /// Returns the inner term stored inside this TermValue.
    /// The value should only be accessed if this Double is defined.
    /// </summary>
    public ITerm Value
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
    /// Returns true if this TermValue is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this TermValue is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      TermValue other = (TermValue)obj;
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
    /// Returns the hashcode of this TermValue.
    /// </summary>
    /// <returns>The hashcode of this TermValue.</returns>
    public override int GetHashCode()
    {
      return (this.m_status == State.Defined) ? 
              this.m_value.GetHashCode() : this.m_status.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this TermValue.
    /// </summary>
    /// <returns>A string representation of this TermValue.</returns>
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
  }
}
