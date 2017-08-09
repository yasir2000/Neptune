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
  /// An object variable assignement is an assignment of an object to an object variable.
  /// </summary>
  [TLPlan]
  public class AssignObjectLocalVar : AssignLocalVar
  {
    /// <summary>
    /// Creates a new object variable assignment for the specified object variable and
    /// assignation expression.
    /// </summary>
    /// <param name="localVariable">The object local variable to assign a value to.</param>
    /// <param name="body">The assignation expression.</param>
    public AssignObjectLocalVar(ObjectLocalVariable localVariable, ITerm body)
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
      bindings.Bind((ObjectLocalVariable)this.m_localVariable,
                    ((ITerm)this.m_body).Evaluate(world, bindings));
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
      FuzzyConstantExp result = ((ITerm)this.m_body).Evaluate(world, bindings);
      if (result.Status == FuzzyConstantExp.State.Unknown)
      {
        return false;
      }
      else
      {
        bindings.Bind((ObjectLocalVariable)this.m_localVariable,
                      result.ToConstantValue());
        return true;
      }
    }
  }
}
