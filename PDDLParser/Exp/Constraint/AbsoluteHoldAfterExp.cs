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

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "hold-after" constraint expression of the PDDL language.
  /// This constraint expression uses absolute timestamps.
  /// </summary>
  /// <remarks>
  /// This object should only be created by <see cref="HoldAfterExp"/> constraint expressions.
  /// </remarks>
  /// <seealso cref="HoldAfterExp"/>
  public class AbsoluteHoldAfterExp : HoldAfterExp
  {
    #region Properties

    /// <summary>
    /// Gets the absolute timestamp after which the constraint must hold.
    /// </summary>
    public double AbsoluteTimestamp
    {
      // The parent's relative timestamp is actually an absolute one.
      get { return this.RelativeTimestamp; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new "hold-after" constraint expression with an absolute timestamp.
    /// </summary>
    /// <param name="exp">The constraint that must hold after a given timestamp.</param>
    /// <param name="absoluteTimestamp">The absolute timestamp after which the constraint must hold.</param>
    public AbsoluteHoldAfterExp(IConstraintExp exp, double absoluteTimestamp)
      : base(exp, absoluteTimestamp)
    {
    }

    #endregion

    #region HoldAfterExp Interface Overrides

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.progress"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, ILocalBindings bindings)
    {
      if (world.getTotalTime() < this.AbsoluteTimestamp)
        return new ProgressionValue(this, this.AbsoluteTimestamp);
      else
        return new AlwaysExp(getExp()).Progress(world, bindings);
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.evaluateIdle"/>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, ILocalBindings bindings)
    {
      return getExp().EvaluateIdle(idleWorld, bindings);
    }

    #endregion
  }
}
