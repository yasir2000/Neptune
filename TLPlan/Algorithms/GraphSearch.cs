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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PDDLParser;
using PDDLParser.Exp.Metric;
using PDDLParser.Exp;
using TLPlan.World;
using TLPlan.Utils;

namespace TLPlan.Algorithms
{
  /// <summary>
  /// Base class for graph search algorithms.
  /// The Solve() method is implemented as a template method design pattern.
  /// </summary>
  public abstract class GraphSearch
  {
    #region Private Members

    /// <summary>
    /// TLPlan options.
    /// </summary>
    protected TLPlanOptions m_options;
    /// <summary>
    /// Search statistics.
    /// </summary>
    protected Statistics m_statistics;
    /// <summary>
    /// The problem's goal formulation.
    /// </summary>
    protected ILogicalExp m_goal;
    /// <summary>
    /// The problem's metric.
    /// </summary>
    protected MetricExp m_metric;
     /// <summary>
    /// Whether the search is currently paused.
    /// </summary>
    private bool m_paused;
    /// <summary>
    /// Whether the search should be paused when the goal world is found.
    /// </summary>
    private bool m_pauseOnGoal;
    /// <summary>
    /// Whether the search is stopped.
    /// </summary>
    private bool m_stopped;
    /// <summary>
    /// The current node (necessary for the GUI).
    /// </summary>
    private Node m_currentNode;
    /// <summary>
    /// The stream to which trace information is output.
    /// </summary>
    private TraceWriter m_traceStream;

    #endregion

    #region Events

    /// <summary>
    /// The pause event is used to pause/resume the search from the GUI.
    /// </summary>
    private AutoResetEvent m_pauseEvent;

    /// <summary>
    /// The Started event is fired when the search starts.
    /// </summary>
    public event EventHandler Started;
    /// <summary>
    /// The Paused event is fired when the search is paused.
    /// </summary>
    public event EventHandler Paused;
    /// <summary>
    /// The Unpaused event is fired when the search is resumed.
    /// </summary>
    public event EventHandler Unpaused;
    /// <summary>
    /// The Stopped event is fired when the search is stopped.
    /// </summary>
    public event EventHandler Stopped;
    /// <summary>
    /// The PlanningFinished event is fired when the planning is finished.
    /// </summary>
    public event EventHandler<Planner.PlanningFinishedEventArgs> PlanningFinished;

    #endregion

    #region Properties

    /// <summary>
    /// Whether the search is currently paused.
    /// </summary>
    public bool IsPaused { get { return m_paused; } }
    /// <summary>
    /// Whether the search is currently stopped.
    /// </summary>
    public bool IsStopped { get { return m_stopped; } }
    /// <summary>
    /// The current node.
    /// </summary>
    public Node CurrentNode { get { return m_currentNode; } }
    /// <summary>
    /// Whether the search should be paused when the goal world is found.
    /// </summary>
    public bool PauseOnGoalWorld
    {
      get { return m_pauseOnGoal; }
      set { m_pauseOnGoal = value; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new graph search algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public GraphSearch(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
    {
      m_options = options;
      m_statistics = statistics;
      m_traceStream = traceWriter;

      m_paused = false;
      m_pauseEvent = new AutoResetEvent(false);
      m_statistics.FailureReason = Statistics.FailureCause.None;
    }

    #endregion

    #region Fire Events

    /// <summary>
    /// Fires the Started event.
    /// </summary>
    private void FireStarted()
    {
      if (Started != null)
        Started(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fires the Paused event.
    /// </summary>
    private void FirePaused()
    {
      if (Paused != null)
        Paused(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fires the Unpaused event.
    /// </summary>
    private void FireUnpaused()
    {
      if (Unpaused != null)
        Unpaused(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fires the Stopped event.
    /// </summary>
    private void FireStopped()
    {
      if (Stopped != null)
        Stopped(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fires the Solved event.
    /// </summary>
    /// <param name="plan">The plan which solves the problem.</param>
    private void FireSolved(Plan plan)
    {
      if (PlanningFinished != null)
        PlanningFinished(this, new Planner.PlanningFinishedEventArgs(plan));
    }

    /// <summary>
    /// Fires the PlanningFinished event with arguments indicating the problem was not solved.
    /// </summary>
    private void FireNotSolved()
    {
      if (PlanningFinished != null)
        PlanningFinished(this, new Planner.PlanningFinishedEventArgs());
    }

    #endregion

    #region Utility Functions

    /// <summary>
    /// Pauses the search.
    /// </summary>
    /// <returns>True if search was not already paused.</returns>
    public bool Pause()
    {
      if (!IsPaused)
      {
        m_pauseEvent.Reset();
        m_paused = true;

        return true;
      }

      return false;
    }

    /// <summary>
    /// Resumes the search.
    /// </summary>
    /// <returns>True if the search was paused.</returns>
    public bool Unpause()
    {
      if (IsPaused)
      {
        m_paused = false;
        m_pauseEvent.Set();

        return true;
      }

      return false;
    }

    /// <summary>
    /// Stops the search.
    /// </summary>
    public void Stop()
    {
      m_stopped = true;
      Unpause();
    }

    /// <summary>
    /// Resets the search.
    /// </summary>
    public void Reset()
    {
      m_statistics.Reset();
      m_paused = false;
      m_stopped = false;
      m_goal = null;
      m_metric = null;
      m_pauseEvent.Reset();
      ResetAlgorithm();
    }

    #endregion

    #region Virtual Functions

    /// <summary>
    /// Initializes the search with the initial node.
    /// </summary>
    /// <param name="initialNode">The initial node.</param>
    protected abstract void Initialize(Node initialNode);

    /// <summary>
    /// Returns the next node to explore.
    /// </summary>
    /// <returns>The next node to explore.</returns>
    protected abstract Node GetNextNode();

    /// <summary>
    /// Closes the given node.
    /// </summary>
    /// <param name="node">The node to close.</param>
    protected abstract void CloseNode(Node node);

    /// <summary>
    /// This function returns whether the given node was already examined and thus
    /// represents a cycle in the search graph.
    /// </summary>
    /// <param name="node">The node to check for cycles.</param>
    /// <returns>Whether the given node has already been examined.</returns>
    protected abstract bool CausesCycle(Node node);

    /// <summary>
    /// Signals a new node to be explored eventually.
    /// This function returns whether to continue adding successors.
    /// </summary>
    /// <param name="successor">The new node to be explored.</param>
    /// <returns>Whether to continue adding successors.</returns>
    protected abstract bool AddSuccessor(Node successor);

    /// <summary>
    /// Returns whether there is at least one node left to explore.
    /// </summary>
    /// <returns>Whether there is at least one node left to explore.</returns>
    protected abstract bool HasNextNode();

    /// <summary>
    /// Returns the number of nodes in the open set, i.e. nodes which should 
    /// eventually be explored.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the open set.</returns>
    protected abstract int GetOpenCount();

    /// <summary>
    /// Returns the number of nodes in the closed set, i.e. nodes which have already
    /// been explored and whose successors have been generated.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the closed set.</returns>
    protected abstract int GetClosedCount();

    /// <summary>
    /// Resets the search.
    /// All data structures should be reinitialized.
    /// </summary>
    protected abstract void ResetAlgorithm();

    /// <summary>
    /// Returns whether the given node is a goal node.
    /// If this function returns true, the search completes successfully by computing
    /// the plan associated with the node's predecessors.
    /// The default implementation returns true if all events have been processed, if 
    /// the node  satisfies the goal formulation (obviously), and if the world idles 
    /// correctly with respect to the trajectory constraints.
    /// </summary>
    /// <param name="node">The node to test.</param>
    /// <returns>Whether the given node is a goal node.</returns>
    protected virtual bool IsGoal(Node node)
    {
      m_statistics.StartGoal();

      bool result = (node.World.EventQueue.Count == 0); // Have all events been processed?
      if (result)
      {
        result = ExpController.Evaluate(m_goal, node.World);

        // Goal expression and constraints must be satisfied
        if (result)
        {
          result = ExpController.EvaluateIdle(node.World.NextConstraints, node.World);
        }
      }

      m_statistics.StopGoal();

      return result;
    }

    /// <summary>
    /// Returns the valid successors of a given node.
    /// Note that successors are computed only when needed (i.e. when iterating over
    /// the enumerable), so very few processing has to be done to retrieve the enumerable
    /// itself.
    /// </summary>
    /// <param name="operators">The operators to use.</param>
    /// <param name="node">The current node.</param>
    /// <returns>The successors of the given node.</returns>
    protected virtual IEnumerable<Node> GetSuccessors(IEnumerable<IOperator> operators, Node node)
    {
      if (!node.IsGoal)
      {
        // Constraints to be satisfied in successor worlds
        IConstraintExp successorConstraints = node.World.NextConstraints;

        foreach (IOperator op in operators)
        {
          // TODO: Review the interface? Since IsApplicable may return true even if Apply returns null (e.g. when violating overall conditions)...
          if (op.IsApplicable(node.World))
          {
            TLPlanDurativeClosedWorld newWorld = op.Apply(node.World);

            if (newWorld != null)
            {
              newWorld.CurrentConstraints = successorConstraints;

              // Progress constraint preferences.
              newWorld.ProgressPreferences(node.World.ConstraintPreferences);

              Node newNode = new Node(newWorld, node, op);
              m_statistics.SuccessorGenerated();

              yield return newNode;
            }
            else
            {
              m_statistics.SuccessorDiscardedApplication();
            }
          }
        }
      }
    }

    #endregion

    /// <summary>
    /// Creates a new node (and world) in which the violated preferences have an impact
    /// over the node's metric evaluation.
    /// </summary>
    /// <param name="goalNode">The goal node to idle.</param>
    /// <returns>A new idle goal node with the updated metric evaluation.</returns>
    protected Node CreateIdleGoalNode(Node goalNode)
    {
      System.Diagnostics.Debug.Assert(!goalNode.World.IsIdleGoalWorld());

      // Create the goal node with an updated world (for preferences).
      TLPlanDurativeClosedWorld newGoalWorld = goalNode.World.Copy();

      // Keep the same constraints as to not modify the hashcode.
      newGoalWorld.CurrentConstraints = goalNode.World.CurrentConstraints; 
      newGoalWorld.NextConstraints = goalNode.World.NextConstraints;

      // Evaluate the constraint preferences on the idle world.
      foreach (IConstraintPrefExp pref in goalNode.World.ConstraintPreferences)
      {
        if (!ExpController.EvaluateIdle(pref.Constraint, newGoalWorld))
        {
          // Preference violated; update the world to reflect this.
          ExpController.Update(pref.GetViolationEffect(), goalNode.World, newGoalWorld);
        }
      }

      newGoalWorld.IsIdleGoal = true; // This must be done before CalculateGCost()
      Node newGoalNode = new Node(newGoalWorld, goalNode.Predecessor, goalNode.Operator);

      CalculateGCost(newGoalNode);
      newGoalNode.HCost = 0;

      return newGoalNode;
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void LogTraceMessage(string message)
    {
      this.m_traceStream.WriteLine(message);
    }

    /// <summary>
    /// Logs the specified parts of the given world.
    /// </summary>
    /// <param name="node">The node containing the world to log.</param>
    /// <param name="worldParts">The world parts to log.</param>
    void LogWorld(Node node, WorldUtils.PrintWorldPart worldParts)
    {
      this.m_traceStream.PrintWorld(node, worldParts);
    }

    /// <summary>
    /// Attempts to solve a given problem.
    /// </summary>
    /// <param name="initialWorld">The initial world.</param>
    /// <param name="operators">The list of operators to use.</param>
    /// <param name="goal">The goal formulation.</param>
    /// <param name="metric">The metric evaluation function.</param>
    /// <returns>A plan that solves the problem, or null if no plan was found.</returns>
    public Plan Solve(TLPlanDurativeClosedWorld initialWorld, List<IOperator> operators, 
                      ILogicalExp goal, MetricExp metric)
    {
      m_goal = goal;
      m_metric = metric;

      FireStarted();

      int searchLimit = m_options.SearchLimit;
      m_statistics.FailureReason = Statistics.FailureCause.None;

      Node initialNode = new Node(initialWorld, null, null);
      Initialize(initialNode);

      try
      {
        bool failed;
        do
        {
          bool checkSuccessors;
          m_statistics.StartExtractNode();

          Node nextNode = GetNextNode();

          if (m_options.TraceLevel >= 1)
            this.LogTraceMessage(String.Format("\nExpanding world {0} (depth {1}, cost {2}, heuristic {3}) ...", 
                                 nextNode.World.WorldNumber,
                                 nextNode.Depth,
                                 nextNode.GCost,
                                 nextNode.HCost));

          m_statistics.NodeExamined();

          if (m_paused)
          {
            // Search has been paused.
            m_statistics.StopExtractNode();
            m_statistics.StopSolve();
            m_currentNode = nextNode;
            FirePaused();
            m_pauseEvent.WaitOne();
            FireUnpaused();
            m_currentNode = null;
            m_statistics.StartSolve();
            m_statistics.StartExtractNode();
          }

          // Progress constraints if it hasn't been done yet.
          if (nextNode.World.NextConstraints == null && !nextNode.World.ProgressCurrentConstraints())
          {
            if (m_options.TraceLevel >= 1)
              this.LogTraceMessage(String.Format("World {0} discarded because it violates the trajectory constraints.", 
                                                 nextNode.World.WorldNumber));
            if (m_options.TraceLevel >= 2)
              this.LogTraceMessage(String.Format("Violated constraints: {0}",
                                                 nextNode.World.CurrentConstraints));

            if (m_options.TraceLevel >= 3)
            {
              this.LogTraceMessage(String.Format("The world {0} is:",
                                   nextNode.World.WorldNumber));
              this.LogWorld(nextNode, WorldUtils.PrintWorldPart.AllFactsAndFluents);
            }

            // The node doesn't satisfy the trajectory constraints.
            m_statistics.SuccessorDiscardedConstraints();
            CloseNode(nextNode);
            checkSuccessors = false;
          }
          else if (IsGoal(nextNode))
          {
            // Evaluate the idle preferences if not done yet
            if (!nextNode.World.IsIdleGoalWorld())
            {
              nextNode = CreateIdleGoalNode(nextNode);
            }

            m_statistics.StopExtractNode();

            if (m_pauseOnGoal)
            {
              // Search is paused because this is the goal world.
              m_paused = true;
              m_statistics.StopSolve();
              m_currentNode = nextNode;
              FirePaused();
              m_pauseEvent.WaitOne();
              FireUnpaused();
              m_currentNode = null;
              m_paused = false;
            }
            
            Plan plan = Plan.ReconstructPlan(nextNode);
            FireSolved(plan);
            return plan;
          }
          else
          {
            CloseNode(nextNode);
            checkSuccessors = true;
          }

          m_statistics.StopExtractNode();

          if (checkSuccessors)
          {
            if (m_options.TraceLevel >= 2)
              this.LogTraceMessage(String.Format("Progressed constraints: {0}",
                                                 nextNode.World.NextConstraints));

            // Add successors to open set when applicable

            m_statistics.StartVerifySuccessors();
            IEnumerable<Node> successors = GetSuccessors(operators, nextNode);

            foreach (Node successor in successors)
            {
              if (m_options.CycleChecking && CausesCycle(successor))
              {
                if (m_options.TraceLevel >= 1)
                  this.LogTraceMessage(String.Format("Successor {0} \"{1}\" discarded because it causes a cycle.",
                                                     successor.World.WorldNumber,
                                                     successor.Operator.ToString()));

                m_statistics.SuccessorDiscardedCycle();
              }
              else
              {
                if (m_options.ImmediatelyPruneSuccessors && !successor.World.ProgressCurrentConstraints())
                {
                  m_statistics.SuccessorDiscardedConstraints();
                  CloseNode(successor);

                  if (m_options.TraceLevel >= 1)
                    this.LogTraceMessage(String.Format("Successor {0} \"{1}\" discarded because it violates the trajectory constraints.",
                                                       successor.World.WorldNumber,
                                                       successor.Operator.ToString()));

                  if (m_options.TraceLevel >= 3)
                  {
                    this.LogTraceMessage(String.Format("Successor {0} was:",
                                         successor.World.WorldNumber));
                    this.LogWorld(successor, WorldUtils.PrintWorldPart.AllFactsAndFluents);
                  }
                }
                else
                {
                  if (m_options.TraceLevel >= 1)
                    this.LogTraceMessage(String.Format("Successor {0} \"{1}\" successfully generated.",
                                                       successor.World.WorldNumber,
                                                       successor.Operator.ToString()));

                  bool keepAddingSuccessor = AddSuccessor(successor);
                  if (!keepAddingSuccessor || !m_options.UseBacktracking)
                    break;
                }
              }
            }
            m_statistics.StopVerifySuccessors();
          }

          failed = true;
          if (m_stopped)
          {
            // Solving has been stopped by someone else
            m_statistics.FailureReason = Statistics.FailureCause.Stopped;
            FireStopped();
          }
          else if (!HasNextNode())
          {
            // We have searched through all possible states
            m_statistics.FailureReason = Statistics.FailureCause.NoWorldsLeftToExplore;
          }
          else if (m_options.HasSearchLimit && --searchLimit <= 0)
          {
            // The search limit has been reached
            m_statistics.FailureReason = Statistics.FailureCause.SearchLimitReached;
          }
          else if (m_options.HasTimeLimit && m_statistics.SolveTime >= m_options.TimeLimit.TotalSeconds)
          {
            // The search limit has been reached
            m_statistics.FailureReason = Statistics.FailureCause.TimeLimitReached;
          }
          else
          {
            failed = false;
          }

        } while (!failed);

        // Failure
        FireNotSolved();

        return null;
      }
      finally
      {
        m_statistics.OpenCount = GetOpenCount();
        m_statistics.ClosedCount = GetClosedCount();
        m_statistics.StopExtractNode();

        m_paused = false;
        m_pauseEvent.Reset();

        m_stopped = false;
      }
    }

    /// <summary>
    /// Calculates the g-cost of a node, i.e. the cumulative cost from the initial node up
    /// to this point.
    /// The problem's metric is used to calculate the cost.
    /// </summary>
    /// <param name="node">The node whose g-cost must be set.</param>
    protected void CalculateGCost(Node node)
    {
      node.GCost = ExpController.EvaluateMetric(this.m_metric, node.World);
    }

    /// <summary>
    /// Calculates the h-cost of a node, i.e. an estimate of the cost to go until the 
    /// final node.
    /// </summary>
    /// <param name="node">The node whose h-cost must be set.</param>
    protected void CalculateHCost(Node node)
    {
      // TODO: compute heuristics!

      // No clue.
      node.HCost = 0;

      //double hCost = 0;
      //if (goal is AndExp)
      //{
      //  AndExp goalExp = (AndExp)goal;
      //  foreach (ILogicalExp exp in goalExp)
      //  {
      //    if (exp is AndExp)
      //      hCost += CalculateHCost(world, exp);
      //    else if (!ExpController.Evaluate(exp, world))
      //      ++hCost;
      //  }
      //}
      //return hCost;
    }
  }
}
