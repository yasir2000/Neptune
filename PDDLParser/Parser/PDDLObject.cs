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
using System.IO;
using PDDLParser.Action;
using PDDLParser.Exp;
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Logical.TLPlan;
using PDDLParser.Exp.Metric;
using PDDLParser.Exp.Term;
using PDDLParser.Exp.Term.Type;
using PDDLParser.Extensions;
using PDDLParser.World;
using Type = PDDLParser.Exp.Term.Type.Type;

namespace PDDLParser.Parser
{
  /// <summary>
  /// A PDDL object is a data structure representing either a parsed 
  /// PDDL domain or problem.
  /// </summary>
  public class PDDLObject : IDomain, IProblem
  {
    #region private class GetAllGoalWorldsDeferredExecution

    /// <summary>
    /// This class is used to defer the actual assignation of goal modalities delegate function,
    /// since these are parsed in the domain while their delegate function must point to the
    /// PDDL problem's function instead.
    /// </summary>
    public class GetAllGoalWorldsDeferredExecution
    {
      /// <summary>
      /// The actual delegate to which the [getAllGoalWorlds()] call must be forwarded.
      /// </summary>
      public GoalModalityExp.GetAllGoalWorldsDelegate Del { get; set; }

      /// <summary>
      /// Enumerates all the worlds within which this logical expression evaluates to true.
      /// This call is simply forwarded to the inner delegate.
      /// </summary>
      /// <returns>All the worlds satisfying this logical expression.</returns>
      public HashSet<PartialWorld> getAllGoalWorlds()
      {
        return Del();
      }
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// This enumeration lists all the the possible types of PDDL objects.
    /// </summary>
    public enum PDDLContent
    {
      /// <summary>
      /// A PDDL domain.
      /// </summary>
      DOMAIN,
      /// <summary>
      /// A PDDL partial problem.
      /// </summary>
      PARTIAL_PROBLEM,
      /// <summary>
      /// A PDDL full problem (domain + problem information).
      /// </summary>
      FULL_PROBLEM,
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The domain name of this PDDL object.
    /// </summary>
    protected string m_domainName;

    /// <summary>
    /// The problem name of this PDDL object.
    /// </summary>
    protected string m_problemName;

    /// <summary>
    /// The type of PDDL object.
    /// </summary>
    protected PDDLContent m_content;

    /// <summary>
    /// The requirements needed to manipulate this PDDL object.
    /// </summary>
    protected HashSet<RequireKey> m_requirements;

    /// <summary>
    /// The constants present in this PDDL object.
    /// </summary>
    protected IDictionary<string, Constant> m_constants;

    /// <summary>
    /// The root formulas present in this PDDL object.
    /// </summary>
    protected IDictionary<string, RootFormula> m_formulas;

    /// <summary>
    /// The trajectory constraints defined in this PDDL object.
    /// </summary>
    protected IConstraintExp m_constraints;

    /// <summary>
    /// The actions defined in this PDDL object.
    /// </summary>
    protected LinkedDictionary<string, IActionDef> m_actions;

    /// <summary>
    /// The list of elements defined in the initial state.
    /// </summary>
    protected List<IInitEl> m_init;

    /// <summary>
    /// The goal formulation of this PDDL object.
    /// </summary>
    protected ILogicalExp m_goal;

    /// <summary>
    /// The metric specification of this PDDL object.
    /// </summary>
    protected MetricExp m_metric;

    /// <summary>
    /// The file where the domain of this PDDL object is defined.
    /// </summary>
    protected string m_domainFile;

    /// <summary>
    /// The file where the problem of this PDDL object is defined.
    /// </summary>
    protected string m_problemFile;

    /// <summary>
    /// The set of all type sets defined in this PDDL object.
    /// </summary>
    protected TypeSetSet m_typeSetSet;

    /// <summary>
    /// A wrapper over a delegate responsible for fetching goal worlds 
    /// (for goal modalities expressions).
    /// This delegate must ultimately be implemented by the linked PDDL object.
    /// </summary>
    private GetAllGoalWorldsDeferredExecution m_getAllGoalWorlds;

    /// <summary>
    /// The cached set of goal worlds.
    /// </summary>
    private HashSet<PartialWorld> m_allGoalWorlds;

    /// <summary>
    /// Verifies whether the domain contains durative actions.
    /// </summary>
    private bool m_containsDurativeActions;

    /// <summary>
    /// Counts the number of dummy formulas created with CreateDummyFormula.
    /// </summary>
    private int m_uniqueNameCounter;

    /// <summary>
    /// This is only used in testing, in order to parse the dummy formulas created by
    /// CreateDummyFormula.
    /// </summary>
    private bool m_createParsableUniqueNames;

    /// <summary>
    /// This dictionary contains all existing predicates, functions and constants.
    /// </summary>
    private IDictionary<string, string> m_existingNames;

    /// <summary>
    /// All preferences listed in this PDDL object.
    /// </summary>
    private IDictionary<string, List<IPrefExp>> m_allPreferences;

    /// <summary>
    /// The counters associated with the preferences of this PDDL object.
    /// (Only preferences over actions preconditions have a corresponding counter).
    /// There is a unique counter per name, hence the storage in a dictionary.
    /// </summary>
    private IDictionary<string, NumericFluentApplication> m_allPreferenceCounters;

    /// <summary>
    /// The constraint preferences listed in this PDDL object.
    /// </summary>
    private List<IConstraintPrefExp> m_constraintPreferences;

    /// <summary>
    /// The is-violated expressions listed in this PDDL object.
    /// </summary>
    private List<IsViolatedExp> m_allIsViolatedExps;

    #endregion

    #region Properties

    /// <summary>
    /// The type of PDDL object.
    /// </summary>
    public PDDLContent Content
    {
      get { return m_content; }
      internal set { m_content = value; }
    }

    /// <summary>
    /// The domain name.
    /// </summary>
    public string DomainName
    {
      get { return m_domainName; }
      set { m_domainName = value; }
    }

    /// <summary>
    /// The problem name.
    /// </summary>
    public string ProblemName
    {
      get { return m_problemName; }
      set { m_problemName = value; }
    }

    /// <summary>
    /// The requirements needed to manipulate this PDDL object.
    /// </summary>
    public HashSet<RequireKey> Requirements
    {
      get { return m_requirements; }
      set { m_requirements = value; }
    }

    /// <summary>
    /// The constants present in this PDDL object.
    /// </summary>
    public IDictionary<string, Constant> Constants
    {
      get { return m_constants; }
      set { m_constants = value; }
    }

    /// <summary>
    /// The set of all type sets defined in this PDDL object.
    /// </summary>
    public TypeSetSet TypeSetSet
    {
      get { return m_typeSetSet; }
      protected internal set { m_typeSetSet = value; }
    }

    /// <summary>
    /// The root formulas present in this PDDL object.
    /// </summary>
    public IDictionary<string, RootFormula> Formulas
    {
      get { return m_formulas; }
      set { m_formulas = value; }
    }

    /// <summary>
    /// Returns all invariant described formulas (described formulas which are never modified
    /// by actions effects, timed initial literals, or anything else).
    /// </summary>
    public IEnumerable<DescribedFormula> InvariantDescribedFormulas
    {
      get { return m_formulas.Values.OfType<DescribedFormula>().Where(f => f.Invariant); }
    }

    /// <summary>
    /// Returns all variant described formulas (described formulas which can be modified by 
    /// actions effects, timed initial literals, or something else).
    /// </summary>
    public IEnumerable<DescribedFormula> VariantDescribedFormulas
    {
      get { return m_formulas.Values.OfType<DescribedFormula>().Where(f => !f.Invariant); }
    }

    /// <summary>
    /// The trajectory constraints defined in this PDDL object.
    /// </summary>
    public IConstraintExp Constraints
    {
      get { return m_constraints; }
      set { m_constraints = value; }
    }

    /// <summary>
    /// The actions defined in this PDDL object.
    /// </summary>
    public LinkedDictionary<string, IActionDef> Actions
    {
      get { return m_actions; }
      set { m_actions = value; }
    }

    /// <summary>
    /// The list of elements defined in the initial state.
    /// </summary>
    public List<IInitEl> InitialWorld
    {
      get { return m_init; }
      set { m_init = value; }
    }

    /// <summary>
    /// The goal formulation of this PDDL object.
    /// </summary>
    public ILogicalExp Goal
    {
      get { return m_goal; }
      set { m_goal = value; }
    }

    /// <summary>
    /// The metric specification of this PDDL object.
    /// </summary>
    public MetricExp Metric
    {
      get { return m_metric; }
      set { m_metric = value; }
    }

    /// <summary>
    /// The file where the domain of this PDDL object is defined.
    /// </summary>
    public string DomainFile
    {
      get { return m_domainFile; }
      internal set { m_domainFile = value; }
    }

    /// <summary>
    /// The file where the problem of this PDDL object is defined.
    /// </summary>
    public string ProblemFile
    {
      get { return m_problemFile; }
      internal set { m_problemFile = value; }
    }

    /// <summary>
    /// Verifies whether the domain contains durative actions.
    /// </summary>
    public bool ContainsDurativeActions
    {
      get { return this.m_containsDurativeActions; }
      set { this.m_containsDurativeActions = value; }
    }

    /// <summary>
    /// Returns the dictionary of the names of all existing predicates, functions and constants.
    /// </summary>
    public IDictionary<string, string> ExistingNames
    {
      get { return this.m_existingNames; }
    }

    /// <summary>
    /// All preferences listed in this PDDL object.
    /// </summary>
    public IDictionary<string, List<IPrefExp>> AllPreferences
    {
      get { return m_allPreferences; }
    }

    /// <summary>
    /// The counters associated with the preferences of this PDDL object.
    /// </summary>
    public IDictionary<string, NumericFluentApplication> AllPreferenceCounters
    {
      get { return m_allPreferenceCounters; }
    }

    /// <summary>
    /// The constraint preferences listed in this PDDL object.
    /// </summary>
    public List<IConstraintPrefExp> ConstraintPreferences
    {
      get { return m_constraintPreferences; }
      private set { m_constraintPreferences = value; }
    }

    /// <summary>
    /// The is-violated expressions listed in this PDDL object.
    /// </summary>
    public List<IsViolatedExp> AllIsViolatedExpressions
    {
      get { return m_allIsViolatedExps; }
      internal set { m_allIsViolatedExps = value; }
    }

    /// <summary>
    /// A wrapper over a delegate responsible for fetching goal worlds 
    /// (for goal modalities expressions).
    /// This delegate must ultimately be implemented by the linked PDDL object.
    /// </summary>
    public GetAllGoalWorldsDeferredExecution GetAllGoalWorldsInstance
    {
      get
      {
        return m_getAllGoalWorlds;
      }
      set
      {
        m_getAllGoalWorlds = value;
        m_getAllGoalWorlds.Del = this.GetAllGoalWorlds;
      }
    }

    /// <summary>
    /// Enumerates all the worlds within which this logical expression evaluates to true.
    /// This method is used to support the goal modality expressions.
    /// A cache is used to store the hashset of goal worlds.
    /// </summary>
    /// <returns>All the worlds satisfying this logical expression.</returns>
    public HashSet<PartialWorld> GetAllGoalWorlds()
    {
      if (m_allGoalWorlds == null)
        m_allGoalWorlds = this.Goal.EnumerateAllSatisfyingWorlds();

      return m_allGoalWorlds;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new PDDL object.
    /// </summary>
    public PDDLObject()
    {
      this.m_content = PDDLContent.DOMAIN;
      this.m_domainFile = null;
      this.m_problemFile = null;
      this.m_domainName = null;
      this.m_problemName = null;
      this.m_requirements = new HashSet<RequireKey>();
      this.m_requirements.Add(RequireKey.STRIPS);
      this.m_constants = new Dictionary<string, Constant>();
      this.m_constraints = TrueExp.True;
      this.m_actions = new LinkedDictionary<string, IActionDef>();
      this.m_init = new List<IInitEl>();
      this.m_goal = TrueExp.True;
      this.m_metric = null;
      this.m_formulas = new Dictionary<string, RootFormula>();
      this.m_typeSetSet = null;
      this.m_containsDurativeActions = false;
      this.m_existingNames = new Dictionary<string, string>();
      this.m_allPreferences = new Dictionary<string, List<IPrefExp>>();
      this.m_allPreferenceCounters = new Dictionary<string, NumericFluentApplication>();
      this.m_constraintPreferences = new List<IConstraintPrefExp>();
      this.m_allIsViolatedExps = new List<IsViolatedExp>();

      this.m_uniqueNameCounter = 0;
      this.m_createParsableUniqueNames = false;

      this.m_allGoalWorlds = null;
      this.m_getAllGoalWorlds = new GetAllGoalWorldsDeferredExecution();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Preprocesses this PDDL object.
    /// This function assigns unique IDs to described formulas, assigns type sets where needed,
    /// and does a lot of other cool stuff.
    /// </summary>
    public void Preprocess()
    {
      // Preprocess each typeset (assign constant IDs, create the domain cache)
      foreach (TypeSet typeSet in this.TypeSetSet)
      {
        typeSet.Preprocess();
      }

      // Update the constraint preferences (they need the typesets' domains)
      List<IConstraintPrefExp> constraintPreferences = new List<IConstraintPrefExp>();
      constraintPreferences.AddRange(this.ConstraintPreferences.Select(pref => pref.GetAllSubstitutedConstraintPreferences()).Flatten());
      this.ConstraintPreferences = constraintPreferences;

      // Update the durative actions' overall preferences (they also need the typesets' domains)
      foreach (List<IPrefExp> lst in this.AllPreferences.Values)
      {
        List<IPrefExp> removedPrefs = new List<IPrefExp>();
        List<IPrefExp> addedPrefs = new List<IPrefExp>();

        foreach (OverAllConditionPrefExp pref in lst.OfType<OverAllConditionPrefExp>())
        {
          List<IEffect> overEffects = new List<IEffect>();
          List<IEffect> endEffects = new List<IEffect>();

          removedPrefs.Add(pref);
          foreach (OverAllConditionPrefExp overallPref in pref.GetAllSubstitutedConditionPreferences())
          {
            addedPrefs.Add(overallPref);
            overallPref.SetAtom(this.CreateDummyFormula("pref-overall", false, false));

            // Add the overall effect
            overEffects.Add(overallPref.GetOverallEffect());
            // Add the end effect
            endEffects.Add(overallPref.ConvertToEffect(this.GetPreferenceCounter(overallPref.Name)));
          }

          DurativeAction action = pref.GetAction();

          pref.GetAction().OverallEffect = new AndEffect(pref.GetAction().OverallEffect.Once().Concat(overEffects));
          pref.GetAction().EndEffect = new AndEffect(pref.GetAction().EndEffect.Once().Concat(endEffects));
        }

        foreach (IPrefExp pref in removedPrefs)
          lst.Remove(pref);
        lst.AddRange(addedPrefs);
      }

      // Preprocess the IsViolated expressions (surprisingly, they need the typesets' domains)
      foreach (IsViolatedExp exp in this.AllIsViolatedExpressions)
      {
        IEnumerable<ILogicalExp> goalPrefs = this.AllPreferences[exp.PreferenceName].OfType<IConditionPrefExp>()
                                                                                    .Where(pref => pref.IsGoalPreference)
                                                                                    .Select(pref => pref.GetAllSubstitutedConditionPreferences())
                                                                                    .Flatten()
                                                                                    .Select(pref => pref.GetViolationCondition());
        IEnumerable<AtomicFormulaApplication> constraintPrefs = this.AllPreferences[exp.PreferenceName].OfType<IConstraintPrefExp>()
                                                                                                       .Select(pref => pref.GetAllSubstitutedConstraintPreferences())
                                                                                                       .Flatten()
                                                                                                       .Select(pref => pref.GetDummyAtomicFormula());
        exp.SetGoalPreferences(goalPrefs);
        exp.SetConstraintPreferences(constraintPrefs);
      }
    }

    /// <summary>
    /// Retrieves all invariant described formulas in this PDDL object.
    /// </summary>
    /// <returns>All invariant described formulas in this PDDL object.</returns>
    public HashSet<DescribedFormula> GetInvariantDescribedFormulas()
    {
      // Compute all modifiable described formulas
      HashSet<DescribedFormula> variantDescribedFormulas = new HashSet<DescribedFormula>();
      foreach (KeyValuePair<string, IActionDef> action in Actions)
        foreach (IEffect effect in action.Value.GetAllEffects())
          variantDescribedFormulas.UnionWith(effect.GetModifiedDescribedFormulas());

      IEnumerable<TimedLiteral> timedLiterals = InitialWorld.OfType<TimedLiteral>();
      variantDescribedFormulas.UnionWith(timedLiterals.Select(timedL => timedL.Predicate).Cast<DescribedFormula>());
      variantDescribedFormulas.UnionWith(this.AllPreferenceCounters.Values.Select<NumericFluentApplication, DescribedFormula>(fluent => fluent.RootFluent));
      variantDescribedFormulas.UnionWith(this.ConstraintPreferences.Select(pref => pref.GetDummyAtomicFormula().Predicate).Cast<DescribedFormula>());

      // Compute all invariant described formulas
      HashSet<DescribedFormula> invariantDescribedFormulas = new HashSet<DescribedFormula>(Formulas.Values.OfType<DescribedFormula>());
      invariantDescribedFormulas.ExceptWith(variantDescribedFormulas);

      return invariantDescribedFormulas;
    }

    /// <summary>
    /// Returns a new unique name used by dummy atomic formulas.
    /// </summary>
    /// <param name="prefix">The prefix to use.</param>
    /// <returns>A new unique name used by dummy atomic formulas.</returns>
    public string CreateUniqueName(string prefix)
    {
      return string.Format("{0}{1}_{2}{3}", (m_createParsableUniqueNames ? string.Empty : "?"),
                                             prefix,
                                            (Content == PDDLContent.DOMAIN ? "dom" : "pb"),
                                             m_uniqueNameCounter++);
    }

    /// <summary>
    /// Creates a dummy atomic formula application with the given attributes.
    /// </summary>
    /// <param name="prefix">The prefix to use.</param>
    /// <param name="args">The arguments of the formula.</param>
    /// <param name="detectCycles">Whether the cycle detection mechanism should use this formula.</param>
    /// <param name="addToKnownPredicates">Whether to add this formula to the set of known predicates.</param>
    /// <returns>A new atomic formula application generated from the specified attributes.</returns>
    public AtomicFormulaApplication CreateDummyFormula(string prefix, List<ObjectParameterVariable> args, 
                                                       bool detectCycles, bool addToKnownPredicates)
    {
      // Create an unparsable root formula
      string name = CreateUniqueName(prefix);
      AtomicFormula rootAtom = new AtomicFormula(name, args, new DescribedFormula.Attributes(detectCycles));
      if (addToKnownPredicates)
        Formulas.Add(rootAtom.Name, rootAtom);

      AtomicFormulaApplication atom = new AtomicFormulaApplication(rootAtom, new List<ITerm>(args.Cast<ITerm>()));
      return atom;
    }

    /// <summary>
    /// Creates a dummy atomic formula application with the given attributes.
    /// </summary>
    /// <param name="prefix">The prefix to use.</param>
    /// <param name="detectCycles">Whether the cycle detection mechanism should use this formula.</param>
    /// <param name="addToKnownPredicates">Whether to add this formula to the set of known predicates.</param>
    /// <returns>A new atomic formula application generated from the specified attributes.</returns>
    public AtomicFormulaApplication CreateDummyFormula(string prefix, bool detectCycles, bool addToKnownPredicates)
    {
      return CreateDummyFormula(prefix, new List<ObjectParameterVariable>(), detectCycles, addToKnownPredicates);
    }

    /// <summary>
    /// Returns the counter associated with the given preference name, and creates it if necessary.
    /// Preference counters are only created in domains, since they can only result from the
    /// transformation of action condition preferences.
    /// </summary>
    /// <param name="prefName">Preference name</param>
    /// <returns>The preference counter</returns>
    public NumericFluentApplication CreatePreferenceCounter(string prefName)
    {
      NumericFluentApplication counter;
      if (!m_allPreferenceCounters.TryGetValue(prefName, out counter))
      {
        string counterName = string.Format("?pref-counter-{0}", prefName);
        RootFormula counterRoot;

        if (!Formulas.TryGetValue(counterName, out counterRoot))
        {
          // The counter should not affect the world's hashcode
          counterRoot = new NumericFluent(counterName, 
                                          new List<ObjectParameterVariable>(), 
                                          new DescribedFormula.Attributes(false));
          Formulas[counterName] = counterRoot;
        }

        counter = new NumericFluentApplication((NumericFluent)counterRoot, new List<ITerm>());
        m_allPreferenceCounters[prefName] = counter;
      }

      return counter;
    }

    /// <summary>
    /// Returns the counter associated with the given preference name, without creating it.
    /// </summary>
    /// <param name="prefName">Preference name</param>
    /// <returns>The preference counter, or null if it does not exist.</returns>
    /// <seealso cref="CreatePreferenceCounter"/>
    public NumericFluentApplication GetPreferenceCounter(string prefName)
    {
      NumericFluentApplication counter;
      m_allPreferenceCounters.TryGetValue(prefName, out counter);
      return counter;
    }

    /// <summary>
    /// Creates a new unnamed condition preference.
    /// Note that this new condition preference is assigned a unique name.
    /// </summary>
    /// <param name="exp">The preference condition.</param>
    /// <returns>A new condition preference with a generated unique name.</returns>
    public IConditionPrefExp CreateUnnamedConditionPreference(ILogicalExp exp)
    {
      string prefName = CreateUniqueName("UnnamedPref");
      return new ConditionPrefExp(prefName, exp, true);
    }

    /// <summary>
    /// Creates a new unnamed constraint preference with the given variables.
    /// Note that this new constraint preference is assigned a unique name.
    /// </summary>
    /// <param name="exp">The constraint expression.</param>
    /// <param name="vars">The list of variables used in the constraint.</param>
    /// <returns>A new unnamed constraint preference with the given variables</returns>
    public IConstraintPrefExp CreateUnnamedConstraintPreference(IConstraintExp exp, List<ObjectParameterVariable> vars)
    {
      AtomicFormulaApplication atom = this.CreateDummyFormula("UnnamedPref", vars, false, true);
      return new ConstraintPrefExp(atom.Name, exp, atom, true);
    }

    /// <summary>
    /// Adds a new preference to the list of preferences.
    /// </summary>
    /// <param name="pref">The new preference to add.</param>
    public void AddPreference(IPrefExp pref)
    {
      List<IPrefExp> prefs;

      if (!AllPreferences.TryGetValue(pref.Name, out prefs))
      {
        prefs = new List<IPrefExp>();
        AllPreferences[pref.Name] = prefs;
      }

      prefs.Add(pref);
    }

    #region ToString Methods

    /// <summary>
    /// Returns a string representation as a domain.
    /// </summary>
    /// <returns>A string representation as a domain.</returns>
    private string ToDomainString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(define (domain " + this.m_domainName + ")\n");
      str.Append(this.RequirementsToString());
      str.Append(this.TypesToString());
      str.Append(this.ConstantsToString("constants"));
      str.Append(this.PredicatesToString());
      str.Append(this.FluentsToString());
      str.Append(this.ConstraintsToString());
      str.Append(this.DefinedFormulasToString());
      str.Append(this.ActionsToString());
      return str.ToString();

    }

    /// <summary>
    /// Returns a string representation as a partial problem.
    /// </summary>
    /// <returns>A string representation as a partial problem.</returns>
    private string ToPartialProblemString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(define (problem " + this.m_problemName + ")\n");
      str.Append("(:domain " + this.m_domainName + ")\n");
      str.Append(this.RequirementsToString());
      str.Append(this.ConstantsToString("objects"));
      str.Append(this.ConstraintsToString());
      str.Append(this.InitToString());
      str.Append(this.GoalToString());
      str.Append(this.MetricToString());
      return str.ToString();

    }

    /// <summary>
    /// Returns a string representation as a full problem.
    /// </summary>
    /// <returns>A string representation as a full problem.</returns>
    private string ToFullProblemString()
    {
      StringBuilder str = new StringBuilder();
      str.Append("(:problem " + this.m_problemName + ")\n");
      str.Append("(:domain " + this.m_domainName + ")\n");
      str.Append(this.RequirementsToString());
      str.Append(this.TypesToString());
      str.Append(this.ConstantsToString("constants"));
      str.Append(this.PredicatesToString());
      str.Append(this.FluentsToString());
      str.Append(this.ConstraintsToString());
      str.Append(this.DefinedFormulasToString());
      str.Append(this.ActionsToString());
      str.Append(this.InitToString());
      str.Append(this.GoalToString() + "\n");
      str.Append(this.MetricToString());
      return str.ToString();

    }

    /// <summary>
    /// Returns a string representation of the requirements.
    /// </summary>
    /// <returns>A string representation of the requirements.</returns>
    private string RequirementsToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.m_requirements.Count != 0)
      {
        str.Append("(:requirements");
        foreach (RequireKey rk in this.m_requirements)
        {
          str.Append(" " + rk.ToString());
        }
        str.Append(")\n");
      }
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of all the types.
    /// </summary>
    /// <returns>A string representation of all the types.</returns>
    private string TypesToString()
    {
      StringBuilder str = new StringBuilder();
      foreach (TypeSet typeSet in this.TypeSetSet)
      {
        str.Append("\n   " + typeSet.ToString());
        HashSet<Type> superTypes = typeSet.SuperTypes;
        if (superTypes.Count != 0)
        {
          str.Append(" -");
          IEnumerator<Type> i = superTypes.GetEnumerator();
          if (superTypes.Count > 1)
          {
            str.Append(" (either");
            while (i.MoveNext())
            {
              str.Append(" " + i.Current.ToString());
            }
            str.Append(")");
          }
          else
          {
            i.MoveNext();
            str.Append(" " + i.Current.ToString());
          }
        }
      }

      return (str.Length != 0 ? "(:types" + str.ToString() + ")\n" : string.Empty);
    }

    /// <summary>
    /// Returns a string representation of the constants.
    /// </summary>
    /// <param name="label">The label to use.</param>
    /// <returns>A string representation of the constants.</returns>
    private string ConstantsToString(string label)
    {
      StringBuilder str = new StringBuilder();
      if (this.m_constants.Count != 0)
      {
        str.Append("(:" + label);
        foreach (Constant cst in this.m_constants.Values)
        {
          if (this.m_requirements.Contains(RequireKey.TYPING))
          {
            str.Append("\n   " + cst.ToTypedString());
          }
          else
          {
            str.Append(" " + cst.ToString());
          }
        }
        str.Append(")\n");
      }
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the predicates.
    /// </summary>
    /// <returns>A string representation of the predicates.</returns>
    private string PredicatesToString()
    {
      StringBuilder str = new StringBuilder();

      IEnumerable<AtomicFormula> atomicFormulas = this.Formulas.Values.OfType<AtomicFormula>();
      str.Append("(:predicates");
      foreach (AtomicFormula atom in atomicFormulas)
      {
        if (this.m_requirements.Contains(RequireKey.TYPING))
        {
          str.Append("\n   " + atom.ToTypedString());
        }
        else
        {
          str.Append(" " + atom.ToString());
        }
      }
      str.Append(")\n");

      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the fluents.
    /// </summary>
    /// <returns>A string representation of the fluents.</returns>
    private string FluentsToString()
    {
      StringBuilder str = new StringBuilder();

      IEnumerable<Fluent> fluents = this.Formulas.Values.OfType<Fluent>();
      str.Append("(:fluents");
      foreach (Fluent func in fluents)
      {
        if (this.m_requirements.Contains(RequireKey.TYPING))
        {
          str.Append("\n   " + func.ToTypedString());
        }
        else
        {
          str.Append("\n   " + func.ToString());
        }
      }
      str.Append(")\n");

      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the constraints.
    /// </summary>
    /// <returns>A string representation of the constraints.</returns>
    private string ConstraintsToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.Constraints != null)
      {
        str.Append("(:constraints");
        if (this.m_requirements.Contains(RequireKey.TYPING))
          str.Append("\n   " + this.Constraints.ToTypedString());
        else
          str.Append("\n   " + this.Constraints.ToString());
        str.Append(")\n");
      }
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the defined formulas.
    /// </summary>
    /// <returns>A string representation of the defined formulas.</returns>
    private string DefinedFormulasToString()
    {
      StringBuilder str = new StringBuilder();

      IEnumerable<DefinedFormula> definedFunctions = this.Formulas.Values.OfType<DefinedFormula>();
      foreach (DefinedFormula definedFunction in definedFunctions)
      {
        if (this.m_requirements.Contains(RequireKey.TYPING))
        {
          str.Append(definedFunction.ToTypedString());
        }
        else
        {
          str.Append(definedFunction.ToString());
        }
        str.Append("\n");
      }

      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the actions.
    /// </summary>
    /// <returns>A string representation of the actions.</returns>
    private string ActionsToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.m_actions.Count != 0)
      {
        foreach (IActionDef action in this.m_actions.Values)
        {
          str.Append(action.ToTypedString());
          str.Append("\n");
        }
      }
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the metric.
    /// </summary>
    /// <returns>A string representation of the metric.</returns>
    private string MetricToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.m_metric != null)
      {
        str.Append(this.m_metric.ToString());
      }
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the initial elements.
    /// </summary>
    /// <returns>A string representation of the initial elements.</returns>
    private string InitToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.m_init.Count != 0)
      {
        str.Append("(:init");
        foreach (IInitEl el in this.m_init)
        {
          str.Append("\n   ");
          str.Append(el.ToString());
        }
        str.Append(")\n");
      }
      return str.ToString();
    }

    /// <summary>
    /// Returns a string representation of the goal.
    /// </summary>
    /// <returns>A string representation of the goal.</returns>
    private string GoalToString()
    {
      StringBuilder str = new StringBuilder();
      if (this.m_goal != null)
      {
        str.Append("(:goal ");
        str.Append("\n   ");
        str.Append(m_goal.ToString());
        str.Append(")");
      }
      return str.ToString();
    }

    #endregion

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns true if this PDDL object is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to test for equality.</param>
    /// <returns>True if this PDDL object is equal to the other object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
      if (obj.GetType() == this.GetType())
      {
        PDDLObject other = (PDDLObject)obj;
        return this.m_domainName.Equals(other.m_domainName)
                    && this.m_problemName.Equals(other.m_problemName)
                    && this.m_content.Equals(other.m_content)
                    && this.m_actions.DictionaryEqual(other.m_actions)
                    && this.m_constants.DictionaryEqual(other.m_constants)
                    && this.m_constraints.Equals(other.m_constraints)
                    && this.m_goal.Equals(other.m_goal)
                    && this.m_init.SequenceEqual(other.m_init)
                    && ((this.m_metric == null && other.m_metric == null) || this.m_metric.Equals(other.m_metric))
                    && this.Formulas.DictionaryEqual(other.Formulas)
                    && this.m_requirements.SequenceEqual(other.m_requirements)
                      // TODO: Verify typing (domains, etc.)?
                    ;
      }
      return false;
    }

    /// <summary>
    /// Returns the hash code of this PDDL object.
    /// </summary>
    /// <returns>The hash code of this PDDL object.</returns>
    public override int GetHashCode()
    {
      return this.m_domainName.GetHashCode() + this.m_problemName.GetHashCode()
                  + this.m_content.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this PDDL object.
    /// </summary>
    /// <returns>A string representation of this PDDL object.</returns>
    public override string ToString()
    {
      string str = null;
      switch (this.m_content)
      {
        case PDDLContent.DOMAIN:
          str = ToDomainString();
          break;
        case PDDLContent.PARTIAL_PROBLEM:
          str = ToPartialProblemString();
          break;
        case PDDLContent.FULL_PROBLEM:
          str = ToFullProblemString();
          break;
      }
      return str;
    }

    #endregion
  }
}
