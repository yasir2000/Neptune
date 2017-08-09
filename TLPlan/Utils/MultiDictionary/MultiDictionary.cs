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
  /// Multi-dictionary implemented as a dictionary (hashtable).
  /// </summary>
  /// <typeparam name="Key">The key type.</typeparam>
  /// <typeparam name="Value">The value type.</typeparam>
  public class MultiDictionary<Key, Value> : AbstractMultiDictionary<Key, Value>
  {
    /// <summary>
    /// Items are retrieved using the hashtable.
    /// </summary>
    private Dictionary<Key, LinkedList<Value>> m_items = new Dictionary<Key, LinkedList<Value>>();

    /// <summary>
    /// Creates a new empty multi dictionary.
    /// </summary>
    public MultiDictionary()
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
  }
}
