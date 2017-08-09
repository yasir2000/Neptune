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
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using TLPlan.Utils;
using Double = PDDLParser.Exp.Struct.Double;

namespace TLPlan.World.Implementations
{
  /// <summary>
  /// A fluents container is responsible for holding and updating both numeric
  /// and object fluents.
  /// </summary>
  public abstract class FluentsContainer : IComparable<FluentsContainer>
  {
    #region Public Methods

    /// <summary>
    /// Copies this fluent container.
    /// </summary>
    /// <returns>A copy of this fluent container.</returns>
    public abstract FluentsContainer Copy();

    #endregion

    #region IConstantWorld Interface

    /// <summary>
    /// Returns the value of the specified numeric fluent in this world.
    /// </summary>
    /// <param name="fluentID">A numeric fluent ID.</param>
    /// <returns>Unknown, undefined, or the value of the numeric fluent.</returns>
    public abstract FuzzyDouble GetNumericFluent(int fluentID);

    /// <summary>
    /// Returns the value of the specified object fluent in this world.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    /// <returns>Unknown, undefined, or a constant representing the value of the 
    /// object fluent.</returns>
    public abstract FuzzyConstantExp GetObjectFluent(int fluentID);

    #endregion

    #region IWorld Interface

    /// <summary>
    /// Sets the new value of the specified numeric fluent.
    /// </summary>
    /// <param name="fluentID">A numeric fluent ID.</param>
    /// <param name="value">The new value of the numeric fluent.</param>
    public abstract void SetNumericFluent(int fluentID, double value);

    /// <summary>
    /// Sets the new value of the specified object fluent.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    /// <param name="value">The constant representing the new value of the object fluent.
    /// </param>
    public abstract void SetObjectFluent(int fluentID, Constant value);

    /// <summary>
    /// Sets the specified object fluent to undefined.
    /// </summary>
    /// <param name="fluentID">An object fluent ID.</param>
    public abstract void UndefineObjectFluent(int fluentID);

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns whether this fluents container is equal to another fluents container.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>Whether this fluents container is equal to the other object.</returns>
    public override abstract bool Equals(object obj);

    /// <summary>
    /// Returns the hash code of this fluents container.
    /// </summary>
    /// <returns>The hash code of this fluents container.</returns>
    public override abstract int GetHashCode();

    #endregion

    #region IComparable<FluentsContainer> interface

    /// <summary>
    /// Compares this fluents container with another fluents container.
    /// </summary>
    /// <param name="other">The other fluents container to compare this fluents container to.</param>
    /// <returns>An integer representing the total order relation between the two fluents containers.
    /// </returns>
    public abstract int CompareTo(FluentsContainer other);

    #endregion
  }
}
