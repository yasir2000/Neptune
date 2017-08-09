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
using PDDLParser.Exp.Constraint.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical.TLPlan
{
  /// <summary>
  /// An If-then-else expression is a conditional expression derived from implication expressions.
  /// (if-then-else e1 e2 e3) is actually a shortcut for (and (implies e1 e2) (implies (not e1) e3)),
  /// but only in the particular case where e1, e2 and e3 can only be true or false
  /// (not unknown nor undefined).
  /// If the condition e1 evaluates to unknown or undefined, the evaluation immediately exits and
  /// returns the aforementioned value.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  [TLPlan]
  public class IfThenElseExp : AbstractIfThenElseExp<ILogicalExp>, ILogicalExp
  {
    /// <summary>
    /// Creates a new IfThenElse expression with the specified condition, consequence and
    /// alternate consequence.
    /// </summary>
    /// <param name="ifExp">The condition.</param>
    /// <param name="thenExp">The consequence (the then clause).</param>
    /// <param name="elseExp">The alternate consequence (the else clause).</param>
    public IfThenElseExp(ILogicalExp ifExp, ILogicalExp thenExp, ILogicalExp elseExp)
      : base(ifExp, thenExp, elseExp)
    {
      System.Diagnostics.Debug.Assert(m_ifExp != null && m_thenExp != null && m_elseExp != null);
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyBool value = m_ifExp.Evaluate(world, bindings);
      if (value == FuzzyBool.True)
      {
        return m_thenExp.Evaluate(world, bindings);
      }
      else if (value == FuzzyBool.False)
      {
        return m_elseExp.Evaluate(world, bindings);
      }
      else
      {
        return value;
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    [TLPlan]
    public ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ShortCircuitFuzzyBool value = m_ifExp.EvaluateWithImmediateShortCircuit(world, bindings);
      if (value == ShortCircuitFuzzyBool.True)
      {
        return m_thenExp.EvaluateWithImmediateShortCircuit(world, bindings);
      }
      else if (value == ShortCircuitFuzzyBool.False)
      {
        return m_elseExp.EvaluateWithImmediateShortCircuit(world, bindings);
      }
      else
      {
        return value;
      }
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      LogicalValue value = m_ifExp.Simplify(world, bindings);
      if (value.Exp != null)
      {
        LogicalValue thenValue = m_thenExp.Simplify(world, bindings);
        LogicalValue elseValue = m_elseExp.Simplify(world, bindings);

        return new LogicalValue(new IfThenElseExp(value.Exp, thenValue.GetEquivalentExp(), 
                                                             elseValue.GetEquivalentExp()));
      }
      else
      {
        if (value == LogicalValue.True)
        {
          return m_thenExp.Simplify(world, bindings);
        }
        else if (value == LogicalValue.False)
        {
          return m_elseExp.Simplify(world, bindings);
        }
        else
        {
          return value;
        }
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      Bool value = m_ifExp.Evaluate(world, bindings);
      if (value == Bool.True)
      {
        return m_thenExp.Evaluate(world, bindings);
      }
      else if (value == Bool.False)
      {
        return m_elseExp.Evaluate(world, bindings);
      }
      else
      {
        return value;
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      ShortCircuitBool value = m_ifExp.EvaluateWithImmediateShortCircuit(world, bindings);
      if (value == ShortCircuitBool.True)
      {
        return m_thenExp.EvaluateWithImmediateShortCircuit(world, bindings);
      }
      else if (value == ShortCircuitBool.False)
      {
        return m_elseExp.EvaluateWithImmediateShortCircuit(world, bindings);
      }
      else
      {
        return value;
      }
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      return GenerateEquivalentExp().EnumerateAllSatisfyingWorlds();
    }

    /// <summary>
    /// Creates a new ground expression equivalent to this if-then-else expression.
    /// </summary>
    /// <returns>A new ground expression equivalent to this if-then-else expression.</returns>
    private ILogicalExp GenerateEquivalentExp()
    {
      return new AndExp(new ImplyExp(m_ifExp, m_thenExp),
                        new ImplyExp(new NotExp(m_ifExp), m_elseExp));
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// This function returns false if this constraint expression is not satisfied 
    /// in the given world;
    /// it returns true if the progression is always satisfied in the next worlds;
    /// else it returns the progressed expression.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(this.Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return this.Evaluate(idleWorld, bindings);
    }
  }
}
