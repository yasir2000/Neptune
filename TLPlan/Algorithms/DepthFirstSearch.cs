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

using TLPlan.Utils;

namespace TLPlan.Algorithms
{
  /// <summary>
  /// Depth-first search explores a graph as far as possible along each branch 
  /// before backtracking.
  /// Hence it uses open set as a LIFO list (successors are always explored first).
  /// </summary>
  public class DepthFirstSearch : UnInformedGraphSearch
  {
    /// <summary>
    /// This list contains the successors of a node, in the order which they were generated.
    /// It is transferred to open set when the successors generation finishes.
    /// </summary>
    private LinkedList<Node> m_currentSuccessors;

    /// <summary>
    /// Creates a new depth-first search algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public DepthFirstSearch(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
      : base(options, statistics, traceWriter)
    {
      this.m_currentSuccessors = new LinkedList<Node>();
    }

    /// <summary>
    /// Transfer all successors to open set, if needed.
    /// </summary>
    private void TransferSuccessorsToOpenSet()
    {
      if (this.m_currentSuccessors.Count != 0)
      {
        //
        // Thanks to the .Net Framework, concatenating two linked lists is a O(n) operation.
        // But each linked list node has a pointer to its parent list! How lovely!
        //
        for (LinkedListNode<Node> current = this.m_currentSuccessors.Last; 
             current != null; 
             current = current.Previous)
        {
          m_linkedOpen.AddFirst(current.Value);
        }
        this.m_currentSuccessors.Clear();
      }
    }

    /// <summary>
    /// Adds the given successor to the open set.
    /// The successors are not added directly at the front of the open list since they are 
    /// generated according to their priority; we want the most promising nodes to be 
    /// explored first. 
    /// </summary>
    /// <param name="successor">The successors to add to open set.</param>
    protected override void AddSuccessorToOpenSet(Node successor)
    {
      m_currentSuccessors.AddLast(successor);
    }

    /// <summary>
    /// Returns the next node in open set.
    /// </summary>
    /// <returns>The next node in open set.</returns>
    protected override Node GetNextNode()
    {
      TransferSuccessorsToOpenSet();

      return base.GetNextNode();
    }

    /// <summary>
    /// Returns whether there is at least one node left in open set.
    /// </summary>
    /// <returns>Whether there is at least one node left in open set.</returns>
    protected override bool HasNextNode()
    {
      TransferSuccessorsToOpenSet();

      return base.HasNextNode();
    }

    /// <summary>
    /// Returns the number of nodes in the open set, i.e. nodes which should 
    /// eventually be explored.
    /// This function is used for computing statistics.
    /// </summary>
    /// <returns>The number of nodes in the open set.</returns>
    protected override int GetOpenCount()
    {
      TransferSuccessorsToOpenSet();

      return base.GetOpenCount();
    }

    /// <summary>
    /// Resets the search.
    /// All data structures should be reinitialized.
    /// </summary>
    protected override void ResetAlgorithm()
    {
      base.ResetAlgorithm();
      m_currentSuccessors.Clear();
    }
  }
}
