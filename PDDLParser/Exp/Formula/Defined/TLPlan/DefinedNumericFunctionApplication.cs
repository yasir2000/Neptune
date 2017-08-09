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
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Formula.TLPlan
{
  /// <summary>
  /// A defined numeric function application is an application of a defined numeric function.
  /// </summary>
  [TLPlan]
  public class DefinedNumericFunctionApplication : DefinedFormulaApplication, INumericExp
  {
    /// <summary>
    /// Creates a new defined numeric function application of a specified defined numeric function
    /// with a given list of arguments.
    /// </summary>
    /// <param name="numericFunction">The defined numeric function to instantiate.</param>
    /// <param name="arguments">The arguments of the new defined numeric function.</param>
    public DefinedNumericFunctionApplication(DefinedNumericFunction numericFunction, List<ITerm> arguments)
      : base(numericFunction, arguments)
    {
      System.Diagnostics.Debug.Assert(numericFunction != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Returns the defined numeric function corresponding to this function application.
    /// </summary>
    /// <returns>The defined numeric function corresponding to this function application.</returns>
    public DefinedNumericFunction RootFunction
    {
      get { return (DefinedNumericFunction)this.m_rootFormula; }
    }

    /// <summary>
    /// Returns a copy of this formula application with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments of the new formula application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public override FormulaApplication Apply(List<ITerm> arguments)
    {
      return new DefinedNumericFunctionApplication(this.RootFunction, arguments);
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified open world.
    /// A defined numeric function application is evaluated by first evaluating its arguments
    /// and then evaluating the corresponding defined function's body with the appropriate bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the resulting numeric value.</returns>
    public FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          return ((DefinedNumericFunction)this.m_rootFormula).Evaluate(world, (DefinedNumericFunctionApplication)result.Value);
        case FuzzyArgsEvalResult.State.Unknown:
          return FuzzyDouble.Unknown;
        case FuzzyArgsEvalResult.State.Undefined:
          return FuzzyDouble.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Simplifies this numeric expression by evaluating its known expression parts.
    /// A defined numeric function application is simplified by first simplifying its arguments 
    /// and then by attempting to evaluate the corresponding defined function's body (if all
    /// its arguments are ground).
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, a numeric value, or the simplified expression.</returns>
    public NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.SimplifyArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          DefinedNumericFunctionApplication formula = (DefinedNumericFunctionApplication)result.Value;
          if (!formula.AllConstantArguments)
          {
            return new NumericValue(formula);
          }
          else
          {
            FuzzyDouble value = ((DefinedNumericFunction)this.m_rootFormula).Evaluate(world, formula);

            return (value.Status == FuzzyDouble.State.Unknown) ?
              new NumericValue(formula) :
              new NumericValue(value.ToDoubleValue());
          }
        case ArgsEvalResult.State.Undefined:
          return NumericValue.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified closed world.
    /// A defined numeric function application is evaluated by first evaluating its arguments
    /// and then evaluating the corresponding defined function's body with the appropriate bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    public Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          return ((DefinedNumericFunction)this.m_rootFormula).Evaluate(world, (DefinedNumericFunctionApplication)result.Value);
        case ArgsEvalResult.State.Undefined:
          return Double.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }
  }
}
