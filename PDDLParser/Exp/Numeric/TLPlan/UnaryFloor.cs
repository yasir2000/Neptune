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
  /// This class implements the floor function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class UnaryFloor : UnaryFunctionExp
  {
    /// <summary>
    /// Creates a new floor function with the specified argument.
    /// </summary>
    /// <param name="arg">The argument of the new floor function.</param>
    public UnaryFloor(INumericExp arg)
      : base("floor", arg)
    {
      System.Diagnostics.Debug.Assert(arg != null);
    }

    /// <summary>
    /// Calculates the largest integer less than or equal the argument.
    /// </summary>
    /// <param name="arg">The number.</param>
    /// <returns>The largest integer less than or equal the argument.</returns>
    protected override double Calculate(double arg)
    {
      // Returns the largest integer less than or equal.
      return Math.Floor(arg);
    }
  }
}
