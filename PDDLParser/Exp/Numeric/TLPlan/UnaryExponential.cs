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
  /// This class implements the exponential function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class UnaryExponential : UnaryFunctionExp
  {
    /// <summary>
    /// Creates a new exponential function with the specified exponent.
    /// </summary>
    /// <param name="exponent">The exponent of the new exponential function.</param>
    public UnaryExponential(INumericExp exponent)
      : base("exp", exponent)
    {
      System.Diagnostics.Debug.Assert(exponent != null);
    }

    /// <summary>
    /// Calculates the exponential function of the argument (the exponentiation of the 
    /// Euler number to the given exponent).
    /// </summary>
    /// <param name="exponent">The exponent.</param>
    /// <returns>The exponential function of the argument.</returns>
    protected override double Calculate(double exponent)
    {
      return Math.Exp(exponent);
    }
  }
}
