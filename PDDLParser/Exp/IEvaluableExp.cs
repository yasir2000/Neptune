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

namespace PDDLParser.Exp
{
  /// <summary>
  /// An evaluable expression is an expression which yields a (non-boolean) value when evaluated.
  /// </summary>
  public interface IEvaluableExp : IExp
  {
    ///// <summary>
    ///// Evalutes the expression in a given world.
    ///// </summary>
    ///// <param name="world">The evaluation world.</param>
    ///// <param name="bindings">A set of variable bindings.</param>
    ///// <returns>The evaluation result.</returns>
    //object Evaluate(IOpenWorld world, LocalBindings bindings, IEvaluationContext context);

    ///// <summary>
    ///// Simplifies this evaluable expression by evaluating its known expression parts.
    ///// The bindings should not be modified by this call.
    ///// The resulting expression should not contain any unbound variables, since
    ///// they are substituted according to the bindings supplied.
    ///// </summary>
    ///// <param name="world">The evaluation world.</param>
    ///// <param name="bindings">A set of variable bindings.</param>
    ///// <returns>The evaluation result.</returns>
    //EvaluableValue Simplify(IOpenWorld world, LocalBindings bindings, IEvaluationContext context);
  }
}
