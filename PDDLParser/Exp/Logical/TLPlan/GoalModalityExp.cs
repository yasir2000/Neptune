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
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical.TLPlan
{
  /// <summary>
  /// A goal modality determines if the goal expression implies another expression,
  /// that is, if for all worlds w (goal |= w) -> (exp |= w).
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class GoalModalityExp : AbstractLogicalExp
  {
    /// <summary>
    /// Delegate returning a set of worlds satisfying a problem's goal formulation.
    /// </summary>
    /// <returns>A set of worlds satisfying a problem's goal formulation.</returns>
    public delegate HashSet<PartialWorld> GetAllGoalWorldsDelegate();

    /// <summary>
    /// The logical expression to test for implication by the goal expression.
    /// </summary>
    private ILogicalExp m_exp;
    /// <summary>
    /// All worlds which satisfy the goal expression.
    /// </summary>
    private GetAllGoalWorldsDelegate m_getAllGoalWorlds;
    /// <summary>
    /// The cached evaluation results.
    /// </summary>
    private IDictionary<LocalBindings, bool> m_evaluationCache;

    /// <summary>
    /// Creates a new goal modality with a specified expression and the list of all worlds
    /// which satisfy the goal expression.
    /// </summary>
    /// <param name="exp">The logical expression to test for implication.</param>
    /// <param name="getAllGoalWorlds">All contexts which satisfy the goal expression.</param>
    public GoalModalityExp(ILogicalExp exp, GetAllGoalWorldsDelegate getAllGoalWorlds)
      : base()
    {
      System.Diagnostics.Debug.Assert(exp != null && getAllGoalWorlds != null);

      this.m_exp = exp;
      this.m_getAllGoalWorlds = getAllGoalWorlds;
      this.m_evaluationCache = new Dictionary<LocalBindings, bool>();
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    public override FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new FuzzyBool(Evaluate(bindings));
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    [TLPlan]
    public override ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new ShortCircuitFuzzyBool(Evaluate(bindings));
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    public override LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new LogicalValue(new Bool(Evaluate(bindings)));
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    public override Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new Bool(Evaluate(bindings));
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    [TLPlan]
    public override ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ShortCircuitBool(Evaluate(bindings));
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotImplementedException">A NotImplementedException is always thrown
    /// by this function.</exception>
    public override HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True or false.</returns>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return Evaluate(idleWorld, bindings);
    }

    /// <summary>
    /// Evaluates this goal modality.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Whether the goal expression implies the goal modality's expression.</returns>
    private bool Evaluate(LocalBindings bindings)
    {
      // TODO: keep bindings on body variables only (m_exp.GetFreeVariables)?
      bool value;
      if (this.m_evaluationCache.TryGetValue(bindings, out value))
      {
        return value;
      }
      else
      {
        HashSet<PartialWorld> allWorlds = m_getAllGoalWorlds();
        System.Diagnostics.Debug.Assert(allWorlds != null);
        bool result = true;
        foreach (IReadOnlyOpenWorld goalContext in allWorlds)
        {
          // If exp evaluates to unknown then exp can be both true or false.
          // GOAL(f) holds if and only if f holds in all worlds where the goal holds.
          if (this.m_exp.Evaluate(goalContext, bindings) != FuzzyBool.True)
          {
            result = false;
            break;
          }
        }
        this.m_evaluationCache.Add(bindings, result);
        return result;
      }
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return m_exp.IsGround();
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      GoalModalityExp other = (GoalModalityExp)base.Apply(bindings);
      other.m_exp = (ILogicalExp)this.m_exp.Apply(bindings);
      other.m_evaluationCache = new Dictionary<LocalBindings, bool>();

      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      GoalModalityExp other = (GoalModalityExp)base.Standardize(images);
      other.m_exp = (ILogicalExp)this.m_exp.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return m_exp.GetFreeVariables();
    }

    /// <summary>
    /// Returns true if this goal modality is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this goal modality is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        return (obj.GetType() == this.GetType() && ((GoalModalityExp)obj).m_exp.Equals(this.m_exp));
      }
    }

    /// <summary>
    /// Returns the hash code of this goal modality.
    /// </summary>
    /// <returns>The hash code of this goal modality.</returns>
    public override int GetHashCode()
    {
      return m_exp.GetHashCode() * 5;
    }

    /// <summary>
    /// Returns a typed string of this expression.
    /// </summary>
    /// <returns>A typed string of this expression.</returns>
    public override string ToTypedString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("(GOAL ");
      sb.Append(m_exp.ToTypedString());
      sb.Append(")");
      return sb.ToString();
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representtation of this expression.</returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("(GOAL ");
      sb.Append(m_exp.ToString());
      sb.Append(")");
      return sb.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this goal modality with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this goal modality to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);

      return (value != 0 ? value : this.m_exp.CompareTo(((GoalModalityExp)other).m_exp));
    }

    #endregion
  }
}
