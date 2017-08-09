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
using PDDLParser.Exp.Term.Type;
using PDDLParser.World;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.Exp
{
  /// <summary>
  /// Base class for the different types of undefined expressions.
  /// </summary>
  public abstract class UndefinedExp : AbstractExp, IEvaluableExp
  {
    /// <summary>
    /// Creates a new undefined expression.
    /// </summary>
    public UndefinedExp()
      : base()
    {
    }

    /// <summary>
    /// Returns true if the expression is ground, i.e. it does not contain any variables.
    /// </summary>
    /// <returns>True.</returns>
    public override bool IsGround()
    {
      return true;
    }

    /// <summary>
    /// Substitutes all occurrences of the variables that occur in this
    /// expression by their corresponding bindings.
    /// </summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>This.</returns>
    public override IExp Apply(ParameterBindings bindings)
    {
      return this;
    }

    /// <summary>
    /// Standardizes all occurrences of the variables that occur in this
    /// expression. 
    /// </summary>
    /// <param name="images">The object that maps old variable images to the standardize
    /// image.</param>
    /// <returns>This.</returns>
    public override IExp Standardize(IDictionary<string, string> images)
    {
      return this;
    }

    /// <summary>
    /// Returns the free variables in this expression.
    /// </summary>
    /// <returns>The empty set.</returns>
    public override HashSet<Variable> GetFreeVariables()
    {
      return new HashSet<Variable>();
    }

    /// <summary>
    /// Returns a clone of this expression.
    /// </summary>
    /// <returns>This.</returns>
    public override object Clone()
    {
      return this;
    }

    /// <summary>
    /// Returns true if this expression is equal to a specified object.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this expression is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      return this == obj;
    }

    /// <summary>
    /// Returns the hash code of this expression.
    /// </summary>
    /// <returns>The hash code of this expression.</returns>
    public override int GetHashCode()
    {
      return FuzzyBool.Undefined.GetHashCode();
    }

    /// <summary>
    /// Returns a typed string of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      return "Undefined";
    }

    /// <summary>
    /// Returns a string representation of this expression.
    /// </summary>
    /// <returns>A string representation of this expression.</returns>
    public override string ToString()
    {
      return "Undefined";
    }
  }

  namespace Logical
  {
    /// <summary>
    /// This class represents an undefined logical expression.
    /// </summary>
    public class UndefinedLogicalExp : UndefinedExp, ILogicalExp
    {
      /// <summary>
      /// The immutable undefined logical expression.
      /// </summary>
      private static UndefinedLogicalExp s_undefinedExp = new UndefinedLogicalExp();

      /// <summary>
      /// Creates a new undefined logical expression. Note that this constructor is private.
      /// </summary>
      private UndefinedLogicalExp()
        : base()
      {
      }

      /// <summary>
      /// The undefined logical expression.
      /// </summary>
      public static UndefinedLogicalExp Undefined
      {
        get
        {
          return s_undefinedExp;
        }
      }

      /// <summary>
      /// Evaluates this logical expression in the specified open world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public FuzzyBool Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return FuzzyBool.Undefined;
      }

      /// <summary>
      /// Evaluates this logical expression in the specified open world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      [TLPlan]
      public ShortCircuitFuzzyBool EvaluateWithImmediateShortCircuit(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return ShortCircuitFuzzyBool.Undefined;
      }

      /// <summary>
      /// Simplifies this logical expression by evaluating its known expression parts.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public LogicalValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return LogicalValue.Undefined;
      }

      /// <summary>
      /// Evaluates this logical expression in the specified closed world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public Bool Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
      {
        return Bool.Undefined;
      }

      /// <summary>
      /// Evaluates this logical expression in the specified closed world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      [TLPlan]
      public ShortCircuitBool EvaluateWithImmediateShortCircuit(IReadOnlyClosedWorld world, LocalBindings bindings)
      {
        return ShortCircuitBool.Undefined;
      }

      /// <summary>
      /// Enumerates all the worlds within which this logical expression evaluates to true.
      /// This method is used to support the goal modality expressions.
      /// Note that no world can satisfy a false expression.
      /// </summary>
      /// <returns>The empty set.</returns>
      public HashSet<PartialWorld> EnumerateAllSatisfyingWorlds()
      {
        return new HashSet<PartialWorld>();
      }

      /// <summary>
      /// Evaluates the progression of this constraint expression in the next worlds.
      /// </summary>
      /// <param name="world">The current world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public ProgressionValue Progress(IReadOnlyDurativeClosedWorld world, LocalBindings bindings)
      {
        return ProgressionValue.Undefined;
      }

      /// <summary>
      /// Evaluates this constraint expression in an idle world, i.e. a world which
      /// won't be modified by further updates.
      /// </summary>
      /// <param name="idleWorld">The (idle) evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public Bool EvaluateIdle(IReadOnlyDurativeClosedWorld idleWorld, LocalBindings bindings)
      {
        return Bool.Undefined;
      }
    }
  }

  namespace Numeric
  {

    /// <summary>
    /// This class represents an undefined numeric expression.
    /// </summary>
    public class UndefinedNumericExp : UndefinedExp, INumericExp
    {
      /// <summary>
      /// The immutable undefined numeric expression.
      /// </summary>
      private static UndefinedNumericExp s_undefinedExp = new UndefinedNumericExp();

      /// <summary>
      /// Creates a new undefined numeric expression. Note that this constructor is private.
      /// </summary>
      private UndefinedNumericExp()
        : base()
      {
      }

      /// <summary>
      /// The undefined numeric expression.
      /// </summary>
      public static UndefinedNumericExp Undefined
      {
        get
        {
          return s_undefinedExp;
        }
      }

      /// <summary>
      /// Evaluates this numeric expression in the specified open world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public FuzzyDouble Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return FuzzyDouble.Undefined;
      }

      /// <summary>
      /// Simplifies this numeric expression by evaluating its known expression parts.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public NumericValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return NumericValue.Undefined;
      }

      /// <summary>
      /// Evaluates this numeric expression in the specified closed world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public Double Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
      {
        return Double.Undefined;
      }
    }
  }

  namespace Term
  {

    /// <summary>
    /// This class represents an undefined term.
    /// </summary>
    public class UndefinedTerm : UndefinedExp, ITerm
    {
      /// <summary>
      /// The immutable undefined term.
      /// </summary>
      private static UndefinedTerm s_undefinedTerm = new UndefinedTerm();

      /// <summary>
      /// Creates a new undefined term. Note that this constructor is private.
      /// </summary>
      private UndefinedTerm()
        : base()
      {
      }

      /// <summary>
      /// The undefined term.
      /// </summary>
      public static UndefinedTerm Undefined
      {
        get
        {
          return s_undefinedTerm;
        }
      }

      /// <summary>
      /// Returns the typeset of this term.
      /// </summary>
      /// <returns>This undefined typeset.</returns>
      public TypeSet GetTypeSet()
      {
        return UndefinedTypeSet.Instance;
      }

      /// <summary>
      /// Evaluates this term in the specified open world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public FuzzyConstantExp Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return FuzzyConstantExp.Undefined;
      }

      /// <summary>
      /// Simplifies this term by evaluating its known expression parts.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings)
      {
        return TermValue.Undefined;
      }

      /// <summary>
      /// Evaluates this term in the specified closed world.
      /// </summary>
      /// <param name="world">The evaluation world.</param>
      /// <param name="bindings">A set of variable bindings.</param>
      /// <returns>Undefined.</returns>
      public ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings)
      {
        return ConstantExp.Undefined;
      }

      /// <summary>
      /// Verifies whether the specified term can be assigned to this term, 
      /// i.e. if the other term's domain is a subset of this term's domain.
      /// </summary>
      /// <param name="term">The other term.</param>
      /// <returns>True if the types are compatible, false otherwise.</returns>
      public bool CanBeAssignedFrom(ITerm term)
      {
        return (this.GetTypeSet().CanBeAssignedFrom(term.GetTypeSet()));
      }

      /// <summary>
      /// Verifies whether the specified term can be compared to this term,
      /// i.e. if their domain overlap.
      /// </summary>
      /// <param name="term">The other term</param>
      /// <returns>True if the types can be compared, false otherwise.</returns>
      public bool IsComparableTo(ITerm term)
      {
        return (this.GetTypeSet().IsComparableTo(term.GetTypeSet()));
      }
    }
  }
}
