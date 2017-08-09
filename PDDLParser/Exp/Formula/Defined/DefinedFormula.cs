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
using PDDLParser.Extensions;
using PDDLParser.Exp.Term;

namespace PDDLParser.Exp.Formula
{
  /// <summary>
  /// A defined formula is a formula whose value is calculated from the values of other formulas.
  /// 
  /// TLPlan allows the use of local variables in the body of defined formulas, which are special
  /// logical expressions always returning true and being evaluated depending on the short-circuits
  /// encountered during the current evaluation.
  /// For example, 
  /// (and (= (location robot1) home)
  ///      (:= ?nr-robots-at-home (+ ?nr-robots-at-home 1)))
  /// If robot1 is at home, the local variable ?nr-robots-at-home is incremented. Else the
  /// evaluation returns (short-circuit) before evaluting the local variable assignation.
  /// When undefined or unknown values are introduced, the expression (= (location robot1) home)
  /// may not always evaluate to true or false, but also to undefined or unknown, which would
  /// normally not short-circuit the evaluation.
  /// The solution is to evaluate a defined formula's body differently:
  /// - evaluating something to unknown immediately shortcircuits the evaluation
  ///   (both conjunctions and disjunctions), thus always returning unknown.
  /// - evaluating something to undefined short-circuits conjunctions, returning undefined.
  /// Thus, in the above example, when robot1's location is undefined, the local variable
  /// ?nr-robots-at-home is not incremented.
  /// 
  /// In conclusion, there is two ways to evaluate logical (boolean expressions): 
  /// - using regular evaluation (Evaluate()), or 
  /// - using immediate short-circuits (EvaluateWithImmediateShortCircuit()),
  /// the latter being used to evaluate a defined formula's body.
  /// </summary>
  public abstract class DefinedFormula : RootFormula
  {
    /// <summary>
    /// The local variables used in the body of this defined formula.
    /// </summary>
    protected List<ILocalVariable> m_localVariables;

    /// <summary>
    /// The first order formula which defines this formula.
    /// </summary>
    private ILogicalExp m_body;

    /// <summary>
    /// The image of this defined formula, which indicates what kind of formula it is.
    /// </summary>
    private string m_image;

    /// <summary>
    /// Creates a new defined formula with a specified name, arguments, 
    /// and local variables.
    /// </summary>
    /// <param name="image">The image of this defined formula.</param>
    /// <param name="name">The name of this defined formula.</param>
    /// <param name="arguments">The arguments of this defined formula.</param>
    /// <param name="localVariables">the local variables used in the body of this defined formula.
    /// </param>
    public DefinedFormula(string image, string name, List<ObjectParameterVariable> arguments,
                          List<ILocalVariable> localVariables)
      : base(name, arguments)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull()
                                   && localVariables != null && !localVariables.ContainsNull());

      this.m_image = image;
      m_localVariables = localVariables;
      // The formula's body is null for now, since a defined formula is usually defined
      // after being declared (like functions in a programming langage).
      this.m_body = null;
    }

    /// <summary>
    /// Gets or sets the first order formula which defines the formula.
    /// </summary>
    internal ILogicalExp Body
    {
      get { return m_body; }
      set { m_body = value; }
    }

    /// <summary>
    /// Gets the local variables used in this defined formula.
    /// </summary>
    public IEnumerable<ILocalVariable> LocalVariables
    {
      get { return this.m_localVariables; }
    }

    /// <summary>
    /// Verifies whether this defined formula matches with another defined formula.
    /// The two formulas match if they are of the same type, have the same name, 
    /// the same arity, and all the arguments of the two formulas have the same domain (pairwise).
    /// </summary>
    /// <param name="formula">The other defined formula to be matched with this one.</param>
    /// <returns>True if the two defined formulas match.</returns>
    public virtual bool Match(DefinedFormula formula)
    {
      bool match = this.GetType() == formula.GetType();
      if (match)
      {
        match = this.Name.Equals(formula.Name)
             && this.m_parameters.Count == formula.m_parameters.Count;
        for (int i = 0; i < this.m_parameters.Count && match; ++i)
        {
          match = this.m_parameters[i].CanBeAssignedFrom(formula.m_parameters[i]) &&
                  formula.m_parameters[i].CanBeAssignedFrom(this.m_parameters[i]);
        }
      }
      return match;
    }

    /// <summary>
    /// Updates this root formula according to another root formula.
    /// </summary>
    /// <param name="other">The other root formula used to update this one.</param>
    internal override void CopyFrom(RootFormula other)
    {
      base.CopyFrom(other);
      if (other is DefinedFormula)
      {
        this.m_localVariables = new List<ILocalVariable>(((DefinedFormula)other).m_localVariables);
      }
    }

    /// <summary>
    /// Returns a "complete" string representation of this derived predicate, i.e.
    /// a representation that includes the local variables and formula's body.
    /// </summary>
    /// <returns>A complete string representation of this derived predicate.</returns>
    public string ToCompleteString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(");
      str.Append(this.m_image);
      str.Append(" ");
      str.Append(this.ToTypedString());
      str.Append(" ");
      if (this.m_localVariables.Count > 0)
      {
        str.Append("(local-vars");
        foreach (ILocalVariable var in this.m_localVariables)
        {
          str.Append(" ");
          str.Append(var.ToString());
        }
        str.Append(")");
      }
      str.Append((Body != null) ? Body.ToString() : "");
      str.Append(")");

      return str.ToString();
    }
  }
}
