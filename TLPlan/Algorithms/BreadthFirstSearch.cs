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
  /// Breadth-first search explores neighboring nodes incrementally, i.e. all 1-neighbors, 
  /// then all 2-neighbors, ... and so on until the goal is reached.
  /// Hence it uses open set as a FIFO list (successors are always explored last).
  /// </summary>
  public class BreadthFirstSearch : UnInformedGraphSearch
  {
    /// <summary>
    /// Creates a new breadth-first search algorithm with the specified options.
    /// </summary>
    /// <param name="options">Search options.</param>
    /// <param name="statistics">Statistics object to use.</param>
    /// <param name="traceWriter">The stream to which traces are to be written.</param>
    public BreadthFirstSearch(TLPlanOptions options, Statistics statistics, TraceWriter traceWriter)
      : base(options, statistics, traceWriter)
    {
    }

    /// <summary>
    /// Adds the given successor to the open set.
    /// </summary>
    /// <param name="successor">The successor to add to open set.</param>
    protected override void AddSuccessorToOpenSet(Node successor)
    {
      m_linkedOpen.AddLast(successor);
    }
  }
}
