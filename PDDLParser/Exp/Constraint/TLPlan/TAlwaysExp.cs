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
  /// Represents the "t-always" constraint expression. This is a time-bounded version of
  /// the <see cref="AlwaysExp"/> constraint expression. This is not part of the PDDL language;
  /// it was rather added for TLPlan.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  /// <seealso cref="AlwaysExp"/>
  /// <seealso cref="HoldDuringExp"/>
  [TLPlan]
  public class TAlwaysExp : IntervalConstraintExp
  {
    /// <summary>
    /// Creates a new "t-always" constraint expression.
    /// </summary>
    /// <param name="interval">The relative time interal to when the expression is first progressed.</param>
    /// <param name="arg">The body of the constraint expression.</param>
    public TAlwaysExp(TimeInterval interval, IConstraintExp arg)
      : base("t-always", interval, arg)
    {
      System.Diagnostics.Debug.Assert(interval.LowerBound.Time >= 0 && interval.UpperBound.Time >= 0 && arg != null);
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This creates an instance of <see cref="AbsoluteTAlwaysExp"/> for progression.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    /// <seealso cref="AbsoluteTAlwaysExp"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new AbsoluteTAlwaysExp(this.RelativeTimeInterval.AddTime(world.GetTotalTime()), Exp).Progress(world, bindings);
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// This creates an instance of <see cref="AbsoluteTAlwaysExp"/> for evaluation.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    /// <seealso cref="AbsoluteTAlwaysExp"/>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return new AbsoluteTAlwaysExp(this.RelativeTimeInterval.AddTime(idleWorld.GetTotalTime()), Exp).EvaluateIdle(idleWorld, bindings);
    }
  }
}