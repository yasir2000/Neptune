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
using TLPlan.World;
using PDDLParser;

namespace TLPlan
{
  /// <summary>
  /// Represents the instantiation of a parsed PDDL action with ground parameters.
  /// </summary>
  public interface IOperator : IComparable<IOperator>
  {
    /// <summary>
    /// Returns whether this operator is applicable to the given world, i.e. if it's preconditions are met.
    /// </summary>
    /// <param name="world">The world in which the verification occurs.</param>
    /// <returns>True if the operator's preconditions are met in the given world.</returns>
    bool IsApplicable(TLPlanReadOnlyDurativeClosedWorld world);
    
    /// <summary>
    /// Applies the effects of this operator on a copy of the given world.
    /// </summary>
    /// <param name="world">The world on which effects must be applied.</param>
    /// <returns>A copy of the given world on which the effects of this operator have been applied, or null,
    /// if the application failed.</returns>
    TLPlanDurativeClosedWorld Apply(TLPlanReadOnlyDurativeClosedWorld world);

    /// <summary>
    /// Gets the name of this operator.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Verify whether this operator should be elided from plans, i.e. if it should not be shown in them.
    /// </summary>
    bool IsElided { get; }

    /// <summary>
    /// Verify whether this operator has a duration.
    /// </summary>
    bool HasDuration { get; }

    /// <summary>
    /// Returns the duration of this operator as evaluated in the given world.
    /// The operator might have no duration, upon which case an exception is thrown.
    /// </summary>
    /// <param name="world">The world in which the duration must be calculated.</param>
    /// <exception cref="NotSupportedException">
    /// The operator has no duration. Call <see cref="HasDuration"/> to verify if the operator has a duration.
    /// - OR -
    /// The durative operator contains unsupported time constraints (duration inequalities, timed-specific duration
    /// constraint or no duration constraint at all).
    /// </exception>
    /// <exception cref="PDDLParser.Exception.UndefinedExpException">The evaluation of duration encoutered an undefined value.</exception>
    /// <returns>The duration of this operator as evaluated in the given world.</returns>
    double GetDuration(TLPlanReadOnlyDurativeClosedWorld world);
  }
}
