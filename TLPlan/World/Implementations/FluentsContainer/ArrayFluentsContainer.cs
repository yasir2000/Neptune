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
using PDDLParser;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using TLPlan.Utils;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// An array fluents container holds fluents in a fixed array.
  /// Since each fluent application is assigned a unique fluent ID, it is possible to
  /// store all possible fluents in a lookup table (indexed by their ID). This approach 
  /// may be more memory-expensive than other methods but it allows for constant-time 
  /// retrieval of fluents' value.
  /// </summary>
  public class ArrayFluentsContainer : FluentsContainer
  {
    #region Private Fields

    /// <summary>
    /// The fixed lookup table of numeric fluents.
    /// </summary>
    private Double[] m_numericFluents;

    /// <summary>
    /// The offset (in a lookup table) corresponding to the numeric fluent at index 0 in 
    /// the numeric fluents array.
    /// </summary>
    int m_numericOffset;

    /// <summary>
    /// The fixed lookup table of object fluents.
    /// </summary>
    private ConstantExp[] m_objectFluents;

    /// <summary>
    /// The offset (in a lookup table) corresponding to the object fluent at index 0 in 
    /// the object fluents array.
    /// </summary>
    int m_objectOffset;

    /// <summary>
    /// The hash code of this fluents container.
    /// </summary>
    private int m_hashCode;

    #endregion

    #region Properties

    /// <summary>
    /// The interval of numeric fluents ID whose value are stored in this fluents container.
    /// </summary>
    public IntegerInterval NumericFluentDefinitionInterval
    {
      get
      {
        return new IntegerInterval(m_numericOffset, m_numericFluents.Length);
      }
    }

    /// <summary>
    /// The interval of object fluents ID whose value are stored in this fluents container.
    /// </summary>
    public IntegerInterval ObjectFluentDefinitionInterval
    {
      get
      {
        return new IntegerInterval(m_objectOffset, m_objectFluents.Length);
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new array fluents container of the specified length.
    /// </summary>
    /// <param name="numericInterval">The interval of numeric fluents ID whose value are stored 
    /// in this new fluents container.</param>
    /// <param name="objectInterval">The interval of object fluents ID whose value are stored 
    /// in this new fluents container.</param>
    public ArrayFluentsContainer(IntegerInterval numericInterval, IntegerInterval objectInterval)
    {
      this.m_numericOffset = numericInterval.Start;
      this.m_objectOffset = objectInterval.Start;
      this.m_numericFluents = Enumerable.Repeat(Double.Undefined, numericInterval.Length).ToArray();
      this.m_objectFluents = Enumerable.Repeat(ConstantExp.Undefined, objectInterval.Length).ToArray();
      this.m_hashCode = 0;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies this fluent container.
    /// </summary>
    /// <returns>A copy of this fluent container.</returns>
    public override FluentsContainer Copy()
    {
      ArrayFluentsContainer world = (ArrayFluentsContainer)base.MemberwiseClone();
      // Do not clone internals, waste of memory? Copy on write?
      world.m_numericFluents = (Double[])this.m_numericFluents.Clone();
      world.m_objectFluents = (ConstantExp[])this.m_objectFluents.Clone();
      return world;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// The fluent's value is retrieved from the lookup table.
    /// </summary>
    /// <param name="fluentID">A numeric fluent ID.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public override FuzzyDouble GetNumericFluent(int fluentID)
    {
      int index = fluentID - m_numericOffset;
      if (index >= 0 && index < m_numericFluents.Length)
        return new FuzzyDouble(m_numericFluents[index]);
      else
        return FuzzyDouble.Unknown;
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// The fluent's value is retrieved from the lookup table.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public override FuzzyConstantExp GetObjectFluent(int fluentID)
    {
      int index = fluentID - m_objectOffset;
      if (index >= 0 && index < m_objectFluents.Length)
        return new FuzzyConstantExp(m_objectFluents[index]);
      else
        return FuzzyConstantExp.Unknown;
    }

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluentID">A numeric fluent ID.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public override void SetNumericFluent(int fluentID, double value)
    {
      int pos = fluentID - m_numericOffset;
      Double oldValue = m_numericFluents[pos];
      if (oldValue.Status != Double.State.Undefined)
      {
        m_hashCode -= (Utils.General.Hash(fluentID) * oldValue.Value.GetHashCode());
      }
      m_numericFluents[pos] = new Double(value);
      m_hashCode += (Utils.General.Hash(fluentID) * value.GetHashCode());
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public override void SetObjectFluent(int fluentID, Constant value)
    {
      int pos = fluentID - m_objectOffset;
      ConstantExp oldValue = m_objectFluents[pos];
      if (oldValue.Status != ConstantExp.State.Undefined)
      {
        m_hashCode -= (Utils.General.Hash(fluentID) * oldValue.Value.GetHashCode());
      }
      m_objectFluents[pos] = new ConstantExp(value);
      m_hashCode += (Utils.General.Hash(fluentID) * value.GetHashCode());
    }

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    public override void UndefineObjectFluent(int fluentID)
    {
      int pos = fluentID - m_objectOffset;
      ConstantExp oldValue = m_objectFluents[pos];
      if (oldValue.Status != ConstantExp.State.Undefined)
      {
        m_hashCode -= (Utils.General.Hash(fluentID) * oldValue.Value.GetHashCode());
      }
      m_objectFluents[pos] = ConstantExp.Undefined;
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this fluents container is equal to another fluents container.
    /// Two array fluent containers are equal if all their respective fluents have the
    /// same value pairwise.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this fluents container is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        ArrayFluentsContainer other = (ArrayFluentsContainer)obj;

        System.Diagnostics.Debug.Assert(this.m_numericOffset == other.m_numericOffset &&
                                        this.m_objectOffset == other.m_objectOffset);

        return this.m_numericFluents.SequenceEqual(other.m_numericFluents) &&
               this.m_objectFluents.SequenceEqual(other.m_objectFluents);
      }
    }

    /// <summary>
    /// Returns the hash code of this fluents container.
    /// </summary>
    /// <returns>The hash code of this fluents container.</returns>
    public override int GetHashCode()
    {
      return m_hashCode;
    }

    #endregion

    #region IComparable<OpenWorld> interface

    /// <summary>
    /// Compares this fluents container with another fluents container.
    /// Comparison is done first on the hash code, then on the individual fluents.
    /// </summary>
    /// <param name="other">The other fluents container to compare this fluents container to.</param>
    /// <returns>An integer representing the total order relation between the two fluents containers.
    /// </returns>
    public override int CompareTo(FluentsContainer other)
    {
      int value = this.GetHashCode().CompareTo(other.GetHashCode());
      if (value != 0)
        return value;

      // We do not compare worlds of different implementations... yet.
      ArrayFluentsContainer otherCnt = (ArrayFluentsContainer)other;

      System.Diagnostics.Debug.Assert(this.m_numericOffset == otherCnt.m_numericOffset &&
                                this.m_objectOffset == otherCnt.m_objectOffset);

      for (int i = 0; i < m_numericFluents.Length; ++i)
      {
        if ((value = this.m_numericFluents[i].CompareTo(otherCnt.m_numericFluents[i])) != 0)
          return value;
      }

      for (int i = 0; i < m_objectFluents.Length; ++i)
      {
        if ((value = this.m_objectFluents[i].CompareTo(otherCnt.m_objectFluents[i])) != 0)
          return value;
      }

      return value;
    }

    #endregion
  }
}
