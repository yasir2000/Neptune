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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula.TLPlan
{
  /// <summary>
  /// A defined function is a defined formula which yields a numeric or object value when
  /// evaluated. Its return value is set with a corresponding local variable having the
  /// same name as the function.
  /// </summary>
  [TLPlan]
  public abstract class DefinedFunction : DefinedFormula
  {
    /// <summary>
    /// Createsa a new defined function with the specified name, arguments, and local
    /// variables.
    /// </summary>
    /// <param name="image">The image of this defined function.</param>
    /// <param name="name">The name of this defined function.</param>
    /// <param name="arguments">The arguments of this defined function.</param>
    /// <param name="localVariables">The local variables used in the body of this
    /// defined function.</param>
    public DefinedFunction(string image, string name, List<ObjectParameterVariable> arguments,
                           List<ILocalVariable> localVariables)
      : base(image, name, arguments, localVariables)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull()
                                   && localVariables != null && !localVariables.ContainsNull());
    }

    /// <summary>
    /// Gets the local variable corresponding to the function's return value.
    /// </summary>
    public abstract ILocalVariable FunctionVariable { get; }
  }
}
