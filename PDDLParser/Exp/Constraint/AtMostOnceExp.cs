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
using PDDLParser.Exp.Constraint.TLPlan;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "at-most-once" constraint expression of the PDDL language.
  /// </summary>
  /// <seealso cref="AlwaysExp"/>
  /// <seealso cref="ImplyConstraintExp"/>
  /// <seealso cref="UntilExp"/>
  /// <seealso cref="NotConstraintExp"/>
  public sealed class AtMostOnceExp : DerivedConstraintExp
  {
    /// <summary>
    /// The body of the constraint expression.
    /// </summary>
    public IConstraintExp m_exp;

    /// <summary>
    /// Creates a new "at-most-once" constraint expression.
    /// </summary>
    /// <param name="exp">The body of the expression.</param>
    public AtMostOnceExp(IConstraintExp exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_exp = exp;
    }

    /// <summary>
    /// Generates and returns the compound constraint expression equivalent to this expression.
    /// In this case, <c>(at-most-once a) = (always (imply a (weak-until a (always ¬a))))</c>.
    /// </summary>
    /// <remarks>
    /// This equivalence was adapated from the "Plan Constraints and Preferences in PDDL3" document,
    /// written by Alfonso Gerevini and Derek Long. The original definition was
    /// <c>(at-most-once a) = (always (imply a (until a (always ¬a))))</c>; the problem with this definition is
    /// that once the predicate <c>a</c> becomes true, it has to become false for this to progress to true.
    /// </remarks>
    /// <returns>An equivalent constraint expression to this expression.</returns>
    /// <seealso cref="AlwaysExp"/>
    /// <seealso cref="ImplyConstraintExp"/>
    /// <seealso cref="UntilExp"/>
    /// <seealso cref="NotConstraintExp"/>
    public override IConstraintExp GenerateEquivalentExp()
    {
      return new AlwaysExp(new ImplyConstraintExp(m_exp, new WeakUntilExp(m_exp, new AlwaysExp(new NotConstraintExp(m_exp)))));
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
      AtMostOnceExp other = (AtMostOnceExp)base.Standardize(images);
      other.m_exp = (IConstraintExp)this.m_exp.Standardize(images);

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
      AtMostOnceExp other = (AtMostOnceExp)base.Apply(bindings);
      other.m_exp = (IConstraintExp)this.m_exp.Apply(bindings);

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
        AtMostOnceExp other = (AtMostOnceExp)obj;
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
      return 79 * this.m_exp.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(at-most-once ");
      str.Append(this.m_exp.ToString());
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
      str.Append("(at-most-once ");
      str.Append(this.m_exp.ToTypedString());
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

      AtMostOnceExp otherExp = (AtMostOnceExp)other;

      return this.m_exp.CompareTo(otherExp.m_exp);
    }

    #endregion
  }
}