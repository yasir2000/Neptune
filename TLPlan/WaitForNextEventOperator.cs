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
  /// Represents the special wait-for-next-event operator, which is used to find the next evaluable
  /// event (which contains overall and end conditions and effects of prior operator applications) 
  /// and to apply it to the current world.
  /// </summary>
  public class WaitForNextEventOperator : AbstractOperator
  {
    #region Constructors

    /// <summary>
    /// Creates a new wait-for-next-event operator.
    /// </summary>
    /// <param name="isElided">Whether the operator should be elided from plans. Take note that if
    /// the operator is not elided, plan validation is not possible.</param>
    public WaitForNextEventOperator(bool isElided)
      : base("(wait-for-next-event)", isElided)
    {
      // TODO: Replace the name of the formula by the one parsed!
    }

    #endregion

    #region IOperator interface

    /// <summary>
    /// Returns whether this operator is applicable to the given world, i.e. whether there are any events left to
    /// process and whether the remaning overall and end conditions are met.
    /// </summary>
    /// <param name="world">The world in which the verification occurs.</param>
    /// <returns>True if the operator's preconditions are met in the given world.</returns>
    public override bool IsApplicable(TLPlanReadOnlyDurativeClosedWorld world)
    {
      return !world.AreAllEventsProcessed() && world.GetNextDurativeConditions().All(cond => world.Satisfies(cond));
    }

    #endregion

    #region AbstractOperator interface overrides

    /// <summary>
    /// Verify whether this operator has a duration. The wait-for-next-event has no duration.
    /// </summary>
    public override bool HasDuration { get { return false; } }

    /// <summary>
    /// Returns the duration of this operator as evaluated in the given world.
    /// The wait-for-next-event has no duration; it therefore throws an exception.
    /// </summary>
    /// <param name="world">The world in which the duration must be calculated (unused).</param>
    /// <exception cref="NotSupportedException">
    /// The operator has no duration. Call <see cref="HasDuration"/> to verify if the operator has a duration.
    /// </exception>
    /// <returns>The duration of this operator as evaluated in the given world.</returns>
    public override double GetDuration(TLPlanReadOnlyDurativeClosedWorld world)
    {
      throw new NotSupportedException("wait-for-next-event has no duration.");
    }

    /// <summary>
    /// Applies the effects of this operator on a copy of the given world. This essentially retrieves the
    /// next events, applies the effects to the world and updates its timestamp.
    /// </summary>
    /// <param name="world">The world on which effects must be applied.</param>
    /// <returns>A copy of the given world on which the effects of this operator have been applied.</returns>
    protected override TLPlanDurativeClosedWorld ApplyInternal(TLPlanReadOnlyDurativeClosedWorld world)
    {
      TLPlanDurativeClosedWorld newWorld = world.Copy();

      // Apply and remove the next effects, and update the timestamp
      newWorld.ApplyNextEndEffects(world);

      return newWorld;
    }

    #endregion
  }
}
