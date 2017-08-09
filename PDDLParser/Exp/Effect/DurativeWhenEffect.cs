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
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Logical;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect
{
  /// <summary>
  /// Represents a conditional effect in a durative action.
  /// </summary>
  /// <remarks>
  /// Durative conditional effects are transformed into simpler conditions, as described
  /// in section 8.1 of "PDDL2.1: An Extenstion to PDDL for Expressing Temporal Planning
  /// Domains", by Maria Fox and Derek Long.
  /// 
  /// Note that conditional effects that have their condition and their effect at the
  /// same time (i.e. at start or at end) should be transformed into a simple
  /// <seealso cref="WhenEffect"/>
  /// </remarks>
  public class DurativeWhenEffect : WhenEffect
  {
    #region Private Fields

    /// <summary>
    /// The action context specific conditions to satisfy.
    /// </summary>
    private List<KeyValuePair<AtomicFormulaApplication, bool>> m_contextConditions;

    /// <summary>
    /// The conditional effects in the action context.
    /// </summary>
    private List<AtomicFormulaApplication> m_contextEffects;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new durative conditional effect.
    /// </summary>
    /// <param name="condition">The condition to satifisy in the world. This can be null, in which case it always evaluates to true.</param>
    /// <param name="contextConditions">The conditions to satisfy in the action context.</param>
    /// <param name="effect">The conditional effect on the world. This can be null, in which case no effect will be applied.</param>
    /// <param name="contextEffects">The conditional effects on the action context.</param>
    public DurativeWhenEffect(ILogicalExp condition, IEnumerable<KeyValuePair<AtomicFormulaApplication, bool>> contextConditions,
                              IEffect effect, IEnumerable<AtomicFormulaApplication> contextEffects)
      : base(condition ?? TrueExp.True, effect ?? new AndEffect())
    {
      System.Diagnostics.Debug.Assert(contextConditions != null && !contextConditions.Keys().ContainsNull());
      System.Diagnostics.Debug.Assert(contextEffects    != null && !contextEffects.ContainsNull());

      m_contextConditions = new List<KeyValuePair<AtomicFormulaApplication, bool>>(contextConditions);
      m_contextEffects = new List<AtomicFormulaApplication>(contextEffects);
    }

    #endregion

    /// <summary>
    /// Updates the specified world and action context with this effect.
    /// A conditional effect updates the world if all its (world- and action context-specific) conditions are satisfied.
    /// </summary>
    /// <param name="evaluationWorld">World to evaluate conditions against.</param>
    /// <param name="updateWorld">World to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context to evaluate conditions against and to update.</param>
    public override void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                                LocalBindings bindings, ActionContext actionContext)
    {
      if (m_condition.Evaluate(evaluationWorld, bindings).ToBool() &&
          m_contextConditions.All(pair => actionContext.IsSet(pair.Key) == pair.Value))
      {
        m_effect.Update(evaluationWorld, updateWorld, bindings, actionContext);

        foreach (AtomicFormulaApplication atom in m_contextEffects)
          actionContext.Set(atom);
      }
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return base.IsGround() && this.m_contextConditions.All(pair => pair.Key.IsGround())
                             && this.m_contextEffects.All(atom => atom.IsGround());
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
        DurativeWhenEffect other = (DurativeWhenEffect)obj;
        return this.m_contextConditions.SequenceEqual(other.m_contextConditions) &&
               this.m_contextEffects.SequenceEqual(other.m_contextEffects);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this durative conditional effect.
    /// </summary>
    /// <returns>The hash code of this durative conditional effect.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode() + this.m_contextConditions.GetOrderedEnumerableHashCode()
                                + this.m_contextEffects.GetOrderedEnumerableHashCode();
    }

    /// <summary>
    /// Returns the hash code of this durative conditional effect.
    /// </summary>
    /// <returns>The hash code of this durative conditional effect.</returns>
    public override string ToString()
    {
      ILogicalExp cond = this.m_condition;
      IEffect eff = this.m_effect;
      
      if (m_contextConditions.Count != 0)
      {
        if (!(this.m_condition is TrueExp))
          cond = new AndExp(cond.Once().Concat(m_contextConditions.Select(pair => (pair.Value ? (ILogicalExp)pair.Key : (ILogicalExp)new NotExp(pair.Key)))));
        else
          cond = new AndExp(m_contextConditions.Select(pair => (pair.Value ? (ILogicalExp)pair.Key : (ILogicalExp)new NotExp(pair.Key))));
      }

      if (m_contextEffects.Count != 0)
      {
        if (!this.m_effect.Equals(new AndEffect()))
          eff = new AndEffect(eff.Once().Concat(m_contextEffects.Cast<IEffect>()));
        else
          eff = new AndEffect(m_contextEffects.Cast<IEffect>());
      }

      StringBuilder str = new StringBuilder();
      str.Append("(when ");
      str.Append(cond.ToString());
      str.Append(") ");
      str.Append(eff.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this durative conditional effect.
    /// </summary>
    /// <returns>A typed string representation of this durative conditional effect.</returns>
    public override string ToTypedString()
    {
      ILogicalExp cond = this.m_condition;
      IEffect eff = this.m_effect;

      if (m_contextConditions.Count != 0)
      {
        if (!(this.m_condition is TrueExp))
          cond = new AndExp(cond.Once().Concat(m_contextConditions.Select(pair => (pair.Value ? (ILogicalExp)pair.Key : (ILogicalExp)new NotExp(pair.Key)))));
        else
          cond = new AndExp(m_contextConditions.Select(pair => (pair.Value ? (ILogicalExp)pair.Key : (ILogicalExp)new NotExp(pair.Key))));
      }

      if (m_contextEffects.Count != 0)
      {
        if (!this.m_effect.Equals(new AndEffect()))
          eff = new AndEffect(eff.Once().Concat(m_contextEffects.Cast<IEffect>()));
        else
          eff = new AndEffect(m_contextEffects.Cast<IEffect>());
      }

      StringBuilder str = new StringBuilder();
      str.Append("(when ");
      str.Append(cond.ToTypedString());
      str.Append(" ");
      str.Append(eff.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this durative conditional effect with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      DurativeWhenEffect otherExp = (DurativeWhenEffect)other;

      value = this.m_contextConditions.Count.CompareTo(otherExp.m_contextConditions.Count);

      for (int i = 0; value == 0 && i < this.m_contextConditions.Count; ++i)
      {
        value = this.m_contextConditions[i].Key.CompareTo(otherExp.m_contextConditions[i].Key);
        if (value == 0)
          value = this.m_contextConditions[i].Value.CompareTo(otherExp.m_contextConditions[i].Value);
      }

      return (value != 0 ? value : this.m_contextEffects.ListCompareTo(otherExp.m_contextEffects));
    }

    #endregion
  }
}
