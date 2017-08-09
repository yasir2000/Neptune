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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Text;
using PDDLParser.Exp.Constraint.TLPlan;
using PDDLParser.Exp.Struct;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "within" constraint expression of the PDDL language.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  public class WithinExp : TEventuallyExp
  {
    /// <summary>
    /// Creates a new "within" constraint expression.
    /// </summary>
    /// <param name="exp">The body of the expression.</param>
    /// <param name="relativeTimestamp">The timestamp within which the body must be true. This timestamp is
    /// relative to the first progression of this constraint expression.</param>
    public WithinExp(IConstraintExp exp, double relativeTimestamp)
        : base(new TimeInterval(0, false, relativeTimestamp, false), exp)
    {
      System.Diagnostics.Debug.Assert(exp != null);

      if (relativeTimestamp < 0)
        throw new System.Exception("Error when constructing WithinExp: the relative timestamp ("
                          + relativeTimestamp + ") must be >= 0.");
      }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(within ");
      str.Append(this.RelativeTimeInterval.UpperBound.Time.ToString());
      str.Append(" ");
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
      str.Append("(within ");
      str.Append(this.RelativeTimeInterval.UpperBound.Time.ToString());
      str.Append(" ");
      str.Append(this.Exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }
  }
}
