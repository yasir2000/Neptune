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

namespace PDDLParser.World
{
  /// <summary>
  /// Constant durative open world supporting timed read-only operations.
  /// </summary>
  public interface IReadOnlyDurativeOpenWorld : IReadOnlyOpenWorld
  {
    /// <summary>
    /// Returns the total time it took to reach the current world.
    /// </summary>
    /// <returns>The total time of the plan up to this point.</returns>
    double GetTotalTime();

    /// <summary>
    /// Returns whether the world is an idle goal world (whether it satisfies a goal
    /// and has been idled).
    /// </summary>
    /// <returns>True if the world is an idle goal world, false otherwise.</returns>
    bool IsIdleGoalWorld();

    //bool IsConstraintPreferenceViolated(int nrConstraintPreference);
  }
}
