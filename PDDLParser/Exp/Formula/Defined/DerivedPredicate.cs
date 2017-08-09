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
using System.Runtime.Serialization;
using PDDLParser.Exception;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A derived predicate is a special defined predicate present in the PDDL specification.
  /// There are two differences between a derived predicate and a TLPlan defined predicate:
  /// 1- A derived predicate is evaluated by performing a regular evaluation of its body
  /// (a first order formula), as opposed to the evaluation of a defined predicate's body,
  /// which is done using shortcircuits.
  /// 2- A derived predicate cannot use local variables.
  /// </summary>
  public class DerivedPredicate : DefinedPredicate
  {
    /// <summary>
    /// Creates a new derived predicate with a specified name and list of arguments.
    /// </summary>
    /// <param name="name">The name of this derived predicate.</param>
    /// <param name="arguments">The arguments of this derived predicate.</param>
    public DerivedPredicate(string name, List<ObjectParameterVariable> arguments)
      : base(":derived", name, arguments, new List<ILocalVariable>())
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Evaluates this derived predicate in the specified open world.
    /// A derived predicate is evaluated by binding its body's free variables to the
    /// corresponding arguments of the function application, and then performing a 
    /// regular evaluation of its body.
    /// The defined predicate's return value corresponds to the boolean value obtained
    /// when evaluating its body.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="formula">A function application with ground (constant) arguments.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public override FuzzyBool Evaluate(IReadOnlyOpenWorld world, DefinedPredicateApplication formula)
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
      FuzzyBool result = Body.Evaluate(world, new LocalBindings(bindings));

      evaluation.Result = result.Value;
      evaluation.Finished = true;

      return result;
    }

    /// <summary>
    /// Evaluates this derived predicate in the specified closed world.
    /// A derived predicate is evaluated by binding its body's free variables to the
    /// corresponding arguments of the function application, and then performing a 
    /// regular evaluation of its body.
    /// The defined predicate's return value corresponds to the boolean value obtained
    /// when evaluating its body.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="formula">A function application with ground (constant) arguments.</param>
    /// <returns>True, false, or undefined.</returns>
    public override Bool Evaluate(IReadOnlyClosedWorld world, DefinedPredicateApplication formula)
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
      Bool result = Body.Evaluate(world, new LocalBindings(bindings));

      evaluation.Result = result.Value;
      evaluation.Finished = true;

      return result;
    }
  }
}
