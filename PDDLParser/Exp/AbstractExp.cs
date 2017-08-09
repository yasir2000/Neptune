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
using System.Runtime.Serialization;

namespace PDDLParser.Exp
{
  /// <summary>
  /// This abstract base class represents the common part of expressions.
  /// The IComparable interface denotes a total order relation between expressions.
  /// </summary>
  public abstract class AbstractExp: IExp, IComparable<IExp>
  {
    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// The base implementation is to simply clone the object.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public virtual IExp Apply(ParameterBindings bindings)
    {
      return (IExp)this.Clone();
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    public virtual IExp Standardize()
    {
      return this.Standardize(new Dictionary<string, string>());
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// The base implementation is to simply clone the object.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public virtual IExp Standardize(IDictionary<string, string> images)
    {
      return (IExp)this.Clone();
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public abstract bool IsGround();

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public abstract HashSet<Variable> GetFreeVariables();

    /// <summary>
    /// Returns a clone of this expression.
    /// The default implementation simply returns a memberwise clone.
    /// </summary>
    /// <returns>A clone of this expression.</returns>
    public virtual object Clone()
    {
      return this.MemberwiseClone();
    }

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public override abstract bool Equals(object obj);

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public override abstract int GetHashCode();

    /// <summary>
    /// Returns a typed string of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public abstract string ToTypedString();

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override abstract string ToString();

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this abstract expression with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this abstract expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public virtual int CompareTo(IExp other)
    {
      if (this == other)
      {
        return 0;
      }
      else
      {
        // Comparison with the hash code seems to yield poor results.
        //int value = this.GetHashCode().CompareTo(other.GetHashCode());
        //if (value != 0)
        //  return value;
        return this.GetType().GUID.CompareTo(other.GetType().GUID);
      }
    }

    #endregion

  }
}