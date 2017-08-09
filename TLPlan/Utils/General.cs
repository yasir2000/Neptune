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
  /// Represents a set of general utility methods to be used in TLPlan.
  /// </summary>
  public static class General
  {
    /// <summary>
    /// Compares two key/value pairs.
    /// Comparison is done on the key first, then on the value in case of equality.
    /// </summary>
    /// <typeparam name="Key">The key type.</typeparam>
    /// <typeparam name="Value">The value type.</typeparam>
    /// <param name="x">The first key/value pair.</param>
    /// <param name="y">The second key/value pair.</param>
    /// <returns>An integer representing the total order relation between the two pairs.</returns>
    public static int ComparePair<Key, Value>(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
      where Key : IComparable<Key>
      where Value : IComparable<Value>
    {
      int value = x.Key.CompareTo(y.Key);
      if (value != 0)
        return value;
      else
        return x.Value.CompareTo(y.Value);
    }

    /// <summary>
    /// Returns the hash code of an integer value.
    /// </summary>
    /// <param name="value">The value to hash.</param>
    /// <returns>The hash code of the integer value.</returns>
    public static int Hash(int value)
    {
      int hashcode, add;
      hashcode = add = value;
      for (int i = 0; i < 2; ++i)
      {
        add *= add;
        hashcode += add;
      }
      return hashcode;
    }
  }
}
