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
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Logical.TLPlan
{
  /// <summary>
  /// This class represents an exclusive disjunction (XOR!) of logical expressions.
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  /// <remarks>
  /// PLEASE NOTE that this implementation of an n-ary xor returns
  /// true iff only one of its arguments is true (as is done in the
  /// original TLPlan). Thus its truth value can be calculated 
  /// independently from its false operands;
  /// for example: (xor T F F) = (xor T)
  /// 
  /// This is different from another definition of n-ary xor which
  /// returns true iff an odd number of its arguments are true. 
  /// </remarks>
  [TLPlan]
  public class XorUniqueExp : AbstractXorUniqueExp<ILogicalExp>, ILogicalExp
  {
    /// <summary>
    /// Creates a new exclusive disjunction of logical expressions.
    /// </summary>
    /// <param name="exps">The logical expressions associated with the new exclusive 
    /// disjunction expression.</param>
    public XorUniqueExp(IEnumerable<ILogicalExp> exps)
      : base(exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new exclusive disjunction of logical expressions.
    /// </summary>
    /// <param name="exps">The logical expressions associated with the new exclusive 
    /// disjunction expression.</param>
    public XorUniqueExp(params ILogicalExp[] exps)
      : this((IEnumerable<ILogicalExp>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      // See class comments for implementation details.
      bool oneTrue = false;
      FuzzyBool value = FuzzyBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        FuzzyBool result = exp.Evaluate(world, bindings);
        if (result == FuzzyBool.True)
        {
          if (oneTrue)
            return FuzzyBool.False;
          else
            oneTrue = true;
        }
        value = value ^ result;
        if (!value)
          break;
      }
      return value;
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
      // See class comments for implementation details.
      bool oneTrue = false;
      ShortCircuitFuzzyBool value = ShortCircuitFuzzyBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        ShortCircuitFuzzyBool result = exp.EvaluateWithImmediateShortCircuit(world, bindings);
        if (result == ShortCircuitFuzzyBool.True)
        {
          if (oneTrue)
            return ShortCircuitFuzzyBool.False;
          else
            oneTrue = true;
        }
        value = value ^ result;
        if (!value)
          break;
      }
      return value;
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      // See class comments for implementation details.

      bool oneTrue = false;
      List<ILogicalExp> simplifications = new List<ILogicalExp>(this.m_expressions.Count);
      FuzzyBool value = FuzzyBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        LogicalValue result = exp.Simplify(world, bindings);
        if (result.Exp != null)
        {
          simplifications.Add(result.Exp);
          value = value ^ FuzzyBool.Unknown;
        }
        else
        {
          if (result == LogicalValue.True)
          {
            if (oneTrue)
              return LogicalValue.False;
            else
              oneTrue = true;
          }
          value = value ^ new FuzzyBool(result.Value);
        }
      }

      if (value != FuzzyBool.Unknown)
      {
        return new LogicalValue(value.ToBoolValue());
      }
      else
      {
        if (oneTrue)
        {
          // All others must be false
          if (simplifications.Count == 1)
          {
            return new LogicalValue(new NotExp(simplifications[0]));
          }
          else
          {
            return new LogicalValue(new NotExp(new OrExp(simplifications)));
          }
        }
        else
        {
          // One must be true, all others false
          if (simplifications.Count == 1)
          {
            return new LogicalValue(simplifications[0]);
          }
          else
          {
            return new LogicalValue(new XorUniqueExp(simplifications));
          }
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
      // See class comments for implementation details.
      bool oneTrue = false;
      Bool value = Bool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        Bool result = exp.Evaluate(world, bindings);
        if (result == Bool.True)
        {
          if (oneTrue)
            return Bool.False;
          else
            oneTrue = true;
        }
        value = value ^ result;
        if (!value)
          break;
      }
      return value;
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// This function is used to evaluate defined formulas' body as is done in the
    /// original TLPlan.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      // See class comments for implementation details.

      bool oneTrue = false;
      ShortCircuitBool value = ShortCircuitBool.False;
      foreach (ILogicalExp exp in this.m_expressions)
      {
        ShortCircuitBool result = exp.EvaluateWithImmediateShortCircuit(world, bindings);
        if (result == ShortCircuitBool.True)
        {
          if (oneTrue)
            return ShortCircuitBool.False;
          else
            oneTrue = true;
        }
        value = value ^ result;
        if (!value)
          break;
      }
      return value;
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
    /// Creates a new ground expression equivalent to this exclusive disjunction expression.
    /// </summary>
    /// <returns>A new ground expression equivalent to this exclusive disjunction expression.
    /// </returns>
    private ILogicalExp GenerateEquivalentExp()
    {
      List<ILogicalExp> orExpressions = new List<ILogicalExp>();
      for (int i = 0; i < this.m_expressions.Count; ++i)
      {
        List<ILogicalExp> andExpressions = new List<ILogicalExp>();
        for (int j = 0; j < this.m_expressions.Count; ++j)
        {
          if (i == j)
            andExpressions.Add((ILogicalExp)this.m_expressions[j]);
          else
            andExpressions.Add(new NotExp((ILogicalExp)this.m_expressions[j]));
        }
        orExpressions.Add(new AndExp(andExpressions));
      }
      return new OrExp(orExpressions);
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