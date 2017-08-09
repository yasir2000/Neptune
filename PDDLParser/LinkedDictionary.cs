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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PDDLParser.Extensions;

namespace PDDLParser
{
  /// <summary>
  /// A linked dictionary maps keys to values. It allows enumerating over the pairs of key/value
  /// in the same order they were added to the dictionary.
  /// </summary>
  /// <typeparam name="Key">The type of the keys in the dictionary.</typeparam>
  /// <typeparam name="Value">The type of the values in the dictionary.</typeparam>
  public class LinkedDictionary<Key, Value> : IDictionary<Key, Value>
  {
    /// <summary>
    /// This internal list holds all the pairs of key/value contained in the dictionary,
    /// in the order which they were added.
    /// </summary>
    private LinkedList<KeyValuePair<Key, Value>> m_list;
    /// <summary>
    /// This internal dictionary maps keys to <see cref="m_list"/>'s nodes.
    /// It allows for fast retrieval of values when given a key.
    /// </summary>
    private Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>> m_dict;

    /// <summary>
    /// Creates a new empty linked dictionary.
    /// </summary>
    public LinkedDictionary()
    {
      this.m_list = new LinkedList<KeyValuePair<Key, Value>>();
      this.m_dict = new Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>>();
    }

    #region IDictionary<Key,Value> Members

    /// <summary>
    /// Adds an element with the specified key to the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    public void Add(Key key, Value value)
    {
      LinkedListNode<KeyValuePair<Key, Value>> node = this.m_list.AddLast(
        new KeyValuePair<Key, Value>(key, value));
      this.m_dict.Add(key, node);
    }

    /// <summary>
    /// Determines whether the dictionary contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns>Whether the dictionary contains an element with the specified key.</returns>
    public bool ContainsKey(Key key)
    {
      return this.m_dict.ContainsKey(key);
    }

    /// <summary>
    /// Returns the collection of keys present in the dictionary.
    /// </summary>
    public ICollection<Key> Keys
    {
      get { return this.m_dict.Keys; }
    }

    /// <summary>
    /// Removes the element with the specified key from the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>True if the element is successfully removed; otherwise, false.</returns>
    public bool Remove(Key key)
    {
      LinkedListNode<KeyValuePair<Key, Value>> node;
      if (this.m_dict.TryGetValue(key, out node))
      {
        this.m_dict.Remove(key);
        this.m_list.Remove(node);
        return true;
      }
      else
      {
        return false;
      }
    }
    
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get.</param>
    /// <param name="value">This parameter is set to the value of the element to look
    /// for, if found.</param>
    /// <returns>True if an element with the specified key is contained in the dictionary.</returns>
    public bool TryGetValue(Key key, out Value value)
    {
      LinkedListNode<KeyValuePair<Key, Value>> node;
      if (this.m_dict.TryGetValue(key, out node))
      {
        value = node.Value.Value;
        return true;
      }
      else
      {
        value = default(Value);
        return false;
      }
    }

    /// <summary>
    /// Returns the collection of values present in the dictionary.
    /// </summary>
    public ICollection<Value> Values
    {
      get
      {
        return new ReadOnlyCollection<Value>(m_list.Values().ToList());
      }
    }

    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get or set.</param>
    /// <returns>The element with the specified key.</returns>
    public Value this[Key key]
    {
      get
      {
        return this.m_dict[key].Value.Value;
      }
      set
      {
        LinkedListNode<KeyValuePair<Key, Value>> node;
        if (this.m_dict.TryGetValue(key, out node))
        {
          node.Value = new KeyValuePair<Key, Value>(node.Value.Key, value);
        }
        else
        {
          this.Add(key, value);
        }
      }
    }

    #endregion

    #region ICollection<KeyValuePair<Key,Value>> Members

    /// <summary>
    /// Adds a pair of key/value to the dictionary.
    /// </summary>
    /// <param name="item">The pair of key/value to add to the dictionary.</param>
    public void Add(KeyValuePair<Key, Value> item)
    {
      LinkedListNode<KeyValuePair<Key, Value>> node = this.m_list.AddLast(item);
      this.m_dict.Add(item.Key, node);
    }

    /// <summary>
    /// Clears the dictionary.
    /// </summary>
    public void Clear()
    {
      m_list.Clear();
      m_dict.Clear();
    }

    /// <summary>
    /// Determines whether the specified pair of key/value is present in the dictionary.
    /// </summary>
    /// <param name="item">The key/value pair of the element to locate.</param>
    /// <returns>True if the dictionary contains an element with the same key and value;
    /// otherwise false.</returns>
    public bool Contains(KeyValuePair<Key, Value> item)
    {
      LinkedListNode<KeyValuePair<Key, Value>> node;
      if (this.m_dict.TryGetValue(item.Key, out node))
      {
        return node.Value.Value.Equals(item.Value);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Copies the elements of the dictionary to an array, starting at a particular index.
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the elements.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex)
    {
      this.m_list.CopyTo(array, arrayIndex);
    }
    
    /// <summary>
    /// Gets the number of elements contained in this dictionary.
    /// </summary>
    public int Count
    {
      get { return this.m_list.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether this dictionary is read-only, which it is not.
    /// </summary>
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <summary>
    /// Removes the element with the specified key and value.
    /// </summary>
    /// <param name="item">The key and value of the element to remove.</param>
    /// <returns>True if an element with the same key and value was removed.</returns>
    public bool Remove(KeyValuePair<Key, Value> item)
    {
      LinkedListNode<KeyValuePair<Key, Value>> node;
      if (this.m_dict.TryGetValue(item.Key, out node))
      {
        if (node.Value.Value.Equals(item.Value))
        {
          this.m_list.Remove(node);
          this.m_dict.Remove(item.Key);
          return true;
        }
        else
        {
          return false;
        }
      }
      else
      {
        return false;
      }
    }

    #endregion

    #region IEnumerable<KeyValuePair<Key,Value>> Members

    /// <summary>
    /// Returns an enumerator over the pairs of key/value in the dictionary.
    /// </summary>
    /// <returns>An enumerator over the pairs of key/value in the dictionary.</returns>
    public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
    {
      return this.m_list.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator over the pairs of key/value in the dictionary.
    /// </summary>
    /// <returns>An enumerator over the pairs of key/value in the dictionary.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.m_list.GetEnumerator();
    }

    #endregion
  }
}
