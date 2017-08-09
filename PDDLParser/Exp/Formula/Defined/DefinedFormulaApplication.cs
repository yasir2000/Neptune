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
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A defined formula application is an application of a defined formula.
  /// When evaluated, a defined formula application performs a (possibly complex) evaluation
  /// of its body.
  /// </summary>
  public abstract class DefinedFormulaApplication : FormulaApplication
  {
    /// <summary>
    /// Creates a new defined formula application of a specified defined formula with a 
    /// given list of arguments.
    /// </summary>
    /// <param name="definedFormula">The defined formula to instantiate.</param>
    /// <param name="arguments">The arguments of the new defined formula application.</param>
    public DefinedFormulaApplication(DefinedFormula definedFormula, List<ITerm> arguments)
      : base(definedFormula, arguments)
    {
      System.Diagnostics.Debug.Assert(definedFormula != null && arguments != null && !arguments.ContainsNull());
    }
  }
}
