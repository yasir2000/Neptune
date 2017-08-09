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
// Please note that this file was inspired in part by the PDDL4J library:
// http://www.math-info.univ-paris5.fr/~pellier/software/software.php 
//
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// An atomic formula is a boolean described formula (a described predicate).
  /// </summary>
  public class AtomicFormula : DescribedFormula
  {
    /// <summary>
    /// Creates a new atomic formula with the specified name and arguments.
    /// </summary>
    /// <param name="name">The name of this atomic formula.</param>
    /// <param name="arguments">The arguments of this atomic formula.</param>
    /// <param name="attributes">The new atomic formula's attributes.</param>
    public AtomicFormula(string name, List<ObjectParameterVariable> arguments, Attributes attributes)
      : base(name, arguments, attributes)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());
    }

    /// <summary>
    /// Instantiates a formula application associated with this atomic formula.
    /// </summary>
    /// <param name="arguments">Arguments of the formula application to instantiate.</param>
    /// <returns>A new atomic formula application associated with this atomic formula.</returns>
    public override FormulaApplication Instantiate(List<ITerm> arguments)
    {
      return new AtomicFormulaApplication(this, arguments);
    }
  }
}