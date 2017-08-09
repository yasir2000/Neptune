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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDDLParser.Exp.Constraint
{
  /// <summary>
  /// Represents the "within" constraint expression of the PDDL language.
  /// This constraint expression uses absolute timestamps.
  /// </summary>
  /// <remarks>
  /// This object should only be created by <see cref="WithinExp"/> constraint expressions.
  /// </remarks>
  /// <seealso cref="WithinExp"/>
  public class AbsoluteWithinExp : WithinExp
  {
    #region Properties

    /// <summary>
    /// Gets the absolute timestamp before which the body must be true.
    /// </summary>
    protected double AbsoluteTimestamp
    {
      // The relative timestamp of the parent is actually an absolute timestamp! :O
      get { return this.RelativeTimestamp; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new "within" constraint expression with an absolute timestamp.
    /// </summary>
    /// <param name="exp">The body of the expression.</param>
    /// <param name="absoluteTimestamp">The absolute timestamp within which the body must be true.</param>
    public AbsoluteWithinExp(IConstraintExp exp, double absoluteTimestamp)
      : base(exp, absoluteTimestamp)
    {
    }

    #endregion

    #region Within Interface Overrides

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    /// <exception cref="BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    /// <seealso cref="IConstraintExp.progress"/>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, ILocalBindings bindings)
    {
      return new ProgressionValue(world.GetTotalTime() < this.AbsoluteTimestamp) &&
             (Exp.Progress(world, bindings) || new ProgressionValue(this, this.AbsoluteTimestamp));
    }

    #endregion
  }
}
