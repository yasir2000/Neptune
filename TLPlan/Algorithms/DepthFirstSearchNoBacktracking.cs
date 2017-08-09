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
using PDDLParser;
using TLPlan.Utils;
using TLPlan.Utils.Set;

namespace TLPlan.Algorithms
{
  /// <summary>
  /// Depth-first search without backtracking behaves exactly like regular depth-first search 
  /// with backtracking option set to off. It explores a graph along one single branch as far as
  /// possible until either a goal is found or there is no successor.
  /// </summary>
  public class DepthFirstSearchNoBacktracking : GraphSearch
  {
    /// <summary>
    /// The next node to explore.
    /// </summary>
    private Node m_nextNode;
    /// <summary>
    /// This structure is used only if the CycleChecking option is on.
    /// The closed set contains nodes which have already been explored.
    /// </summary>
    private TLPlan.Utils.Set.ISet<Node> m_closed;

    /// <summary>
    /// Creates a new depth-first search no-backtracking algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public DepthFirstSearchNoBacktracking(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
      : base(options, statistics, traceWriter)
    {
    }

    /// <summary>
    /// Initializes the search with the initial node.
    /// </summary>
    /// <param name="initialNode">The initial node.</param>
    protected override void Initialize(Node initialNode)
    {
      m_nextNode = initialNode;
      if (m_options.CycleChecking)
      {
        m_closed = m_options.CreateSet<Node>();
      }
    }

    /// <summary>
    /// Returns the next node to explore.
    /// </summary>
    /// <returns>The next node to explore.</returns>
    protected override Node GetNextNode()
    {
      return m_nextNode;
    }

    /// <summary>
    /// Closes the given node.
    /// </summary>
    /// <param name="node">The node to close.</param>
    protected override void CloseNode(Node node)
    {
      m_nextNode = null;
      if (m_options.CycleChecking)
      {
        m_closed.Add(node);
      }
    }

    /// <summary>
    /// This function returns whether the given node was already examined and thus
    /// represents a cycle in the search graph.
    /// </summary>
    /// <param name="node">The node to check for cycles.</param>
    /// <returns>Whether the given node has already been examined.</returns>
    protected override bool CausesCycle(Node node)
    {
      return m_closed.Contains(node);
    }

    /// <summary>
    /// Signals a new node to be explored eventually.
    /// This function returns whether to continue adding successors.
    /// </summary>
    /// <param name="successor">The new node to be explored.</param>
    /// <returns>Whether to continue adding successors, which is true only if
    /// no valid successor has been found yet.</returns>
    protected override bool AddSuccessor(Node successor)
    {
      m_nextNode = successor;
      return false;
    }

    /// <summary>
    /// Returns whether there is at least one node left to explore.
    /// </summary>
    /// <returns>Whether there is at least one node left to explore.</returns>
    protected override bool HasNextNode()
    {
      return m_nextNode != null;
    }

    /// <summary>
    /// Returns the number of nodes in the open set, i.e. nodes which should 
    /// eventually be explored.
    /// This function is used for computing statistics.
    /// Since backtracking is not used, there is always exactly zero or one node
    /// in open set.
    /// </summary>
    /// <returns>The number of nodes in the open set.</returns>
    protected override int GetOpenCount()
    {
      return (m_nextNode != null) ? 1 : 0;
    }

    /// <summary>
    /// Returns the number of nodes in the closed set, i.e. nodes which have already
    /// been explored and whose successors have been generated.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the closed set.</returns>
    protected override int GetClosedCount()
    {
      return (m_options.CycleChecking) ? m_closed.Count : 0;
    }

    /// <summary>
    /// Resets the search.
    /// All data structures should be reinitialized.
    /// </summary>
    protected override void ResetAlgorithm()
    {
      m_nextNode = null;
      m_closed = null;
    }
  }
}
