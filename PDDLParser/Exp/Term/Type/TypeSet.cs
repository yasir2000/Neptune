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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PDDLParser.Exp.Term.Type
{
  /// <summary>
  /// Represents a set of primitives types. This set can contain one or multiple types.
  /// </summary>
  /// <remarks>
  /// All primitive types (see <see cref="PDDLParser.Exp.Term.Type.Type"/>) 
  /// have their own <see cref="PDDLParser.Exp.Term.Type.TypeSet"/>.
  /// Typesets containing more than one type can be constructed using the <c>either</c> PDDL keyword.
  /// </remarks>
  public class TypeSet : IEnumerable<Type>
  {
    #region Private Fields

    /// <summary>
    /// The set on types in this typeset.
    /// </summary>
    private HashSet<Type> m_types;
    /// <summary>
    /// The direct subtypes of the types contained in this typeset.
    /// </summary>
    private HashSet<Type> m_subTypes;
    /// <summary>
    /// The direct supertypes of the types contained in this typeset.
    /// </summary>
    private HashSet<Type> m_superTypes;
    /// <summary>
    /// All the direct and indirect subtypes of the types contained in this typeset.
    /// </summary>
    private HashSet<Type> m_allSubTypes;
    /// <summary>
    /// All the direct and indirect supertypes of the types contained in this typeset.
    /// </summary>
    private HashSet<Type> m_allSuperTypes;
    /// <summary>
    /// This typeset's domain cache.
    /// </summary>
    private List<Constant> m_domainCache;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of this typeset.
    /// </summary>
    public string Name
    {
      get
      {
        if (m_types.Count == 1)
          return m_types.First().Name;
        else
          return string.Format("(either {0})", string.Join(" ", m_types.Select(type => type.Name).ToArray()));
      }
    }

    /// <summary>
    /// Gets the direct subtypes of the types contained in this typeset.
    /// </summary>
    public HashSet<Type> SubTypes { get { return m_subTypes; } }
    /// <summary>
    /// Gets the direct supertypes of the types contained in this typeset.
    /// </summary>
    public HashSet<Type> SuperTypes { get { return m_superTypes; } }
    /// <summary>
    /// Gets all the direct and indirect subtypes of the types contained in this typeset.
    /// </summary>
    public HashSet<Type> AllSubTypes { get { return m_allSubTypes; } }
    /// <summary>
    /// Gets all the direct and indirect supertypes of the types contained in this typeset.
    /// </summary>
    public HashSet<Type> AllSuperTypes { get { return m_allSuperTypes; } }

    /// <summary>
    /// Retrieves this typeset's domain.
    /// </summary>
    /// <remarks>
    /// A call to <see cref="Preprocess"/> is required to cache the typeset's domain.
    /// </remarks>
    public List<Constant> Domain
    {
      get
      {
        System.Diagnostics.Debug.Assert(m_domainCache != null, "Before fetching the domain (and after modifying type domains), call Preprocess() !");

        return m_domainCache;
      }
    }

    #endregion

    #region Events and Delegates

    /// <summary>
    /// A delegate used with the <see cref="TypeSetEventHandler"/> event.
    /// </summary>
    /// <param name="sender">Teh typeset whose domain has changed.</param>
    public delegate void TypeSetEventHandler(TypeSet sender);

    /// <summary>
    /// This event is triggered whenever the typeset's domain changes. This is used to clear cached domains.
    /// </summary>
    /// <seealso cref="PDDLParser.Exp.Term.Type.Type"/>
    public event TypeSetEventHandler TypeDomainChanged;

    #endregion

    #region Static Members

    /// <summary>
    /// Creates the original typesets containing only a single primitive type from the hierarchy of primitive types.
    /// </summary>
    /// <param name="types">The hierarchy of primitive types.</param>
    /// <returns>A dictionary that maps typeset names to their corresponding typeset.</returns>
    public static IDictionary<string, TypeSet> CreateTypeSets(IDictionary<string, Type> types)
    {
      IDictionary<string, TypeSet> typeSets = new Dictionary<string, TypeSet>();

      foreach (Type type in types.Values)
      {
        typeSets[type.Name] = new TypeSet(type);
      }

      return typeSets;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new typeset, containing only the given type.
    /// </summary>
    /// <param name="type">The single type in the typeset.</param>
    private TypeSet(Type type)
      : this()
    {
      System.Diagnostics.Debug.Assert(type != null);

      this.m_types.Add(type);
      this.m_subTypes.UnionWith(type.SubTypes);
      this.m_superTypes.UnionWith(type.SuperTypes);
      this.m_allSubTypes.UnionWith(type.AllSubTypes);
      this.m_allSuperTypes.UnionWith(type.AllSuperTypes);

      type.TypeDomainChanged += new Type.TypeEventHandler(Type_TypeDomainChanged);
    }

    /// <summary>
    /// Creates an empty typeset.
    /// </summary>
    /// <remarks>
    /// This is only to be used by <see cref="TypeSet"/> and <see cref="TypeSetSet"/>.
    /// Unfortunately, there was no way in C# to restrain the use of this method to those classes.
    /// </remarks>
    internal TypeSet()
    {
      this.m_types = new HashSet<Type>();
      this.m_subTypes = new HashSet<Type>();
      this.m_superTypes = new HashSet<Type>();
      this.m_allSubTypes = new HashSet<Type>();
      this.m_allSuperTypes = new HashSet<Type>();
      this.m_domainCache = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether this typeset can be assigned from another typeset.
    /// </summary>
    /// <remarks>
    /// A typeset is considered assignable from another one if the set of all its direct and indirect
    /// subtypes, along with the typeset itself, is a superset of the set of all the other typeset's
    /// direct and indirect subtypes.
    /// </remarks>
    /// <param name="other">The typeset from which we want to assign to this one.</param>
    /// <returns>True if this typeset can be assigned from the other typeset; false otherwise.</returns>
    public virtual bool CanBeAssignedFrom(TypeSet other)
    {
      if (other is UndefinedTypeSet)
        return true; // Special case: Undefined constant

      HashSet<Type> thisType = new HashSet<Type>(this.AllSubTypes);
      HashSet<Type> otherType = new HashSet<Type>(other.AllSubTypes);
      thisType.UnionWith(this);
      otherType.UnionWith(other);

      return thisType.IsSupersetOf(otherType);
    }

    /// <summary>
    /// Returns whether this typeset is comparable to another typeset.
    /// </summary>
    /// <remarks>
    /// A typeset is considered comparable to another one if the set of all its direct and indirect
    /// subtypes, along with the typeset itself, overlaps the set of all the other typeset's
    /// direct and indirect subtypes.
    /// </remarks>
    /// <param name="other">The other typeset to which this one is to be compared.</param>
    /// <returns>True if this typeset is comparable to the other typeset; false otherwise.</returns>
    public virtual bool IsComparableTo(TypeSet other)
    {
      if (other is UndefinedTypeSet)
        return false; // Special case: Undefined constant

      HashSet<Type> thisType = new HashSet<Type>(this.AllSubTypes);
      HashSet<Type> otherType = new HashSet<Type>(other.AllSubTypes);
      thisType.UnionWith(this);
      otherType.UnionWith(other);

      thisType.IntersectWith(otherType);

      return thisType.Count != 0;
    }

    /// <summary>
    /// Preprocesses this typeset, creating the domain cache and updating its constants IDs.
    /// </summary>
    /// <remarks>
    /// This has to be called before any call to <see cref="Domain"/> is made.
    /// </remarks>
    public void Preprocess()
    {
      int constantID = 0;
      m_domainCache = new List<Constant>();

      // Fetch the typeset's domain
      HashSet<Constant> domain = new HashSet<Constant>();
      foreach (Type type in this)
        domain.UnionWith(type.TypeDomain);

      // Assign constant IDs, and insert the constant into the cache (the cache will therefore be sorted).
      foreach (Constant constant in domain)
      {
        constant.SetConstantID(this, constantID++);
        m_domainCache.Add(constant);
      }
    }

    /// <summary>
    /// Returns the <see cref="PDDLParser.Exp.Term.Constant"/> whose ID is the given one.
    /// </summary>
    /// <param name="constantID">The constant ID.</param>
    /// <returns>The <see cref="PDDLParser.Exp.Term.Constant"/> corresponding to the ID.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the ID does not
    /// correspond to any <see cref="PDDLParser.Exp.Term.Constant"/>.</exception>
    public Constant GetConstant(int constantID)
    {
      return Domain[constantID];
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Merges this typeset with another one, invalidating the domain cache.
    /// </summary>
    /// <remarks>
    /// This is only to be used by <see cref="TypeSetSet"/>.
    /// Unfortunately, there was no way in C# to restrain the use of this method to the aforementionned class.
    /// </remarks>
    /// <param name="typeSet">The other typeset which is to be merged in this one.</param>
    internal void Merge(TypeSet typeSet)
    {
      this.m_types.UnionWith(typeSet.m_types);
      this.m_subTypes.UnionWith(typeSet.SubTypes);
      this.m_superTypes.UnionWith(typeSet.SuperTypes);
      this.m_allSubTypes.UnionWith(typeSet.m_allSubTypes);
      this.m_allSuperTypes.UnionWith(typeSet.m_allSuperTypes);

      this.m_domainCache = null;

      typeSet.TypeDomainChanged += new TypeSetEventHandler(TypeSet_TypeDomainChanged);
    }

    #region Event Handlers

    /// <summary>
    /// This is the event handler for the <see cref="Type.TypeDomainChanged"/> event.
    /// This invalidates the domain cache and forwards the event to registered objects.
    /// </summary>
    /// <param name="type">The primitive type whose domain has changed.</param>
    private void Type_TypeDomainChanged(Type type)
    {
      this.m_domainCache = null;
      FireTypeDomainChanged();
    }

    /// <summary>
    /// This is the event handler for the <see cref="TypeSet.TypeDomainChanged"/> event.
    /// This invalidates the domain cache and forwards the event to registered objects.
    /// </summary>
    /// <param name="typeset">The typeset whose domain has changed.</param>
    private void TypeSet_TypeDomainChanged(TypeSet typeset)
    {
      this.m_domainCache = null;
      FireTypeDomainChanged();
    }

    #endregion

    #region Fire Events

    /// <summary>
    /// Fires the <see cref="TypeDomainChanged"/> event, using this typeset as the argument.
    /// </summary>
    private void FireTypeDomainChanged()
    {
      if (TypeDomainChanged != null)
        TypeDomainChanged(this);
    }

    #endregion

    #endregion

    #region IEnumerable Interfaces

    /// <summary>
    /// Returns an enumerator over the primitive types forming this typeset.
    /// </summary>
    /// <returns>An enumerator over the primitive types forming this typeset.</returns>
    public IEnumerator<Type> GetEnumerator()
    {
      return this.m_types.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator over the primitive types forming this typeset.
    /// </summary>
    /// <returns>An enumerator over the primitive types forming this typeset.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Type>)this).GetEnumerator();
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns true if this typeset is equal to a specified object.
    /// Two typesets are equal if they contain the same types.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this typeset is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      return (this == obj);
    }

    /// <summary>
    /// Returns the hash code of this typeset.
    /// </summary>
    /// <returns>The hash code of this typeset.</returns>
    public override int GetHashCode()
    {
      return this.m_types.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of typeset.
    /// </summary>
    /// <returns>A string representation of typeset.</returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.m_types.Count == 0)
      {
        str.Append("empty-type");
      }
      else if (this.m_types.Count == 1)
      {
        IEnumerator<Type> e = this.m_types.GetEnumerator();
        e.MoveNext();
        str.Append(e.Current.ToString());
      }
      else
      {
        str.Append("(either");
        foreach (Type t in this.m_types)
        {
          str.Append(" " + t.ToString());
        }
        str.Append(")");
      }
      return str.ToString();
    }

    #endregion
  }

  /// <summary>
  /// Represents the <see cref="TypeSet"/> of the <c>Undefined</c> constant.
  /// </summary>
  /// <remarks>
  /// The <see cref="UndefinedTypeSet"/> class was created so that the <see cref="TypeSet.CanBeAssignedFrom"/>
  /// and <see cref="TypeSet.IsComparableTo"/> methods can work properly with <c>Undefined</c> values.
  /// </remarks>
  public sealed class UndefinedTypeSet : TypeSet
  {
    #region Static Members

    /// <summary>
    /// This is the only instance of <see cref="UndefinedTypeSet"/>.
    /// </summary>
    private static UndefinedTypeSet s_instance;

    /// <summary>
    /// Gets the only instance of <see cref="UndefinedTypeSet"/>.
    /// </summary>
    public static UndefinedTypeSet Instance { get { return s_instance; } }

    /// <summary>
    /// Initializes the static fields of this class.
    /// </summary>
    static UndefinedTypeSet()
    {
      s_instance = new UndefinedTypeSet();
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty typeset, representing the <c>Undefined</c> constant.
    /// </summary>
    private UndefinedTypeSet()
      : base()
    { }

    #endregion

    #region TypeSet Interface Overrides

    /// <summary>
    /// Returns whether this typeset can be assigned from another typeset.
    /// </summary>
    /// <param name="other">The typeset from which we want to assign to this one.</param>
    /// <returns>False.</returns>
    /// <seealso cref="TypeSet.CanBeAssignedFrom"/>
    public override bool CanBeAssignedFrom(TypeSet other)
    {
      return false;
    }

    /// <summary>
    /// Returns whether this typeset is comparable to another typeset.
    /// </summary>
    /// <param name="other">The other typeset to which this one is to be compared.</param>
    /// <returns>False.</returns>
    /// <seealso cref="TypeSet.IsComparableTo"/>
    public override bool IsComparableTo(TypeSet other)
    {
      return false;
    }

    #endregion
  }
}