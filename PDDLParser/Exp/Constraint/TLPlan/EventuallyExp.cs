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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// Represents the "eventually" constraint expression. This is equivalent to
  /// <see cref="SometimeExp"/> constraint expression.
  /// This is not part of the PDDL language; it was rather added for TLPlan.
  /// </summary>
  public class EventuallyExp : SometimeExp
  {
    /// <summary>
    /// Creates a new "eventually" constraint expression.
    /// </summary>
    /// <param name="arg">The body of the constraint expression.</param>
    public EventuallyExp(IConstraintExp arg)
      : base(arg)
    {
      System.Diagnostics.Debug.Assert(arg != null);
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(eventually ");
      str.Append(this.Exp.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(eventually ");
      str.Append(this.Exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }
  }
}
