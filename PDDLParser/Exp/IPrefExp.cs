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

namespace PDDLParser.Exp
{
  /// <summary>
  /// Represents the basic interface of preferences. Note that these are not evaluable.
  /// </summary>
  public interface IPrefExp : IExp
  {
    /// <summary>
    /// Returns whether the preference is unnamed.
    /// </summary>
    /// <remarks>
    /// An unnamed preference still has a name; however, this name is unique and not parsable,
    /// hence there will be no conflicts with other preference names.
    /// </remarks>
    bool Unnamed { get; }

    /// <summary>
    /// Returns the name of the preferences.
    /// </summary>
    /// <remarks>
    /// This may be an unparsable name if the preference is unnamed.
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Returns the body of the preference.
    /// </summary>
    /// <returns>The body of the preference.</returns>
    IExp GetOriginalExp();
  }
}
