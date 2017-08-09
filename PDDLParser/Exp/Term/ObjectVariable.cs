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
// Please note that this file was inspired in part by the PDDL4J library:
// http://www.math-info.univ-paris5.fr/~pellier/software/software.php 
//
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term.Type;
using PDDLParser.World;

namespace PDDLParser.Exp.Term
{
  /// <summary>
  /// An object variable is a variable bound to an object (a constant from the domain).
  /// </summary>
  [TLPlan]
  public abstract class ObjectVariable : Variable, ITerm
  {
    /// <summary>
    /// The typeset of this object variable.
    /// </summary>
    private TypeSet m_typeSet;

    /// <summary>
    /// An handler for messages transmitted by object variables.
    /// </summary>
    /// <param name="sender">The object variable which sends the message.</param>
    public delegate void VariableEventHandler(ObjectVariable sender);

    /// <summary>
    /// This event is raised whenever the domain of this object variable changes.
    /// This mechanism has been implemented to allow formulas to update their arguments ID
    /// if the domain of one of their argument is modified.
    /// </summary>
    public event VariableEventHandler TypeDomainChanged;

    /// <summary>
    /// Creates a new object variable with the specified name and typeset.
    /// </summary>
    /// <param name="name">The name of the object variable.</param>
    /// <param name="typeSet">The typeset of the object variable.</param>
    public ObjectVariable(string name, TypeSet typeSet)
      : base(name)
    {
      System.Diagnostics.Debug.Assert(typeSet != null);

      this.SetTypeSet(typeSet);
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the constant resulting from the evaluation.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public abstract FuzzyConstantExp Evaluate(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Simplifies this term by evaluating its known expression parts.
    /// The bindings should not be modified by this call.
    /// The resulting expression should not contain any unbound variables, since
    /// they are substituted according to the bindings supplied.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the term (possibly a constant) resulting from 
    /// the simplification.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public abstract TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this term in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the constant resulting from the evaluation.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if this
    /// variable is not bound.</exception>
    public abstract ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings);

    /// <summary>
    /// Returns the typeset of this term.
    /// </summary>
    /// <returns>This term's typeset.</returns>
    public TypeSet GetTypeSet()
    {
      return this.m_typeSet;
    }

    /// <summary>
    /// Verifies whether the specified term can be assigned to this term, 
    /// i.e. if the other term's domain is a subset of this term's domain.
    /// </summary>
    /// <param name="term">The other term.</param>
    /// <returns>True if the types are compatible, false otherwise.</returns>
    public bool CanBeAssignedFrom(ITerm term)
    {
      return (this.GetTypeSet().CanBeAssignedFrom(term.GetTypeSet()));
    }

    /// <summary>
    /// Verifies whether the specified term can be compared to this term,
    /// i.e. if their domain overlap.
    /// </summary>
    /// <param name="term">The other term</param>
    /// <returns>True if the types can be compared, false otherwise.</returns>
    public bool IsComparableTo(ITerm term)
    {
      return (this.GetTypeSet().IsComparableTo(term.GetTypeSet()));
    }

    /// <summary>
    /// Returns a clone of this expression.
    /// </summary>
    /// <returns>A clone of this expression.</returns>
    public override object Clone()
    {
      ObjectParameterVariable var = (ObjectParameterVariable)base.Clone();

      GetTypeSet().TypeDomainChanged += new TypeSet.TypeSetEventHandler(TypeSet_TypeDomainChanged);

      return var;
    }

    /// <summary>
    /// Sets the typset of this object variable. Registration must be done on the TypeDomainChanged
    /// event of the typeset in the case where its domain changes.
    /// </summary>
    /// <param name="typeSet">The typeset of this object variable.</param>
    internal void SetTypeSet(TypeSet typeSet)
    {
      if (this.m_typeSet != null)
      {
        this.m_typeSet.TypeDomainChanged -= new TypeSet.TypeSetEventHandler(TypeSet_TypeDomainChanged); 
      }
      this.m_typeSet = typeSet;
      this.m_typeSet.TypeDomainChanged += new TypeSet.TypeSetEventHandler(TypeSet_TypeDomainChanged);
    }

    /// <summary>
    /// Handles typeset TypeDomainChanged events.
    /// </summary>
    /// <param name="sender">The typeset which sent the event.</param>
    private void TypeSet_TypeDomainChanged(TypeSet sender)
    {
      FireTypeDomainChanged();
    }

    /// <summary>
    /// Fires a TypeDomainChanged event.
    /// </summary>
    private void FireTypeDomainChanged()
    {
      if (TypeDomainChanged != null)
        TypeDomainChanged(this);
    }

    /// <summary>
    /// Returns a typed string representation of this expression.
    /// </summary>
    /// <returns>A typed string representation of this expression.</returns>
    public override string ToTypedString()
    {
      return this.ToString() + " - " + this.GetTypeSet().ToString();
    }

  }
}
