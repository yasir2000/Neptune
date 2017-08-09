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

using System.Collections.Generic;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// Represents the "t-eventually" constraint expression. This is a time-bounded version of
  /// TLPlan's "eventually" constraint expression, which is equivalent to the  <see cref="SometimeExp"/>
  /// constraint expression. This is not part of the PDDL language; it was rather added for TLPlan.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  /// <seealso cref="SometimeExp"/>
  [TLPlan]
  public class TEventuallyExp : IntervalConstraintExp
  {
    /// <summary>
    /// Creates a new "t-eventually" constraint expression.
    /// </summary>
    /// <param name="interval">The relative time interal to when the expression is first progressed.</param>
    /// <param name="exp">The body of the constraint expression.</param>
    public TEventuallyExp(TimeInterval interval, IConstraintExp exp)
      : base("t-eventually", interval, exp)
    {
      System.Diagnostics.Debug.Assert(interval.LowerBound.Time >= 0 
                                   && interval.UpperBound.Time >= 0 
                                   && exp != null);
    }
    
    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This creates an instance of <see cref="AbsoluteTEventuallyExp"/> for progression.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    /// <seealso cref="AbsoluteTEventuallyExp"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new AbsoluteTEventuallyExp(this.RelativeTimeInterval.AddTime(world.GetTotalTime()), Exp).Progress(world, bindings);
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// This creates an instance of <see cref="AbsoluteTEventuallyExp"/> for evaluation.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    /// <seealso cref="AbsoluteTEventuallyExp"/>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return new AbsoluteTEventuallyExp(this.RelativeTimeInterval.AddTime(idleWorld.GetTotalTime()), Exp).EvaluateIdle(idleWorld, bindings);
    }
  }
}