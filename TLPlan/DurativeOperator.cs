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
using PDDLParser.Exp;
using PDDLParser.Exp.Comparison;
using PDDLParser.Exception;

namespace TLPlan
{
  /// <summary>
  /// Represents a durative operator.
  /// </summary>
  class DurativeOperator : AbstractOperator
  {
    #region Private Fields

    /// <summary>
    /// The duration expression of this operator.
    /// </summary>
    private ILogicalExp m_durationExp;

    /// <summary>
    /// The start condition of this operator.
    /// </summary>
    private ILogicalExp m_startCondition;
    /// <summary>
    /// The overall condition of this operator.
    /// </summary>
    private ILogicalExp m_overallCondition;
    /// <summary>
    /// The end condition of this operator.
    /// </summary>
    private ILogicalExp m_endCondition;

    /// <summary>
    /// The start effect of this operator.
    /// </summary>
    private IEffect m_startEffect;
    /// <summary>
    /// The continuous effect of this operator.
    /// </summary>
    private IEffect m_continuousEffect;
    /// <summary>
    /// The overall effect of this operator.
    /// As of now, this is only used for "over all" preferences.
    /// </summary>
    private IEffect m_overallEffect;
    /// <summary>
    /// The end effect of this operator.
    /// </summary>
    private IEffect m_endEffect;

    /// <summary>
    /// Whether multiple operators can be used concurrently.
    /// </summary>
    private bool m_isConcurrent;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new durative operator with the given name, duration, conditions, and effects.
    /// </summary>
    /// <param name="name">The name of the operator.</param>
    /// <param name="duration">The duration expression of the operator.</param>
    /// <param name="startCondition">The start condition of the operator.</param>
    /// <param name="overallCondition">The overall condition of the operator.</param>
    /// <param name="endCondition">The end condition of the operator.</param>
    /// <param name="startEffect">The start effect of the operator.</param>
    /// <param name="continuousEffect">The continuous effect of the operator.</param>
    /// <param name="overallEffect">The overall effect of the operator.</param>
    /// <param name="endEffect">The end effect of the operator.</param>
    /// <param name="isConcurrent">Whether multiple operator can be used concurrently when planning.</param>
    /// <param name="isElided">Whether this operator should be elided from plans.</param>
    public DurativeOperator(string name, ILogicalExp duration,
                            ILogicalExp startCondition, ILogicalExp overallCondition, ILogicalExp endCondition,
                            IEffect startEffect, IEffect continuousEffect, IEffect overallEffect, IEffect endEffect,
                            bool isConcurrent, bool isElided)
      : base(name, isElided)
    {
      m_durationExp = duration;

      m_startCondition = startCondition;
      m_overallCondition = overallCondition;
      m_endCondition = endCondition;

      m_startEffect = startEffect;
      m_continuousEffect = continuousEffect;
      m_overallEffect = overallEffect;
      m_endEffect = endEffect;

      m_isConcurrent = isConcurrent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns the duration of this operator as evaluated in the given world.
    /// </summary>
    /// <param name="world">The world in which the duration must be calculated.</param>
    /// <exception cref="NotSupportedException">
    /// The durative operator contains unsupported time constraints (duration inequalities, timed-specific duration
    /// constraint or no duration constraint at all).
    /// </exception>
    /// <exception cref="UndefinedExpException">The evaluation of duration encoutered an undefined value.</exception>
    /// <returns>The duration of this operator as evaluated in the given world.</returns>
    public override double GetDuration(TLPlanReadOnlyDurativeClosedWorld world)
    {
      // TODO: Find a better way than this one to obtain the actual duration of the operator!
      if (m_durationExp is NumericEqualComp)
      {
        PDDLParser.Exp.Struct.Double value = ((NumericEqualComp)m_durationExp).Arg2.Evaluate(world, new LocalBindings());
        switch (value.Status)
        {
          case PDDLParser.Exp.Struct.Double.State.Undefined:
            throw new UndefinedExpException("Evaluating " + ((NumericEqualComp)m_durationExp).Arg2.ToString()
                              + " yields an undefined value!");
          default:
            return value.Value;
        }
      }
      else
        throw new NotSupportedException("Duration inequalities and time-specified duration constraints are not yet supported.");
    }

    #endregion

    #region AbstractOperator interface overrides

    /// <summary>
    /// Applies the effects of this operator on a copy of the given world. If planning is concurrent, only
    /// the start effects are applied and other effects are scheduled for later application. If planning is not
    /// concurrent, all effects are applied at once.
    /// </summary>
    /// <param name="world">The world on which effects must be applied.</param>
    /// <returns>A copy of the given world on which the effects of this operator have been applied.</returns>
    protected override TLPlanDurativeClosedWorld ApplyInternal(TLPlanReadOnlyDurativeClosedWorld world)
    {
      TLPlanDurativeClosedWorld newWorld = world.Copy();
      ActionContext newContext = new ActionContext();

      double duration = GetDuration(world);

      newWorld.Modify(world, m_startEffect, newContext);

      if (m_isConcurrent)
      {
        newWorld.AddEvent(duration, new Event(m_overallCondition, m_endCondition, m_overallEffect, m_endEffect, newContext));
        newWorld.TimeStamp += TLPlanReadOnlyDurativeClosedWorld.SmallTimeOffset; // Ensure that start effects do not allow other actions to start immediately
      }
      else // Immediately update the timestamp and effects if actions are not concurrent.
      {
        if (newWorld.Satisfies(m_overallCondition))
        {
          // Apply the overall effects (for preferences and conditional effects)
          newWorld.Modify(world, m_overallEffect, newContext);

          newWorld.TimeStamp += duration + TLPlanReadOnlyDurativeClosedWorld.SmallTimeOffset; // Delay for at end effect/at start condition on the same formula
          newWorld.Modify(world, m_endEffect, newContext);
        }
        else
          newWorld = null;
      }

      return newWorld;
    }

    #endregion

    #region IOperator interface

    /// <summary>
    /// Returns whether this durative operator is applicable to the given world, i.e. if it's start conditions are met.
    /// </summary>
    /// <param name="world">The world in which the verification occurs.</param>
    /// <returns>True if the durative operator's start conditions are met in the given world.</returns>
    public override bool IsApplicable(TLPlanReadOnlyDurativeClosedWorld world)
    {
      return world.Satisfies(m_startCondition);
    }

    #endregion
  }
}
