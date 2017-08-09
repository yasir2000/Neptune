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
using PDDLParser.Exp;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.World;
using PDDLParser.World.Context;
using TLPlan.World.Implementations;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World
{
  /// <summary>
  /// A transitive initial world is used to initialize both the initial world and the invariant
  /// world, and does so by forwarding all updates to the appropriate world (depending on whether
  /// the described formula to update is invariant).
  /// </summary>
  public class TransitiveInitialWorld : IDurativeOpenWorld
  {
    /// <summary>
    /// The initial world to initialize.
    /// </summary>
    private IDurativeOpenWorld m_initialWorld;

    /// <summary>
    /// The invariant world to initialize.
    /// </summary>
    private InvariantWorld m_invariants;

    /// <summary>
    /// Creates a new transitive initial world that will forward all updates to the
    /// initial world or the invariant world.
    /// </summary>
    /// <param name="initialWorld">The initial world to initialize.</param>
    /// <param name="invariants">The invariant world to initialize.</param>
    public TransitiveInitialWorld(IDurativeClosedWorld initialWorld,
                                  InvariantWorld invariants)
    {
      this.m_initialWorld = initialWorld;
      this.m_invariants = invariants;
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public bool IsIdleGoalWorld()
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public bool IsSet(AtomicFormulaApplication formula)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    FuzzyBool IReadOnlyOpenWorld.IsSet(AtomicFormulaApplication formula)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public void Set(AtomicFormulaApplication formula)
    {
      if (formula.Invariant)
      {
        m_invariants.SetInitialPredicate(formula);
      }
      else
      {
        m_initialWorld.Set(formula);
      }
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public IEvaluationRecord<FuzzyBoolValue> GetEvaluation(DefinedPredicateApplication pred, out bool existing)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public void Unset(AtomicFormulaApplication formula)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public Double GetNumericFluent(NumericFluentApplication fluent)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    FuzzyDouble IReadOnlyOpenWorld.GetNumericFluent(NumericFluentApplication fluent)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="function">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public void SetNumericFluent(NumericFluentApplication function, double value)
    {
      if (function.Invariant)
      {
        m_invariants.SetInitialNumericFluent(function, value);
      }
      else
      {
        m_initialWorld.SetNumericFluent(function, value);
      }
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public IEvaluationRecord<FuzzyDouble> GetEvaluation(DefinedNumericFunctionApplication pred, out bool existing)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public ConstantExp GetObjectFluent(ObjectFluentApplication fluent)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    FuzzyConstantExp IReadOnlyOpenWorld.GetObjectFluent(ObjectFluentApplication fluent)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="function">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public void SetObjectFluent(ObjectFluentApplication function, Constant value)
    {
      if (function.Invariant)
      {
        m_invariants.SetInitialObjectFluent(function, value);
      }
      else
      {
        m_initialWorld.SetObjectFluent(function, value);
      }
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public void UndefineObjectFluent(ObjectFluentApplication function)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public IEvaluationRecord<FuzzyConstantExp> GetEvaluation(DefinedObjectFunctionApplication pred, out bool existing)
    {
      // This should never be called
      throw new NotSupportedException();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">This function is not supported.
    /// </exception>
    public double GetTotalTime()
    {
      // This should never be called.
      throw new NotSupportedException();
    }

    /// <summary>
    /// Add an effect which will take place after a fixed duration.
    /// </summary>
    /// <param name="timeOffset">The relative time offset at which the effect takes place.</param>
    /// <param name="effect">The delayed effect.</param>
    public void AddEndEffect(double timeOffset, IEffect effect)
    {
      m_initialWorld.AddEndEffect(timeOffset, effect);
    }
  }
}
