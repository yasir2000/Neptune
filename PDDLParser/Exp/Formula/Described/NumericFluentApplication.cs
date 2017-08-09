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

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A numeric fluent application is an application of a numeric fluent.
  /// Note that a numeric fluent application is undefined until its value has been set.
  /// </summary>
  public class NumericFluentApplication : FluentApplication, INumericExp, IComparable<NumericFluentApplication>
  {
    /// <summary>
    /// Creates a new numeric fluent application of a specified numeric fluent and a given
    /// list of arguments.
    /// </summary>
    /// <param name="fluent">The numeric fluent to instantiate.</param>
    /// <param name="arguments">The arguments of the new numeric fluent application.</param>
    public NumericFluentApplication(NumericFluent fluent, List<ITerm> arguments)
      : base(fluent, arguments)
    {
      System.Diagnostics.Debug.Assert(fluent != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Gets the numeric fluent associated with this numeric fluent application.
    /// </summary>
    private NumericFluent RootNumericFluent
    {
      get { return (NumericFluent)this.m_rootFormula; }
    }

    /// <summary>
    /// Returns a copy of this numeric fluent application with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments of the new fluent application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public override FormulaApplication Apply(List<ITerm> arguments)
    {
      return new NumericFluentApplication(this.RootNumericFluent, arguments);
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified open world.
    /// A numeric fluent application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
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
          return world.GetNumericFluent((NumericFluentApplication)result.Value);
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
    /// A numeric fluent application is simplified by first simplifying its arguments and then
    /// by attempting to retrieve its value in the specified world (if it is ground).
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
          NumericFluentApplication formula = (NumericFluentApplication)result.Value;
          if (!formula.AllConstantArguments)
          {
            return new NumericValue(formula);
          }
          else
          {
            FuzzyDouble value = world.GetNumericFluent(formula);
            if (value.Status == FuzzyDouble.State.Unknown)
            {
              return new NumericValue(formula);
            }
            else
            {
              return new NumericValue(value.ToDoubleValue());
            }
          }
        case ArgsEvalResult.State.Undefined:
          return NumericValue.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified closed world.
    /// A numeric fluent application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
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
          return world.GetNumericFluent((NumericFluentApplication)result.Value);
        case ArgsEvalResult.State.Undefined:
          return Double.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    #region IComparable<NumericFluentApplication> Members

     /// <summary>
     /// Compares two numeric fluent applications.
     /// </summary>
     /// <param name="other">The other numeric fluent application to compare this formula to.</param>
     /// <returns>An integer representing the total order relation between the two formulas.</returns>
    public int CompareTo(NumericFluentApplication other)
    {
      return this.CompareTo((IExp)other);
    }

    #endregion
  }
}
