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

namespace PDDLParser.Exp
{
  /// <summary>
  /// Base class for variables.
  /// </summary>
  public abstract class Variable : AbstractExp, IVariable
  {
    /// <summary>
    /// The name of this variable.
    /// </summary>
    protected string m_name;

    /// <summary>
    /// Creates a new variable with a specified name.
    /// </summary>
    /// <param name="name">The name of the new variable.</param>
    public Variable(string name)
    {
      this.m_name = name;
    }

    /// <summary>
    /// Gets the name of this variable.
    /// </summary>
    public string Name
    {
      get { return this.m_name; }
    }

    /// <summary>
    /// Standardizes a quantified variable.
    /// </summary>
    /// <param name="var">The quantified variable to standardize.</param>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of the variable.</returns>
    public static Variable standardizeQuantifiedVariable(Variable var, IDictionary<string, string> images)
    {
      string newImage = null;
      images.TryGetValue(var.m_name, out newImage);
      if (newImage == null)
      {
        string oldImage = var.m_name;
        newImage = Variable.getStandardizedImage(oldImage);
        images[oldImage] = newImage;
      }
      else
      {
        string oldImage = newImage;
        newImage = Variable.getStandardizedImage(oldImage);
        images[var.m_name] = newImage;
      }
      Variable newVar = (Variable)var.Clone();
      var.m_name = newImage;
      return var;
    }

    /// <summary>
    /// Returns the standardize name of a variable given its image.
    /// </summary>
    /// <param name="image">The variable's image.</param>
    /// <returns>The standardize name of the variable.</returns>
    public static string getStandardizedImage(string image)
    {
      string newImage = null;
      string[] str = image.Split('_');
      if (str.Length == 2)
      {
        long index = long.Parse(str[1]);
        index++;
        newImage = str[0] + "_" + index;

      }
      else
      {
        newImage = str[0] + "_0";
      }
      return newImage;
    }

    #region IEvaluable Interface

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. The IDictionary argument is used to store the variable already
    /// standardized. Remember that free variables are existentially quantified.
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>A standardized copy of this expression.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      string newImage = null;
      images.TryGetValue(this.m_name, out newImage);
      if (newImage == null)
      {
        string oldImage = this.m_name;
        newImage = Variable.getStandardizedImage(oldImage);
        images[oldImage] = newImage;
      }
      Variable var = (Variable)this.Clone();
      var.m_name = newImage;
      return var;
    }

    /// <summary>
    /// Returns true if this variable is free, false it is bound.
    /// </summary>
    /// <returns>Whether this variable is free.</returns>
    public abstract bool IsFree();

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// A variable is not ground.
    /// </summary>
    /// <returns>False.</returns>
    public override sealed bool IsGround()
    {
      return false;  
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override sealed HashSet<Variable> GetFreeVariables()
    {
      HashSet<Variable> vars = new HashSet<Variable>();
      if (this.IsFree())
      {
        vars.Add(this);
      }
      return vars;
    }

    /// <summary>
    /// Compares this variable with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this variable to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      Variable otherVar = (Variable)other;

      return this.m_name.CompareTo(otherVar.m_name);
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this variable is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this variable is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        return ((Variable)obj).m_name.Equals(this.m_name);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this variable.
    /// </summary>
    /// <returns>The hash code of this variable.</returns>
    public override int GetHashCode()
    {
      return m_name.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this variable.
    /// </summary>
    /// <returns>A string representation of this variable.</returns>
    public override string ToString()
    {
      return "?" + this.m_name;
    }

    #endregion

    #region IComparable<IVariable> Members

    /// <summary>
    /// Returns whether this variable is equal to another variable.
    /// </summary>
    /// <param name="other">The other variable to test for equality.</param>
    /// <returns>True if this variable is equal to the other variable.</returns>
    public int CompareTo(IVariable other)
    {
      int value = this.m_name.CompareTo(other.Name);
      if (value != 0)
        return value;
      else
        return this.GetType().GUID.CompareTo(other.GetType().GUID);
    }

    #endregion
  }
}