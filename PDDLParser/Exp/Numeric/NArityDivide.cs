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
// Please note that this file was inspired in part by the PDDL4J library:
// http://www.math-info.univ-paris5.fr/~pellier/software/software.php 
//
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System.Collections.Generic;
using System.Linq;
using PDDLParser.Exception;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Numeric
{
  /// <summary>
  /// This class implements a n-ary division.
  /// If only one argument is given, the result is the multiplicative inverse.
  /// </summary>
  public class NArityDivide : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new division function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new division function.</param>
    public NArityDivide(List<INumericExp> args)
      : base("/", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Calculates the left-associative quotient of the arguments.
    /// If only one argument is given, the result is the multiplicative inverse.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The left-associative quotient of the arguments.</returns>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if a division
    /// by 0 occurs.</exception>
    protected override double Calculate(double[] args)
    {
      double result = args.Aggregate((i, j) => i / j);
      if (double.IsNaN(result) || double.IsInfinity(result))
        throw new NumericException(this, args);
      else
        return result;
    }
  }
}
