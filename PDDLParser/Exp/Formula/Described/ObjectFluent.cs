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
  /// An object fluent is a described formula associated with an object value 
  /// (a constant defined in the domain or problem).
  /// </summary>
  public class ObjectFluent : Fluent
  {
    /// <summary>
    /// The typeset associated with this object fluent.
    /// </summary>
    private TypeSet m_typeSet;

    /// <summary>
    /// Creates a new object fluent with the same properties as another specified fluent.
    /// </summary>
    /// <param name="other">The other fluent to copy.</param>
    /// <param name="typeSet">The typeset of the new object fluent.</param>
    public ObjectFluent(Fluent other, TypeSet typeSet)
      : this(other.Name, new List<ObjectParameterVariable>(other.Parameters),
             typeSet, other.m_attributes)
    {
    }

    /// <summary>
    /// Creates a new object fluent with the specified name, arguments, and typeset.
    /// </summary>
    /// <param name="name">The name of the new object fluent.</param>
    /// <param name="arguments">The arguments of the new object fluent.</param>
    /// <param name="typeSet">The typeset of the new object fluent.</param>
    /// <param name="attributes">The new described formula's attributes.</param>
    public ObjectFluent(string name, List<ObjectParameterVariable> arguments, TypeSet typeSet, 
                        Attributes attributes)
      : base(name, arguments, attributes)
    {
      System.Diagnostics.Debug.Assert(arguments != null && !arguments.ContainsNull() && typeSet != null);

      this.m_typeSet = typeSet;
    }

    /// <summary>
    /// Returns the typeset of this object fluent.
    /// </summary>
    /// <returns>The typeset of this object fluent.</returns>
    internal TypeSet GetTypeSet()
    {
      return m_typeSet;
    }

    /// <summary>
    /// Instantiates a formula application associated with this object fluent.
    /// </summary>
    /// <param name="arguments">Arguments of the formula application to instantiate.</param>
    /// <returns>A new object fluent application associated with this object fluent.</returns>
    public override FormulaApplication Instantiate(List<ITerm> arguments)
    {
      return new ObjectFluentApplication(this, arguments);
    }

    /// <summary>
    /// Returns a typed string representation of this formula.
    /// </summary>
    /// <returns>A typed string representation of this formula.</returns>
    public override string ToTypedString()
    {
      return base.ToTypedString() + " - " + this.m_typeSet.ToString();
    }
  }
}
