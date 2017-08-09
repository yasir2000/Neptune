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
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A fluent application is an application of a fluent.
  /// </summary>
  public abstract class FluentApplication : DescribedFormulaApplication
  {
    /// <summary>
    /// Creates a new fluent application of a specified fluent with a given list
    /// of arguments.
    /// </summary>
    /// <param name="fluent">The fluent to instantiate.</param>
    /// <param name="arguments">The arguments of this fluent application.</param>
    public FluentApplication(Fluent fluent, List<ITerm> arguments)
      : base(fluent, arguments)
    {
      System.Diagnostics.Debug.Assert(fluent != null && arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Gets the fluent associated with this fluent application.
    /// </summary>
    public Fluent RootFluent
    {
      get { return (Fluent)this.m_rootFormula; }
    }
  }
}
