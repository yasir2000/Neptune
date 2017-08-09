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
  /// A set stores elements without duplicates.
  /// Contrary to a regular set, it allows one to retrieve the item present in the set
  /// from an equal item (using TryGetValue).
  /// </summary>
  /// <typeparam name="Value">The type of the elements.</typeparam>
  public interface ISet<Value> : IEnumerable<Value>
  {
    /// <summary>
    /// Adds a new item in the set.
    /// </summary>
    /// <param name="item">The item to add to the set.</param>
    /// <remarks>This call should simply overwrite the existing item if an equal item exists.
    /// No exception should be thrown.</remarks>
    void Add(Value item);

    /// <summary>
    /// Removes an item from the set.
    /// </summary>
    /// <param name="item">The item to remove from the set.</param>
    /// <returns>Whether the item was present in the set.</returns>
    bool Remove(Value item);

    /// <summary>
    /// Determines whether the set contains the specified item.
    /// </summary>
    /// <param name="item">The object to locate in the set.</param>
    /// <returns>Whether the set contains the specified item.</returns>
    bool Contains(Value item);

    /// <summary>
    /// Attempts to retrieve an item present in the set from an equivalent item.
    /// This function returns true if an existing item was found in the set.
    /// </summary>
    /// <param name="item">The item used as key.</param>
    /// <param name="oldItem">The item present in the set.</param>
    /// <returns>True if an existing item was found in the set.</returns>
    bool TryGetValue(Value item, out Value oldItem);

    /// <summary>
    /// Returns the number of elements in the set.
    /// </summary>
    int Count { get; }
  }
}
