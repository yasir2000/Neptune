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
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;
using TLPlan.World.Implementations;

using Double = PDDLParser.Exp.Struct.Double;
using EventQueue = TLPlan.Utils.SortedLinkedList<double, TLPlan.Event>;

namespace TLPlan.World
{
  /// <summary>
  /// Read-only durative closed world implementation for TLPlan.
  /// These are the worlds which constitute the nodes of a planning search graph.
  /// A world holds:
  /// - a timestamp
  /// - invariants information
  /// - facts and fluents values
  /// - a queue of future events
  /// - the trajectory constraints ("hard" constraints)
  /// - the trajectory constraint preferences ("soft" constraints)
  /// </summary>
  public abstract class TLPlanReadOnlyDurativeClosedWorld : IReadOnlyDurativeClosedWorld,
                                                            IComparable<TLPlanReadOnlyDurativeClosedWorld>
  {
    #region Constant Values

    /// <summary>
    /// A small time offset used not to have concurrent actions.
    /// </summary>
    public const double SmallTimeOffset = 0.001;

    #endregion

    #region Static Fields

    /// <summary>
    /// The number of worlds created up to now.
    /// </summary>
    private static int s_worldCount;

    #endregion

    #region Private Fields

    /// <summary>
    /// The world's timestamp.
    /// </summary>
    protected double m_timeStamp;

    /// <summary>
    /// Invariant world encapsulating invariant information.
    /// </summary>
    protected InvariantWorld m_invariants;

    /// <summary>
    /// Open world responsible for holding facts and fluents values.
    /// Different implementations are possible.
    /// </summary>
    protected ExtendedOpenWorld m_openWorld;

    /// <summary>
    /// Future events, sorted by their timestamps.
    /// </summary>
    protected EventQueue m_eventQueue;

    /// <summary>
    /// Trajectory constraints which must not evaluate to false in current world.
    /// Note: 
    /// - If option PRUNE_ALL_SUCCESSORS is on, the current constraints are not kept
    ///   in worlds and m_currentConstraints is thus always null.
    /// - Else (when option PRUNE_ALL_SUCCESSORS is off), the current constraints are always
    ///   kept in worlds since they are not progressed immediately. The next constraints
    ///   may be defined as well.
    /// </summary>
    protected IConstraintExp m_currentConstraints;

    /// <summary>
    /// Trajectory constraints which must not evaluate to false in successors world.
    /// - When option PRUNE_ALL_SUCCESSORS is on, the next constraints are always kept
    ///   in worlds since the current constraints are progressed immediately.
    /// - Else when option PRUNE_ALL_SUCCESSORS is off, the next constraints may be null,
    ///   i.e. in the case where a new node is generated and the current constraints haven't
    ///   been progressed yet.
    /// </summary>
    protected IConstraintExp m_nextConstraints;

    /// <summary>
    /// The list of all the constraint preferences.
    /// </summary>
    protected LinkedList<IConstraintPrefExp> m_constraintPreferences;

    /// <summary>
    /// The next absolute constraint timestamp at which something interesting happens.
    /// This can be considered as a special event.
    /// </summary>
    protected TimeValue m_nextAbsoluteConstraintTimestamp;

    /// <summary>
    /// Whether this world has been idled, which happens when the world satisfies the goal 
    /// expression and its metric evaluation has been updated to reflect violated goal
    /// constraints.
    /// </summary>
    protected bool m_isIdleGoal;

    /// <summary>
    /// The evaluation cache.
    /// </summary>
    protected EvaluationCache m_evaluationCache;

    /// <summary>
    /// TLPlan options.
    /// </summary>
    protected TLPlanOptions m_options;

    /// <summary>
    /// This world's number.
    /// </summary>
    private int m_worldNumber;

    #endregion

    #region Properties

    /// <summary>
    /// Whether the event queue is empty.
    /// </summary>
    public bool IsEventQueueEmpty
    {
      get { return m_eventQueue.Count == 0; }
    }

    /// <summary>
    /// Gets the event queue.
    /// </summary>
    public EventQueue EventQueue
    {
      get { return m_eventQueue; }
    }

    /// <summary>
    /// Gets the world timestamp.
    /// </summary>
    public double TimeStamp
    {
      get { return m_timeStamp; }
    }

    /// <summary>
    /// Gets the invariant world associated with this world.
    /// Note that all worlds belonging to the same graph are associated
    /// with the same invariant world.
    /// </summary>
    public InvariantWorld Invariants
    {
      get { return m_invariants; }
    }

    /// <summary>
    /// Gets the current constraints of this world.
    /// The current constraints are constraints that must not evaluate to false in
    /// this world.
    /// </summary>
    public IConstraintExp CurrentConstraints
    {
      get { return m_currentConstraints; }
    }

    /// <summary>
    /// Gets the constraints applicable in the future worlds.
    /// The next constraints are constraints that must not evalute to false in
    /// successor worlds.
    /// </summary>
    public IConstraintExp NextConstraints
    {
      get { return m_nextConstraints; }
    }

    /// <summary>
    /// Gets the list of constraint preferences.
    /// </summary>
    public LinkedList<IConstraintPrefExp> ConstraintPreferences
    {
      get { return m_constraintPreferences; }
    }

    /// <summary>
    /// Gets the next absolute constraint timestamp at which something interesting happens.
    /// </summary>
    public TimeValue NextAbsoluteConstraintTimestamp
    {
      get { return m_nextAbsoluteConstraintTimestamp; }
    }

    /// <summary>
    /// Gets the evaluation cache, instantiating it if necessary.
    /// </summary>
    private EvaluationCache Cache
    {
      get
      {
        if (m_evaluationCache == null)
          m_evaluationCache = new EvaluationCache();

        return m_evaluationCache;
      }
    }

    /// <summary>
    /// Gets this world's number.
    /// </summary>
    public int WorldNumber
    {
      get { return m_worldNumber; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the world count.
    /// </summary>
    static TLPlanReadOnlyDurativeClosedWorld()
    {
      // The world count is initialized to 2 to keep the same world numbers as 
      // the C version of TLPlan.
      s_worldCount = 2;
    }

    /// <summary>
    /// Creates a new read-only closed durative world with the specified timestamp and inner world.
    /// </summary>
    /// <param name="timeStamp">The world timestamp.</param>
    /// <param name="openWorld">The inner world responsible for storing facts and fluents values.</param>
    /// <param name="invariants">The invariant world storing invariant facts and fluents values.</param>
    /// <param name="currentConstraints">The trajectory constraints which the new world must respect.</param>
    /// <param name="options">The TLPlan options.</param>
    public TLPlanReadOnlyDurativeClosedWorld(double timeStamp, ExtendedOpenWorld openWorld, 
                                       InvariantWorld invariants, IConstraintExp currentConstraints,
                                       TLPlanOptions options)
    {
      m_timeStamp = timeStamp;
      m_openWorld = openWorld;
      m_invariants = invariants;
      m_currentConstraints = currentConstraints;
      m_nextConstraints = null;
      m_nextAbsoluteConstraintTimestamp = ProgressionValue.NoTimestamp;
      m_constraintPreferences = new LinkedList<IConstraintPrefExp>();
      m_eventQueue = new EventQueue();
      m_evaluationCache = new EvaluationCache();
      m_options = options;
      m_isIdleGoal = false;

      m_worldNumber = s_worldCount++;
    }

    #endregion

    #region IReadOnlyDurativeWorld interface

    /// <summary>
    /// Returns whether the world is an idle goal world (i.e. it satisfies a problem's goal).
    /// </summary>
    /// <returns>True if the world is an idle goal world, false otherwise.</returns>
    public bool IsIdleGoalWorld()
    {
      return m_isIdleGoal;
    }

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True or false.</returns>
    public bool IsSet(AtomicFormulaApplication formula)
    {
      FuzzyBool result;
      if (formula.Invariant)
      {
        result = m_invariants.IsSet(formula);

        System.Diagnostics.Debug.Assert(result == FuzzyBool.True || result == FuzzyBool.False);
      }
      else
      {
        result = m_openWorld.IsSet(formula);
      }

      return (result == FuzzyBool.True);
    }

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, or unknown.</returns>
    FuzzyBool IReadOnlyOpenWorld.IsSet(AtomicFormulaApplication formula)
    {
      return new FuzzyBool(this.IsSet(formula));
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined predicate has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined predicate application.</param>
    /// <param name="existing">This flag is set to true if the defined predicate is already in the
    /// process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined predicate and 
    /// arguments.</returns>
    public IEvaluationRecord<BoolValue> GetEvaluation(DefinedPredicateApplication pred, out bool existing)
    {
      return this.Cache.GetEvaluation(pred, out existing);
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined predicate has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined predicate application.</param>
    /// <param name="existing">This flag is set to true if the defined predicate is already in the
    /// process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined predicate and 
    /// arguments.</returns>
    IEvaluationRecord<FuzzyBoolValue> IReadOnlyOpenWorld.GetEvaluation(DefinedPredicateApplication pred, out bool existing)
    {
      return this.Cache.GetFuzzyEvaluation(pred, out existing);
    }

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Undefined, or the value of the numeric fluent.</returns>
    public Double GetNumericFluent(NumericFluentApplication fluent)
    {
      FuzzyDouble result;
      if (fluent.Invariant)
      {
        result = m_invariants.GetNumericFluent(fluent);

        System.Diagnostics.Debug.Assert(result.Status != FuzzyDouble.State.Unknown);
      }
      else
      {
        result = m_openWorld.GetNumericFluent(fluent);
      }

      return (result.Status == FuzzyDouble.State.Unknown) ? Double.Undefined :
                                                            result.ToDoubleValue();
    }

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    FuzzyDouble IReadOnlyOpenWorld.GetNumericFluent(NumericFluentApplication fluent)
    {
      return new FuzzyDouble(GetNumericFluent(fluent));
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined numeric function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined numeric function application.</param>
    /// <param name="existing">This flag is set to true if the defined numeric function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined numeric function and 
    /// arguments.</returns>
    public IEvaluationRecord<Double> GetEvaluation(DefinedNumericFunctionApplication pred, out bool existing)
    {
      return this.Cache.GetEvaluation(pred, out existing);
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined numeric function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined numeric function application.</param>
    /// <param name="existing">This flag is set to true if the defined numeric function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined numeric function and 
    /// arguments.</returns>
    IEvaluationRecord<FuzzyDouble> IReadOnlyOpenWorld.GetEvaluation(DefinedNumericFunctionApplication pred, out bool existing)
    {
      return this.Cache.GetFuzzyEvaluation(pred, out existing);
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Undefined, or a constant representing the value of
    /// the object fluent.</returns>
    public ConstantExp GetObjectFluent(ObjectFluentApplication fluent)
    {
      FuzzyConstantExp result;
      if (fluent.Invariant)
      {
        result = m_invariants.GetObjectFluent(fluent);

        System.Diagnostics.Debug.Assert(result.Status != FuzzyConstantExp.State.Unknown);
      }
      else
      {
        result = m_openWorld.GetObjectFluent(fluent);
      }

      return (result.Status == FuzzyConstantExp.State.Unknown) ? ConstantExp.Undefined :
                                                                 result.ToConstantValue();
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    FuzzyConstantExp IReadOnlyOpenWorld.GetObjectFluent(ObjectFluentApplication fluent)
    {
      return new FuzzyConstantExp(GetObjectFluent(fluent));
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined object function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined object function application.</param>
    /// <param name="existing">This flag is set to true if the defined object function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined object function and 
    /// arguments.</returns>
    public IEvaluationRecord<ConstantExp> GetEvaluation(DefinedObjectFunctionApplication pred, out bool existing)
    {
      return this.Cache.GetEvaluation(pred, out existing);
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined object function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined object function application.</param>
    /// <param name="existing">This flag is set to true if the defined object function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined object function and 
    /// arguments.</returns>
    IEvaluationRecord<FuzzyConstantExp> IReadOnlyOpenWorld.GetEvaluation(DefinedObjectFunctionApplication pred, out bool existing)
    {
      return this.Cache.GetFuzzyEvaluation(pred, out existing);
    }

    /// <summary>
    /// Returns the total time it took to reach the current world.
    /// </summary>
    /// <returns>The total time of the plan up to this point.</returns>
    public double GetTotalTime()
    {
      return TimeStamp;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether this world satisfies the specified condition.
    /// </summary>
    /// <param name="condition">The condition to test.</param>
    /// <returns>Whether this world satisfies the specified condition</returns>
    public bool Satisfies(ILogicalExp condition)
    {
      return ExpController.Evaluate(condition, this);
    }

    /// <summary>
    /// Progresses the world current constraints.
    /// This function returns false if the current constraints are not satisfied.
    /// </summary>
    /// <returns>True if the constraints have been successfully progressed, false
    /// if they evaluate to false.</returns>
    public bool ProgressCurrentConstraints()
    {
      // This function must not modify the hash code!!!

      int oldHashCode = this.GetHashCode();

      ProgressionValue value = ExpController.Progress(CurrentConstraints, this);
      this.m_nextConstraints = value.GetEquivalentExp();
      this.m_nextAbsoluteConstraintTimestamp = value.NextAbsoluteTimestamp;

      System.Diagnostics.Debug.Assert(oldHashCode == this.GetHashCode());

      return !value.IsFalse();
    }

    /// <summary>
    /// Determines whether the world is consistent by evaluating the overall conditions of 
    /// each of its queued events.
    /// </summary>
    /// <returns>Whether the world is consistent.</returns>
    public bool IsConsistent()
    {
      foreach (Event ev in m_eventQueue.Values)
        if (!this.Satisfies(ev.OverallConditions))
          return false;

      return true;
    }

    /// <summary>
    /// Returns whether the world has no more events to process.
    /// </summary>
    /// <returns>True if there are no more events to process and no timestamp to look forward to.</returns>
    public bool AreAllEventsProcessed()
    {
      return IsEventQueueEmpty && NextAbsoluteConstraintTimestamp.Equals(ProgressionValue.NoTimestamp);
    }

    /// <summary>
    /// Retrieves the next absolute timestamp, be it the next one in the event queue, or the one returned by the
    /// last progressions.
    /// </summary>
    /// <returns>The next absolute timestamp.</returns>
    public double GetNextTimestamp()
    {
      if (IsEventQueueEmpty)
        return ExtractNextTime(NextAbsoluteConstraintTimestamp);

      return Math.Min(m_eventQueue.First.Key, ExtractNextTime(NextAbsoluteConstraintTimestamp));
    }

    /// <summary>
    /// Returns the next durative conditions which the wait-for-next-event operator must verify.
    /// These conditions consist of the next end conditions contained in the list (the first values
    /// in the list always imply the end of an action) and the remaining overall conditions, which
    /// must also be satisfied.
    /// </summary>
    /// <returns>The next end and overall conditions to be assessed</returns>
    public IEnumerable<ILogicalExp> GetNextDurativeConditions()
    {
      // Extract the initial timestamp
      double initialTimestamp = GetNextTimestamp();

      foreach (KeyValuePair<double, Event> current in m_eventQueue)
      {
        if (current.Key == initialTimestamp)
          // Extract the next end condition that needs to be verified
          yield return current.Value.EndConditions;
        else
          // Extract the remaining overall condition which needs to be verified
          yield return current.Value.OverallConditions;
      }
    }

    /// <summary>
    /// Copies this read-only durative world, returning an updatable durative world.
    /// </summary>
    /// <returns>An updatable copy of this closed durative world.</returns>
    public abstract TLPlanDurativeClosedWorld Copy();

    /// <summary>
    /// Copies the internal structures of this world.
    /// </summary>
    /// <returns>A copy of this world.</returns>
    protected TLPlanReadOnlyDurativeClosedWorld InternalCopy()
    {
      TLPlanReadOnlyDurativeClosedWorld world = (TLPlanReadOnlyDurativeClosedWorld)base.MemberwiseClone();
      world.m_currentConstraints = null;
      world.m_nextConstraints = null;
      world.m_constraintPreferences = new LinkedList<IConstraintPrefExp>();
      world.m_openWorld = this.m_openWorld.Copy();
      world.m_eventQueue = new EventQueue(m_eventQueue.Select(pair => new KeyValuePair<double, Event>(pair.Key, pair.Value.Copy())));
      world.m_evaluationCache = this.m_evaluationCache;

      world.m_worldNumber = s_worldCount++;
      return world;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Calculates the hash code of the event queue.
    /// </summary>
    /// <returns>The hash code of the event queue.</returns>
    protected int GetEventQueueHashCode()
    {
      // Calculate the event queue's hashcode with the time remaining until the actions, not the absolute time.
      return m_eventQueue.Keys().Select(t => t - this.TimeStamp).GetOrderedEnumerableHashCode()
             + 31 * m_eventQueue.Values().GetOrderedEnumerableHashCode();
    }

    /// <summary>
    /// Determines whether this world's event queue is equal to another world's event queue.
    /// </summary>
    /// <param name="other">The other world.</param>
    /// <returns>Whether this world's event queue is equal the other world's event queue.</returns>
    protected bool EventQueueEquals(TLPlanReadOnlyDurativeClosedWorld other)
    {
      // Compare the event queues with the time remaining until the actions, not the absolute time.
      return (this.m_eventQueue == other.m_eventQueue ||
              (this.m_eventQueue.Keys().Select(t => t - this.TimeStamp).SequenceEqual(other.m_eventQueue.Keys().Select(t => t - other.TimeStamp))
              && this.m_eventQueue.Values().SequenceEqual(other.m_eventQueue.Values())));
    }

    /// <summary>
    /// Compares this world's event queue with another world's event queue.
    /// </summary>
    /// <param name="other">The other world.</param>
    /// <returns>An integer representing the total order relation between the two worlds'
    /// event queue.</returns>
    protected int EventQueueCompareTo(TLPlanReadOnlyDurativeClosedWorld other)
    {
      // Compare the event queues with the time remaining until the actions, not the absolute time.
      int value;
      if (this.m_eventQueue == other.m_eventQueue)
      {
        value = 0;
      }
      else
      {
        value = this.m_eventQueue.Keys().Select(t => t - this.TimeStamp).SequenceCompareTo(other.m_eventQueue.Keys().Select(t => t - other.TimeStamp));
        if (value == 0)
        {
          value = this.m_eventQueue.Values().SequenceCompareTo(other.m_eventQueue.Values);
        }
      }
      return value;
    }

    /// <summary>
    /// Extracts the next time at which an interesting event will happen, according to a time bound.
    /// </summary>
    /// <remarks>
    /// The next interesting time will differ whether the bound is lower or upper, and whether the interval
    /// is closed or not. The "interesting" time of a lower, open bound (just like an upper, closed bound)
    /// is slightly after the given time, since no change happens on the boundary itself.
    /// </remarks>
    /// <param name="value">The time value from which the next time will be extracted.</param>
    /// <returns>The next "interesting" time.</returns>
    protected double ExtractNextTime(TimeValue value)
    {
      if ((value.IsOpen && value.IsLowerBound) || (!value.IsOpen && value.IsUpperBound))
      {
        return value.Time + SmallTimeOffset;
      }
      else
      {
        return value.Time;
      }
    }

    #endregion

    #region Object interface override

    /// <summary>
    /// Returns whether this world is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this world is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      // Assume same type

      if (obj == this)
      {
        return true;
      }
      else
      {
        TLPlanReadOnlyDurativeClosedWorld other = (TLPlanReadOnlyDurativeClosedWorld)obj;
        return this.m_isIdleGoal == other.m_isIdleGoal &&
               this.EventQueueEquals(other) &&
               this.CurrentConstraints.Equals(other.CurrentConstraints) &&
               this.m_openWorld.Equals(other.m_openWorld);
      }
    }

    /// <summary>
    /// Returns the hash code of this world.
    /// </summary>
    /// <returns>The hash code of this world.</returns>
    public override int GetHashCode()
    {
        return this.m_isIdleGoal.GetHashCode()
              + 5 * this.m_openWorld.GetHashCode()
              + 17 * GetEventQueueHashCode()
              + 31 * this.CurrentConstraints.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this world.
    /// </summary>
    /// <returns>A string representation of this world.</returns>
    public override string ToString()
    {
      return m_openWorld.ToString() + "\nInvariants: " + m_invariants.ToString();
    }

    #endregion

    #region IComparable<DurativeClosedWorld> Interface

    /// <summary>
    /// Compares this world with another world.
    /// </summary>
    /// <param name="other">The other world to compare this world to.</param>
    /// <returns>An integer representing the total order relation between the two worlds.
    /// </returns>
    public int CompareTo(TLPlanReadOnlyDurativeClosedWorld other)
    {
      // hash code
      int value = this.GetHashCode().CompareTo(other.GetHashCode());
      if (value == 0)
      {
        // is idle goal
        value = this.m_isIdleGoal.CompareTo(other.m_isIdleGoal);
        if (value == 0)
        {
          // current constraints
          value = this.CurrentConstraints.CompareTo(other.CurrentConstraints);
          if (value == 0)
          {
            // event queue
            value = EventQueueCompareTo(other);
            if (value == 0)
            {
              // open world
              value = this.m_openWorld.CompareTo(other.m_openWorld);
            }
          }
        }
      }

      // TODO: this is obviously slow in debug mode...
      System.Diagnostics.Debug.Assert((value == 0) == (this.Equals(other)));

      return value;
    }

    #endregion
  }
}
