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
using PDDLParser.Exp.Term.Type;
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// An object fluent application is an application of an object fluent.
  /// Note that an object fluent application is undefined until its value has been set.
  /// It is also possible to undefine an object fluent application, for example in the context
  /// of durative actions where its value should not be accessed.
  /// </summary>
  public class ObjectFluentApplication : FluentApplication, ITerm, IComparable<ObjectFluentApplication>
  {
    /// <summary>
    /// Creates a new object fluent application of a specified object fluent with a given
    /// list of arguments.
    /// </summary>
    /// <param name="fluent">The object fluent to instantiate.</param>
    /// <param name="arguments">The arguments of this object fluent application.</param>
    public ObjectFluentApplication(ObjectFluent fluent, List<ITerm> arguments)
      : base(fluent, arguments)
    {
      System.Diagnostics.Debug.Assert(fluent != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Gets the object fluent associated with this object fluent application.
    /// </summary>
    public ObjectFluent RootObjectFluent
    {
      get { return (ObjectFluent)this.m_rootFormula; }
    }

    /// <summary>
    /// Returns a copy of this object fluent application with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments of the new fluent application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public override FormulaApplication Apply(List<ITerm> arguments)
    {
      return new ObjectFluentApplication(this.RootObjectFluent, arguments);
    }

    /// <summary>
    /// Returns the typeset of this objet fluent application.
    /// </summary>
    /// <returns>The typeset of this object fluent application.</returns>
    public TypeSet GetTypeSet()
    {
      return this.RootObjectFluent.GetTypeSet();
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// An object fluent application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the constant resulting from the evaluation.</returns>
    public FuzzyConstantExp Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      FuzzyArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case FuzzyArgsEvalResult.State.Defined:
          return world.GetObjectFluent((ObjectFluentApplication)result.Value);
        case FuzzyArgsEvalResult.State.Unknown:
          return FuzzyConstantExp.Unknown;
        case FuzzyArgsEvalResult.State.Undefined:
          return FuzzyConstantExp.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Simplifies this term by evaluating its known expression parts.
    /// An object fluent application is simplified by first simplifying its arguments and then
    /// by attempting to retrieve its value in the specified world (if it is ground).
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the term (possibly a constant) resulting from 
    /// the simplification.</returns>
    public TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.SimplifyArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          ObjectFluentApplication formula = (ObjectFluentApplication)result.Value;
          if (!formula.AllConstantArguments)
          {
            return new TermValue(formula);
          }
          else
          {
            FuzzyConstantExp value = world.GetObjectFluent(formula);
            switch (value.Status)
            {
              case FuzzyConstantExp.State.Defined:
                return new TermValue(value.Value);
              case FuzzyConstantExp.State.Unknown:
                return new TermValue(formula);
              case FuzzyConstantExp.State.Undefined:
                return TermValue.Undefined;
              default:
                throw new System.Exception("Invalid FuzzyConstantValueStatus status: " + value.Status);
            }
          }
        case ArgsEvalResult.State.Undefined:
          return TermValue.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this term in the specified closed world.
    /// An object fluent application is evaluated by first evaluating its arguments and then
    /// by retrieving its value in the specified world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the constant resulting from the evaluation.</returns>
    public ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      ArgsEvalResult result = this.EvaluateArguments(world, bindings);
      switch (result.Status)
      {
        case ArgsEvalResult.State.Defined:
          return world.GetObjectFluent((ObjectFluentApplication)result.Value);
        case ArgsEvalResult.State.Undefined:
          return ConstantExp.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Verifies whether the specified term can be assigned to this term, 
    /// i.e. if the other term's domain is a subset of this term's domain.
    /// </summary>
    /// <param name="term">The other term.</param>
    /// <returns>True if the types are compatible, false otherwise.</returns>
    public bool CanBeAssignedFrom(ITerm term)
    {
      return this.GetTypeSet().CanBeAssignedFrom(term.GetTypeSet());
    }

    /// <summary>
    /// Verifies whether the specified term can be compared to this term,
    /// i.e. if their domain overlap.
    /// </summary>
    /// <param name="term">The other term</param>
    /// <returns>True if the types can be compared, false otherwise.</returns>
    public bool IsComparableTo(ITerm term)
    {
      return (this.GetTypeSet().IsComparableTo(term.GetTypeSet()));
    }

    #region IComparable<ObjectFluentApplication> Members

    /// <summary>
    /// Compares two object fluent applications.
    /// </summary>
    /// <param name="other">The other object fluent application to compare this formula to.</param>
    /// <returns>An integer representing the total order relation between the two formulas.</returns>
    public int CompareTo(ObjectFluentApplication other)
    {
      return this.CompareTo((IExp)other);
    }

    #endregion
  }
}
