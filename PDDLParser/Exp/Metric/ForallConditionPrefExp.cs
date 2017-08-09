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
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Term;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents a quantified conditional preference expression of the PDDL language, i.e. a
  /// quantified preference that lies in an action's conditions or in a goal.
  /// </summary>
  public class ForallConditionPrefExp : AbstractForallPrefExp, IConditionPrefExp
  {
    #region Properties

    /// <summary>
    /// Gets or sets whether this is a goal preference. Goal preferences are only evaluated on a goal world.
    /// </summary>
    public bool IsGoalPreference
    {
      get { return GetPreference().IsGoalPreference; }
      set { GetPreference().IsGoalPreference = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new quantified condition preference expression.
    /// </summary>
    /// <param name="vars">The set of variables quantifying the preference expression.</param>
    /// <param name="pref">The quantified preference expression.</param>
    public ForallConditionPrefExp(HashSet<ObjectParameterVariable> vars, IConditionPrefExp pref)
      : base(vars, pref)
    {
      System.Diagnostics.Debug.Assert(vars != null);
    }

    #endregion

    #region IConditionPrefExp Interface

    /// <summary>
    /// Converts an action condition preference into the appropriate effect, which will increment the preference's
    /// counter every time the action is used.
    /// </summary>
    /// <param name="counter">The preference's counter</param>
    /// <returns>The effect to append to the action's effects.</returns>
    public IEffect ConvertToEffect(NumericFluentApplication counter)
    {
      return new ForallEffect(new HashSet<ObjectParameterVariable>(m_sortedVars),
                              GetPreference().ConvertToEffect(counter));
    }

    /// <summary>
    /// It is an error to call this on a quantified preference.
    /// Call <see cref="GetAllSubstitutedConditionPreferences"/> first.
    /// </summary>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">Thrown when this is called on a quantified expression.</exception>
    public ILogicalExp GetViolationCondition()
    {
      throw new NotSupportedException("GetViolationCondition() cannot be called on a quantified preference. Call GetAllSubstitutedConditionPreferences() to obtain all the equivalent simple preferences.");
    }

    /// <summary>
    /// Returns all substituted preferences in their simplest form. This will therefore expand quantified expressions.
    /// </summary>
    /// <returns>All substituted and grounded (against the preference quantifiers) preferences.</returns>
    public IEnumerable<IConditionPrefExp> GetAllSubstitutedConditionPreferences()
    {
      foreach (IConditionPrefExp pref in GetPreference().GetAllSubstitutedConditionPreferences())
      {
        IEnumerable<IExp> enumExp = new QuantifiedExp<IExp>.BindingsEnumerable(pref.GetCondition(), this.m_sortedVars);
        foreach (ILogicalExp exp in enumExp)
          yield return new ConditionPrefExp(this.Name, exp);
      }
    }

    /// <summary>
    /// Returns the original condition of the preference.
    /// </summary>
    /// <returns>The original condition of the preference.</returns>
    public ILogicalExp GetCondition()
    {
      return GetPreference().GetCondition();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the wrapped conditional preference expression.
    /// </summary>
    /// <returns>The wrapped conditional preference expression.</returns>
    IConditionPrefExp GetPreference()
    {
      return (IConditionPrefExp)m_prefExp;
    }

    #endregion
  }
}
