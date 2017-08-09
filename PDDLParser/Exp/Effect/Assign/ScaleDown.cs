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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System.Collections.Generic;
using PDDLParser.Exception;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Effect.Assign
{
  /// <summary>
  /// This class represents a fluent scale-down operation.
  /// </summary>
  public sealed class ScaleDown : NumericAssignEffect
  {
    /// <summary>
    /// Creates a new fluent scale-down operation.
    /// </summary>
    /// <param name="head">The numeric fluent whose value is to be scaled down.</param>
    /// <param name="body">The numeric value to scale down the fluent by.</param>
    public ScaleDown(NumericFluentApplication head, INumericExp body)
      : base("scale-down", head, body)
    {
      System.Diagnostics.Debug.Assert(head != null && body != null);
    }

    /// <summary>
    /// Updates the world with this fluent assignment.
    /// </summary>
    /// <param name="head">The evaluated fluent application to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    protected override void UpdateWorldWithAssignEffect(FluentApplication head, IDurativeOpenWorld updateWorld,
                                                        LocalBindings bindings)
    {
      FuzzyDouble headValue = ((NumericFluentApplication)head).Evaluate(updateWorld, bindings);
      FuzzyDouble bodyValue = Body.Evaluate(updateWorld, bindings);

      if (headValue.Status != FuzzyDouble.State.Defined)
      {
        throw new UndefinedExpException(this.ToString() +
          " failed since the first operand evaluates to undefined or unknown.");
      }
      else if (bodyValue.Status != FuzzyDouble.State.Defined)
      {
        throw new UndefinedExpException(this.ToString() +
          " failed since the second operand evaluates to undefined or unknown.");
      }
      else
      {
        double result = headValue.Value / bodyValue.Value;
        if (double.IsNaN(result) || double.IsInfinity(result))
        {
          throw new NumericException(this, new double[] { headValue.Value, bodyValue.Value });
        }
        else
        {
          updateWorld.SetNumericFluent((NumericFluentApplication)head, result);
        }
      }
    }
  }
}