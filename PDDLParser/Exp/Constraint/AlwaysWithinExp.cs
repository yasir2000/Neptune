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

using System;
using System.Collections.Generic;
using System.Text;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "always-within" constraint expression of the PDDL language.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  /// <seealso cref="AlwaysExp"/>
  /// <seealso cref="WithinExp"/>
  public class AlwaysWithinExp : DerivedConstraintExp
  {
    /// <summary>
    /// The trigger expression, i.e. the expression which triggers the "within" part of the expression.
    /// </summary>
    private IConstraintExp m_exp;

    /// <summary>
    /// The constraint which must be satisfied once the "within" part has been triggered.
    /// </summary>
    /// <seealso cref="m_exp"/>
    private IConstraintExp m_impliedExp;

    /// <summary>
    /// The time limit within which the constraint must be satisfied.
    /// </summary>
    /// <remarks>
    /// This value is relative to the time at which the "within" part was triggered. For example,
    /// if this value is 10, and the "within" part is triggered at absolute time 3, then the 
    /// constraint must be satisfied before absolute time 13.
    /// </remarks>
    private double m_relativeTimestamp;

    /// <summary>
    /// Creates a new "always-within" constraint expression.
    /// </summary>
    /// <param name="exp">The trigger expression, i.e. the expression which triggers the "within" part.</param>
    /// <param name="impliedExp">The constraint which must be satisfied in the "within" part.</param>
    /// <param name="relativeTimestamp">The relative time limit of the within part.</param>
    public AlwaysWithinExp(IConstraintExp exp, IConstraintExp impliedExp, double relativeTimestamp)
    {
      System.Diagnostics.Debug.Assert(exp != null && impliedExp != null);
      if (relativeTimestamp < 0)
        throw new System.Exception("Error when constructing AlwaysWithinExp: the relative timestamp ("
                          + relativeTimestamp + ") must be >= 0.");

      this.m_exp = exp;
      this.m_impliedExp = impliedExp;
      this.m_relativeTimestamp = relativeTimestamp;
    }

    /// <summary>
    /// Generates and returns the compound constraint expression equivalent to this expression.
    /// In this case, (always-within ts exp1 exp2) = (always (imply (exp1 (within ts exp2)))).
    /// </summary>
    /// <returns>An equivalent constraint expression to this expression.</returns>
    public override IConstraintExp GenerateEquivalentExp()
    {
      return new AlwaysExp(new ImplyConstraintExp(m_exp, new WithinExp(m_impliedExp, m_relativeTimestamp)));
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
      AlwaysWithinExp other = (AlwaysWithinExp)base.Standardize(images);
      other.m_exp = (IConstraintExp)this.m_exp.Standardize(images);
      other.m_impliedExp = (IConstraintExp)this.m_impliedExp.Standardize(images);

      return other;
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
      AlwaysWithinExp other = (AlwaysWithinExp)base.Apply(bindings);
      other.m_exp = (IConstraintExp)this.m_exp.Apply(bindings);
      other.m_impliedExp = (IConstraintExp)this.m_impliedExp.Apply(bindings);

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
        AlwaysWithinExp other = (AlwaysWithinExp)obj;
        return this.m_exp.Equals(other.m_exp)
            && this.m_impliedExp.Equals(other.m_impliedExp)
            && this.m_relativeTimestamp.Equals(other.m_relativeTimestamp);
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
      return 13 * this.m_exp.GetHashCode() + 
             29 * this.m_impliedExp.GetHashCode()+ 
             43 * this.m_relativeTimestamp.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
        StringBuilder str = new StringBuilder();
        str.Append("(always-within ");
        str.Append(this.m_relativeTimestamp.ToString());
        str.Append(" ");
        str.Append(this.m_exp.ToString());
        str.Append(" ");
        str.Append(this.m_impliedExp.ToString());
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
        str.Append("(always-within ");
        str.Append(this.m_relativeTimestamp);
        str.Append(" ");
        str.Append(this.m_exp.ToTypedString());
        str.Append(" ");
        str.Append(this.m_impliedExp.ToTypedString());
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

      AlwaysWithinExp otherExp = (AlwaysWithinExp)other;

      value = this.m_exp.CompareTo(otherExp.m_exp);
      if (value != 0)
        return value;

      value = this.m_impliedExp.CompareTo(otherExp.m_impliedExp);
      if (value != 0)
        return value;

      return this.m_relativeTimestamp.CompareTo(otherExp.m_relativeTimestamp);
    }

    #endregion
  }
}