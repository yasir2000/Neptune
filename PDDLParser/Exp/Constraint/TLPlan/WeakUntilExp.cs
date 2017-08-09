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
  /// Represents the "weak-until" operator of the LTL language. This is not part of PDDL; it was rather added
  /// for TLPlan.
  /// </summary>
  /// <remarks>
  /// Note that (weak-until a b) = (until a (or b (always a))).
  /// </remarks>
  /// <seealso cref="UntilExp"/>
  [TLPlan]
  public class WeakUntilExp : UntilExp
  { 
    /// <summary>
    /// Creates a new "weak-until" constraint expression.
    /// </summary>
    /// <param name="arg1">The first expression, which must always be true until the second one is.</param>
    /// <param name="arg2">The second expression. When this one becomes true, the value of the first one
    /// becomes irrelevant. Note that, unlike in the "until" operator, the second expression does not have to
    /// become true for a trajectory to be valid, so long as the first expression is always true.</param>
    /// <seealso cref="PDDLParser.Exp.Constraint.TLPlan.UntilExp"/>
    public WeakUntilExp(IConstraintExp arg1, IConstraintExp arg2) 
      : base(arg1, arg2)
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);
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
      return Bool.True;
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(weak-until ");
      str.Append(this.Exp1.ToString());
      str.Append(" ");
      str.Append(this.Exp2.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(weak-until ");
      str.Append(this.Exp1.ToTypedString());
      str.Append(" ");
      str.Append(this.Exp2.ToTypedString());
      str.Append(")");
      return str.ToString();
    }
  }
}
