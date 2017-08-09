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
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.World.Context;

namespace PDDLParser.World
{
  /// <summary>
  /// A partial world is a (partial) world implementation used to describe the set of worlds which
  /// satisfy a given logical formula, i.e. ILogicalExp.EnumerateAllSatisfyingWorlds()
  /// </summary>
  public class PartialWorld: IOpenWorld, ICloneable
  {
    /// <summary>
    /// The hash code of this partial world.
    /// </summary>
    protected int m_hashCode;

    /// <summary>
    /// The set of predicates having a fixed value. All predicates which do not appear
    /// in this set may have arbitrary values.
    /// </summary>
    protected Dictionary<AtomicFormulaApplication, bool> m_facts;

    /// <summary>
    /// Creates a new partial world within which all predicates have arbitrary values.
    /// </summary>
    public PartialWorld()
    {
      this.m_facts = new Dictionary<AtomicFormulaApplication, bool>();
      this.m_hashCode = 0;
    }

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// Any predicate not contained in the facts dictionary is unknown.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, undefined, or unknown.</returns>
    public FuzzyBool IsSet(AtomicFormulaApplication formula)
    {
      bool IsSet = false;
      if (m_facts.TryGetValue(formula, out IsSet))
      {
        return new FuzzyBool(IsSet);
      }
      else
      {
        return FuzzyBool.Unknown;
      }
    }

    /// <summary>
    /// Sets the specified described atomic formula to true.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    public void Set(AtomicFormulaApplication formula)
    {
      bool value;
      if (m_facts.TryGetValue(formula, out value))
      {
        m_hashCode -= (formula.GetHashCode() * (value.GetHashCode() + 1));
      }
      m_hashCode += (formula.GetHashCode() * (true.GetHashCode() + 1));
      m_facts[formula] = true;
    }

    /// <summary>
    /// Sets the specified described atomic formula to false.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    public void Unset(AtomicFormulaApplication formula)
    {
      bool value;
      if (m_facts.TryGetValue(formula, out value))
      {
        m_hashCode -= (formula.GetHashCode() * (value.GetHashCode() + 1));
      }
      m_hashCode += (formula.GetHashCode() * (true.GetHashCode() + 1));
      m_facts[formula] = false;
    }

    /// <summary>
    /// Returns a new unfinished evaluation record.
    /// </summary>
    /// <param name="pred">The defined predicate application.</param>
    /// <param name="existing">This flag is set to true if the defined predicate is already in the
    /// process of (or has finished) being evaluated.</param>
    /// <returns>An unfinished evaluation record.</returns>
    public IEvaluationRecord<FuzzyBoolValue> GetEvaluation(DefinedPredicateApplication pred, out bool existing)
    {
      existing = false;
      return new EvaluationRecord<FuzzyBoolValue>();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since 
    /// fluents are not supported.</exception>
    public FuzzyDouble GetNumericFluent(NumericFluentApplication fluent)
    {
      throw new NotSupportedException("Fluents are not supported!");
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since 
    /// fluents are not supported.</exception>
    public void SetNumericFluent(NumericFluentApplication fluent, double value)
    {
      throw new NotSupportedException("Fluents are not supported!");
    }

    /// <summary>
    /// Returns a new unfinished evaluation record.
    /// </summary>
    /// <param name="pred">The defined function application.</param>
    /// <param name="existing">This flag is set to true if the defined function is already in the
    /// process of (or has finished) being evaluated.</param>
    /// <returns>An unfinished evaluation record.</returns>
    public IEvaluationRecord<FuzzyDouble> GetEvaluation(DefinedNumericFunctionApplication pred, out bool existing)
    {
      existing = false;
      return new EvaluationRecord<FuzzyDouble>();
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since 
    /// fluents are not supported.</exception>
    public FuzzyConstantExp GetObjectFluent(ObjectFluentApplication fluent)
    {
      throw new NotSupportedException("Fluents are not supported!");
    }


    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.</param>
    /// <returns>Throws an exception.</returns>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since 
    /// fluents are not supported.</exception>
    public void SetObjectFluent(ObjectFluentApplication fluent, Constant value)
    {
      throw new NotSupportedException("Fluents are not supported!");
    }

    /// <summary>
    /// This function is not supported.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <exception cref="NotSupportedException">A NotSupportedException is always thrown since 
    /// fluents are not supported.</exception>
    public void UndefineObjectFluent(ObjectFluentApplication fluent)
    {
      throw new NotSupportedException("Fluents are not supported!");
    }

    /// <summary>
    /// Returns a new unfinished evaluation record.
    /// </summary>
    /// <param name="pred">The defined function application.</param>
    /// <param name="existing">This flag is set to true if the defined function is already in the
    /// process of (or has finished) being evaluated.</param>
    /// <returns>An unfinished evaluation record.</returns>
    public IEvaluationRecord<FuzzyConstantExp> GetEvaluation(DefinedObjectFunctionApplication pred, out bool existing)
    {
      existing = false;
      return new EvaluationRecord<FuzzyConstantExp>();
    }

    /// <summary>
    /// Returns the negation of this world, which is achieved by negating all
    /// its facts.
    /// </summary>
    /// <returns>The negation of this world.</returns>
    public PartialWorld Negate()
    {
      PartialWorld newContext = new PartialWorld();
      foreach (KeyValuePair<AtomicFormulaApplication, bool> pair in this.m_facts)
      {
        newContext.m_facts.Add(pair.Key, !pair.Value);
      }
      return newContext;
    }

    /// <summary>
    /// Joins this world with another partial world, resulting in a world where all facts are
    /// merged.
    /// Note that this function does not modify the current world, and it returns null if some 
    /// facts are inconsistent between the two worlds, for example (on a b) and (not (on a b)).
    /// </summary>
    /// <param name="other">Other world to join this world with.</param>
    /// <returns>A new world in which the facts are merged.</returns>
    public PartialWorld Join(PartialWorld other)
    {
      PartialWorld newContext = (PartialWorld)this.Clone();

      foreach (KeyValuePair<AtomicFormulaApplication, bool> pair in other.m_facts)
      {
        bool value;
        if (newContext.m_facts.TryGetValue(pair.Key, out value))
        {
          if (value != pair.Value)
            return null;
        }
        else
        {
          m_hashCode += (pair.Key.GetHashCode() * (pair.Value.GetHashCode() + 1));
          newContext.m_facts.Add(pair.Key, pair.Value);
        }
      }
      return newContext;
    }

    /// <summary>
    /// Returns the list of all atomic formula applications whose value are known inside
    /// this world.
    /// </summary>
    /// <returns>The list of all atomic formula applications whose value are known inside
    /// this world.</returns>
    public HashSet<AtomicFormulaApplication> GetAllPredicates()
    {
      return new HashSet<AtomicFormulaApplication>(this.m_facts.Keys);
    }

    /// <summary>
    /// Returns the hash code of this partial world.
    /// </summary>
    /// <returns>The hash code of this partial world.</returns>
    public override int GetHashCode()
    {
      return m_hashCode;
    }

    /// <summary>
    /// Returns whether this quantified expression is equal to another object.
    /// Two quantified expressions are equal if they are defined over the same quantified variables
    /// and their body is equal.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this quantified expression is equal to another object.</returns>
    public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }
      else if (this.GetType() == obj.GetType())
      {
        PartialWorld other = (PartialWorld)obj;
        return this.m_facts.DictionaryEqual(other.m_facts);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns a clone of this expression.
    /// The default implementation simply returns a memberwise clone.
    /// </summary>
    /// <returns>A clone of this expression.</returns>
    public object Clone()
    {
      PartialWorld other = (PartialWorld)this.MemberwiseClone();
      other.m_facts = new Dictionary<AtomicFormulaApplication, bool>(this.m_facts);

      return other;
    }
  }
}
