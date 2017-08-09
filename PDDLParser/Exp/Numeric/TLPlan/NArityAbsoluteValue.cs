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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Numeric.TLPlan
{
  /// <summary>
  /// This class implements the absolute value function, which generalizes to n arguments
  /// by computing the euclidean norm.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class NArityAbsoluteValue : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new absolute value function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new absolute value function.</param>
    public NArityAbsoluteValue(List<INumericExp> args)
      : base("abs", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Calculates the euclidean norm of the given arguments (used as a vector).
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The euclidean norm of the given arguments.</returns>
    protected override double Calculate(double[] args)
    {
      return Math.Sqrt(args.Sum(d => d*d));
    }
  }
}