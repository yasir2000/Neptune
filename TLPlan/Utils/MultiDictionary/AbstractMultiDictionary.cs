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

namespace TLPlan.Utils.MultiDictionary
{
  /// <summary>
  /// Base class for multi-dictionary implementations.
  /// </summary>
  /// <typeparam name="Key">The key type.</typeparam>
  /// <typeparam name="Value">The value type.</typeparam>
  public abstract class AbstractMultiDictionary<Key, Value>
    : IMultiDictionary<Key, Value>
  {
    #region Private class MultiDictionaryEnumerator

    /// <summary>
    /// Enumerator over the key-value pairs of the multi-dictionary.
    /// </summary>
    private sealed class MultiDictionaryEnumerator : IEnumerator<KeyValuePair<Key, Value>>
    {
      /// <summary>
      /// Enumerator over the lists of elements.
      /// </summary>
      private IEnumerator<KeyValuePair<Key, LinkedList<Value>>> m_mainEnumerator;
      /// <summary>
      /// Enumerator over the individual elements of a list.
      /// </summary>
      private IEnumerator<Value> m_listEnumerator;

      /// <summary>
      /// Creates a new multi-dictionary enumerator.
      /// </summary>
      /// <param name="dict">The mult-dictionary to enumerate over.</param>
      public MultiDictionaryEnumerator(AbstractMultiDictionary<Key, Value> dict)
      {
        m_mainEnumerator = dict.Items.GetEnumerator();
        if (m_mainEnumerator.MoveNext())
          m_listEnumerator = m_mainEnumerator.Current.Value.GetEnumerator();
      }

      /// <summary>
      /// Gets the current pair pointed to by the enumerator.
      /// </summary>
      public KeyValuePair<Key, Value> Current
      {
        get
        {
          return new KeyValuePair<Key, Value>(m_mainEnumerator.Current.Key,
                                              m_listEnumerator.Current);
        }
      }

      /// <summary>
      /// Gets the current pair pointed to by the enumerator.
      /// </summary>
      object System.Collections.IEnumerator.Current
      {
        get { return Current; }
      }

      /// <summary>
      /// Advances the enumerator to the next element of the collection.
      /// </summary>
      /// <returns>True if the enumerator was successfully advanced to the next element;
      /// false if the enumerator has passed the end of the collection.</returns>
      public bool MoveNext()
      {
        if (m_listEnumerator == null)
        {
          return false;
        }
        while (!m_listEnumerator.MoveNext())
        {
          if (!m_mainEnumerator.MoveNext())
            return false;

          m_listEnumerator = m_mainEnumerator.Current.Value.GetEnumerator();
        }
        return true;
      }

      /// <summary>
      /// Sets the enumerator to its initial position, which is before the first element 
      /// in the collection.
      /// </summary>
      public void Reset()
      {
        m_mainEnumerator.Reset();
        if (m_mainEnumerator.MoveNext())
          m_listEnumerator = m_mainEnumerator.Current.Value.GetEnumerator();
      }

      /// <summary>
      /// Disposes the enumerator.
      /// </summary>
      public void Dispose()
      {
      }
    }

    #endregion

    /// <summary>
    /// The number of items in the multi-dictionary.
    /// </summary>
    protected int m_itemCount;

    /// <summary>
    /// Creates a new empty multi-dictionary.
    /// </summary>
    public AbstractMultiDictionary()
    {
      this.m_itemCount = 0;
    }

    /// <summary>
    /// Returns the all the items (sorted by key) present in the multi-dictionary.
    /// </summary>
    protected abstract IDictionary<Key, LinkedList<Value>> Items
    {
      get;
    }

    /// <summary>
    /// Returns an enumerator over all key-value pairs in the multi-dictionary.
    /// </summary>
    /// <returns>An enumerator over all key-value pairs in the multi-dictionary</returns>
    public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
    {
      return new MultiDictionaryEnumerator(this);
    }

    /// <summary>
    /// Returns an enumerator over all key-value pairs in the multi-dictionary.
    /// </summary>
    /// <returns>An enumerator over all key-value pairs in the multi-dictionary</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Adds a value with the specific key.
    /// </summary>
    /// <param name="key">The dictionary's key.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>A pointer on the linked list's node which stores the value.</returns>
    public LinkedListNode<Value> Add(Key key, Value value)
    {
      LinkedList<Value> list = null;
      if (!Items.TryGetValue(key, out list))
      {
        list = new LinkedList<Value>();
        Items.Add(key, list);
      }
      ++m_itemCount;
      return list.AddLast(value);
    }

    /// <summary>
    /// Removes a node from a linked list in the dictionary.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    public void Remove(LinkedListNode<Value> node)
    {
      --m_itemCount;
      node.List.Remove(node);
      // There is no real way to remove the list if it is empty since we do not
      // its associated key...
    }

    /// <summary>
    /// Returns whether the multi-dictionary is empty.
    /// </summary>
    /// <returns>Whether the multi-dictionary is empty.</returns>
    public bool IsEmpty()
    {
      return Items.Count == 0;
    }

    /// <summary>
    /// Returns the number of elements stored in the multi-dictionary.
    /// </summary>
    public int Count
    {
      get
      {
        return m_itemCount;
      }
    }
  }
}
