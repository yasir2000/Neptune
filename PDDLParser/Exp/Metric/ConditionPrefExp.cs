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
using PDDLParser.Exp.Effect.Assign;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Logical;
using PDDLParser.Extensions;
using Number = PDDLParser.Exp.Numeric.Number;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents a conditional preference expression of the PDDL language, i.e. a preference
  /// that lies in an action's conditions or in a goal.
  /// </summary>
  public class ConditionPrefExp : AbstractPrefExp, IConditionPrefExp
  {
    #region Private Fields

    /// <summary>
    /// Whether this is a goal preference.
    /// </summary>
    private bool m_isGoalPreference;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets whether this is a goal preference. Goal preferences are only evaluated on a goal world.
    /// </summary>
    public bool IsGoalPreference
    {
      get { return m_isGoalPreference; }
      set { m_isGoalPreference = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new condition preference expression.
    /// </summary>
    /// <param name="name">The name of this preference.</param>
    /// <param name="exp">The body of the preference.</param>
    /// <param name="unnamed">Whether this preference is unnamed.</param>
    public ConditionPrefExp(string name, ILogicalExp exp, bool unnamed)
      : base(name, exp, unnamed)
    {
      m_isGoalPreference = false;
    }

    /// <summary>
    /// Creates a new named condition preference expression.
    /// </summary>
    /// <param name="name">The name of this preference.</param>
    /// <param name="exp">The body of the preference.</param>
    public ConditionPrefExp(string name, ILogicalExp exp)
      : this(name, exp, false)
    { }

    #endregion

    #region IConditionPrefExp Interface

    /// <summary>
    /// Converts an action condition preference into the appropriate effect, which will increment the preference's
    /// counter every time the action is used.
    /// </summary>
    /// <param name="counter">The preference's counter</param>
    /// <returns>The effect to append to the action's effects.</returns>
    public virtual IEffect ConvertToEffect(NumericFluentApplication counter)
    {
      return new WhenEffect(new NotExp(GetCondition()), new Increase(counter, new Number(1)));
    }

    /// <summary>
    /// Returns the violation condition of this preference. This is only to be used on goal preferences.
    /// The violation condition is true if the preference has been violated.
    /// </summary>
    /// <returns>The violation condition of the preference.</returns>
    public ILogicalExp GetViolationCondition()
    {
      return new NotExp(GetCondition());
    }

    /// <summary>
    /// Returns all substituted preferences in their simplest form.
    /// </summary>
    /// <returns>An enumeration containing only this preference.</returns>
    /// <seealso cref="IConditionPrefExp.GetAllSubstitutedConditionPreferences"/>
    public IEnumerable<IConditionPrefExp> GetAllSubstitutedConditionPreferences()
    {
      yield return this;
    }

    /// <summary>
    /// Returns the original condition of the preference.
    /// </summary>
    /// <returns>The original condition of the preference.</returns>
    public ILogicalExp GetCondition()
    {
      return (ILogicalExp)m_exp;
    }

    #endregion
  }
}
