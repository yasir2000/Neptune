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
  /// A boolean variable assignement as an assignement of a boolean value to a boolean variable.
  /// </summary>
  [TLPlan]
  public class AssignBooleanLocalVar : AssignLocalVar
  {
    /// <summary>
    /// Creates a new boolean variable assignement for the specified boolean local variable
    /// and assignation expression.
    /// </summary>
    /// <param name="localVariable">The boolean local variable to assign a value to.</param>
    /// <param name="body">The assignation expression.</param>
    public AssignBooleanLocalVar(BooleanLocalVariable localVariable, ILogicalExp body)
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
      bindings.Bind((BooleanLocalVariable)this.m_localVariable,
                    ((ILogicalExp)this.m_body).Evaluate(world, bindings));
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
      FuzzyBool result = ((ILogicalExp)this.m_body).Evaluate(world, bindings);
      if (result == FuzzyBool.Unknown)
      {
        return false;
      }
      else
      {
        bindings.Bind((BooleanLocalVariable)this.m_localVariable,
                      result.ToBoolValue());
        return true;
      }
    }
  }
}
