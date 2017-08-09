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

namespace PDDLParser.Exp
{
  /// <summary>
  /// Represents a conditional preference, be it in an action's conditions, or in a goal.
  /// <remarks>
  /// Preferences in action conditions are transformed into counters which are incremented
  /// every time the action is used.
  /// </remarks>
  /// </summary>
  public interface IConditionPrefExp : IPrefExp
  {
    /// <summary>
    /// Converts an action condition preference into the appropriate effect, which will increment the preference's
    /// counter every time the action is used.
    /// </summary>
    /// <param name="counter">The preference's counter</param>
    /// <returns>The effect to append to the action's effects.</returns>
    IEffect ConvertToEffect(NumericFluentApplication counter);

    /// <summary>
    /// Returns the violation condition of this preference. This is only to be used on goal preferences.
    /// The violation condition is true if the preference has been violated.
    /// </summary>
    /// <remarks>It is an error to call this on a quantified preference. Call <see cref="GetAllSubstitutedConditionPreferences"/> first.</remarks>
    /// <returns>The violation condition of the preference.</returns>
    ILogicalExp GetViolationCondition();

    /// <summary>
    /// Returns all substituted preferences in their simplest form. This will therefore expand quantified expressions.
    /// </summary>
    /// <returns>All substituted and grounded (against the preference quantifiers) preferences.</returns>
    /// <remarks>An unquantified preference returns only itself.</remarks>
    IEnumerable<IConditionPrefExp> GetAllSubstitutedConditionPreferences();

    /// <summary>
    /// Gets or sets whether this is a goal preference. Goal preferences are only evaluated on a goal world.
    /// </summary>
    bool IsGoalPreference { get; set; }

    /// <summary>
    /// Returns the original condition of the preference.
    /// </summary>
    /// <returns>The original condition of the preference.</returns>
    ILogicalExp GetCondition();
  }
}
