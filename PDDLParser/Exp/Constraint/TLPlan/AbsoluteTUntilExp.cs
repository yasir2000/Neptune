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
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// Represents the "t-until" constraint expression. This is a time-bounded version of
  /// TLPlan's "until" constraint expression. This is not part of the PDDL language;
  /// it was rather added for TLPlan.
  /// This constraint expression uses absolute timestamps.
  /// </summary>
  /// <remarks>
  /// This object should only be created by <see cref="TUntilExp"/> constraint expressions.
  /// </remarks>
  /// <seealso cref="TUntilExp"/>
  /// <seealso cref="UntilExp"/>
  [TLPlan]
  public class AbsoluteTUntilExp : TUntilExp
  {
    #region Properties

    /// <summary>
    /// Gets the absolute time interval.
    /// </summary>
    public TimeInterval AbsoluteTimeInterval
    {
      // The parent's relative timestamp is actually an absolute timestamp.
      get { return this.RelativeTimeInterval; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new "t-until" constraint expression with absolute timestamps.
    /// </summary>
    /// <param name="absoluteInterval">The absolute interval.</param>
    /// <param name="arg1">The first expression, which must always be true until the second one is.</param>
    /// <param name="arg2">The second expression. The second expression has to become true within the interval.
    /// When this one becomes true, the value of the first one becomes irrelevant. Note that the second 
    /// expression must become true before the end of the trajectory.</param>
    public AbsoluteTUntilExp(TimeInterval absoluteInterval, IConstraintExp arg1, IConstraintExp arg2)
      : base(absoluteInterval, arg1, arg2)
    {
    }

    #endregion

    #region TEventuallyExp Interface Overrides

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      double worldTime = world.GetTotalTime();
      int timeComparison = this.AbsoluteTimeInterval.CompareTimeToInterval(worldTime);

      if (timeComparison < 0) // Time is lower than the interval
      {
        return this.Exp1.Progress(world, bindings) && new ProgressionValue(this, this.AbsoluteTimeInterval.LowerBound);
      }
      else if (timeComparison == 0) // Time is in interval
      {
        TimeValue nextTimestamp = double.IsInfinity(this.AbsoluteTimeInterval.UpperBound.Time) ? ProgressionValue.NoTimestamp
                                                                                               : this.AbsoluteTimeInterval.UpperBound;

        return this.Exp2.Progress(world, bindings) || (this.Exp1.Progress(world, bindings) && new ProgressionValue(this, nextTimestamp));
      }
      else // Time is greater than the interval (interval timed out)
      {
        return ProgressionValue.False;
      }
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      if (this.AbsoluteTimeInterval.IsTimeGreaterThanInterval(idleWorld.GetTotalTime()))
      {
        // The interval timed out.
        return Bool.False;
      }
      else
      {
        // We are still in the interval (i.e. : upper bound is infinite)
        return Exp2.EvaluateIdle(idleWorld, bindings);
      }
    }

    #endregion
  }
}
