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
  /// This class implements the logarithm function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class BinaryLogarithm : BinaryFunctionExp
  {
    /// <summary>
    /// Creates a new logarithm function with the specified number and base.
    /// </summary>
    /// <param name="n">A number.</param>
    /// <param name="lBase">The logarithm base.</param>
    public BinaryLogarithm(INumericExp n, INumericExp lBase)
      : base("log", n, lBase)
    {
    }

    /// <summary>
    /// Calculates the logarithm of a given number in a given base.
    /// </summary>
    /// <param name="n">The number.</param>
    /// <param name="lBase">The logarithm base.</param>
    /// <returns>The logarithm of a given number in a given base.</returns>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if the base is less than 
    /// or equal to 1 or the number is less than or equal to 0.</exception>
    protected override double Calculate(double n, double lBase)
    {
      double result = Math.Log(n, lBase);
      if (double.IsNaN(result) || double.IsInfinity(result))
        throw new NumericException(this, new double[] { n, lBase });
      else
        return result;
    }
  }
}
