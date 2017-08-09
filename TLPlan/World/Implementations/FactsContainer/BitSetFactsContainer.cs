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
using PDDLParser.Extensions;
using TLPlan.Utils;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// A bitset facts container holds facts in a fixed bitset.
  /// Since each atomic formula application is assigned a unique formula ID, it is possible to
  /// store all possible facts in a bitset (indexed by their ID). This approach may be more
  /// memory-expensive than other methods but it allows for constant-time retrieval of
  /// predicates' value.
  /// </summary>
  public class BitSetFactsContainer : FactsContainer
  {
    #region Private Fields

    /// <summary>
    /// The bitset containing all facts.
    /// </summary>
    private BitSet m_facts;

    /// <summary>
    /// The hash code of this facts container.
    /// </summary>
    private int m_hashCode;

    /// <summary>
    /// The offset (in a lookup table) corresponding to the predicate at bit 0 of this bitset.
    /// </summary>
    private int m_offset;

    #endregion

    #region Properties

    /// <summary>
    /// The interval of predicates ID whose value are stored in this bitset.
    /// </summary>
    public IntegerInterval DefinitionInterval
    {
      get
      {
        return new IntegerInterval(m_offset, m_facts.Count);
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new bitset facts container of the specified length.
    /// </summary>
    /// <param name="interval">The interval of predicates ID whose value are stored
    /// in this new bitset facts container.</param>
    public BitSetFactsContainer(IntegerInterval interval)
    {
      this.m_facts = new BitSet(interval.Length);
      this.m_offset = interval.Start;
      this.m_hashCode = 0;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies this facts container.
    /// </summary>
    /// <returns>A copy of this facts container.</returns>
    public override FactsContainer Copy()
    {
      BitSetFactsContainer world = (BitSetFactsContainer)base.MemberwiseClone();
      // Do not clone internals, waste of memory? Copy on write?
      world.m_facts = m_facts.Clone();
      return world;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// The value (a bit) is retrieved from the bitset.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    /// <returns>True, false, or unknown.</returns>
    public override FuzzyBool IsSet(int formulaID)
    {
      int bit = formulaID - m_offset;
      if (bit >= 0 && bit < m_facts.Count)
        return new FuzzyBool(m_facts.Get(bit));
      else
        return FuzzyBool.Unknown;
    }

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// This function updates the appropriate bit.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    public override void Set(int formulaID)
    {
      int bit = formulaID - m_offset;
      bool oldValue = m_facts.Set(bit);
      if (!oldValue)
      {
        m_hashCode += Utils.General.Hash(formulaID);
      }
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// This function updates the appropriate bit.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    public override void Unset(int formulaID)
    {
      int bit = formulaID - m_offset;
      bool oldValue = m_facts.Clear(bit);
      if (oldValue)
      {
        m_hashCode -= Utils.General.Hash(formulaID);
      }
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this facts container is equal to another object.
    /// Two bitset facts containers are equal if they hold the same facts.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this facts container is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        BitSetFactsContainer other = (BitSetFactsContainer)obj;
        System.Diagnostics.Debug.Assert(this.m_offset == other.m_offset);

        return this.m_facts.Equals(other.m_facts);
      }
    }

    /// <summary>
    /// Returns the hash code of this facts container.
    /// </summary>
    /// <returns>The hash code of this facts container.</returns>
    public override int GetHashCode()
    {
      return m_hashCode;
    }

    #endregion

    #region IComparable<FactsContainer> interface

    /// <summary>
    /// Compares this facts container with another facts container.
    /// Comparison is done first on the hashcode, then on the internal bitsets. 
    /// </summary>
    /// <param name="other">The other facts container to compare this facts container to.</param>
    /// <returns>An integer representing the total order relation between the two facts containers.
    /// </returns>
    public override int CompareTo(FactsContainer other)
    {
      int value = this.GetHashCode().CompareTo(other.GetHashCode());
      if (value != 0)
        return value;

      // We do not compare worlds of different implementations... yet.

      BitSetFactsContainer otherCnt = (BitSetFactsContainer)other;

      System.Diagnostics.Debug.Assert(this.m_offset == otherCnt.m_offset);

      value = this.m_offset.CompareTo(otherCnt.m_offset);
      if (value != 0)
        return value;

      return this.m_facts.CompareTo(otherCnt.m_facts);
    }

    #endregion
  }
}
