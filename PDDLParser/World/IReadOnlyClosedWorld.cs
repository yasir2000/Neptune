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
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.World.Context;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.World
{
  /// <summary>
  /// Constant closed world supporting read-only operations.
  /// In a closed world, the value of predicates and fluents is always known. 
  /// Consequently predicates always evaluate to true or false. Fluents, on the other hand,
  /// may be undefined (see PDDL specification) but are never unknown.
  /// </summary>
  public interface IReadOnlyClosedWorld : IReadOnlyOpenWorld
  {
    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True or false.</returns>
    new bool IsSet(AtomicFormulaApplication formula);

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Undefined, or the value of the numeric fluent.</returns>
    new Double GetNumericFluent(NumericFluentApplication fluent);

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Undefined, or a constant representing the value of
    /// the object fluent.</returns>
    new ConstantExp GetObjectFluent(ObjectFluentApplication fluent);

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
    new IEvaluationRecord<BoolValue> GetEvaluation(DefinedPredicateApplication pred, out bool existing);

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined numeric function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined numeric function application.</param>
    /// <param name="existing">This flag is set to true if the defined numeric function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined numeric function and 
    /// arguments.</returns>
    new IEvaluationRecord<Double> GetEvaluation(DefinedNumericFunctionApplication pred, out bool existing);

    /// <summary>
    /// Returns an evaluation record which indicates whether the given defined object function has
    /// already been evaluated with the provided arguments, as well as the cached evaluation 
    /// value.
    /// </summary>
    /// <param name="pred">The defined object function application.</param>
    /// <param name="existing">This flag is set to true if the defined object function is already
    /// in the process of (or has finished) being evaluated.</param>
    /// <returns>An evaluation record corresponding to the specified defined object function and 
    /// arguments.</returns>
    new IEvaluationRecord<ConstantExp> GetEvaluation(DefinedObjectFunctionApplication pred, out bool existing);
  }
}
