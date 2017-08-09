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
using System.Runtime.Serialization;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term.Type;
using PDDLParser.World;

namespace PDDLParser.Exp.Term
{
  /// <summary>
  /// Base class for terms, which are expressions evaluating to constants.
  /// </summary>
  public abstract class AbstractTerm: AbstractExp, ITerm
  {  
    /// <summary>
    /// The typeset of this term.
    /// </summary>
    protected TypeSet m_typeSet;
    
    /// <summary>
    /// Creates a new term with the specified typeset.
    /// </summary>
    /// <param name="typeset">The typeset of the new term.</param>
    protected AbstractTerm(TypeSet typeset) 
      : base()
    {
      System.Diagnostics.Debug.Assert(typeset != null);

      this.m_typeSet = typeset;
    }

    /// <summary>
    /// Evaluates this term in the specified open world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, unknown, or the constant resulting from the evaluation.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
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
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public abstract TermValue Simplify(IReadOnlyOpenWorld world, LocalBindings bindings);

    /// <summary>
    /// Evaluates this term in the specified closed world.
    /// The bindings should not be modified by this call.
    /// </summary>
    /// <param name="world">The evaluation world.</param>
    /// <param name="bindings">A set of variable bindings.</param>
    /// <returns>Undefined, or the constant resulting from the evaluation.</returns>
    /// <exception cref="PDDLParser.Exception.BindingException">A BindingException is thrown if an attempt
    /// is made to evaluate an unbound variable.</exception>
    public abstract ConstantExp Evaluate(IReadOnlyClosedWorld world, LocalBindings bindings);

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
    /// Returns the typeset of this term.
    /// </summary>
    /// <returns>This term's typeset.</returns>
    public TypeSet GetTypeSet()
    {
      return this.m_typeSet;
    }
  }
}