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
using PDDLParser.Exp.Constraint;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical
{
  /// <summary>
  /// Base class for logical expressions.
  /// </summary>
  public abstract class AbstractLogicalExp : AbstractConstraintExp, ILogicalExp
  {
    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public abstract FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// The bindings should not be modified by this call.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// This function is used to evaluate defined formulas' body as is done in the
    /// original TLPlan.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    [TLPlan]
    public abstract ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// The bindings should not be modified by this call.
    /// The resulting expression should not contain any unbound variables, since
    /// they are substituted according to the bindings supplied.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public abstract LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public abstract Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// This function is used to evaluate defined formulas' body as is done in the
    /// original TLPlan.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    [TLPlan]
    public abstract ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public abstract HashSet<PartialWorld> EnumerateAllSatisfyingWorlds();

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This function returns false if this constraint expression is not satisfied 
    /// in the given world;
    /// it returns true if the progression is always satisfied in the next worlds;
    /// else it returns the progressed expression.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(this.Evaluate(world, bindings));
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
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return this.Evaluate(idleWorld, bindings);
    }
  }
}
