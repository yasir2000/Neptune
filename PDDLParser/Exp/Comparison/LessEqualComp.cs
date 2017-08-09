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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PDDLParser.Exp.Struct;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Comparison
{
  /// <summary>
  /// This class implements the less or equal numeric comparison.
  /// </summary>
  public class LessEqualComp : NumericCompExp
  {
    /// <summary>
    /// Creates a new less or equal numeric comparison.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    public LessEqualComp(INumericExp arg1, INumericExp arg2)
      : base("<=", arg1, arg2)
    {
      System.Diagnostics.Debug.Assert(arg1 != null && arg2 != null);
    }

    /// <summary>
    /// Compares the two arguments. This comparison should return undefined if at least
    /// one of the two arguments is undefined.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>True, false, or undefined.</returns>
    protected override Bool Compare(Double arg1, Double arg2)
    {
      return arg1 <= arg2;
    }

    /// <summary>
    /// Compares the two arguments. This comparison should return undefined if at least one 
    /// of the two arguments is undefined, or unknown if at least one of the two arguments is 
    /// unknown.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    protected override FuzzyBool Compare(FuzzyDouble arg1, FuzzyDouble arg2)
    {
      return arg1 <= arg2;
    }
  }
}