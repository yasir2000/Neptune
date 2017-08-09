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
using PDDLParser.Exp;

namespace PDDLParser.Exception
{
  /// <summary>
  /// A BindingException is thrown if an attempt is made to set to retrieve the value of
  /// an unbound variable.
  /// </summary>
  public class BindingException : System.Exception, ISerializable
  {
    /// <summary>
    /// The variable which caused the exception.
    /// </summary>
    private Variable m_cause;

    /// <summary>
    /// Creates a new BindingException with a specific cause.
    /// </summary>
    /// <param name="cause">The expression that caused the exception.</param>
    public BindingException(Variable cause) 
      : base() 
    {
      this.m_cause = cause;
    }

    /// <summary>
    /// The exception's message.
    /// </summary>
    public override string Message
    {
      get
      {
        return "Unbound variable " + this.m_cause.ToTypedString();
      }
    }
  }
}
