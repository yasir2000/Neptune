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
using System.Text;

namespace PDDLParser.Exp.Numeric.TLPlan
{
  /// <summary>
  /// This class implements a truncate function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class UnaryTruncate : UnaryFunctionExp
  {
    /// <summary>
    /// Creates a new truncate function with the specified argument.
    /// </summary>
    /// <param name="arg">The argument of the new truncate function.</param>
    public UnaryTruncate(INumericExp arg)
      : base("int", arg)
    {
      System.Diagnostics.Debug.Assert(arg != null);
    }

    /// <summary>
    /// Truncates the argument to the nearest integer (toward 0).
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>The nearest integer toward 0.</returns>
    protected override double Calculate(double arg)
    {
      // Truncate to nearest integer (toward 0).
      return Math.Truncate(arg);
    }
  }
}
