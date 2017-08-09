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

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// Represents a conditional expression which is equivalent to 
  /// (and (implies e1 e2) (implies (not e1) e3)). This is not part 
  /// of PDDL; it was rather added for TLPlan.
  /// </summary>
  /// <typeparam name="T">The type of the expressions.</typeparam>
  [TLPlan]
  public class AbstractIfThenElseExp<T> : AbstractExp
    where T : class, IExp
  {
    /// <summary>
    /// The condition of the if-then-else expression.
    /// </summary>
    protected T m_ifExp;

    /// <summary>
    /// The consequence of the if-then-else expression (the "then" clause).
    /// </summary>
    protected T m_thenExp;

    /// <summary>
    /// The alternate consequence of the if-then-else expression (the "else" clause).
    /// </summary>
    protected T m_elseExp;

    /// <summary>
    /// Creates a new if-then-else expression with the specified condition, consequence and
    /// alternate consequence.
    /// </summary>
    /// <param name="ifExp">The condition.</param>
    /// <param name="thenExp">The consequence (the then clause).</param>
    /// <param name="elseExp">The alternate consequence (the else clause).</param>
    public AbstractIfThenElseExp(T ifExp, T thenExp, T elseExp)
      : base()
    {
      System.Diagnostics.Debug.Assert(ifExp != null && thenExp != null && elseExp != null);

      this.m_ifExp = ifExp;
      this.m_thenExp = thenExp;
      this.m_elseExp = elseExp;
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AbstractIfThenElseExp<T> other = (AbstractIfThenElseExp<T>)base.Apply(bindings);
      other.m_ifExp = (T)this.m_ifExp.Apply(bindings);
      other.m_thenExp = (T)this.m_thenExp.Apply(bindings);
      other.m_elseExp = (T)this.m_elseExp.Apply(bindings);

      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      AbstractIfThenElseExp<T> other = (AbstractIfThenElseExp<T>)base.Standardize(images);
      other.m_ifExp = (T)this.m_ifExp.Standardize(images);
      other.m_thenExp = (T)this.m_thenExp.Standardize(images);
      other.m_elseExp = (T)this.m_elseExp.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return this.m_ifExp.IsGround() && this.m_thenExp.IsGround() && this.m_elseExp.IsGround();
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
      else if (obj.GetType() == this.GetType())
      {
        AbstractIfThenElseExp<T> other = (AbstractIfThenElseExp<T>)obj;
        return (this.m_ifExp.Equals(other.m_ifExp) &&
                this.m_thenExp.Equals(other.m_thenExp) &&
                this.m_elseExp.Equals(other.m_elseExp));
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
      return this.m_ifExp.GetHashCode() + this.m_thenExp.GetHashCode() + this.m_elseExp.GetHashCode();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      vars.UnionWith(this.m_ifExp.GetFreeVariables());
      vars.UnionWith(this.m_thenExp.GetFreeVariables());
      vars.UnionWith(this.m_elseExp.GetFreeVariables());
      return vars;
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(if-then-else ");
      str.Append(this.m_ifExp.ToString());
      str.Append(" ");
      str.Append(this.m_thenExp.ToString());
      str.Append(" ");
      str.Append(this.m_elseExp.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(if-then-else ");
      str.Append(this.m_ifExp.ToTypedString());
      str.Append(" ");
      str.Append(this.m_thenExp.ToTypedString());
      str.Append(" ");
      str.Append(this.m_elseExp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      AbstractIfThenElseExp<T> otherExp = (AbstractIfThenElseExp<T>)other;

      value = this.m_ifExp.CompareTo(otherExp.m_ifExp);
      if (value != 0)
        return value;

      value = this.m_thenExp.CompareTo(otherExp.m_thenExp);
      if (value != 0)
        return value;

      return this.m_elseExp.CompareTo(otherExp.m_elseExp);
    }

    #endregion
  }

  /// <summary>
  /// Represents a conditional constraint expression which is equivalent
  /// to (and (implies e1 e2) (implies (not e1) e3)). This is not part 
  /// of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class IfThenElseConstraintExp : AbstractIfThenElseExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new if-then-else expression with the specified condition, consequence and
    /// alternate consequence.
    /// </summary>
    /// <param name="ifExp">The condition.</param>
    /// <param name="thenExp">The consequence (the then clause).</param>
    /// <param name="elseExp">The alternate consequence (the else clause).</param>
    public IfThenElseConstraintExp(IConstraintExp ifExp, IConstraintExp thenExp, IConstraintExp elseExp)
      : base(ifExp, thenExp, elseExp)
    {
      System.Diagnostics.Debug.Assert(ifExp != null && thenExp != null && elseExp != null);
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
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      ProgressionValue value = m_ifExp.Progress(world, bindings);
      if (value.Exp != null)
      {
        ProgressionValue thenValue = m_thenExp.Progress(world, bindings);
        ProgressionValue elseValue = m_elseExp.Progress(world, bindings);

        return new ProgressionValue(new IfThenElseConstraintExp(value.Exp, thenValue.GetEquivalentExp(),
                                                                           elseValue.GetEquivalentExp()),
                                    TimeValue.Min(thenValue.NextAbsoluteTimestamp, elseValue.NextAbsoluteTimestamp));
      }
      else
      {
        if (value == ProgressionValue.True)
        {
          return m_thenExp.Progress(world, bindings);
        }
        else if (value == ProgressionValue.False)
        {
          return m_elseExp.Progress(world, bindings);
        }
        else
        {
          return value;
        }
      }
    }

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
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      Bool value = m_ifExp.EvaluateIdle(idleWorld, bindings);
      if (value == Bool.True)
      {
        return m_thenExp.EvaluateIdle(idleWorld, bindings);
      }
      else if (value == Bool.False)
      {
        return m_elseExp.EvaluateIdle(idleWorld, bindings);
      }
      else
      {
        return value;
      }
    }
  }
}
