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
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Numeric
{
  /// <summary>
  /// This class implements a n-ary substraction.
  /// If only one argument is present, then the result is its additive inverse.
  /// </summary>
  public class NAritySubstract : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new substraction function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new substraction function.</param>
    public NAritySubstract(List<INumericExp> args)
       : base("-", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Calculates the left-associative difference of all arguments.
    /// If only one argument is present, then the result is its additive inverse.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The left-associative difference of all arguments.</returns>
    protected override double Calculate(double[] args)
    {
      double diff = 0;
      int nbArgs = 0;
      foreach (double arg in args)
      {
        if (nbArgs == 0)
        {
          diff = arg;
        }
        else
        {
          diff -= arg;
        }
        ++nbArgs;
      }
      return (nbArgs == 1) ? -diff : diff;
    }
  }
}
