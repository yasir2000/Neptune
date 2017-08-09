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
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Formula.TLPlan.LocalVar
{
  /// <summary>
  /// A numeric local variable is a local variable bound to a numeric value.
  /// </summary>
  [TLPlan]
  public class NumericLocalVariable : NumericVariable, ILocalVariable
  {
    /// <summary>
    /// Creates a new numeric local variable with the specified name.
    /// </summary>
    /// <param name="name">The name of this numeric local variable.</param>
    public NumericLocalVariable(string name)
      : base(name)
    {
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified open world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the resulting numeric value.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      Double binding;
      if (bindings.TryGetBinding(this, out binding))
      {
        return new FuzzyDouble(binding);
      }
      else
      {
        return FuzzyDouble.Unknown;
      }
    }

    /// <summary>
    /// Simplifies this numeric expression by evaluating its known expression parts.
    /// A local variable is simplified by retrieving its corresponding binding, if accessible.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, a numeric value, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      Double value;
      if (bindings.TryGetBinding(this, out value))
      {
        return new NumericValue(value);
      }
      else
      {
        return new NumericValue(this);
      }
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified closed world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return bindings.GetBinding(this);
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
    /// A numeric local variable is bound.
    /// </summary>
    /// <returns>False.</returns>
    public override bool IsFree()
    {
      return false;
    }
  }
}
