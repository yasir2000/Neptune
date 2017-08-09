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

namespace TLPlan.Utils.Set
{
  /// <summary>
  /// Set implemented as a dictionary (hash-table).
  /// </summary>
  /// <typeparam name="Value">The type of the elements.</typeparam>
  public class Set<Value> : AbstractSet<Value>
  {
    /// <summary>
    /// Internal set implemented as a dictionary (hash-table).
    /// </summary>
    private Dictionary<Value, Value> m_items;

    /// <summary>
    /// Creates a new empty set.
    /// </summary>
    public Set()
      : base()
    {
      m_items = new Dictionary<Value, Value>();
    }

    /// <summary>
    /// Creates a new set from an existing set.
    /// </summary>
    /// <param name="other">The other set to retrieve the elements from.</param>
    public Set(Set<Value> other)
    {
      m_items = new Dictionary<Value, Value>(other.m_items);
    }

    /// <summary>
    /// The internal set is implemented as a dictionary of values mapping to themselves.
    /// </summary>
    protected override IDictionary<Value, Value> Items
    {
      get
      {
        return m_items;
      }
    }
  }
}
