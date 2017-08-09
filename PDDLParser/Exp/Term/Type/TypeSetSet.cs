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

namespace PDDLParser.Exp.Term.Type
{
  /// <summary>
  /// Represents a set of typesets. A single instance of this class contains all the types defined
  /// in a full problem (i.e. a parsed and linked domain/problem pair).
  /// </summary>
  public class TypeSetSet : IEnumerable<TypeSet>
  {
    #region Private Fields

    /// <summary>
    /// All existing types.
    /// </summary>
    private IDictionary<string, Type> m_types;
    /// <summary>
    /// All existing typesets.
    /// </summary>
    private IDictionary<string, TypeSet> m_typeSets;

    /// <summary>
    /// All existing domain constants.
    /// </summary>
    /// <remarks>
    /// Domain constants are not cleared from the types' domains when reseting the <see cref="TypeSetSet"/>
    /// using <see cref="TypeSetSet.ClearAllButDomainConstants"/>.
    /// </remarks>
    private List<Constant> m_domainConstants;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the Object typeset.
    /// </summary>
    public TypeSet Object { get { return GetTypeSet(Type.OBJECT_SYMBOL); } }
    /// <summary>
    /// Gets the Numerbe typeset.
    /// </summary>
    public TypeSet Number { get { return GetTypeSet(Type.NUMBER_SYMBOL); } }

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new empty set of typesets, useful for untyped domains.
    /// </summary>
    public TypeSetSet()
    {
      // Create only the object type (along with the number type)
      Dictionary<string, HashSet<string>> hierarchy = new Dictionary<string, HashSet<string>>();
      hierarchy[Type.OBJECT_SYMBOL] = new HashSet<string>();

      m_types = Type.CreateTypes(hierarchy);
      m_typeSets = TypeSet.CreateTypeSets(m_types);

      m_domainConstants = new List<Constant>();
    }

    /// <summary>
    /// Create a new set of typesets based on a given type hierarchy.
    /// </summary>
    /// <param name="hierarchy">The type hierarchy.</param>
    public TypeSetSet(IDictionary<string, HashSet<string>> hierarchy)
    {
      System.Diagnostics.Debug.Assert(hierarchy != null);

      m_types = Type.CreateTypes(hierarchy);
      m_typeSets = TypeSet.CreateTypeSets(m_types);

      m_domainConstants = new List<Constant>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets a typeset from its name. This is used to get typesets containing
    /// only a single primitive type.
    /// </summary>
    /// <param name="typeName">The name of the type (and typeset).</param>
    /// <returns>The typeset whose name was given.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the requested typeset does not exist.</exception>
    public TypeSet GetTypeSet(string typeName)
    {
      return m_typeSets[typeName];
    }

    /// <summary>
    /// Gets a typeset from the names of all the types it contains. This is used to get
    /// typesets containing combined types (i.e. (either typeX typeY)).
    /// </summary>
    /// <remarks>
    /// If the requested combination of types does not yet exist as a typeset, it is created.
    /// </remarks>
    /// <param name="typeNames">The list of type names.</param>
    /// <returns>The corresponding typeset.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when one of the requested primitive type does not exist.</exception>
    public TypeSet GetTypeSet(HashSet<string> typeNames)
    {
      List<string> sortedNames = new List<string>(typeNames);
      sortedNames.Sort();

      string typeKey = string.Join(" ", sortedNames.ToArray());

      TypeSet typeSet;
      if (!m_typeSets.TryGetValue(typeKey, out typeSet))
      {
        typeSet = new TypeSet();
        foreach (string typeName in typeNames)
        {
          TypeSet subTypeSet = m_typeSets[typeName];
          typeSet.Merge(subTypeSet);
        }

        m_typeSets[typeKey] = typeSet;
      }

      return typeSet;
    }

    /// <summary>
    /// Returns whether a given type exists.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <returns>True if the type exists; false otherwise.</returns>
    public bool TypeExists(string typeName)
    {
      return m_typeSets.ContainsKey(typeName);
    }

    /// <summary>
    /// Flag the given constants as domain constants.
    /// </summary>
    /// <remarks>
    /// See <see cref="TypeSetSet.m_domainConstants"/> and <see cref="TypeSetSet.ClearAllButDomainConstants"/>
    /// for more information about domains constants.
    /// </remarks>
    /// <param name="domainConstants">The constants to be flagged as domain constants.</param>
    /// <seealso cref="TypeSetSet.m_domainConstants"/>
    /// <seealso cref="TypeSetSet.ClearAllButDomainConstants"/>
    public void FlagAsDomainConstants(IEnumerable<Constant> domainConstants)
    {
      m_domainConstants = new List<Constant>(domainConstants);
    }

    /// <summary>
    /// Clears all types' domains, but keeps the domain constants, as they do not change when a different problem is
    /// parsed and linked with the same domain.
    /// </summary>
    /// <seealso cref="TypeSetSet.FlagAsDomainConstants"/>
    /// <seealso cref="TypeSetSet.m_domainConstants"/>
    public void ClearAllButDomainConstants()
    {
      // Clear all type domains.
      IEnumerable<Type> types = Enumerable.Empty<Type>();
      foreach (TypeSet typeSet in this)
        types = types.Concat(typeSet);

      foreach (Type type in types)
        type.TypeDomain = new HashSet<Constant>();

      // Add domain constants
      IDictionary<Type, HashSet<Constant>> typeDomains = new Dictionary<Type, HashSet<Constant>>();

      foreach (Constant cst in m_domainConstants)
      {
        foreach (Type type in cst.GetTypeSet())
        {
          HashSet<Constant> setCst;
          if (!typeDomains.TryGetValue(type, out setCst))
          {
            setCst = new HashSet<Constant>();
            typeDomains[type] = setCst;
          }
          setCst.Add(cst);
        }
      }

      foreach (KeyValuePair<Type, HashSet<Constant>> typeDomain in typeDomains)
      {
        typeDomain.Key.TypeDomain = typeDomain.Value;
      }
    }

    #endregion

    #region IEnumerable<TypeSet> Interface

    /// <summary>
    /// Returns an enumerator over the typesets forming this set of typeset.
    /// </summary>
    /// <returns>An enumerator over the typesets forming this set of typeset.</returns>
    public IEnumerator<TypeSet> GetEnumerator()
    {
      return m_typeSets.Values.GetEnumerator();
    }

    #endregion

    #region IEnumerable Interface

    /// <summary>
    /// Returns an enumerator over the typesets forming this set of typeset.
    /// </summary>
    /// <returns>An enumerator over the typesets forming this set of typeset.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<TypeSet>)this).GetEnumerator();
    }

    #endregion
  }
}
