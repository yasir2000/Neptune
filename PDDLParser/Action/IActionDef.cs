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
using System.Collections;
using System.Collections.Generic;
using PDDLParser.Exp;
using PDDLParser.Exp.Term;

namespace PDDLParser.Action
{
  /// <summary>
  /// This interface is implemented by all types of action defined in the PDDL language.
  /// </summary>
  public interface IActionDef : IEnumerable<ObjectParameterVariable>, ICloneable
  {
    /// <summary>
    /// Returns the name of the action.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns the priority of the action. A higher priority means the action will be chosen
    /// first. Defining priorities is possible only in TLPlan domains.
    /// </summary>
    [TLPlan]
    double Priority { get; }

    /// <summary>
    /// Returns all effects of the action, independently of their time of effect.
    /// </summary>
    /// <returns>All effects of the action.</returns>
    IEnumerable<IEffect> GetAllEffects();

    /// <summary>
    /// Returns the parameters of the action.
    /// </summary>
    /// <returns>The parameters of the action.</returns>
    IEnumerable<ObjectParameterVariable> GetParameters();

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// action. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    IActionDef Standardize();

    /// <summary>
    /// Returns a typed string representation of this action.
    /// </summary>
    /// <returns>A typed string representation of this action.</returns>
    string ToTypedString();
  }
}