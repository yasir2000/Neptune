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
using System.IO;
using System.Linq;
using System.Text;
using PDDLParser;
using TLPlan.Utils;
using TLPlan.Utils.MultiDictionary;
using TLPlan.World;

namespace TLPlan
{
  /// <summary>
  /// Represents a plan, i.e. certain number of operators with a partial order relationship.
  /// </summary>
  public class Plan
  {
    #region Nested Classes

    /// <summary>
    /// Represents an operator that can be ordered in a plan.
    /// </summary>
    public class PlanOperator : IComparable<PlanOperator>
    {
      #region Private Fields

      /// <summary>
      /// The operator.
      /// </summary>
      private IOperator m_op;
      /// <summary>
      /// The time elapsed since the beginning of the plan at which time the operator was used.
      /// </summary>
      private double m_elapsedTime;
      /// <summary>
      /// The duration of the operator.
      /// </summary>
      private double m_duration;
      /// <summary>
      /// The sequence number of the operator.
      /// </summary>
      private int m_sequenceNr;

      #endregion

      #region Properties

      /// <summary>
      /// Gets the operator.
      /// </summary>
      public IOperator Operator
      {
        get { return m_op; }
      }

      /// <summary>
      /// Gets the elapsed time since the beginning of planning.
      /// </summary>
      public double ElapsedTime
      {
        get { return m_elapsedTime; }
      }

      /// <summary>
      /// Gets whether the operator should be elided from the plan.
      /// </summary>
      public bool IsElided
      {
        get { return m_op.IsElided; }
      }

      /// <summary>
      /// Gets the operator's sequence number.
      /// </summary>
      public int SequenceNr
      {
        get { return m_sequenceNr; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a plan operator with no duration.
      /// </summary>
      /// <param name="op">The operator.</param>
      /// <param name="sequenceNr">The operator's sequence number.</param>
      /// <param name="elapsedTime">The elapsed time since the beginning of planning.</param>
      public PlanOperator(IOperator op, int sequenceNr, double elapsedTime)
        : this(op, sequenceNr, elapsedTime, 0.0)
      { }

      /// <summary>
      /// Creates a plan operator.
      /// </summary>
      /// <param name="op">The operator.</param>
      /// <param name="sequenceNr">The operator's sequence number.</param>
      /// <param name="elapsedTime">The elapsed time since the beginning of planning.</param>
      /// <param name="duration">The duration of the operator.</param>
      public PlanOperator(IOperator op, int sequenceNr, double elapsedTime, double duration)
      {
        this.m_op = op;
        this.m_sequenceNr = sequenceNr;
        this.m_elapsedTime = elapsedTime;
        this.m_duration = duration;
      }

      #endregion

      /// <summary>
      /// Compares this plan operator with another plan operator.
      /// </summary>
      /// <param name="po">The other plan operator to compare this plan operator to.</param>
      /// <returns>An integer representing the total order relation between the two plan operators.</returns>
      public int CompareTo(PlanOperator po)
      {
        return m_sequenceNr.CompareTo(po.SequenceNr);
      }

      /// <summary>
      /// Returns true if this plan operator is equal to a specified object.
      /// </summary>
      /// <param name="obj">Object to test for equality.</param>
      /// <returns>True if this plan operator is equal to the specified objet.</returns>
      public override bool Equals(object obj)
      {
        if (obj == this)
        {
          return true;
        }
        else if (obj.GetType() == this.GetType())
        {
          PlanOperator po = (PlanOperator)obj;
          return (SequenceNr.Equals(po.SequenceNr));
        }
        else
        {
          return false;
        }
      }

      /// <summary>
      /// Returns the hash code of this plan operator.
      /// </summary>
      /// <returns>The hash code of this plan operator.</returns>
      public override int GetHashCode()
      {
        return SequenceNr.GetHashCode();
      }

      /// <summary>
      /// Returns a string representation of this plan operator.
      /// </summary>
      /// <returns>A string representation of this plan operator.</returns>
      public override string ToString()
      {
        return m_sequenceNr + ":" + m_op.ToString();
      }

      /// <summary>
      /// Returns a string representation of this plan operator, specially formatted for plan output.
      /// </summary>
      /// <returns>A string representation of this plan operator, specially formatted for plan output.</returns>
      public string ToPlanString()
      {
        return string.Format("{0:0.000000} : {1}{2}", m_elapsedTime, m_op.ToString(),
                                                     (m_duration == 0.0 ? string.Empty
                                                                        : string.Format(" [{0:0.000000}]", m_duration)));
      }
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// List of all operators in the plan.
    /// </summary>
    private List<PlanOperator> m_operators = new List<PlanOperator>();

    /// <summary>
    /// Sorted set of pairs of operators (representing the predecessor-successor relationship)
    /// </summary>
    private IMultiDictionary<PlanOperator, PlanOperator> m_order = new MultiDictionary<PlanOperator, PlanOperator>();

    /// <summary>
    /// The cost of the plan.
    /// </summary>
    private double m_cost;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of all operators in the plan.
    /// </summary>
    public IEnumerable<PlanOperator> Operators { get { return m_operators; } }

    /// <summary>
    /// Gets the sorted set of pairs of operators (representing the predecessor-successor relationship)
    /// </summary>
    public IEnumerable<KeyValuePair<PlanOperator, PlanOperator>> Order { get { return m_order; } }

    /// <summary>
    /// Gets the cost of the plan.
    /// </summary>
    public double PlanCost { get { return m_cost; } }

    #endregion

    #region Static Methods

    /// <summary>
    /// Reconstructs a plan from its last node (the one containing the goal world for a full plan).
    /// </summary>
    /// <param name="node">The last node of the plan (the one containing the goal world for a full plan).</param>
    /// <returns>A plan that leads to the given last node.</returns>
    public static Plan ReconstructPlan(Node node)
    {
      Plan plan = new Plan();

      plan.ScanPlan(node);
      plan.m_cost = node.GCost;

      return plan;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Recursively scans the input node until the first one is reached, then adds
    /// the operators and their order to reconstruct the plan.
    /// </summary>
    /// <param name="node">The node to scan.</param>
    /// <returns>A plan operator representing the scanned node.</returns>
    private PlanOperator ScanPlan(Node node)
    {
      if (node == null || node.Operator == null)
      {
        return null;
      }
      else
      {
        PlanOperator prevOp = ScanPlan(node.Predecessor);
        PlanOperator curOp = AddOperator(node);

        if (prevOp != null)
        {
          AddOrder(prevOp, curOp);
        }
        return curOp;
      }
    }

    /// <summary>
    /// Adds an plan operator to the plan an returns it.
    /// </summary>
    /// <param name="node">The node used to create the plan operator.</param>
    /// <returns>The created plan operator, based on the given node.</returns>
    private PlanOperator AddOperator(Node node)
    {
      TLPlanReadOnlyDurativeClosedWorld prevWorld = node.Predecessor.World;
      IOperator op = node.Operator;
      int opNo = m_operators.Count;
      PlanOperator pOp;
      if (op.HasDuration)
        pOp = new PlanOperator(op, opNo, prevWorld.GetTotalTime(), op.GetDuration(prevWorld));
      else
        pOp = new PlanOperator(op, opNo, prevWorld.GetTotalTime());

      m_operators.Add(pOp);
      return pOp;
    }

    /// <summary>
    /// Adds an order relation between the two plan operators.
    /// </summary>
    /// <param name="op1">The predecessor plan operator.</param>
    /// <param name="op2">The successor plan operator.</param>
    private void AddOrder(PlanOperator op1, PlanOperator op2)
    {
      m_order.Add(op1, op2);
    }

    #endregion

    #region Public Print Methods

    /// <summary>
    /// Prints the plan's operators on the given stream.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    public void PrintOperators(TextWriter stream)
    {
      foreach (Plan.PlanOperator op in this.Operators)
        stream.WriteLine(op);
    }

    /// <summary>
    /// Print the plan's operator order on the given stream.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    public void PrintOrder(TextWriter stream)
    {
      foreach (KeyValuePair<Plan.PlanOperator, Plan.PlanOperator> order in this.Order)
        stream.WriteLine("{0} -> {1}", order.Key, order.Value);
    }

    /// <summary>
    /// Prints the plan on the given stream.
    /// Each step has the following format: "time : operator [duration]"
    /// </summary>
    /// <param name="stream">The output stream.</param>
    public void PrintPlan(TextWriter stream)
    {
      // TODO: For now, the actual plan is given by the order in which the operators used in planning.
      foreach (Plan.PlanOperator op in this.Operators)
        if (!op.IsElided)
          stream.WriteLine(op.ToPlanString());
    }

    /// <summary>
    /// Prints the metric on the given stream.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    public void PrintMetric(TextWriter stream)
    {
      stream.WriteLine(this.PlanCost.ToString("0.000000"));
    }

    #endregion
  }
}
