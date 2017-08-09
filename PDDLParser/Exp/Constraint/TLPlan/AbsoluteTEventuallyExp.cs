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
  /// Represents the "t-eventually" constraint expression. This is a time-bounded version of
  /// TLPlan's "eventually" constraint expression, which is equivalent to the  <see cref="SometimeExp"/>
  /// constraint expression. This is not part of the PDDL language; it was rather added for TLPlan.
  /// This constraint expression uses absolute timestamps.
  /// </summary>
  /// <remarks>
  /// This object should only be created by <see cref="TEventuallyExp"/> constraint expressions.
  /// </remarks>
  /// <seealso cref="TEventuallyExp"/>
  [TLPlan]
  public class AbsoluteTEventuallyExp : TEventuallyExp
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
    /// Creates a new "t-eventually" constraint expression with absolute timestamps.
    /// </summary>
    /// <param name="absoluteInterval">The absolute interval.</param>
    /// <param name="exp">The body of the constraint expression.</param>
    public AbsoluteTEventuallyExp(TimeInterval absoluteInterval, IConstraintExp exp)
      : base(absoluteInterval, exp)
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
        return new ProgressionValue(this, this.AbsoluteTimeInterval.LowerBound);
      }
      else if (timeComparison == 0) // Time is in interval
      {
        TimeValue nextTimestamp = double.IsInfinity(this.AbsoluteTimeInterval.UpperBound.Time) ? ProgressionValue.NoTimestamp
                                                                                               : this.AbsoluteTimeInterval.UpperBound;

        return Exp.Progress(world, bindings) || new ProgressionValue(this, nextTimestamp);
      }
      else // Time is greater than the interval
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
        return Exp.EvaluateIdle(idleWorld, bindings);
      }
    }

    #endregion
  }
}
