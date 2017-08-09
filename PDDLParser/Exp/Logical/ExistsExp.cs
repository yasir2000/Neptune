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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical
{
  /// <summary>
  /// This class represents an existential quantification of a logical expression.
  /// </summary>
  public class ExistsExp : AbstractExistsExp<ILogicalExp>, ILogicalExp
  {
    /// <summary>
    /// Creates a new existential expression with the given quantified variables and
    /// logical expression.
    /// </summary>
    /// <param name="vars">The quantified variables.</param>
    /// <param name="exp">The logical expression.</param>
    public ExistsExp(HashSet<ObjectParameterVariable> vars, ILogicalExp exp)
      : base(vars, exp)
    {
      System.Diagnostics.Debug.Assert(exp != null && vars != null && !vars.ContainsNull());
    }

    /// <summary>
    /// Creates a new ground expression equivalent to this quantified expression.
    /// </summary>
    /// <returns>A new ground expression equivalent to this quantified expression.</returns>
    protected override ILogicalExp GenerateEquivalentExp()
    {
      return new OrExp(GetBodySubstitutions());
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// An existential expression is evaluated as the disjunction of all its variable substitutions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return this.GetEquivalentExp().Evaluate(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// An existential expression is evaluated as the disjunction of all its variable substitutions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    [TLPlan]
    public ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return this.GetEquivalentExp().EvaluateWithImmediateShortCircuit(world, bindings);
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// An existential expression is simplified as a disjunction of its yet unknown substitutions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return this.GetEquivalentExp().Simplify(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// An existential expression is evaluated as the disjunction of all its variable substitutions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return this.GetEquivalentExp().Evaluate(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// An existential expression is evaluated as the disjunction of all its variable substitutions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return this.GetEquivalentExp().EvaluateWithImmediateShortCircuit(world, bindings);
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      return this.GetEquivalentExp().EnumerateAllSatisfyingWorlds();
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