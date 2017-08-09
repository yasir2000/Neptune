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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents the common part of all quantified preference expressions of the PDDL language.
  /// </summary>
  public abstract class AbstractForallPrefExp : AbstractExp, IPrefExp
  {
    #region Private Fields

    /// <summary>
    /// The set of quantified variables contained in this quantified preference expression.
    /// They are kept in a list to allow efficient comparisons between two quantified expressions.
    /// </summary>
    protected List<ObjectParameterVariable> m_sortedVars;

    /// <summary>
    /// The wrapped preference expression used as a base.
    /// </summary>
    protected IPrefExp m_prefExp;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new quantified preference expression.
    /// </summary>
    /// <param name="vars">The set of variables quantifying the preference expression.</param>
    /// <param name="pref">The quantified preference expression.</param>
    public AbstractForallPrefExp(HashSet<ObjectParameterVariable> vars, IPrefExp pref)
    {
      System.Diagnostics.Debug.Assert(vars != null);
      System.Diagnostics.Debug.Assert(pref != null);

      this.m_prefExp = pref;
      this.m_sortedVars = new List<ObjectParameterVariable>(vars);
      this.m_sortedVars.Sort();
    }

    #endregion

    #region IPrefExp Members

    /// <summary>
    /// Returns whether the preference is unnamed.
    /// </summary>
    /// <seealso cref="IPrefExp.Unnamed"/>
    public bool Unnamed
    {
      get { return m_prefExp.Unnamed; }
    }

    /// <summary>
    /// Returns the name of the preferences.
    /// </summary>
    /// <seealso cref="IPrefExp.Name"/>
    public string Name
    {
      get { return m_prefExp.Name; }
    }

    /// <summary>
    /// Returns the body of the preference.
    /// </summary>
    /// <returns>The body of the preference.</returns>
    public IExp GetOriginalExp()
    {
      return m_prefExp.GetOriginalExp();
    }

    #endregion

    /// <summary>
    /// Returns true if the preference is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the preference is ground.</returns>
    public override bool IsGround()
    {
      return m_prefExp.IsGround();
    }

    /// <summary>
    /// Returns the free variables in this preference.
    /// </summary>
    /// <returns>The free variables in this preference.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      vars.UnionWith(this.m_prefExp.GetFreeVariables());
      vars.ExceptWith(this.m_sortedVars.Cast<Variable>());
      return vars;
    }

    /// <summary>
    /// Returns true if this preference is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this preference is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        AbstractForallPrefExp other = (AbstractForallPrefExp)obj;
        return this.m_sortedVars.SequenceEqual(other.m_sortedVars)
            && this.m_prefExp.Equals(other.m_prefExp);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this preference.
    /// </summary>
    /// <returns>The hash code of this preference.</returns>
    public override int GetHashCode()
    {
      return this.m_sortedVars.GetOrderedEnumerableHashCode() + 53 * this.m_prefExp.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this preference.
    /// </summary>
    /// <returns>A string representation of this preference.</returns>
    public override string ToString()
    {
      return string.Format("(forall ({0}) {1})",
                           string.Join(" ", m_sortedVars.Select(var => var.ToString()).ToArray()),
                           m_prefExp.ToString());
    }

    /// <summary>
    /// Returns a typed string of this preference.
    /// </summary>
    /// <returns>A typed string representation of this preference.</returns>
    public override string ToTypedString()
    {
      return string.Format("(forall ({0}) {1})",
                           string.Join(" ", m_sortedVars.Select(var => var.ToTypedString()).ToArray()),
                           m_prefExp.ToTypedString());
    }
  }
}
