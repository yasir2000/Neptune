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

namespace PDDLParser.Exp.Struct
{
  /// <summary>
  /// Represents a bounded time value, to be used in an interval.
  /// </summary>
  public struct TimeValue : IComparable<TimeValue>
  {
    #region Private Fields

    /// <summary>
    /// The actual time value of the bound.
    /// </summary>
    private double m_timeValue;
    /// <summary>
    /// Whether the interval is open or closed.
    /// </summary>
    private bool m_isIntervalOpen;
    /// <summary>
    /// Whether this is the lower or the upper bound of an interval.
    /// </summary>
    private bool m_isLowerBound;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the actual time value of the bound.
    /// </summary>
    public double Time
    {
      get { return m_timeValue; }
      set { m_timeValue = value; }
    }

    /// <summary>
    /// Gets or sets whether the interval is open or closed.
    /// </summary>
    public bool IsOpen
    {
      get { return m_isIntervalOpen; }
      set { m_isIntervalOpen = value; }
    }

    /// <summary>
    /// Gets whether this is a lower bound.
    /// </summary>
    /// <remarks><c>IsLowerBound == !<see cref="IsUpperBound"/></c></remarks>
    /// <seealso cref="IsUpperBound"/>
    public bool IsLowerBound { get { return m_isLowerBound; } }
    /// <summary>
    /// Gets whether this is an upper bound.
    /// </summary>
    /// <remarks><c>IsUpperBound == !<see cref="IsLowerBound"/></c></remarks>
    /// <seealso cref="IsLowerBound"/>
    public bool IsUpperBound { get { return !m_isLowerBound; } }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new bounded time value.
    /// </summary>
    /// <remarks>When creating an infinite bounded value, it must be open.</remarks>
    /// <param name="time">The time of the bound.</param>
    /// <param name="isOpen">Whether the interval is open.</param>
    /// <param name="isLowerBound">Whether this is a lower bound.</param>
    public TimeValue(double time, bool isOpen, bool isLowerBound)
    {
      System.Diagnostics.Debug.Assert(!double.IsInfinity(time) || isOpen);

      m_timeValue = time;
      m_isIntervalOpen = isOpen;
      m_isLowerBound = isLowerBound;
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Returns the minimum of two bounded time values. This takes into account their
    /// actual time, as well as whether they are open and whether they are lower bounds.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two bounded time values with the same time are not necessarily equivalent. For example,
    /// A closed upper bound time value of 5 will be considered greater than an open upper bound
    /// time value of 5. This is because the actual limit of the open upper bound is slightly less
    /// than five, whereas the limit of the closed upper bound is exactly five.
    /// </para>
    /// <para>
    /// Also note that, while two values may be regarded as the same when using <c>Min</c>,
    /// they may not be considered equal. More specifically, if both bounds are closed, <c>Min</c>
    /// may return either, but they might not be equal (e.g. one is a lower bound, the other, an upper bound).
    /// </para>
    /// </remarks>
    /// <param name="value1">The first bounded time value to compare.</param>
    /// <param name="value2">The second time value to compare.</param>
    /// <returns>The smallest bounded time value.</returns>
    public static TimeValue Min(TimeValue value1, TimeValue value2)
    {
      if (value1.Time < value2.Time)
      {
        return value1;
      }
      else if (value1.Time > value2.Time)
      {
        return value2;
      }
      else // value1.Time == value2.Time
      {
        if (value1.IsOpen)
        {
          if (value2.IsOpen)
          {
            if (value1.IsLowerBound != value2.IsLowerBound)
            {
              if (value1.IsUpperBound)
              {
                return value1;
              }
              else
              {
                return value2;
              }
            }
            else
            {
              // No difference.
              return value1;
            }
          }
          else
          {
            if (value1.IsUpperBound)
            {
              return value1;
            }
            else
            {
              return value2;
            }
          }
        }
        else if (value2.IsOpen)
        {
          // value1 is not open!
          if (value2.IsUpperBound)
          {
            return value2;
          }
          else
          {
            return value1;
          }
        }
        else
        {
          // No difference.
          return value1;
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the bounded time value is lower than the given time.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>True if the bounded time value is lower than the given time.</returns>
    public bool IsLowerThan(double time)
    {
      return (time > this.Time || (this.IsOpen && this.IsUpperBound && time == this.Time));
    }

    /// <summary>
    /// Returns whether the bounded time value is greater than the given time.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>True if the bounded time value is greater than the given time.</returns>
    public bool IsGreaterThan(double time)
    {
      return (time < this.Time || (this.IsOpen && this.IsLowerBound && time == this.Time));
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns the hash code of this bounded time value.
    /// </summary>
    /// <returns>The hash code of this bounded time value.</returns>
    public override int GetHashCode()
    {
      return m_timeValue.GetHashCode() + 31 * m_isIntervalOpen.GetHashCode() + 53 * m_isLowerBound.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this bounded time value.
    /// </summary>
    /// <returns>A string representation of this bounded time value.</returns>
    public override string ToString()
    {
      if (this.IsLowerBound)
      {
        return (this.IsOpen ? "(" : "[") +
               (double.IsInfinity(this.Time) ? "inf-" : this.Time.ToString());
      }
      else
      {
        return (double.IsInfinity(this.Time) ? "inf+" : this.Time.ToString()) + 
               (this.IsOpen ? ")" : "]");
      }
    }

    #endregion

    #region IComparable<TimeValue> Interface

    /// <summary>
    /// Compares this bounded time value with another bounded time value.
    /// </summary>
    /// <param name="other">The other bounded time value to compare this bounded time value to.</param>
    /// <returns>An integer representing the total order relation between the two bounded time values.
    /// </returns>
    public int CompareTo(TimeValue other)
    {
      int value = this.Time.CompareTo(other.Time);
      if (value != 0)
        return value;

      value = this.IsOpen.CompareTo(other.IsOpen);
      if (value != 0)
        return value;

      value = this.IsLowerBound.CompareTo(other.IsLowerBound);

      return value;
    }

    #endregion
  }

  /// <summary>
  /// Represents a time interval, which can have both ends open or closed, finite or infinite.
  /// </summary>
  public struct TimeInterval : IComparable<TimeInterval>
  {
    #region Private Fields

    /// <summary>
    /// The lower bound of the interval.
    /// </summary>
    private TimeValue m_lowerBound;
    /// <summary>
    /// The upper bound of the interval.
    /// </summary>
    private TimeValue m_upperBound;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the lower bound of the interval.
    /// </summary>
    public TimeValue LowerBound { get { return m_lowerBound; } }
    /// <summary>
    /// Gets the upper bound of the interval.
    /// </summary>
    public TimeValue UpperBound { get { return m_upperBound; } }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new time interval.
    /// </summary>
    /// <param name="lowerTimeBound">The lower bound time.</param>
    /// <param name="lowerBoundOpen">Whether the lower bound is open.</param>
    /// <param name="upperTimeBound">The upper bound time.</param>
    /// <param name="upperBoundOpen">Whether the upper bound is open.</param>
    /// <remarks>When using infinite values, the bound must be open.</remarks>
    public TimeInterval(double lowerTimeBound, bool lowerBoundOpen, double upperTimeBound, bool upperBoundOpen)
    {
      if (upperTimeBound < lowerTimeBound)
        throw new System.Exception("Error when constructing TimeInterval: "
                          + "the upper bound (" + upperTimeBound + ") must be greater or equal than "
                          + "the lower bound (" + lowerTimeBound + ").");

      m_lowerBound = new TimeValue(lowerTimeBound, lowerBoundOpen, true);
      m_upperBound = new TimeValue(upperTimeBound, upperBoundOpen, false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the given is before the interval.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>Whether the given is before the interval.</returns>
    public bool IsTimeLowerThanInterval(double time)
    {
      return m_lowerBound.IsGreaterThan(time);
    }

    /// <summary>
    /// Returns whether the given is after the interval.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>Whether the given is after the interval.</returns>
    public bool IsTimeGreaterThanInterval(double time)
    {
      return m_upperBound.IsLowerThan(time);
    }

    /// <summary>
    /// Returns whether the given is in the interval.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>Whether the given is in the interval.</returns>
    public bool IsTimeInInterval(double time)
    {
      return !IsTimeLowerThanInterval(time) && !IsTimeGreaterThanInterval(time);
    }

    /// <summary>
    /// Compares the time with the interval.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>
    /// An integer representing the total order relation between the two worlds.
    /// A value smaller than zero indicates that the time is before the interval.
    /// A value of zero indicates that the time is in the interval.
    /// A value greater than zero indicates that the time is after the interval.
    /// </returns>
    public int CompareTimeToInterval(double time)
    {
      if (IsTimeLowerThanInterval(time))
        return -1;
      else if (IsTimeGreaterThanInterval(time))
        return 1;

      return 0;
    }

    /// <summary>
    /// Returns a copy of this time interval with the added time.
    /// </summary>
    /// <param name="time">The time to add to the lower and upper bounds.</param>
    /// <returns>A copy of this time interval with the added time.</returns>
    public TimeInterval AddTime(double time)
    {
      return new TimeInterval(this.LowerBound.Time + time, this.LowerBound.IsOpen,
                              this.UpperBound.Time + time, this.UpperBound.IsOpen);
    }


    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns the hash code of this time interval.
    /// </summary>
    /// <returns>The hash code of this time interval.</returns>
    public override int GetHashCode()
    {
      return m_lowerBound.GetHashCode() + 97 * m_upperBound.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this time interval.
    /// </summary>
    /// <returns>A string representation of this time interval.</returns>
    public override string ToString()
    {
      return string.Format(":{0},{1}:", this.LowerBound, this.UpperBound);
    }

    #endregion

    #region IComparable<TimeInterval> Interface

    /// <summary>
    /// Compares this time interval with another time interval.
    /// </summary>
    /// <param name="other">The other time interval to compare this time interval to.</param>
    /// <returns>An integer representing the total order relation between the two time intervals.
    /// </returns>
    public int CompareTo(TimeInterval other)
    {
      int value = this.m_lowerBound.CompareTo(other.m_lowerBound);
      if (value != 0)
        return value;

      value = this.m_upperBound.CompareTo(other.m_upperBound);
      
      return value;
    }

    #endregion
  }
}
