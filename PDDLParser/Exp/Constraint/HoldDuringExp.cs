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
  /// Represents the "hold-during" constraint expression of the PDDL language.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  /// <remarks>
  /// This is implemented using TLPlan's "<see cref="TAlwaysExp">t-always</see>" constraint expression, using a closed
  /// lower time interval bound and an open upper bound.
  /// </remarks>
  /// <seealso cref="TAlwaysExp"/>
  /// <seealso cref="AbsoluteTAlwaysExp"/>
  public class HoldDuringExp : TAlwaysExp
  {
    /// <summary>
    /// Creates a new "hold-during" constraint expression.
    /// </summary>
    /// <param name="lowerRelativeTimestamp">The lower bound of the time interval, relatively to when the expression is progressed.</param>
    /// <param name="upperRelativeTimestamp">The upper bound of the time interval, relatively to when the expression is progressed.</param>
    /// <param name="arg">The body of the constraint expression.</param>
    public HoldDuringExp(double lowerRelativeTimestamp, double upperRelativeTimestamp, IConstraintExp arg)
      : base(new TimeInterval(lowerRelativeTimestamp, false, upperRelativeTimestamp, true), arg)
    {
      if (lowerRelativeTimestamp < 0 || upperRelativeTimestamp < 0)
        throw new System.Exception("Error when constructing HoldDuringExp: both the upper timestamp ("
                          + upperRelativeTimestamp + ") and the lower timestamp ("
                          + lowerRelativeTimestamp + ") must be >= 0.");
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(hold-during ");
      str.Append(this.RelativeTimeInterval.LowerBound.Time.ToString());
      str.Append(" ");
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
      str.Append("(hold-during ");
      str.Append(this.RelativeTimeInterval.LowerBound.Time.ToString());
      str.Append(" ");
      str.Append(this.RelativeTimeInterval.UpperBound.Time.ToString());
      str.Append(" ");
      str.Append(this.Exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }
  }
}