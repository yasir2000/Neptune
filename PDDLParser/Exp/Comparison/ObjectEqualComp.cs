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
using PDDLParser.Exp.Term;
using PDDLParser.World;

namespace PDDLParser.Exp.Comparison
{
  /// <summary>
  /// This class implements an object equality comparison.
  /// </summary>
  public class ObjectEqualComp : ComparisonExp<ITerm>
  {
    /// <summary>
    /// Creates a new object equality comparison.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    public ObjectEqualComp(ITerm arg1, ITerm arg2)
      : base("=", arg1, arg2)
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public override sealed FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return FuzzyConstantExp.ConstantEquals(m_arg1.Evaluate(world, bindings), 
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
      return new ShortCircuitFuzzyBool(FuzzyConstantExp.ConstantEquals(m_arg1.Evaluate(world, bindings), 
                                                                       m_arg2.Evaluate(world, bindings)));
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public override LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      TermValue term1 = m_arg1.Simplify(world, bindings);
      TermValue term2 = m_arg2.Simplify(world, bindings);

      if (term1.Status == TermValue.State.Undefined ||
          term2.Status == TermValue.State.Undefined)
      {
        return LogicalValue.Undefined;
      }
      else if (term1.Value is Constant && term2.Value is Constant)
      {
        return new LogicalValue(term1.Value.Equals(term2.Value));
      }
      else
      {
        return new LogicalValue(new ObjectEqualComp(term1.Value, term2.Value));
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public override Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return ConstantExp.ConstantEquals(m_arg1.Evaluate(world, bindings), 
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
    public override ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ShortCircuitBool(ConstantExp.ConstantEquals(m_arg1.Evaluate(world, bindings),
                                                             m_arg2.Evaluate(world, bindings)));
    }
  }
}
