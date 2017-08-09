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
using PDDLParser;
using PDDLParser.World;
using TLPlan.Utils;

namespace TLPlan.World
{
  /// <summary>
  /// An invariant world is a read-only open world containing information about invariants
  /// facts and fluents. It knows nothing except the value of these invariant formulas, which
  /// can be represented as intervals (of formulas ID).
  /// Hence when an invariant world is queried for the value of an invariant formula
  /// (one whose ID belongs to the interval), it cannot return unknown.
  /// </summary>
  public interface IInvariantWorld : IReadOnlyOpenWorld
  {
    /// <summary>
    /// The interval of invariant predicates IDs whose value are known.
    /// </summary>
    IntegerInterval InvariantPredicateInterval { get; }

    /// <summary>
    /// The interval of invariant numeric fluent IDs whose value are known.
    /// </summary>
    IntegerInterval InvariantNumericFluentInterval { get; }

    /// <summary>
    /// The interval of invariant object fluent IDs whose value are known.
    /// </summary>
    IntegerInterval InvariantObjectFluentInterval { get; }
  }
}
