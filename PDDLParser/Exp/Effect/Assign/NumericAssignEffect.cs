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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Formula;

namespace PDDLParser.Exp.Effect.Assign
{
  /// <summary>
  /// This class represents a numeric fluent assignment.
  /// </summary>
  public abstract class NumericAssignEffect : AssignEffect
  {
    /// <summary>
    /// Creates a new numeric fluent assignment.
    /// </summary>
    /// <param name="image">The image of this fluent assignment.</param>
    /// <param name="head">The numeric fluent to assign a value to.</param>
    /// <param name="body">The value to assign to the numeric fluent.</param>
    public NumericAssignEffect(string image, NumericFluentApplication head, INumericExp body)
      : base(image, head, body)
    {
      System.Diagnostics.Debug.Assert(head != null && body != null);
    }

    /// <summary>
    /// Gets the numeric fluent to assign a value to.
    /// </summary>
    protected NumericFluentApplication Head
    {
      get { return (NumericFluentApplication)this.m_head; }
    }

    /// <summary>
    /// Gets the numeric value to assign to the fluent.
    /// </summary>
    protected INumericExp Body
    {
      get { return (INumericExp)this.m_body; }
    }
  }
}
