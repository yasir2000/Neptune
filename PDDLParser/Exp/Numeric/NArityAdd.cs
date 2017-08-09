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
  /// This class implements a n-ary addition.
  /// </summary>
  public class NArityAdd : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new addition function with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments of the new addition function.</param>
    public NArityAdd(List<INumericExp> args)
      : base("+", args)
    {
      System.Diagnostics.Debug.Assert(args != null && !args.ContainsNull());
    }

    /// <summary>
    /// Calculates the sum of all the arguments.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The sum of all the arguments.</returns>
    protected override double Calculate(double[] args)
    {
      return args.Sum();
    }
  }
}