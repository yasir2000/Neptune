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
using PDDLParser.Exp.Struct;
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp.Numeric
{
  /// <summary>
  /// A number is a invariable numeric expression.
  /// </summary>
  public class Number : AbstractNumericExp
  {
    /// <summary>
    /// The value of this number.
    /// </summary>
    private double m_value;

    /// <summary>
    /// Creates a new number with the specified value.
    /// </summary>
    /// <param name="value">The value of the number.</param>
    public Number(double value)
      : base()
    {
      this.m_value = value;
    }

    /// <summary>
    /// Gets the value of this number.
    /// </summary>
    public double Value
    {
      get { return m_value; }
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified open world.
    /// Evaluating a number yields its value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The value of this number.</returns>
    public override FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new FuzzyDouble(m_value);
    }

    /// <summary>
    /// Simplifies this numeric expression by evaluating its known expression parts.
    /// Simplifying a number yields its value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The value of this number.</returns>
    public override NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      return new NumericValue(m_value);
    }

    /// <summary>
    /// Evaluates this numeric expression in the specified closed world.
    /// Evaluating a number yields its value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>The value of this number.</returns>
    public override Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      return new Double(m_value);
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// A number is ground.
    /// </summary>
    /// <returns>True.</returns>
    public override bool IsGround()
    {
      return true;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// A number contains no free variables.
    /// </summary>
    /// <returns>The empty set.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return new HashSet<Variable>();
    }

    /// <summary>
    /// Returns whether this number is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this number is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        return (obj.GetType() == this.GetType() && ((Number)obj).m_value.Equals(this.m_value));
      }
    }

    /// <summary>
    /// Returns the hash code of this number.
    /// </summary>
    /// <returns>The hash code of this number.</returns>
    public override int GetHashCode()
    {
      return m_value.GetHashCode();
    }
    
    /// <summary>
    /// Returns a string representation of this number.
    /// </summary>
    /// <returns>A string representation of this number.</returns>
    public override string ToString()
    {
      return this.Value.ToString();
    }

    /// <summary>
    /// Returns a typed string representation of this number.
    /// </summary>
    /// <returns>A typed string representation of this number.</returns>
    public override string ToTypedString()
    {
      return this.Value.ToString();
    }

    #region IComparable<IExp> Interface

    /// <summary>
    /// Compares this number with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this number to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      Number otherNumber = (Number)other;

      return this.m_value.CompareTo(otherNumber.m_value);
    }

    #endregion
  }
}