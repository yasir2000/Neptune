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
  /// Represents a constraint preference (i.e. a preference that is in the :constraints part of a domain or problem).
  /// <remarks>
  /// Constraint preferences create unparsable dummy literals as a way to determine whether a preference was violated.
  /// If a preference's constraint progresses or evaluates to false, the corresponding literal is set to true. The
  /// metric's "is-violated" modalities then count the number of these literals set to true in order to compute their
  /// value.
  /// </remarks>
  /// </summary>
  public interface IConstraintPrefExp : IPrefExp
  {
    /// <summary>
    /// Returns the preference's corresponding dummy literal, which is used to determine whether the preference has been violated.
    /// </summary>
    /// <returns>The preference's corresponding violation literal.</returns>
    AtomicFormulaApplication GetDummyAtomicFormula();

    /// <summary>
    /// Gets or sets the constraint of the preference (i.e. the preference's "body"). Setting this property should only be used in
    /// order to update the preference's constraint with its progressed form.
    /// </summary>
    IConstraintExp Constraint { get; set; }

    /// <summary>
    /// Returns the effect which must be applied to the world if the preference is violated.
    /// </summary>
    /// <remarks>This is only setting a given literal to true.</remarks>
    /// <remarks>It is an error to call this on a quantified preference. Call <see cref="GetAllSubstitutedConstraintPreferences"/> first.</remarks>
    /// <returns>The effect to apply to the world upon violation of the preference.</returns>
    AtomicFormulaApplication GetViolationEffect();

    /// <summary>
    /// Returns all substituted preferences, in their simplest form. This will therefore expand preference quantifiers.
    /// </summary>
    /// <returns>All substituted and grounded preferences.</returns>
    /// <remarks>An unquantified preference returns only itself.</remarks>
    IEnumerable<IConstraintPrefExp> GetAllSubstitutedConstraintPreferences();
  }
}
