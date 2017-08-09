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
using PDDLParser.Exp.Constraint;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical
{
  /// <summary>
  /// This class represents a disjunction of logical expressions.
  /// </summary>
  public class OrExp : AbstractOrExp<ILogicalExp>, ILogicalExp
  {
    /// <summary>
    /// Creates a new disjunction of logical expressions.
    /// </summary>
    /// <param name="exps">The logical expressions associated with the new disjunctive
    /// expression.</param>
    public OrExp(IEnumerable<ILogicalExp> exps)
      : base(exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new disjunction of logical expressions.
    /// </summary>
    /// <param name="exps">The logical expressions associated with the new disjunctive
    /// expression.</param>
    public OrExp(params ILogicalExp[] exps)
      : this((IEnumerable<ILogicalExp>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyBool value = FuzzyBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        value = value | exp.Evaluate(world, bindings);
        if (value)
          break;
      }
      return value;
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
    public ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ShortCircuitFuzzyBool value = ShortCircuitFuzzyBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        value = value | exp.EvaluateWithImmediateShortCircuit(world, bindings);
        if (value)
          break;
      }
      return value;
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      LogicalValue value = LogicalValue.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        value = value | exp.Simplify(world, bindings);
        if (value)
          break;
      }
      return value;
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      Bool value = Bool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        value = value | exp.Evaluate(world, bindings);
        if (value)
          break;
      }
      return value;
    }

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
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      ShortCircuitBool value = ShortCircuitBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        value = value | exp.EvaluateWithImmediateShortCircuit(world, bindings);
        if (value)
          break;
      }
      return value;
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// The worlds satisfying a disjunctive expression correspond to the union of the worlds
    /// satifying the individual logical expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      HashSet<PartialWorld> contexts = new HashSet<PartialWorld>();
      foreach (ILogicalExp exp in this.m_expressions)
      {
        contexts.UnionWith(exp.EnumerateAllSatisfyingWorlds());
      }
      return contexts;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This function returns false if this constraint expression is not satisfied 
    /// in the given world;
    /// it returns true if the progression is always satisfied in the next worlds;
    /// else it returns the progressed expression.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(this.Evaluate(world, bindings));
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
      return this.Evaluate(idleWorld, bindings);
    }
  }
}