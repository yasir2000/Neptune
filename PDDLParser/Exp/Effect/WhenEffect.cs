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
using PDDLParser.Exp.Formula;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect
{
  /// <summary>
  /// This class represents a conditional effect.
  /// </summary>
  public class WhenEffect: AbstractExp, IEffect
  {
    /// <summary>
    /// The condition to satisfy.
    /// </summary>
    protected ILogicalExp m_condition;

    /// <summary>
    /// The conditional effect.
    /// </summary>
    protected IEffect m_effect;

    /// <summary>
    /// Creates a new conditional effect.
    /// </summary>
    /// <param name="condition">The condition to satisfy.</param>
    /// <param name="effect">The conditional effect.</param>
    public WhenEffect(ILogicalExp condition, IEffect effect)
      : base()
    {
      System.Diagnostics.Debug.Assert(condition != null && effect != null);

      this.m_condition = condition;
      this.m_effect = effect;
    }

    /// <summary>
    /// Updates the specified world with this effect.
    /// A conditional effect updates the world if its condition is satisfied. Note that the
    /// condition is evaluated against the evaluation world, and NOT the update world.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public virtual void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                               LocalBindings bindings, ActionContext actionContext)
    {
      if (m_condition.Evaluate(evaluationWorld, bindings).ToBool())
        m_effect.Update(evaluationWorld, updateWorld, bindings, actionContext);
    }

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      return m_effect.GetModifiedDescribedFormulas();
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      WhenEffect other = (WhenEffect)base.Apply(bindings);
      other.m_condition = (ILogicalExp)this.m_condition.Apply(bindings);
      other.m_effect = (IEffect)this.m_effect.Apply(bindings);

      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      WhenEffect other = (WhenEffect)base.Standardize(images);
      other.m_condition = (ILogicalExp)this.m_condition.Standardize(images);
      other.m_effect = (IEffect)this.m_effect.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_condition.IsGround() && this.m_effect.IsGround();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      vars.UnionWith(this.m_condition.GetFreeVariables());
      vars.UnionWith(this.m_effect.GetFreeVariables());
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
        WhenEffect other = (WhenEffect)obj;
        return this.m_condition.Equals(other.m_condition)
            && this.m_effect.Equals(other.m_effect);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this conditional effect.
    /// </summary>
    /// <returns>The hash code of this conditional effect.</returns>
    public override int GetHashCode()
    {
      return this.m_condition.GetHashCode() + this.m_effect.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this conditional effect.
    /// </summary>
    /// <returns>A string representation of this conditional effect.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(when ");
      str.Append(this.m_condition.ToString());
      str.Append(" ");
      str.Append(this.m_effect.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this conditional effect.
    /// </summary>
    /// <returns>A typed string representation of this conditional effect.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(when ");
      str.Append(this.m_condition.ToTypedString());
      str.Append(" ");
      str.Append(this.m_effect.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this conditional effect with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      WhenEffect otherExp = (WhenEffect)other;

      value = this.m_condition.CompareTo(otherExp.m_condition);
      if (value != 0)
        return value;

      return this.m_effect.CompareTo(otherExp.m_effect);
    }

    #endregion
  }
}