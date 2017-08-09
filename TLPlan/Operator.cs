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

namespace TLPlan
{
  /// <summary>
  /// Represents a STRIPS-like, non-durative operator.
  /// </summary>
  public class Operator : AbstractOperator
  {
    #region Private Fields

    /// <summary>
    /// This operator's precondition.
    /// </summary>
    private ILogicalExp m_precondition;
    /// <summary>
    /// This operator's effect.
    /// </summary>
    private IEffect m_effect;
    /// <summary>
    /// This operator's duration.
    /// </summary>
    private double m_duration;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new STRIPS-like operator.
    /// </summary>
    /// <param name="name">The name of the operator.</param>
    /// <param name="precondition">The precondition of the operator.</param>
    /// <param name="effect">The effect of the operator.</param>
    /// <param name="isConcurrent">Whether this operator is used in concurrent planning.</param>
    /// <param name="isElided">Whether this operator should be elided from plans.</param>
    public Operator(string name,
                    ILogicalExp precondition,
                    IEffect effect,
                    bool isConcurrent,
                    bool isElided)
      : base(name, isElided)
    {
      m_precondition = precondition;
      m_effect = effect;
      m_duration = (isConcurrent ? 0.0 : 1.0);
    }

    #endregion

    #region AbstractOperator Interface Overrides

    /// <summary>
    /// Returns the duration of this operator. This is always 0.0 when planning is concurrent, and 1.0 otherwise.
    /// </summary>
    /// <param name="world">The world in which the duration must be calculated (this is unused).</param>
    /// <returns>The duration of this operator.</returns>
    public override double GetDuration(TLPlanReadOnlyDurativeClosedWorld world)
    {
      return m_duration;
    }

    /// <summary>
    /// Returns whether this operator is applicable to the given world, i.e. if it's preconditions are met.
    /// </summary>
    /// <param name="world">The world in which the verification occurs.</param>
    /// <returns>True if the operator's preconditions are met in the given world.</returns>
    public override bool IsApplicable(TLPlanReadOnlyDurativeClosedWorld world)
    {
      return world.Satisfies(m_precondition);
    }

    /// <summary>
    /// Applies the effects of this operator on a copy of the given world.
    /// </summary>
    /// <param name="world">The world on which effects must be applied.</param>
    /// <returns>A copy of the given world on which the effects of this operator have been applied.</returns>
    protected override TLPlanDurativeClosedWorld ApplyInternal(TLPlanReadOnlyDurativeClosedWorld world)
    {
      TLPlanDurativeClosedWorld newWorld = world.Copy();
      newWorld.Modify(world, m_effect);
      newWorld.TimeStamp += m_duration;
      return newWorld;
    }

    #endregion
  }
}
