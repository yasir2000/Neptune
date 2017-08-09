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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents a universal quantification of an expression.
  /// </summary>
  public class ForallConstraintExp : AbstractForallExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new universal expression with the given quantified variables and 
    /// constraint expression.
    /// </summary>
    /// <param name="vars">The quantified variables.</param>
    /// <param name="exp">The quantified expression</param>
    public ForallConstraintExp(HashSet<ObjectParameterVariable> vars, IConstraintExp exp)
      : base(vars, exp)
    {
      System.Diagnostics.Debug.Assert(exp != null && vars != null && !vars.ContainsNull());
    }

    /// <summary>
    /// Creates a new ground expression equivalent to this quantified expression.
    /// </summary>
    /// <returns>A new ground expression equivalent to this quantified expression.</returns>
    protected override IConstraintExp GenerateEquivalentExp()
    {
      return new AndConstraintExp(GetBodySubstitutions());
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return this.GetEquivalentExp().Progress(world, bindings);
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
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return this.GetEquivalentExp().EvaluateIdle(idleWorld, bindings);
    }
  }
}
