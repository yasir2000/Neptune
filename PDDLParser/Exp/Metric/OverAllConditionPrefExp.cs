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
using PDDLParser.Action;
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Effect.Assign;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Logical;
using PDDLParser.Extensions;
using Number = PDDLParser.Exp.Numeric.Number;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents a conditional preference expression of the PDDL language occuring
  /// in an "over all" condition of a durative action.
  /// </summary>
  /// <remarks>
  /// This class wraps another <see cref="IConditionPrefExp"/> preference.
  /// </remarks>
  public class OverAllConditionPrefExp : AbstractExp, IConditionPrefExp
  {
    #region Private Fields

    /// <summary>
    /// The wrapped <see cref="IConditionPrefExp"/> preference.
    /// </summary>
    protected IConditionPrefExp m_prefExp;
    /// <summary>
    /// The <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> used in the 
    /// action context which tracks whether the preference has been violated.
    /// </summary>
    protected AtomicFormulaApplication m_atom;
    /// <summary>
    /// The action in which this preference occurs.
    /// </summary>
    protected DurativeAction m_action;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new "over all" preference expression.
    /// </summary>
    /// <param name="pref">The wrapped preference.</param>
    /// <param name="action">The action in which this preference occurs.</param>
    public OverAllConditionPrefExp(IConditionPrefExp pref, DurativeAction action)
    {
      System.Diagnostics.Debug.Assert(pref   != null);
      System.Diagnostics.Debug.Assert(action != null);

      this.m_prefExp = pref;
      this.m_atom = null;
      this.m_action = action;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the effect that must be applied after the over all conditions have been asserted.
    /// </summary>
    /// <remarks>
    /// The over all effect is used to update, in the action context, whether the preference has been
    /// violated. It by no means modifies the world itself.
    /// </remarks>
    /// <returns>The effect that must be applied after the over all conditions have been asserted.</returns>
    public IEffect GetOverallEffect()
    {
      return new DurativeWhenEffect(new NotExp(GetCondition()),
                                    new KeyValuePair<AtomicFormulaApplication, bool>(m_atom, false).Once(),
                                    null,
                                    m_atom.Once());
    }

    /// <summary>
    /// Sets the <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> used 
    /// in the action context which tracks whether the preference has been violated.
    /// </summary>
    /// <param name="atom">The <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> used 
    /// in the action context which tracks whether the preference has been violated.</param>
    public void SetAtom(AtomicFormulaApplication atom)
    {
      this.m_atom = atom;
    }

    /// <summary>
    /// Gets the durative action in which the preference occurs.
    /// </summary>
    /// <returns>The durative action which the preference occurs.</returns>
    public DurativeAction GetAction()
    {
      return this.m_action;
    }

    #endregion

    #region IConditionPrefExp Members

    /// <summary>
    /// Converts an action condition preference into the appropriate effect, which will increment the preference's
    /// counter every time the action is used.
    /// </summary>
    /// <param name="counter">The preference's counter</param>
    /// <returns>The effect to append to the action's effects.</returns>
    /// <remarks>The counter will be incremented only if the predicate <see cref="m_atom"/> is set in the action context.</remarks>
    /// <seealso cref="IConditionPrefExp.ConvertToEffect"/>
    public IEffect ConvertToEffect(NumericFluentApplication counter)
    {
      return new DurativeWhenEffect(null,
                                    new KeyValuePair<AtomicFormulaApplication, bool>(m_atom, true).Once(),
                                    new Increase(counter, new Number(1)),
                                    Enumerable.Empty<AtomicFormulaApplication>());
    }

    /// <summary>
    /// Returns all substituted preferences in their simplest form. This will therefore expand quantified expressions.
    /// </summary>
    /// <returns>All substituted and grounded (against the preference quantifiers) preferences.</returns>
    /// <remarks>An unquantified preference returns only itself.</remarks>
    public IEnumerable<IConditionPrefExp> GetAllSubstitutedConditionPreferences()
    {
      foreach (IConditionPrefExp pref in m_prefExp.GetAllSubstitutedConditionPreferences())
        yield return new OverAllConditionPrefExp(pref, this.GetAction());
    }

    /// <summary>
    /// It is an error to call this method on <see cref="OverAllConditionPrefExp"/>, as this is surely not a goal preference.
    /// See <see cref="IConditionPrefExp.GetViolationCondition"/>.
    /// </summary>
    /// <returns>Nothing; throws an exception.</returns>
    /// <exception cref="NotSupportedException">Thrown when this is called on <see cref="OverAllConditionPrefExp"/>.</exception>
    /// <seealso cref="IConditionPrefExp.GetViolationCondition"/>
    public ILogicalExp GetViolationCondition()
    {
      throw new NotSupportedException("GetViolationCondition() is reserved for goal preferences. An overall preference cannot be in a goal.");
    }

    /// <summary>
    /// Gets whether this is a goal preference. Always returns false for <see cref="OverAllConditionPrefExp"/>.
    /// </summary>
    /// <remarks>It is an error to set a <see cref="OverAllConditionPrefExp"/> as a goal preference.</remarks>
    /// <exception cref="NotSupportedException">Thrown when trying to set this property on <see cref="OverAllConditionPrefExp"/>.</exception>
    public bool IsGoalPreference
    {
      get { return false; }
      set
      {
        throw new NotSupportedException("An overall preference cannot be in a goal.");
      }
    }

    /// <summary>
    /// Returns the original condition of the preference.
    /// </summary>
    /// <returns>The original condition of the preference.</returns>
    public ILogicalExp GetCondition()
    {
      return m_prefExp.GetCondition();
    }

    #endregion

    #region IPrefExp Interface

    /// <summary>
    /// Gets whether this preference is unnamed.
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
      return m_prefExp.GetFreeVariables();
    }

    /// <summary>
    /// Returns true if this preference is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this preference is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }
      else if (this.GetType().Equals(obj.GetType()))
      {
        OverAllConditionPrefExp other = (OverAllConditionPrefExp)obj;
        return this.m_prefExp.Equals(other.m_prefExp) &&
               this.m_action.Equals(other.m_action) &&
               ((this.m_atom == null && other.m_atom == null) ||
                (this.m_atom != null && other.m_atom != null && this.m_atom.Equals(other.m_atom)));
      }

      return false;
    }

    /// <summary>
    /// Returns the hash code of this preference.
    /// </summary>
    /// <returns>The hash code of this preference.</returns>
    public override int GetHashCode()
    {
      return this.m_prefExp.GetHashCode() + 17 * this.m_action.GetHashCode() + 31 * (this.m_atom != null ? this.m_atom.GetHashCode() : 0);
    }

    /// <summary>
    /// Returns a typed string of this preference.
    /// </summary>
    /// <returns>A typed string representation of this preference.</returns>
    public override string ToTypedString()
    {
      return m_prefExp.ToTypedString();
    }

    /// <summary>
    /// Returns a string representation of this preference.
    /// </summary>
    /// <returns>A string representation of this preference.</returns>
    public override string ToString()
    {
      return m_prefExp.ToString();
    }
  }
}
