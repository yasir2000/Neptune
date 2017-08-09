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
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Formula.TLPlan.LocalVar
{
  /// <summary>
  /// A numeric variable assignement is an assignment of a numeric value to a numeric variable.
  /// </summary>
  [TLPlan]
  public class AssignNumericLocalVar : AssignLocalVar
  {
    /// <summary>
    /// Creates a new numeric variable assignment for the specified numeric variable and 
    /// assignation expression.
    /// </summary>
    /// <param name="localVariable">The numeric local variable to assign a value to.</param>
    /// <param name="body">The assignation expression.</param>
    public AssignNumericLocalVar(NumericLocalVariable localVariable, INumericExp body)
      : base(localVariable, body)
    {
    }

    /// <summary>
    /// Binds the local variable associated with this assignment to the evaluated assignation
    /// expression.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    protected override void BindLocalVariable(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      bindings.Bind((NumericLocalVariable)this.m_localVariable,
                    ((INumericExp)this.m_body).Evaluate(world, bindings));
    }

    /// <summary>
    /// Tries and binds the local variable associated with this assignment to the evaluated assignation
    /// expression.
    /// Note that this function returns false if the assignation expression could not be evaluated.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True if the binding was successfully done.</returns>
    protected override bool TryBindLocalVariable(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyDouble result = ((INumericExp)this.m_body).Evaluate(world, bindings);
      switch (result.Status)
      {
        case FuzzyDouble.State.Unknown:
          return false;
        default:
          bindings.Bind((NumericLocalVariable)this.m_localVariable, result.ToDoubleValue());
          return true;
      }
    }
  }
}
