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
using PDDLParser.Exp.Formula;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents a quantified constraint preference expression of the PDDL language, i.e.
  /// a quantified preference that lies in a domain or a problem's constraints.
  /// </summary>
  public class ForallConstraintPrefExp : AbstractForallPrefExp, IConstraintPrefExp
  {
    #region Constructors

    /// <summary>
    /// Creates a new quantified constraint preference expression.
    /// </summary>
    /// <param name="vars">The set of variables quantifying the preference expression.</param>
    /// <param name="pref">The quantified constraint preference expression.</param>
    public ForallConstraintPrefExp(HashSet<ObjectParameterVariable> vars, IConstraintPrefExp pref)
      : base(vars, pref)
    {
      System.Diagnostics.Debug.Assert(vars != null);
      System.Diagnostics.Debug.Assert(pref != null);
    }

    #endregion

    #region IConstraintPrefExp Interface

    /// <summary>
    /// Returns the preference's corresponding dummy literal, which is used to determine whether the preference has been violated.
    /// </summary>
    /// <returns>The preference's corresponding violation literal.</returns>
    public AtomicFormulaApplication GetDummyAtomicFormula()
    {
      return GetPreference().GetDummyAtomicFormula();
    }

    /// <summary>
    /// Gets or sets the constraint of the preference (i.e. the preference's "body"). Setting this property should only be used in
    /// order to update the preference's constraint with its progressed form.
    /// </summary>
    public IConstraintExp Constraint
    {
      get { return GetPreference().Constraint; }
      set { GetPreference().Constraint = value; }
    }

    /// <summary>
    /// It is an error to call this on a quantified preference expression.
    /// Call <see cref="GetAllSubstitutedConstraintPreferences"/> first.
    /// </summary>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">Thrown because this is not supported. Call <see cref="GetAllSubstitutedConstraintPreferences"/> first.</exception>
    public AtomicFormulaApplication GetViolationEffect()
    {
      throw new NotSupportedException("GetViolationEffect() cannot be called on a quantified preference. Call GetAllSubstitutedConstraintPreferences() to obtain all the equivalent simple preferences.");
    }

    /// <summary>
    /// Returns all substituted preferences, in their simplest form. This will therefore expand preference quantifiers.
    /// </summary>
    /// <returns>All substituted and grounded preferences.</returns>
    public IEnumerable<IConstraintPrefExp> GetAllSubstitutedConstraintPreferences()
    {
      foreach (IConstraintPrefExp pref in GetPreference().GetAllSubstitutedConstraintPreferences())
      {
        IEnumerator<IExp> atomEnum = new QuantifiedExp<IExp>.BindingsEnumerable(pref.GetDummyAtomicFormula(), this.m_sortedVars).GetEnumerator();
        IEnumerator<IExp> constraintEnum = new QuantifiedExp<IExp>.BindingsEnumerable(pref.Constraint, this.m_sortedVars).GetEnumerator();

        while (atomEnum.MoveNext() && constraintEnum.MoveNext())
        {
          AtomicFormulaApplication atom = (AtomicFormulaApplication)atomEnum.Current;
          IConstraintExp constraint = (IConstraintExp)constraintEnum.Current;

          yield return new ConstraintPrefExp(this.Name, constraint, atom);
        }
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the wrapped constraint preference.
    /// </summary>
    /// <returns>The wrapped constraint preference.</returns>
    IConstraintPrefExp GetPreference()
    {
      return (IConstraintPrefExp)m_prefExp;
    }

    #endregion
  }
}
