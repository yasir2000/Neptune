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
  /// A treemap fluents container stores fluents in a treemap (sorted dictionary).
  /// This implementation is O(log n) for retrieving fluents' value, but allows for
  /// a much faster comparison between two fluents container, since treesets keep fluents
  /// sorted.
  /// Note that as is, a treeset fluents container cannot determine whether a fluent ID not
  /// contained in the treeset is actually "undefined" or "unknown", and hence always returns 
  /// unknown.
  /// </summary>
  public class TreeMapFluentsContainer : FluentsContainer
  {
    #region Private Fields

    /// <summary>
    /// The treeset of numeric fluents IDs.
    /// </summary>
    private SortedDictionary<int, double> m_numericFluents;

    /// <summary>
    /// The treeset of object fluents IDs.
    /// </summary>
    private SortedDictionary<int, Constant> m_objectFluents;
    
    /// <summary>
    /// The hash code of this fluents container.
    /// </summary>
    private int m_hashCode;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new empty treeset fluents container.
    /// </summary>
    /// <param name="numericInterval">The interval of numeric fluents ID whose value are stored 
    /// in this new fluents container.</param>
    /// <param name="objectInterval">The interval of object fluents ID whose value are stored 
    /// in this new fluents container.</param>
    public TreeMapFluentsContainer(IntegerInterval numericInterval, IntegerInterval objectInterval)
    {
      this.m_numericFluents = new SortedDictionary<int, double>();
      this.m_objectFluents = new SortedDictionary<int, Constant>();
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
      TreeMapFluentsContainer world = (TreeMapFluentsContainer)base.MemberwiseClone();
      // Do not clone internals, waste of memory? Copy on write?
      world.m_numericFluents = new SortedDictionary<int, double>(m_numericFluents);
      world.m_objectFluents = new SortedDictionary<int, Constant>(m_objectFluents);
      return world;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluentID">A numeric fluent ID.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public override FuzzyDouble GetNumericFluent(int fluentID)
    {
      double value;
      if (m_numericFluents.TryGetValue(fluentID, out value))
        return new FuzzyDouble(value);
      else
        return FuzzyDouble.Unknown;
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public override FuzzyConstantExp GetObjectFluent(int fluentID)
    {
      Constant value;
      if (m_objectFluents.TryGetValue(fluentID, out value))
        return new FuzzyConstantExp(value);
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
      double oldValue;
      if (m_numericFluents.TryGetValue(fluentID, out oldValue))
      {
        m_hashCode -= (Utils.General.Hash(fluentID) * oldValue.GetHashCode());
      }
      m_numericFluents[fluentID] = value;
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
      Constant oldValue;
      if (m_objectFluents.TryGetValue(fluentID, out oldValue))
      {
        m_hashCode -= (Utils.General.Hash(fluentID) * oldValue.GetHashCode());
      }
      m_objectFluents[fluentID] = value;
      m_hashCode += (Utils.General.Hash(fluentID) * value.GetHashCode());
    }

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    public override void UndefineObjectFluent(int fluentID)
    {
      Constant oldValue;
      if (m_objectFluents.TryGetValue(fluentID, out oldValue))
      {
        m_hashCode -= (Utils.General.Hash(fluentID) * oldValue.GetHashCode());
        m_objectFluents.Remove(fluentID);
      }
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this fluents container is equal to another fluents container.
    /// Two hashset fluent containers are equal if all their respective fluents have the
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
        TreeMapFluentsContainer other = (TreeMapFluentsContainer)obj;
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
    /// Comparison is done first on the hash code, then on the number of defined fluents, and
    /// finally on the individual fluents themselves.
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
      TreeMapFluentsContainer otherCnt = (TreeMapFluentsContainer)other;

      if ((value = this.m_numericFluents.Count.CompareTo(otherCnt.m_numericFluents.Count)) != 0)
        return value;

      if ((value = this.m_objectFluents.Count.CompareTo(otherCnt.m_objectFluents.Count)) != 0)
        return value;

      if ((value = this.m_numericFluents.SequenceCompareTo(otherCnt.m_numericFluents)) != 0)
        return value;

      return this.m_objectFluents.SequenceCompareTo(otherCnt.m_objectFluents);
    }

    #endregion
  }
}
