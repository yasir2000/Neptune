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
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Formula.TLPlan.LocalVar
{
  /// <summary>
  /// This class represents an assignement of a value to a local variable.
  /// </summary>
  [TLPlan]
  public abstract class AssignLocalVar : AbstractLogicalExp
  {
    /// <summary>
    /// The local variable to which a value must be assigned.
    /// </summary>
    protected ILocalVariable m_localVariable;
    /// <summary>
    /// The evaluable expression corresponding to the value to assign to the local variable.
    /// </summary>
    protected IEvaluableExp m_body;

    /// <summary>
    /// Creates a new local variable assignement with the specified local variable and
    /// assignation expression.
    /// </summary>
    /// <param name="localVariable">The local variable to assign a value to.</param>
    /// <param name="body">The assignation expression.</param>
    public AssignLocalVar(ILocalVariable localVariable, IEvaluableExp body)
    {
      this.m_localVariable = localVariable;
      this.m_body = body;
    }

    /// <summary>
    /// Binds the local variable associated with this assignment to the evaluated assignation
    /// expression.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    protected abstract void BindLocalVariable(IReadOnlyClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// Tries and binds the local variable associated with this assignment to the evaluated assignation
    /// expression.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True if the binding was successfully done.</returns>
    protected abstract bool TryBindLocalVariable(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// Evaluating an assignment expression always returns true, but has the side effect of
    /// binding a local variable to a certain value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public override FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      TryBindLocalVariable(world, bindings);

      return FuzzyBool.True;
    }

    /// <summary>
    /// Evaluates this logical expression in the specified open world.
    /// In addition to False, Undefined and Unknown also shortcircuit conjunctions.
    /// In addition to True, Unknown also shortcircuits disjunctions.
    /// Evaluating an assignment expression always returns true, but has the side effect of
    /// binding a local variable to a certain value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    [TLPlan]
    public override ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      TryBindLocalVariable(world, bindings);

      return ShortCircuitFuzzyBool.True;
    }

    /// <summary>
    /// Simplifies this logical expression by evaluating its known expression parts.
    /// Simplifying an assigment returns true if the assignation expression is directly
    /// evaluable (and it binds the variable accordingly), else it returns the assignement
    /// itself.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    public override LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
    {
      if (TryBindLocalVariable(world, bindings))
      {
        return LogicalValue.True;
      }
      else
      {
        // TODO: this could be improved by simplifying this assignment's body.
        return new LogicalValue(this);
      }
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// Evaluating an assignment expression always returns true, but has the side effect of
    /// binding a local variable to a certain value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public override Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      BindLocalVariable(world, bindings);

      return Bool.True;
    }

    /// <summary>
    /// Evaluates this logical expression in the specified closed world.
    /// In addition to False, Undefined also shortcircuit conjunctions.
    /// Evaluating an assignment expression always returns true, but has the side effect of
    /// binding a local variable to a certain value.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    [TLPlan]
    public override ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
    {
      BindLocalVariable(world, bindings);

      return ShortCircuitBool.True;
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// All worlds satisfy an assignement expression since it always evaluates to true.
    /// </summary>
    /// <returns>All possible worlds.</returns>
    public override HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
    {
      HashSet<PartialWorld> world = new HashSet<PartialWorld>();
      world.Add(new PartialWorld());
      return world;
    }

    /// <summary>
    /// Evaluates the progression of this constraint expression in the next worlds.
    /// </summary>
    /// <param name="world">The current world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, undefined, or a progressed expression.</returns>
    public override ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
    {
      return new ProgressionValue(Evaluate(world, bindings));
    }

    /// <summary>
    /// Evaluates this constraint expression in an idle world, i.e. a world which
    /// won't be modified by further updates.
    /// </summary>
    /// <param name="idleWorld">The (idle) evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>True, false, or undefined.</returns>
    public override Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
    {
      return Evaluate(idleWorld, bindings);
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>A substituted copy of this expression.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      AssignLocalVar clone = (AssignLocalVar)this.Clone();
      clone.m_body = (IEvaluableExp)this.m_body.Apply(bindings);

      return clone;
    }

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
      AssignLocalVar clone = (AssignLocalVar)this.Clone();
      clone.m_body = (IEvaluableExp)this.m_body.Standardize(images);

      return clone;
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>Whether the expression is ground.</returns>
    public override bool IsGround()
    {
      return m_body.IsGround();
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The free variables in this expression.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return this.m_body.GetFreeVariables();
    }

    /// <summary>
    /// Returns whether this assignment is equal to another object.
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
        AssignLocalVar other = (AssignLocalVar)obj;
        return this.m_localVariable.Equals(other.m_localVariable)
            && this.m_body.Equals(other.m_body);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this assignment.
    /// </summary>
    /// <returns>The hash code of this assignment.</returns>
    public override int GetHashCode()
    {
      return this.m_localVariable.GetHashCode() + this.m_body.GetHashCode();
    }

    /// <summary>
    /// Returns a typed string representation of this assignement.
    /// </summary>
    /// <returns>A typed string representation of this assignement.</returns>
    public override string ToTypedString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:= ");
      str.Append(this.m_localVariable.ToTypedString());
      str.Append(" ");
      str.Append(this.m_body.ToTypedString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of this assignement.
    /// </summary>
    /// <returns>A string representation of this assignement.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:= ");
      str.Append(this.m_localVariable.ToString());
      str.Append(" ");
      str.Append(this.m_body.ToString());
      str.Append(")");
      return str.ToString();
    }

    /// <summary>
    /// Compares this assignment with another expression.
    /// </summary>
    /// <param name="other">The other expression to compare this assignment to.</param>
    /// <returns>An integer representing the total order relation between the two expressions.
    /// </returns>
    public override int CompareTo(IExp other)
    {
      int value = base.CompareTo(other);
      if (value != 0)
        return value;

      AssignLocalVar assign = (AssignLocalVar)other;
      value = this.m_localVariable.CompareTo(assign.m_localVariable);
      if (value != 0)
        return value;

      return this.m_body.CompareTo(assign.m_body);
    }
  }
}
