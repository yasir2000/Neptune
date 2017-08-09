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

namespace PDDLParser.Exp.Formula.TLPlan
{
  /// <summary>
  /// A defined predicate application is an application of a defined predicate.
  /// </summary>
  [TLPlan]
  public class DefinedPredicateApplication : DefinedFormulaApplication, ILogicalExp
  {
    /// <summary>
    /// Creates a new defined predicate application of a specified defined predicate with
    /// a given list of arguments.
    /// </summary>
    /// <param name="root">The defined predicate to instantiate.</param>
    /// <param name="arguments">The arguments of this defined predicate application.</param>
    public DefinedPredicateApplication(DefinedPredicate root, List<ITerm> arguments)
      : base(root, arguments)
    {
      System.Diagnostics.Debug.Assert(root != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Returns the defined predicate associated with this defined predicate application.
    /// </summary>
    /// <returns>The defined predicate associated with this defined predicate application.
    /// </returns>
    public DefinedPredicate getDefinedPredicate()
    {
      return (DefinedPredicate)this.m_rootFormula;
    }

    /// <summary>
    /// Returns a copy of this formula application with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments of the new formula application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public override FormulaApplication Apply(List<ITerm> arguments)
    {
      return new DefinedPredicateApplication(this.getDefinedPredicate(), arguments);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// A defined predicate application is evaluated by first evaluating its arguments 
    /// and then evaluating the corresponding defined predicate's body with the appropriate
    /// bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          return ((DefinedPredicate)this.m_rootFormula).Evaluate(world, (DefinedPredicateApplication)result.Value);
        case FuzzyArgsEvalResult.State.Unknown:
          return FuzzyBool.Unknown;
        case FuzzyArgsEvalResult.State.Undefined:
          return FuzzyBool.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// A defined predicate application is evaluated by first evaluating its arguments 
    /// and then evaluating the corresponding defined predicate's body with the appropriate
    /// bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    [TLPlan]
    public ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          return new ShortCircuitFuzzyBool(((DefinedPredicate)this.m_rootFormula).Evaluate(world, (DefinedPredicateApplication)result.Value));
        case FuzzyArgsEvalResult.State.Unknown:
          return ShortCircuitFuzzyBool.Unknown;
        case FuzzyArgsEvalResult.State.Undefined:
          return ShortCircuitFuzzyBool.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// A defined predicate application is simplified by first simplifying its arguments
    /// and then by attempting to evaluate the corresponding defined predicate's body (if all
    /// its arguments are ground).
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.SimplifyArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          DefinedPredicateApplication formula = (DefinedPredicateApplication)result.Value;
          if (!formula.AllConstantArguments)
          {
            return new LogicalValue(formula);
          }
          else
          {
            FuzzyBool value = ((DefinedPredicate)this.m_rootFormula).Evaluate(world, formula);
            return (value == FuzzyBool.Unknown) ? new LogicalValue(formula) :
                                                  new LogicalValue(value.ToBoolValue());
          }
        case ArgsEvalResult.State.Undefined:
          return LogicalValue.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// A defined predicate application is evaluated by first evaluating its arguments 
    /// and then evaluating the corresponding defined predicate's body with the appropriate
    /// bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          return ((DefinedPredicate)this.m_rootFormula).Evaluate(world,
            (DefinedPredicateApplication)result.Value);
        case ArgsEvalResult.State.Undefined:
          return Bool.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// A defined predicate application is evaluated by first evaluating its arguments 
    /// and then evaluating the corresponding defined predicate's body with the appropriate
    /// bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          return new ShortCircuitBool(((DefinedPredicate)this.m_rootFormula).Evaluate(world, (DefinedPredicateApplication)result.Value));
        case ArgsEvalResult.State.Undefined:
          return ShortCircuitBool.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown
    /// since this function is not supported.</exception>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return Evaluate(idleWorld, bindings);
    }
  }
}