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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Formula;
using PDDLParser.Extensions;
using PDDLParser.World.Context;

namespace PDDLParser
{
  /// <summary>
  /// A context containing private action-specific data. This is mostly used for durative "when" effects.
  /// </summary>
  public class ActionContext : IComparable<ActionContext>
  {
    #region Private Fields

    /// <summary>
    /// The set of all true atomic formulas.
    /// </summary>
    private HashSet<AtomicFormulaApplication> m_facts;

    /// <summary>
    /// The sorted list of all true atomic formulas. This is used in the <see cref="CompareTo"/> method.
    /// </summary>
    private List<AtomicFormulaApplication> m_sortedFacts;

    /// <summary>
    /// The cached hash code of this action context.
    /// </summary>
    private int m_hashcode;

    /// <summary>
    /// The empty action context.
    /// </summary>
    private static ActionContext s_emptyActionContext = new ActionContext();

    #endregion

    #region Properties

    /// <summary>
    /// Gets the empty action context.
    /// </summary>
    public static ActionContext EmptyActionContext
    {
      get { return s_emptyActionContext; }
    }

    /// <summary>
    /// Returns the sorted list of all true atomic formulas. This list is cached;
    /// it is assumed that the atomic formulas will not be modified once this
    /// property is called, unless a copy occurs.
    /// </summary>
    private List<AtomicFormulaApplication> SortedFacts
    {
      get
      {
        if (m_sortedFacts == null)
        {
          m_sortedFacts = new List<AtomicFormulaApplication>(m_facts);
          m_sortedFacts.Sort();
        }

        return m_sortedFacts;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an instance of the action context with no true atomic formulas.
    /// </summary>
    public ActionContext()
    {
      m_facts = new HashSet<AtomicFormulaApplication>();
      m_sortedFacts = null;

      m_hashcode = 0;
    }

    #endregion

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public void Set(AtomicFormulaApplication formula)
    {
      if (m_facts.Add(formula))
        m_hashcode += formula.GetHashCode();
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public void Unset(AtomicFormulaApplication formula)
    {
      if (m_facts.Remove(formula))
        m_hashcode -= formula.GetHashCode();
    }

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this context.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True or false.</returns>
    public bool IsSet(AtomicFormulaApplication formula)
    {
      return m_facts.Contains(formula);
    }

    /// <summary>
    /// Returns a copy the this action context, clearing the sorted cache.
    /// </summary>
    /// <returns>A copy of the action context.</returns>
    public ActionContext Copy()
    {
      ActionContext actionContext = (ActionContext)this.MemberwiseClone();

      actionContext.m_facts = new HashSet<AtomicFormulaApplication>(this.m_facts);
      actionContext.m_sortedFacts = null;

      return actionContext;
    }

    #region IComparable<ActionContext> Interface

    /// <summary>
    /// Compares this action context with another action context.
    /// </summary>
    /// <param name="other">The other action context to compare this action context to.</param>
    /// <returns>An integer representing the total order relation between the two action contexts.</returns>
    public int CompareTo(ActionContext other)
    {
      int value = this.GetHashCode().CompareTo(other.GetHashCode());
      if (value != 0)
        return value;

      // We do not compare contexts of different types... yet.
      ActionContext context = (ActionContext)other;

      return this.SortedFacts.ListCompareTo(context.SortedFacts);
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns true if this action context is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this action context is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;

      if (this.GetType() == obj.GetType())
      {
        ActionContext other = (ActionContext)obj;
        return this.m_facts.SetEquals(other.m_facts);
      }

      return false;
    }

    /// <summary>
    /// Returns the cached hashcode of this action context.
    /// </summary>
    /// <returns>The hashcode of this action context.</returns>
    public override int GetHashCode()
    {
      return m_hashcode;
    }

    /// <summary>
    /// Returns a string representation of this action context.
    /// </summary>
    /// <returns>A string representation of this action context.</returns>
    public override string ToString()
    {
      return m_facts.Aggregate(new StringBuilder(), (sb, atom) => { sb.Append(atom + " "); return sb; }).ToString();
    }

    #endregion
  }
}
