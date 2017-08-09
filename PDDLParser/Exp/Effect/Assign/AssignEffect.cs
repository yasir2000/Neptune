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
using System.Text;
using PDDLParser.Exception;
using PDDLParser.Exp.Formula;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect.Assign
{
  /// <summary>
  /// This class represents a fluent assignment.
  /// </summary>
  public abstract class AssignEffect : AbstractExp, IEffect
  {
    /// <summary>
    /// The fluent application to assign the value to.
    /// </summary>
    protected FluentApplication m_head;

    /// <summary>
    /// The value to assign to the fluent application.
    /// </summary>
    protected IEvaluableExp m_body;

    /// <summary>
    /// The image of this fluent assignment.
    /// </summary>
    private string m_image;

    /// <summary>
    /// Creates a new fluent assignment.
    /// </summary>
    /// <param name="image">The image of this fluent assignment.</param>
    /// <param name="head">The fluent application to assign the value to.</param>
    /// <param name="body">The value to assign to the fluent application.</param>
    public AssignEffect(string image, FluentApplication head, IEvaluableExp body)
      : base()
    {
      System.Diagnostics.Debug.Assert(head != null && body != null);

      this.m_image = image;
      this.m_head = head;
      this.m_body = body;
    }

    /// <summary>
    /// Updates the world with this fluent assignment.
    /// </summary>
    /// <param name="head">The evaluated fluent application to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    protected abstract void UpdateWorldWithAssignEffect(FluentApplication head, 
                                                        IDurativeOpenWorld updateWorld, 
                                                        LocalBindings bindings);

    /// <summary>
    /// Updates the specified world with this effect.
    /// A fluent assignment assigns a value to a fluent.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                       LocalBindings bindings, ActionContext actionContext)
    {
      FuzzyArgsEvalResult result = m_head.EvaluateArguments(updateWorld, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          UpdateWorldWithAssignEffect((FluentApplication)result.Value, updateWorld, bindings);
          break;
        case FuzzyArgsEvalResult.State.Undefined:
          throw new UndefinedExpException(this.ToString() +
            " failed since the first operand contains as least one argument which evaluates to undefined.");
        case FuzzyArgsEvalResult.State.Unknown:
          throw new UnknownExpException(m_head.ToString() +
            " failed since the first operand contains as least one argument which evaluates to unknown.");
        default:
          throw new System.Exception("Invalid EvalStatus value: " + result.Status);
      }
    }

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      HashSet<DescribedFormula> formulas = new HashSet<DescribedFormula>();
      formulas.Add(this.m_head.RootFluent);
      return formulas;
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AssignEffect other = (AssignEffect)base.Apply(bindings);
      other.m_head = (FluentApplication)this.m_head.Apply(bindings);
      other.m_body = (IEvaluableExp)this.m_body.Apply(bindings);

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
      AssignEffect other = (AssignEffect)base.Standardize(images);
      other.m_head = (FluentApplication)this.m_head.Standardize(images);
      other.m_body = (IEvaluableExp)this.m_body.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_head.IsGround() && this.m_body.IsGround();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = this.m_head.GetFreeVariables();
      vars.UnionWith(this.m_body.GetFreeVariables());

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
        AssignEffect other = (AssignEffect)obj;
        return this.m_head.Equals(other.m_head)
            && this.m_body.Equals(other.m_body);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this fluent assignment.
    /// </summary>
    /// <returns>The hash code of this fluent assignment.</returns>
    public override int GetHashCode()
    {
      return this.m_head.GetHashCode() + this.m_body.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this fluent assignment.
    /// </summary>
    /// <returns>A string representation of this fluent assignment.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(" + this.m_image + " ");
      str.Append(this.m_head.ToString());
      str.Append(" ");
      str.Append(this.m_body.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this fluent assignment.
    /// </summary>
    /// <returns>A typed string representation of this fluent assignment.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(" + this.m_image + " ");
      str.Append(this.m_head.ToTypedString());
      str.Append(" ");
      str.Append(this.m_body.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this fluent assignment with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);

      if (value != 0)
        return value;

      AssignEffect otherExp = (AssignEffect)other;

      value = this.m_head.CompareTo(otherExp.m_head);
      if (value != 0)
        return value;

      return this.m_body.CompareTo(otherExp.m_body);
    }

    #endregion
  }
}