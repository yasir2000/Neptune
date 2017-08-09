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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PDDLParser.Extensions;

namespace PDDLParser.Exp.Numeric.TLPlan
{
  /// <summary>
  /// Base class for binary arithmetic functions.
  /// </summary>
  [TLPlan]
  public abstract class BinaryFunctionExp : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new binary arithmetic function with the two specified arguments.
    /// </summary>
    /// <param name="image">The image of the arithmetic function.</param>
    /// <param name="arg1">The first argument of the new arithmetic function.</param>
    /// <param name="arg2">The second argument of the new arithmetic function.</param>
    public BinaryFunctionExp(string image, INumericExp arg1, INumericExp arg2)
      : base(image, new List<INumericExp>(new INumericExp[] { arg1, arg2 }))
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);
    }

    /// <summary>
    /// Calculates the result of this arithmetic function called with the specified two arguments.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>The result of the arithmetic function.</returns>
    protected abstract double Calculate(double arg1, double arg2);

    /// <summary>
    /// Calculates the result of this arithmetic function called with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The result of the arithmetic function.</returns>
    protected override double Calculate(double[] args)
    {
      return Calculate(args[0], args[1]);
    }  
  }
}