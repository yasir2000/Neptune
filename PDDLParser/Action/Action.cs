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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp;
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Action
{
  /// <summary>
  /// Represents a STRIPS-like action of the PDDL language (i.e. one with no duration).
  /// </summary>
  public class Action : AbstractActionDef
  {
    #region Private Fields

    /// <summary>
    /// The precondition of the action.
    /// </summary>
    private ILogicalExp m_precondition;

    /// <summary>
    /// The effect of the action.
    /// </summary>
    private IEffect m_effect;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the precondition of this action.
    /// </summary>
    public ILogicalExp Precondition
    {
      get { return this.m_precondition; }
      set { this.m_precondition = value; }
    }
    
    /// <summary>
    /// Gets or sets the effect of this action.
    /// </summary>
    public IEffect Effect
    {
      get { return this.m_effect; }
      set { this.m_effect = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new STRIP-like action with a given name, priority, and parameter list.
    /// </summary>
    /// <param name="name">The name of the action.</param>
    /// <param name="priority">The priority of the action.</param>
    /// <param name="parameters">The parameters of the action.</param>
    public Action(string name, double priority, List<ObjectParameterVariable> parameters)
      : base(name, priority, parameters)
    {
      this.m_precondition = TrueExp.True;
      this.m_effect = new AndEffect();
    }

    #endregion

    #region ActionDef Interface

    /// <summary>
    /// Returns the effect of the action.
    /// Note: In this case, calling this will return an enumerable with only one argument.
    /// </summary>
    /// <returns>The effect of the action.</returns>
    public override IEnumerable<IEffect> GetAllEffects()
    {
      return Effect.Once();
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// action. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    public override IActionDef Standardize()
    {
      IDictionary<string, string> images = new Dictionary<string, string>();
      Action other = (Action)this.MemberwiseClone();
      other.m_parameters = new List<ObjectParameterVariable>();
      foreach (ObjectParameterVariable param in this.m_parameters)
      {
        other.m_parameters.Add((ObjectParameterVariable)param.Standardize(images));
      }
      other.m_precondition = (ILogicalExp)this.m_precondition.Standardize(images);
      other.m_effect = (IEffect)this.m_effect.Standardize(images);
      return other;
    }

    /// <summary>
    /// Returns a typed string representation of this action.
    /// </summary>
    /// <returns>A typed string representation of this action.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:action ");
      str.Append(this.Name.ToString());
      str.Append("\n");
      str.Append("   :parameters ");
      str.Append("(");
      str.Append(string.Join(" ", this.m_parameters.Select(var => var.ToTypedString()).ToArray()));
      str.Append(")");
      str.Append("\n   :precondition ");
      str.Append(this.m_precondition.ToString());
      str.Append("\n   :effect ");
      str.Append(this.m_effect.ToString());
      str.Append(")");
      return str.ToString();
    }

    #endregion

    #region ICloneable Interface

    /// <summary>
    /// Returns a deep copy of this abstract action.
    /// </summary>
    /// <returns>A deep copy of this abstract action.</returns>
    public override object Clone()
    {
      Action other = (Action)base.Clone();
      other.m_precondition = (ILogicalExp)this.m_precondition.Clone();
      other.m_effect = (IEffect)this.m_effect.Clone();
      return other;
    }

    #endregion

    #region Object Interface overrides

    /// <summary>
    /// Returns a string representation of this action.
    /// </summary>
    /// <returns>A string representation of this action.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:action ");
      str.Append(this.Name.ToString());
      str.Append("\n");
      str.Append("   :parameters ");
      str.Append("(");
      str.Append(string.Join(" ", this.m_parameters.Select(var => var.ToString()).ToArray()));
      str.Append(")");
      str.Append("\n   :precondition ");
      str.Append(this.m_precondition.ToString());
      str.Append("\n   :effect ");
      str.Append(this.m_effect.ToString());
      str.Append(")");
      return str.ToString();
    }

    #endregion
  }
}
