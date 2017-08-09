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
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// A partial-cycle-check world does not take into account the described formulas with
  /// the attribute "DetectCycles" set to false when comparing itself with another world.
  /// To achieve this, it stores cycle-check attributes and no-cycle-check attributes in two
  /// different worlds and simplify forward query/updates to the appropriate world.
  /// </summary>
  public class PartialCycleCheckWorld : ExtendedOpenWorld
  {
    #region Private Fields

    /// <summary>
    /// The world responsible for storing the cycle-check formulas.
    /// </summary>
    private ExtendedOpenWorld m_cycleCheckWorld;

    /// <summary>
    /// The world responsible for storing the no-cycle-check formulas.
    /// </summary>
    private ExtendedOpenWorld m_noCycleCheckWorld;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new partial-cycle-check worlds with the specified cycle-check world and
    /// no-cycle-check world.
    /// </summary>
    /// <param name="cycleCheckWorld">The cycle-check world.</param>
    /// <param name="noCycleCheckWorld">The no-cycle-check world.</param>
    /// <param name="options">A set of TLPlan options.</param>
    public PartialCycleCheckWorld(ExtendedOpenWorld cycleCheckWorld, 
                                  ExtendedOpenWorld noCycleCheckWorld,
                                  TLPlanOptions options)
      : base(options)
    {
      this.m_cycleCheckWorld = cycleCheckWorld;
      this.m_noCycleCheckWorld = noCycleCheckWorld;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies this partial-cycle-check world.
    /// </summary>
    /// <returns>A copy of this partial-cycle-check  world.</returns>
    public override ExtendedOpenWorld Copy()
    {
      PartialCycleCheckWorld copy = (PartialCycleCheckWorld)this.MemberwiseClone();
      copy.m_cycleCheckWorld = this.m_cycleCheckWorld.Copy();
      copy.m_noCycleCheckWorld = this.m_noCycleCheckWorld.Copy();

      return copy;
    }

    #endregion

    #region IReadOnlyOpenWorld Members

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, or unknown.</returns>
    public override FuzzyBool IsSet(AtomicFormulaApplication formula)
    {
      if (formula.DetectCycles)
      {
        return m_cycleCheckWorld.IsSet(formula);
      }
      else
      {
        return m_noCycleCheckWorld.IsSet(formula);
      }
    }

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public override FuzzyDouble InternalGetNumericFluent(NumericFluentApplication fluent)
    {
      if (fluent.DetectCycles)
      {
        return m_cycleCheckWorld.GetNumericFluent(fluent);
      }
      else
      {
        return m_noCycleCheckWorld.GetNumericFluent(fluent);
      }
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public override FuzzyConstantExp InternalGetObjectFluent(ObjectFluentApplication fluent)
    {
      if (fluent.DetectCycles)
      {
        return m_cycleCheckWorld.GetObjectFluent(fluent);
      }
      else
      {
        return m_noCycleCheckWorld.GetObjectFluent(fluent);
      }
    }

    #endregion

    #region IOpenWorld Members

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public override void Set(AtomicFormulaApplication formula)
    {
      if (formula.DetectCycles)
      {
        m_cycleCheckWorld.Set(formula);
      }
      else
      {
        m_noCycleCheckWorld.Set(formula);
      }
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formula">A atomic formula with constant arguments.</param>
    public override void Unset(AtomicFormulaApplication formula)
    {
      if (formula.DetectCycles)
      {
        m_cycleCheckWorld.Unset(formula);
      }
      else
      {
        m_noCycleCheckWorld.Unset(formula);
      }
    }

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public override void SetNumericFluent(NumericFluentApplication fluent, double value)
    {
      if (fluent.DetectCycles)
      {
        m_cycleCheckWorld.SetNumericFluent(fluent, value);
      }
      else
      {
        m_noCycleCheckWorld.SetNumericFluent(fluent, value);
      }
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public override void SetObjectFluent(ObjectFluentApplication fluent, Constant value)
    {
      if (fluent.DetectCycles)
      {
        m_cycleCheckWorld.SetObjectFluent(fluent, value);
      }
      else
      {
        m_noCycleCheckWorld.SetObjectFluent(fluent, value);
      }
    }

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    public override void UndefineObjectFluent(ObjectFluentApplication fluent)
    {
      if (fluent.DetectCycles)
      {
        m_cycleCheckWorld.UndefineObjectFluent(fluent);
      }
      else
      {
        m_noCycleCheckWorld.UndefineObjectFluent(fluent);
      }
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this world is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this world is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        PartialCycleCheckWorld other = (PartialCycleCheckWorld)obj;
        return this.m_cycleCheckWorld.Equals(other.m_cycleCheckWorld);
      }
    }

    /// <summary>
    /// Returns the hash code of this world.
    /// </summary>
    /// <returns>The hash code of this world.</returns>
    public override int GetHashCode()
    {
      return this.m_cycleCheckWorld.GetHashCode();
    }

    #endregion

    #region IComparable<IExtendedOpenWorld> Members

    /// <summary>
    /// Compares this world with another world.
    /// </summary>
    /// <param name="other">The other world to compare this world to.</param>
    /// <returns>An integer representing the total order relation between the two worlds.
    /// </returns>
    public override int CompareTo(ExtendedOpenWorld other)
    {
      // No comparison is performed on m_noCycleCheckIExtendedOpenWorld

      PartialCycleCheckWorld otherWorld = (PartialCycleCheckWorld)other;

      return this.m_cycleCheckWorld.CompareTo(otherWorld.m_cycleCheckWorld);
    }

    #endregion
  }
}
