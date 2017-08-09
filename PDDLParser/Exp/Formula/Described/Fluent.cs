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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A fluent represents a described formula associated with a numeric or object value.
  /// </summary>
  public abstract class Fluent : DescribedFormula
  {
    /// <summary>
    /// Creates a new fluent with the specified name and arguments.
    /// </summary>
    /// <param name="name">The name of the new fluent.</param>
    /// <param name="arguments">The arguments of the new fluent.</param>
    /// <param name="attributes">The new fluent's attributes.</param>
    public Fluent(string name, List<ObjectParameterVariable> arguments, Attributes attributes)
      : base(name, arguments, attributes)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());
    }
  }
}