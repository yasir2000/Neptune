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
using PDDLParser.Exception;

namespace PDDLParser.Exp.Term.Type
{
  /// <summary>
  /// Represents a single, primitive PDDL type. Note that only one instance of 
  /// <see cref="PDDLParser.Exp.Term.Type.Type"/> is to exists for each PDDL 
  /// primitive type.
  /// </summary>
  public sealed class Type
  {
    #region Private Fields

    /// <summary>
    /// The name of this type.
    /// </summary>
    private string m_name;

    /// <summary>
    /// The domain of this type.
    /// </summary>
    private HashSet<Constant> m_domain;

    /// <summary>
    /// Direct subtypes of this type.
    /// </summary>
    private HashSet<Type> m_subTypes;
    /// <summary>
    /// All direct and indirect subtypes of this type.
    /// </summary>
    private HashSet<Type> m_allSubTypes;

    /// <summary>
    /// Direct supertypes of this type.
    /// </summary>
    private HashSet<Type> m_superTypes;
    /// <summary>
    /// All direct and indirect supertypes of this type.
    /// </summary>
    private HashSet<Type> m_allSuperTypes;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of this type.
    /// </summary>
    public string Name { get { return this.m_name; } }

    /// <summary>
    /// Gets or sets this type's domain.
    /// </summary>
    /// <remarks>
    /// The type's domain contains its own domain as well as all of its subtypes' domains.
    /// Also note that when setting the type's domain, an event is triggered to warn other objects
    /// that they will have to clear their domain cache.
    /// </remarks>
    /// <seealso cref="PDDLParser.Exp.Term.Type.TypeSet"/>
    public HashSet<Constant> TypeDomain
    {
      get
      {
        HashSet<Constant> domain = new HashSet<Constant>(m_domain);
        return SubTypes.Aggregate<Type, HashSet<Constant>>(domain, (dom, type) => { dom.UnionWith(type.TypeDomain); return dom; });
      }
      internal set
      {
        if (!m_domain.SetEquals(value))
        {
          m_domain = value;
          FireTypeDomainChanged();
        }
      }
    }

    /// <summary>
    /// Gets the direct subtypes of this type.
    /// </summary>
    public HashSet<Type> SubTypes { get { return m_subTypes; } }
    /// <summary>
    /// Gets all direct and indirect supertypes of this type.
    /// </summary>
    public HashSet<Type> AllSubTypes { get { return m_allSubTypes; } }

    /// <summary>
    /// Gets the direct supertypes of this type.
    /// </summary>
    public HashSet<Type> SuperTypes { get { return m_superTypes; } }
    /// <summary>
    /// Gets all direct and indirect supertypes of this type.
    /// </summary>
    public HashSet<Type> AllSuperTypes { get { return m_allSuperTypes; } }

    #endregion

    #region Events and Delegates

    /// <summary>
    /// A delegate used with the <see cref="TypeDomainChanged"/> event.
    /// </summary>
    /// <param name="sender">The type whose domain has changed.</param>
    public delegate void TypeEventHandler(Type sender);

    /// <summary>
    /// This event is triggered whenever the type's domain changes. This is used to clear cached domains.
    /// </summary>
    /// <seealso cref="PDDLParser.Exp.Term.Type.TypeSet"/>
    public event TypeEventHandler TypeDomainChanged;

    #endregion

    #region Static Members

    /// <summary>
    /// The object type symbol.
    /// </summary>
    public static readonly string OBJECT_SYMBOL = "object";

    /// <summary>
    /// The number type symbol.
    /// </summary>
    public static readonly string NUMBER_SYMBOL = "number";

    /// <summary>
    /// The number primitive type.
    /// </summary>
    public static readonly Type Number = new Type(Type.NUMBER_SYMBOL);

    /// <summary>
    /// Creates a hierarchy of types based on a dictionary that maps types to their direct subtypes.
    /// </summary>
    /// <param name="hierarchy">The dictionary that maps types to their direct subtypes.</param>
    /// <returns>A hierarchy of instantiated types.</returns>
    /// <exception cref="TypingException">Thrown when a cycle is detected in the type hierarchy, or when another error occurs.</exception>
    public static IDictionary<string, Type> CreateTypes(IDictionary<string, HashSet<string>> hierarchy)
    {
      IDictionary<string, Type> types = new Dictionary<string, Type>();

      // First, create the types...
      foreach (string strType in hierarchy.Keys)
      {
        //if (strType == Type.OBJECT_SYMBOL)
        //  continue;

        Type type;
        if (!types.TryGetValue(strType, out type))
          types[strType] = new Type(strType);
        else
          throw new TypingException(type);
      }

      // Put in the number type (or replace it if it existed)
      types[Type.NUMBER_SYMBOL] = Type.Number;

      // Then, add subtypes and supertypes
      foreach (KeyValuePair<string, HashSet<string>> pair in hierarchy)
      {
        Type type = types[pair.Key];
        foreach (string strSubType in pair.Value)
        {
          Type subType;
          if (types.TryGetValue(strSubType, out subType))
          {
            type.AddSubType(subType);
            subType.AddSuperType(type);
          }
          else
            throw new TypingException(subType);
        }
      }

      Type objectType = types[Type.OBJECT_SYMBOL];

      // Add the "object" type as supertype where it needs to be and fill in its subtypes
      foreach (KeyValuePair<string, Type> pair in types)
      {
        if (pair.Key != Type.OBJECT_SYMBOL && pair.Key != Type.NUMBER_SYMBOL)
        {
          if (pair.Value.m_superTypes.Count == 0)
          {
            objectType.AddSubType(pair.Value);
            pair.Value.AddSuperType(objectType);
          }
        }
      }

      // Check for cycles in the type hierarchy
      foreach (Type type in types.Values)
      {
        type.VerifyCycles(new HashSet<Type>());
      }

      // Finally, cache all sub- and supertypes
      objectType.CacheSubTypesAndSuperTypes(new HashSet<Type>(), new HashSet<Type>());

      return types;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new primitive type with a given name.
    /// </summary>
    /// <param name="name">The name of the primitive type.</param>
    private Type(string name)
    {
      this.m_name = name;
      this.m_domain = new HashSet<Constant>();
      this.m_subTypes = new HashSet<Type>();
      this.m_allSubTypes = new HashSet<Type>();
      this.m_superTypes = new HashSet<Type>();
      this.m_allSuperTypes = new HashSet<Type>();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a given direct subtype to this type.
    /// </summary>
    /// <param name="type">The direct subtype.</param>
    private void AddSubType(Type type)
    {
      m_subTypes.Add(type);
      m_allSubTypes.Add(type);
    }

    /// <summary>
    /// Adds a given indirect subtype to this type.
    /// </summary>
    /// <param name="type">The indirect subtype.</param>
    private void AddIndirectSubType(Type type)
    {
      m_allSubTypes.Add(type);
    }

    /// <summary>
    /// Adds a given direct supertype to this type.
    /// </summary>
    /// <param name="type">The direct supertype.</param>
    private void AddSuperType(Type type)
    {
      m_superTypes.Add(type);
      m_allSuperTypes.Add(type);
    }

    /// <summary>
    /// Adds a given indirect supertype to this type.
    /// </summary>
    /// <param name="type">The indirect supertype.</param>
    private void AddIndirectSuperType(Type type)
    {
      m_allSuperTypes.Add(type);
    }

    /// <summary>
    /// Verifies whether a cycle exists in the type hierarchy.
    /// </summary>
    /// <param name="encounteredTypes">The type hierarchy.</param>
    /// <exception cref="TypingException">Thrown if there is a cycle in the type hierarchy.</exception>
    private void VerifyCycles(HashSet<Type> encounteredTypes)
    {
      if (encounteredTypes.Contains(this))
        throw new TypingException(this);

      HashSet<Type> allTypes = new HashSet<Type>(encounteredTypes);
      allTypes.Add(this);

      foreach (Type subType in m_subTypes)
      {
        subType.VerifyCycles(allTypes);
      }
    }

    /// <summary>
    /// Caches all the supertype and subtypes. This is called recursively.
    /// </summary>
    /// <param name="subTypes">The subtypes encountered by the callee.</param>
    /// <param name="superTypes">The supertypes encountered by the caller.</param>
    private void CacheSubTypesAndSuperTypes(HashSet<Type> subTypes, HashSet<Type> superTypes)
    {
      m_allSuperTypes.UnionWith(superTypes);

      HashSet<Type> newSuperTypes = new HashSet<Type>(superTypes);
      newSuperTypes.Add(this);

      foreach (Type subType in m_subTypes)
      {
        HashSet<Type> newSubTypes = new HashSet<Type>();
        subType.CacheSubTypesAndSuperTypes(newSubTypes, newSuperTypes);
        m_allSubTypes.UnionWith(newSubTypes);
      }

      m_allSubTypes.UnionWith(m_subTypes);

      subTypes.UnionWith(m_allSubTypes);
    }

    #region Fire Events

    /// <summary>
    /// Fires the <see cref="TypeDomainChanged"/> event, setting this type as its parameter.
    /// </summary>
    private void FireTypeDomainChanged()
    {
      if (TypeDomainChanged != null)
        TypeDomainChanged(this);
    }

    #endregion

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns true if this primitive type is equal to a specified object.
    /// Two primitive types are equal if they have the same name.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this primitive type is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj.GetType() == this.GetType())
      {
        Type other = (Type)obj;
        return this.m_name.Equals(other.m_name);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code of this primitive type.
    /// </summary>
    /// <returns>The hash code of this primitive type.</returns>
    public override int GetHashCode()
    {
      return this.m_name.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of primitive type.
    /// </summary>
    /// <returns>A string representation of primitive type.</returns>
    public override string ToString()
    {
      return this.m_name;
    }

    #endregion
  }

}