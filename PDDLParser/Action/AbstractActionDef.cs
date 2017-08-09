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

using System.Collections.Generic;
using System.Collections;
using System;
using PDDLParser.Exp;
using PDDLParser.Exp.Term;

namespace PDDLParser.Action
{
  /// <summary>
  /// Represents the common structures and methods shared by all PDDL actions.
  /// </summary>
  public abstract class AbstractActionDef : IActionDef
  {
    /// <summary>
    /// The name of the action.
    /// </summary>
    protected readonly string m_name;

    /// <summary>
    /// The priority of the action.
    /// </summary>
    protected readonly double m_priority;

    /// <summary>
    /// The parameters of the action.
    /// </summary>
    protected List<ObjectParameterVariable> m_parameters;

    /// <summary>
    /// Creates an instance of action with the given name, priority, and parameters.
    /// </summary>
    /// <param name="name">The name of the action.</param>
    /// <param name="priority">The priority of the action.</param>
    /// <param name="parameters">The parameters of the action.</param>
    protected AbstractActionDef(string name, double priority, List<ObjectParameterVariable> parameters)
    {
      this.m_name = name;
      this.m_priority = priority;
      this.m_parameters = parameters;
    }

    /// <summary>
    /// Returns the name of the action.
    /// </summary>
    public string Name
    {
      get { return this.m_name; }
    }

    /// <summary>
    /// Returns the priority of the action. A higher priority means the action will be chosen
    /// first. Defining priorities is possible only in TLPlan domains.
    /// </summary>
    [TLPlan]
    public double Priority
    {
      get { return this.m_priority; }
    }

    /// <summary>
    /// Returns the parameters of the action.
    /// </summary>
    /// <returns>The parameters of the action.</returns>
    public IEnumerable<ObjectParameterVariable> GetParameters()
    {
      return this.m_parameters;
    }

    /// <summary>
    /// Returns an enumerator over the parameters of this action.
    /// </summary>
    /// <returns>An enumerator over the parameters of this action.</returns>
    public IEnumerator<ObjectParameterVariable> GetEnumerator()
    {
      return this.m_parameters.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator over the parameters of this action.
    /// </summary>
    /// <returns>An enumerator over the parameters of this action.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Returns a deep copy of this abstract action.
    /// </summary>
    /// <returns>A deep copy of this abstract action.</returns>
    public virtual object Clone()
    {
      AbstractActionDef other = (AbstractActionDef)base.MemberwiseClone();
      other.m_parameters = new List<ObjectParameterVariable>();
      foreach (ObjectParameterVariable param in this.m_parameters)
      {
        other.m_parameters.Add((ObjectParameterVariable)param.Clone());
      }
      return other;
    }

    /// <summary>
    /// Returns all effects of the action, independently of their time of effect.
    /// </summary>
    /// <returns>All effects of the action.</returns>
    public abstract IEnumerable<IEffect> GetAllEffects();

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// action. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    public abstract IActionDef Standardize();

    /// <summary>
    /// Returns a typed string representation of this action.
    /// </summary>
    /// <returns>A typed string representation of this action.</returns>
    public abstract string ToTypedString();
  }
}