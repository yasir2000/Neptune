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

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents a constraint expression which is converted into an equivalent
  /// form using other constraint expressions.
  /// </summary>
  public abstract class DerivedConstraintExp : AbstractConstraintExp
  {
    /// <summary>
    /// The equivalent constraint expression.
    /// </summary>
    private IConstraintExp m_equivalentExp;

    /// <summary>
    /// Creates a new derived constraint expression.
    /// </summary>
    public DerivedConstraintExp()
    {
      // The equivalent expression must be generated only when needed as some more processing
      // may be performed after the creation of the object (e.g. quantified expressions expansion)
      this.m_equivalentExp = null;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return GetEquivalentExp().Progress(world, bindings);
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
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return GetEquivalentExp().EvaluateIdle(idleWorld, bindings);
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return GetEquivalentExp().IsGround();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return GetEquivalentExp().GetFreeVariables();
    }

    /// <summary>
    /// Returns the equivalent compound constraint expression of this expression.
    /// </summary>
    /// <remarks>
    /// If the equivalent expression has not yet been generated, it is on the first call.
    /// The equivalent expression must be generated only when needed as some more processing
    /// may be performed after the creation of this expression (e.g. quantified expressions expansion)
    /// </remarks>
    /// <returns>An equivalent constraint expression to this expression.</returns>
    private IConstraintExp GetEquivalentExp()
    {
      if (m_equivalentExp == null)
        m_equivalentExp = GenerateEquivalentExp();

      return m_equivalentExp;
    }

    /// <summary>
    /// Generates and returns the compound constraint expression equivalent to this expression.
    /// </summary>
    /// <returns>An equivalent constraint expression to this expression.</returns>
    public abstract IConstraintExp GenerateEquivalentExp();

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public abstract override bool Equals(object obj);

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public abstract override int GetHashCode();
  }
}
