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
  /// A described formula is a formula whose value is described directly in the world.
  /// Thus a world holds values corresponding to described formulas only.
  /// </summary>
  public abstract class DescribedFormula : RootFormula
  {
    #region internal struct Attributes

    /// <summary>
    /// This structures holds all possible described formulas' attributes.
    /// These attributes are not present in the PDDL grammar; they were rather added
    /// for TLPlan.
    /// </summary>
    public struct Attributes
    {
      /// <summary>
      /// Whether the cycles detection mechanism should use this formula.
      /// </summary>
      private bool m_detectCycles;

      /// <summary>
      /// Whether the cycles detection mechanism should use this formula.
      /// </summary>
      public bool DetectCycles
      {
        get { return m_detectCycles; }
        set { m_detectCycles = value; }
      }

      /// <summary>
      /// Initialize the set of attributes with the specified values.
      /// </summary>
      /// <param name="detectCycles">Whether to check for cycles.</param>
      public Attributes(bool detectCycles)
      {
        this.m_detectCycles = detectCycles;
      }
    }

    #endregion

    /// <summary>
    /// The default attributes.
    /// </summary>
    private static readonly Attributes s_defaultAttributes;
    /// <summary>
    /// The default attributes.
    /// </summary>
    public static Attributes DefaultAttributes
    {
      get
      {
        return s_defaultAttributes;
      }
    }

    /// <summary>
    /// Whether this formula is invariant (a formula is invariant if its value is never
    /// modified by actions).
    /// This value is set in the preprocessing phase.
    /// </summary>
    private bool m_isInvariant;
    /// <summary>
    /// The attributes of this described formula.
    /// </summary>
    internal Attributes m_attributes;

    /// <summary>
    /// Static constructor used to initialize the static members.
    /// </summary>
    static DescribedFormula()
    {
      s_defaultAttributes.DetectCycles = true;
    }

    /// <summary>
    /// Creates a new described formula with a specified name, arguments, and set of attributes.
    /// </summary>
    /// <param name="name">The name of the new described formula.</param>
    /// <param name="arguments">The arguments (variables) of the new described formula.</param>
    /// <param name="attributes">The new described formula's attributes.</param>
    public DescribedFormula(string name, List<ObjectParameterVariable> arguments, Attributes attributes)
      : base(name, arguments)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull());

      this.m_isInvariant = false;
      this.m_attributes = attributes;
    }

    /// <summary>
    /// Whether the cycles detection mechanism should use this formula.
    /// </summary>
    public bool DetectCycles
    { 
      get { return m_attributes.DetectCycles; }
    }

    /// <summary>
    /// Whether this formula is invariant (a formula is invariant if its value is never
    /// modified by actions).
    /// </summary>
    public bool Invariant
    { 
      get { return m_isInvariant; }
      set { m_isInvariant = value; }
    }
  }
}
