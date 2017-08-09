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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term.Type;
using PDDLParser.World;

namespace PDDLParser.Exp.Term
{
  /// <summary>
  /// A constant is an object of a specific type defined in either the domain or problem.
  /// </summary>
  public class Constant : AbstractTerm, IComparable<Constant>
  {
    /// <summary>
    /// The name of this constant.
    /// </summary>
    private string m_name;

    /// <summary>
    /// The mappings from each typeset to the specified constant ID. For example,
    /// the constant "robot1" may be the 12th "object" but the 1st "robot".
    /// </summary>
    private IDictionary<TypeSet, int> m_typesetToConstantIDs;

    /// <summary>
    /// Creates a new constant with the specified name and typeset.
    /// </summary>
    /// <param name="name">The name of the new constant.</param>
    /// <param name="typeSet">The typeset of the new constant.</param>
    public Constant(string name, TypeSet typeSet)
      : base(typeSet)
    {
      System.Diagnostics.Debug.Assert(typeSet != null);

      this.m_name = name;
      this.m_typesetToConstantIDs = new Dictionary<TypeSet, int>();
    }

    /// <summary>
    /// Gets the name of this constant.
    /// </summary>
    public string Name
    {
      get { return this.m_name; }
    }

    /// <summary>
    /// Sets the typeset of this constant. This function should only be used internally.
    /// </summary>
    /// <param name="typeSet">The typeset of this constant.</param>
    internal void SetTypeSet(TypeSet typeSet)
    {
      this.m_typeSet = typeSet;
    }

    /// <summary>
    /// Returns the constant ID corresponding to the given typeset.
    /// </summary>
    /// <param name="typeSet">A typeset this constant is member of.</param>
    /// <returns>The constant ID corresponding to the given typeset.</returns>
    public int GetConstantID(TypeSet typeSet)
    {
      return m_typesetToConstantIDs[typeSet];
    }

    /// <summary>
    /// Sets the constant ID corresponding to the given typeset.
    /// </summary>
    /// <param name="typeSet">A typeset this constant is member of.</param>
    /// <param name="constantID">The constant ID corresponding to this typeset.</param>
    public void SetConstantID(TypeSet typeSet, int constantID)
    {
      m_typesetToConstantIDs[typeSet] = constantID;
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// A constant expression evaluates to itself.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>This constant expression.</returns>
    public override FuzzyConstantExp Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new FuzzyConstantExp(this);
    }

    /// <summary>
    /// Simplifies this term by evaluating its known expression parts.
    /// A constant expression simplifies to itself.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>This constant expression.</returns>
    public override TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new TermValue(this);
    }

    /// <summary>
    /// Evaluates this term in the specified closed world.
    /// A constant expression evaluates to itself.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>This constant expression.</returns>
    public override ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new ConstantExp(this);
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// A constant expression is ground.
    /// </summary>
    /// <returns>True.</returns>
    public override bool IsGround()
    {
      return true;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// A constant expression holds no free variables.
    /// </summary>
    /// <returns>The empty set.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return new HashSet<Variable>();
    }

    /// <summary>
    /// Returns whether this constant is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this constant is equal to another object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      else if (this.GetType().Equals(obj.GetType()))
        return this.m_name == ((Constant)obj).m_name;
      else
        return false;
    }

    /// <summary>
    /// Returns the hash code of this constant.
    /// </summary>
    /// <returns>The hash code of this constant.</returns>
    public override int GetHashCode()
    {
      return m_name.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this constant.
    /// </summary>
    /// <returns>A string representation of this constant.</returns>
    public override string ToString()
    {
      return this.m_name;
    }

    /// <summary>
    /// Returns a typed string representation of this constant.
    /// </summary>
    /// <returns>A typed string representation of this constant.</returns>
    public override string ToTypedString()
    {
      return this.m_name + " - " + this.GetTypeSet();
    }

    #region IComparable<IExp> Members

    /// <summary>
    /// Compares this constant with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this constant to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      Constant otherCst = (Constant)other;

      return this.CompareTo(otherCst);
    }

    #endregion

    #region IComparable<Constant> Members

    /// <summary>
    /// Compares this constant with another constant.
    /// </summary>
    /// <param name="other">The other constant to compare this constant to.</param>
    /// <returns>An integer representing the total order relation between the two constants.
    /// </returns>
    public int CompareTo(Constant other)
    {
      return this.m_name.CompareTo(other.m_name);
    }

    #endregion
  }
}
