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
using PDDLParser.Exception;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// An atomic formula application is an application of an atomic formula.
  /// It can be used both as a logical expression and as an effect.
  /// </summary>
  public class AtomicFormulaApplication : DescribedFormulaApplication, ILogicalExp, ILiteral, IComparable<AtomicFormulaApplication>
  {
    /// <summary>
    /// Creates a new atomic formula application of a specified atomic formula with a 
    /// given list of arguments.
    /// </summary>
    /// <param name="atomicFormula">The atomic formula to instantiate.</param>
    /// <param name="arguments">The arguments of this atomic formula application.</param>
    public AtomicFormulaApplication(AtomicFormula atomicFormula, List<ITerm> arguments)
      : base(atomicFormula, arguments)
    {
      System.Diagnostics.Debug.Assert(atomicFormula != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Gets the predicate associated with this literal.
    /// </summary>
    public AtomicFormula Predicate
    {
      get { return (AtomicFormula)this.m_rootFormula; }
    }

    /// <summary>
    /// Returns a copy of this formula application with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments of the new formula application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public override FormulaApplication Apply(List<ITerm> arguments)
    {
      return new AtomicFormulaApplication(this.Predicate, arguments);
    }

    /// <summary>
    /// Updates the specified world by setting to true this atomic formula application.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                       LocalBindings bindings, ActionContext actionContext)
    {
      FuzzyArgsEvalResult result = this.EvaluateArguments(evaluationWorld, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          updateWorld.Set((AtomicFormulaApplication)result.Value);
          break;
        case FuzzyArgsEvalResult.State.Undefined:
          throw new UndefinedExpException(this.ToString() + 
            " failed to update the world since at least one of its arguments evaluates to undefined.");
        case FuzzyArgsEvalResult.State.Unknown:
          throw new UnknownExpException(this.ToString() +
            " failed to update the world since at least one of its arguments evaluates to unknown.");
        default:
          throw new System.Exception("Invalid EvalStatus value: " + result.Status);
      }
    }

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      HashSet<DescribedFormula> formulas = new HashSet<DescribedFormula>();
      formulas.Add(this.Predicate);
      return formulas;
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// An atomic formula application is evaluated by first evaluating its arguments and then
    /// by retrieving the value of this application in the world.
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
          return world.IsSet((AtomicFormulaApplication)result.Value);
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
    /// An atomic formula application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    [TLPlan]
    public ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new ShortCircuitFuzzyBool(Evaluate(world, bindings));
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// An atomic formula application is simplified by first simplifying its arguments and then
    /// by attempting to retrieve its value in the specified world (if it is ground).
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
          AtomicFormulaApplication formula = (AtomicFormulaApplication)result.Value;
          if (!formula.AllConstantArguments)
          {
            return new LogicalValue(formula);
          }
          else
          {
            FuzzyBool value = world.IsSet(formula);
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
    /// An atomic formula application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
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
          return new Bool(world.IsSet((AtomicFormulaApplication)result.Value));
        case ArgsEvalResult.State.Undefined:
          return Bool.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// An atomic formula application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ShortCircuitBool(Evaluate(world, bindings));
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      HashSet<PartialWorld> list = new HashSet<PartialWorld>();
      PartialWorld world = new PartialWorld();
      world.Set(this);
      list.Add(world);
      return list;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// The progression is computed by evaluating the atomic formula application.
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

    #region IComparable<AtomicFormulaApplication> Members

    /// <summary>
    /// Compares two atomic formula applications.
    /// </summary>
    /// <param name="other">The other atomic formula application to compare this formula to.</param>
    /// <returns>An integer representing the total order relation between the two formulas.</returns>
    public int CompareTo(AtomicFormulaApplication other)
    {
      return this.CompareTo((IExp)other);
    }

    #endregion
  }
}
