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

namespace PDDLParser.Exp.Formula.TLPlan
{
  /// <summary>
  /// A defined object function application is an application of a defined object function.
  /// </summary>
  [TLPlan]
  public class DefinedObjectFunctionApplication : DefinedFormulaApplication, ITerm
  {
    /// <summary>
    /// Creates a new defined object function application of a specified defined object function
    /// with a given list of arguments.
    /// </summary>
    /// <param name="objectFunction">The defined object function to instantiate.</param>
    /// <param name="arguments">The arguments of this new defined object function application.</param>
    public DefinedObjectFunctionApplication(DefinedObjectFunction objectFunction, List<ITerm> arguments)
      : base(objectFunction, arguments)
    {
      System.Diagnostics.Debug.Assert(objectFunction != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Returns the defined object function corresponding to this function application.
    /// </summary>
    /// <returns>The defined object function corresponding to this function application.</returns>
    public DefinedObjectFunction getDefinedObjectFunction()
    {
      return (DefinedObjectFunction)this.m_rootFormula;
    }

    /// <summary>
    /// Returns a copy of this formula application with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments of the new formula application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public override FormulaApplication Apply(List<ITerm> arguments)
    {
      return new DefinedObjectFunctionApplication(this.getDefinedObjectFunction(), arguments);
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// An object function application is evaluated by first evaluating its arguments 
    /// and then evaluating the corresponding defined function's body with the appropriate bindings.
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
          return ((DefinedObjectFunction)this.m_rootFormula).Evaluate(world, (DefinedObjectFunctionApplication)result.Value);
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
    /// An object function application is simplified by first simplifying its arguments
    /// and then by attempting to evaluate the corresponding defined function's body (if all
    /// its arguments are ground).
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
          DefinedObjectFunctionApplication formula = (DefinedObjectFunctionApplication)result.Value;
          if (!formula.AllConstantArguments)
          {
            return new TermValue(formula);
          }
          else
          {
            FuzzyConstantExp value = ((DefinedObjectFunction)this.m_rootFormula).Evaluate(world, formula);
            if (value.Status == FuzzyConstantExp.State.Unknown)
            {
              return new TermValue(formula);
            }
            else
            {
              return new TermValue(value.ToConstantValue());
            }
          }
        case ArgsEvalResult.State.Undefined:
          return TermValue.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// An object function application is evaluated by first evaluating its arguments 
    /// and then evaluating the corresponding defined function's body with the appropriate bindings.
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
          return ((DefinedObjectFunction)this.m_rootFormula).Evaluate(world, (DefinedObjectFunctionApplication)result.Value);
        case ArgsEvalResult.State.Undefined:
          return ConstantExp.Undefined;
        default:
          throw new System.Exception("Invalid EvalStatus status: " + result.Status);
      }
    }

    /// <summary>
    /// Return the typeset of the value returned by this object function application.
    /// </summary>
    /// <returns>The typeset of the value returned by this object function
    /// application.</returns>
    public TypeSet GetTypeSet()
    {
      return ((DefinedObjectFunction)this.m_rootFormula).GetTypeSet();
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
  }
}
