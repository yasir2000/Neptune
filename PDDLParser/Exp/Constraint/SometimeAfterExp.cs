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

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "sometime-after" constraint expression of the PDDL language.
  /// </summary>
  /// <seealso cref="AlwaysExp"/>
  /// <seealso cref="ImplyConstraintExp"/>
  /// <seealso cref="SometimeExp"/>
  public class SometimeAfterExp : DerivedConstraintExp
  {
    /// <summary>
    /// The first, conditional constraint expression.
    /// </summary>
    public IConstraintExp m_exp;

    /// <summary>
    /// The second, consequent constraint expression that must be true after the first one.
    /// </summary>
    /// <remarks>This may never be progressed if the <see cref="m_exp"/> never progresses to true.</remarks>
    public IConstraintExp m_afterExp;

    /// <summary>
    /// Creates a new "sometime-after" constraint expression.
    /// </summary>
    /// <param name="exp">The first, conditional constraint expression.</param>
    /// <param name="afterExp">The second, consequent constraint expression (progressed only if the first one ever progresses to true).</param>
    public SometimeAfterExp(IConstraintExp exp, IConstraintExp afterExp)
    {
      System.Diagnostics.Debug.Assert(exp != null && afterExp != null);

      this.m_exp = exp;
      this.m_afterExp = afterExp;
    }

    /// <summary>
    /// Generates and returns the compound constraint expression equivalent to this expression.
    /// in this case, (sometime-after a b) = (always (imply a (sometime b))
    /// </summary>
    /// <returns>An equivalent constraint expression to this expression.</returns>
    /// <seealso cref="AlwaysExp"/>
    /// <seealso cref="ImplyConstraintExp"/>
    /// <seealso cref="SometimeExp"/>
    public override IConstraintExp GenerateEquivalentExp()
    {
      return new AlwaysExp(new ImplyConstraintExp(m_exp, new SometimeExp(m_afterExp)));
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
      SometimeAfterExp other = (SometimeAfterExp)base.Apply(bindings);
      other.m_exp = (IConstraintExp)this.m_exp.Apply(bindings);
      other.m_afterExp = (IConstraintExp)this.m_afterExp.Apply(bindings);

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
      SometimeAfterExp other = (SometimeAfterExp)base.Standardize(images);
      other.m_exp = (IConstraintExp)this.m_exp.Standardize(images);
      other.m_afterExp = (IConstraintExp)this.m_afterExp.Standardize(images);

      return other;
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
        SometimeAfterExp other = (SometimeAfterExp)obj;
        return this.m_exp.Equals(other.m_exp) && this.m_afterExp.Equals(other.m_afterExp);
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
      return 23 * this.m_exp.GetHashCode() + 
             61 * this.m_afterExp.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(sometime-after ");
      str.Append(this.m_exp.ToString());
      str.Append(" ");
      str.Append(this.m_afterExp.ToString());
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
      str.Append("(sometime-after ");
      str.Append(this.m_exp.ToTypedString());
      str.Append(" ");
      str.Append(this.m_afterExp.ToTypedString());
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

      SometimeAfterExp sfa = (SometimeAfterExp)other;

      value = this.m_exp.CompareTo(sfa.m_exp);
      if (value != 0)
        return value;

      return this.m_afterExp.CompareTo(sfa.m_afterExp);
    }

    #endregion
  }
}