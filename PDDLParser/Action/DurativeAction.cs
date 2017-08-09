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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PDDLParser.Exp;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Action
{
  /// <summary>
  /// Represents a durative action of the PDDl language.
  /// </summary>
  public class DurativeAction : AbstractActionDef
  {
    #region Private Fields
    /// <summary>
    /// The duration constraint of the action.
    /// </summary>
    private ILogicalExp m_duration;

    /// <summary>
    /// The start condition of the durative action.
    /// </summary>
    private ILogicalExp m_startCondition;

    /// <summary>
    /// The over all condition of the durative action.
    /// </summary>
    private ILogicalExp m_overallCondition;

    /// <summary>
    /// The end condition of the durative action.
    /// </summary>
    private ILogicalExp m_endCondition;

    /// <summary>
    /// The start effect of the durative action.
    /// </summary>
    private IEffect m_startEffect;

    /// <summary>
    /// The end effect of the durative action.
    /// </summary>
    private IEffect m_endEffect;

    /// <summary>
    /// The continuous effect of the durative action.
    /// </summary>
    private IEffect m_continuousEffect;

    /// <summary>
    /// The overall effect of the durative action. This is not an effect that can be
    /// used by the user; it is rather created when using "over all" preferences.
    /// </summary>
    private IEffect m_overallEffect;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the duration constraint of the durative action.
    /// </summary>
    public ILogicalExp Duration
    {
      get { return this.m_duration; }
      set { this.m_duration = value; }
    }

    /// <summary>
    /// Gets or sets the start conditions of the durative action.
    /// </summary>
    public ILogicalExp StartCondition
    {
      get { return m_startCondition; }
      set { m_startCondition = value; }
    }

    /// <summary>
    /// Gets or sets the overall conditions of the durative action.
    /// </summary>
    public ILogicalExp OverallCondition
    {
      get { return m_overallCondition; }
      set { m_overallCondition = value; }
    }

    /// <summary>
    /// Gets or sets the end conditions of the durative action.
    /// </summary>
    public ILogicalExp EndCondition
    {
      get { return m_endCondition; }
      set { m_endCondition = value; }
    }

    /// <summary>
    /// Gets or sets the start effect of the durative action.
    /// </summary>
    public IEffect StartEffect
    {
      get { return m_startEffect; }
      set { m_startEffect = value; }
    }

    /// <summary>
    /// Gets or sets the end effect of the durative action.
    /// </summary>
    public IEffect EndEffect
    {
      get { return m_endEffect; }
      set { m_endEffect = value; }
    }

    /// <summary>
    /// Gets or sets the continuous effect of the durative action.
    /// </summary>
    public IEffect ContinuousEffect
    {
      get { return m_continuousEffect; }
      set { m_continuousEffect = value; }
    }

    /// <summary>
    /// Gets or sets the over all effect of the durative action. This is not an effect that can be
    /// used by the user; it is rather created when using "over all" preferences.
    /// </summary>
    public IEffect OverallEffect
    {
      get { return m_overallEffect; }
      set { m_overallEffect = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new durative action with a given name, priority, and parameter list.
    /// </summary>
    /// <param name="name">The durative action's name.</param>
    /// <param name="priority">The durative action's priority.</param>
    /// <param name="parameters">The durative action's parameters.</param>
    public DurativeAction(string name, double priority, List<ObjectParameterVariable> parameters)
      : base(name, priority, parameters)
    {
    }

    #endregion

    /// <summary>
    /// Returns all effects of the action, independently of their time of effect.
    /// </summary>
    /// <returns>All effects of the action.</returns>
    public override IEnumerable<IEffect> GetAllEffects()
    {
      return EnumerableExtensions.Enumerable(StartEffect, EndEffect, ContinuousEffect, OverallEffect);
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// action. Remember that free variables are existentially quantified.
    /// </summary>
    /// <returns>A standardized copy of this expression.</returns>
    public override IActionDef Standardize()
    {
      IDictionary<string, string> images = new Dictionary<string, string>();
      DurativeAction other = (DurativeAction)this.MemberwiseClone();
      other.m_parameters = new List<ObjectParameterVariable>();
      foreach (ObjectParameterVariable param in this.m_parameters)
      {
        other.m_parameters.Add((ObjectParameterVariable)param.Standardize(images));
      }
      other.m_duration = (ILogicalExp)this.m_duration.Standardize(images);

      other.m_startCondition = (ILogicalExp)this.m_startCondition.Standardize(images);
      other.m_overallCondition = (ILogicalExp)this.m_overallCondition.Standardize(images);
      other.m_endCondition = (ILogicalExp)this.m_endCondition.Standardize(images);

      other.m_startEffect = (IEffect)this.m_startEffect.Standardize(images);
      other.m_continuousEffect = (IEffect)this.m_continuousEffect.Standardize(images);
      other.m_overallEffect = (IEffect)this.m_overallEffect.Standardize(images);
      other.m_endEffect = (IEffect)this.m_endEffect.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns a deep copy of this abstract action.
    /// </summary>
    /// <returns>A deep copy of this abstract action.</returns>
    public override object Clone()
    {
      DurativeAction other = (DurativeAction)base.Clone();
      other.m_duration = (ILogicalExp)this.m_duration.Clone();

      other.m_startCondition = (ILogicalExp)this.m_startCondition.Clone();
      other.m_overallCondition = (ILogicalExp)this.m_overallCondition.Clone();
      other.m_endCondition = (ILogicalExp)this.m_endCondition.Clone();

      other.m_startEffect = (IEffect)this.m_startEffect.Clone();
      other.m_continuousEffect = (IEffect)this.m_continuousEffect.Clone();
      other.m_overallEffect = (IEffect)this.m_overallEffect.Clone();
      other.m_endEffect = (IEffect)this.m_endEffect.Clone();

      return other;
    }

    /// <summary>
    /// Returns a string representation of this action.
    /// </summary>
    /// <returns>A string representation of this action.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:durative-action ");
      str.Append(this.Name);
      str.Append(":parameters ");
      str.Append("(");
      str.Append(string.Join(" ", this.m_parameters.Select(var => var.ToString()).ToArray()));
      str.Append(")");
      str.Append("\n:duration ");
      str.Append(this.m_duration.ToString());
      str.Append("\n:condition (and ");
      str.Append("                (at start " + StartCondition.ToString() + ")\n");
      str.Append("                (over all " + OverallCondition.ToString() + ")\n");
      str.Append("                (at end " + EndCondition.ToString() + "))\n");
      str.Append("\n:effect (and ");
      str.Append("             (at start " + StartEffect.ToString() + ")\n");
      str.Append("             (at end " + EndEffect.ToString() + ")\n");
      str.Append("             " + ContinuousEffect.ToString() + "))\n");
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this action.
    /// </summary>
    /// <returns>A typed string representation of this action.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:durative-action ");
      str.Append(this.Name);
      str.Append(":parameters ");
      str.Append("(");
      str.Append(string.Join(" ", this.m_parameters.Select(var => var.ToTypedString()).ToArray()));
      str.Append(")");
      str.Append("\n:duration ");
      str.Append(this.m_duration.ToTypedString());
      str.Append("\n:condition (and ");
      str.Append("                (at start " + StartCondition.ToTypedString() + ")\n");
      str.Append("                (over all " + OverallCondition.ToTypedString() + ")\n");
      str.Append("                (at end " + EndCondition.ToTypedString() + "))\n");
      str.Append("\n:effect (and ");
      str.Append("             (at start " + StartEffect.ToTypedString() + ")\n");
      str.Append("             (at end " + EndEffect.ToTypedString() + ")\n");
      str.Append("             " + ContinuousEffect.ToTypedString() + ")\n");
      str.Append(")");
      return str.ToString();
    }
  }
}
