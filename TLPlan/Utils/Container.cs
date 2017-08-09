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
  /// A container stores an object.
  /// This class (which looks useless at first sight) actually provides an additional 
  /// level of indirection when passing objects around.
  /// </summary>
  /// <typeparam name="T">The type of the object to store.</typeparam>
  public class Container<T>
  {
    /// <summary>
    /// Creates a new container for the specified object.
    /// </summary>
    /// <param name="value">The type of the object to store.</param>
    public Container(T value)
    {
      this.Value = value;
    }

    /// <summary>
    /// The stored object.
    /// </summary>
    public T Value
    {
      get;
      set;
    }

    /// <summary>
    /// Verifies whether this container is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether the two container are equal.</returns>
    public override bool Equals(object obj)
    {
      return this.Value.Equals(((Container<T>)obj).Value);
    }

    /// <summary>
    /// Returns the hash code of this container.
    /// </summary>
    /// <returns>The hash code of this container.</returns>
    public override int GetHashCode()
    {
      return this.Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this container.
    /// </summary>
    /// <returns>A string representation of this container.</returns>
    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
