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
using PDDLParser.Exp.Formula.TLPlan.LocalVar;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Formula.TLPlan
{
  /// <summary>
  /// A defined numeric functions is a defined function which yields a numeric value when
  /// evaluated.
  /// </summary>
  [TLPlan]
  public class DefinedNumericFunction : DefinedFunction
  {
    /// <summary>
    /// The local variable corresponding to this function's return value.
    /// </summary>
    protected NumericLocalVariable m_functionVariable;

    /// <summary>
    /// Creates a new defined numeric function with the specified name, arguments,
    /// and local variables.
    /// </summary>
    /// <param name="name">The name of this defined numeric function.</param>
    /// <param name="arguments">The arguments of this defined numeric function.</param>
    /// <param name="localVariables">The local variables used in the body of this
    /// defined numeric function.</param>
    public DefinedNumericFunction(string name, List<ObjectParameterVariable> arguments,
                            List<ILocalVariable> localVariables)
      : base(":defined-function", name, arguments, localVariables)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull()
                                   && localVariables != null && !localVariables.ContainsNull());

      this.m_functionVariable = new NumericLocalVariable(name);
    }

    /// <summary>
    /// Gets the local variable corresponding to the function's return value.
    /// </summary>
    public override ILocalVariable FunctionVariable
    {
      get { return this.m_functionVariable; }
    }

    /// <summary>
    /// Evaluates this defined numeric function in the specified open world.
    /// A defined numeric function is evaluated by binding its body's free variables to the
    /// corresponding arguments of the function application, and then evaluating its
    /// body using immediate short-circuit.
    /// If its body evaluates to unknown, the evaluation returns unknown.
    /// If its body evaluates to undefined, the evaluation returns undefined.
    /// Else the evaluation returns the final binding of the function variable.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="formula">A function application with ground (constant) arguments.</param>
    /// <returns>Undefined, unknown, or the resulting numeric value.</returns>
    public FuzzyDouble Evaluate(IReadOnlyOpenWorld world, DefinedNumericFunctionApplication formula)
    {
      bool existing;
      IEvaluationRecord<FuzzyDouble> evaluation = world.GetEvaluation(formula, out existing);

      if (existing)
      {
        if (evaluation.Finished)
        {
          // Evaluation result is cached; return it.
          // IMPORTANT HYPOTHESIS: evaluation is reentrant and causes no side effect!
          return evaluation.Result;
        }
        else
        {
          // A cycle is detected; the derived predicate is thus incorrectly defined (domain bug).
          evaluation.Finished = true;
          throw new CycleException(formula);
        }
      }

      ParameterBindings bindings = GetParameterBindings(formula.GetArguments().Cast<Constant>());
      LocalBindings localBindings = new LocalBindings(bindings);

      ShortCircuitFuzzyBool bodyResult = Body.EvaluateWithImmediateShortCircuit(world, localBindings);
      evaluation.Result = (bodyResult == ShortCircuitFuzzyBool.Unknown) ?
        FuzzyDouble.Unknown :
        new FuzzyDouble(localBindings.GetBinding((NumericLocalVariable)this.m_functionVariable));
      evaluation.Finished = true;

      return evaluation.Result;
    }

    /// <summary>
    /// Evaluates this defined numeric function in the specified closed world.
    /// A defined numeric function is evaluated by binding its body's free variables to the
    /// corresponding arguments of the function application, and then evaluating its
    /// body using immediate short-circuit.
    /// If its body evaluates to undefined, the evaluation returns undefined.
    /// Else the evaluation returns the final binding of the function variable.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="formula">A function application with ground (constant) arguments.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    public Double Evaluate(IReadOnlyClosedWorld world, DefinedNumericFunctionApplication formula)
    {
      bool existing;
      IEvaluationRecord<Double> evaluation = world.GetEvaluation(formula, out existing);

      if (existing)
      {
        if (evaluation.Finished)
        {
          // Evaluation result is cached; return it.
          // IMPORTANT HYPOTHESIS: evaluation is reentrant and causes no side effect!
          return evaluation.Result;
        }
        else
        {
          // A cycle is detected; the derived predicate is thus incorrectly defined (domain bug).
          evaluation.Finished = true;
          throw new CycleException(formula);
        }
      }

      ParameterBindings bindings = GetParameterBindings(formula.GetArguments().Cast<Constant>());
      LocalBindings localBindings = new LocalBindings(bindings);

      Body.EvaluateWithImmediateShortCircuit(world, localBindings);
      Double result = localBindings.GetBinding((NumericLocalVariable)this.m_functionVariable);

      evaluation.Result = result;
      evaluation.Finished = true;

      return result;
    }

    /// <summary>
    /// Instantiates a formula application associated with this defined numeric function.
    /// </summary>
    /// <param name="arguments">Arguments of the formula application to instantiate.</param>
    /// <returns>A new formula application associated with this defined numeric function.</returns>
    public override FormulaApplication Instantiate(List<ITerm> arguments)
    {
      return new DefinedNumericFunctionApplication(this, arguments);
    }

    /// <summary>
    /// Returns a typed string representation of this formula.
    /// </summary>
    /// <returns>A typed string representation of this formula.</returns>
    public override string ToTypedString()
    {
      return base.ToTypedString() + " - Number";
    }
  }
}
