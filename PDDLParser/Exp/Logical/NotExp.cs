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
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical
{
  /// <summary>
  /// This class represents the negation of an expression.
  /// </summary>
  public class NotExp : AbstractNotExp<ILogicalExp>, ILogicalExp
  {
    /// <summary>
    /// Creates a new negative expression.
    /// </summary>
    /// <param name="exp">The expression to negate.</param>
    public NotExp(ILogicalExp exp)
      : base(exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);
    }


    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return ~m_exp.Evaluate(world, bindings);
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
      return ~m_exp.EvaluateWithImmediateShortCircuit(world, bindings);
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return ~m_exp.Simplify(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return ~m_exp.Evaluate(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return ~m_exp.EvaluateWithImmediateShortCircuit(world, bindings);
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// The worlds satisfying a negative expression correspond to the complement of the set of 
    /// worlds satisfying the negated expression.
    /// </summary>
    /// <returns>All worlds except the ones satisfying the negated expression.</returns>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      HashSet<PartialWorld> worlds = new HashSet<PartialWorld>();
      HashSet<PartialWorld> badWorlds = m_exp.EnumerateAllSatisfyingWorlds();
      PartialWorld currentWorld = new PartialWorld();

      if (badWorlds.Count == 0)
      {
        worlds.Add(currentWorld);
        return worlds;
      }

      HashSet<AtomicFormulaApplication> allPredicates = new HashSet<AtomicFormulaApplication>();
      foreach (PartialWorld badWorld in badWorlds)
      {
        allPredicates.UnionWith(badWorld.GetAllPredicates());
      }
      foreach (AtomicFormulaApplication predicate in allPredicates)
      {
        currentWorld.Set(predicate);
      }

      int lastIndex = allPredicates.Count - 1;
      bool hasNext;
      do
      {
        if (!badWorlds.Contains(currentWorld))
          worlds.Add((PartialWorld)currentWorld.Clone());

        hasNext = false;
        foreach (AtomicFormulaApplication predicate in allPredicates)
        {
          bool value = currentWorld.IsSet(predicate).ToBool();
          if (value)
          {
            currentWorld.Unset(predicate);
            hasNext = true;
          }
          else
          {
            currentWorld.Set(predicate);
          }
        }
      } while (hasNext);

      return worlds;
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
