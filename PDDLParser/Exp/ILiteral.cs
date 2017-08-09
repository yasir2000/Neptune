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
using PDDLParser.Exp.Formula;

namespace PDDLParser.Exp
{
  /// <summary>
  /// A literal is an atomic formula or the negation of an atomic formula.
  /// </summary>
  public interface ILiteral: IEffect, IInitEl
  {
    /// <summary>
    /// Gets the predicate associated with this literal.
    /// </summary>
    AtomicFormula Predicate { get; }
  }
}