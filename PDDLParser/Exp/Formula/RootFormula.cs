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
using PDDLParser.Exp.Term;
using PDDLParser.Exp.Term.Type;
using PDDLParser.Extensions;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// This abstract class is the base class for root formula, i.e. formulas "template"
  /// defined in a PDDL domain.
  /// </summary>
  public abstract class RootFormula : IComparable<RootFormula>
  {
    /// <summary>
    /// The offset (in a lookup table) at which the values of the formula applications
    /// of this type can be found.
    /// This value is set in the preprocessing phase by the setOffSet() call.
    /// </summary>
    private int m_offset;

    /// <summary>
    /// The name of this root formula.
    /// </summary>
    protected readonly string m_name;

    /// <summary>
    /// The parameter variables of this root formula.
    /// </summary>
    protected List<ObjectParameterVariable> m_parameters;

    /// <summary>
    /// The hash code of this root formula.
    /// </summary>
    protected readonly int m_hashCode;

    /// <summary>
    /// Creates a new root formula with a specified name and arguments.
    /// </summary>
    /// <param name="name">The name of the new root formula.</param>
    /// <param name="arguments">The arguments (variables) of the new root formula.</param>
    public RootFormula(string name, List<ObjectParameterVariable> arguments)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());

      this.m_offset = -1;
      this.m_name = name;
      this.m_parameters = arguments;
      this.m_hashCode = m_name.GetHashCode();
    }

    /// <summary>
    /// Gets the name of this root formula.
    /// </summary>
    public string Name
    {
      get { return m_name; }
    }

    /// <summary>
    /// Gets/sets the offset (in a lookup table) at which the values of the instantiated formulas
    /// of this type can be found.
    /// </summary>
    public int Offset
    {
      get { return m_offset; }
      set { m_offset = value; }
    }

    /// <summary>
    /// Gets all the parameters (variables) of this root formula.
    /// </summary>
    public IEnumerable<ObjectParameterVariable> Parameters
    {
      get { return m_parameters; }
    }

    /// <summary>
    /// Updates this root formula according to another root formula.
    /// </summary>
    /// <param name="source">The other root formula used to update this one.</param>
    internal virtual void CopyFrom(RootFormula source)
    {
      this.m_parameters = new List<ObjectParameterVariable>(source.Parameters);
    }

    /// <summary>
    /// Returns the arity (number of arguments) of this root formula.
    /// </summary>
    /// <returns>The arity of this root formula.</returns>
    public int GetArity()
    {
      return m_parameters.Count;
    }

    /// <summary>
    /// Returns the p'th parameter of this root formula.
    /// </summary>
    /// <param name="p">The index of the parameter.</param>
    /// <returns>The p'th parameter of this root formula.</returns>
    public ObjectParameterVariable GetParameter(int p)
    {
      return m_parameters[p];
    }

    /// <summary>
    /// Instantiates a formula application associated with this root formula.
    /// </summary>
    /// <param name="arguments">Arguments of the formula application to instantiate.</param>
    /// <returns>A new formula application associated with this root formula.</returns>
    public abstract FormulaApplication Instantiate(List<ITerm> arguments);

    /// <summary>
    /// Returns the unique formula application associated with the given ID.
    /// </summary>
    /// <param name="argumentsID">The unique ID of the formula application to create.</param>
    /// <returns>The unique formula application associated with the given ID.</returns>
    public FormulaApplication InstantiateFromID(int argumentsID)
    {
      List<ITerm> constants = new List<ITerm>();
      foreach (ObjectParameterVariable var in this.m_parameters)
      {
        TypeSet typeSet = var.GetTypeSet();
        int typeSetCardinality = typeSet.Domain.Count;
        Constant constant = typeSet.GetConstant(argumentsID % typeSetCardinality);
        argumentsID /= typeSetCardinality;
        constants.Add(constant);
      }
      return Instantiate(constants);
    }

    /// <summary>
    /// Verifies whether this root formula matches with a specified formula application.
    /// The two formulas match if they have the same name, the same arity, and all the
    /// arguments of the formula application can be assigned to their respective variable
    /// counterpart of the root formula.
    /// </summary>
    /// <param name="formula">The formula application to be matched with this one.</param>
    /// <returns>True if the two formulas match.</returns>
    public bool Match(FormulaApplication formula)
    {
      bool match = this.Name.Equals(formula.Name)
                && this.m_parameters.Count == formula.GetArguments().Count;
      for (int i = 0; i < this.m_parameters.Count && match; ++i)
      {
        match = this.m_parameters[i].CanBeAssignedFrom(formula.GetArguments()[i]);
      }
      return match;
    }

    /// <summary>
    /// Returns the cardinality of this root formula's domain.
    /// The root formula's domain corresponds to all possible bindings of constants (objects)
    /// it may be evaluated with.
    /// </summary>
    /// <returns>The domain cardinality of this root formula.</returns>
    public int GetDomainCardinality()
    {
      int cardinality = 1;
      foreach (ObjectParameterVariable var in this.m_parameters)
      {
        List<Constant> constants = var.GetTypeSet().Domain;
        cardinality *= constants.Count;
      }
      return cardinality;
    }

    /// <summary>
    /// Creates a set of parameter bindings from a specified set of constant parameters.
    /// </summary>
    /// <param name="parameters">A set of constant parameters.</param>
    /// <returns>A set of parameter bindings.</returns>
    protected ParameterBindings GetParameterBindings(IEnumerable<Constant> parameters)
    {
      ParameterBindings parameterBindings = new ParameterBindings();
      IEnumerator<Constant> enumCst = parameters.GetEnumerator();

      foreach (ObjectParameterVariable var in this.m_parameters)
      {
        enumCst.MoveNext();
        parameterBindings.Bind(var, enumCst.Current);
      }
      return parameterBindings;
    }

    /// <summary>
    /// Returns true if this root formula is equal to a specified object.
    /// Two root formulas are equal if they are reference-equal.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this formula is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this root formula.
    /// </summary>
    /// <returns>The hash code of this root formula.</returns>
    public override int GetHashCode()
    {
      return this.m_hashCode;
    }

    /// <summary>
    /// Returns a string representation of this formula.
    /// </summary>
    /// <returns>A string representation of this formula.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.Name);
      foreach (ObjectParameterVariable t in this.m_parameters)
      {
        str.Append(" ");
        str.Append(t.ToString());
      }
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this formula.
    /// </summary>
    /// <returns>A typed string representation of this formula.</returns>
    public virtual string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.Name);
      foreach (ObjectParameterVariable t in this.m_parameters)
      {
        str.Append(" ");
        str.Append(t.ToTypedString());
      }
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<RootFormula> Members

    /// <summary>
    /// Compares this root formula with another root formula.
    /// </summary>
    /// <param name="other">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public int CompareTo(RootFormula other)
    {
      return this.m_name.CompareTo(other.m_name);
    }

    #endregion
  }
}
