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
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Numeric
{
  /// <summary>
  /// Base class for numeric expressions.
  /// </summary>
  public abstract class AbstractNumericExp : AbstractExp, INumericExp
  {
    /// <summary>
    /// Creates a new numeric expression.
    /// </summary>
    public AbstractNumericExp()
      : base()
    {
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the resulting numeric value.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public abstract FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Simplifies this numeric expression by evaluating its known expression parts.
    /// The bindings should not be modified by this call.
    /// The resulting expression should not contain any unbound variables, since
    /// they are substituted according to the bindings supplied.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, a numeric value, or the simplified expression.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public abstract NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this numeric expression in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the resulting numeric value.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <exception cref="PDDLParser.Exception.NumericException">A NumericException is thrown if an 
    /// illegal operation is performed (like a division by zero).</exception>
    public abstract Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings);
  }
}
