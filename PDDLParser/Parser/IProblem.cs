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
using PDDLParser.Exp;
using PDDLParser.Exp.Metric;
using PDDLParser.Exp.Term;

namespace PDDLParser.Parser
{
  /// <summary>
  /// This (incomplete) interface lists the attributes and functions available from a domain.
  /// </summary>
  public interface IProblem
  {
    /// <summary>
    /// Returns all the requirements necessary to parse this problem.
    /// </summary>
    HashSet<RequireKey> Requirements { get; }

    /// <summary>
    /// Returns all constants defined in this problem.
    /// </summary>
    IDictionary<string, Constant> Constants { get; }

    /// <summary>
    /// Returns the metric defined in this problem.
    /// </summary>
    MetricExp Metric { get; }

    /// <summary>
    /// Returns the goal formulation of this problem.
    /// </summary>
    ILogicalExp Goal { get; }

    /// <summary>
    /// Returns the list of initial elements present in this problem.
    /// </summary>
    List<IInitEl> InitialWorld { get; }

    /// <summary>
    /// Returns the file in which this problem is defined.
    /// </summary>
    string ProblemFile { get; }

    /// <summary>
    /// Returns the name of this problem.
    /// </summary>
    string ProblemName { get; }

    /// <summary>
    /// Returns the name of the domain to which this problem refers.
    /// </summary>
    string DomainName { get; }
  }
}