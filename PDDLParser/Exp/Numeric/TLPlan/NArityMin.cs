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
  /// This class implements the min function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class NArityMin : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new min function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new min function.</param>
    public NArityMin(List<INumericExp> args)
      : base("min", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Returns the smallest argument.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The smallest argument.</returns>
    protected override double Calculate(double[] args)
    {
      return args.Min();
    }
  }
}