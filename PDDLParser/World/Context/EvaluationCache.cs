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
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.World.Context
{
  /// <summary>
  /// A fuzzy evaluation cache stores evaluation results of formulas evaluated in open worlds.
  /// </summary>
  public class FuzzyEvaluationCache : GenericEvaluationCache<FuzzyBoolValue, FuzzyDouble, FuzzyConstantExp>
  {
  }

  /// <summary>
  /// An evaluation cache stores evaluation results of formulas evaluated in closed worlds.
  /// </summary>
  public class EvaluationCache : GenericEvaluationCache<BoolValue, Double, ConstantExp>
  {
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
    public IEvaluationRecord<FuzzyBoolValue> GetFuzzyEvaluation(DefinedPredicateApplication pred,
                                                                out bool existing)
    {
      return new FuzzyCertaintyEvaluationRecordWrapper(this.GetEvaluation(pred, out existing));
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
    public IEvaluationRecord<FuzzyDouble> GetFuzzyEvaluation(DefinedNumericFunctionApplication function,
                                                             out bool existing)
    {
      return new FuzzyNumericEvaluationRecordWrapper(this.GetEvaluation(function, out existing));
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
    public IEvaluationRecord<FuzzyConstantExp> GetFuzzyEvaluation(DefinedObjectFunctionApplication function,
                                                                  out bool existing)
    {
      return new FuzzyObjectEvaluationRecordWrapper(this.GetEvaluation(function, out existing));
    }
  }

  /// <summary>
  /// An evaluation cache stores evaluation records in a dictionary.
  /// </summary>
  public class GenericEvaluationCache<BooleanType, NumericType, ObjectType>
  {
    /// <summary>
    /// The evaluation records for all evaluated defined predicates along with their arguments.
    /// </summary>
    private IDictionary<DefinedPredicate, IDictionary<int, EvaluationRecord<BooleanType>>> m_predicateEvaluations;

    /// <summary>
    /// The evaluation records for all evaluated defined numeric functions along with their arguments.
    /// </summary>
    private IDictionary<DefinedNumericFunction, IDictionary<int, EvaluationRecord<NumericType>>> m_functionEvaluations;

    /// <summary>
    /// The evaluation records for all evaluated defined object functions along with their arguments.
    /// </summary>
    private IDictionary<DefinedObjectFunction, IDictionary<int, EvaluationRecord<ObjectType>>> m_objectFunctionEvaluations;

    /// <summary>
    /// Creates a new empty evaluation cache.
    /// </summary>
    public GenericEvaluationCache()
    {
      this.m_predicateEvaluations = new Dictionary<DefinedPredicate, IDictionary<int, EvaluationRecord<BooleanType>>>();
      this.m_functionEvaluations = new Dictionary<DefinedNumericFunction, IDictionary<int, EvaluationRecord<NumericType>>>();
      this.m_objectFunctionEvaluations = new Dictionary<DefinedObjectFunction, IDictionary<int, EvaluationRecord<ObjectType>>>();
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
    public IEvaluationRecord<BooleanType> GetEvaluation(DefinedPredicateApplication pred, 
                                                        out bool existing)
    {
      int argumentsID = pred.FormulaID;
      IDictionary<int, EvaluationRecord<BooleanType>> evaluations;
      EvaluationRecord<BooleanType> evaluation;
      if (!m_predicateEvaluations.TryGetValue(pred.getDefinedPredicate(), out evaluations))
      {
        existing = false;
        evaluations = new Dictionary<int, EvaluationRecord<BooleanType>>();
        m_predicateEvaluations.Add(pred.getDefinedPredicate(), evaluations);
        evaluation = new EvaluationRecord<BooleanType>();
        evaluations.Add(argumentsID, evaluation);
      }
      else
      {
        existing = evaluations.TryGetValue(pred.FormulaID, out evaluation);
        if (!existing)
        {
          evaluation = new EvaluationRecord<BooleanType>();
          evaluations.Add(argumentsID, evaluation);
        }
      }
      return evaluation;
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
    public IEvaluationRecord<NumericType> GetEvaluation(DefinedNumericFunctionApplication function, 
                                                        out bool existing)
    {
      int argumentsID = function.FormulaID;
      IDictionary<int, EvaluationRecord<NumericType>> evaluations;
      EvaluationRecord<NumericType> evaluation;
      if (!m_functionEvaluations.TryGetValue(function.RootFunction, out evaluations))
      {
        existing = false;
        evaluations = new Dictionary<int, EvaluationRecord<NumericType>>();
        m_functionEvaluations.Add(function.RootFunction, evaluations);
        evaluation = new EvaluationRecord<NumericType>();
        evaluations.Add(argumentsID, evaluation);
      }
      else
      {
        existing = evaluations.TryGetValue(argumentsID, out evaluation);
        if (!existing)
        {
          evaluation = new EvaluationRecord<NumericType>();
          evaluations.Add(argumentsID, evaluation);
        }
      }
      return evaluation;
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
    public IEvaluationRecord<ObjectType> GetEvaluation(DefinedObjectFunctionApplication function,
                                                       out bool existing)
    {
      int argumentsID = function.FormulaID;
      IDictionary<int, EvaluationRecord<ObjectType>> evaluations;
      EvaluationRecord<ObjectType> evaluation;
      if (!m_objectFunctionEvaluations.TryGetValue(function.getDefinedObjectFunction(), out evaluations))
      {
        existing = false;
        evaluations = new Dictionary<int, EvaluationRecord<ObjectType>>();
        m_objectFunctionEvaluations.Add(function.getDefinedObjectFunction(), evaluations);
        evaluation = new EvaluationRecord<ObjectType>();
        evaluations.Add(argumentsID, evaluation);
      }
      else
      {
        existing = evaluations.TryGetValue(argumentsID, out evaluation);
        if (!existing)
        {
          evaluation = new EvaluationRecord<ObjectType>();
          evaluations.Add(argumentsID, evaluation);
        }
      }
      return evaluation;
    }
  }
}
