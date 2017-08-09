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
  /// Base class for negation expressions.
  /// </summary>
  /// <typeparam name="T">The type of the expression to negate.</typeparam>
  public abstract class AbstractNotExp<T> : AbstractExp
    where T : class, IExp
  {
    /// <summary>
    /// The expression to negate.
    /// </summary>
    protected T m_exp;

    /// <summary>
    /// Creates a new negative expression.
    /// </summary>
    /// <param name="exp">The expression to negate.</param>
    public AbstractNotExp(T exp)
      : base()
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_exp = exp;
    }

    /// <summary>
    /// Gets the negated expression.
    /// </summary>
    internal T Exp
    {
      get { return m_exp; }
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AbstractNotExp<T> other = (AbstractNotExp<T>)base.Apply(bindings);
      other.m_exp = (T)this.m_exp.Apply(bindings);

      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      AbstractNotExp<T> other = (AbstractNotExp<T>)base.Standardize(images);
      other.m_exp = (T)this.m_exp.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_exp.IsGround();
    }

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        AbstractNotExp<T> other = (AbstractNotExp<T>)obj;
        return this.m_exp.Equals(other.m_exp);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public override int GetHashCode()
    {
      return this.m_exp.GetHashCode();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return this.m_exp.GetFreeVariables();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(not ");
      str.Append(this.m_exp.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(not ");
      str.Append(this.m_exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);

      return (value != 0 ? value : this.m_exp.CompareTo(((AbstractNotExp<T>)other).m_exp));
    }

    #endregion
  }

  /// <summary>
  /// Represents the negation of a constraint expression.
  /// </summary>
  public class NotConstraintExp : AbstractNotExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new negative expression.
    /// </summary>
    /// <param name="exp">The constraint expression to negate.</param>
    public NotConstraintExp(IConstraintExp exp)
      : base(exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// The algorithm is: Progress(not formula1) => (not Progress(formula1))
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return ~m_exp.Progress(world, bindings);
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
      return ~m_exp.EvaluateIdle(idleWorld, bindings);
    }
  }
}
