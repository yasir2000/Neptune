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
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents the "is-violated" expression of the PDDL language.
  /// </summary>
  public class IsViolatedExp : ReservedNumericExp
  {
    /// <summary>
    /// The name of the preference to monitor.
    /// </summary>
    private string m_prefName;

    /// <summary>
    /// The preference counter which counts action condition preferences violations.
    /// </summary>
    private NumericFluentApplication m_counter;
    /// <summary>
    /// The list of <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> 
    /// objects that track constraint preferences violations.
    /// </summary>
    private List<AtomicFormulaApplication> m_constraintPrefs;
    /// <summary>
    /// The list of goal preferences.
    /// </summary>
    private List<ILogicalExp> m_goalPrefs;

    /// <summary>
    /// Gets the name of the preference to monitor.
    /// </summary>
    public string PreferenceName { get { return m_prefName; } }

    /// <summary>
    /// Creates a new "is-violated" expression.
    /// </summary>
    /// <param name="prefName">The name of the preference to monitor.</param>
    /// <param name="counter">The preference counter which counts action condition preferences violations. This can be <see langword="null"/>.</param>
    public IsViolatedExp(string prefName,
                         NumericFluentApplication counter)
      : base("is-violated")
    {
      System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(prefName));

      this.m_prefName = prefName;
      this.m_counter = counter; // Can be null (no counter)
      this.m_constraintPrefs = new List<AtomicFormulaApplication>();
      this.m_goalPrefs = new List<ILogicalExp>();
    }

    /// <summary>
    /// Sets the goal preferences.
    /// </summary>
    /// <param name="goalPrefs">The goal preferences.</param>
    public void SetGoalPreferences(IEnumerable<ILogicalExp> goalPrefs)
    {
      System.Diagnostics.Debug.Assert(goalPrefs != null && !goalPrefs.ContainsNull());
      this.m_goalPrefs = new List<ILogicalExp>(goalPrefs);
    }

    /// <summary>
    /// Sets the list of <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> 
    /// objects that track constraint preferences violations.
    /// </summary>
    /// <param name="constraintPrefs">The list of <see cref="PDDLParser.Exp.Formula.AtomicFormulaApplication"/> 
    /// objects that track constraint preferences violations.</param>
    public void SetConstraintPreferences(IEnumerable<AtomicFormulaApplication> constraintPrefs)
    {
      System.Diagnostics.Debug.Assert(constraintPrefs != null && !constraintPrefs.ContainsNull());
      this.m_constraintPrefs = new List<AtomicFormulaApplication>(constraintPrefs);
    }

    /// <summary>
    /// Evaluates this "is-violated" expression in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    public override FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      // Count each violated trajectory preference, add all violated goal preferences, and add the value of the preference counter (tracking action condition preferences)
      return new FuzzyDouble(m_constraintPrefs.Count(exp => exp.Evaluate(world, bindings) == FuzzyBool.True) +
                            (((IReadOnlyDurativeOpenWorld)world).IsIdleGoalWorld() ? m_goalPrefs.Count(exp => exp.Evaluate(world, bindings) == FuzzyBool.True) : 0) +
                            (m_counter != null ? m_counter.Evaluate(world, bindings).Value : 0));
    }

    /// <summary>
    /// Evaluates this "is-violated" expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    public override Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      // Count each violated trajectory preference, add all violated goal preferences, and add the value of the preference counter (tracking action condition preferences)
      return new Double(m_constraintPrefs.Count(exp => exp.Evaluate(world, bindings) == Bool.True) +
                       (((IReadOnlyDurativeOpenWorld)world).IsIdleGoalWorld() ? m_goalPrefs.Count(exp => exp.Evaluate(world, bindings) == Bool.True) : 0) +
                       (m_counter != null ? m_counter.Evaluate(world, bindings).Value : 0));
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      return string.Format("(is-violated {0})", m_prefName);
    }

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode() + 31 * m_prefName.GetHashCode();
    }
  }
}
