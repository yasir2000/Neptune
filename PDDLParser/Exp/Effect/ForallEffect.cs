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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World;
using PDDLParser.World.Context;

namespace PDDLParser.Exp.Effect
{
  /// <summary>
  /// This class represents a universally quantified effect.
  /// </summary>
  public class ForallEffect : AbstractForallExp<IEffect>, IEffect
  {
    /// <summary>
    /// Creates a new universally quantified effect.
    /// </summary>
    /// <param name="vars">The quantified variables.</param>
    /// <param name="body">The body of the quantified effect.</param>
    public ForallEffect(HashSet<ObjectParameterVariable> vars, IEffect body)
      : base(vars, body)
    {
      System.Diagnostics.Debug.Assert(body != null && vars != null && !vars.ContainsNull());
    }
    
    /// <summary>
    /// Creates an expression equivalent to this universally quantified effect.
    /// </summary>
    /// <returns>An expression equivalent to this universally quantified effect.</returns>
    protected override IEffect GenerateEquivalentExp()
    {
      return new AndEffect(GetBodySubstitutions());
    }

    /// <summary>
    /// Updates the specified world with this effect.
    /// A universal effect updates the world for all substitutions of its quantified variables.
    /// </summary>
    /// <param name="evaluationWorld">The world to evaluate conditions against.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <param name="actionContext">The action evaluation context.</param>
    public void Update(IReadOnlyOpenWorld evaluationWorld, IDurativeOpenWorld updateWorld, 
                       LocalBindings bindings, ActionContext actionContext)
    {
      this.GetEquivalentExp().Update(evaluationWorld, updateWorld, bindings, actionContext);
    }

    /// <summary>
    /// Retrieves all the described formulas modified by this effect.
    /// </summary>
    /// <returns>All the described formulas modified by this effect.</returns>
    public HashSet<DescribedFormula> GetModifiedDescribedFormulas()
    {
      return this.GetEquivalentExp().GetModifiedDescribedFormulas();
    }
  }
}
