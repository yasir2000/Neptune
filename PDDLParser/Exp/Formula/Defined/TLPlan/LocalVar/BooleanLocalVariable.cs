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
  /// A boolean local variable is a local variable bound to a boolean value.
  /// </summary>
  [TLPlan]
  public class BooleanLocalVariable : BooleanVariable, ILocalVariable
  {
    /// <summary>
    /// Creates a new boolean local variable with the specified name.
    /// </summary>
    /// <param name="name">The name of this boolean local variable.</param>
    public BooleanLocalVariable(string name)
      : base(name)
    {
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      Bool binding;
      if (bindings.TryGetBinding(this, out binding))
      {
        return new FuzzyBool(binding);
      }
      else
      {
        return FuzzyBool.Unknown;
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    [TLPlan]
    public override ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      Bool binding;
      if (bindings.TryGetBinding(this, out binding))
      {
        return new ShortCircuitFuzzyBool(binding);
      }
      else
      {
        return ShortCircuitFuzzyBool.Unknown;
      }
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// A local variable is simplified by retrieving its corresponding binding, if accessible.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      Bool value;
      if (bindings.TryGetBinding(this, out value))
      {
        return new LogicalValue(value);
      }
      else
      {
        return new LogicalValue(this);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return bindings.GetBinding(this);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    [TLPlan]
    public override ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ShortCircuitBool(bindings.GetBinding(this));
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      return this;
    }

    /// <summary>
    /// Returns true if this variable is free, false it is bound.
    /// A boolean local variable is bound.
    /// </summary>
    /// <returns>False.</returns>
    public override bool IsFree()
    {
      return false;
    }
  }
}
