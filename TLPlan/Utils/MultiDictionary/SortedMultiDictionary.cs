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
using TLPlan.Utils.Set;

namespace TLPlan.Utils.MultiDictionary
{
  /// <summary>
  /// Sorted multi-dictionary implemented as a sorted dictionary.
  /// </summary>
  /// <typeparam name="Key">The key type.</typeparam>
  /// <typeparam name="Value">The value type.</typeparam>
  public class SortedMultiDictionary<Key, Value> : AbstractMultiDictionary<Key, Value>
  {
    /// <summary>
    /// Items are retrieved using the sorted dictionary.
    /// </summary>
    private SortedDictionary<Key, LinkedList<Value>> m_items = new SortedDictionary<Key, LinkedList<Value>>();

    /// <summary>
    /// Creates a new empty sorted multi-dictionary.
    /// </summary>
    public SortedMultiDictionary()
      : base()
    {
    }

    /// <summary>
    /// Returns the all the items (sorted by key) present in the multi-dictionary.
    /// </summary>
    protected override IDictionary<Key, LinkedList<Value>> Items
    {
      get { return m_items; }
    }

    /// <summary>
    /// Removes the first item present in the dictionary.
    /// </summary>
    /// <returns>The first item which was present in the dictionary.</returns>
    public KeyValuePair<Key, Value> RemoveFirst()
    {
      foreach (KeyValuePair<Key, LinkedList<Value>> pair in m_items)
      {
        if (pair.Value.Count != 0)
        {
          Value value = pair.Value.First.Value;
          pair.Value.RemoveFirst();
          KeyValuePair<Key, Value> first = new KeyValuePair<Key, Value>(pair.Key, value);
          if (pair.Value.Count == 0)
          {
            Items.Remove(pair.Key);
          }
          this.m_itemCount--;
          return first;
        }
      }
      throw new System.Exception("No items left in dictionary!");
    }
  }
}
