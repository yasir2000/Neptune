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
using System.Runtime.Serialization;

using PDDLParser.Extensions;

namespace PDDLParser.Exp.Metric
{

  /// <summary>
  /// Represents the common part of all preference expressions of the PDDL language.
  /// </summary>
  public abstract class AbstractPrefExp : AbstractExp, IPrefExp
  {
    #region Private Fields

    /// <summary>
    /// The name of this preference.
    /// </summary>
    protected string m_name;

    /// <summary>
    /// The expression that this preference could violate.
    /// </summary>
    protected IExp m_exp;

    /// <summary>
    /// Whether this preference is unnamed.
    /// </summary>
    protected bool m_unnamed;

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether this preference is unnamed.
    /// </summary>
    /// <remarks>
    /// Even if a preference is unnamed, it still has a name. This name, however,
    /// is not parsable; therefore, a user cannot produce a preference with the same
    /// name as an unnamed preference.
    /// </remarks>
    public bool Unnamed { get { return m_unnamed; } }

    /// <summary>
    /// Returns the name of this preference.
    /// </summary>
    /// <seealso cref="Unnamed"/>
    public string Name
    {
      get { return m_name; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new preference expression.
    /// </summary>
    /// <param name="name">The name of this preference.</param>
    /// <param name="exp">The preference's body.</param>
    /// <param name="unnamed">Whether this preference is unnamed.</param>
    public AbstractPrefExp(string name, IExp exp, bool unnamed)
      : base()
    {
      System.Diagnostics.Debug.Assert(name    != null);
      System.Diagnostics.Debug.Assert(exp     != null);

      this.m_name = name;
      this.m_exp = exp;
      this.m_unnamed = unnamed;
    }

    /// <summary>
    /// Creates a new, named preference expression.
    /// </summary>
    /// <param name="name">The name of this preference.</param>
    /// <param name="exp">The preference's body.</param>
    public AbstractPrefExp(string name, IExp exp)
      : this(name, exp, false)
    { }

    #endregion
    
    /// <summary>
    /// Returns the original expression that this preference could violate.
    /// </summary>
    /// <returns>The original expression that this preference could violate.</returns>
    public IExp GetOriginalExp()
    {
      return m_exp;
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// preference by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this preference.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AbstractPrefExp other = (AbstractPrefExp)base.Apply(bindings);
      other.m_exp = (ILogicalExp)this.m_exp.Apply(bindings);

      return other;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// preference. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this preference.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      AbstractPrefExp other = (AbstractPrefExp)base.Standardize(images);
      other.m_exp = (ILogicalExp)this.m_exp.Standardize(images);

      return other;
    }

    /// <summary>
    /// Returns true if the preference is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the preference is ground.</returns>
    public override bool IsGround()
    {
      return this.m_exp.IsGround();
    }

    /// <summary>
    /// Returns true if this preference is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this preference is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        AbstractPrefExp other = (AbstractPrefExp)obj;
        return this.m_name.Equals(other.m_name)
            && this.m_exp.Equals(other.m_exp);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this preference.
    /// </summary>
    /// <returns>The hash code of this preference.</returns>
    public override int GetHashCode()
    {
      return this.m_name.GetHashCode()
           + this.m_exp.GetHashCode();
    }

    /// <summary>
    /// Returns the free variables in this preference.
    /// </summary>
    /// <returns>The free variables in this preference.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return this.m_exp.GetFreeVariables();
    }

    /// <summary>
    /// Returns a string representation of this preference.
    /// </summary>
    /// <returns>A string representation of this preference.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(preference ");
      str.Append(this.m_name.ToString());
      str.Append(" ");
      str.Append(this.m_exp.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string of this preference.
    /// </summary>
    /// <returns>A typed string representation of this preference.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(preference ");
      str.Append(this.m_name.ToString());
      str.Append(" ");
      str.Append(this.m_exp.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this preference with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this preference to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.</returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      AbstractPrefExp otherExp = (AbstractPrefExp)other;

      value = this.m_name.CompareTo(otherExp.m_name);
      if (value != 0)
        return value;

      return this.m_exp.CompareTo(otherExp.m_exp);
    }

    #endregion
  }
}
