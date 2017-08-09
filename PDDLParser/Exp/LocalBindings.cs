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
using PDDLParser.Exp.Formula.TLPlan.LocalVar;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp
{
  /// <summary>
  /// This class extends the ParameterBindings class by adding local variables bindings.
  /// </summary>
  public class LocalBindings : ParameterBindings
  {
    /// <summary>
    /// The previous set of parameter bindings in the hierarchy. This is used when, for
    /// example, a quantified expression is nested inside another quantified expression.
    /// </summary>
    private ParameterBindings m_previous;

    /// <summary>
    /// The set of local object variables bindings. Note that a local object variable can be
    /// bound to an undefined value.
    /// </summary>
    private IDictionary<ObjectVariable, ConstantExp> m_objectBindings;

    /// <summary>
    /// The set of local numeric variables bindings. Note that a local numeric variable can be
    /// bound to an undefined value.
    /// </summary>
    private IDictionary<NumericVariable, Double> m_numericBindings;

    /// <summary>
    /// The set of local boolean variables bindings. Note that a local boolean variable can be
    /// bound to an undefined value.
    /// </summary>
    private IDictionary<BooleanVariable, Bool> m_booleanBindings;

    /// <summary>
    /// The empty set of bindings
    /// </summary>
    private static LocalBindings s_emptyBindings = new LocalBindings();

    /// <summary>
    /// Gets the empty set of bindings.
    /// </summary>
    public static LocalBindings EmptyBindings
    {
      get { return s_emptyBindings; }
    }

    /// <summary>
    /// Creates a new empty set of local/parameter bindings.
    /// </summary>
    public LocalBindings()
      : this(null)
    {
    }

    /// <summary>
    /// Creates a new empty set of local/parameter bindings with a specified parent set.
    /// </summary>
    /// <param name="previous">The parent set of parameter bindings.</param>
    public LocalBindings(ParameterBindings previous)
      : base()
    {
      this.m_previous = previous;
      this.m_objectBindings = new Dictionary<ObjectVariable, ConstantExp>();
      this.m_numericBindings = new Dictionary<NumericVariable, Double>();
      this.m_booleanBindings = new Dictionary<BooleanVariable, Bool>();
    }

    /// <summary>
    /// Retrieves the constant value associated with the specified parameter variable.
    /// This function returns true if the specified parameter variable is bound, else
    /// it returns false and the returned binding is not defined. Note that if the
    /// current binding can not be found in this set of bindings, it is searched for
    /// in the parent set.
    /// </summary>
    /// <param name="var">The parameter variable for which the binding must be retrieved.
    /// </param>
    /// <param name="binding">If found, returns the constant value associated with the
    /// parameter variable.</param>
    /// <returns>True if the specified parameter variable is bound.</returns>
    public override bool TryGetBinding(ObjectParameterVariable var, out Constant binding)
    {
      if (base.TryGetBinding(var, out binding))
      {
        return true;
      }
      else if (m_previous != null)
      {
        return m_previous.TryGetBinding(var, out binding);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the constant (or undefined) value associated with the specified 
    /// object local variable.
    /// </summary>
    /// <param name="var">The object local variable for which the binding must be 
    /// retrieved.</param>
    /// <returns>The constant (or undefined) value associated with the object 
    /// local variable.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if the
    /// specified object local variable is not bound.</exception>
    public ConstantExp GetBinding(ObjectLocalVariable var)
    {
      ConstantExp binding;
      if (!this.m_objectBindings.TryGetValue(var, out binding))
      {
        throw new BindingException(var);
      }
      return binding;
    }

    /// <summary>
    /// Retrieves the constant (or undefined) value associated with the specified 
    /// object local variable.
    /// This function returns true if the specified object local variable is bound, 
    /// else it returns false and the returned binding is not defined.
    /// </summary>
    /// <param name="var">The object local variable for which the binding must be 
    /// retrieved.</param>
    /// <param name="binding">If found, returns the constant (or undefined) value 
    /// associated with the object local variable.</param>
    /// <returns>True if the specified object local variable is bound.</returns>
    public bool TryGetBinding(ObjectLocalVariable var, out ConstantExp binding)
    {
      if (m_objectBindings.TryGetValue(var, out binding))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Binds the specified object local variable to a constant (or undefined) value.
    /// </summary>
    /// <param name="var">The object local variable to bind.</param>
    /// <param name="value">The constant (or undefined) value to bind the local 
    /// variable to.</param>
    public void Bind(ObjectLocalVariable var, ConstantExp value)
    {
      System.Diagnostics.Debug.Assert(value.Status == ConstantExp.State.Undefined ||
                                      var.CanBeAssignedFrom(value.Value));

      this.m_objectBindings[var] = value;
    }

    /// <summary>
    /// Returns the numeric (or undefined) value associated with the specified 
    /// numeric local variable.
    /// </summary>
    /// <param name="var">The numeric local variable for which the binding must 
    /// be retrieved.</param>
    /// <returns>The numeric (or undefined) value associated with the numeric 
    /// local variable.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if the
    /// specified numeric local variable is not bound.</exception>
    public Double GetBinding(NumericLocalVariable var)
    {
      Double binding;
      if (!this.m_numericBindings.TryGetValue(var, out binding))
      {
        throw new BindingException(var);
      }
      return binding;
    }

    /// <summary>
    /// Retrieves the numeric (or undefined) value associated with the specified 
    /// numeric local variable.
    /// This function returns true if the specified numeric local variable is bound, 
    /// else it returns false and the returned binding is not defined.
    /// </summary>
    /// <param name="var">The numeric local variable for which the binding must be 
    /// retrieved.</param>
    /// <param name="binding">If found, returns the numeric (or undefined) value 
    /// associated with the numeric local variable.</param>
    /// <returns>True if the specified numeric local variable is bound.</returns>
    public bool TryGetBinding(NumericLocalVariable var, out Double binding)
    {
      if (m_numericBindings.TryGetValue(var, out binding))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Binds the specified numeric local variable to a numeric (or undefined) value.
    /// </summary>
    /// <param name="var">The numeric local variable to bind.</param>
    /// <param name="value">The numeric (or undefined) value to bind the local 
    /// variable to.</param>
    public void Bind(NumericLocalVariable var, Double value)
    {
      this.m_numericBindings[var] = value;
    }

    /// <summary>
    /// Returns the boolean (or undefined) value associated with the specified 
    /// boolean local variable.
    /// </summary>
    /// <param name="var">The boolean local variable for which the binding must 
    /// be retrieved.</param>
    /// <returns>The boolean (or undefined) value associated with the boolean 
    /// local variable.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if the
    /// specified boolean local variable is not bound.</exception>
    public Bool GetBinding(BooleanLocalVariable var)
    {
      Bool binding;
      if (!this.m_booleanBindings.TryGetValue(var, out binding))
      {
        throw new BindingException(var);
      }
      return binding;
    }

    /// <summary>
    /// Retrieves the boolean (or undefined) value associated with the specified 
    /// boolean local variable.
    /// This function returns true if the specified boolean local variable is bound, 
    /// else it returns false and the returned binding is not defined.
    /// </summary>
    /// <param name="var">The boolean local variable for which the binding must be 
    /// retrieved.</param>
    /// <param name="binding">If found, returns the boolean (or undefined) value 
    /// associated with the boolean local variable.</param>
    /// <returns>True if the specified boolean local variable is bound.</returns>
    public bool TryGetBinding(BooleanLocalVariable var, out Bool binding)
    {
      if (m_booleanBindings.TryGetValue(var, out binding))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Binds the specified boolean local variable to a boolean (or undefined) value.
    /// </summary>
    /// <param name="var">The boolean local variable to bind.</param>
    /// <param name="value">The boolean (or undefined) value to bind the local 
    /// variable to.</param>
    public void Bind(BooleanLocalVariable var, Bool value)
    {
      this.m_booleanBindings[var] = value;
    }

    /// <summary>
    /// Returns a clone of this set of local/parameter bindings.
    /// </summary>
    /// <returns>A clone of this set of local/parameter bindings.</returns>
    public override object Clone()
    {
      LocalBindings other = (LocalBindings)base.Clone();
      other.m_previous = this.m_previous;
      other.m_objectBindings = new Dictionary<ObjectVariable, ConstantExp>(this.m_objectBindings);
      other.m_numericBindings = new Dictionary<NumericVariable, Double>(this.m_numericBindings);
      other.m_booleanBindings = new Dictionary<BooleanVariable, Bool>(this.m_booleanBindings);

      return other;
    }

    /// <summary>
    /// Returns whether this set of local/parameter bindings is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to compare this set of bindings to.</param>
    /// <returns>True if this set of local/parameter bindings is equal to another object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        LocalBindings other = (LocalBindings)obj;
        return base.Equals(obj) &&
               this.m_objectBindings.DictionaryEqual(other.m_objectBindings) &&
               this.m_numericBindings.DictionaryEqual(other.m_numericBindings) &&
               this.m_booleanBindings.DictionaryEqual(other.m_booleanBindings) &&
               this.m_previous.EqualsOrBothNull(other.m_previous);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this set of local/parameter bindings.
    /// </summary>
    /// <returns>The hash code of this set of local/parameter bindings.</returns>
    public override int GetHashCode()
    {
      return m_parameterBindings.DictionaryGetHashCode() +
             m_objectBindings.DictionaryGetHashCode() +
             m_numericBindings.DictionaryGetHashCode() +
             m_booleanBindings.DictionaryGetHashCode() +
             ((m_previous == null) ? 0 : m_previous.GetHashCode());
    }

    /// <summary>
    /// Returns a string representation of this set of local/parameter bindings.
    /// </summary>
    /// <returns>A string representation of this set of local/parameter bindings.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("[");
      IEnumerable<KeyValuePair<Variable, string>> variables =
        this.m_parameterBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToString())).Concat(
        this.m_objectBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToString()))).Concat(
        this.m_numericBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToString()))).Concat(
        this.m_booleanBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToString())));

      bool first = true;
      foreach (KeyValuePair<Variable, string> p in variables)
      {
        if (!first)
        {
          str.Append(", ");
        }
        first = false;
        str.Append(p.Key.ToString() + "/" + p.Value);
      }
      str.Append("]");
      if (m_previous != null)
      {
        str.Append(m_previous.ToString());
      }

      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this set of local/parameter bindings.
    /// </summary>
    /// <returns>A typed string representation of this set of local/parameter bindings.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("[");
      IEnumerable<KeyValuePair<Variable, string>> variables =
        this.m_parameterBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToTypedString())).Concat(
        this.m_objectBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToTypedString()))).Concat(
        this.m_numericBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToString()))).Concat(
        this.m_booleanBindings.Select(p => new KeyValuePair<Variable, string>(p.Key, p.Value.ToString())));


      bool first = true;
      foreach (KeyValuePair<Variable, string> p in variables)
      {
        if (!first)
        {
          str.Append(", ");
        }
        first = false;
        str.Append(p.Key.ToString() + "/" + p.Value);
      }
      str.Append("]");
      if (m_previous != null)
      {
        str.Append(m_previous.ToTypedString());
      }

      return str.ToString();
    }
  }
}
