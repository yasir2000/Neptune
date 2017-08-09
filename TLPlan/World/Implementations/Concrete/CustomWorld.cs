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
using PDDLParser.Extensions;
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// A custom world is an updatable world with a custom facts container and a custom
  /// fluents container.
  /// As is, a custom world cannot evaluate formulas on its own: indeed, some facts container
  /// implementation like hashset return unknown when a certain predicate ID is not contained
  /// in their internal set of IDs, causing both actual unknown predicates and false predicates
  /// to be evaluated to unknown.
  /// </summary>
  public class CustomWorld : ExtendedOpenWorld, IOpenWorld
  {
    #region Private Fields

    /// <summary>
    /// The facts container holds all facts.
    /// </summary>
    protected FactsContainer m_factsContainer;

    /// <summary>
    /// The fluents container holds all numeric and object fluents.
    /// </summary>
    protected FluentsContainer m_fluentsContainer;

    #endregion   

    #region Constructor

    /// <summary>
    /// Creates a new custom world with the specified facts and fluents containers.
    /// </summary>
    /// <param name="factsContainer">The facts container.</param>
    /// <param name="fluentsContainer">The fluents container.</param>
    /// <param name="options">The TLPlan options.</param>
    public CustomWorld(FactsContainer factsContainer, FluentsContainer fluentsContainer, 
                       TLPlanOptions options)
      : base(options)
    {
      this.m_factsContainer = factsContainer;
      this.m_fluentsContainer = fluentsContainer;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies this custom world.
    /// </summary>
    /// <returns>A copy of this custom world.</returns>
    public override ExtendedOpenWorld Copy()
    {
      CustomWorld world = (CustomWorld)base.MemberwiseClone();
      // Do not clone internals, waste of memory? Copy on write?
      world.m_factsContainer = this.m_factsContainer.Copy();
      world.m_fluentsContainer = this.m_fluentsContainer.Copy();
      return world;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, or unknown.</returns>
    public override FuzzyBool IsSet(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      return this.m_factsContainer.IsSet(formula.FormulaID);
    }

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public override FuzzyDouble InternalGetNumericFluent(NumericFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      return this.m_fluentsContainer.GetNumericFluent(fluent.FormulaID);
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public override FuzzyConstantExp InternalGetObjectFluent(ObjectFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      return this.m_fluentsContainer.GetObjectFluent(fluent.FormulaID);
    }

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public override void Set(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      this.m_factsContainer.Set(formula.FormulaID);
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formula">A atomic formula with constant arguments.</param>
    public override void Unset(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      this.m_factsContainer.Unset(formula.FormulaID);
    }

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public override void SetNumericFluent(NumericFluentApplication fluent, double value)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.SetNumericFluent(fluent.FormulaID, value);
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public override void SetObjectFluent(ObjectFluentApplication fluent, Constant value)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.SetObjectFluent(fluent.FormulaID, value);
    }

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    public override void UndefineObjectFluent(ObjectFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.UndefineObjectFluent(fluent.FormulaID);
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
        CustomWorld other = (CustomWorld)obj;
        return this.m_factsContainer.Equals(other.m_factsContainer) &&
               this.m_fluentsContainer.Equals(other.m_fluentsContainer);
      }
    }

    /// <summary>
    /// Returns the hash code of this world.
    /// </summary>
    /// <returns>The hash code of this world.</returns>
    public override int GetHashCode()
    {
      return this.m_factsContainer.GetHashCode() + this.m_fluentsContainer.GetHashCode();
    }

    #endregion

    #region IComparable<ExtendedOpenWorld> Members

    /// <summary>
    /// Compares this custom world with another extended world.
    /// </summary>
    /// <param name="other">The other extended world to compare this custom world to.</param>
    /// <returns>An integer representation the total order relation between the two worlds.
    /// </returns>
    public override int CompareTo(ExtendedOpenWorld other)
    {
      CustomWorld otherWorld = (CustomWorld)other;

      int value = this.GetHashCode().CompareTo(other.GetHashCode());
      if (value != 0)
        return value;

      if ((value = this.m_factsContainer.CompareTo(otherWorld.m_factsContainer)) != 0)
        return value;

      return this.m_fluentsContainer.CompareTo(otherWorld.m_fluentsContainer);
    }

    #endregion
  }
}
