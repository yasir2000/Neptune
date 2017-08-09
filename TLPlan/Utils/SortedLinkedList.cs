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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Extensions;

namespace TLPlan.Utils
{
  /// <summary>
  /// Represents a linked list sorted on its keys.
  /// </summary>
  /// <typeparam name="K">The type of the key. This must implement IComparable.</typeparam>
  /// <typeparam name="V">The type of the value. This must implement IComparable.</typeparam>
  public class SortedLinkedList<K, V> : IComparable<SortedLinkedList<K, V>>, IEnumerable<KeyValuePair<K, V>>
                                        where K : IComparable<K>
                                        where V : IComparable<V>
  {
    #region Private Fields

    /// <summary>
    /// The linked list which is sorted on keys.
    /// </summary>
    private LinkedList<KeyValuePair<K, V>> m_sortedList;

    #endregion

    #region Properties

    /// <summary>
    /// Returns the number of elements in the linked list.
    /// </summary>
    public int Count { get { return m_sortedList.Count; } }

    /// <summary>
    /// Returns an enumerable over the values of the linked list, sorted by keys.
    /// </summary>
    public IEnumerable<V> Values { get { return m_sortedList.Values(); } }

    /// <summary>
    /// Returns the first element of the sorted linked list.
    /// </summary>
    public KeyValuePair<K, V> First { get { return m_sortedList.First.Value; } }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty instance of the sorted linked list.
    /// </summary>
    public SortedLinkedList()
    {
      this.m_sortedList = new LinkedList<KeyValuePair<K, V>>();
    }

    /// <summary>
    /// Creates an instance of sorted linked list containing the given keys and values.
    /// </summary>
    /// <param name="ienum">The contents of the linked list.</param>
    public SortedLinkedList(IEnumerable<KeyValuePair<K, V>> ienum)
    {
      m_sortedList = new LinkedList<KeyValuePair<K, V>>(ienum.OrderBy(pair => pair.Key));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Inserts an element in the linked list so that it remains sorted.
    /// </summary>
    /// <param name="key">The key at which to insert the value.</param>
    /// <param name="value">The value to insert.</param>
    public void InsertSorted(K key, V value)
    {
      LinkedListNode<KeyValuePair<K, V>> node = this.m_sortedList.First;
      while (node != null)
      {
        if (node.Value.Key.CompareTo(key) > 0)
          break;

        node = node.Next;
      }

      if (node != null)
        this.m_sortedList.AddBefore(node, new KeyValuePair<K, V>(key, value));
      else
        this.m_sortedList.AddLast(new KeyValuePair<K, V>(key, value));
    }

    /// <summary>
    /// Returns the first values in the sorted linked list. The first values are all the values
    /// at the beginning of the linked list that have the same key.
    /// </summary>
    /// <returns>The first values of the linked list.</returns>
    public IEnumerable<KeyValuePair<K, V>> GetFirstValues()
    {
      foreach (LinkedListNode<KeyValuePair<K, V>> node in GetFirstNodes())
        yield return node.Value;
    }

    /// <summary>
    /// Removes and returns the first values in the sorted linked list. The first values are 
    /// all the values at the beginning of the linked list that have the same key.
    /// </summary>
    /// <returns>The first values of the linked list.</returns>
    public IEnumerable<KeyValuePair<K, V>> RemoveFirstValues()
    {
      foreach (LinkedListNode<KeyValuePair<K, V>> node in GetFirstNodes())
      {
        yield return node.Value;
        this.m_sortedList.Remove(node);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the first nodes in the sorted linked list. The first nodes are 
    /// all the nodes at the beginning of the linked list that have the same key.
    /// </summary>
    /// <returns>The first nodes in the sorted linked list.</returns>
    private IEnumerable<LinkedListNode<KeyValuePair<K, V>>> GetFirstNodes()
    {
      LinkedListNode<KeyValuePair<K, V>> node = this.m_sortedList.First;
      if (node == null)
        yield break;

      K k = node.Value.Key;

      while (node != null && node.Value.Key.CompareTo(k) == 0)
      {
        yield return node;
        node = node.Next;
      }
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns the hash code of this sorted linked list.
    /// </summary>
    /// <returns>The hash code of this sorted linked list.</returns>
    public override int GetHashCode()
    {
      return this.m_sortedList.Keys().GetOrderedEnumerableHashCode()
           + this.m_sortedList.Values().GetOrderedEnumerableHashCode();
    }

    /// <summary>
    /// Returns true if this sorted linked list is equal to a specified object.
    /// Two sorted linked list are equal if they contain the same key/value pairs.
    /// </summary>
    /// <param name="obj">Object to test for equality.</param>
    /// <returns>True if this sorted linked list is equal to the specified objet.</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }
      else if (obj is SortedLinkedList<K, V>)
      {
        SortedLinkedList<K, V> other = (SortedLinkedList<K, V>)obj;
        return this.m_sortedList.Keys().SequenceEqual(other.m_sortedList.Keys()) 
            && this.m_sortedList.Values().SequenceEqual(other.m_sortedList.Values());
      }
      else
      {
        return false;
      }
    }

    #endregion

    #region IComparable<SortedLinkedList<K, V>> Interface

    /// <summary>
    /// Compares this sorted linked list with another sorted linked list.
    /// </summary>
    /// <param name="other">The other sorted linked list to compare this sorted linked list to.</param>
    /// <returns>An integer representing the total order relation between the two sorted linked list.</returns>
    public virtual int CompareTo(SortedLinkedList<K, V> other)
    {
      IEnumerator<K> kThisIt  = this.m_sortedList.Keys().GetEnumerator();
      IEnumerator<V> vThisIt  = this.m_sortedList.Values().GetEnumerator();
      IEnumerator<K> kOtherIt = other.m_sortedList.Keys().GetEnumerator();
      IEnumerator<V> vOtherIt = other.m_sortedList.Values().GetEnumerator();

      int value = this.m_sortedList.Count.CompareTo(other.m_sortedList.Count);
      while (value == 0 && kThisIt.MoveNext() && kOtherIt.MoveNext() && vThisIt.MoveNext() && vOtherIt.MoveNext())
      {
        value = kThisIt.Current.CompareTo(kOtherIt.Current);
        if (value == 0)
          value = vThisIt.Current.CompareTo(vOtherIt.Current);
      }

      return value;
    }

    #endregion

    #region IEnumerable<KeyValuePair<K,V>> Interface

    /// <summary>
    /// Returns an enumerator over the key/value pairs stored this sorted linked list.
    /// </summary>
    /// <returns>An enumerator over the key/value pairs stored this sorted linked list.</returns>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
      return this.m_sortedList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Interface

    /// <summary>
    /// Returns an enumerator over the key/value pairs stored this sorted linked list.
    /// </summary>
    /// <returns>An enumerator over the key/value pairs stored this sorted linked list.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
  }
}
