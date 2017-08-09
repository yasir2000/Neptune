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

using System.Collections.Generic;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents the minimize expression used in metric specification of the PDDL language.
  /// </summary>
  public class MinimizeExp : MetricExp
  {
    /// <summary>
    /// Creates a minimize metric with the given expression.
    /// </summary>
    /// <param name="exp">The expression to minimize.</param>
    /// <param name="unnamedPrefs">The unnamed preferences.</param>
    public MinimizeExp(INumericExp exp, INumericExp unnamedPrefs)
      : base(exp, unnamedPrefs)
    {
      System.Diagnostics.Debug.Assert(exp != null);
    }

    /// <summary>
    /// Gets a string representatin of the type of optimization required.
    /// </summary>
    public override string TypeName
    {
      get { return "minimize"; }
    }

    /// <summary>
    /// Returns a minimized version of the metric evaluated on a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The minimized version of the metric.</returns>
    protected override FuzzyDouble EvaluateMinimizedMetric(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return Exp.Evaluate(world, bindings);
    }

    /// <summary>
    /// Returns a minimized version of the metric evaluated on a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The minimized version of the metric.</returns>
    protected override Double EvaluateMinimizedMetric(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return Exp.Evaluate(world, bindings);
    }
  }
}