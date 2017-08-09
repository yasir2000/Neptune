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
using System.Linq;
using System.Text;

using PDDLParser;
using TLPlan.World;

namespace TLPlan
{
  /// <summary>
  /// A node encapsulates a world by adding planning information.
  /// </summary>
  public class Node : IComparable<Node>
  {
    /// <summary>
    /// The world inside this node.
    /// </summary>
    private TLPlanDurativeClosedWorld m_world;
    /// <summary>
    /// The operator which generated this world.
    /// </summary>
    private IOperator m_operator;
    /// <summary>
    /// The predecessor of this node, used to memorize the trajectory.
    /// </summary>
    private Node m_predecessor;
    /// <summary>
    /// The depth of the node in the search graph.
    /// </summary>
    private int m_depth;
    /// <summary>
    /// The g-cost is the cumulative cost from the starting point.
    /// It is evaluated with the problem's metric.
    /// </summary>
    private double m_gCost;
    /// <summary>
    /// The h-cost is an estimation of the cost-to-go to the final world.
    /// </summary>
    private double m_hCost;
    /// <summary>
    /// Whether this node is a goal node.
    /// Note that this variable does not imply m_world.IsIdleGoalNode()!
    /// </summary>
    private bool m_isGoal;
    /// <summary>
    /// The cached hash code of this world.
    /// </summary>
    private int m_worldHashCode;

    /// <summary>
    /// Creates a new node with the specified node, predecessor, and operator.
    /// </summary>
    /// <param name="world">The world to store in this node.</param>
    /// <param name="predecessor">The predecessor of this node.</param>
    /// <param name="op">The operator which generated this world.</param>
    public Node(TLPlanDurativeClosedWorld world, Node predecessor, IOperator op)
    {
      m_world       = world;
      m_operator    = op; // The operator that brought in this world
      m_predecessor = predecessor;
      m_depth       = (predecessor != null) ? predecessor.Depth + 1 : 0;
      m_gCost       = 0;
      m_hCost       = 0;
      m_isGoal      = false;
      // IMPORTANT: once stored in a node, a world is never modified!!!
      m_worldHashCode = world.GetHashCode();
    }

    /// <summary>
    /// The world inside this node.
    /// </summary>
    public TLPlanReadOnlyDurativeClosedWorld World
    {
      get { return m_world; }
    }

    /// <summary>
    /// The operator which generated this world.
    /// </summary>
    public IOperator Operator
    {
      get { return m_operator; }
      private set { m_operator = value; }
    }

    /// <summary>
    /// The predecessor of this node, used to keep track of the trajectory.
    /// </summary>
    public Node Predecessor
    {
      get { return m_predecessor; }
      private set { m_predecessor = value; }
    }

    /// <summary>
    /// The depth of this node in the search graph.
    /// </summary>
    public int Depth
    {
      get { return m_depth; }
    }

    /// <summary>
    /// The f-cost is the sum of the g-cost and the h-cost.
    /// </summary>
    public double FCost
    {
      get { return m_gCost + m_hCost; }
    }

    /// <summary>
    /// The g-cost is the cumulative cost from the starting point.
    /// It is evaluated with the problem's metric.
    /// </summary>
    public double GCost
    {
      get { return m_gCost; }
      set { m_gCost = value; }
    }

    /// <summary>
    /// The h-cost is an estimation of the cost-to-go to the final world.
    /// </summary>
    public double HCost
    {
      get { return m_hCost; }
      set { m_hCost = value; }
    }

    /// <summary>
    /// Whether this node is a goal node.
    /// Note that this does NOT imply its inner goal world has been idled.
    /// </summary>
    public bool IsGoal
    {
      get { return m_isGoal; }
      set { m_isGoal = value; }
    }

    /// <summary>
    /// Returns whether this node is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this node is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      bool equals = obj == this;

      if (!equals)
      {
        // Assume same type
        Node other = (Node)obj;
        equals = this.m_world.Equals(other.m_world);
      }
      return equals;
    }

    /// <summary>
    /// Returns the hash code of this node.
    /// </summary>
    /// <returns>The hash code of this node.</returns>
    public override int GetHashCode()
    {
      // Hash code must NOT use predecessor or f/g/h cost

      // Make sure hash code never changes.
      System.Diagnostics.Debug.Assert(m_worldHashCode == m_world.GetHashCode());

      return m_worldHashCode;
    }

    /// <summary>
    /// Returns the string representation of this node.
    /// </summary>
    /// <returns>The string representation of this node.</returns>
    public override string ToString()
    {
      return String.Format("({5}) {4}{0} ({1} + {2}){3}", this.GetHashCode(), GCost, HCost, 
                            World.IsIdleGoalWorld() ? " IDLE GOAL" : String.Empty,
                            (Operator != null ? Operator.ToString() + ": " : ""),
                            World.WorldNumber);
    }

    #region IComparable<Node> Interface

    /// <summary>
    /// Compares this node with another node.
    /// </summary>
    /// <param name="other">The other node to compare this node to.</param>
    /// <returns>An integer representing the total order relation between the two nodes.
    /// </returns>
    public int CompareTo(Node other)
    {
      return this.m_world.CompareTo(other.m_world);
    }

    #endregion
  }
}
