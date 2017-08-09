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

namespace PDDLParser.Exp
{
  /// <summary>
  /// Base class for quantified ForAll expressions.
  /// </summary>
  public abstract class AbstractForallExp<T> : QuantifiedExp<T>
    where T : class, IExp
  {
    /// <summary>
    /// Creates a new ForAll expression with the specified vars and body.
    /// </summary>
    /// <param name="vars">The quantified variables.</param>
    /// <param name="body">The quantified expression's body.</param>
    public AbstractForallExp(HashSet<ObjectParameterVariable> vars, T body)
      : base("forall", vars, body)
    {
      System.Diagnostics.Debug.Assert(body != null && vars != null && !vars.ContainsNull());
    }
  }
}
