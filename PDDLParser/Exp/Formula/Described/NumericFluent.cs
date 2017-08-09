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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A numeric fluent is a described formula which is associated with a numeric value.
  /// </summary>
  public class NumericFluent : Fluent
  {
    /// <summary>
    /// Creates a new numeric fluent with the specified name and arguments.
    /// </summary>
    /// <param name="name">The name of the new numeric fluent.</param>
    /// <param name="arguments">The arguments of the new numeric fluent.</param>
    /// <param name="attributes">The new described formula's attributes.</param>
    public NumericFluent(string name, List<ObjectParameterVariable> arguments, Attributes attributes)
      : base(name, arguments, attributes)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Instantiates a formula application associated with this numeric fluent.
    /// </summary>
    /// <param name="arguments">Arguments of the formula application to instantiate.</param>
    /// <returns>A new numeric fluent application associated with this numeric fluent.</returns>
    public override FormulaApplication Instantiate(List<ITerm> arguments)
    {
      return new NumericFluentApplication(this, arguments);
    }

    /// <summary>
    /// Returns a typed string representation of this formula.
    /// </summary>
    /// <returns>A typed string representation of this formula.</returns>
    public override string ToTypedString()
    {
      return base.ToTypedString() + " - Number";
    }
  }
}
