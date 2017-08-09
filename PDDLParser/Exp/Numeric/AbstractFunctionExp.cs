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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Numeric
{
  /// <summary>
  /// Base class for arithmetic functions.
  /// </summary>
  public abstract class AbstractFunctionExp : AbstractNumericExp
  {
    /// <summary>
    /// The arguments of this arithmetic function.
    /// </summary>
    private List<INumericExp> m_arguments;

    /// <summary>
    /// The image of the function.
    /// </summary>
    private string m_image;

    /// <summary>
    /// Creates a new arithmetic function with the specified arguments.
    /// </summary>
    /// <param name="image">The image of the arithmetic function.</param>
    /// <param name="arguments">The arguments of the new arithmetic function.</param>
    public AbstractFunctionExp(string image, List<INumericExp> arguments)
      : base()
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());

      this.m_image = image;
      this.m_arguments = arguments;
    }

    /// <summary>
    /// Calculates the result of this arithmetic function called with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The result of the arithmetic function.</returns>
    protected abstract double Calculate(double[] args);

    /// <summary>
    /// Evaluates this numeric expression in the specified open world.
    /// The value of an arithmetic function is obtained by recursively evaluating its arguments
    /// and then applying the function.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the resulting numeric value.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public override FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      double[] values = new double[this.m_arguments.Count];
      for (int i = 0; i < this.m_arguments.Count; ++i)
      {
        FuzzyDouble value = this.m_arguments[i].Evaluate(world, bindings);
        if (value.Status != FuzzyDouble.State.Defined)
        {
          return value;
        }
        else
        {
          values[i] = value.Value;
        }
      }
      return new FuzzyDouble(Calculate(values));
    }

    /// <summary>
    /// Simplifies this numeric expression by evaluating its known expression parts.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, a numeric value, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public override NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      List<INumericExp> newArgs = new List<INumericExp>(this.m_arguments.Count);
      double[] values = new double[this.m_arguments.Count];
      bool allSimplified = true;
      for (int i = 0; i < this.m_arguments.Count; ++i)
      {
        NumericValue simplifiedExp = this.m_arguments[i].Simplify(world, bindings);
        if (simplifiedExp.Exp != null)
        {
          newArgs.Add(simplifiedExp.Exp);
          allSimplified = false;
        }
        else
        {
          if (simplifiedExp.Value.Status != Double.State.Defined)
          {
            return new NumericValue(simplifiedExp.Value);
          }
          else
          {
            newArgs.Add(new Number(simplifiedExp.Value.Value));
            values[i] = simplifiedExp.Value.Value;
          }
        }
      }
      if (allSimplified)
      {
        return new NumericValue(new Double(Calculate(values)));
      }
      else
      {
        AbstractFunctionExp other = (AbstractFunctionExp)this.MemberwiseClone();
        // Same expression but with simplified arguments.
        other.m_arguments = newArgs;

        return new NumericValue(other);
      }
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified closed world.
    /// The value of an arithmetic function is obtained by recursively evaluating its arguments
    /// and then applying the function.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public override Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      double[] values = new double[this.m_arguments.Count];
      for (int i = 0; i < this.m_arguments.Count; ++i)
      {
        Double value = this.m_arguments[i].Evaluate(world, bindings);
        if (value.Status != Double.State.Defined)
        {
          return value;
        }
        else
        {
          values[i] = value.Value;
        }
      }
      return new Double(Calculate(values));
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AbstractFunctionExp other = (AbstractFunctionExp)base.Apply(bindings);
      other.m_arguments = new List<INumericExp>(this.m_arguments.Count);
      foreach (INumericExp arg in this.m_arguments)
      {
        other.m_arguments.Add((INumericExp)arg.Apply(bindings));
      }
      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      AbstractFunctionExp other = (AbstractFunctionExp)base.Standardize(images);
      other.m_arguments = new List<INumericExp>(this.m_arguments.Count);
      foreach (INumericExp arg in this.m_arguments)
      {
        other.m_arguments.Add((INumericExp)arg.Standardize(images));
      }
      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// An arithmetic function is ground if all its arguments are ground.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      foreach (INumericExp arg in this.m_arguments)
      {
        if (!arg.IsGround())
          return false;
      }
      return true;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      foreach (INumericExp arg in this.m_arguments)
      {
        vars.UnionWith(arg.GetFreeVariables());
      }
      return vars;
    }

    /// <summary>
    /// Returs a clone of this arithmetic function.
    /// </summary>
    /// <returns>A clone of this arithmetic function..</returns>
    public override object Clone()
    {
      AbstractFunctionExp clone = (AbstractFunctionExp)base.Clone();
      clone.m_arguments = new List<INumericExp>(this.m_arguments);

      return clone;
    }

    /// <summary>
    /// Returns whether this arithmetic function is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this arithmetic function is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        AbstractFunctionExp other = (AbstractFunctionExp)obj;
        return this.m_arguments.SequenceEqual(other.m_arguments);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this arithmetic function.
    /// </summary>
    /// <returns>The hash code of this arithmetic function.</returns>
    public override int GetHashCode()
    {
      return this.m_arguments.GetOrderedEnumerableHashCode();
    }

    /// <summary>
    /// Returns a string representation of this arithmetic function.
    /// </summary>
    /// <returns>A string representation of this arithmetic function.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.m_image);
      foreach (INumericExp arg in this.m_arguments)
      {
        str.Append(" " + arg.ToString());
      }
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this arithmetic function.
    /// </summary>
    /// <returns>A typed string representation of this arithmetic function.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.m_image);
      foreach (INumericExp arg in this.m_arguments)
      {
        str.Append(" " + arg.ToTypedString());
      }
      str.Append(")");
      str.Append(" - ");
      str.Append("Number");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this arithmetic function with another expression
    /// </summary>
    /// <param name="other">The expression to compare this arithmetic function  to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      AbstractFunctionExp otherExp = (AbstractFunctionExp)other;

      value = this.m_arguments.Count.CompareTo(otherExp.m_arguments.Count);
      if (value != 0)
        return value;

      for (int i = 0; i < this.m_arguments.Count; ++i)
      {
        value = this.m_arguments[i].CompareTo(otherExp.m_arguments[i]);
        if (value != 0)
          return value;
      }

      return value;
    }

    #endregion
  }
}
