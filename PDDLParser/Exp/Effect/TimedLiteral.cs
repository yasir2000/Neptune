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
using System.Collections;
using System;
using System.Text;
using PDDLParser.Exp.Formula;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect 
{
  /// <summary>
  /// Represents a timed literal primarily used in initial state definition of the PDDL language.
  /// </summary>
  public class TimedLiteral : AbstractExp, ILiteral
  {
    /// <summary>
    /// The time stamp of the timed literal.
    /// </summary>
    private double m_ts;

    /// <summary>
    /// The literal which must be set in the world at the given time stamp.
    /// </summary>
    private ILiteral m_literal;

    /// <summary>
    /// Creates a new timed literal with a literal.
    /// </summary>
    /// <param name="ts">The time stamp of the timed literal, which must not be smaller than 0.</param>
    /// <param name="literal">The literal of the timed literal.</param>
    public TimedLiteral(double ts, ILiteral literal)
    {
      System.Diagnostics.Debug.Assert(ts >= 0 && literal != null);
      this.m_ts = ts;
      this.m_literal = literal;
    }

    /// <summary>
    /// Gets the predicate associated with this literal.
    /// </summary>
    public AtomicFormula Predicate
    {
      get { return this.m_literal.Predicate; }
    }

    /// <summary>
    /// Updates the specified world with this effect.
    /// A timed literal adds a delayed effect the to world, in order to set the literal at
    /// the given time stamp.
    /// </summary>
    /// <param name="evaluationWorld">World to evaluate conditions against.</param>
    /// <param name="updateWorld">World to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                       LocalBindings bindings, ActionContext actionContext)
    {
      updateWorld.AddEndEffect(m_ts, m_literal);
    }

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      return m_literal.GetModifiedDescribedFormulas();
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this timed
    /// literal by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The variable bindings.</param>
    /// <returns>A substituted copy of this list expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      TimedLiteral clone = (TimedLiteral)this.Clone();
      clone.m_literal = (ILiteral)this.m_literal.Apply(bindings);
      return clone;
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
      TimedLiteral clone = (TimedLiteral)this.Clone();
      clone.m_literal = (ILiteral)this.m_literal.Standardize(images);
      return clone;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return m_literal.GetFreeVariables();
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_literal.IsGround();
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
        TimedLiteral other = (TimedLiteral)obj;
        return this.m_literal.Equals(other.m_literal) && this.m_ts.Equals(other.m_ts);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this timed literal.
    /// </summary>
    /// <returns>The hash code of this timed literal.</returns>
    public override int GetHashCode()
    {
      return this.GetType().GetHashCode() + 31 * this.m_literal.GetHashCode() + 53 * this.m_ts.GetHashCode();
    }

    /// <summary>
    /// Returns a clone of this list expression.
    /// </summary>
    /// <returns>A clone of this list expression.</returns>
    public override object Clone()
    {
      TimedLiteral other = (TimedLiteral) base.Clone();
      return other;
    }

    /// <summary>
    /// Returns a string representation of this timed literal.
    /// </summary>
    /// <returns>A string representation of this timed literal.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(at ");
      str.Append(this.m_ts.ToString());
      str.Append(" ");
      str.Append(this.m_literal.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this timed literal.
    /// </summary>
    /// <returns>A typed string representation of this timed literal.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(at ");
      str.Append(this.m_ts);
      str.Append(" ");
      str.Append(this.m_literal.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this timed literal with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      TimedLiteral otherLiteral = (TimedLiteral)other;

      return this.m_ts.CompareTo(otherLiteral.m_ts);
    }

    #endregion
  }
}