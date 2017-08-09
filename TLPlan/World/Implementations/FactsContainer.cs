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
using PDDLParser;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using TLPlan.Utils;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// A facts container is responsible for holding and updating facts.
  /// </summary>
  public abstract class FactsContainer : IComparable<FactsContainer>
  {
    #region Public Methods

    /// <summary>
    /// Copies this facts container.
    /// </summary>
    /// <returns>A copy of this facts container.</returns>
    public abstract FactsContainer Copy();

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Checks whether the specified described atomic formula holds in this world.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    /// <returns>True, false, or unknown.</returns>
    public abstract FuzzyBool IsSet(int formulaID);

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the specified atomic formula to true.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    public abstract void Set(int formulaID);

    /// <summary>
    /// Sets the specified atomic formula to false.
    /// </summary>
    /// <param name="formulaID">A formula ID.</param>
    public abstract void Unset(int formulaID);

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this facts container is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this facts container is equal to the other object.</returns>
    public override abstract bool Equals(object obj);
    
    /// <summary>
    /// Returns the hash code of this facts container.
    /// </summary>
    /// <returns>The hash code of this facts container.</returns>
    public override abstract int GetHashCode();

    #endregion

    #region IComparable<OpenWorld> interface

    /// <summary>
    /// Compares this facts container with another facts container.
    /// </summary>
    /// <param name="other">The other facts container to compare this facts container to.</param>
    /// <returns>An integer representing the total order relation between the two facts containers.
    /// </returns>
    public abstract int CompareTo(FactsContainer other);

    #endregion
  }
}
