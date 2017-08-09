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
using System;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

using Double = PDDLParser.Exp.Struct.Double;
namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents the optimization expression used in metric specification of the PDDL language.
  /// </summary>
  /// <remarks>
  /// It has arbitrarily been decided that all metrics will return values that need to be minimized.
  /// </remarks>
  public abstract class MetricExp : AbstractExp, INumericExp
  {
    /// <summary>
    /// The expression to optimize.
    /// </summary>
    private INumericExp m_exp;

    /// <summary>
    /// The numeric expression representing all unnamed preferences.
    /// </summary>
    private INumericExp m_unnamedPreferences;

    /// <summary>
    /// Gets the numeric expression representing all unnamed preferences.
    /// </summary>
    public INumericExp UnnamedPreferences
    {
      get { return m_unnamedPreferences; }
    }

    /// <summary>
    /// Creates a new metric expression.
    /// </summary>
    /// <param name="exp">The expression to optimize.</param>
    /// <param name="unnamedPrefs">The unnamed preferences.</param>
    public MetricExp(INumericExp exp, INumericExp unnamedPrefs)
        : base()
    {
      System.Diagnostics.Debug.Assert(exp != null);

      this.m_exp = exp;
      this.m_unnamedPreferences = unnamedPrefs; // This can be null (no unnamed preferences)
    }

    /// <summary>
    /// Gets a string representatin of the type of optimization required.
    /// </summary>
    public abstract string TypeName { get; }

    /// <summary>
    /// Returns a minimized version of the metric evaluated on a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The minimized version of the metric.</returns>
    protected abstract FuzzyDouble EvaluateMinimizedMetric(IReadOnlyOpenWorld world, LocalBindings bindings);
    /// <summary>
    /// Returns a minimized version of the metric evaluated on a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The minimized version of the metric.</returns>
    protected abstract Double EvaluateMinimizedMetric(IReadOnlyClosedWorld world, LocalBindings bindings);
    
    /// <summary>
    /// Gets the expression to optimize.
    /// </summary>
    public INumericExp Exp
    {
      get { return this.m_exp; }
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      MetricExp other = (MetricExp)base.Apply(bindings);
      other.m_exp = (INumericExp)this.m_exp.Apply(bindings);

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
      MetricExp other = (MetricExp)base.Standardize(images);
      other.m_exp = (INumericExp)this.m_exp.Standardize(images);

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
        MetricExp other = (MetricExp)obj;
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
      return this.m_exp.GetHashCode();
    }

    /// <summary>
    /// Returns a string of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:metric " + this.TypeName.ToLower() + " ");
      str.Append(this.Exp.ToString());
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
      str.Append("(:metric " + this.TypeName.ToLower() + " ");
      str.Append(this.Exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region INumericExp Interface

    /// <summary>
    /// Evaluates the metric in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <remarks>
    /// Unnamed preferences are counted in the metric.
    /// Also, it has arbitrarily been decided that all metrics will return values that need to be minimized.
    /// </remarks>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting metric value to be minimized.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      if (UnnamedPreferences != null)
        return EvaluateMinimizedMetric(world, bindings) + UnnamedPreferences.Evaluate(world, bindings);

      return EvaluateMinimizedMetric(world, bindings);
    }

    /// <summary>
    /// Evaluates the metric in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <remarks>
    /// Unnamed preferences are counted in the metric.
    /// Also, it has arbitrarily been decided that all metrics will return values that need to be minimized.
    /// </remarks>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting metric value to be minimized.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      if (UnnamedPreferences != null)
        return EvaluateMinimizedMetric(world, bindings) + UnnamedPreferences.Evaluate(world, bindings);

      return EvaluateMinimizedMetric(world, bindings);
    }

    /// <summary>
    /// Simplifies this metric by evaluating its known expression parts.
    /// The bindings should not be modified by this call.
    /// The resulting metric should not contain any unbound variables, since
    /// they are substituted according to the bindings supplied.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, a metric value, or the simplified metric.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public virtual NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      MetricExp other = (MetricExp)this.Clone();
      other.m_exp = this.m_exp.Simplify(world, bindings).GetEquivalentExp();
      other.m_unnamedPreferences = this.m_unnamedPreferences.Simplify(world, bindings).GetEquivalentExp();

      return new NumericValue(other);
    }

    #endregion
  }
}