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
using PDDLParser.Exception;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;
using TLPlan.Utils;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// An invariant world is read-only world that holds facts and fluents values.
  /// As specified in <see cref="IInvariantWorld"/>, an invariant world cannot return unknown
  /// when queried for the value of an invariant formula.
  /// It holds its facts and fluents in a fixed-size structure (respectively bitset and array) 
  /// to allow constant-time retrieval. Although it is true that these structures are more expensive
  /// memory-wise than other possible implementations (hashset, treeset...), this isn't an issue
  /// since the invariant world is initialized only once.
  /// </summary>
  public class InvariantWorld : IReadOnlyOpenWorld, IInvariantWorld
  {
    #region Private Fields

    /// <summary>
    /// The facts container holds all facts.
    /// </summary>
    protected FactsContainer m_factsContainer;

    /// <summary>
    /// The fluents container holds all numeric and object fluents.
    /// </summary>
    protected FluentsContainer m_fluentsContainer;

    /// <summary>
    /// The TLPlan options.
    /// </summary>
    protected TLPlanOptions m_options;

    /// <summary>
    /// The evaluation cache.
    /// </summary>
    protected FuzzyEvaluationCache m_cache;

    /// <summary>
    /// Whether to skip evaluation of certain defined formulas.
    /// </summary>
    protected IDictionary<DefinedFormula, bool> m_skipDefinedFormulas;

    #endregion   

    #region Constructor

    /// <summary>
    /// Creates a new custom world with the specified facts and fluents containers.
    /// </summary>
    /// <param name="factsContainer">The facts container.</param>
    /// <param name="fluentsContainer">The fluents container.</param>
    /// <param name="options">The TLPlan options.</param>
    public InvariantWorld(BitSetFactsContainer factsContainer, ArrayFluentsContainer fluentsContainer,
                          TLPlanOptions options)
    {
      this.m_factsContainer = factsContainer;
      this.m_fluentsContainer = fluentsContainer;
      this.m_options = options;
      this.m_skipDefinedFormulas = new Dictionary<DefinedFormula, bool>();
      this.m_cache = new FuzzyEvaluationCache();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns whether to skip evaluation of the given defined formula.
    /// </summary>
    /// <param name="formula">A defined formula.</param>
    /// <returns>Whether to skip evaluation of the given defined formula</returns>
    private bool SkipDefinedFormula(DefinedFormula formula)
    {
      bool skip = m_options.PreprocessLevel <= TLPlanOptions.PreprocessingLevel.ALL_EXCEPT_DEFINED_FORMULAS;
      //if (!skip)
      //{
      //  if (!m_skipDefinedFormulas.TryGetValue(formula, out skip))
      //  {
      //    // Check whether this derived predicate always evaluates to "unknown" when using invariants.
      //    skip = formula.GetEvaluationDomain(new InvariantEvaluationContext()).Equals(EvaluationDomain.Unknown);
      //    m_skipDefinedFormulas.Add(formula, skip);
      //  }
      //}
      return skip;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, or unknown.</returns>
    public FuzzyBool IsSet(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      return this.m_factsContainer.IsSet(formula.FormulaID);
    }

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public FuzzyDouble GetNumericFluent(NumericFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      FuzzyDouble value = this.m_fluentsContainer.GetNumericFluent(fluent.FormulaID);
      if (!m_options.AllowUndefinedFluents && value.Status == FuzzyDouble.State.Undefined)
        throw new UndefinedExpException(fluent.ToString() + " is undefined (option \"disallow undefined fluents\" is set).");

      return value;
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public FuzzyConstantExp GetObjectFluent(ObjectFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      FuzzyConstantExp value = this.m_fluentsContainer.GetObjectFluent(fluent.FormulaID);
      if (!m_options.AllowUndefinedFluents && value.Status == FuzzyConstantExp.State.Undefined)
        throw new UndefinedExpException(fluent.ToString() + " is undefined (option \"disallow undefined fluents\" is set).");

      return value;
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined predicate has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined predicate application.</param>
    /// <param name="existing">This flag is set to true if the defined predicate is already in the
    /// process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined predicate and 
    /// arguments.</returns>
    public IEvaluationRecord<FuzzyBoolValue> GetEvaluation(DefinedPredicateApplication pred,
                                                           out bool existing)
    {
      if (SkipDefinedFormula(pred.getDefinedPredicate()))
      {
        existing = true;
        return new EvaluationRecord<FuzzyBoolValue>(FuzzyBoolValue.Unknown);
      }
      else
      {
        IEvaluationRecord<FuzzyBoolValue> evaluation = m_cache.GetEvaluation(pred, out existing);
        if (existing && !evaluation.Finished)
        {
          // Cycle detected! It may happen since the evaluation is taking place in the 
          // invariant world, which often yields unknown values.
          evaluation.Finished = true;
          evaluation.Result = FuzzyBoolValue.Unknown;
        }
        return evaluation;
      }
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined numeric function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="function">The defined numeric function application.</param>
    /// <param name="existing">This flag is set to true if the defined numeric function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined numeric function and 
    /// arguments.</returns>
    public IEvaluationRecord<FuzzyDouble> GetEvaluation(DefinedNumericFunctionApplication function,
                                                        out bool existing)
    {
      if (SkipDefinedFormula(function.RootFunction))
      {
        existing = true;
        return new EvaluationRecord<FuzzyDouble>(FuzzyDouble.Unknown);
      }
      else
      {
        IEvaluationRecord<FuzzyDouble> evaluation = m_cache.GetEvaluation(function, out existing);
        if (existing && !evaluation.Finished)
        {
          // Cycle detected! It may happen since the evaluation is taking place in the 
          // invariant world, which often yields unknown values.
          evaluation.Finished = true;
          evaluation.Result = FuzzyDouble.Unknown;
        }
        return evaluation;
      }
    }

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined object function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="function">The defined object function application.</param>
    /// <param name="existing">This flag is set to true if the defined object function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined object function and 
    /// arguments.</returns>
    public IEvaluationRecord<FuzzyConstantExp> GetEvaluation(DefinedObjectFunctionApplication function,
                                                             out bool existing)
    {
      if (SkipDefinedFormula(function.getDefinedObjectFunction()))
      {
        existing = true;
        return new EvaluationRecord<FuzzyConstantExp>(FuzzyConstantExp.Unknown);
      }
      else
      {
        IEvaluationRecord<FuzzyConstantExp> evaluation = m_cache.GetEvaluation(function, out existing);
        if (existing && !evaluation.Finished)
        {
          // Cycle detected! It may happen since the evaluation is taking place in the 
          // invariant world, which often yields unknown values.
          evaluation.Finished = true;
          evaluation.Result = FuzzyConstantExp.Unknown;
        }
        return evaluation;
      }
    }

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public void SetInitialPredicate(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      this.m_factsContainer.Set(formula.FormulaID);
    }

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public void SetInitialNumericFluent(NumericFluentApplication fluent, double value)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.SetNumericFluent(fluent.FormulaID, value);
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public void SetInitialObjectFluent(ObjectFluentApplication fluent, Constant value)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.SetObjectFluent(fluent.FormulaID, value);
    }

    #endregion

    #region IInvariantWorld Members

    /// <summary>
    /// The interval of predicates ID whose value are stored in this invariant world.
    /// </summary>
    public IntegerInterval InvariantPredicateInterval
    {
      get
      {
        return ((BitSetFactsContainer)this.m_factsContainer).DefinitionInterval;
      }
    }

    /// <summary>
    /// The interval of numeric fluents ID whose value are stored in this invariant world.
    /// </summary>
    public IntegerInterval InvariantNumericFluentInterval
    {
      get
      {
        return ((ArrayFluentsContainer)this.m_fluentsContainer).NumericFluentDefinitionInterval;
      }
    }

    /// <summary>
    /// The interval of object fluents ID whose value are stored in this invariant world.
    /// </summary>
    public IntegerInterval InvariantObjectFluentInterval
    {
      get
      {
        return ((ArrayFluentsContainer)this.m_fluentsContainer).ObjectFluentDefinitionInterval;
      }
    }

    #endregion
  }
}
