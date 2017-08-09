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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp
{
  /// <summary>
  /// Base class for quantified expressions, which are defined on the domains of variables. 
  /// </summary>
  public abstract class QuantifiedExp<T> : AbstractExp, IEnumerable<ObjectParameterVariable>
    where T : class, IExp
  {
    /// <summary>
    /// The set of quantified variables contained in this quantified expression.
    /// They are kept in a list to allow efficient comparisons between two quantified expressions.
    /// </summary>
    protected List<ObjectParameterVariable> m_sortedVars;

    /// <summary>
    /// The body of this quantified expression.
    /// </summary>
    protected T m_body;

    /// <summary>
    /// A ground expression equivalent to this quantified expression.
    /// </summary>
    private T m_equivalentExp;

    /// <summary>
    /// The image of this quantified expression.
    /// </summary>
    private string m_image;

    /// <summary>
    /// Creates a new quantified expression with the specified set of quantified variables and body.
    /// </summary>
    /// <param name="image">The image of the quantified expression.</param>
    /// <param name="vars">The set of quantified variables.</param>
    /// <param name="body">The body of the new quantified expression</param>
    protected QuantifiedExp(string image, HashSet<ObjectParameterVariable> vars, T body)
      : base()
    {
      System.Diagnostics.Debug.Assert(body != null && vars != null && !vars.ContainsNull());

      this.m_image = image;
      this.m_sortedVars = new List<ObjectParameterVariable>(vars);
      this.m_sortedVars.Sort();
      this.m_body = body;

      foreach (ObjectParameterVariable var in vars)
        var.TypeDomainChanged += new ObjectVariable.VariableEventHandler(Variable_TypeDomainChanged);
    }

    /// <summary>
    /// Handler to be called when the domain of a variable changes.
    /// </summary>
    /// <param name="sender">The variable whose domain has changed.</param>
    private void Variable_TypeDomainChanged(ObjectVariable sender)
    {
      m_equivalentExp = null;
    }

    /// <summary>
    /// Returns a ground expression equivalent to this quantified expression.
    /// Such an expression always exists since variables' domain are finite.
    /// </summary>
    /// <returns>A ground expression equivalent to this quantified expression.</returns>
    protected T GetEquivalentExp()
    {
      if (m_equivalentExp == null)
      {
        m_equivalentExp = GenerateEquivalentExp();
      }
      return m_equivalentExp;
    }

    /// <summary>
    /// Creates a new ground expression equivalent to this quantified expression.
    /// </summary>
    /// <returns>A new ground expression equivalent to this quantified expression.</returns>
    protected abstract T GenerateEquivalentExp();

    /// <summary>
    /// Returns an enumerable over all expressions which correspond to all substitutions of the
    /// quantified variables applied on this quantified expression's body.
    /// </summary>
    /// <returns>An enumerable over all substitutions of the body of this expression</returns>
    protected IEnumerable<T> GetBodySubstitutions()
    {
      return new BindingsEnumerable(this.m_body, this.m_sortedVars).Cast<T>();
    }

    /// <summary>
    /// Returns an enumerator over the quantified variables defined in this quantified expression.
    /// </summary>
    /// <returns>An enumerator over the quantified variables defined in this quantified expression.</returns>
    public IEnumerator<ObjectParameterVariable> GetEnumerator()
    {
      return this.m_sortedVars.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator over the quantified variables defined in this quantified expression.
    /// </summary>
    /// <returns>An enumerator over the quantified variables defined in this quantified expression.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      QuantifiedExp<T> other = (QuantifiedExp<T>)base.Apply(bindings);
      other.m_body = (T)this.m_body.Apply(bindings);
      other.m_equivalentExp = null;

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
      IDictionary<string, string> newImages = new Dictionary<string, string>(images);
      QuantifiedExp<T> other = (QuantifiedExp<T>)base.Standardize(images);
      other.m_sortedVars = new List<ObjectParameterVariable>();
      foreach (ObjectParameterVariable var in this.m_sortedVars)
      {
        other.m_sortedVars.Add((ObjectParameterVariable)Variable.standardizeQuantifiedVariable(var, newImages));
      }
      other.m_body = (T)other.m_body.Standardize(newImages);
      foreach (KeyValuePair<string, string> e in newImages)
      {
        if (!images.ContainsKey(e.Key))
        {
          images.Add(e.Key, e.Value);
        }
      }
      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_body.IsGround();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return this.m_body.GetFreeVariables();
    }

    /// <summary>
    /// Returns whether this quantified expression is equal to another object.
    /// Two quantified expressions are equal if they are defined over the same quantified variables
    /// and their body is equal.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this quantified expression is equal to another object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        QuantifiedExp<T> other = (QuantifiedExp<T>)obj;
        return this.m_sortedVars.SequenceEqual(other.m_sortedVars)
            && this.m_body.Equals(other.m_body);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this quantified expression.
    /// </summary>
    /// <returns>The hash code of this quantified expression.</returns>
    public override int GetHashCode()
    {
      return this.m_sortedVars.GetOrderedEnumerableHashCode() + this.m_body.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this quantified expression.
    /// </summary>
    /// <returns>A string representation of this quantified expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(" + this.m_image + " ");
      str.Append("(");
      if (this.m_sortedVars.Count != 0)
      {
        IEnumerator<ObjectParameterVariable> i = this.GetEnumerator();
        i.MoveNext();
        str.Append(i.Current.ToString());
        while (i.MoveNext())
        {
          str.Append(" " + i.Current.ToString());
        }
      }
      str.Append(")");
      str.Append(" ");
      str.Append(this.m_body.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this quantified expression.
    /// </summary>
    /// <returns>A typed string representation of this quantified expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(" + this.m_image + " ");
      str.Append("(");
      if (this.m_sortedVars.Count != 0)
      {
        IEnumerator<ObjectParameterVariable> i = this.GetEnumerator();
        i.MoveNext();
        str.Append(i.Current.ToTypedString());
        while (i.MoveNext())
        {
          str.Append(" " + i.Current.ToTypedString());
        }
      }
      str.Append(")");
      str.Append(" ");
      str.Append(this.m_body.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this quantified expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      QuantifiedExp<T> otherExp = (QuantifiedExp<T>)other;

      value = this.m_sortedVars.Count.CompareTo(otherExp.m_sortedVars.Count);
      if (value != 0)
        return value;

      value = this.m_sortedVars.SequenceCompareTo(otherExp.m_sortedVars);
      if (value != 0)
        return value;

      return this.m_body.CompareTo(otherExp.m_body);
    }

    #endregion

    #region private class BindingsEnumerator

    /// <summary>
    /// Enumerator over all the substitutions of an expression given a set of quantified
    /// variables.
    /// </summary>
    private class BindingsEnumerator : IEnumerator<IExp>
    {
      /// <summary>
      /// The original expression.
      /// </summary>
      private IExp m_originalExp;
      /// <summary>
      /// The current substituted copy of the original expression.
      /// </summary>
      private IExp m_currentSubstitutedExp;
      /// <summary>
      /// The cached domain of the quantified variables.
      /// </summary>
      private List<KeyValuePair<ObjectParameterVariable, IEnumerator<Constant>>> m_domains;
      /// <summary>
      /// The current set of bindings.
      /// </summary>
      private ParameterBindings m_currentBindings;

      /// <summary>
      /// Creates a new binding enumerator for the specified expression and quantified
      /// variables.
      /// </summary>
      /// <param name="exp">The expression used in the substitutions.</param>
      /// <param name="variables">The variables used for the substitutions.</param>
      public BindingsEnumerator(IExp exp, IEnumerable<ObjectParameterVariable> variables)
      {
        this.m_originalExp = exp;
        this.m_domains = new List<KeyValuePair<ObjectParameterVariable, IEnumerator<Constant>>>();
        foreach (ObjectParameterVariable variable in variables)
        {
          IEnumerator<Constant> values = variable.GetTypeSet().Domain.GetEnumerator();
          this.m_domains.Add(new KeyValuePair<ObjectParameterVariable, IEnumerator<Constant>>(variable, values));
        }
        Reset();
      }

      /// <summary>
      /// Gets the element in the collection at the current position of the enumerator.
      /// </summary>
      public IExp Current
      {
        get { return m_currentSubstitutedExp; }
      }

      /// <summary>
      /// Gets the element in the collection at the current position of the enumerator.
      /// </summary>
      object System.Collections.IEnumerator.Current
      {
        get { return Current; }
      }

      /// <summary>
      /// Advances the enumerator to the next element of the collection.
      /// </summary>
      /// <returns>True if the enumerator was successfully advanced to the next element; 
      /// false if the enumerator has passed the end of the collection.</returns>
      public bool MoveNext()
      {
        int dim = m_domains.Count;
        if (m_currentBindings == null)
        {
          m_currentBindings = new ParameterBindings();
          for (int i = 0; i < dim; ++i)
          {
            ObjectParameterVariable var = m_domains[i].Key;
            IEnumerator<Constant> values = m_domains[i].Value;
            values.Reset();
            if (!values.MoveNext())
              return false;
            m_currentBindings.Bind(var, values.Current);
          }
          m_currentSubstitutedExp = m_originalExp.Apply(m_currentBindings);
          return true;
        }
        else
        {
          for (int i = dim - 1; i >= 0; --i)
          {
            ObjectParameterVariable variable = m_domains[i].Key;
            IEnumerator<Constant> values = m_domains[i].Value;
            if (values.MoveNext())
            {
              m_currentBindings.Bind(variable, values.Current);
              m_currentSubstitutedExp = m_originalExp.Apply(m_currentBindings);
              return true;
            }
            values.Reset();
            values.MoveNext();
            m_currentBindings.Bind(variable, values.Current);
          }
          return false;
        }
      }

      /// <summary>
      /// Sets the enumerator to its initial position, which is before the first element
      /// in the collection.
      /// </summary>
      public void Reset()
      {
        m_currentBindings = null;
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or
      /// resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
      }
    }

    #endregion

    #region Nested class BindingsEnumerable

    /// <summary>
    /// Enumerable over all the substitutions of an expression given a set of quantified
    /// variables.
    /// </summary>
    internal class BindingsEnumerable : IEnumerable<IExp>
    {
      /// <summary>
      /// The original expression.
      /// </summary>
      private IExp m_originalExp;
      /// <summary>
      /// The quantified variables over which the substitutions are defined.
      /// </summary>
      private IEnumerable<ObjectParameterVariable> m_vars;

      /// <summary>
      /// Creates a new enumerable of bindings given an expression and a set of
      /// quantified variables.
      /// </summary>
      /// <param name="exp">The expression to substitute.</param>
      /// <param name="vars">The quantified variables over which the substitutions are defined.
      /// </param>
      public BindingsEnumerable(IExp exp, IEnumerable<ObjectParameterVariable> vars)
      {
        this.m_originalExp = exp;
        this.m_vars = vars;
      }

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that iterates through the collection.</returns>
      public IEnumerator<IExp> GetEnumerator()
      {
        return new BindingsEnumerator(m_originalExp, m_vars);
      }

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that iterates through the collection.</returns>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }

    #endregion
  }
}