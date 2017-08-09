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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

 using System.Collections.Generic;
 using System.Text;
using PDDLParser.Exp.Constraint.TLPlan;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "sometime-before" constraint expression of the PDDL language.
  /// </summary>
  /// <remarks>
  /// On the goal world, this constraint expression will evaluate to true even if
  /// the "before" expression was never true when the other expression was never
  /// true either.
  /// </remarks>
  /// <seealso cref="WeakUntilExp"/>
  /// <seealso cref="AndConstraintExp"/>
  /// <seealso cref="NotConstraintExp"/>
  public class SometimeBeforeExp : DerivedConstraintExp
  {
    /// <summary>
    /// The main constraint expression which happens second.
    /// </summary>
    public IConstraintExp m_exp;

    /// <summary>
    /// The constraint expression which must happen first.
    /// </summary>
    public IConstraintExp m_beforeExp;

    /// <summary>
    /// Creates a new "sometime-before" constraint expression.
    /// </summary>
    /// <param name="exp">The expression that must happen second.</param>
    /// <param name="beforeExp">The expression that must happen first.</param>
    public SometimeBeforeExp(IConstraintExp exp, IConstraintExp beforeExp)
    {
      System.Diagnostics.Debug.Assert(exp != null && beforeExp != null);

      this.m_exp = exp;
      this.m_beforeExp = beforeExp;
    }

    /// <summary>
    /// Generates and returns the compound constraint expression equivalent to this expression.
    /// In this case, (sometime-before a b) = (weak-until (and ¬a ¬b) (and b ¬a))
    /// </summary>
    /// <returns>An equivalent constraint expression to this expression.</returns>
    /// <seealso cref="WeakUntilExp"/>
    /// <seealso cref="AndConstraintExp"/>
    /// <seealso cref="NotConstraintExp"/>
    public override IConstraintExp GenerateEquivalentExp()
    {
      return new WeakUntilExp(new AndConstraintExp(new NotConstraintExp(m_exp), new NotConstraintExp(m_beforeExp)),
                              new AndConstraintExp(m_beforeExp, new NotConstraintExp(m_exp)));
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
      SometimeBeforeExp other = (SometimeBeforeExp)base.Apply(bindings);
      other.m_exp = (IConstraintExp)this.m_exp.Apply(bindings);
      other.m_beforeExp = (IConstraintExp)this.m_beforeExp.Apply(bindings);

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
      SometimeBeforeExp other = (SometimeBeforeExp)base.Standardize(images);
      other.m_exp = (IConstraintExp)this.m_exp.Standardize(images);
      other.m_beforeExp = (IConstraintExp)this.m_beforeExp.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj.GetType() == this.GetType())
      {
        SometimeBeforeExp other = (SometimeBeforeExp)obj;
        return this.m_exp.Equals(other.m_exp) && this.m_beforeExp.Equals(other.m_beforeExp);
      }
      return false;
    }

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public override int GetHashCode()
    {
      return 37 * this.m_exp.GetHashCode() + 
             43 * this.m_beforeExp.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(sometime-before ");
      str.Append(this.m_exp.ToString());
      str.Append(" ");
      str.Append(this.m_beforeExp.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(sometime-before ");
      str.Append(this.m_exp.ToTypedString());
      str.Append(" ");
      str.Append(this.m_beforeExp.ToTypedString());
      str.Append(")");
      return str.ToString();
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

      SometimeBeforeExp otherExp = (SometimeBeforeExp)other;

      value = this.m_beforeExp.CompareTo(otherExp.m_beforeExp);
      if (value != 0)
        return value;

      return this.m_exp.CompareTo(otherExp.m_exp);
    }

    #endregion
  }
}