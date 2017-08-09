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
  /// A described formula application is an application of a described formula.
  /// </summary>
  public abstract class DescribedFormulaApplication : FormulaApplication
  {
    /// <summary>
    /// Whether the cycles detection mechanism should use this formula application.
    /// </summary>
    public bool DetectCycles
    {
      get { return ((DescribedFormula)this.m_rootFormula).DetectCycles; }
    }

    /// <summary>
    /// Returns whether this described formula application is invariant, i.e. if it is never 
    /// modified by actions.
    /// </summary>
    public bool Invariant
    {
      get { return ((DescribedFormula)this.m_rootFormula).Invariant; }
    }

    /// <summary>
    /// Creates a new described formula application of a specified described formula with a given
    /// list of arguments.
    /// </summary>
    /// <param name="describedFormula">The descrbed formula to instantiate.</param>
    /// <param name="arguments">The arguments of this formula application.</param>
    public DescribedFormulaApplication(DescribedFormula describedFormula, List<ITerm> arguments)
      : base(describedFormula, arguments)
    {
      System.Diagnostics.Debug.Assert(describedFormula != null && arguments != null && !arguments.ContainsNull());
    }
  }
}
