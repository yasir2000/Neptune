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
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Exp.Term.Type;
using PDDLParser.Extensions;
using PDDLParser.World;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// This abstract class is the base class for formula applications, 
  /// i.e. instances of root formulas.
  /// </summary>
  public abstract class FormulaApplication : AbstractExp, IEvaluableExp
  {
    /// <summary>
    /// ID assigned to a formula with some non-constant arguments.
    /// </summary>
    private static int ID_NOT_CONSTANT_ARGUMENTS = -1;

    /// <summary>
    /// An ID representing this formula application (unique for the root formula).
    /// The ID is equal to -1 if the formula application is not ground.
    /// </summary>
    protected int? m_argumentsID;

    /// <summary>
    /// The corresponding root formula of this formula application.
    /// </summary>
    protected RootFormula m_rootFormula;

    /// <summary>
    /// The arguments of the formula.
    /// </summary>
    protected List<ITerm> m_arguments;

    /// <summary>
    /// Creates a new formula application of a specified root formula with a given
    /// list of arguments.
    /// </summary>
    /// <param name="rootFormula">The root formula to instantiate.</param>
    /// <param name="arguments">The arguments of this formula application.</param>
    public FormulaApplication(RootFormula rootFormula, List<ITerm> arguments)
    {
      System.Diagnostics.Debug.Assert(rootFormula != null && arguments != null && !arguments.ContainsNull());

      this.m_rootFormula = rootFormula;
      this.m_arguments = arguments;
      this.m_argumentsID = null;
    }

    /// <summary>
    /// Gets whether all the arguments of this formula are constant.
    /// </summary>
    public bool AllConstantArguments
    {
      get { return this.ArgumentsID != ID_NOT_CONSTANT_ARGUMENTS; }
    }

    /// <summary>
    /// Gets the name of this formula application.
    /// </summary>
    public string Name
    {
      get { return m_rootFormula.Name; }
    }

    /// <summary>
    /// Gets the argument ID of this formula application, which is calculated by summing
    /// up the ID of all its arguments.
    /// </summary>
    public int ArgumentsID
    {
      get
      {
        if (this.m_argumentsID == null)
        {
          this.m_argumentsID = CalculateArgumentsID();
        }
        return this.m_argumentsID.Value;
      }
    }

    /// <summary>
    /// Gets the unique formula ID of this formula application, which is calculated
    /// as the sum of the formula's offset and this formula application's arguments ID.
    /// </summary>
    public int FormulaID
    {
      get
      {
        return this.ArgumentsID + this.m_rootFormula.Offset;
      }
    }

    /// <summary>
    /// Evaluates the arguments of this formula application in a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Unknown, undefined, or a new formula application defined by a list of 
    /// evaluated (constant) arguments.</returns>
    internal FuzzyArgsEvalResult EvaluateArguments(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      if (this.AllConstantArguments)
      {
        return new FuzzyArgsEvalResult(this);
      }
      else
      {
        bool atLeastOneUnknownArg = false;
        List<ITerm> constants = new List<ITerm>(m_arguments.Count);
        foreach (ITerm term in m_arguments)
        {
          FuzzyConstantExp result = term.Evaluate(world, bindings);
          switch (result.Status)
          {
            case FuzzyConstantExp.State.Defined:
              constants.Add(result.Value);
              break;
            case FuzzyConstantExp.State.Unknown:
              // Do not return immediately since an argument could be undefined.
              atLeastOneUnknownArg = true;
              break;
            case FuzzyConstantExp.State.Undefined:
              return FuzzyArgsEvalResult.Undefined;
            default:
              throw new System.Exception("Invalid FuzzyConstantValueStatus: " + result.Status);
          }
        }
        if (atLeastOneUnknownArg)
          return FuzzyArgsEvalResult.Unknown;
        else
          return new FuzzyArgsEvalResult(this.Apply(constants));
      }
    }

    /// <summary>
    /// Simplifies, if possible, the arguments of this formula application by evaluating them in a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or a new formula application defined by a list of simplified arguments.</returns>
    internal ArgsEvalResult SimplifyArguments(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      if (this.AllConstantArguments)
      {
        return new ArgsEvalResult(this);
      }
      else
      {
        List<ITerm> arguments = this.GetArguments();
        List<ITerm> newArguments = new List<ITerm>(arguments.Count);

        foreach (ITerm term in arguments)
        {
          TermValue value = term.Simplify(world, bindings);
          switch (value.Status)
          {
            case TermValue.State.Defined:
              newArguments.Add(value.Value);
              break;
            case TermValue.State.Undefined:
              return ArgsEvalResult.Undefined;
            default:
              throw new System.Exception("Invalid TermValueStatus: " + value.Status);
          }
        }
        return new ArgsEvalResult(this.Apply(newArguments));
      }
    }

    /// <summary>
    /// Evaluates the arguments of this formula application in a given world.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or a new formula application defined by a list of 
    /// evaluated (constant) arguments.</returns>
    internal ArgsEvalResult EvaluateArguments(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      if (this.AllConstantArguments)
      {
        return new ArgsEvalResult(this);
      }
      else
      {
        List<ITerm> constants = new List<ITerm>(m_arguments.Count);
        foreach (ITerm term in m_arguments)
        {
          ConstantExp result = term.Evaluate(world, bindings);
          switch (result.Status)
          {
            case ConstantExp.State.Defined:
              constants.Add(result.Value);
              break;
            case ConstantExp.State.Undefined:
              return ArgsEvalResult.Undefined;
            default:
              throw new System.Exception("Invalid ConstantValueStatus: " + result.Status);
          }
        }
        return new ArgsEvalResult(this.Apply(constants));
      }
    }

    /// <summary>
    /// Calculates the unique ID of this formula application.
    /// </summary>
    /// <returns>The unique ID of this formula application, which is -1 if this formula
    /// application is not ground.</returns>
    private int CalculateArgumentsID()
    {
      int id = 0;
      int cur = 1;
      IEnumerator<ObjectParameterVariable> variables = m_rootFormula.Parameters.GetEnumerator();
      IEnumerator<ITerm> arguments = this.m_arguments.GetEnumerator();

      while (variables.MoveNext() && arguments.MoveNext())
      {
        if (!(arguments.Current is Constant))
          return ID_NOT_CONSTANT_ARGUMENTS;

        Constant constant = (Constant)arguments.Current;
        TypeSet typeSet = variables.Current.GetTypeSet();
        id += (cur * constant.GetConstantID(typeSet));
        cur *= typeSet.Domain.Count;
      }
      return id;
    }

    /// <summary>
    /// Returns the arguments of this formula.
    /// </summary>
    /// <returns>The arguments of this formula.</returns>
    internal List<ITerm> GetArguments()
    {
      return this.m_arguments;
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      List<ITerm> arguments = new List<ITerm>(this.m_arguments.Count);
      foreach (ITerm arg in this.m_arguments)
      {
        arguments.Add((ITerm)arg.Apply(bindings));
      }
      return Apply(arguments);
    }

    /// <summary>
    /// Returns a copy of this formula application with the specified arguments.
    /// Implementation by MemberwiseClone() seems slower than calling the appropriate 
    /// constructor directly.
    /// </summary>
    /// <param name="arguments">The arguments of the new formula application.</param>
    /// <returns>A copy of this expression with the given arguments.</returns>
    public abstract FormulaApplication Apply(List<ITerm> arguments);

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      List<ITerm> arguments = new List<ITerm>(this.m_arguments.Select(arg => (ITerm)arg.Standardize(images)));

      return Apply(arguments);
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      if (!this.AllConstantArguments)
      {
        foreach (ITerm term in this.m_arguments)
        {
          if (!term.IsGround())
            return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      foreach (ITerm arg in this.m_arguments)
      {
        vars.UnionWith(arg.GetFreeVariables());
      }
      return vars;
    }

    /// <summary>
    /// Returns a clone of this expression.
    /// </summary>
    /// <returns>A clone of this expression.</returns>
    public override object Clone()
    {
      FormulaApplication other = (FormulaApplication)base.Clone();

      other.m_arguments = new List<ITerm>(this.m_arguments);
      return other;
    }

    /// <summary>
    /// Returns true if this formula application is equal to a specified object.
    /// By default, two formula applications are equal if their root formulas are the same,
    /// and their IDs are the same (and not == -1) or their IDs are both -1 and they are 
    /// equal according to the base class criteria.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this application formula is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        FormulaApplication other = (FormulaApplication)obj;
        return ((this.FormulaID == other.FormulaID &&
                 this.FormulaID != ID_NOT_CONSTANT_ARGUMENTS) ||
                (this.m_rootFormula.Equals(other.m_rootFormula) &&
                 this.m_arguments.SequenceEqual(other.m_arguments)));
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this application formula.
    /// </summary>
    /// <returns>The hash code of this application formula.</returns>
    public override int GetHashCode()
    {
      int ID = this.FormulaID;
      if (ID != ID_NOT_CONSTANT_ARGUMENTS)
      {
        return m_rootFormula.GetHashCode() * (1 + ID + (ID * ID));
      }
      else
      {
        return this.m_rootFormula.GetHashCode() * (this.Name.GetHashCode() + this.m_arguments.GetOrderedEnumerableHashCode());
      }
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
      foreach (ITerm t in this.m_arguments)
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
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.Name);
      foreach (ITerm t in this.m_arguments)
      {
        str.Append(" ");
        str.Append(t.ToTypedString());
      }
      str.Append(")");
      return str.ToString();
    }

    #region IComparable<IExp> Members

    /// <summary>
    /// Compares this formula application with another expression.
    /// </summary>
    /// <param name="obj">The other expression to compare this expression to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp obj)
    {
      int value = base.CompareTo(obj);
      if (value != 0)
        return value;

      FormulaApplication other = (FormulaApplication)obj;
      value = this.FormulaID.CompareTo(other.FormulaID);
      if (value != 0)
        return value;

      value = this.m_rootFormula.CompareTo(other.m_rootFormula);
      if (value != 0)
        return value;

      if (this.FormulaID != ID_NOT_CONSTANT_ARGUMENTS)
      {
        return 0;
      }
      else
      {
        value = this.m_rootFormula.CompareTo(other.m_rootFormula);
        if (value != 0)
          return value;

        value = this.m_arguments.Count.CompareTo(other.m_arguments.Count);
        if (value != 0)
          return value;

        for (int i = 0; value == 0 && i < this.m_arguments.Count; ++i)
        {
          value = this.m_arguments[i].CompareTo(other.m_arguments[i]);
        }
        return value;
      }
    }

    #endregion
  }
}
