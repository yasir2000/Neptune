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
using PDDLParser.Exp.Formula.TLPlan.LocalVar;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term.Type;

namespace PDDLParser.Exp.Term
{
  /// <summary>
  /// An object parameter variable is a parameter variable which must be assigned a constant value.
  /// </summary>
  public class ObjectParameterVariable : ObjectVariable, IParameterVariable, IComparable<ObjectParameterVariable>
  {
    /// <summary>
    /// Whether this parameter variable is free.
    /// </summary>
    private bool m_isFree;

    /// <summary>
    /// Creates a new object parameter variable with the specified name and typeset.
    /// </summary>
    /// <param name="name">The name of the new parameter variable.</param>
    /// <param name="typeSet">The typeset of the new parameter variable.</param>
    /// <param name="isFree">Whether the new parameter variable is free.</param>
    public ObjectParameterVariable(string name, TypeSet typeSet, bool isFree)
      : base(name, typeSet)
    {
      System.Diagnostics.Debug.Assert(typeSet != null);

      this.m_isFree = isFree;
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// An object parameter variable evaluates to the constant to which it is bound.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The constant to which this variable is bound.</returns>
    public override FuzzyConstantExp Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new FuzzyConstantExp(bindings.GetBinding(this));
    }

    /// <summary>
    /// Simplifies this term by evaluating its known expression parts.
    /// An object parameter variable simplifies to the constant to which it is bound.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The constant to which this variable is bound.</returns>
    public override TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new TermValue(bindings.GetBinding(this));
    }

    /// <summary>
    /// Evaluates this term in the specified closed world.
    /// An object parameter variable evaluates to the constant to which it is bound.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The constant to which this variable is bound.</returns>
    public override ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ConstantExp(bindings.GetBinding(this));
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      Constant binding;
      if (bindings.TryGetBinding(this, out binding))
      {
        return binding;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Returns true if this variable is free, false it is bound.
    /// </summary>
    /// <returns>Whether this variable is free.</returns>
    public override bool IsFree()
    {
      return this.m_isFree;
    }

    #region IComparable<ObjectParameterVariable> Members

    /// <summary>
    /// Compares this object parameter variable with another object parameter variable.
    /// </summary>
    /// <param name="other">The other variable to compare this variable to.</param>
    /// <returns>An integer representing the total order relation between the two variables.
    /// </returns>
    public int CompareTo(ObjectParameterVariable other)
    {
      return this.m_name.CompareTo(other.m_name);
    }

    #endregion
  }
}
