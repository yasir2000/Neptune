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
// Please note that this file was inspired in part by the PDDL4J library:
// http://www.math-info.univ-paris5.fr/~pellier/software/software.php 
//
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp
{
  /// <summary>
  /// A logical expression yields a boolean value when evaluated.
  /// Evaluation in a closed world returns true, false, or undefined.
  /// Evaluation in an open world returns true, false, undefined, or unknown.
  /// </summary>
  public interface ILogicalExp : IConstraintExp, IEvaluableExp
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
    FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings);

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
    ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings);

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
    LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings);

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
    ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    [TLPlan]
    HashSet<PartialWorld> EnumerateAllSatisfyingWorlds();
  }
}
