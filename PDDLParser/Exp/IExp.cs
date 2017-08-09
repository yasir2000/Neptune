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
  /// This interface is implemented by all expressions of the PDDL langage.
  /// </summary>
  public interface IExp : ICloneable, IComparable<IExp>
  {   
    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    IExp Apply(ParameterBindings bindings);
       
    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    IExp Standardize();
    
    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    IExp Standardize(IDictionary<string, string> images);

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    bool IsGround();

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    HashSet<Variable> GetFreeVariables();

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    string ToTypedString();
  }
}
