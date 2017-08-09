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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PDDLParser.Exp.Formula;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents a constraint preference expression of the PDDL language, i.e. a preference
  /// that lies in a domain or a problem's constraints.
  /// </summary>
  public class ConstraintPrefExp : AbstractPrefExp, IConstraintPrefExp
  {
    /// <summary>
    /// The associated <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> 
    /// which tracks whether this preference is violated.
    /// </summary>
    protected AtomicFormulaApplication m_atom;

    /// <summary>
    /// Creates a new constraint preference expression.
    /// </summary>
    /// <param name="name">The preference's name.</param>
    /// <param name="exp">The preference's body.</param>
    /// <param name="atom">The <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> 
    /// which tracks whether this preference is violated.</param>
    /// <param name="unnamed">Whether this preference is unnamed.</param>
    public ConstraintPrefExp(string name, IConstraintExp exp, AtomicFormulaApplication atom, bool unnamed)
      : base(name, exp, unnamed)
    {
      System.Diagnostics.Debug.Assert(exp != null);
      System.Diagnostics.Debug.Assert(atom != null);

      this.m_atom = atom;
    }

    /// <summary>
    /// Creates a new named constraint preference expression.
    /// </summary>
    /// <param name="name">The preference's name.</param>
    /// <param name="exp">The preference's body.</param>
    /// <param name="atom">The <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> 
    /// which tracks whether this preference is violated.</param>
    public ConstraintPrefExp(string name, IConstraintExp exp, AtomicFormulaApplication atom)
      : this(name, exp, atom, false)
    { }

    #region IConstraintPrefExp Interface

    /// <summary>
    /// Returns the preference's corresponding dummy literal, which is used to determine whether the preference has been violated.
    /// </summary>
    /// <returns>The preference's corresponding violation literal.</returns>
    public AtomicFormulaApplication GetDummyAtomicFormula()
    {
      return m_atom;
    }

    /// <summary>
    /// Gets or sets the constraint of the preference (i.e. the preference's "body"). Setting this property should only be used in
    /// order to update the preference's constraint with its progressed form.
    /// </summary>
    public IConstraintExp Constraint
    {
      get { return (IConstraintExp)m_exp; }
      set { m_exp = value; }
    }

    /// <summary>
    /// Returns the effect which must be applied to the world if the preference is violated.
    /// </summary>
    /// <remarks>This is only setting a given literal to true.</remarks>
    /// <returns>The effect to apply to the world upon violation of the preference.</returns>
    public AtomicFormulaApplication GetViolationEffect()
    {
      return m_atom;
    }

    /// <summary>
    /// Returns all substituted preferences, in their simplest form. This will therefore expand preference quantifiers.
    /// </summary>
    /// <returns>An enumerable containing only this preference.</returns>
    /// <seealso cref="IConstraintPrefExp.GetAllSubstitutedConstraintPreferences"/>
    public IEnumerable<IConstraintPrefExp> GetAllSubstitutedConstraintPreferences()
    {
      yield return this;
    }

    #endregion
  }
}