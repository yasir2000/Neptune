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
  /// A hashset facts container holds facts IDs in a hashset.
  /// This implementation is fast for retrieving predicates' value, but comparison
  /// between two hashset facts container is slow because their respective
  /// elements must be sorted beforehand.
  /// Note that as is, a hashset facts container cannot determine whether a predicate ID not
  /// contained in the hashset is actually "false" or "unknown", and hence always returns 
  /// unknown.
  /// </summary>
  public class HashSetFactsContainer : FactsContainer
  {
    #region Private Fields

    /// <summary>
    /// The hashset of facts IDs.
    /// </summary>
    private HashSet<int> m_facts;

    /// <summary>
    /// The cached list of sorted facts IDs, necessary only for comparisons 
    /// with other containers.
    /// </summary>
    private List<int> m_sortedFacts;

    /// <summary>
    /// The hash code of this facts container.
    /// </summary>
    private int m_hashCode;

    #endregion

    #region Properties

    /// <summary>
    /// The list of sorted facts IDs, necessary only for comparisons with other containers.
    /// </summary>
    protected List<int> SortedFacts
    {
      get
      {
        if (m_sortedFacts == null)
        {
          m_sortedFacts = new List<int>(m_facts);
          m_sortedFacts.Sort();
        }
        return m_sortedFacts;
      }
    }

   #endregion

    #region Constructors

    /// <summary>
    /// Creates a new empty hashset facts container.
    /// </summary>
    /// <param name="interval">The interval of predicates ID whose value are stored
    /// in this new hashset facts container.</param>
    public HashSetFactsContainer(IntegerInterval interval)
    {
      this.m_facts = new HashSet<int>();
      this.m_sortedFacts = null;
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
      HashSetFactsContainer world = (HashSetFactsContainer)base.MemberwiseClone();
      // Do not clone internals, waste of memory? Copy on write?
      world.m_facts = new HashSet<int>(m_facts);
      world.m_sortedFacts = null;
      return world;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    /// <returns>True, or unknown.</returns>
    public override FuzzyBool IsSet(int formulaID)
    {
      return m_facts.Contains(formulaID) ? FuzzyBool.True :
                                           FuzzyBool.Unknown;
    }

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    public override void Set(int formulaID)
    {
      if (m_facts.Add(formulaID))
        m_hashCode += Utils.General.Hash(formulaID);
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    public override void Unset(int formulaID)
    {
      if (m_facts.Remove(formulaID))
        m_hashCode -= Utils.General.Hash(formulaID);
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this facts container is equal to another object.
    /// Two hashset facts container are equal if they both store the same facts.
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
        HashSetFactsContainer other = (HashSetFactsContainer)obj;
        return this.m_facts.SetEquals(other.m_facts);
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
    /// Comparison is done first on the hash code, then on the number of facts, 
    /// and finally facts are sorted and compared individually.
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
      HashSetFactsContainer otherCnt = (HashSetFactsContainer)other;

      if ((value = this.m_facts.Count.CompareTo(otherCnt.m_facts.Count)) != 0)
        return value;

      return this.SortedFacts.SequenceCompareTo(otherCnt.SortedFacts);
    }

    #endregion
  }
}
