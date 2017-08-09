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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp;

namespace PDDLParser.World
{
  /// <summary>
  /// Updatable durative open world supporting timed effects.
  /// </summary>
  public interface IDurativeOpenWorld : IReadOnlyDurativeOpenWorld, IOpenWorld
  {
    // TODO: Add ways to inhibit timed effects (as is doable in the original TLPlan)?

    /// <summary>
    /// Add an effect which will take place after a fixed duration.
    /// </summary>
    /// <param name="timeOffset">The relative time offset at which the effect takes place.</param>
    /// <param name="effect">The delayed effect.</param>
    void AddEndEffect(double timeOffset, IEffect effect);

    //void SignalConstraintPreferenceViolated(int nrConstraintPreference);
  }
}
