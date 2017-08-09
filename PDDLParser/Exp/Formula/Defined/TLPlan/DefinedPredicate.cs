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
using PDDLParser.Extensions;
using PDDLParser.Exception;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Formula.TLPlan
{
  /// <summary>
  /// A defined predicate is a defined formula which yields a boolean value when evaluated.
  /// </summary>
  [TLPlan]
  public class DefinedPredicate : DefinedFormula
  {
    /// <summary>
    /// Creates a new defined name with the specified predicate, arguments, and
    /// local variables.
    /// </summary>
    /// <param name="name">The name of this defined predicate.</param>
    /// <param name="arguments">The arguments of this defined predicate.</param>
    /// <param name="localVariables">The local variables to use in the body of this
    /// defined predicate.</param>
    public DefinedPredicate(string name, List<ObjectParameterVariable> arguments,
                            List<ILocalVariable> localVariables)
      : this(":defined-predicate", name, arguments, localVariables)
    {
    }

    /// <summary>
    /// Creates a new defined name with the specified predicate, arguments, and
    /// local variables.
    /// </summary>
    /// <param name="image">The image of this defined predicate.</param>
    /// <param name="name">The name of this defined predicate.</param>
    /// <param name="arguments">The arguments of this defined predicate.</param>
    /// <param name="localVariables">The local variables to use in the body of this
    /// defined predicate.</param>
    protected DefinedPredicate(string image, string name, List<ObjectParameterVariable> arguments,
                            List<ILocalVariable> localVariables)
      : base(image, name, arguments, localVariables)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull()
                                   && localVariables != null && !localVariables.ContainsNull());
    }

    /// <summary>
    /// Evaluates this defined predicate in the specified open world.
    /// A defined predicate is evaluated by binding its body's free variables to the
    /// corresponding arguments of the function application, and then evaluating its
    /// body using immediate short-circuit.
    /// The defined predicate's return value corresponds to the boolean value obtained
    /// when evaluating its body.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="formula">A function application with ground (constant) arguments.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public virtual FuzzyBool Evaluate(IReadOnlyOpenWorld world, DefinedPredicateApplication formula)
    {
      bool existing;
      IEvaluationRecord<FuzzyBoolValue> evaluation = world.GetEvaluation(formula, out existing);

      if (existing)
      {
        if (evaluation.Finished)
        {
          // Evaluation result is cached; return it.
          // IMPORTANT HYPOTHESIS: evaluation is reentrant and causes no side effect!
          return new FuzzyBool(evaluation.Result);
        }
        else
        {
          // A cycle is detected; the derived predicate is thus incorrectly defined (domain bug).
          evaluation.Finished = true;
          throw new CycleException(formula);
        }
      }
      ParameterBindings bindings = GetParameterBindings(formula.GetArguments().Cast<Constant>());
      ShortCircuitFuzzyBool result = Body.EvaluateWithImmediateShortCircuit(world, new LocalBindings(bindings));

      evaluation.Result = result.Value;
      evaluation.Finished = true;

      return new FuzzyBool(evaluation.Result);
    }

    /// <summary>
    /// Evaluates this defined predicate in the specified closed world.
    /// A defined predicate is evaluated by binding its body's free variables to the
    /// corresponding arguments of the function application, and then evaluating its
    /// body using immediate short-circuit.
    /// The defined predicate's return value corresponds to the boolean value obtained
    /// when evaluating its body.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="formula">A function application with ground (constant) arguments.</param>
    /// <returns>True, false, or undefined.</returns>
    public virtual Bool Evaluate(IReadOnlyClosedWorld world, DefinedPredicateApplication formula)
    {
      bool existing;
      IEvaluationRecord<BoolValue> evaluation = world.GetEvaluation(formula, out existing);

      if (existing)
      {
        if (evaluation.Finished)
        {
          // Evaluation result is cached; return it.
          // IMPORTANT HYPOTHESIS: evaluation is reentrant and causes no side effect!
          return new Bool(evaluation.Result);
        }
        else
        {
          // A cycle is detected; the derived predicate is thus incorrectly defined (domain bug).
          evaluation.Finished = true;
          throw new CycleException(formula);
        }
      }
      ParameterBindings bindings = GetParameterBindings(formula.GetArguments().Cast<Constant>());
      ShortCircuitBool result = Body.EvaluateWithImmediateShortCircuit(world, new LocalBindings(bindings));

      evaluation.Result = result.Value;
      evaluation.Finished = true;

      return new Bool(evaluation.Result);
    }

    /// <summary>
    /// Instantiates a formula application associated with this defined predicate.
    /// </summary>
    /// <param name="arguments">Arguments of the formula application to instantiate.</param>
    /// <returns>A new formula application associated with this defined predicate.</returns>
    public override FormulaApplication Instantiate(List<ITerm> arguments)
    {
      return new DefinedPredicateApplication(this, arguments);
    }
  }
}
