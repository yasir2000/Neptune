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
  /// This class implements the round function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class UnaryRound : UnaryFunctionExp
  {
    /// <summary>
    /// Creates a new round function with the specified argument.
    /// </summary>
    /// <param name="arg">The argument of the new round function.</param>
    public UnaryRound(INumericExp arg)
      : base("round", arg)
    {
      System.Diagnostics.Debug.Assert(arg != null);
    }

    /// <summary>
    /// Rounds the argument to the nearest integer (away from 0).
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>The argument rounded to the nearest integer (away from 0).</returns>
    protected override double Calculate(double arg)
    {
      // Round to nearest integer (away from 0).
      return Math.Round(arg, MidpointRounding.AwayFromZero);
    }
  }
}
