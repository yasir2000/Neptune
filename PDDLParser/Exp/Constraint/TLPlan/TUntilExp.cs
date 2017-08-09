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
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// Represents the "t-until" constraint expression. This is a time-bounded version of
  /// TLPlan's "<see cref="UntilExp">until</see>" constraint expression. This is not part of the PDDL language;
  /// it was rather added for TLPlan.
  /// This constraint expression uses timestamps relative to the first time it is progressed.
  /// </summary>
  /// <seealso cref="SometimeExp"/>
  /// <seealso cref="UntilExp"/>
  [TLPlan]
  public class TUntilExp : BinaryConstraintExp
  {
    /// <summary>
    /// The relative time interval.
    /// </summary>
    private TimeInterval m_relativeTimeInterval;

    /// <summary>
    /// Gets the relative time interval.
    /// </summary>
    public TimeInterval RelativeTimeInterval
    {
      get { return m_relativeTimeInterval; }
    }

    /// <summary>
    /// Creates a new "t-until" constraint expression.
    /// </summary>
    /// <param name="interval">The relative time interal to when the expression is first progressed.</param>
    /// <param name="arg1">The first expression, which must always be true until the second one is.</param>
    /// <param name="arg2">The second expression. The second expression has to become true within the interval.
    /// When this one becomes true, the value of the first one becomes irrelevant. Note that the second 
    /// expression must become true before the end of the trajectory.</param>
    public TUntilExp(TimeInterval interval, IConstraintExp arg1, IConstraintExp arg2)
      : base(arg1, arg2)
    {
      System.Diagnostics.Debug.Assert(interval.LowerBound.Time >= 0 && interval.UpperBound.Time >= 0 && interval.UpperBound.Time >= interval.LowerBound.Time);

      this.m_relativeTimeInterval = interval;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This creates an instance of <see cref="AbsoluteTUntilExp"/> for progression.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    /// <seealso cref="AbsoluteTUntilExp"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new AbsoluteTUntilExp(this.RelativeTimeInterval.AddTime(world.GetTotalTime()), Exp1, Exp2).Progress(world, bindings);
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// This creates an instance of <see cref="AbsoluteTUntilExp"/> for evaluation.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    /// <seealso cref="AbsoluteTUntilExp"/>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return new AbsoluteTUntilExp(this.RelativeTimeInterval.AddTime(idleWorld.GetTotalTime()), Exp1, Exp2).EvaluateIdle(idleWorld, bindings);
    }

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
        TUntilExp other = (TUntilExp)obj;
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
      str.Append("(t-until ");
      str.Append(RelativeTimeInterval.ToString());
      str.Append(" ");
      str.Append(this.Exp1.ToTypedString());
      str.Append(" ");
      str.Append(this.Exp2.ToTypedString());
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
      str.Append("(t-until ");
      str.Append(RelativeTimeInterval.ToString());
      str.Append(" ");
      str.Append(this.Exp1.ToTypedString());
      str.Append(" ");
      str.Append(this.Exp2.ToTypedString());
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

      TUntilExp otherExp = (TUntilExp)other;
      return this.m_relativeTimeInterval.CompareTo(otherExp.m_relativeTimeInterval);
    }

    #endregion
  }
}