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
using PDDLParser.Extensions;

namespace PDDLParser.Exp
{
  /// <summary>
  /// Base class for conjunctive expressions.
  /// </summary>
  public class AbstractAndExp<T> : ListExp<T>
    where T : class,IExp
  {
    /// <summary>
    /// Creates a new And expression with the specified arguments.
    /// </summary>
    /// <param name="exps">The arguments of this conjunctive expression.</param>
    public AbstractAndExp(IEnumerable<T> exps)
      : base("and", exps)
    {
      System.Diagnostics.Debug.Assert(exps != null && !exps.ContainsNull());
    }

    /// <summary>
    /// Adds a new expression in this conjunctive expression. Nested conjunctive 
    /// expressions are added to the top-level expression.
    /// </summary>
    /// <param name="elt">The expression to add.</param>
    protected override void AddElement(T elt)
    {
      if (elt.GetType() == this.GetType())
      {
        ListExp<T> listExp = (AbstractAndExp<T>)(IExp)elt;
        foreach (T exp in listExp)
        {
          base.AddElement(exp);
        }
      }
      else
      {
        base.AddElement(elt);
      }
    }
  }
}
