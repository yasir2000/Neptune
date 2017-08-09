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
using System.Runtime.Serialization;

namespace PDDLParser.Exception
{
  /// <summary>
  /// An UndefinedExpException is thrown if an attempt is made to set to retrieve the value
  /// of an undefined expression.
  /// </summary>
  public class UndefinedExpException : System.Exception, ISerializable
  {
    /// <summary>
    /// Creates a new UndefinedExpException with a specific message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public UndefinedExpException(string message) 
      : base(message)
    { 
    }
  }
}
