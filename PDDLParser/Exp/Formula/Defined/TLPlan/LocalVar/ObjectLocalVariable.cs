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
using PDDLParser.Exp.Term;
using PDDLParser.Exp.Term.Type;
using PDDLParser.World;

namespace PDDLParser.Exp.Formula.TLPlan.LocalVar
{
  /// <summary>
  /// An object local variable is a local variable bound to an object (a constant from the domain).
  /// </summary>
  [TLPlan]
  public class ObjectLocalVariable : ObjectVariable, ILocalVariable
  {
    /// <summary>
    /// Creates a new object local variable with the specified name and typeset.
    /// </summary>
    /// <param name="name">The name of this object local variable.</param>
    /// <param name="typeSet">The typeset of this object local variable.</param>
    public ObjectLocalVariable(string name, TypeSet typeSet)
      : base(name, typeSet)
    {
      System.Diagnostics.Debug.Assert(typeSet != null);
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the constant resulting from the evaluation.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override FuzzyConstantExp Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ConstantExp binding;
      if (bindings.TryGetBinding(this, out binding))
      {
        return new FuzzyConstantExp(binding);
      }
      else
      {
        return FuzzyConstantExp.Unknown;
      }
    }

    /// <summary>
    /// Simplifies this term by evaluating its known expression parts.
    /// A local variable is simplified by retrieving its corresponding binding, if accessible.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the term (possibly a constant) resulting from 
    /// the simplification.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ConstantExp value;
      if (bindings.TryGetBinding(this, out value))
      {
        return new TermValue(value);
      }
      else
      {
        return new TermValue(this);
      }
    }

    /// <summary>
    /// Evaluates this term in the specified closed world.
    /// A local variable is evaluated by retrieving its binding in the provided set of bindings.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the constant resulting from the evaluation.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public override ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
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
    /// An object local variable is bound.
    /// </summary>
    /// <returns>False.</returns>
    public override bool IsFree()
    {
      return false;
    }
  }
}
