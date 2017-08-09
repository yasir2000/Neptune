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
using PDDLParser.Exp.Formula;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp
{
  /// <summary>
  /// An effect represents an update (or set of updates) applicable to a world.
  /// </summary>
  public interface IEffect : IExp
  {
    /// <summary>
    /// Updates the specified world with this effect.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against. Note that this is 
    /// usually the un-modified version of the world to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                LocalBindings bindings, ActionContext actionContext);

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    HashSet<DescribedFormula> GetModifiedDescribedFormulas();
  }
}
