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
using PDDLParser.World;

namespace PDDLParser.Exp.Formula.TLPlan.LocalVar
{
  /// <summary>
  /// A boolean variable is a variable bound to a boolean value.
  /// </summary>
  [TLPlan]
  public abstract class BooleanVariable : Variable, ILogicalExp
  {
    /// <summary>
    /// Creates a new boolean variable with the specified name.
    /// </summary>
    /// <param name="name">The name of the boolean variable.</param>
    public BooleanVariable(string name)
      : base(name)
    {
    }

    #region ILogicalExp Members

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public abstract FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// The bindings should not be modified by this call.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// This function is used to evaluate defined formulas' body as is done in the
    /// original TLPlan.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    [TLPlan]
    public abstract ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// The bindings should not be modified by this call.
    /// The resulting expression should not contain any unbound variables, since
    /// they are substituted according to the bindings supplied.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public abstract LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public abstract Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings);

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
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    [TLPlan]
    public abstract ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// This method is not supported.
    /// </summary>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since
    /// this method is not supported.</exception>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      return this.ToString();
    }

    #endregion

    #region IConstraintExp Members

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
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return Evaluate(idleWorld, bindings);
    }

    #endregion
  }
}
