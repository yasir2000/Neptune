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
using PDDLParser;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using TLPlan.Utils;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// A qualified world is a custom world which stores facts and fluents using
  /// qualified hashsets.
  /// </summary>
  public class QualifiedWorld : ExtendedOpenWorld
  {
    #region Private class QualifiedHashSetFactsContainer

    /// <summary>
    /// A qualified hashset facts container holds qualified facts in a hashset.
    /// This implementation is fast for retrieving predicates' value, but comparison
    /// between two qualified hashset facts container is slow because their respective
    /// elements must be sorted beforehand.
    /// Moreover it is a little slower than a regular hashset facts container,
    /// but has the (debugging) advantage of offering a decent string representation
    /// of the facts.
    /// </summary>
    public class QualifiedHashSetFactsContainer : IComparable<QualifiedHashSetFactsContainer>
    {
      #region Private Fields

      /// <summary>
      /// The hashset of facts.
      /// </summary>
      private HashSet<AtomicFormulaApplication> m_facts;

      /// <summary>
      /// The cached list of sorted facts, necessary only for comparisons 
      /// with other containers.
      /// </summary>
      private List<AtomicFormulaApplication> m_sortedFacts;

      /// <summary>
      /// The hash code of this facts container.
      /// </summary>
      private int m_hashCode;

      #endregion

      #region Properties

      /// <summary>
      /// The list of sorted facts, necessary only for comparisons with other containers.
      /// </summary>
      protected List<AtomicFormulaApplication> SortedFacts
      {
        get
        {
          if (m_sortedFacts == null)
          {
            m_sortedFacts = new List<AtomicFormulaApplication>(m_facts);
            m_sortedFacts.Sort();
          }

          return m_sortedFacts;
        }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a new empty qualified hashset facts container.
      /// </summary>
      public QualifiedHashSetFactsContainer()
      {
        this.m_facts = new HashSet<AtomicFormulaApplication>();
        this.m_sortedFacts = null;
        this.m_hashCode = 0;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Copies this facts container.
      /// </summary>
      /// <returns>A copy of this facts container.</returns>
      public QualifiedHashSetFactsContainer Copy()
      {
        QualifiedHashSetFactsContainer world = (QualifiedHashSetFactsContainer)base.MemberwiseClone();
        // Do not clone internals, waste of memory? Copy on write?
        world.m_facts = new HashSet<AtomicFormulaApplication>(m_facts);
        world.m_sortedFacts = null;
        return world;
      }

      #endregion

      #region IConstantWorld Interface

      /// <summary>
      /// Checks whether the specified described atomic formula holds in this world.
      /// </summary>
      /// <param name="formula">A described (and ground) atomic formula.</param>
      /// <returns>True, or unknown.</returns>
      public FuzzyBool IsSet(AtomicFormulaApplication formula)
      {
        return m_facts.Contains(formula) ? FuzzyBool.True :
                                           FuzzyBool.Unknown;
      }

      #endregion

      #region IWorld Interface

      /// <summary>
      /// Sets the specified atomic formula to true.
      /// </summary>
      /// <param name="formula">An atomic formula with constant arguments.</param>
      public void Set(AtomicFormulaApplication formula)
      {
        if (m_facts.Add(formula))
          m_hashCode += formula.GetHashCode();
      }

      /// <summary>
      /// Sets the specified atomic formula to false.
      /// </summary>
      /// <param name="formula">An atomic formula with constant arguments.</param>
      public void Unset(AtomicFormulaApplication formula)
      {
        if (m_facts.Remove(formula))
          m_hashCode -= formula.GetHashCode();
      }

      #endregion

      #region Object Interface Overrides

      /// <summary>
      /// Returns whether this facts container is equal to another object.
      /// Two qualified hashset facts container are equal if they both store the same facts.
      /// </summary>
      /// <param name="obj">The other object to test for equality.</param>
      /// <returns>Whether this facts container is equal to the other object.</returns>
      public override bool Equals(object obj)
      {
        if (obj == this)
        {
          return true;
        }
        else
        {
          QualifiedHashSetFactsContainer other = (QualifiedHashSetFactsContainer)obj;
          return this.m_facts.SetEquals(other.m_facts);
        }
      }

      /// <summary>
      /// Returns the hash code of this facts container.
      /// </summary>
      /// <returns>The hash code of this facts container.</returns>
      public override int GetHashCode()
      {
        return m_hashCode;
      }

      /// <summary>
      /// Returns a string representation of this facts container.
      /// </summary>
      /// <returns>A string representation of this facts container.</returns>
      public override string ToString()
      {
        return m_facts.Aggregate("", (string s, AtomicFormulaApplication f) => s + " " + f);
      }

      #endregion

      #region IComparable<FactsContainer> interface

      /// <summary>
      /// Compares this facts container with another facts container.
      /// Comparison is done first on the hash code, then on the number of facts, 
      /// and finally facts are sorted and compared individually.
      /// </summary>
      /// <param name="other">The other facts container to compare this facts container to.</param>
      /// <returns>An integer representing the total order relation between the two facts containers.
      /// </returns>
      public int CompareTo(QualifiedHashSetFactsContainer other)
      {
        int value = this.GetHashCode().CompareTo(other.GetHashCode());
        if (value != 0)
          return value;

        if ((value = this.m_facts.Count.CompareTo(other.m_facts.Count)) != 0)
          return value;

        return this.SortedFacts.SequenceCompareTo(other.SortedFacts);
      }

      #endregion

    }

    #endregion

    #region Private class QualifiedHashSetFluentsContainer

    /// <summary>
    /// A qualified hashset fluents container holds fluents in a hashset.
    /// This implementation is fast for retrieving fluents' value, but comparison
    /// between two qualified hashset fluents container is slow because their respective
    /// elements must be sorted beforehand.
    /// Moreover it is a little slower than a regular hashset facts container,
    /// but has the (debugging) advantage of offering a decent string representation
    /// of the fluents.
    /// </summary>
    public class QualifiedHashSetFluentsContainer
    {
      #region Private Fields

      /// <summary>
      /// The dictionary of numeric fluents.
      /// </summary>
      private Dictionary<NumericFluentApplication, double> m_numericFluents;

      /// <summary>
      /// The cached list of sorted numeric fluents/values, necessary only for 
      /// comparisons with other containers.
      /// </summary>
      private List<KeyValuePair<NumericFluentApplication, double>> m_sortedNumericFluents;

      /// <summary>
      /// The dictionary of object fluents.
      /// </summary>
      private Dictionary<ObjectFluentApplication, Constant> m_objectFluents;

      /// <summary>
      /// The cached list of sorted object fluents IDs/values, necessary only for
      /// comparisons with other containers.
      /// </summary>
      private List<KeyValuePair<ObjectFluentApplication, Constant>> m_sortedObjectFluents;

      /// <summary>
      /// The hash code of this fluents container.
      /// </summary>
      private int m_hashCode;

      #endregion

      #region Properties

      /// <summary>
      /// The list of sorted numeric fluents/values, necessary only for 
      /// comparisons with other containers.
      /// </summary>
      protected List<KeyValuePair<NumericFluentApplication, double>> SortedNumericFluents
      {
        get
        {
          if (m_sortedNumericFluents == null)
          {
            m_sortedNumericFluents = m_numericFluents.ToList();
            m_sortedNumericFluents.Sort((x, y) => Utils.General.ComparePair(x, y));
          }

          return m_sortedNumericFluents;
        }
      }

      /// <summary>
      /// The list of sorted object fluents/values, necessary only for
      /// comparisons with other containers.
      /// </summary>
      protected List<KeyValuePair<ObjectFluentApplication, Constant>> SortedObjectFluents
      {
        get
        {
          if (m_sortedObjectFluents == null)
          {
            m_sortedObjectFluents = m_objectFluents.ToList();
            m_sortedObjectFluents.Sort((x, y) => Utils.General.ComparePair(x, y));
          }

          return m_sortedObjectFluents;
        }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a new empty qualified hashset fluents container.
      /// </summary>
      public QualifiedHashSetFluentsContainer()
      {
        this.m_numericFluents = new Dictionary<NumericFluentApplication, double>();
        this.m_sortedNumericFluents = null;
        this.m_objectFluents = new Dictionary<ObjectFluentApplication, Constant>();
        this.m_sortedObjectFluents = null;
        this.m_hashCode = 0;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Copies this fluent container.
      /// </summary>
      /// <returns>A copy of this fluent container.</returns>
      public QualifiedHashSetFluentsContainer Copy()
      {
        QualifiedHashSetFluentsContainer world = (QualifiedHashSetFluentsContainer)base.MemberwiseClone();
        // Do not clone internals, waste of memory? Copy on write?
        world.m_numericFluents = new Dictionary<NumericFluentApplication, double>(m_numericFluents);
        world.m_objectFluents = new Dictionary<ObjectFluentApplication, Constant>(m_objectFluents);
        world.m_sortedNumericFluents = null;
        world.m_sortedObjectFluents = null;
        return world;
      }

      #endregion

      #region IConstantWorld Interface

      /// <summary>
      /// Returns the value of the specified numeric fluent in this world.
      /// </summary>
      /// <param name="fluent">A described (and ground) numeric fluent.</param>
      /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
      public FuzzyDouble GetNumericFluent(NumericFluentApplication fluent)
      {
        double value;
        if (m_numericFluents.TryGetValue(fluent, out value))
          return new FuzzyDouble(value);
        else
          return FuzzyDouble.Unknown;
      }

      /// <summary>
      /// Returns the value of the specified object fluent in this world.
      /// </summary>
      /// <param name="fluent">A described (and ground) object fluent.</param>
      /// <returns>Unknown, undefined, or a constant representing the value of the 
      /// object fluent.</returns>
      public FuzzyConstantExp GetObjectFluent(ObjectFluentApplication fluent)
      {
        Constant value;
        if (m_objectFluents.TryGetValue(fluent, out value))
          return new FuzzyConstantExp(value);
        else
          return FuzzyConstantExp.Unknown;
      }

      #endregion

      #region IWorld Interface

      /// <summary>
      /// Sets the new value of the specified numeric fluent.
      /// </summary>
      /// <param name="fluent">A numeric fluent with constant arguments.</param>
      /// <param name="value">The new value of the numeric fluent.</param>
      public void SetNumericFluent(NumericFluentApplication fluent, double value)
      {
        double oldValue;
        if (m_numericFluents.TryGetValue(fluent, out oldValue))
        {
          m_hashCode -= (fluent.GetHashCode() * oldValue.GetHashCode());
        }
        m_numericFluents[fluent] = value;
        m_hashCode += (fluent.GetHashCode() * value.GetHashCode());
      }

      /// <summary>
      /// Sets the new value of the specified object fluent.
      /// </summary>
      /// <param name="fluent">A object fluent with constant arguments.</param>
      /// <param name="value">The constant representing the new value of the object fluent.
      /// </param>
      public void SetObjectFluent(ObjectFluentApplication fluent, Constant value)
      {
        Constant oldValue;
        if (m_objectFluents.TryGetValue(fluent, out oldValue))
        {
          m_hashCode -= (fluent.GetHashCode() * oldValue.GetHashCode());
        }
        m_objectFluents[fluent] = value;
        m_hashCode += (fluent.GetHashCode() * value.GetHashCode());
      }

      /// <summary>
      /// Sets the specified object fluent to undefined.
      /// </summary>
      /// <param name="fluent">A object fluent with constant arguments.</param>
      public void UndefineObjectFluent(ObjectFluentApplication fluent)
      {
        Constant oldValue;
        if (m_objectFluents.TryGetValue(fluent, out oldValue))
        {
          m_hashCode -= (fluent.GetHashCode() * oldValue.GetHashCode());
          m_objectFluents.Remove(fluent);
        }
      }

      #endregion

      #region Object Interface Overrides

      /// <summary>
      /// Returns whether this fluents container is equal to another fluents container.
      /// Two hashset fluent containers are equal if all their respective fluents have the
      /// same value pairwise.
      /// </summary>
      /// <param name="obj">The other object to test for equality.</param>
      /// <returns>Whether this fluents container is equal to the other object.</returns>
      public override bool Equals(object obj)
      {
        if (obj == this)
        {
          return true;
        }
        else
        {
          QualifiedHashSetFluentsContainer other = (QualifiedHashSetFluentsContainer)obj;
          return this.m_numericFluents.DictionaryEqual(other.m_numericFluents) &&
                 this.m_objectFluents.DictionaryEqual(other.m_objectFluents);
        }
      }

      /// <summary>
      /// Returns the hash code of this fluents container.
      /// </summary>
      /// <returns>The hash code of this fluents container.</returns>
      public override int GetHashCode()
      {
        return m_hashCode;
      }

      /// <summary>
      /// Returns a string representation of this fluents container.
      /// </summary>
      /// <returns>A string representation of this fluents container.</returns>
      public override string ToString()
      {
        return m_numericFluents.Aggregate("", (string s, KeyValuePair<NumericFluentApplication, double> p) => s + "(= " + p.Key + " " + p.Value + ") ") + "\n"
             + m_objectFluents.Aggregate("", (string s, KeyValuePair<ObjectFluentApplication, Constant> p) => s + "(= " + p.Key + " " + p.Value + ") ");
      }

      #endregion

      #region IComparable<OpenWorld> interface

      /// <summary>
      /// Compares this fluents container with another fluents container.
      /// Comparison is done first on the hash code, then on the number of defined fluents, and
      /// finally fluents are sorted and compared individually.
      /// </summary>
      /// <param name="other">The other fluents container to compare this fluents container to.</param>
      /// <returns>An integer representing the total order relation between the two fluents containers.
      /// </returns>
      public int CompareTo(QualifiedHashSetFluentsContainer other)
      {
        int value = this.GetHashCode().CompareTo(other.GetHashCode());
        if (value != 0)
          return value;

        // We do not compare worlds of different implementations... yet.

        if ((value = this.m_numericFluents.Count.CompareTo(other.m_numericFluents.Count)) != 0)
          return value;

        if ((value = this.m_objectFluents.Count.CompareTo(other.m_objectFluents.Count)) != 0)
          return value;

        if ((value = this.SortedNumericFluents.SequenceCompareTo(other.SortedNumericFluents)) != 0)
          return value;

        return this.SortedObjectFluents.SequenceCompareTo(other.SortedObjectFluents);
      }

      #endregion
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The facts container holds all facts.
    /// </summary>
    private QualifiedHashSetFactsContainer m_factsContainer;
    /// <summary>
    /// The fluents container holds all numeric and object fluents.
    /// </summary>
    private QualifiedHashSetFluentsContainer m_fluentsContainer;

    #endregion   

    #region Constructor

    /// <summary>
    /// Creates a new qualified world using the specified set of options.
    /// </summary>
    /// <param name="options">A set of TLPlan options.</param>
    public QualifiedWorld(TLPlanOptions options)
      : base(options)
    {
      this.m_factsContainer = new QualifiedHashSetFactsContainer();
      this.m_fluentsContainer = new QualifiedHashSetFluentsContainer();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies this qualified world.
    /// </summary>
    /// <returns>A copy of this qualified world.</returns>
    public override ExtendedOpenWorld Copy()
    {
      QualifiedWorld world = (QualifiedWorld)base.MemberwiseClone();
      // Do not clone internals, waste of memory? Copy on write?
      world.m_factsContainer = this.m_factsContainer.Copy();
      world.m_fluentsContainer = this.m_fluentsContainer.Copy();
      return world;
    }

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formula">A described (and ground) atomic formula.</param>
    /// <returns>True, false, or unknown.</returns>
    public override FuzzyBool IsSet(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      return this.m_factsContainer.IsSet(formula);
    }

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) numeric fluent.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public override FuzzyDouble InternalGetNumericFluent(NumericFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      return this.m_fluentsContainer.GetNumericFluent(fluent);
    }

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluent">A described (and ground) object fluent.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public override FuzzyConstantExp InternalGetObjectFluent(ObjectFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      return this.m_fluentsContainer.GetObjectFluent(fluent);
    }

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formula">An atomic formula with constant arguments.</param>
    public override void Set(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      this.m_factsContainer.Set(formula);
    }

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formula">A atomic formula with constant arguments.</param>
    public override void Unset(AtomicFormulaApplication formula)
    {
      System.Diagnostics.Debug.Assert(formula.AllConstantArguments);

      this.m_factsContainer.Unset(formula);
    }

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluent">A numeric fluent with constant arguments.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public override void SetNumericFluent(NumericFluentApplication fluent, double value)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.SetNumericFluent(fluent, value);
    }

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public override void SetObjectFluent(ObjectFluentApplication fluent, Constant value)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.SetObjectFluent(fluent, value);
    }

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluent">A object fluent with constant arguments.</param>
    public override void UndefineObjectFluent(ObjectFluentApplication fluent)
    {
      System.Diagnostics.Debug.Assert(fluent.AllConstantArguments);

      this.m_fluentsContainer.UndefineObjectFluent(fluent);
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this world is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this world is equal to the other object.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else
      {
        QualifiedWorld other = (QualifiedWorld)obj;
        return this.m_factsContainer.Equals(other.m_factsContainer) &&
               this.m_fluentsContainer.Equals(other.m_fluentsContainer);
      }
    }

    /// <summary>
    /// Returns the hash code of this world.
    /// </summary>
    /// <returns>The hash code of this world.</returns>
    public override int GetHashCode()
    {
      return this.m_factsContainer.GetHashCode() + this.m_fluentsContainer.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this qualified world.
    /// </summary>
    /// <returns>A string representation of this qualified world.</returns>
    public override string ToString()
    {
      return this.m_factsContainer.ToString() + " " + this.m_fluentsContainer.ToString();
    }

    #endregion

    #region IComparable<OpenWorld> interface

    /// <summary>
    /// Compares this world with another world.
    /// </summary>
    /// <param name="world">The other world to compare this world to.</param>
    /// <returns>An integer representing the total order relation between the two worlds.
    /// </returns>
    public override int CompareTo(ExtendedOpenWorld world)
    {
      QualifiedWorld other = (QualifiedWorld)world;

      int value = this.GetHashCode().CompareTo(other.GetHashCode());
      if (value != 0)
        return value;

      // We do not compare worlds of different implementations... yet.

      if ((value = this.m_factsContainer.CompareTo(other.m_factsContainer)) != 0)
        return value;

      return this.m_fluentsContainer.CompareTo(other.m_fluentsContainer);
    }

    #endregion
  }
}
