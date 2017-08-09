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
using PDDLParser.Exception;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;

namespace PDDLParser.Exp
{
  /// <summary>
  /// This class represents a set of (object) parameter variable bindings.
  /// </summary>
  public class ParameterBindings : ICloneable
  {
    /// <summary>
    /// The set of parameter bindings. Note that parameter variables cannot be bound 
    /// to an undefined value.
    /// </summary>
    protected IDictionary<ObjectParameterVariable, Constant> m_parameterBindings;

    /// <summary>
    /// Creates a new empty set of parameter bindings.
    /// </summary>
    public ParameterBindings()
    {
      this.m_parameterBindings = new Dictionary<ObjectParameterVariable, Constant>();
    }

    /// <summary>
    /// Returns the constant value associated with the specified parameter variable.
    /// </summary>
    /// <param name="var">The parameter variable for which the binding must be retrieved.
    /// </param>
    /// <returns>The constant value associated with the parameter variable.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if the
    /// specified parameter variable is not bound.</exception>
    public Constant GetBinding(ObjectParameterVariable var)
    {
      Constant binding;
      bool success = this.TryGetBinding(var, out binding);
      if (success)
      {
        return binding;
      }
      else
      {
        throw new BindingException(var);
      }
    }

    /// <summary>
    /// Retrieves the constant value associated with the specified parameter variable.
    /// This function returns true if the specified parameter variable is bound, else
    /// it returns false and the returned binding is not defined.
    /// </summary>
    /// <param name="var">The parameter variable for which the binding must be retrieved.
    /// </param>
    /// <param name="binding">If found, returns the constant value associated with the
    /// parameter variable.</param>
    /// <returns>True if the specified parameter variable is bound.</returns>
    public virtual bool TryGetBinding(ObjectParameterVariable var, out Constant binding)
    {
      if (m_parameterBindings.TryGetValue(var, out binding))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Binds a parameter variable to a specified constant value.
    /// </summary>
    /// <param name="var">The parameter variable to bind.</param>
    /// <param name="value">The constant to bind this variable to.</param>
    public void Bind(ObjectParameterVariable var, Constant value)
    {
      System.Diagnostics.Debug.Assert(var.CanBeAssignedFrom(value));

      this.m_parameterBindings[var] = value;
    }

    /// <summary>
    /// Returns a clone of this set of parameter bindings.
    /// </summary>
    /// <returns>A clone of this set of parameter bindings.</returns>
    public virtual object Clone()
    {
      ParameterBindings other = (ParameterBindings)this.MemberwiseClone();
      other.m_parameterBindings = new Dictionary<ObjectParameterVariable, Constant>(this.m_parameterBindings);

      return other;
    }

    /// <summary>
    /// Returns whether this set of parameter bindings is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to compare this set of bindings to.</param>
    /// <returns>True if this set of parameter bindings is equal to another object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        return this.m_parameterBindings.DictionaryEqual(((ParameterBindings)obj).m_parameterBindings);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this set of parameter bindings.
    /// </summary>
    /// <returns>The hash code of this set of parameter bindings.</returns>
    public override int GetHashCode()
    {
      return m_parameterBindings.DictionaryGetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this set of parameter bindings.
    /// </summary>
    /// <returns>A string representation of this set of parameter bindings.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("[");
      bool first = true;
      foreach (KeyValuePair<ObjectParameterVariable, Constant> p in m_parameterBindings)
      {
        if (!first)
        {
          str.Append(", ");
        }
        first = false;
        str.Append(p.Key.ToString() + "/" + p.Value.ToString());
      }
      str.Append("]");

      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this set of parameter variables.
    /// </summary>
    /// <returns>A typed string representation of this set of parameter variables.</returns>
    public virtual string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("[");
      bool first = true;
      foreach (KeyValuePair<ObjectParameterVariable, Constant> p in m_parameterBindings)
      {
        if (!first)
        {
          str.Append(", ");
        }
        first = false;
        str.Append(p.Key.ToTypedString() + "/" + p.Value.ToTypedString());
      }
      str.Append("]");

      return str.ToString();
    }
  }
}
