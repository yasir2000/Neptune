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
  /// Base class for implication expressions.
  /// </summary>
  /// <typeparam name="T">The type of the expressions.</typeparam>
  public abstract class AbstractImplyExp<T> : AbstractExp
    where T : class, IExp
  {
    /// <summary>
    /// The antecedent of the implication expression.
    /// </summary>
    protected T m_antecedent;

    /// <summary>
    /// The consequent of the implication expression.
    /// </summary>
    protected T m_consequent;

    /// <summary>
    /// Creates a new implication expression with the specified antecedent and consequent.
    /// </summary>
    /// <param name="antecedent">The antecedent of the implication.</param>
    /// <param name="consequent">The consequent of the implication.</param>
    public AbstractImplyExp(T antecedent, T consequent)
      : base()
    {
      System.Diagnostics.Debug.Assert(antecedent != null && consequent != null);

      this.m_antecedent = antecedent;
      this.m_consequent = consequent;
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AbstractImplyExp<T> other = (AbstractImplyExp<T>)base.Apply(bindings);
      other.m_antecedent = (T)this.m_antecedent.Apply(bindings);
      other.m_consequent = (T)this.m_consequent.Apply(bindings);

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
      AbstractImplyExp<T> other = (AbstractImplyExp<T>)base.Standardize(images);
      other.m_antecedent = (T)this.m_antecedent.Standardize(images);
      other.m_consequent = (T)this.m_consequent.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_antecedent.IsGround() && this.m_consequent.IsGround();
    }
    
    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      vars.UnionWith(this.m_antecedent.GetFreeVariables());
      vars.UnionWith(this.m_consequent.GetFreeVariables());
      return vars;
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
        AbstractImplyExp<T> other = (AbstractImplyExp<T>)obj;
        return (this.m_antecedent == other.m_antecedent || this.m_antecedent.Equals(other.m_antecedent))
            && (this.m_consequent == other.m_consequent || this.m_consequent.Equals(other.m_consequent));
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
      return this.m_antecedent.GetHashCode() + this.m_consequent.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(imply ");
      str.Append(this.m_antecedent.ToString());
      str.Append(" ");
      str.Append(this.m_consequent.ToString());
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
      str.Append("(imply ");
      str.Append(this.m_antecedent.ToTypedString());
      str.Append(" ");
      str.Append(this.m_consequent.ToTypedString());
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
      if (value != 0)
        return value;

      AbstractImplyExp<T> otherExp = (AbstractImplyExp<T>)other;

      value = this.m_antecedent.CompareTo(otherExp.m_antecedent);
      if (value != 0)
        return value;

      return this.m_consequent.CompareTo(otherExp.m_consequent);
    }

    #endregion
  }

  /// <summary>
  /// Represents an implication constraint expression.
  /// </summary>
  public class ImplyConstraintExp : AbstractImplyExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new implication constraint expression with the specified antecedent and consequent.
    /// </summary>
    /// <param name="antecedent">The antecedent of the implication.</param>
    /// <param name="consequent">The consequent of the implication.</param>
    public ImplyConstraintExp(IConstraintExp antecedent, IConstraintExp consequent)
      : base(antecedent, consequent)
    {
      System.Diagnostics.Debug.Assert(antecedent != null && consequent != null);
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
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return ~m_antecedent.Progress(world, bindings) || m_consequent.Progress(world, bindings);
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
      return ~m_antecedent.EvaluateIdle(idleWorld, bindings) || m_consequent.EvaluateIdle(idleWorld, bindings);
    }
  }
}
