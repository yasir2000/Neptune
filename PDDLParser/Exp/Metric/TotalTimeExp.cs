﻿//
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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Metric
{
  /// <summary>
  /// Represents the "total-time" expression of the PDDL language.
  /// </summary>
  public class TotalTimeExp : ReservedNumericExp
  {
    /// <summary>
    /// Creates a new "total-time" expression.
    /// </summary>
    public TotalTimeExp()
      : base("total-time")
    { }

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
    public override FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      IReadOnlyDurativeOpenWorld durativeWorld = (IReadOnlyDurativeOpenWorld)world;
      return new FuzzyDouble(durativeWorld.GetTotalTime());
    }

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
    public override Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      IReadOnlyDurativeOpenWorld durativeWorld = (IReadOnlyDurativeOpenWorld)world;
      return new Double(durativeWorld.GetTotalTime());
    }
  }
}
