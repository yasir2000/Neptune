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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PDDLParser.Action;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Term;
using PDDLParser.Exp.Term.Type;

namespace PDDLParser.Parser
{
  /// <summary>
  /// This (incomplete) interface lists the attributes and functions available from a domain. 
  /// </summary>
  public interface IDomain
  {
    /// <summary>
    /// Returns all the requirements necessary to parse this domain.
    /// </summary>
    HashSet<RequireKey> Requirements { get; }

    /// <summary>
    /// Returns the set of all type sets defined in this domain.
    /// </summary>
    TypeSetSet TypeSetSet { get; }

    /// <summary>
    /// Returns all constants defined in this domain.
    /// </summary>
    IDictionary<string, Constant> Constants { get; }

    /// <summary>
    /// Returns all root formulas present in this domain.
    /// </summary>
    IDictionary<string, RootFormula> Formulas { get; }

    /// <summary>
    /// Returns all the actions defined in this domain.
    /// Note that the actions must be stored in the same order they are parsed.
    /// </summary>
    LinkedDictionary<string, IActionDef> Actions { get; }

    /// <summary>
    /// Returns whether this domain contains durative actions.
    /// </summary>
    bool ContainsDurativeActions { get; }

    /// <summary>
    /// Returns the file in which this domain is defined.
    /// </summary>
    string DomainFile { get; }

    /// <summary>
    /// Returns the name of this domain.
    /// </summary>
    string DomainName { get; }
  }
}