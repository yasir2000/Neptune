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

using System.Collections.Generic;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// Represents the base class for time-bounded constraint expression, such as "t-always" and "t-eventually"
  /// .the "t-always" constraint expression. This is not part of the PDDL language;
  /// it was rather added for TLPlan.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  /// <seealso cref="TAlwaysExp"/>
  /// <seealso cref="TEventuallyExp"/>
  [TLPlan]
  public abstract class IntervalConstraintExp : UnaryConstraintExp
  {
    /// <summary>
    /// The relative time interval.
    /// </summary>
    private TimeInterval m_relativeTimeInterval;

    /// <summary>
    /// The name of the timed-bounded constraint expression.
    /// </summary>
    private string m_name;

    /// <summary>
    /// Gets the relative time interval.
    /// </summary>
    public TimeInterval RelativeTimeInterval
    {
      get { return m_relativeTimeInterval; }
    }

    /// <summary>
    /// Creates a new "t-always" constraint expression.
    /// </summary>
    /// <param name="name">The name of the time-bounded constraint expression.</param>
    /// <param name="interval">The relative time interal to when the expression is first progressed.</param>
    /// <param name="exp">The body of the constraint expression.</param>
    public IntervalConstraintExp(string name, TimeInterval interval, IConstraintExp exp)
      : base(exp)
    {
      System.Diagnostics.Debug.Assert(interval.LowerBound.Time >= 0 
                                   && interval.UpperBound.Time >= 0 
                                   && interval.UpperBound.Time >= interval.LowerBound.Time 
                                   && exp != null);
      System.Diagnostics.Debug.Assert(name != null);

      this.m_name = name;
      this.m_relativeTimeInterval = interval;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public override abstract ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    public override abstract Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings);

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (base.Equals(obj))
      {
        IntervalConstraintExp other = (IntervalConstraintExp)obj;
        return this.m_relativeTimeInterval.Equals(other.m_relativeTimeInterval);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode() +
             13 * this.m_relativeTimeInterval.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.m_name);
      str.Append(" ");
      str.Append(RelativeTimeInterval.ToString());
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
      str.Append("(");
      str.Append(this.m_name);
      str.Append(" ");
      str.Append(RelativeTimeInterval.ToString());
      str.Append(" ");
      str.Append(this.Exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this abstract expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);

      if (value != 0)
        return value;

      IntervalConstraintExp otherExp = (IntervalConstraintExp)other;
      return this.m_relativeTimeInterval.CompareTo(otherExp.m_relativeTimeInterval);
    }

    #endregion
  }
}