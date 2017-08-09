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

using System.Collections.Generic;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents a unary constraint expression of the PDDL language.
  /// </summary>
  public abstract class UnaryConstraintExp : AbstractConstraintExp
  {
    /// <summary>
    /// The body of the unary constraint expression.
    /// </summary>
    private IConstraintExp m_exp;

    /// <summary>
    /// Creates a new unary constraint expression with a specified body.
    /// </summary>
    /// <param name="exp">The body of the unary constraint expression.</param>
    public UnaryConstraintExp(IConstraintExp exp)
      : base()
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_exp = exp;
    }

    /// <summary>
    /// Returns the body of the unary constraint expression.
    /// </summary>
    /// <returns>The body of the unary constraint expression.</returns>
    protected IConstraintExp Exp
    {
      get { return this.m_exp; }
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// The base implementation is to simply clone the object.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      UnaryConstraintExp other = (UnaryConstraintExp)base.Apply(bindings);
      other.m_exp = (IConstraintExp)this.m_exp.Apply(bindings);

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
      UnaryConstraintExp other = (UnaryConstraintExp)base.Standardize(images);
      other.m_exp = (IConstraintExp)this.m_exp.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return m_exp.GetFreeVariables();
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
        UnaryConstraintExp other = (UnaryConstraintExp)obj;
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
      return this.GetType().GetHashCode() * this.m_exp.GetHashCode();
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

      UnaryConstraintExp otherExp = (UnaryConstraintExp)other;

      return m_exp.CompareTo(otherExp.m_exp);
    }

    #endregion
  }
}