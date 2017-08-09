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
// Please note that this file was inspired in part by the PDDL4J library:
// http://www.math-info.univ-paris5.fr/~pellier/software/software.php 
//
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using Type = PDDLParser.Exp.Term.Type.Type;

namespace PDDLParser.Exception
{
  /// <summary>
  /// A typing exception is thrown when an error involving types occurs, for example if a cycle
  /// is found in the cycle hierarchy.
  /// </summary>
  public class TypingException : System.Exception
  {
    /// <summary>
    /// The type that caused the exception.
    /// </summary>
    private Type m_type;
    
    /// <summary>
    /// Creates a new typing exception involving the specified type.
    /// </summary>
    /// <param name="type">The type which caused the exception.</param>
    public TypingException(Type type)
    {
      this.m_type = type;
    }

    /// <summary>
    /// The message of this exception.
    /// </summary>
    public override string Message
    {
      get
      {
        return "The type \"" + this.m_type.Name + "\" has caused an exception.";
      }
    }
  }
}