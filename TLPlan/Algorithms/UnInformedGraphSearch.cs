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
  /// Unweighted graph search is the base class for search algorithms which do not use 
  /// domain information (i.e. a cost function) to operate.
  /// </summary>
  public abstract class UnInformedGraphSearch : GraphSearch
  {
    /// <summary>
    /// The open set contains nodes yet to be explored.
    /// </summary>
    protected LinkedList<Node> m_linkedOpen;
    /// <summary>
    /// This structure is used only if the CycleChecking option is on.
    /// It contains all nodes generated up to this point and allows for efficient detection
    /// of cycles.
    /// </summary>
    private TLPlan.Utils.Set.ISet<Node> m_allNodes;

    /// <summary>
    /// Creates a new unweighted search algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public UnInformedGraphSearch(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
      : base(options, statistics, traceWriter)
    {
    }

    /// <summary>
    /// Adds the given successor to the open set.
    /// </summary>
    /// <param name="successor">The successors to add to open set.</param>
    protected abstract void AddSuccessorToOpenSet(Node successor);

    /// <summary>
    /// Initializes the search with the initial node.
    /// </summary>
    /// <param name="initialNode">The initial node.</param>
    protected override void Initialize(Node initialNode)
    {
      m_linkedOpen = new LinkedList<Node>();
      m_linkedOpen.AddFirst(initialNode);
      if (m_options.CycleChecking)
      {
        m_allNodes = m_options.CreateSet<Node>();
        m_allNodes.Add(initialNode);
      }
    }

    /// <summary>
    /// Returns the next node in open set.
    /// </summary>
    /// <returns>The next node in open set.</returns>
    protected override Node GetNextNode()
    {
      Node node = m_linkedOpen.First.Value;
      m_linkedOpen.RemoveFirst();

      return node;
    }

    /// <summary>
    /// Closes the given node.
    /// </summary>
    /// <param name="node">The node to close.</param>
    protected override void CloseNode(Node node)
    {
      if (m_options.CycleChecking)
      {
        m_allNodes.Add(node);
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
      return m_allNodes.Contains(node);
    }

    /// <summary>
    /// Signals a new node to be explored eventually.
    /// This function returns whether to continue adding successors, which is always true.
    /// </summary>
    /// <param name="successor">The new node to be explored.</param>
    /// <returns>Whether to continue adding successors, which is always true.</returns>
    protected override bool AddSuccessor(Node successor)
    {
      AddSuccessorToOpenSet(successor);
      if (m_options.CycleChecking)
      {
        m_allNodes.Add(successor);
      }
      return true;
    }

    /// <summary>
    /// Returns whether there is at least one node left in open set.
    /// </summary>
    /// <returns>Whether there is at least one node left in open set.</returns>
    protected override bool HasNextNode()
    {
      return m_linkedOpen.Count != 0;
    }

    /// <summary>
    /// Returns the number of nodes in the open set, i.e. nodes which should 
    /// eventually be explored.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the open set.</returns>
    protected override int GetOpenCount()
    {
      return m_linkedOpen.Count;
    }

    /// <summary>
    /// Returns the number of nodes in the closed set, i.e. nodes which have already
    /// been explored and whose successors have been generated.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the closed set.</returns>
    protected override int GetClosedCount()
    {
      return (m_options.CycleChecking) ? m_allNodes.Count : 0;
    }

    /// <summary>
    /// Resets the search.
    /// All data structures should be reinitialized.
    /// </summary>
    protected override void ResetAlgorithm()
    {
      m_linkedOpen = null;
      m_allNodes = null;
    }
  }
}
