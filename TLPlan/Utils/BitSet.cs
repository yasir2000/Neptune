﻿//------------------------------------------------------------------------------
// <copyright file="BitSet.cs" company="Microsoft">
//     
//      Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>
//------------------------------------------------------------------------------

#pragma warning disable 1591 // Missing XML comment for publicly visible type of member.

using System;
using System.Text;
using System.Diagnostics;

namespace TLPlan.Utils
{
  public sealed class BitSet : IComparable<BitSet>
  {
    private const int bitSlotShift = 5;
    private const int bitSlotMask = (1 << bitSlotShift) - 1;

    private int count;
    private uint[] bits;

    private BitSet()
    {
    }

    public BitSet(int count)
    {
      this.count = count;
      bits = new uint[Subscript(count + bitSlotMask)];
    }

    public int Count
    {
      get { return count; }
    }

    public bool this[int index]
    {
      get
      {
        return Get(index);
      }
    }

    public void Clear()
    {
      int bitsLength = bits.Length;
      for (int i = bitsLength; i-- > 0; )
      {
        bits[i] = 0;
      }
    }

    public bool Clear(int index)
    {
      int nBitSlot = Subscript(index);
      //EnsureLength(nBitSlot + 1);

      uint mask = (uint)(1 << (index & bitSlotMask));
      bool oldValue = ((bits[nBitSlot] & mask) != 0);
      bits[nBitSlot] &= ~(mask);
      return oldValue;
    }

    public bool Set(int index)
    {
      int nBitSlot = Subscript(index);
      //EnsureLength(nBitSlot + 1);

      uint mask = (uint)(1 << (index & bitSlotMask));
      bool oldValue = ((bits[nBitSlot] & mask) != 0);
      bits[nBitSlot] |= mask;
      return oldValue;
    }

    public bool Get(int index)
    {
      bool fResult = false;
      if (index < count)
      {
        int nBitSlot = Subscript(index);
        fResult = ((bits[nBitSlot] & (1 << (index & bitSlotMask))) != 0);
      }
      return fResult;
    }

    public int NextSet(int startFrom)
    {
      Debug.Assert(startFrom >= -1 && startFrom <= count);
      int offset = startFrom + 1;
      if (offset == count)
      {
        return -1;
      }
      int nBitSlot = Subscript(offset);
      offset &= bitSlotMask;
      uint word = bits[nBitSlot] >> offset;
      // locate non-empty slot
      while (word == 0)
      {
        if ((++nBitSlot) == bits.Length)
        {
          return -1;
        }
        offset = 0;
        word = bits[nBitSlot];
      }
      while ((word & (uint)1) == 0)
      {
        word >>= 1;
        offset++;
      }
      return (nBitSlot << bitSlotShift) + offset;
    }

    public void And(BitSet other)
    {
      /*
       * Need to synchronize  both this and other->
       * This might lead to deadlock if one thread grabs them in one order
       * while another thread grabs them the other order.
       * Use a trick from Doug Lea's book on concurrency,
       * somewhat complicated because BitSet overrides hashCode().
       */
      if (this == other)
      {
        return;
      }
      int bitsLength = bits.Length;
      int setLength = other.bits.Length;
      int n = (bitsLength > setLength) ? setLength : bitsLength;
      for (int i = n; i-- > 0; )
      {
        bits[i] &= other.bits[i];
      }
      for (; n < bitsLength; n++)
      {
        bits[n] = 0;
      }
    }


    public void Or(BitSet other)
    {
      if (this == other)
      {
        return;
      }
      int setLength = other.bits.Length;
      EnsureLength(setLength);
      for (int i = setLength; i-- > 0; )
      {
        bits[i] |= other.bits[i];
      }
    }

    public override int GetHashCode()
    {
      int h = 1234;
      for (int i = bits.Length; --i >= 0; )
      {
        h ^= (int)bits[i] * (i + 1);
      }
      return (int)((h >> 32) ^ h);
    }

    public override bool Equals(object obj)
    {
      // assume the same type
      if (this == obj)
      {
        return true;
      }
      BitSet other = (BitSet)obj;

      int bitsLength = bits.Length;
      int setLength = other.bits.Length;
      int n = (bitsLength > setLength) ? setLength : bitsLength;
      for (int i = n; i-- > 0; )
      {
        if (bits[i] != other.bits[i])
        {
          return false;
        }
      }
      if (bitsLength > n)
      {
        for (int i = bitsLength; i-- > n; )
        {
          if (bits[i] != 0)
          {
            return false;
          }
        }
      }
      else
      {
        for (int i = setLength; i-- > n; )
        {
          if (other.bits[i] != 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    public BitSet Clone()
    {
      BitSet newset = new BitSet();
      newset.count = count;
      newset.bits = (uint[])bits.Clone();
      return newset;
    }


    public bool IsEmpty
    {
      get
      {
        uint k = 0;
        for (int i = 0; i < bits.Length; i++)
        {
          k |= bits[i];
        }
        return k == 0;
      }
    }

    public bool Intersects(BitSet other)
    {
      int i = Math.Min(this.bits.Length, other.bits.Length);
      while (--i >= 0)
      {
        if ((this.bits[i] & other.bits[i]) != 0)
        {
          return true;
        }
      }
      return false;
    }

    private int Subscript(int bitIndex)
    {
      return bitIndex >> bitSlotShift;
    }

    private void EnsureLength(int nRequiredLength)
    {
      /* Doesn't need to be synchronized because it's an internal method. */
      if (nRequiredLength > bits.Length)
      {
        /* Ask for larger of doubled size or required size */
        int request = 2 * bits.Length;
        if (request < nRequiredLength)
          request = nRequiredLength;
        uint[] newBits = new uint[request];
        Array.Copy(bits, newBits, bits.Length);
        bits = newBits;
      }
    }

#if DEBUG
    public void Dump(StringBuilder bb)
    {
      for (int i = 0; i < count; i++)
      {
        bb.Append(Get(i) ? "1" : "0");
      }
    }
#endif

    #region IComparable<BitSet> Interface

    public int CompareTo(BitSet other)
    {
      int value = this.Count.CompareTo(other.Count);

      for (int i = Subscript(this.Count - 1); value == 0 && i >= 0; --i)
        value = this.bits[i].CompareTo(other.bits[i]);

      return value;
    }

    #endregion
  };

}


