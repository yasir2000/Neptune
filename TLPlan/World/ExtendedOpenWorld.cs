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
using PDDLParser.World;
using PDDLParser.World.Context;

namespace TLPlan.World
{
  /// <summary>
  /// An extended open world is an open world which can be copied and compared to
  /// other extended worlds.
  /// </summary>
  public abstract class ExtendedOpenWorld : IOpenWorld, IComparable<ExtendedOpenWorld>
  {
    #region Private Fields

    /// <summary>
    /// The TLPlan options.
    /// </summary>
    protected TLPlanOptions m_options;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new extended open world with the given set of options.
    /// </summary>
    /// <param name="options">A set of options.</param>
    public ExtendedOpenWorld(TLPlanOptions options)
    {
      this.m_options = options;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies this extended world.
    /// </summary>
    /// <returns>A copy of this extended world.</returns>
    public abstract ExtendedOpenWorld Copy();

    #endregion

    #region IReadOnlyOpenWorld Members

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, or unknown.</returns>
    public abstract FuzzyBool IsSet(AtomicFormulaApplication formula);

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public abstract FuzzyDouble InternalGetNumericFluent(NumericFluentApplication fluent);

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// This function throws if the fluent is undefined and undefined fluents are not allowed.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public FuzzyDouble GetNumericFluent(NumericFluentApplication fluent)
    {
      FuzzyDouble value = InternalGetNumericFluent(fluent);
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
    public abstract FuzzyConstantExp InternalGetObjectFluent(ObjectFluentApplication fluent);

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    ///     /// This function throws if the fluent is undefined and undefined fluents are not allowed.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public FuzzyConstantExp GetObjectFluent(ObjectFluentApplication fluent)
    {
      FuzzyConstantExp value = InternalGetObjectFluent(fluent);
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
      existing = false;
      return new EvaluationRecord<FuzzyBoolValue>();
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
      existing = false;
      return new EvaluationRecord<FuzzyDouble>();
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
      existing = false;
      return new EvaluationRecord<FuzzyConstantExp>();
    }
    #endregion

    #region IOpenWorld Members

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public abstract void Set(AtomicFormulaApplication formula);

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formula">A atomic formula with constant arguments.</param>
    public abstract void Unset(AtomicFormulaApplication formula);

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public abstract void SetNumericFluent(NumericFluentApplication fluent, double value);

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public abstract void SetObjectFluent(ObjectFluentApplication fluent, Constant value);

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    public abstract void UndefineObjectFluent(ObjectFluentApplication fluent);

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this world is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this world is equal to the other object.</returns>
    public override abstract bool Equals(object obj);

    /// <summary>
    /// Returns the hash code of this world.
    /// </summary>
    /// <returns>The hash code of this world.</returns>
    public override abstract int GetHashCode();

    #endregion

    #region IComparable<ExtendedOpenWorld> Interface

    /// <summary>
    /// Compares this world with another world.
    /// </summary>
    /// <param name="other">The other world to compare this world to.</param>
    /// <returns>An integer representing the total order relation between the two worlds.
    /// </returns>
    public abstract int CompareTo(ExtendedOpenWorld other);

    #endregion
  }
}
