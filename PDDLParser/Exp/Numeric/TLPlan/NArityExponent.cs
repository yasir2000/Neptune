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
using PDDLParser.Exception;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Numeric.TLPlan
{
  /// <summary>
  /// This class implements a n-ary exponentiation function.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class NArityExponent : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new exponentiation function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new exponentiation function.</param>
    public NArityExponent(List<INumericExp> args)
      : base("expt", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Calculates the left-associative power of the arguments.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The left-associative power of the arguments.</returns>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if a negative number
    /// is exponentiated by a non-integer. Try (expt a (round b)) instead of (expt a b)
    /// </exception>
    protected override double Calculate(double[] args)
    {
      double result = args.Aggregate((i, j) => Math.Pow(i, j));
      if (double.IsNaN(result) || double.IsInfinity(result))
        throw new NumericException(this, args);
      else
        return result;
    }
  }
}