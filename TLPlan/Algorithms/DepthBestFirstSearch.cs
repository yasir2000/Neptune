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
using TLPlan.Utils.MultiDictionary;
using TLPlan.Utils.Set;

namespace TLPlan.Algorithms
{
  using NodeContainer = Container<LinkedListNode<Node>>;

  /// <summary>
  /// Depth-best-first search is a depth-first heuristic search.
  /// The successors of each node are explored in a best-first manner, but the graph itself
  /// is explored depth-first.
  /// </summary>
  public class DepthBestFirstSearch : GraphSearch
  {
    /// <summary>
    /// The open set contains nodes yet to be explored.
    /// </summary>
    private LinkedList<Node> m_linkedOpen;
    /// <summary>
    /// This structure is used only if the CycleChecking option is on.
    /// It maps nodes to their location in <see cref="m_linkedOpen"/>, hence
    /// allowing efficient detection of cycles.
    /// </summary>
    private IDictionary<Node, NodeContainer> m_open;
    /// <summary>
    /// This structure is used only if the CycleChecking option is on.
    /// The close set contains nodes which have already been explored.
    /// </summary>
    private TLPlan.Utils.Set.ISet<Node> m_closed;
    /// <summary>
    /// When not null, this container holds the last node found in open which is
    /// equal to the current node but has a worse g-cost.
    /// </summary>
    private NodeContainer m_worseOpenNodeContainer;

    /// <summary>
    /// When not null, this container holds the last node found in closed which is
    /// equal to the current node but has a worse g-cost.
    /// </summary>
    private Node m_worseClosedNode;

    /// <summary>
    /// Creates a new depth-best-first search algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public DepthBestFirstSearch(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
      : base(options, statistics, traceWriter)
    {
    }

    /// <summary>
    /// Initializes the search with the initial node.
    /// </summary>
    /// <param name="initialNode">The initial node.</param>
    protected override void Initialize(Node initialNode)
    {
      CalculateGCost(initialNode);
      CalculateHCost(initialNode);

      m_linkedOpen = new LinkedList<Node>();
      LinkedListNode<Node> listNode = m_linkedOpen.AddFirst(initialNode);
      if (m_options.CycleChecking)
      {
        m_open = m_options.CreateDictionary<Node, NodeContainer>();
        m_open.Add(initialNode, new NodeContainer(listNode));

        m_closed = m_options.CreateSet<Node>();
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

      if (m_options.CycleChecking)
      {
        bool removed = m_open.Remove(node);
        System.Diagnostics.Debug.Assert(removed);
      }

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
        // Add to closedset
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
      CalculateGCost(node);

      m_worseOpenNodeContainer = null;
      m_worseClosedNode = null;

      if (m_closed.TryGetValue(node, out m_worseClosedNode))
      {
        return node.GCost >= m_worseClosedNode.GCost;
      }
      else if (m_open.TryGetValue(node, out m_worseOpenNodeContainer))
      {
        Node worseOpenNode = m_worseOpenNodeContainer.Value.Value;
        return node.GCost >= worseOpenNode.GCost;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Signals a new node to be explored eventually.
    /// This function returns whether to continue adding successors.
    /// </summary>
    /// <param name="successor">The new node to be explored.</param>
    /// <returns>Whether to continue adding successors, which is always true.</returns>
    protected override bool AddSuccessor(Node successor)
    {
      // Always add the new successor to the front of the open list, since
      // the best successors are always added after the worse ones (hence they are
      // explored first).

      if (m_worseClosedNode != null)
      {
        // Node is in closed set but a better path was found; put it in open set.
        bool removedFromClosed = m_closed.Remove(successor);
        System.Diagnostics.Debug.Assert(removedFromClosed);

        LinkedListNode<Node> listNode = m_linkedOpen.AddFirst(successor);
        m_open.Add(successor, new NodeContainer(listNode));
        m_statistics.SuccessorBetterClosed();
      }
      else if (m_worseOpenNodeContainer != null)
      {
        // Node is in open set and a better path was found; update open set.
        m_linkedOpen.Remove(m_worseOpenNodeContainer.Value);

        LinkedListNode<Node> listNode = m_linkedOpen.AddFirst(successor);
        m_worseOpenNodeContainer.Value = listNode;
        m_statistics.SuccessorBetterOpen();
      }
      else
      {
        // Node is neither in closed set nor open set; add node to open set.
        CalculateHCost(successor);
        LinkedListNode<Node> listNode = m_linkedOpen.AddFirst(successor);
        if (m_options.CycleChecking)
        {
          m_open.Add(successor, new NodeContainer(listNode));
        }
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
      return (m_options.CycleChecking) ? m_closed.Count : 0;
    }

    /// <summary>
    /// Resets the search.
    /// All data structures should be reinitialized.
    /// </summary>
    protected override void ResetAlgorithm()
    {
      m_linkedOpen = null;
      m_open = null;
      m_closed = null;
    }

    /// <summary>
    /// Returns the valid successors of a given node.
    /// Instead of deferring the evaluation, all successors are immediately generated are
    /// sorted by their costs (descending).
    /// </summary>
    /// <param name="operators">The operators to use.</param>
    /// <param name="node">The current node.</param>
    /// <returns>The successors of the given node.</returns>
    protected override IEnumerable<Node> GetSuccessors(IEnumerable<IOperator> operators, Node node)
    {
      // The worst successors are returned first, so that the most promising successors are
      // actually added to the front of the open list.

      List<Node> successors = new List<Node>();
      foreach (Node successor in base.GetSuccessors(operators, node))
      {
        CalculateGCost(successor);
        CalculateHCost(successor);

        successors.Add(successor);
      }

      successors.Sort((n1, n2) => -n1.FCost.CompareTo(n2.FCost));

      return successors;
    }
  }
}
