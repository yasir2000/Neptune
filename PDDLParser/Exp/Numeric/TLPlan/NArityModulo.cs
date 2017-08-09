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
  /// This class implements a n-ary modulo function.
  /// If only one argument is present, then the result is that argument. 
  /// If more than two arguments are present, the calculation is left associative.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class NArityModulo : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new modulo function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new modulo function.</param>
    public NArityModulo(List<INumericExp> args)
      : base("mod", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Calculates the remainder after division of all arguments. If only one argument 
    /// is present, then the result is that argument. If more than two arguments are present, 
    /// the calculation is left associative. Taking the remainder of two floating point arguments 
    /// is perfectly legal. Division by zero is an error.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The remainder after division of all arguments.</returns>
    protected override double Calculate(double[] args)
    {
      double result = args.Aggregate((i, j) => (i - j * Math.Floor(i / j)));
      if (double.IsNaN(result) || double.IsInfinity(result))
        throw new NumericException(this, args);
      else
        return result;
    }
  }
}
