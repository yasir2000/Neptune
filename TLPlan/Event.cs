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
using PDDLParser;
using PDDLParser.Exp;
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Logical;
using PDDLParser.Extensions;

namespace TLPlan
{
  /// <summary>
  /// Represents an event to occur at a future time. This stores overall and end conditions and effects, as well
  /// as the action context of the operator which created it.
  /// </summary>
  public class Event : IComparable<Event>
  {
    #region Private Fields

    /// <summary>
    /// The overall condition to satisfy.
    /// </summary>
    private ILogicalExp m_overallCondition;
    /// <summary>
    /// The end condition to satisfy.
    /// </summary>
    private ILogicalExp m_endCondition;
    /// <summary>
    /// The overall effect. As of now, this is only used for "over all" preferences and conditional effects.
    /// </summary>
    private IEffect m_overallEffect;
    /// <summary>
    /// The end effect.
    /// </summary>
    private IEffect m_endEffect;

    /// <summary>
    /// The action context of this event. This is unique per operator and per use of the same operator.
    /// </summary>
    private ActionContext m_actionContext;

    /// <summary>
    /// The hashcode of the event.
    /// </summary>
    private int m_hashcode;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the overall condition to satisfy.
    /// </summary>
    public ILogicalExp OverallConditions
    {
      get { return m_overallCondition; }
    }

    /// <summary>
    /// Gets the end condition to satisfy.
    /// </summary>
    public ILogicalExp EndConditions
    {
      get { return m_endCondition; }
    }

    /// <summary>
    /// Gets the overall effect. As of now, this is only used for "over all" preferences and conditional effects.
    /// </summary>
    public IEffect OverallEffects
    {
      get { return m_overallEffect; }
    }

    /// <summary>
    /// Gets the end effect.
    /// </summary>
    public IEffect EndEffects
    {
      get { return m_endEffect; }
    }

    /// <summary>
    /// The action context of this event. This is unique per operator and per use of the same operator.
    /// </summary>
    public ActionContext ActionContext
    {
      get { return m_actionContext; }
      protected set { m_actionContext = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="overallCondition">The overall condition. If null, it defaults to true.</param>
    /// <param name="endCondition">The end condition. If null, it defaults to true.</param>
    /// <param name="overallEffect">The overall effects. If null, it defaults to no effect.</param>
    /// <param name="endEffect">The end effect. If null, it defaults to no effect.</param>
    /// <param name="actionContext">The action context associated with the event. If null, a new action context is created.</param>
    public Event(ILogicalExp overallCondition, ILogicalExp endCondition, IEffect overallEffect, IEffect endEffect, ActionContext actionContext)
	{
      m_overallCondition = overallCondition ?? TrueExp.True;
      m_endCondition = endCondition ?? TrueExp.True;
      m_overallEffect = overallEffect ?? new AndEffect();
      m_endEffect = endEffect ?? new AndEffect();

      m_actionContext = actionContext ?? new ActionContext();

      m_hashcode = m_overallCondition.GetHashCode() + m_endCondition.GetHashCode() +
                   m_overallEffect.GetHashCode() + m_endEffect.GetHashCode(); // ActionContext's hash code may change
	}

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a copy of this event. Only the action context is actually copied; the conditions and effects
    /// do not need to be copied, as they are never modified.
    /// </summary>
    /// <returns>A copy of this event.</returns>
    public Event Copy()
    {
      Event ev = (Event)this.MemberwiseClone();
      ev.ActionContext = this.ActionContext.Copy();
      // The effects and conditions do not need to be copied, as they do not change.

      return ev;
    }

    #endregion

    #region IComparable<Event> Interface

    /// <summary>
    /// Compares this event with another event.
    /// </summary>
    /// <param name="other">The other event to compare this event to.</param>
    /// <returns>An integer representing the total order relation between the two events.</returns>
    public int CompareTo(Event other)
    {
      int value = this.OverallConditions.CompareTo(other.OverallConditions);
      if (value != 0)
        return value;

      value = this.EndConditions.CompareTo(other.EndConditions);
      if (value != 0)
        return value;

      value = this.EndEffects.CompareTo(other.EndEffects);
      if (value != 0)
        return value;

      value = this.OverallEffects.CompareTo(other.OverallEffects);
      if (value != 0)
        return value;

      value = this.ActionContext.CompareTo(other.ActionContext);

      return value;
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns true if this event is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this event is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;

      if (this.GetType().Equals(obj.GetType()))
      {
        Event other = (Event)obj;

        return (this.m_overallCondition.Equals(other.m_overallCondition) &&
                this.m_endCondition.Equals(other.m_endCondition) &&
                this.m_overallEffect.Equals(other.m_overallEffect) &&
                this.m_endEffect.Equals(other.m_endEffect) &&
                this.m_actionContext.Equals(other.m_actionContext));
      }

      return false;
    }

    /// <summary>
    /// Returns the hash code of this event.
    /// </summary>
    /// <returns>The hash code of this event.</returns>
    public override int GetHashCode()
    {
      return m_hashcode + m_actionContext.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this event.
    /// </summary>
    /// <returns>A string representation of this event.</returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      AndEffect dummyEff = null;

      // TODO: Review how to verifiy that each field was properly initialized.

      if (!(m_overallCondition is TrueExp))
        sb.AppendLine("Overall condition: " + m_overallCondition.ToString());
      if ((dummyEff = m_overallEffect as AndEffect) != null && !dummyEff.IsEmpty())
        sb.AppendLine("Overall effect   : " + m_overallEffect.ToString());
      if (!(m_endCondition is TrueExp))
        sb.AppendLine("End condition    : " + m_endCondition.ToString());
      if ((dummyEff = m_endEffect as AndEffect) != null && !dummyEff.IsEmpty())
        sb.AppendLine("End effect       : " + m_endEffect.ToString());
      if (!m_actionContext.Equals(new ActionContext()))
        sb.AppendLine("Action context   : " + m_actionContext.ToString());

      return sb.ToString();
    }

    #endregion
  }
}
