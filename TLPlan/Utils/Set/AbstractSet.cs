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
  /// Base class for set implementation.
  /// </summary>
  /// <typeparam name="Value">The type of the elements.</typeparam>
  public abstract class AbstractSet<Value> : ISet<Value>
  {
    /// <summary>
    /// The internal set is implemented as a dictionary of values mapping to themselves.
    /// </summary>
    protected abstract IDictionary<Value, Value> Items
    {
      get;
    }

    /// <summary>
    /// Creates a new empty set
    /// </summary>
    public AbstractSet()
    {
    }

    /// <summary>
    /// Adds a new item in the set.
    /// </summary>
    /// <param name="item">The item to add to the set.</param>
    public void Add(Value item)
    {
      Items[item] = item;
    }

    /// <summary>
    /// Removes an item from the set.
    /// </summary>
    /// <param name="item">The item to remove from the set.</param>
    /// <returns>Whether the item was present in the set.</returns>
    public bool Remove(Value item)
    {
      return Items.Remove(item);
    }

    /// <summary>
    /// Determines whether the set contains the specified item.
    /// </summary>
    /// <param name="item">The object to locate in the set.</param>
    /// <returns>Whether the set contains the specified item.</returns>
    public bool Contains(Value item)
    {
      return Items.ContainsKey(item);
    }

    /// <summary>
    /// Attempts to retrieve an item present in the set from an equivalent item.
    /// This function returns true if an existing item was found in the set.
    /// </summary>
    /// <param name="item">The item used as key.</param>
    /// <param name="oldItem">The item present in the set.</param>
    /// <returns>True if an existing item was found in the set.</returns>
    public bool TryGetValue(Value item, out Value oldItem)
    {
      return Items.TryGetValue(item, out oldItem);
    }

    /// <summary>
    /// Returns the number of elements in the set.
    /// </summary>
    public int Count { get { return Items.Count; } }

    #region IEnumerable<Value> Members

    /// <summary>
    /// Returns an enumerator over all the elements contained in the set.
    /// </summary>
    /// <returns>An enumerator over all the elements contained in the set.</returns>
    public IEnumerator<Value> GetEnumerator()
    {
      return Items.Keys.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator over all the elements contained in the set.
    /// </summary>
    /// <returns>An enumerator over all the elements contained in the set.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return Items.Keys.GetEnumerator();
    }

    #endregion
  }
}
