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

namespace TLPlan.Utils
{
  /// <summary>
  /// A multi-dictionary associates keys to lists of values.
  /// </summary>
  /// <typeparam name="Key">The key type.</typeparam>
  /// <typeparam name="Value">The value type.</typeparam>
  public interface IMultiDictionary<Key, Value> : IEnumerable<KeyValuePair<Key, Value>>
  {
    /// <summary>
    /// Adds a value with the specific key.
    /// </summary>
    /// <param name="key">The dictionary's key.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>A pointer on the linked list's node which stores the value.</returns>
    LinkedListNode<Value> Add(Key key, Value value);

    /// <summary>
    /// Removes a node from a linked list in the dictionary.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    void Remove(LinkedListNode<Value> node);

    /// <summary>
    /// Returns whether the multi-dictionary is empty.
    /// </summary>
    /// <returns>Whether the multi-dictionary is empty.</returns>
    bool IsEmpty();

    /// <summary>
    /// Returns the number of elements stored in the multi-dictionary.
    /// </summary>
    int Count { get; }
  }
}
