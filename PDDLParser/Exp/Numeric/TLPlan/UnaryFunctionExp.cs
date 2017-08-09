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
  /// Base class for unary arithmetic functions.
  /// </summary>
  [TLPlan]
  public abstract class UnaryFunctionExp : AbstractFunctionExp
  {
    /// <summary>
    /// Creates a new unary arithmetic function with the specified argument.
    /// </summary>
    /// <param name="image">The image of the arithmetic function.</param>
    /// <param name="arg">The argument of the new arithmetic function.</param>
    public UnaryFunctionExp(string image, INumericExp arg)
      : base(image, Enumerable.Repeat(arg, 1).ToList())
    {
      System.Diagnostics.Debug.Assert(arg != null);
    }

    /// <summary>
    /// Calculates the result of this arithmetic function called with the specified argument.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>The result of the arithmetic function.</returns>
    protected abstract double Calculate(double arg);

    /// <summary>
    /// Calculates the result of this arithmetic function called with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments to call the function with.</param>
    /// <returns>The result of the arithmetic function.</returns>
    protected override double Calculate(double[] args)
    {
      return Calculate(args[0]);
    }  
  }
}