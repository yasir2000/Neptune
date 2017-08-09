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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDDLParser.Extensions
{
  /// <summary>
  /// Represents an extension class for any object or structure.
  /// </summary>
  public static class ObjectExtentions
  {
    /// <summary>
    /// Returns an enumeration containing only the given object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The given to wrap in an enumerable.</param>
    /// <returns>An enumeration containing only the given object.</returns>
    public static IEnumerable<T> Once<T>(this T obj)
    {
      yield return obj;
    }

    /// <summary>
    /// Returns whether two objects are equal, or both null.
    /// </summary>
    /// <param name="obj">The first object.</param>
    /// <param name="other">The second object.</param>
    /// <returns>Whether two objects are equal or both null.</returns>
    public static bool EqualsOrBothNull(this object obj, object other)
    {
      if (obj == null)
      {
        return (other == null);
      }
      else if (other == null)
      {
        return false;
      }
      else
      {
        return obj.Equals(obj);
      }
    }
  }

  /// <summary>
  /// Represents an extension class for IEnumerable's.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Returns that hashcode of the given enumerable; the hashcode takes into account the order of the elements.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="enumerable">The enumerable to compute the hashcode for.</param>
    /// <returns>The order-dependent hashcode of the enumerable.</returns>
    public static int GetOrderedEnumerableHashCode<T>(this IEnumerable<T> enumerable)
    {
      int hashCode = 1;
      foreach (T obj in enumerable)
        hashCode = 31 * hashCode + (obj == null ? 0 : obj.GetHashCode());

      return hashCode;
    }

    /// <summary>
    /// Returns that hashcode of the given enumerable; the hashcode is order-independent.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="enumerable">The enumerable to compute the hashcode for.</param>
    /// <returns>The order-independent hashcode of the enumerable.</returns>
    public static int GetUnorderedEnumerableHashCode<T>(this IEnumerable<T> enumerable)
    {
      int hashCode = 0;
      foreach (T obj in enumerable)
        hashCode += obj.GetHashCode();

      return hashCode;
    }

    /// <summary>
    /// Compares two enumerables, taking the order into account. Note that comparing the count
    /// of both enumerables before calling this may be faster.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <param name="enumerable">The current enumerable used to compare.</param>
    /// <param name="other">The enumerable to compare to.</param>
    /// <returns>An integer representing the total order relation between the two enumerables.</returns>
    public static int SequenceCompareTo<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
      where T: IComparable<T>
    {
      IEnumerator<T> enumThis = enumerable.GetEnumerator();
      IEnumerator<T> enumOther = other.GetEnumerator();
      int value = 0;
      bool thisMoveNext = false;
      bool otherMoveNext = false;
      do
      {
        thisMoveNext = enumThis.MoveNext();
        otherMoveNext = enumOther.MoveNext();
        if ((value = thisMoveNext.CompareTo(otherMoveNext)) == 0 && thisMoveNext)
          value = enumThis.Current.CompareTo(enumOther.Current);
      } while (value == 0 && thisMoveNext);
      return value;
    }

    /// <summary>
    /// Verifies whether the given enumerable contains a null element.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to verify.</param>
    /// <returns>Whether the enumerable contains a null element.</returns>
    public static bool ContainsNull<T>(this IEnumerable<T> enumerable) where T : class
    {
      foreach (T t in enumerable)
      {
        if (t == null)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Returns whether the enumerable is empty. This is more efficient than using the 
    /// extension Count method, since Count has to traverse the whole enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check for emptiness.</param>
    /// <returns>Whether the enumerable is empty.</returns>
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
      return !enumerable.GetEnumerator().MoveNext();
    }

    /// <summary>
    /// Returns an enumerable containing the given elements.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <param name="values">All the elements to put in the enumerable.</param>
    /// <returns>An enumerable containing the given elements.</returns>
    public static IEnumerable<T> Enumerable<T>(params T[] values)
    { 
      foreach (T t in values)
        yield return t;
    }

    /// <summary>
    /// Flattens an enumerable of enumerables into a single enumerable. Note that this
    /// only flattens one "layer" of enumerables, i.e. an enumerable of enumerables of
    /// enumerables will be flattened in an enumerable of enumerables.
    /// </summary>
    /// <typeparam name="T">The type of the flattened enumerable.</typeparam>
    /// <param name="enumerable">The enumerable of enumerables to flatten.</param>
    /// <returns>The flattened enumerable.</returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
      if (enumerable.IsEmpty())
        return System.Linq.Enumerable.Empty<T>();

      return enumerable.Aggregate((enum1, enum2) => enum1.Concat(enum2));
    }
  }

  /// <summary>
  /// Represents extensions methods for lists.
  /// </summary>
  public static class ListExtensions
  {
    /// <summary>
    /// Compares two lists, taking the order into account. This first compares the counts, then 
    /// compares the content. Note that this should not be called from CompareTo methods if
    /// multiple lists have to be compared; comparing all their counts and then calling
    /// <see cref="ListContentCompareTo"/> should be faster.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    /// <param name="lst1">The list which will be compared to.</param>
    /// <param name="lst2">The list to compare to.</param>
    /// <returns>An integer representing the total order relation between the two lists.</returns>
    public static int ListCompareTo<T>(this List<T> lst1, List<T> lst2) where T : IComparable<T>
    {
      int value = lst1.Count.CompareTo(lst2.Count);
      if (value != 0)
        return value;

      return lst1.ListContentCompareTo(lst2);
    }

    /// <summary>
    /// Compares the content of two lists, taking the order into account. This asusmes that both
    /// list have the same number of elements.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    /// <param name="lst1">The list which will be compared to.</param>
    /// <param name="lst2">The list to compare to.</param>
    /// <returns>An integer representing the total order relation between the two lists.</returns>
    public static int ListContentCompareTo<T>(this List<T> lst1, List<T> lst2) where T : IComparable<T>
    {
      // This assumes that both lists have the same count!
      int value = 0;

      for (int i = 0; value == 0 && i < lst1.Count; ++i)
        value = lst1[i].CompareTo(lst2[i]);

      return value;
    }
  }

  /// <summary>
  /// Represents extensions methods for dictionaries.
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>
    /// Verifies whether two dictionaries are equals, by comparing each eky/value pair.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="dic">The first dictionary.</param>
    /// <param name="other">The dictionary to compare.</param>
    /// <returns>Whether the dictionaries are equal.</returns>
    public static bool DictionaryEqual<K, V>(this IDictionary<K, V> dic, IDictionary<K, V> other)
    {
      if (dic == null)
      {
        if (other == null)
          return true;
        return false;
      }

      if (dic.Count != other.Count)
        return false;

      foreach (K k in dic.Keys)
      {
        V v1 = dic[k];
        V v2;
        if (!other.TryGetValue(k, out v2) || !v1.Equals(v2))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Retrieves the hashcode of a dictionary.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="dic">The dictionary used to compute the hashcode.</param>
    /// <returns>The dictionary's hashcode.</returns>
    public static int DictionaryGetHashCode<K, V>(this IDictionary<K, V> dic)
    {
      return dic.Keys.GetUnorderedEnumerableHashCode() + dic.Values.GetUnorderedEnumerableHashCode();
    }

    /// <summary>
    /// Compares two dictionaries by sorting their keys (if necessary) and comparing their values one by one. If multiplie dictionaries
    /// have to be compared, it would be wiser to compare their count prior to calling this methods, as comparing the
    /// contents of dictionaries can be costly.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="dic">The dictionary used to compare.</param>
    /// <param name="other">The dictionary to compare to.</param>
    /// <param name="thisSortedKeys">The sorted keys of the first dictionary. This can be null, in which case the the dictionaries key will be sorted.</param>
    /// <param name="otherSortedKeys">The sorted keys of the second dictionary. This can be null, in which case the the dictionaries key will be sorted.</param>
    /// <returns>An integer representing the total order relation between the two dictionaries.</returns>
    public static int DictionaryCompareTo<K, V>(this IDictionary<K, V> dic, IDictionary<K, V> other, ref IEnumerable<K> thisSortedKeys, ref IEnumerable<K> otherSortedKeys)
      where K : IComparable<K> where V : IComparable<V>
    {
      int value = dic.Count.CompareTo(other.Count);

      if (value != 0)
        return value;

      if (thisSortedKeys == null)
      {
        List<K> thisSortedKeyList = new List<K>(dic.Keys);
        thisSortedKeyList.Sort();
        thisSortedKeys = thisSortedKeyList;
      }

      if (otherSortedKeys == null)
      {
        List<K> otherSortedKeyList = new List<K>(other.Keys);
        otherSortedKeyList.Sort();
        otherSortedKeys = otherSortedKeyList;
      }

      IEnumerator<K> enumThis = thisSortedKeys.GetEnumerator();
      IEnumerator<K> enumOther = otherSortedKeys.GetEnumerator();

      while (value == 0 && enumThis.MoveNext() && enumOther.MoveNext())
      {
        value = enumThis.Current.CompareTo(enumOther.Current);
        if (value == 0)
          value = dic[enumThis.Current].CompareTo(other[enumOther.Current]);
      }
      return value;
    }

    /// <summary>
    /// Merges two dictionaries of lists.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="dic">The dictionary which will contain the merged list.</param>
    /// <param name="other">The dictionary whose lists will be merged in the other dictionary.</param>
    public static void DictionaryMergeList<K, V>(this IDictionary<K, List<V>> dic, IDictionary<K, List<V>> other)
    {
      foreach (KeyValuePair<K, List<V>> pair in other)
      {
        List<V> list;
        if (!dic.TryGetValue(pair.Key, out list))
        {
          list = new List<V>();
          dic[pair.Key] = list;
        }

        list.AddRange(pair.Value);
      }
    }
  }

  /// <summary>
  /// Represents extension methods for KeyValuePair and enumables of KeyValuePair.
  /// </summary>
  public static class KeyValueExtensions
  {
    /// <summary>
    /// Returns an enumerable over the keys of the KeyValuePair's.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="ienum">The enumerable to enumerate over.</param>
    /// <returns>An enumerable over all the keys.</returns>
    public static IEnumerable<K> Keys<K, V>(this IEnumerable<KeyValuePair<K, V>> ienum)
    {
      foreach (KeyValuePair<K, V> pair in ienum)
        yield return pair.Key;
    }

    /// <summary>
    /// Returns an enumerable over the values of the KeyValuePair's.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="ienum">The enumerable to enumerate over.</param>
    /// <returns>An enumerable over all the values.</returns>
    public static IEnumerable<V> Values<K, V>(this IEnumerable<KeyValuePair<K, V>> ienum)
    {
      foreach (KeyValuePair<K, V> pair in ienum)
        yield return pair.Value;
    }

    /// <summary>
    /// Compares to enumerables of KeyValuePair's, taking the order into account.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    /// <param name="ienum">The enumerable used to compare.</param>
    /// <param name="other">The enumerable to compare to.</param>
    /// <returns>An integer representing the total order relation between the two enumerables.</returns>
    public static int SequenceCompareTo<K, V>(this IEnumerable<KeyValuePair<K, V>> ienum,
                                                   IEnumerable<KeyValuePair<K, V>> other)
      where K : IComparable<K>
      where V : IComparable<V>
    {
      IEnumerator<KeyValuePair<K, V>> enumThis = ienum.GetEnumerator();
      IEnumerator<KeyValuePair<K, V>> enumOther = other.GetEnumerator();
      int value = 0;
      bool thisMoveNext = false;
      bool otherMoveNext = false;
      do
      {
        thisMoveNext = enumThis.MoveNext();
        otherMoveNext = enumOther.MoveNext();
        if ((value = thisMoveNext.CompareTo(otherMoveNext)) == 0 &&
            thisMoveNext)
        {
          if ((value = enumThis.Current.Key.CompareTo(enumOther.Current.Key)) == 0)
          {
            value = enumThis.Current.Value.CompareTo(enumOther.Current.Value);
          }
        }
      } while (value == 0 && thisMoveNext);
      return value;
    }
  }
}
