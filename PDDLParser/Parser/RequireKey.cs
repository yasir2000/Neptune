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

namespace PDDLParser.Parser
{
  /// <summary>
  /// A require key indicates the specific functionality needed by a planner to attempt 
  /// to solve a PDDL domain or problem.
  /// </summary>
  public class RequireKey
  {
    /// <summary>
    /// The PDDL :strips requirement.
    /// </summary>
    public static readonly RequireKey STRIPS = new RequireKey(":strips");
    /// <summary>
    /// The PDDL :typing requirement.
    /// </summary>
    public static readonly RequireKey TYPING = new RequireKey(":typing");
    /// <summary>
    /// The PDDL :negative-preconditions requirement.
    /// </summary>
    public static readonly RequireKey NEGATIVE_PRECONDITIONS = new RequireKey(":negative-preconditions");
    /// <summary>
    /// The PDDL :disjunctive-preconditions requirement.
    /// </summary>
    public static readonly RequireKey DISJUNCTIVE_PRECONDITIONS = new RequireKey(":disjunctive-preconditions");
    /// <summary>
    /// The PDDL :equality requirement.
    /// </summary>
    public static readonly RequireKey EQUALITY = new RequireKey(":equality");
    /// <summary>
    /// The PDDL :existential-preconditions requirement.
    /// </summary>
    public static readonly RequireKey EXISTENTIAL_PRECONDITIONS = new RequireKey(":existential-preconditions");
    /// <summary>
    /// The PDDL :universal-preconditions requirement.
    /// </summary>
    public static readonly RequireKey UNIVERSAL_PRECONDITIONS = new RequireKey(":universal-preconditions");
    /// <summary>
    /// The PDDL :quantified-preconditions requirement.
    /// </summary>
    public static readonly RequireKey QUANTIFIED_PRECONDITIONS = new RequireKey(":quantified-preconditions");
    /// <summary>
    /// The PDDL :conditional-effects requirement.
    /// </summary>
    public static readonly RequireKey CONDITIONAL_EFFECTS = new RequireKey(":conditional-effects");
    /// <summary>
    /// The PDDL :numeric-fluents requirement.
    /// </summary>
    public static readonly RequireKey NUMERIC_FLUENTS = new RequireKey(":numeric-fluents");
    /// <summary>
    /// The PDDL :object-fluents requirement.
    /// </summary>
    public static readonly RequireKey OBJECT_FLUENTS = new RequireKey(":object-fluents");
    /// <summary>
    /// The PDDL :fluents requirement.
    /// </summary>
    public static readonly RequireKey FLUENTS = new RequireKey(":fluents");
    /// <summary>
    /// The PDDL :adl requirement.
    /// </summary>
    public static readonly RequireKey ADL = new RequireKey(":adl");
    /// <summary>
    /// The PDDL :durative-actions requirement.
    /// </summary>
    public static readonly RequireKey DURATIVE_ACTIONS = new RequireKey(":durative-actions");
    /// <summary>
    /// The PDDL :derived-predicates requirement.
    /// </summary>
    public static readonly RequireKey DERIVED_PREDICATES = new RequireKey(":derived-predicates");
    /// <summary>
    /// The PDDL :timed-initial-literals requirement.
    /// </summary>
    public static readonly RequireKey TIMED_INITIAL_LITERALS = new RequireKey(":timed-initial-literals");
    /// <summary>
    /// The PDDL :preferences requirement.
    /// </summary>
    public static readonly RequireKey PREFERENCES = new RequireKey(":preferences");
    /// <summary>
    /// The PDDL :constraints requirement.
    /// </summary>
    public static readonly RequireKey CONSTRAINTS = new RequireKey(":constraints");
    /// <summary>
    /// The PDDL :continuous-effects requirement.
    /// </summary>
    public static readonly RequireKey CONTINOUS_EFFECTS = new RequireKey(":continous-effects");
    /// <summary>
    /// The PDDL :duration-inequalities requirement.
    /// </summary>
    public static readonly RequireKey DURATION_INEQUALITIES = new RequireKey(":duration-inequalities");
    /// <summary>
    /// The TLPlan requirement (TLPlan-specific constructs).
    /// </summary>
    public static readonly RequireKey TLPLAN = new RequireKey(":tlplan");

    /// <summary>
    /// The set of all require keys.
    /// </summary>
    public static readonly RequireKey[] AllKeys = new RequireKey[] {
                                  STRIPS,
                                  TYPING,
                                  NEGATIVE_PRECONDITIONS,
                                  DISJUNCTIVE_PRECONDITIONS,
                                  EQUALITY,
                                  EXISTENTIAL_PRECONDITIONS,
                                  UNIVERSAL_PRECONDITIONS,
                                  QUANTIFIED_PRECONDITIONS,
                                  CONDITIONAL_EFFECTS,
                                  NUMERIC_FLUENTS,
                                  OBJECT_FLUENTS,
                                  FLUENTS,
                                  ADL,
                                  DURATIVE_ACTIONS,
                                  DERIVED_PREDICATES,
                                  TIMED_INITIAL_LITERALS,
                                  PREFERENCES,
                                  CONSTRAINTS,
                                  CONTINOUS_EFFECTS,
                                  DURATION_INEQUALITIES,
                                  TLPLAN
                              };

    /// <summary>
    /// The image associated with this require key.
    /// </summary>
    private readonly string image;

    /// <summary>
    /// Creates a new require key with a specific image.
    /// </summary>
    /// <param name="image">The image of the new require key.</param>
    private RequireKey(string image)
    {
      this.image = image;
    }

    /// <summary>
    /// Returns the require key that corresponds to the specified image.
    /// </summary>
    /// <param name="image">The image of the require key to find.</param>
    /// <returns>The require key that corresponds to the specified image.</returns>
    public static RequireKey GetRequireKey(string image)
    {
      foreach (RequireKey rk in AllKeys)
      {
        if (rk.image.Equals(image))
        {
          return rk;
        }
      }
      return null;
    }

    /// <summary>
    /// Returns the image of this require key.
    /// </summary>
    /// <returns>The image of this require key.</returns>
    public string GetImage()
    {
      return this.image;
    }

    /// <summary>
    /// Returns a string representation of this require key.
    /// </summary>
    /// <returns>A string representation of this require key.</returns>
    public override string ToString()
    {
      return image;
    }
  }
}
