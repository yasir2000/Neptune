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
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Base class for disjunction of expressions.
  /// </summary>
  /// <typeparam name="T">The type of the expressions.</typeparam>
  public abstract class AbstractOrExp<T> : ListExp<T>
    where T : class, IExp
  {
    /// <summary>
    /// Creates a new disjunction of expressions.
    /// </summary>
    /// <param name="exps">The expressions associated with the new disjunctive
    /// expression.</param>
    public AbstractOrExp(IEnumerable<T> exps)
      : base("or", exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new disjunction of expressions.
    /// </summary>
    /// <param name="exps">The expressions associated with the new disjunctive
    /// expression.</param>
    public AbstractOrExp(params T[] exps)
      : this((IEnumerable<T>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Adds a new expression to this disjunctive expression.
    /// If the expression to add is also a disjunctive expression, its elements are recursively
    /// added to this top-level disjunctive expression.
    /// Note that this method is protected and should be called only in the constructor.
    /// </summary>
    /// <param name="elt">The new expression to add to this list expression.</param>
    protected override void AddElement(T elt)
    {
      if (elt.GetType() == this.GetType())
      {
        AbstractOrExp<T> listExp = (AbstractOrExp<T>)(object)elt;
        foreach (T exp in listExp)
        {
          this.AddElement(exp);
        }
      }
      else
      {
        base.AddElement(elt);
      }
    }
  }

  /// <summary>
  /// Represents a disjunction of constraint expressions.
  /// </summary>
  public class OrConstraintExp : AbstractOrExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new disjunction of constraint expressions.
    /// </summary>
    /// <param name="exps">The constraint expressions associated with the new disjunctive
    /// expression.</param>
    public OrConstraintExp(IEnumerable<IConstraintExp> exps)
      : base(exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new disjunction of constraint expressions.
    /// </summary>
    /// <param name="exps">The constraint expressions associated with the new disjunctive
    /// expression.</param>
    public OrConstraintExp(params IConstraintExp[] exps)
      : this((IEnumerable<IConstraintExp>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// The algorithm is: Progress(or formula1 ...) => (or Progress(formula1) ...)
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      ProgressionValue result = ProgressionValue.False;
      foreach (IConstraintExp exp in this.m_expressions)
      {
        result = result | exp.Progress(world, bindings);
        if (result)
          break;
      }
      return result;
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
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      Bool value = Bool.False;
      foreach (IConstraintExp exp in this.m_expressions)
      {
        value = value | exp.EvaluateIdle(idleWorld, bindings);
        if (value)
          break;
      }
      return value;
    }
  }
}
