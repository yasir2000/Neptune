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
using PDDLParser.World;
using PDDLParser.Exp.Struct;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Comparison
{
  /// <summary>
  /// This class represents a comparison between two numeric expressions.
  /// </summary>
  public abstract class NumericCompExp : ComparisonExp<INumericExp>
  {
    /// <summary>
    /// Creates a new numeric comparison expression with the two arguments.
    /// </summary>
    /// <param name="op">A string representation of this operator.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    public NumericCompExp(string op, INumericExp arg1, INumericExp arg2)
      : base(op, arg1, arg2)
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);
    }

    /// <summary>
    /// Compares the two arguments. This comparison should return undefined if at least
    /// one of the two arguments is undefined.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>True, false, or undefined.</returns>
    protected abstract Bool Compare(Double arg1, Double arg2);

    /// <summary>
    /// Compares the two arguments. This comparison should return undefined if at least one 
    /// of the two arguments is undefined, or unknown if at least one of the two arguments is 
    /// unknown.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    protected abstract FuzzyBool Compare(FuzzyDouble arg1, FuzzyDouble arg2);

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public override sealed FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return Compare(m_arg1.Evaluate(world, bindings), 
                     m_arg2.Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    [TLPlan]
    public override sealed ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new ShortCircuitFuzzyBool(Compare(m_arg1.Evaluate(world, bindings), 
                                               m_arg2.Evaluate(world, bindings)));
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public override LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      NumericValue exp1 = m_arg1.Simplify(world, bindings);
      NumericValue exp2 = m_arg2.Simplify(world, bindings);

      if (exp1.Exp == null && exp2.Exp == null)
      {
        // Both arguments were simplified to numeric values.
        return new LogicalValue(Compare(exp1.Value, exp2.Value));
      }
      else if ((exp1.Exp == null && exp1.Value.Status == Double.State.Undefined) ||
               (exp2.Exp == null && exp2.Value.Status == Double.State.Undefined))
      {
        // At least one of the two arguments was simplified to undefined.
        return LogicalValue.Undefined;
      }
      else
      {
        // At least one of the two arguments was simplified to an expression.
        NumericCompExp clone = (NumericCompExp)this.Clone();
        clone.m_arg1 = exp1.GetEquivalentExp();
        clone.m_arg2 = exp2.GetEquivalentExp();

        return new LogicalValue(clone);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public override sealed Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return Compare(m_arg1.Evaluate(world, bindings), 
                     m_arg2.Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public override sealed ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ShortCircuitBool(Compare(m_arg1.Evaluate(world, bindings),
                                          m_arg2.Evaluate(world, bindings)));
    }
  }
}
