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
using PDDLParser.Exception;
using PDDLParser.Exp.Formula;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect
{
  /// <summary>
  /// A negative atomic formula application unsets an atomic formula from a given world.
  /// Unlike an atomic formula application, a negative atomic formula application can only 
  /// be used as an effect.
  /// </summary>
  public class NotAtomicFormulaApplication: AbstractExp, ILiteral
  {
    /// <summary>
    /// The atomic formula application to unset.
    /// </summary>
    private AtomicFormulaApplication m_atom;

    /// <summary>
    /// Creates a new negative atomic formula application.
    /// </summary>
    /// <param name="atom">The atomic formula application to unset.</param>
    public NotAtomicFormulaApplication(AtomicFormulaApplication atom)
      : base()
    {
      System.Diagnostics.Debug.Assert(atom != null);

      this.m_atom = atom;
    }

    /// <summary>
    /// Gets the predicate associated with this literal.
    /// </summary>
    public AtomicFormula Predicate
    {
      get { return this.m_atom.Predicate; }
    }

    /// <summary>
    /// Updates the specified world with this effect.
    /// A negative atomic formula application unsets the appropriate negative atomic formula
    /// application in the specified world.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against. Note that this is 
    /// usually the un-modified version of the world to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                       LocalBindings bindings, ActionContext actionContext)
    {
      FuzzyArgsEvalResult result = this.m_atom.EvaluateArguments(evaluationWorld, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          updateWorld.Unset((AtomicFormulaApplication)result.Value);
          break;
        case FuzzyArgsEvalResult.State.Undefined:
          throw new UndefinedExpException(this.ToString() +
            " failed to update the world since at least one of its arguments evaluates to undefined.");
        case FuzzyArgsEvalResult.State.Unknown:
          throw new UnknownExpException(this.ToString() +
            " failed to update the world since at least one of its arguments evaluates to unknown.");
        default:
          throw new System.Exception("Invalid EvalStatus value: " + result.Status);
      }
    }

    /// <summary>
    /// Retrieves the root atomic formula modified by this atomic formula application.
    /// </summary>
    /// <returns>The root atomic formula modified by this atomic formula application.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      return this.m_atom.GetModifiedDescribedFormulas();
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      NotAtomicFormulaApplication other = (NotAtomicFormulaApplication)base.Apply(bindings);
      other.m_atom = (AtomicFormulaApplication)this.m_atom.Apply(bindings);

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
      NotAtomicFormulaApplication other = (NotAtomicFormulaApplication)base.Standardize(images);
      other.m_atom = (AtomicFormulaApplication)this.m_atom.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_atom.IsGround();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return this.m_atom.GetFreeVariables();
    }

    /// <summary>
    /// Returns true if this negative atomic formula application is equal to a specified object.
    /// Two negative atomic formula applications are equal if their inner expressions are equal.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this negative atomic formula is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        NotAtomicFormulaApplication other = (NotAtomicFormulaApplication)obj;
        return this.m_atom.Equals(other.m_atom);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this negative atomic formula application.
    /// </summary>
    /// <returns>The hash code of this negative atomic formula application.</returns>
    public override int GetHashCode()
    {
      return this.m_atom.GetHashCode();
    }

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(not ");
      str.Append(this.m_atom.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(not ");
      str.Append(this.m_atom.ToString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Members

    /// <summary>
    /// Compares this negative atomic formula application with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      NotAtomicFormulaApplication otherFormula = (NotAtomicFormulaApplication)other;
      return this.m_atom.CompareTo(otherFormula.m_atom);
    }

    #endregion
  }
}