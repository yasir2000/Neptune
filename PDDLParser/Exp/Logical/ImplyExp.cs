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
using PDDLParser.World;

namespace PDDLParser.Exp.Logical
{
  /// <summary>
  /// This class represents an implication expression.
  /// </summary>
  public class ImplyExp : AbstractImplyExp<ILogicalExp>, ILogicalExp
  {
    /// <summary>
    /// Creates a new implication expression with the specified antecedent and consequent.
    /// </summary>
    /// <param name="antecedent">The antecedent of the implication.</param>
    /// <param name="consequent">The consequent of the implication.</param>
    public ImplyExp(ILogicalExp antecedent, ILogicalExp consequent)
      : base(antecedent, consequent)
    {
      System.Diagnostics.Debug.Assert(antecedent != null && consequent != null);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return ~m_antecedent.Evaluate(world, bindings) || m_consequent.Evaluate(world, bindings);
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
      return ~m_antecedent.EvaluateWithImmediateShortCircuit(world, bindings) || m_consequent.EvaluateWithImmediateShortCircuit(world, bindings);
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return ~m_antecedent.Simplify(world, bindings) || m_consequent.Simplify(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return ~m_antecedent.Evaluate(world, bindings) || m_consequent.Evaluate(world, bindings);
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
      return ~m_antecedent.EvaluateWithImmediateShortCircuit(world, bindings) || m_consequent.EvaluateWithImmediateShortCircuit(world, bindings);
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      return GenerateEquivalentExp().EnumerateAllSatisfyingWorlds();
    }

    /// <summary>
    /// Creates a new ground expression equivalent to this implication expression.
    /// </summary>
    /// <returns>A new ground expression equivalent to this implication expression.</returns>
    private ILogicalExp GenerateEquivalentExp()
    {
      return new OrExp(new NotExp(this.m_antecedent), this.m_consequent);
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