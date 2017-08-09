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
  /// Represents a binary constraint expression.
  /// </summary>
  public abstract class BinaryConstraintExp : AbstractConstraintExp
  {
    /// <summary>
    /// The first expression argument of this conditional binary expression.
    /// </summary>
    private IConstraintExp m_arg1;

    /// <summary>
    /// The second expression argument of this conditional binary expression.
    /// </summary>
    private IConstraintExp m_arg2;

    /// <summary>
    /// Creates a new binary conditional expression with a specific expression.
    /// </summary>
    /// <param name="arg1">The first expression argument.</param>
    /// <param name="arg2">The second expression argument.</param>
    public BinaryConstraintExp(IConstraintExp arg1, IConstraintExp arg2)
      : base()
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);

      this.m_arg1 = arg1;
      this.m_arg2 = arg2;
    }
    /// <summary>
    /// Returns the first argument of this conditional binary expression.
    /// </summary>
    /// <returns>The first argument of this conditional binary expression.</returns>
    public IConstraintExp Exp1
    {
      get { return this.m_arg1; }
    }

    /// <summary>
    /// Returns the second argument of this conditional binary expression.
    /// </summary>
    /// <returns>The second argument of this conditional binary expression</returns>
    public IConstraintExp Exp2
    {
      get { return this.m_arg2; }
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
      BinaryConstraintExp other = (BinaryConstraintExp)base.Apply(bindings);
      other.m_arg1 = (IConstraintExp)this.m_arg1.Apply(bindings);
      other.m_arg2 = (IConstraintExp)this.m_arg2.Apply(bindings);

      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// The base implementation is to simply clone the object.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      BinaryConstraintExp other = (BinaryConstraintExp)base.Standardize(images);
      other.m_arg1 = (IConstraintExp)this.m_arg1.Standardize(images);
      other.m_arg2 = (IConstraintExp)this.m_arg2.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      vars.UnionWith(m_arg1.GetFreeVariables());
      vars.UnionWith(m_arg2.GetFreeVariables());
      return vars;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_arg1.IsGround() && this.m_arg2.IsGround();
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
        BinaryConstraintExp other = (BinaryConstraintExp)obj;
        return this.m_arg1.Equals(other.m_arg1)
            && this.m_arg2.Equals(other.m_arg2);
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
      return this.GetType().GetHashCode() * 
            (7 * this.m_arg1.GetHashCode() + 11 * this.m_arg2.GetHashCode());
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this abstract expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this abstract expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      BinaryConstraintExp otherExp = (BinaryConstraintExp)other;

      value = m_arg1.CompareTo(otherExp.m_arg1);
      if (value != 0)
        return value;

      return m_arg2.CompareTo(otherExp.m_arg2);
    }

    #endregion
  }
}