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

using System.Diagnostics;

namespace TLPlan
{
  /// <summary>
  /// Represents statistics gathered during the parsing and solving of a PDDL problem.
  /// </summary>
  public class Statistics
  {
    #region Enumerations

    /// <summary>
    /// Represents the reason why the planning failed. A value of "None" means
    /// the planning did not fail.
    /// </summary>
    public enum FailureCause
    {
      /// <summary>
      /// The search has not failed yet.
      /// </summary>
      None,
      /// <summary>
      /// The search was stopped by an external event.
      /// </summary>
      Stopped,
      /// <summary>
      /// The search limit was reached and no plan was found yet.
      /// </summary>
      SearchLimitReached,
      /// <summary>
      /// The time limit was reached and no plan was found yet.
      /// </summary>
      TimeLimitReached,
      /// <summary>
      /// There is no world left to explore and no plan was found.
      /// </summary>
      NoWorldsLeftToExplore,
      /// <summary>
      /// The options specified are not compatible with the domain or problem.
      /// </summary>
      IncompatibleOptions,
    };

    #endregion

    #region Private Fields

    /// <summary>
    /// Whether the planning was aborted.
    /// </summary>
    private bool m_aborted;

    /// <summary>
    /// A watch that keeps track of parsing time.
    /// </summary>
    private Stopwatch m_parseWatch;

    /// <summary>
    /// A watch that keeps track of solving time.
    /// </summary>
    private Stopwatch m_solveWatch;
    /// <summary>
    /// A watch that keeps track of node extraction time.
    /// </summary>
    private Stopwatch m_extractNodeWatch;

    /// <summary>
    /// A watch that keeps track of operator creation time.
    /// </summary>
    private Stopwatch m_operatorsWatch;
    /// <summary>
    /// The total number of operators to create.
    /// </summary>
    private int m_operatorCount;
    /// <summary>
    /// The number of operators actually created (simplification may occur, depedinng on the options).
    /// </summary>
    private int m_filteredOperatorCount;

    /// <summary>
    /// The total number of generated successors.
    /// </summary>
    private int m_successorCount;
    /// <summary>
    /// The number of successor discarded because of violated constraints.
    /// </summary>
    private int m_successorsDiscardedConstraints;
    /// <summary>
    /// The number of successors discarded because of violated action conditions during application.
    /// </summary>
    private int m_successorsDiscardedApplication;
    /// <summary>
    /// The number of successors discarded because they caused a cycle.
    /// </summary>
    private int m_successorsDiscardedCycle;
    /// <summary>
    /// The number of successors which replaced a worse node in open.
    /// </summary>
    private int m_successorsBetterOpen;
    /// <summary>
    /// The number of successors which were added in open to replace a worse node in closed.
    /// </summary>
    private int m_successorsBetterClosed;
    /// <summary>
    /// The number of world for which successors were generated.
    /// </summary>
    private int m_successorsCallCount;

    /// <summary>
    /// A watch that keeps track of the time it takes to generate and insert successors in the search graph.
    /// </summary>
    private Stopwatch m_verifySuccessorWatch;

    /// <summary>
    /// A watch that keeps track of goal verification time.
    /// </summary>
    private Stopwatch m_goalWatch;
    /// <summary>
    /// The number of worlds which were verified to be goals.
    /// </summary>
    private int m_goalCallCount;

    /// <summary>
    /// The total number of examined node.
    /// </summary>
    private int m_nodeCount;

    /// <summary>
    /// The total number of nodes in open.
    /// </summary>
    private int m_openCount;
    /// <summary>
    /// The total number of nodes in closed.
    /// </summary>
    private int m_closedCount;

    /// <summary>
    /// The reason why the planning failed.
    /// </summary>
    private FailureCause m_failure;

    /// <summary>
    /// The options which were passed to the planner.
    /// </summary>
    private TLPlanOptions m_options;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the reason why the planning failed. A value of "None" means
    /// the planning did not fail.
    /// </summary>
    public FailureCause FailureReason
    {
      get { return m_failure; }
      set { m_failure = value; }
    }

    /// <summary>
    /// Gets or sets whether the planning was aborted.
    /// </summary>
    public bool Aborted
    {
      get { return m_aborted; }
      set { m_aborted = value; }
    }

    /// <summary>
    /// Verifies whether a plan was found.
    /// </summary>
    public bool PlanFound
    {
      get { return m_failure == FailureCause.None; }
    }

    /// <summary>
    /// Gets or sets the total number of operators to create.
    /// </summary>
    public int OperatorCount
    {
      get { return m_operatorCount; }
      set { m_operatorCount = value; }
    }

    /// <summary>
    /// Gets to sets the actual number of operators created (simplification can occur).
    /// </summary>
    public int FilteredOperatorCount
    {
      get { return m_filteredOperatorCount; }
      set { m_filteredOperatorCount = value; }
    }

    /// <summary>
    /// Gets or sets the number of nodes in open.
    /// </summary>
    public int OpenCount
    {
      get { return m_openCount; }
      set { m_openCount = value; }
    }

    /// <summary>
    /// Gets or sets the number of nodes in closed.
    /// </summary>
    public int ClosedCount
    {
      get { return m_closedCount; }
      set { m_closedCount = value; }
    }

    /// <summary>
    /// Returns the number of examined nodes.
    /// </summary>
    public int ExaminedNodeCount
    {
      get { return m_nodeCount; }
    }

    /// <summary>
    /// Returns the numbers of worlds for which successors were generated.
    /// </summary>
    public int SuccessorCount
    {
      get { return m_successorCount; }
    }

    /// <summary>
    /// Returns the number of successors which were discarded because of violated action conditions during application.
    /// </summary>
    public int SuccessorCountDiscardedAfterApplication
    {
      get { return m_successorsDiscardedApplication; }
    }


    /// <summary>
    /// Returns the time it took to parse the problem.
    /// </summary>
    public double ParseTime
    {
      get { return m_parseWatch.Elapsed.TotalSeconds; }
    }

    /// <summary>
    /// Returns the time it took to solve the problem.
    /// </summary>
    public double SolveTime
    {
      get { return m_solveWatch.Elapsed.TotalSeconds; }
    }

    /// <summary>
    /// Gets or sets the options used by the planner.
    /// </summary>
    public TLPlanOptions Options
    {
      get { return m_options; }
      set { m_options = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new, empty instance of the statistics.
    /// </summary>
    public Statistics()
    {
      Reset();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Resets all values and stopwatches to zero.
    /// </summary>
    public void Reset()
    {
      m_parseWatch = new Stopwatch();

      ResetSolve();
    }

    /// <summary>
    /// Resets all values to zero and all stopwatches except the one for parsing to zero.
    /// </summary>
    public void ResetSolve()
    {
      m_aborted = false;

      m_solveWatch = new Stopwatch();
      m_extractNodeWatch = new Stopwatch();

      m_operatorsWatch = new Stopwatch();
      m_operatorCount = 0;
      m_filteredOperatorCount = 0;

      m_successorCount = 0;
      m_successorsDiscardedConstraints = 0;
      m_successorsDiscardedApplication = 0;
      m_successorsDiscardedCycle = 0;
      m_successorsBetterOpen = 0;
      m_successorsBetterClosed = 0;
      m_successorsCallCount = 0;

      m_verifySuccessorWatch = new Stopwatch();

      m_goalWatch = new Stopwatch();
      m_goalCallCount = 0;

      m_nodeCount = 0;

      m_openCount = 0;
      m_closedCount = 0;
    }

    /// <summary>
    /// Start measuring parse time.
    /// </summary>
    public void StartParse() { m_parseWatch.Start(); }
    /// <summary>
    /// Stop measuring parse time.
    /// </summary>
    public void StopParse() { m_parseWatch.Stop(); }

    /// <summary>
    /// Start measuring solve time.
    /// </summary>
    public void StartSolve() { m_solveWatch.Start(); }
    /// <summary>
    /// Stop measuring solve time.
    /// </summary>
    public void StopSolve() { m_solveWatch.Stop(); }

    /// <summary>
    /// Start measuring node extraction time.
    /// </summary>
    public void StartExtractNode() { m_extractNodeWatch.Start(); }
    /// <summary>
    /// Stop measuring node extraction time.
    /// </summary>
    public void StopExtractNode() { m_extractNodeWatch.Stop(); }

    /// <summary>
    /// Start measuring operator creation time.
    /// </summary>
    public void StartOperators() { m_operatorsWatch.Start(); }
    /// <summary>
    /// Stop measuring operator creation time.
    /// </summary>
    public void StopOperators() { m_operatorsWatch.Stop(); }

    /// <summary>
    /// Count one more successor generated.
    /// </summary>
    public void SuccessorGenerated() { ++m_successorCount; }
    /// <summary>
    /// Count one more successor generated and discarded because of violated constraints.
    /// </summary>
    public void SuccessorDiscardedConstraints() { SuccessorGenerated(); ++m_successorsDiscardedConstraints; }
    /// <summary>
    /// Count one more successor generated and discarded because of violated action conditions in application.
    /// </summary>
    public void SuccessorDiscardedApplication() { SuccessorGenerated(); ++m_successorsDiscardedApplication; }
    /// <summary>
    /// Count one more successor discarded because it caused a cycle.
    /// </summary>
    public void SuccessorDiscardedCycle() { ++m_successorsDiscardedCycle; }
    /// <summary>
    /// Count one more successor replacing a worse one was in open.
    /// </summary>
    public void SuccessorBetterOpen() { ++m_successorsBetterOpen; }
    /// <summary>
    /// Count one more successor added to open to replace a worse one was in closed.
    /// </summary>
    public void SuccessorBetterClosed() { ++m_successorsBetterClosed; }

    /// <summary>
    /// Start measuring the time it takes to generate and insert successors in the search graph and count one more world for which successors were generated.
    /// </summary>
    public void StartVerifySuccessors() { m_verifySuccessorWatch.Start(); ++m_successorsCallCount; }
    /// <summary>
    /// Stop measuring the time it takes to generate and insert successors in the search graph.
    /// </summary>
    public void StopVerifySuccessors() { m_verifySuccessorWatch.Stop(); }

    /// <summary>
    /// Start measuring goal verification time.
    /// </summary>
    public void StartGoal() { m_goalWatch.Start(); }
    /// <summary>
    /// Stop measuring goal verification time and count one more world verified to be a goal.
    /// </summary>
    public void StopGoal() { m_goalWatch.Stop(); ++m_goalCallCount; }

    /// <summary>
    /// Count one more node examined.
    /// </summary>
    public void NodeExamined() { ++m_nodeCount; }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns a string representation of all the statistics compiled up to now.
    /// </summary>
    /// <returns>A string representation of all the statistics compiled up to now.</returns>
    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();

      if (Aborted)
        builder.AppendLine("Planning aborted; the following statistics may not be accurate.");
      else
        builder.AppendFormat("Planning completed. Plan {0}.\n", PlanFound ? "found" : "not found");

      double extractTime = m_extractNodeWatch.Elapsed.TotalSeconds - m_goalWatch.Elapsed.TotalSeconds;

      builder.Append((m_options != null) ? m_options.ToString() : String.Empty);
      builder.AppendLine();

      builder.AppendFormat("Parse time: {0:0.00000} sec.\n", m_parseWatch.Elapsed.TotalSeconds);
      builder.AppendFormat("Solve time: {0:0.00000} sec.\n", m_solveWatch.Elapsed.TotalSeconds);
      builder.AppendFormat("  Operator creation     : {0:0.00000} sec. ({1}/{2} operators created)\n", m_operatorsWatch.Elapsed.TotalSeconds, FilteredOperatorCount, OperatorCount);
      builder.AppendFormat("  Node extraction       : {0:0.00000} sec. (avg {1:0.00000} per node)\n", extractTime, extractTime / m_nodeCount);
      builder.AppendFormat("  Successor generation  : {0:0.00000} sec. ({1} calls, avg {2:0.00000} sec.)\n", m_verifySuccessorWatch.Elapsed.TotalSeconds, m_successorsCallCount, m_verifySuccessorWatch.Elapsed.TotalSeconds / m_successorsCallCount);
      builder.AppendFormat("  Goal verification     : {0:0.00000} sec. ({1} calls, avg {2:0.00000} sec.)\n\n", m_goalWatch.Elapsed.TotalSeconds, m_goalCallCount, m_goalWatch.Elapsed.TotalSeconds / m_goalCallCount);

      int successorsKept = m_successorCount - (m_successorsDiscardedApplication + m_successorsDiscardedConstraints + m_successorsDiscardedCycle);
      int successorsNewInOpen = successorsKept - (m_successorsBetterOpen + m_successorsBetterClosed);
      int[] numbers = new int[] { m_nodeCount, m_successorCount, m_successorsDiscardedConstraints, m_successorsDiscardedApplication, OpenCount, ClosedCount };
      int length = numbers.Max().ToString().Length;

      builder.AppendFormat("Total nodes examined           : {0," + length + "}\n", m_nodeCount);
      builder.AppendFormat("Total nodes generated          : {0," + length + "} (avg {1:0.00000})\n", m_successorCount, ((double)m_successorCount) / m_successorsCallCount);
      builder.AppendFormat("  Kept nodes                   : {0," + length + "} (avg {1:0.00000})\n", successorsKept, ((double)successorsKept) / m_successorsCallCount);
      builder.AppendFormat("    New in open                : {0," + length + "} (avg {1:0.00000})\n", successorsNewInOpen, ((double)successorsNewInOpen) / m_successorsCallCount);
      builder.AppendFormat("    Replacement from open      : {0," + length + "} (avg {1:0.00000})\n", m_successorsBetterOpen, ((double)m_successorsBetterOpen) / m_successorsCallCount);
      builder.AppendFormat("    Replacement from closed    : {0," + length + "} (avg {1:0.00000})\n", m_successorsBetterClosed, ((double)m_successorsBetterClosed) / m_successorsCallCount);
      builder.AppendFormat("  Discarded nodes (constraints): {0," + length + "} (avg {1:0.00000})\n", m_successorsDiscardedConstraints, ((double)m_successorsDiscardedConstraints) / m_successorsCallCount);
      builder.AppendFormat("  Discarded nodes (apply)      : {0," + length + "} (avg {1:0.00000})\n", m_successorsDiscardedApplication, ((double)m_successorsDiscardedApplication) / m_successorsCallCount);
      builder.AppendFormat("  Discarded nodes (cycle)      : {0," + length + "} (avg {1:0.00000})\n", m_successorsDiscardedCycle, ((double)m_successorsDiscardedCycle) / m_successorsCallCount);
      builder.AppendFormat("Nodes in open set              : {0," + length + "}\n", OpenCount);
      builder.AppendFormat("Nodes in closed set            : {0," + length + "}\n", ClosedCount);

      return builder.ToString();
    }

    #endregion
  }
}
