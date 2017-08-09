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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser;
using PDDLParser.Exp;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;
using TLPlan.Utils;
using TLPlan.World.Implementations;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World
{
  /// <summary>
  /// Updatable durative closed world implementation for TLPlan.
  /// Updates concerning facts and fluents are forwarded to the inner world responsible for
  /// storing their values.
  /// </summary>
  public class TLPlanDurativeClosedWorld : TLPlanReadOnlyDurativeClosedWorld, IDurativeClosedWorld,
                                           IComparable<TLPlanDurativeClosedWorld>
  {
    #region Properties

    /// <summary>
    /// Returns whether the world is an idle goal world (whether it satisfies a goal and
    /// has been idled).
    /// </summary>
    /// <returns>True if the world is an idle goal world, false otherwise.</returns>
    public bool IsIdleGoal
    {
      get { return m_isIdleGoal; }
      set { m_isIdleGoal = value; }
    }

    /// <summary>
    /// The world's timestamp.
    /// </summary>
    public new double TimeStamp
    {
      get { return base.TimeStamp; }
      set { m_timeStamp = value; }
    }

    /// <summary>
    /// Trajectory constraints which must not evaluate to false in current world.
    /// </summary>
    public new IConstraintExp CurrentConstraints
    {
      get { return base.CurrentConstraints; }
      set { m_currentConstraints = value; }
    }

    /// <summary>
    /// Trajectory constraints which must not evaluate to false in successors world.
    /// </summary>
    public new IConstraintExp NextConstraints
    {
      get { return base.NextConstraints; }
      set { m_nextConstraints = value; }
    }

    /// <summary>
    /// Gets the next absolute constraint timestamp at which something interesting happens.
    /// </summary>
    public new TimeValue NextAbsoluteConstraintTimestamp
    {
      get { return base.NextAbsoluteConstraintTimestamp; }
      set { m_nextAbsoluteConstraintTimestamp = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new closed durative world with the specified timestamp and inner world.
    /// </summary>
    /// <param name="timeStamp">The world timestamp.</param>
    /// <param name="openWorld">The inner world responsible for storing facts and fluents values.</param>
    /// <param name="invariantWorld">The invariant world storing invariant facts and fluents values.</param>
    /// <param name="currentConstraints">The trajectory constraints which the new world must respect.</param>
    /// <param name="options">The TLPlan options.</param>
    public TLPlanDurativeClosedWorld(double timeStamp, ExtendedOpenWorld openWorld, 
                               InvariantWorld invariantWorld, IConstraintExp currentConstraints, 
                               TLPlanOptions options)
      : base(timeStamp, openWorld, invariantWorld, currentConstraints, options)
    {
    }

    #endregion

    #region IDurativeWorld interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public virtual void Set(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(!formula.Invariant);

      m_openWorld.Set(formula);
      InvalidateEvaluationCache();
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formula">A atomic formula with constant arguments.</param>
    public void Unset(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(!formula.Invariant);

      m_openWorld.Unset(formula);
      InvalidateEvaluationCache();
    }

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public void SetNumericFluent(NumericFluentApplication fluent, double value)
    {
      System.Diagnostics.Debug.Assert(!fluent.Invariant);

      m_openWorld.SetNumericFluent(fluent, value);
      InvalidateEvaluationCache();
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public void SetObjectFluent(ObjectFluentApplication fluent, Constant value)
    {
      System.Diagnostics.Debug.Assert(!fluent.Invariant);

      m_openWorld.SetObjectFluent(fluent, value);
      InvalidateEvaluationCache();
    }

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    public void UndefineObjectFluent(ObjectFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(!fluent.Invariant);

      m_openWorld.UndefineObjectFluent(fluent);
      InvalidateEvaluationCache();
    }

    /// <summary>
    /// Add an effect which will take place after a fixed duration.
    /// </summary>
    /// <param name="timeOffset">The relative time offset at which the effect takes place.</param>
    /// <param name="effect">The delayed effect.</param>
    public void AddEndEffect(double timeOffset, IEffect effect)
    {
      this.AddEvent(timeOffset, new Event(null, null, null, effect, null));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Invalidates the evaluation cache.
    /// This is called every time the world is modified.
    /// </summary>
    protected void InvalidateEvaluationCache()
    {
      m_evaluationCache = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Modifies this world by applying the specified effect.
    /// </summary>
    /// <param name="evaluationWorld">The evaluation world used to evaluate conditional effects.
    /// </param>
    /// <param name="effect">The effect that modifies the world.</param>
    public void Modify(IReadOnlyClosedWorld evaluationWorld, IEffect effect)
    {
      this.Modify(evaluationWorld, effect, ActionContext.EmptyActionContext);
    }

    /// <summary>
    /// Modifies this world by applying the specified effect.
    /// </summary>
    /// <param name="evaluationWorld">The evaluation world used to evaluate conditional effects.
    /// </param>
    /// <param name="effect">The effect that modifies the world.</param>
    /// <param name="context">The action context.</param>
    public void Modify(IReadOnlyClosedWorld evaluationWorld, IEffect effect, ActionContext context)
    {
      System.Diagnostics.Debug.Assert(context != null);

      ExpController.Update(effect, evaluationWorld, this, context);
    }

    /// <summary>
    /// Applies the overall effects of each event is the event queue.
    /// </summary>
    /// <param name="evaluationWorld">The evaluation world used to evaluate conditional effects.
    /// </param>
    public void ApplyOverallEffects(IReadOnlyClosedWorld evaluationWorld)
    {
      // Overall effects are restricted to preferences and durative when conditions, which are 
      // produced by the parser, not the user.
      // Therefore, there should be no problem when applying these effects.

      foreach (Event ev in EventQueue.Values())
        this.Modify(evaluationWorld, ev.OverallEffects, ev.ActionContext);
    }

    /// <summary>
    /// Apply the next operators' end effect, update the world's timestamp and remove the
    /// applied effects from the list.
    /// </summary>
    /// <param name="evaluationWorld">The evaluation world used to evaluate conditional effects.
    /// </param>
    public void ApplyNextEndEffects(IReadOnlyClosedWorld evaluationWorld)
    {
      double newTimeStamp = TimeStamp;

      if (!IsEventQueueEmpty && GetNextTimestamp() == EventQueue.First.Key)
      {
        // Iterate through all effects that occur at the next timestamp
        foreach (KeyValuePair<double, Event> pair in EventQueue.RemoveFirstValues())
        {
          newTimeStamp = pair.Key;
          Event ev = pair.Value;

          this.Modify(evaluationWorld, ev.EndEffects, ev.ActionContext);
        }

        // TODO: shift timestamp only if world was actually modified

        // Set the updated timestamp
        this.TimeStamp = newTimeStamp + TLPlanReadOnlyDurativeClosedWorld.SmallTimeOffset; // Delay for at end effect/at start condition on the same formula
      }
      else
      {
        // Only update the timestamp
        this.TimeStamp = GetNextTimestamp();
      }
    }

    /// <summary>
    /// Progresses the preference constraints retrieved from the previous world.
    /// </summary>
    /// <param name="prefs">The preference constraints to progress.</param>
    public void ProgressPreferences(IEnumerable<IConstraintPrefExp> prefs)
    {
      foreach (IConstraintPrefExp pref in prefs)
      {
        ProgressionValue result = ExpController.Progress(pref.Constraint, this);
        if (result.IsFalse())
        {
          // Constraint is false; the preference has been violated. Update the world.
          AtomicFormulaApplication atom = pref.GetViolationEffect();
          System.Diagnostics.Debug.Assert(atom.AllConstantArguments);
          this.Set(atom);
        }
        else if (result.IsTrue())
        {
          // Constraint is always true; nothing to do.
        }
        else
        {
          // The constraint progressed correctly. Update it and add it in the new world's constraint preferences.
          IConstraintPrefExp newPref = (IConstraintPrefExp)pref.Clone();
          newPref.Constraint = result.Exp;
          this.ConstraintPreferences.AddLast(newPref);
          this.NextAbsoluteConstraintTimestamp = TimeValue.Min(this.NextAbsoluteConstraintTimestamp,
                                                               result.NextAbsoluteTimestamp);
        }
      }
    }

    #region EventQueue related methods

    /// <summary>
    /// Adds an event to the event queue.
    /// </summary>
    /// <param name="timeOffset">The time offset of the event.</param>
    /// <param name="ev">The event to add.</param>
    public void AddEvent(double timeOffset, Event ev)
    {
      EventQueue.InsertSorted(TimeStamp + timeOffset, ev);
    }

    #endregion

    /// <summary>
    /// Copies this durative world.
    /// </summary>
    /// <returns>A copy of this closed durative world.</returns>
    public override TLPlanDurativeClosedWorld Copy()
    {
      return (TLPlanDurativeClosedWorld)base.InternalCopy();
    }

    #endregion

    #region IComparable<DurativeClosedWorld> Interface

    /// <summary>
    /// Compares this world with another world.
    /// </summary>
    /// <param name="other">The other world to compare this world to.</param>
    /// <returns>An integer representing the total order relation between the two worlds.
    /// </returns>
    public int CompareTo(TLPlanDurativeClosedWorld other)
    {
      return base.CompareTo(other);
    }

    #endregion
  }
}
