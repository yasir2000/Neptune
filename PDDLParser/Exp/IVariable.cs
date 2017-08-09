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

namespace PDDLParser.Exp
{
  /// <summary>
  /// A variable is an evaluable expression whose value may change.
  /// </summary>
  public interface IVariable : IEvaluableExp, IComparable<IVariable>
  {
    /// <summary>
    /// Returns the name of this variable.
    /// </summary>
    /// <returns>The name of this variable.</returns>
    string Name { get; }
  }
}
