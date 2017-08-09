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
  /// An interval is defined as a starting value and length.
  /// </summary>
  public struct IntegerInterval
  {
    /// <summary>
    /// The first value.
    /// </summary>
    private int m_start;
    /// <summary>
    /// The length of the interval.
    /// </summary>
    private int m_length;

    /// <summary>
    /// Creates a new interval with the specified first value and length.
    /// </summary>
    /// <param name="start">The first value.</param>
    /// <param name="length">The length of the interval.</param>
    public IntegerInterval(int start, int length)
    {
      this.m_start = start;
      this.m_length = length;
    }

    /// <summary>
    /// The first value.
    /// </summary>
    public int Start
    {
      get { return m_start; }
    }

    /// <summary>
    /// The length of the interval.
    /// </summary>
    public int Length
    {
      get { return m_length; }
    }

    /// <summary>
    /// Returns whether the specified value is contained in the interval.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <returns>Whether the specified value is contained in the interval.</returns>
    public bool Contains(int value)
    {
      int index = value - Start;
      return (index >= 0 && index < Length);
    }
  }
}
