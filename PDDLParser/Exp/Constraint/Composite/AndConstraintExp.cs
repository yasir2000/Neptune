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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// This class represents a conjunction of constraint expressions.
  /// </summary>
  public class AndConstraintExp : AbstractAndExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new conjunction of constraint expressions.
    /// </summary>
    /// <param name="exps">The constraint expressions associated with the new conjunctive 
    /// expression.</param>
    public AndConstraintExp(IEnumerable<IConstraintExp> exps)
      : base(exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new conjunction of constraint expressions.
    /// </summary>
    /// <param name="exps">The constraint expressions associated with the new conjunctive 
    /// expression.</param>
    public AndConstraintExp(params IConstraintExp[] exps)
      : this((IEnumerable<IConstraintExp>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// The algorithm is: Progress(and formula1 ...) => (and Progress(formula1) ...)
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <seealso cref="IConstraintExp.Progress"/>
    public virtual ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      ProgressionValue result = ProgressionValue.True;
      foreach (IConstraintExp exp in this.m_expressions)
      {
        result = result & exp.Progress(world, bindings);
        if (!result)
          break;
      }
      return result;
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    public virtual Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      Bool value = Bool.True;
      foreach (IConstraintExp exp in this.m_expressions)
      {
        value = value & exp.EvaluateIdle(idleWorld, bindings);
        if (!value)
          break;
      }
      return value;
    }
  }
}
