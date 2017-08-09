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
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect
{
  /// <summary>
  /// This class represents a conjunction of effects.
  /// </summary>
  public class AndEffect : AbstractAndExp<IEffect>, IEffect
  {
    /// <summary>
    /// Creates a new conjunction of effects.
    /// </summary>
    /// <param name="effects">The effects associated with the conjunctive expression.</param>
    public AndEffect(IEnumerable<IEffect> effects)
      : base(effects)
    {
      System.Diagnostics.Debug.Assert(!effects.ContainsNull());
    }

    /// <summary>
    /// Creates a new conjunction of effects.
    /// </summary>
    /// <param name="effects">The effects associated with the conjunctive expression.</param>
    public AndEffect(params IEffect[] effects)
      : this((IEnumerable<IEffect>)effects)
    {
      System.Diagnostics.Debug.Assert(!effects.ContainsNull());
    }

    /// <summary>
    /// Updates the specified world with this effect.
    /// A conjunction of effects sequentially updates the world with each internal effect.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                       LocalBindings bindings, ActionContext actionContext)
    {
      foreach (IEffect effect in this.m_expressions)
      {
        effect.Update(evaluationWorld, updateWorld, bindings, actionContext);
      }
    }

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      HashSet<DescribedFormula> formulas = new HashSet<DescribedFormula>();
      foreach (IEffect effect in this.m_expressions)
      {
        formulas.UnionWith(effect.GetModifiedDescribedFormulas());
      }
      return formulas;
    }
  }
}
