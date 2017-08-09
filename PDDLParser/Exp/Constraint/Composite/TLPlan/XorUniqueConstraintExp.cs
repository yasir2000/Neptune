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
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Constraint.TLPlan
{
  /// <summary>
  /// This class represents an exclusive disjunction (XOR!) of expressions. 
  /// This is not part of PDDL; it was rather added for TLPlan.
  /// </summary>
  /// <typeparam name="T">The type of the expressions.</typeparam>
  [TLPlan]
  public class AbstractXorUniqueExp<T> : ListExp<T>
    where T : class, IExp
  {
    /// <summary>
    /// Creates a new exclusive disjunction of expressions.
    /// </summary>
    /// <param name="exps">The expressions associated with the new exclusive 
    /// disjunction expression.</param>
    public AbstractXorUniqueExp(IEnumerable<T> exps)
      : base("xor!", exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new exclusive disjunction of expressions.
    /// </summary>
    /// <param name="exps">The expressions associated with the new exclusive 
    /// disjunction expression.</param>
    public AbstractXorUniqueExp(params T[] exps)
      : this((IEnumerable<T>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }
  }

  /// <summary>
  /// This class represents an exclusive disjunction (XOR!) of constraint expressions. 
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
  public class XorUniqueConstraintExp : AbstractXorUniqueExp<IConstraintExp>, IConstraintExp
  {
    /// <summary>
    /// Creates a new exclusive disjunction of constraint expressions.
    /// </summary>
    /// <param name="exps">The constraint expressions associated with the new exclusive 
    /// disjunction expression.</param>
    public XorUniqueConstraintExp(IEnumerable<IConstraintExp> exps)
      : base(exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Creates a new exclusive disjunction of constraint expressions.
    /// </summary>
    /// <param name="exps">The constraint expressions associated with the new exclusive 
    /// disjunction expression.</param>
    public XorUniqueConstraintExp(params IConstraintExp[] exps)
      : this((IEnumerable<IConstraintExp>)exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Returns the expression equivalent to this exclusive disjunctive expression.
    /// </summary>
    /// <returns>The expression equivalent to this exclusive disjunctive expression.</returns>
    private IConstraintExp GenerateEquivalentExp()
    {
      List<IConstraintExp> orExpressions = new List<IConstraintExp>();
      for (int i = 0; i < this.m_expressions.Count; ++i)
      {
        List<IConstraintExp> andExpressions = new List<IConstraintExp>();
        for (int j = 0; j < this.m_expressions.Count; ++j)
        {
          if (i == j)
            andExpressions.Add((IConstraintExp)this.m_expressions[j]);
          else
            andExpressions.Add(new NotConstraintExp((IConstraintExp)this.m_expressions[j]));
        }
        orExpressions.Add(new AndConstraintExp(andExpressions));
      }
      return new OrConstraintExp(orExpressions);
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.Progress"/>
    public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      bool oneTrue = false;
      List<IConstraintExp> progressions = new List<IConstraintExp>(this.m_expressions.Count);
      FuzzyBool value = FuzzyBool.False;
      foreach (IConstraintExp exp in this.m_expressions)
      {
        ProgressionValue result = exp.Progress(world, bindings);
        if (result.Exp != null)
        {
          progressions.Add(result.Exp);
          value = value ^ FuzzyBool.Unknown;
        }
        else
        {
          if (result == ProgressionValue.True)
          {
            if (oneTrue)
              return ProgressionValue.False;
            else
              oneTrue = true;
          }
          value = value ^ new FuzzyBool(result.Value);
        }
      }

      if (value != FuzzyBool.Unknown)
      {
        return new ProgressionValue(value.ToBoolValue());
      }
      else
      {
        if (oneTrue)
        {
          // All others must be false
          if (progressions.Count == 1)
          {
            return new ProgressionValue(new NotConstraintExp(progressions[0]), ProgressionValue.NoTimestamp);
          }
          else
          {
            return new ProgressionValue(new NotConstraintExp(new OrConstraintExp(progressions)), ProgressionValue.NoTimestamp);
          }
        }
        else
        {
          // One must be true, all others false
          if (progressions.Count == 1)
          {
            return new ProgressionValue(progressions[0], ProgressionValue.NoTimestamp);
          }
          else
          {
            return new ProgressionValue(new XorUniqueConstraintExp(progressions), ProgressionValue.NoTimestamp);
          }
        }
      }
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.EvaluateIdle"/>
    public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      // See class comments for implementation details.
      bool oneTrue = false;
      Bool value = Bool.False;
      foreach (IConstraintExp exp in this.m_expressions)
      {
        Bool result = exp.EvaluateIdle(idleWorld, bindings);
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
  }
}
