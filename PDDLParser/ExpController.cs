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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exception;
using PDDLParser.Exp;
using PDDLParser.Exp.Metric;
using PDDLParser.Exp.Struct;
using PDDLParser.World;
using PDDLParser.World.Context;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser
{
  /// <summary>
  /// An expression controller supplies high-level methods for manipulating expressions.
  /// </summary>
  public static class ExpController
  {
    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// The expression to evaluate must be ground since no bindings are supplied.
    /// </summary>
    /// <param name="exp">The logical expression to evaluate.</param>
    /// <param name="world">The evaluation world.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public static FuzzyBool Evaluate(ILogicalExp exp, IReadOnlyOpenWorld world)
    {
      return Evaluate(exp, world, LocalBindings.EmptyBindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world, with the given
    /// set of bindings.
    /// </summary>
    /// <param name="exp">The logical expression to evaluate.</param>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A given set of bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public static FuzzyBool Evaluate(ILogicalExp exp, IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return exp.Evaluate(world, bindings);
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// The expression to simplify must be ground since no bindings are supplied.
    /// </summary>
    /// <param name="exp">The logical expression to simplify.</param>
    /// <param name="world">The evaluation world.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public static LogicalValue Simplify(ILogicalExp exp, IReadOnlyOpenWorld world)
    {
      return Simplify(exp, world, LocalBindings.EmptyBindings);
    }

    /// <summary>
    /// Simplifies this logical expression with a given set of bindings by evaluating
    /// its known expression parts.
    /// </summary>
    /// <param name="exp">The logical expression to simplify.</param>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A given set of bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public static LogicalValue Simplify(ILogicalExp exp, IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return exp.Simplify(world, bindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// The expression to evaluate must be ground since no bindings are supplied.
    /// </summary>
    /// <param name="exp">The logical expression to evaluate.</param>
    /// <param name="world">The evaluation world.</param>
    /// <returns>True or false.</returns>
    public static bool Evaluate(ILogicalExp exp, IReadOnlyClosedWorld world)
    {
      return Evaluate(exp, world, LocalBindings.EmptyBindings);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world, with the given
    /// set of bindings.
    /// </summary>
    /// <param name="exp">The logical expression to evaluate.</param>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A given set of bindings.</param>
    /// <returns>True or false.</returns>
    public static bool Evaluate(ILogicalExp exp, IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return exp.Evaluate(world, bindings).ToBool();
    }

    /// <summary>
    /// Evaluates the given metric against a specified durative closed world.
    /// Note that a durative world is required to evaluate a metric.
    /// </summary>
    /// <param name="metric">The metric to evaluate.</param>
    /// <param name="world">The evaluation world.</param>
    /// <returns>True or false.</returns>
    public static double EvaluateMetric(MetricExp metric, IReadOnlyDurativeClosedWorld world)
    {
      // At first sight, metric.Evaluate() does not require a durative world, but casting
      // from ReadOnlyClosedWorld to ReadOnlyDurativeClosedWorld is performed during the evaluation.
      Double result = metric.Evaluate(world, LocalBindings.EmptyBindings);

      if (result.Status == Double.State.Undefined)
        throw new UndefinedExpException("Evaluating the metric yields an undefined value.");

      return result.Value;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This function returns false if this constraint expression is not satisfied 
    /// in the given world;
    /// it returns true if the progression is always satisfied in the next worlds;
    /// else it returns the progressed expression.
    /// The expression to progress must be ground since no bindings are supplied.
    /// </summary>
    /// <param name="exp">The constraint expression to progress.</param>
    /// <param name="world">The current world.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    public static ProgressionValue Progress(IConstraintExp exp, IReadOnlyDurativeClosedWorld world)
    {
      return Progress(exp, world, LocalBindings.EmptyBindings);
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds,
    /// given a set of bindings.
    /// This function returns false if this constraint expression is not satisfied 
    /// in the given world;
    /// it returns true if the progression is always satisfied in the next worlds;
    /// else it returns the progressed expression.
    /// </summary>
    /// <param name="exp">The constraint expression to progress.</param>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    public static ProgressionValue Progress(IConstraintExp exp, IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return exp.Progress(world, bindings);
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="exp">The constraint expression to evaluate against an idle world.</param>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <returns>True, false, or undefined.</returns>
    public static bool EvaluateIdle(IConstraintExp exp, IReadOnlyDurativeClosedWorld idleWorld)
    {
      return EvaluateIdle(exp, idleWorld, LocalBindings.EmptyBindings);
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="exp">The constraint expression to evaluate against an idle world.</param>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public static bool EvaluateIdle(IConstraintExp exp, IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return exp.EvaluateIdle(idleWorld, bindings).ToBool();
    }

    /// <summary>
    /// Updates the specified world with this effect.
    /// </summary>
    /// <param name="effect">The effect used to update the world.</param>
    /// <param name="evaluationWorld">The world to evaluate conditions against. Note that this is 
    /// usually the un-modified version of the world to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    public static void Update(IEffect effect, IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld)
    {
      Update(effect, evaluationWorld, updateWorld, ActionContext.EmptyActionContext);
    }

    /// <summary>
    /// Updates the specified world with this effect, given an action context.
    /// </summary>
    /// <param name="effect">The effect used to update the world.</param>
    /// <param name="evaluationWorld">The world to evaluate conditions against. Note that this is 
    /// usually the un-modified version of the world to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public static void Update(IEffect effect, IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                              ActionContext actionContext)
    {
      Update(effect, evaluationWorld, updateWorld, actionContext, LocalBindings.EmptyBindings);
    }

    /// <summary>
    /// Updates the specified world with this effect, given an action context of a set of
    /// variable bindings.
    /// </summary>
    /// <param name="effect">The effect used to update the world.</param>
    /// <param name="evaluationWorld">The world to evaluate conditions against. Note that this is 
    /// usually the un-modified version of the world to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    public static void Update(IEffect effect, IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                              ActionContext actionContext, LocalBindings bindings)
    {
      effect.Update(evaluationWorld, updateWorld, bindings, actionContext);
    }
  }
}
