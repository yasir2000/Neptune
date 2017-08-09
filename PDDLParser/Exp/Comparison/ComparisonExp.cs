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
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Comparison
{
  /// <summary>
  /// This class represents a comparison between two expressions.
  /// </summary>
  /// <typeparam name="T">The type of the expressions to compare.</typeparam>
  public abstract class ComparisonExp<T> : AbstractLogicalExp
    where T : class, IEvaluableExp
  {
    /// <summary>
    /// The first argument of the comparison operator.
    /// </summary>
    protected T m_arg1;

    /// <summary>
    /// The second argument of the comparison operator.
    /// </summary>
    protected T m_arg2;

    /// <summary>
    /// A string representation of the operator.
    /// </summary>
    private string m_op;

    /// <summary>
    /// Creates a new comparison expression with the two specified arguments.
    /// </summary>
    /// <param name="op">A string representation of this operator.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    protected ComparisonExp(string op, T arg1, T arg2)
      : base()
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);

      this.m_op = op;
      this.m_arg1 = arg1;
      this.m_arg2 = arg2;
    }

    /// <summary>
    /// Returns the first argument of the comparison expression.
    /// </summary>
    public T Arg1
    {
      get { return m_arg1; }
    }

    /// <summary>
    /// Returns the second argument of the comparison expression.
    /// </summary>
    public T Arg2
    {
      get { return m_arg2; }
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since
    /// this function is not supported.</exception>
    public override HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return Evaluate(idleWorld, bindings);
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
      ComparisonExp<T> other = (ComparisonExp<T>)base.Apply(bindings);
      other.m_arg1 = (T)this.m_arg1.Apply(bindings);
      other.m_arg2 = (T)this.m_arg2.Apply(bindings);

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
      ComparisonExp<T> other = (ComparisonExp<T>)base.Standardize(images);
      other.m_arg1 = (T)this.m_arg1.Standardize(images);
      other.m_arg2 = (T)this.m_arg2.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      vars.UnionWith(this.m_arg1.GetFreeVariables());
      vars.UnionWith(this.m_arg2.GetFreeVariables());
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
    /// Returns a clone of this expression.
    /// </summary>
    /// <returns>A clone of this expression.</returns>
    public override object Clone()
    {
      ComparisonExp<T> other = (ComparisonExp<T>)base.Clone();
      other.m_arg1 = (T)this.m_arg1.Clone();
      other.m_arg2 = (T)this.m_arg2.Clone();
      return other;
    }

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public override bool Equals(Object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        ComparisonExp<T> other = (ComparisonExp<T>)obj;
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
      return this.m_arg1.GetHashCode()
           + this.m_arg2.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.m_op.ToString().ToLower());
      str.Append(" ");
      str.Append(this.m_arg1.ToString());
      str.Append(" ");
      str.Append(this.m_arg2.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.m_op.ToString().ToLower());
      str.Append(" ");
      str.Append(this.m_arg1.ToTypedString());
      str.Append(" ");
      str.Append(this.m_arg2.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

     #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this comparison expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      ComparisonExp<T> otherExp = (ComparisonExp<T>)other;

      value = this.m_arg1.CompareTo(otherExp.m_arg1);
      if (value != 0)
        return value;

      return this.m_arg2.CompareTo(otherExp.m_arg2);
    }

    #endregion
  }
}