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
  /// Best-first search explores the most promising worlds first, i.e. the nodes with the
  /// lowest f-cost.
  /// </summary>
  public class BestFirstSearch : GraphSearch
  {
    /// <summary>
    /// The open set contains nodes yet to be explored.
    /// This structure is sorted by nodes' f-cost to allow efficient retrieval of the
    /// next node.
    /// </summary>
    private SortedMultiDictionary<double, Node> m_openSorted;
    /// <summary>
    /// This structure is used only if the CycleChecking option is on.
    /// It maps nodes to their location in <see cref="m_openSorted"/>, hence
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
    /// Creates a new best-first search algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public BestFirstSearch(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
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

      m_openSorted = new SortedMultiDictionary<double, Node>();

      LinkedListNode<Node> listNode = m_openSorted.Add(initialNode.FCost, initialNode);
      if (m_options.CycleChecking)
      {
        m_open = m_options.CreateDictionary<Node, NodeContainer>();
        m_open.Add(initialNode, new NodeContainer(listNode));

        m_closed = m_options.CreateSet<Node>();
      }
    }

    /// <summary>
    /// Returns the next node to explore.
    /// Best-first search returns the open node with the lowest f-cost.
    /// </summary>
    /// <returns>The next node to explore.</returns>
    protected override Node GetNextNode()
    {
      Node node = m_openSorted.RemoveFirst().Value;

      if (m_options.CycleChecking)
      {
        bool removed = m_open.Remove(node);
        System.Diagnostics.Debug.Assert(removed);
      }

      return node;
    }

    /// <summary>
    /// Returns whether the given node is a goal node.
    /// When a goal node satisfying the goal formulation is first discovered, its metric 
    /// evaluation is updated (if necessary) to reflect the violated goal preferences
    /// and the node is sent back to open set.
    /// The first explored idle goal node represents the best solution (assuming the
    /// metric function is monotone).
    /// </summary>
    /// <param name="node">The node to test.</param>
    /// <returns>Whether the given node is a goal node.</returns>
    protected override bool IsGoal(Node node)
    {
      if (node.World.IsIdleGoalWorld())
      {
        // Return true only if the goal preferences have been evaluated.
        return true;
      }
      else if (base.IsGoal(node))
      {
        // Create the goal node and insert it in open (if it isn't there already)
        Node goalNode = CreateIdleGoalNode(node);

        if (m_options.CycleChecking)
        {
          NodeContainer existingNodeContainer;
          if (m_open.TryGetValue(goalNode, out existingNodeContainer))
          {
            Node existingNode = existingNodeContainer.Value.Value;
            if (goalNode.GCost < existingNode.GCost)
            {
              // Discard the existing node with the worse g-cost
              m_openSorted.Remove(existingNodeContainer.Value);

              LinkedListNode<Node> listNode = m_openSorted.Add(goalNode.FCost, goalNode);
              existingNodeContainer.Value = listNode;
            }
          }
          else
          {
            // Add goalNode to open
            LinkedListNode<Node> listNode = m_openSorted.Add(goalNode.FCost, goalNode);
            m_open.Add(goalNode, new NodeContainer(listNode));
          }
        }
        else
        {
          m_openSorted.Add(goalNode.FCost, goalNode);
        }
        return false;
      }
      else
      {
        return false;
      }
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
    /// If CycleChecking is off, the node is always added to open set.
    /// Else the node is added if:
    /// - no better (g-cost-wise) identical nodes were found in open set; and
    /// - no better (g-cost-wise) identical nodes were found in closed set.
    /// </summary>
    /// <param name="successor">The new node to be explored.</param>
    /// <returns>Whether to continue adding successors, which is always true.</returns>
    protected override bool AddSuccessor(Node successor)
    {
      if (m_worseClosedNode != null)
      {
        // Node is in closed set but a better path was found; put it in open set.
        bool removedFromClosed = m_closed.Remove(m_worseClosedNode);
        System.Diagnostics.Debug.Assert(removedFromClosed);

        LinkedListNode<Node> listNode = m_openSorted.Add(successor.FCost, successor);
        m_open.Add(successor, new NodeContainer(listNode));
        m_statistics.SuccessorBetterClosed();
      }
      else if (m_worseOpenNodeContainer != null)
      {
        // Node is in open set and a better path was found; update open set.
        m_openSorted.Remove(m_worseOpenNodeContainer.Value);

        LinkedListNode<Node> listNode = m_openSorted.Add(successor.FCost, successor);
        m_worseOpenNodeContainer.Value = listNode;
        m_statistics.SuccessorBetterOpen();
      }
      else
      {
        // Node is neither in closed set nor open set; add node to open set.
        CalculateHCost(successor);
        LinkedListNode<Node> listNode = m_openSorted.Add(successor.FCost, successor);
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
      return !m_openSorted.IsEmpty();
    }

    /// <summary>
    /// Returns the number of nodes in the open set, i.e. nodes which should 
    /// eventually be explored.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the open set.</returns>
    protected override int GetOpenCount()
    {
      return m_openSorted.Count;
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
      m_openSorted = null;
      m_open = null;
      m_closed = null;
    }
  }
}
