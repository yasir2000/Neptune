using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exception;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Struct;
using PDDLParser.World;

namespace PDDLParser.Exp.Effect.Assign
{
  /// <summary>
  /// This class represents an object fluent assignment.
  /// </summary>
  public class ObjectAssign : AssignEffect, IInitEl
  {
    /// <summary>
    /// Creates a new object fluent assignment.
    /// </summary>
    /// <param name="head">The object fluent to assign a value to.</param>
    /// <param name="body">The value to assign to the object fluent.</param>
    public ObjectAssign(ObjectFluentApplication head, ITerm body)
      : base("assign", head, body)
    {
      System.Diagnostics.Debug.Assert(head != null && body != null);
    }

    /// <summary>
    /// Updates the world with this fluent assignment.
    /// </summary>
    /// <param name="head">The evaluated fluent application to update.</param>
    /// <param name="updateWorld">The world to update.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    protected override void UpdateWorldWithAssignEffect(FluentApplication head, IDurativeOpenWorld updateWorld,
                                                        LocalBindings bindings)
    {
      FuzzyConstantExp bodyValue = Body.Evaluate(updateWorld, bindings);
      switch (bodyValue.Status)
      {
        case FuzzyConstantExp.State.Defined:
          updateWorld.SetObjectFluent((ObjectFluentApplication)head, bodyValue.Value);
          break;
        case FuzzyConstantExp.State.Unknown:
          throw new UnknownExpException(this.m_body.ToString() + " cannot assign its value to " + 
                                        this.m_head.ToString() + " since it evaluates to unknown!");
        case FuzzyConstantExp.State.Undefined:
          updateWorld.UndefineObjectFluent((ObjectFluentApplication)head);
          break;
        default:
          throw new System.Exception("Invalid FuzzyConstantExp: " + bodyValue.Status);
      }
    }

    /// <summary>
    /// Gets the object fluent to assign a value to.
    /// </summary>
    protected ObjectFluentApplication Head
    {
      get { return (ObjectFluentApplication)this.m_head; }
    }

    /// <summary>
    /// Gets the numeric value to assign to the fluent.
    /// </summary>
    protected ITerm Body
    {
      get { return (ITerm)this.m_body; }
    }
  }
}
