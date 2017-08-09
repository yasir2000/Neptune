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
using PDDLParser.Exception;

namespace PDDLParser.Exp.Numeric.TLPlan
{
  /// <summary>
  /// This class implements the square root function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class UnarySquareRoot : UnaryFunctionExp
  {
    /// <summary>
    /// Creates a new square root function with the specified argument.
    /// </summary>
    /// <param name="arg">The argument of this new square root function.</param>
    public UnarySquareRoot(INumericExp arg)
      : base("sqrt", arg)
    {
      System.Diagnostics.Debug.Assert(arg != null);
    }

    /// <summary>
    /// Calculates the square root of the argument.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>The square root of the argument.</returns>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if
    /// the argument is negative.</exception>
    protected override double Calculate(double arg)
    {
      double result = Math.Sqrt(arg);
      if (double.IsNaN(result) || double.IsInfinity(result))
        throw new NumericException(this, Enumerable.Repeat(arg, 1));
      else
        return result;
    }
  }
}
