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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLPlan.World;

using PDDLParser;

namespace TLPlan
{
  /// <summary>
  /// Represents the common part of all instantiated operators.
  /// </summary>
  public abstract class AbstractOperator : IOperator
  {
    #region Private Fields

    /// <summary>
    /// The name of this operator.
    /// </summary>
    private string m_name;

    /// <summary>
    /// Whether this operator should be elided from plans.
    /// </summary>
    private bool m_isElided;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of this operator.
    /// </summary>
    public string Name { get { return m_name; } }

    /// <summary>
    /// Verify whether this operator should be elided from plans, i.e. if it should not be shown in them.
    /// </summary>
    public virtual bool IsElided { get { return m_isElided; } }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an new instantiated operator.
    /// </summary>
    /// <param name="name">The name of the operator.</param>
    /// <param name="isElided">Whether the operator should be elided from plans.</param>
    public AbstractOperator(string name, bool isElided)
    {
      m_name = name;
      m_isElided = isElided;
    }

    #endregion

    #region AbstractOperator Methods

    /// <summary>
    /// Applies the effects of this operator on a copy of the given world.
    /// </summary>
    /// <param name="world">The world on which effects must be applied.</param>
    /// <returns>A copy of the given world on which the effects of this operator have been applied.</returns>
    protected abstract TLPlanDurativeClosedWorld ApplyInternal(TLPlanReadOnlyDurativeClosedWorld world);

    #endregion

    #region IOperator Interface

    /// <summary>
    /// Applies the effects of this operator on a copy of the given world. This also verifies whether the new world
    /// violates any action conditions, and applies overall effects to it.
    /// </summary>
    /// <param name="world">The world on which effects must be applied.</param>
    /// <returns>A copy of the given world on which the effects of this operator have been applied, or null,
    /// if the new world violates an action conditon.</returns>
    public /* sealed */ TLPlanDurativeClosedWorld Apply(TLPlanReadOnlyDurativeClosedWorld world)
    {
      TLPlanDurativeClosedWorld newWorld = ApplyInternal(world);

      // Verify the world's consistency (i.e. whether it violates any condition)
      if (newWorld != null)
      {
        if (!newWorld.IsConsistent())
          return null;

        // Update the world's preferences violations
        newWorld.ApplyOverallEffects(world);
      }

      return newWorld;
    }

    /// <summary>
    /// Returns whether this operator is applicable to the given world, i.e. if it's preconditions are met.
    /// </summary>
    /// <param name="world">The world in which the verification occurs.</param>
    /// <returns>True if the operator's preconditions are met in the given world.</returns>
    public abstract bool IsApplicable(TLPlanReadOnlyDurativeClosedWorld world);

    /// <summary>
    /// Verify whether this operator has a duration.
    /// </summary>
    public virtual bool HasDuration { get { return true; } }

    /// <summary>
    /// Returns the duration of this operator as evaluated in the given world.
    /// The operator might have no duration, upon which case an exception is thrown.
    /// </summary>
    /// <param name="world">The world in which the duration must be calculated.</param>
    /// <exception cref="NotSupportedException">
    /// The operator has no duration. Call <see cref="HasDuration"/> to verify if the operator has a duration.
    /// </exception>
    /// <returns>The duration of this operator as evaluated in the given world.</returns>
    public abstract double GetDuration(TLPlanReadOnlyDurativeClosedWorld world);

    #endregion

    #region IComparable<IOperator> Interface

    /// <summary>
    /// Compares this operator with another operator.
    /// </summary>
    /// <param name="op">The other operator to compare this operator to.</param>
    /// <returns>An integer representing the total order relation between the two operators.</returns>
    public int CompareTo(IOperator op)
    {
      return this.Name.CompareTo(op.Name);
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns a string representation of this operator.
    /// </summary>
    /// <returns>A string representation of this operator.</returns>
    public override string ToString()
    {
      return Name;
    }

    #endregion
  }
}
