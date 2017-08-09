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

using PDDLParser.Extensions;

namespace PDDLParser.Exp
{
  /// <summary>
  /// Base class for list expressions, i.e. expressions which contain other expressions.
  /// </summary>
  /// <typeparam name="T">The type of the expressions.</typeparam>
  public abstract class ListExp<T> : AbstractExp, IEnumerable<T>
    where T : class, IExp
  {
    /// <summary>
    /// The list of expressions contained in this list expression.
    /// </summary>
    protected List<T> m_expressions;

    /// <summary>
    /// The image of this list expression.
    /// </summary>
    protected string m_image;

    /// <summary>
    /// Creates a new list expression which will store the specified expressions.
    /// </summary>
    /// <param name="image">The image of this list expression.</param>
    /// <param name="exps">The expressions to store.</param>
    protected ListExp(string image, IEnumerable<T> exps)
      : base()
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());

      this.m_image = image;
      this.m_expressions = new List<T>();
      foreach (T exp in exps)
      {
        AddElement(exp);
      }
    }
   
    /// <summary>
    /// Adds a new expression to this list expression.
    /// Note that this method is protected and should be called only in the constructor.
    /// The default implementation is to add the expression to the list.
    /// </summary>
    /// <param name="elt">The new expression to add to this list expression.</param>
    protected virtual void AddElement(T elt)
    {
      this.m_expressions.Add(elt);
    }
    
    /// <summary>
    /// Returns an enumerator over the expressions stored this list expression.
    /// </summary>
    /// <returns>An enumerator over the expressions stored this list expression.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      return this.m_expressions.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator over the expressions stored this list expression.
    /// </summary>
    /// <returns>An enumerator over the expressions stored this list expression.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<T>)this).GetEnumerator();
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this list
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The variable bindings.</param>
    /// <returns>A substituted copy of this list expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      ListExp<T> other = (ListExp<T>)base.Apply(bindings);
      other.m_expressions = new List<T>(this.m_expressions.Count);
      foreach (T exp in this)
      {
        other.m_expressions.Add((T)exp.Apply(bindings));
      }
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
      ListExp<T> other = (ListExp<T>)base.Standardize(images);
      other.m_expressions = new List<T>(this.m_expressions.Count);
      foreach (T exp in this.m_expressions)
      {
        other.m_expressions.Add((T)exp.Standardize(images));
      }
      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      foreach (T exp in this)
      {
        if (!exp.IsGround())
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
      foreach (T exp in this.m_expressions)
      {
        vars.UnionWith(exp.GetFreeVariables());  
      }
      return vars;
    }

    /// <summary>
    /// Returns true if this list expression is equal to a specified object.
    /// By default, two list expressions are equal if they have the same arguments in
    /// the same order.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this list expression is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        ListExp<T> other = (ListExp<T>)obj;
        return this.m_expressions.SequenceEqual(other.m_expressions);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this list expression.
    /// </summary>
    /// <returns>The hash code of this list expression.</returns>
    public override int GetHashCode()
    {
      return this.m_expressions.GetOrderedEnumerableHashCode();
    }

    /// <summary>
    /// Returns a clone of this list expression.
    /// </summary>
    /// <returns>A clone of this list expression.</returns>
    public override object Clone()
    {
      ListExp<T> other = (ListExp<T>)base.Clone();
      other.m_expressions = new List<T>(this.m_expressions);

      return other;
    }

    /// <summary>
    /// Returns a string representation of this conjunctive expression.
    /// </summary>
    /// <returns>A string representation of this conjunctive expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(m_image);
      foreach (T elt in this.m_expressions)
      {
        str.Append(" ");
        str.Append(elt.ToString());
      }
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this conjunctive expression.
    /// </summary>
    /// <returns>A typed string representation of this conjunctive expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(m_image);
      foreach (T elt in this.m_expressions)
      {
        str.Append(" ");
        str.Append(elt.ToTypedString());
      }
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Members

    /// <summary>
    /// Compares this list expression with another expression.
    /// </summary>
    /// <param name="obj">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp obj)
    {
      int value = base.CompareTo(obj);
      if (value != 0)
        return value;

      ListExp<T> other = (ListExp<T>)obj;
      value = this.m_expressions.Count.CompareTo(other.m_expressions.Count);
      if (value != 0)
        return value;

      for (int i = 0; i < this.m_expressions.Count; ++i)
      {
        value = this.m_expressions[i].CompareTo(other.m_expressions[i]);
        if (value != 0)
          return value;
      }
      return value;
    }

    #endregion
  }
}