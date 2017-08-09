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
// Implementation: Daniel Castonguay / Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PDDLParser;
using PDDLParser.Exp.Formula;
using PDDLParser.Extensions;
using PDDLParser.Parser;
using TLPlan.Utils;
using TLPlan.Utils.Set;
using TLPlan.World;
using TLPlan.World.Implementations;


namespace TLPlan
{
  /// <summary>
  /// This class regroups all possible TLPlan options.
  /// </summary>
  /// <remarks>
  /// The DifferentBehavior attribute indicates that changing this option may have an effect
  /// on the planner's search mechanism. Hence the search graph may be explored differently and
  /// different plans may be found.
  /// On the other hand, the SameBehavior attribute indicates that changing this attribute should have no effect
  /// on the search mechanism, but it may slow or speed up the search, depending on the
  /// problem. The same plan should always be returned.
  /// </remarks>
  public class TLPlanOptions : ICloneable
  {
    #region public class DefinedIf

    /// <summary>
    /// Indicates that a property is defined only if a certain other property has a specific value.
    /// </summary>
    public class DefinedIf
    {
      /// <summary>
      /// The name of the parent property.
      /// </summary>
      private string m_propertyName;
      /// <summary>
      /// The value the parent property must take for the other property to be defined.
      /// </summary>
      private object m_propertyValue;

      /// <summary>
      /// Creates a new DefinedIf instance with the specified parent property name and value.
      /// </summary>
      /// <param name="propertyName">The parent property name.</param>
      /// <param name="propertyValue">The parent property value.</param>
      public DefinedIf(string propertyName, object propertyValue)
      {
        this.m_propertyName = propertyName;
        this.m_propertyValue = propertyValue;
      }

      /// <summary>
      /// Returns whether the other property is defined, given the options instance.
      /// </summary>
      /// <param name="instance">An options instance.</param>
      /// <returns>Whether the other property is defined.</returns>
      public bool isDefined(TLPlanOptions instance)
      {
        return typeof(TLPlanOptions).GetProperties().Where(prop => prop.Name.Equals(m_propertyName)).First().
            GetValue(instance, null).Equals(m_propertyValue);
      }
    }

    #endregion

    #region public class BehaviorAttribute

    /// <summary>
    /// Base class for behavior attributes.
    /// </summary>
    public abstract class BehaviorAttribute : Attribute
    {
      /// <summary>
      /// The conditions indicating when this property is defined.
      /// </summary>
      private DefinedIf[] m_conditions;

      /// <summary>
      /// Creates a new behavior attribute.
      /// </summary>
      public BehaviorAttribute()
      {
        this.m_conditions = new DefinedIf[0];
      }

      /// <summary>
      /// Creates a new behavior attribute. The property is defined only if a parent
      /// property takes a specific value.
      /// </summary>
      /// <param name="propertyName">The parent property name.</param>
      /// <param name="propertyValue">The parent property value it must take for this property
      /// to be defined.</param>
      public BehaviorAttribute(string propertyName, object propertyValue)
      {
        this.m_conditions = new DefinedIf[] { new DefinedIf(propertyName, propertyValue) };
      }

      /// <summary>
      /// Returns whether this property is defined, given an options instance.
      /// </summary>
      /// <param name="instance">An options instance.</param>
      /// <returns>Whether this property is defined.</returns>
      public bool IsDefined(TLPlanOptions instance)
      {
        foreach (DefinedIf condition in m_conditions)
        {
          if (!condition.isDefined(instance))
            return false;
        }
        return true;
      }
    }

    #endregion

    #region public class SameBehaviorAttribute

    /// <summary>
    /// This attribute indicates that the property should not modify the behavior of 
    /// the planner, i.e. the same plan should always be found.
    /// </summary>
    public abstract class SameBehaviorAttribute : BehaviorAttribute
    {
      /// <summary>
      /// Creates a new attribute indicating that the property does not modify the 
      /// planner behavior.
      /// </summary>
      public SameBehaviorAttribute()
        : base()
      {
      }

      /// <summary>
      /// Creates a new attribute indicating that the property does not modify the 
      /// planner behavior. Furthermore, the property is defined only if a parent property 
      /// takes a specific value.     
      /// </summary>
      /// <param name="propertyName">The parent property name.</param>
      /// <param name="propertyValue">The parent property value it must take for this property
      /// to be defined.</param>
      public SameBehaviorAttribute(string propertyName, object propertyValue)
        : base(propertyName, propertyValue)
      {
      }
    }

    #endregion

    #region public class SameBehaviorSameStatsAttribute

    /// <summary>
    /// This attribute indicates that the property should not modify the behavior of 
    /// the planner, i.e. the same plan should always be found.
    /// </summary>
    public class SameBehaviorSameStatsAttribute : SameBehaviorAttribute
    {
      /// <summary>
      /// Creates a new attribute indicating that the property does not modify neither
      /// the planner behavior nor the stats produced.
      /// </summary>
      public SameBehaviorSameStatsAttribute()
        : base()
      {
      }

      /// <summary>
      /// Creates a new attribute indicating that the property does not modify neither
      /// the planner behavior nor the stats produced. Furthermore, the property is 
      /// defined only if a parent property takes a specific value.     
      /// </summary>
      /// <param name="propertyName">The parent property name.</param>
      /// <param name="propertyValue">The parent property value it must take for this property
      /// to be defined.</param>
      public SameBehaviorSameStatsAttribute(string propertyName, object propertyValue)
        : base(propertyName, propertyValue)
      {
      }
    }

    #endregion

    #region public class SameBehaviorDifferentStatsAttribute

    /// <summary>
    /// This attribute indicates that the property should not modify the behavior of 
    /// the planner, i.e. the same plan should always be found.
    /// </summary>
    public class SameBehaviorDifferentStatsAttribute : SameBehaviorAttribute
    {
      /// <summary>
      /// Creates a new attribute indicating that the property does not modify 
      /// the planner behavior but does modify the stats produced.
      /// </summary>
      public SameBehaviorDifferentStatsAttribute()
        : base()
      {
      }

      /// <summary>
      /// Creates a new attribute indicating that the property does not modify 
      /// the planner behavior but does modify the stats produced. Furthermore, the 
      /// property is defined only if a parent property takes a specific value.     
      /// </summary>
      /// <param name="propertyName">The parent property name.</param>
      /// <param name="propertyValue">The parent property value it must take for this property
      /// to be defined.</param>
      public SameBehaviorDifferentStatsAttribute(string propertyName, object propertyValue)
        : base(propertyName, propertyValue)
      {
      }
    }

    #endregion

    #region public class DifferentBehaviorAttribute

    /// <summary>
    /// This attribute indicates that the property modifies the planner's search mechanism, 
    /// hence the search graph is explored differently and a different plan may be found.
    /// </summary>
    public class DifferentBehaviorAttribute : BehaviorAttribute
    {
      /// <summary>
      /// Creates a new attribute indicating that the property modifies the behavior of the
      /// planner.
      /// </summary>
      public DifferentBehaviorAttribute()
        : base()
      {
      }

      /// <summary>
      /// Creates a new attribute indicating that the property modifies the behavior of the
      /// planner. Furthermore, the property is defined only if a parent property 
      /// takes a specific value. 
      /// </summary>
      /// <param name="propertyName">The parent property name.</param>
      /// <param name="propertyValue">The parent property value it must take for this property
      /// to be defined.</param>
      public DifferentBehaviorAttribute(string propertyName, object propertyValue)
        : base(propertyName, propertyValue)
      {
      }
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// All possible graph search strategies.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum GraphSearchStrategy
    {
      /// <summary>
      /// Explores the most promising worlds first.
      /// </summary>
      BEST_FIRST,
      /// <summary>
      /// Explores all neighborhing nodes first.
      /// </summary>
      BREADTH_FIRST,
      /// <summary>
      /// Explores all neighborhing nodes first, try the actions with the highest priority first.
      /// </summary>
      BREADTH_FIRST_PRIORITY,
      /// <summary>
      /// Explores as far as possible along each branch before backtracking.
      /// </summary>
      DEPTH_FIRST,
      /// <summary>
      /// Explores as far as possible along each branch before backtracking, 
      /// try the actions with the highest priority first.
      /// </summary>
      DEPTH_FIRST_PRIORITY,
      /// <summary>
      /// Explores as far as possible along a single branch. The search fails if a node
      /// has no valid successors.
      /// </summary>
      DEPTH_FIRST_NO_BACKTRACKING,
      /// <summary>
      /// Explores as far as possible along each branch before backtracking,
      /// try the most promising worlds first.
      /// </summary>
      DEPTH_BEST_FIRST
    };

    /// <summary>
    /// All possible world types.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum WorldType
    {
      /// <summary>
      /// A qualified world uses a hashset of qualified predicates and fluents.
      /// </summary>
      QUALIFIED,
      /// <summary>
      /// A custom world may be initialized with any combination of facts/fluents container.
      /// </summary>
      CUSTOM
    };

    /// <summary>
    /// All possible facts container types. Note that the facts container type is 
    /// defined only if the world type is Custom.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum FactsContainerType
    {
      /// <summary>
      /// A bitset facts container stores predicates in a fixed bitset.
      /// </summary>
      BITSET,
      /// <summary>
      /// A hashset facts container stores predicates in a hashset.
      /// </summary>
      HASHSET,
      /// <summary>
      /// A treeset facts container stores predicates in a treeset.
      /// </summary>
      TREESET
    };

    /// <summary>
    /// All possible fluents container types. Note that the fluents container type is
    /// defined only if the world type is Custom.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum FluentsContainerType
    {
      /// <summary>
      /// An array fluents container stores fluents in a fixed array.
      /// </summary>
      ARRAY,
      /// <summary>
      /// A hashmap fluents container stores fluents in a hashmap.
      /// </summary>
      HASHMAP,
      /// <summary>
      /// A treemap fluents container stores fluents in a treemap.
      /// </summary>
      TREEMAP
    };

    /// <summary>
    /// All possible sets implementation type. This is used to generate the appropriate
    /// implementation for the open and closed set needed in certain search strategies.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum SetImplementationType
    {
      /// <summary>
      /// Generates hashsets, whose elements are not sorted.
      /// </summary>
      HASHSET,
      /// <summary>
      /// Generates treesets, whose elements are sorted.
      /// </summary>
      TREESET
    };

    /// <summary>
    /// All possible levels of preprocessing. The preprocessing phase takes place before the
    /// actual graph search process and uses the invariants to simplify actions preconditions.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum PreprocessingLevel
    {
      /// <summary>
      /// Do not preprocess preconditions.
      /// </summary>
      NONE = 0,
      /// <summary>
      /// Preprocess all preconditions except the defined formulas.
      /// </summary>
      ALL_EXCEPT_DEFINED_FORMULAS = 1,
      /// <summary>
      /// Preprocess all preconditions.
      /// </summary>
      ALL = 2
    };

    #endregion

    #region Properties

    /// <summary>
    /// The domain file to use.
    /// </summary>
    public string Domain { get; private set; }
    /// <summary>
    /// The problem file to use.
    /// </summary>
    public string Problem { get; private set; }

    /// <summary>
    /// The graph search strategy to use.
    /// </summary>
    private GraphSearchStrategy m_graphSearchStrategy;

    /// <summary>
    /// The graph search strategy to use.
    /// </summary>
    [DifferentBehavior]
    public GraphSearchStrategy SearchStrategy 
    { 
      get 
      { 
        return m_graphSearchStrategy;
      }
      set
      { 
        m_graphSearchStrategy = value;
        switch (m_graphSearchStrategy)
        {
          case GraphSearchStrategy.DEPTH_FIRST_NO_BACKTRACKING:
            this.UseBacktracking = false;
            break;
        }
      }
    }

    /// <summary>
    /// The world implementation to use.
    /// </summary>
    [SameBehaviorSameStats]
    public WorldType WorldImp { get; set; }

    /// <summary>
    /// The facts container implementation to use. Note that this is only defined if the world
    /// implementation is custom.
    /// </summary>
    [SameBehaviorSameStats("WorldImp", WorldType.CUSTOM)]
    public FactsContainerType FactsContainerImp { get; set; }

    /// <summary>
    /// The fluents container implementation to use. Note that this is only defined if the world
    /// implementation is custom.
    /// </summary>
    [SameBehaviorSameStats("WorldImp", WorldType.CUSTOM)]
    public FluentsContainerType FluentsContainerImp { get; set; }

    /// <summary>
    /// The sets implementation. This is used to generate the appropriate
    /// implementation for the open and closed set needed in certain search strategies.
    /// </summary>
    [SameBehaviorSameStats]
    public SetImplementationType SetStructureImp { get; set; }

    /// <summary>
    /// Whether invariants should be computed and stored separately from the each 
    /// world generated during the search process.
    /// </summary>
    [SameBehaviorSameStats]
    public bool ComputeInvariants { get; set; }

    /// <summary>
    /// The level of preprocessing to complete before starting the actual graph search process.
    /// The preprocessing phase is used to simplify actions preconditions.
    /// </summary>
    [SameBehaviorSameStats]
    public PreprocessingLevel PreprocessLevel { get; set; }

    /// <summary>
    /// If true, immediately prune successors using the temporal control formula.
    /// Else, discard successors only when they are the next ones to be explored.
    /// </summary>
    [SameBehaviorDifferentStats]
    public bool ImmediatelyPruneSuccessors { get; set; }

    /// <summary>
    /// Whether to check for cycles when exploring the graph.
    /// If set to false, the search may never return since it may explore cycles 
    /// ad vitam eternam.
    /// </summary>
    [DifferentBehavior]
    public bool CycleChecking { get; set; }

    /// <summary>
    /// Whether to use backtracking when exploring the graph.
    /// If set to false, all successors of a world, except the first one, are discarded.
    /// </summary>
    [DifferentBehavior]
    public bool UseBacktracking { get; set; }

    /// <summary>
    /// Whether to support concurrency in planning.
    /// If set to false, only sequential plans are generated.
    /// </summary>
    [DifferentBehavior]
    public bool ConcurrentActions { get; set; }

    /// <summary>
    /// Whether to allow the use of undefined fluents.
    /// If set to false, the program throws an exception whenever an undefined fluent is
    /// evaluated.
    /// </summary>
    public bool AllowUndefinedFluents { get; set; }
    
    /// <summary>
    /// Whether to sort actions by priority.
    /// Two search strategies, i.e. breadth-first-priority and depth-first-priority sort
    /// actions by priority.
    /// </summary>
    public bool OrderActionsByPriority
    {
      get
      {
        return m_graphSearchStrategy == GraphSearchStrategy.BREADTH_FIRST_PRIORITY ||
               m_graphSearchStrategy == GraphSearchStrategy.DEPTH_FIRST_PRIORITY;
      }
    }

    /// <summary>
    /// Whether to hide the wait-for-next-event in plans.
    /// </summary>
    public bool ElideWaitEvent { get; set; }

    /// <summary>
    /// The trace level represents the amount of trace information output by the
    /// search algorithm.
    /// 0 - No tracing.
    /// 1 - Print the world being expanded and the successors being generated.
    /// 2 - Print the level 1 information and the progressed constraints.
    /// </summary>
    public int TraceLevel { get; set; }

    /// <summary>
    /// The maximum number of worlds to visit during the search.
    /// Set this value to 0 for no limit.
    /// </summary>
    public int SearchLimit { get; set; }

    /// <summary>
    /// Whether there is a search limit.
    /// </summary>
    public bool HasSearchLimit { get { return SearchLimit != 0; } }

    /// <summary>
    /// The maximum time limit during the search.
    /// Set this value to 0 for no limit.
    /// </summary>
    public TimeSpan TimeLimit { get; set; }

    /// <summary>
    /// Whether there is a time limit.
    /// </summary>
    public bool HasTimeLimit { get { return TimeLimit.Ticks != 0; } }

    /// <summary>
    /// Whether to validate the plan (using the validator) when one is found.
    /// </summary>
    public bool ValidatePlan { get; set; }

    /// <summary>
    /// The PDDL domain used for validation. This domain must be PDDL3.0-compliant
    /// (no TLPlan special operators, no object fluents, ...).
    /// </summary>
    public string ValidationDomain { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the set of options to default values.
    /// </summary>
    public TLPlanOptions()
    {
      Reset();
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Retrieves all properties bearing the [OptionType] attribute.
    /// </summary>
    /// <typeparam name="OptionType">The type of attributes, which must be BehaviorAttribute or 
    /// some subclass.</typeparam>
    /// <param name="baseOptions">The base set of options to use.</param>
    /// <param name="allSubstitutions">Return all possible substitutions or change all the
    /// options against the base set.</param>
    /// <returns>All properties bearing the [OptionType] attribute.</returns>
    public static IEnumerable<TLPlanOptions> GetPossibleTLPlanOptions<OptionType>(
      TLPlanOptions baseOptions,
      bool allSubstitutions)

      where OptionType : BehaviorAttribute
    {
      // Retrieve all properties bearing the "OptionType" attribute
      List<PropertyInfo> properties = new List<PropertyInfo>(typeof(TLPlanOptions).GetProperties()
        .Where(p => !p.GetCustomAttributes(true).OfType<OptionType>().IsEmpty()));

      return GetAllOptions<OptionType>(properties, 0, baseOptions, allSubstitutions);
    }

    /// <summary>
    /// Enumerates all possible options.
    /// </summary>
    /// <typeparam name="OptionType">The type of attributes, which must be BehaviorAttribute or 
    /// some subclass.</typeparam>
    /// <param name="properties">The list of option's properties.</param>
    /// <param name="currentIndex">The current property.</param>
    /// <param name="options">The options instance to return.</param>
    /// <param name="allSubstitutions">Return all possible substitutions or change all the
    /// options against the base set.</param>
    /// <returns>An enumerable over all options substitutions.</returns>
    public static IEnumerable<TLPlanOptions> GetAllOptions<OptionType>(
      List<PropertyInfo> properties, 
      int currentIndex, 
      TLPlanOptions options,
      bool allSubstitutions)

      where OptionType : BehaviorAttribute
    {
      if (currentIndex == 0)
        yield return options;

      PropertyInfo prop = properties[currentIndex];
      System.Type type = prop.PropertyType;

      // Make sure the option is defined.
      bool isDefined = true;
      IEnumerable<OptionType> optionAttributes = prop.GetCustomAttributes(true).OfType<OptionType>();
      foreach (OptionType attribute in optionAttributes)
      {
        if (!attribute.IsDefined(options))
        {
          isDefined = false;
          break;
        }
      }
      
      // Retrieves all possible values this property may be assigned to.
      IEnumerable<object> values;
      if (isDefined || !allSubstitutions)
      {
        if (type.Equals(typeof(bool)))
        {
          values = new object[] { true, false };
        }
        else if (typeof(Enum).IsAssignableFrom(type))
        {
          values = Enum.GetValues(type).Cast<object>();
        }
        else
        {
          throw new NotSupportedException("Type " + type.ToString() + " is not supported.");
        }
      }
      else
      {
        values = new object[] { null };
      }

      object oldValue = prop.GetValue(options, null);

      // Iterates over all possible values.
      foreach (object value in values)
      {
        if (allSubstitutions)
        {
          if (value != null)
            prop.SetValue(options, value, null);

          // Return all substitutions (of all options)
          if (currentIndex == (properties.Count - 1))
          {
            yield return (TLPlanOptions)options.Clone();
          }
          else
          {
            foreach (TLPlanOptions option in GetAllOptions<OptionType>(properties, currentIndex + 1, options, allSubstitutions))
            {
              yield return options;
            }
          }
        }
        else
        {
          if (value != null && !value.Equals(oldValue))
          {
            prop.SetValue(options, value, null);

            // Return this option's possible substitutions
            yield return (TLPlanOptions)options.Clone();
          }
        }
      }

      if (!allSubstitutions)
      {
        prop.SetValue(options, oldValue, null);
        if (currentIndex != (properties.Count - 1))
        {
          foreach (TLPlanOptions option in GetAllOptions<OptionType>(properties, currentIndex + 1, options, allSubstitutions))
          {
            yield return options;
          }
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the domain file.
    /// </summary>
    /// <param name="domain">The domain file.</param>
    public void SetDomain(string domain)
    {
      if (!File.Exists(domain))
        throw new ArgumentException("Domain file \"" + domain + "\" does not exist");

      Domain = domain;
    }

    /// <summary>
    /// Sets the problem file.
    /// </summary>
    /// <param name="problem">The problem file.</param>
    public void SetProblem(string problem)
    {
      if (!File.Exists(problem))
        throw new ArgumentException("Problem file \"" + problem + "\" does not exist");

      Problem = problem;
    }

    /// <summary>
    /// Sets the validation domain file.
    /// </summary>
    /// <param name="domain">The validation domain file.</param>
    public void SetValidationDomain(string domain)
    {
      if (!string.IsNullOrEmpty(domain))
      {
        if (!File.Exists(domain))
          throw new ArgumentException("Validation domain file \"" + domain + "\" does not exist");

        ValidationDomain = domain;
      }
      else
      {
        ValidationDomain = Domain;
      }
    }

    /// <summary>
    /// Sets the search strategy, given its string representation.
    /// </summary>
    /// <param name="strategy">The string representation of the search strategy.</param>
    public void SetSearchStrategy(string strategy)
    {
      try
      {
        SearchStrategy = (GraphSearchStrategy)Enum.Parse(typeof(GraphSearchStrategy), strategy, true);
      }
      catch (System.Exception)
      {
        throw new ArgumentException("Unrecognized graph search strategy: " + strategy);
      }
    }

    /// <summary>
    /// Sets the structure implementation, given its string representation.
    /// </summary>
    /// <param name="imp">The string representation of the structure implementation.</param>
    public void SetStructureImplementation(string imp)
    {
      try
      {
        SetStructureImp = (SetImplementationType)Enum.Parse(typeof(SetImplementationType), imp, true);
      }
      catch (System.Exception)
      {
        throw new ArgumentException("Unrecognized structure implementation: " + imp);
      }
    }

    /// <summary>
    /// Sets the world implementation, given its string representation.
    /// </summary>
    /// <param name="imp">The string representation of the world implementation.</param>
    public void SetWorldImplementation(string imp)
    {
      try
      {
        WorldImp = (WorldType)Enum.Parse(typeof(WorldType), imp, true);
      }
      catch (System.Exception)
      {
        throw new ArgumentException("Unrecognized world implementation: " + imp);
      }
    }

    /// <summary>
    /// Sets the facts container implementation, given its string representation.
    /// </summary>
    /// <param name="imp">The string representation of the facts container implementation.</param>
    public void SetFactsContainerImplementation(string imp)
    {
      try
      {
        FactsContainerImp = (FactsContainerType)Enum.Parse(typeof(FactsContainerType), imp, true);
        WorldImp = WorldType.CUSTOM;
      }
      catch (System.Exception)
      {
        throw new ArgumentException("Unrecognized facts container implementation: " + imp);
      }
    }

    /// <summary>
    /// Sets the fluents container implementation, given its string representation.
    /// </summary>
    /// <param name="imp">The string representation of the fluents container implementation.</param>
    public void SetFluentsContainerImplementation(string imp)
    {
      try
      {
        FluentsContainerImp = (FluentsContainerType)Enum.Parse(typeof(FluentsContainerType), imp, true);
        WorldImp = WorldType.CUSTOM;
      }
      catch (System.Exception)
      {
        throw new ArgumentException("Unrecognized fluents container implementation: " + imp);
      }
    }

    /// <summary>
    /// Sets the preprocessing level, given its string representation.
    /// </summary>
    /// <param name="imp">The string representation of the processing level.</param>
    public void SetPreprocessingLevel(string imp)
    {
      try
      {
        PreprocessLevel = (PreprocessingLevel)Enum.Parse(typeof(PreprocessingLevel), imp, true);
      }
      catch (System.Exception)
      {
        throw new ArgumentException("Unrecognized preprocessing level: " + imp);
      }
    }

    /// <summary>
    /// Validates the options.
    /// </summary>
    /// <returns>True if the options are valid.</returns>
    public bool ValidateOptions()
    {
      if (ValidatePlan && string.IsNullOrEmpty(ValidationDomain))
        ValidationDomain = Domain;

      return (Domain != null && Problem != null);
    }

    /// <summary>
    /// Resets the options to their default values.
    /// </summary>
    public void Reset()
    {
      Domain = null;
      Problem = null;
      ValidationDomain = null;

      ComputeInvariants = true;
      PreprocessLevel = PreprocessingLevel.ALL;
      ImmediatelyPruneSuccessors = false;
      ConcurrentActions = true;
      ElideWaitEvent = true;
      AllowUndefinedFluents = true;
      CycleChecking = true;
      UseBacktracking = true;
      ValidatePlan = false;

      SearchStrategy = GraphSearchStrategy.BEST_FIRST;
      WorldImp = WorldType.QUALIFIED;
      SetStructureImp = SetImplementationType.HASHSET;
      FactsContainerImp = FactsContainerType.HASHSET;
      FluentsContainerImp = FluentsContainerType.HASHMAP;

      SearchLimit = 0;
      TimeLimit = new TimeSpan(0);
    }

    /// <summary>
    /// Creates the appropriate set given the set implementation.
    /// </summary>
    /// <typeparam name="Value">The type of the elements in the set.</typeparam>
    /// <returns>A set of the given implementation.</returns>
    public TLPlan.Utils.Set.ISet<Value> CreateSet<Value>()
    {
      switch (SetStructureImp)
      {
        case SetImplementationType.TREESET:
              return new TLPlan.Utils.Set.SortedSet<Value>();

        case SetImplementationType.HASHSET:
        default:
          return new Set<Value>();
      }
    }

    /// <summary>
    /// Creates the appropriate dictionary given the dictionary implementation.
    /// </summary>
    /// <typeparam name="Key">The type of the keys in the set.</typeparam>
    /// <typeparam name="Value">The type of the values in the set.</typeparam>
    /// <returns>A dictionary of the given implementation.</returns>
    public IDictionary<Key, Value> CreateDictionary<Key, Value>()
    {
      switch (SetStructureImp)
      {
        case SetImplementationType.TREESET:
          return new SortedDictionary<Key, Value>();

        case SetImplementationType.HASHSET:
        default:
          return new Dictionary<Key, Value>();
      }
    }

    /// <summary>
    /// Creates a new open world used to store the values of the predicates and fluents of the
    /// given PDDL problem. The world is created according to this set of options.
    /// </summary>
    /// <param name="problem">The PDDL problem to solve.</param>
    /// <returns>A new open world.</returns>
    public ExtendedOpenWorld CreateWorld(PDDLObject problem)
    {
      IEnumerable<DescribedFormula> variantFormulas =
        problem.Formulas.Values.OfType<DescribedFormula>().Where(f => !f.Invariant);

      IEnumerable<DescribedFormula> cycleCheckFormulas =
        variantFormulas.Where(f => f.DetectCycles);

      ExtendedOpenWorld cycleCheckWorld = CreateWorld(cycleCheckFormulas);

      IEnumerable<DescribedFormula> noCycleCheckFormulas =
        variantFormulas.Where(f => !f.DetectCycles);

      if (noCycleCheckFormulas.IsEmpty())
      {
        // Optimization: do not create a partial-cycle-checking world if all formulas
        // are marked as cycle-check
        return cycleCheckWorld;
      }
      else
      {
        ExtendedOpenWorld noCycleCheckWorld = CreateWorld(noCycleCheckFormulas);

        return new PartialCycleCheckWorld(cycleCheckWorld, noCycleCheckWorld, this);
      }
    }

    /// <summary>
    /// Creates a new open world responsible for storing the values of the specified 
    /// described formulas (and no other formulas).
    /// </summary>
    /// <param name="formulas">The described formulas whose value must be stored.</param>
    /// <returns>A new open world.</returns>
    private ExtendedOpenWorld CreateWorld(IEnumerable<DescribedFormula> formulas)
    {
      switch (WorldImp)
      {
        case WorldType.CUSTOM:
          FactsContainer factsContainer = CreateFactsContainer(formulas.OfType<AtomicFormula>());
          FluentsContainer fluentsContainer = CreateFluentsContainer(formulas.OfType<Fluent>());
          return new CustomWorld(factsContainer, fluentsContainer, this);

        case TLPlanOptions.WorldType.QUALIFIED:
        default:
          return new QualifiedWorld(this);
      }
    }

    /// <summary>
    /// Creates a new invariant world used to store the values of all the invariant predicates 
    /// and fluents of the given PDDL problem.
    /// </summary>
    /// <param name="problem">The PDDL problem to solve.</param>
    /// <returns>The invariant world for the given PDDL problem.</returns>
    public InvariantWorld CreateInvariantWorld(PDDLObject problem)
    {
      IEnumerable<DescribedFormula> invariantFormulas =
        problem.Formulas.Values.OfType<DescribedFormula>().Where(f => f.Invariant);

      IntegerInterval predicateInterval = GetDefinitionInterval(invariantFormulas.OfType<AtomicFormula>());

      IntegerInterval numericInterval = GetDefinitionInterval(invariantFormulas.OfType<NumericFluent>());

      IntegerInterval objectInterval = GetDefinitionInterval(invariantFormulas.OfType<ObjectFluent>());

      return new InvariantWorld(new BitSetFactsContainer(predicateInterval), 
                                new ArrayFluentsContainer(numericInterval, objectInterval),
                                this);
    }

    /// <summary>
    /// Creates an empty facts container according to this set of options.
    /// </summary>
    /// <param name="predicates">All the predicates whose values must be stored.</param>
    /// <returns>An empty facts container.</returns>
    public FactsContainer CreateFactsContainer(IEnumerable<AtomicFormula> predicates)
    {
      IntegerInterval interval = GetDefinitionInterval(predicates);
      switch (FactsContainerImp)
      {
        case FactsContainerType.BITSET:
          return new BitSetFactsContainer(interval);
        case FactsContainerType.TREESET:
          return new TreeSetFactsContainer(interval);
        case FactsContainerType.HASHSET:
        default:
          return new HashSetFactsContainer(interval);
      }
    }

    /// <summary>
    /// Creates an empty fluents container according to this set of options.
    /// </summary>
    /// <param name="fluents">All the fluents whose value must be stored.</param>
    /// <returns>An empty fluents container.</returns>
    public FluentsContainer CreateFluentsContainer(IEnumerable<Fluent> fluents)
    {
      IntegerInterval numericInterval = GetDefinitionInterval(fluents.OfType<NumericFluent>());
      IntegerInterval objectInterval = GetDefinitionInterval(fluents.OfType<ObjectFluent>());
      switch (FluentsContainerImp)
      {
        case FluentsContainerType.ARRAY:
          return new ArrayFluentsContainer(numericInterval, objectInterval);
        case FluentsContainerType.TREEMAP:
          return new TreeMapFluentsContainer(numericInterval, objectInterval);
        case FluentsContainerType.HASHMAP:
        default:
          return new HashMapFluentsContainer(numericInterval, objectInterval);
      }
    }

    /// <summary>
    /// Returns the sum of all filtered described formulas' domain cardinality.
    /// </summary>
    /// <typeparam name="T">The type of formulas.</typeparam>
    /// <param name="formulas">An enumeartion of the formulas whose domain cardinality is to sum.</param>
    /// <returns>The sum of all filtered described formulas' domain cardinality.</returns>
    private IntegerInterval GetDefinitionInterval<T>(IEnumerable<T> formulas) 
      where T : DescribedFormula
    {
      List<T> list = formulas.ToList();
      list.Sort((f1, f2) => f1.Offset.CompareTo(f2.Offset));

      int offset = -1;
      int total = 0;
      foreach (T formula in list)
      {
        if (offset == -1)
          offset = formula.Offset;
        total += formula.GetDomainCardinality();
      }

      if (offset == -1)
        offset = 0;

      return new IntegerInterval(offset, total);
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns a string representation of the options.
    /// </summary>
    /// <returns>A string representation of the options.</returns>
    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      builder.AppendFormat("Search strategy: {0}.\n", SearchStrategy);
      builder.AppendFormat("World implementation: {0}. ", WorldImp);
      if (WorldImp == WorldType.CUSTOM)
      {
        builder.AppendFormat("(Facts container: {0}, Fluents container: {1})", 
                              FactsContainerImp, FluentsContainerImp);
      }
      else
      {
        builder.AppendFormat("\n");
      }
      builder.AppendFormat("Structure implementation: {0}.\n", SetStructureImp);
      builder.AppendFormat("Options: Enable concurrency           = {0}\n", ConcurrentActions);
      builder.AppendFormat("         Immediately prune successors = {0}\n", ImmediatelyPruneSuccessors);
      builder.AppendFormat("         Cycle checking               = {0}\n", CycleChecking);
      builder.AppendFormat("         Backtracking                 = {0}\n", UseBacktracking);
      builder.AppendFormat("         Compute invariants           = {0}\n", ComputeInvariants);
      builder.AppendFormat("         Preprocess level             = {0}\n", PreprocessLevel);
      builder.AppendFormat("         Elide Wait event             = {0}\n", ElideWaitEvent);
      builder.AppendFormat("         Allow undefined fluents      = {0}\n", AllowUndefinedFluents);
      builder.AppendFormat("         Search limit                 = {0}\n", (HasSearchLimit ? SearchLimit.ToString() : "None"));
      builder.AppendFormat("         Time limit                   = {0}\n", (HasTimeLimit ? TimeLimit.ToString() : "None"));
      builder.AppendFormat("         Validate plan                = {0}\n", ValidatePlan);
      if (ValidatePlan)
        builder.AppendFormat("         Validation Domain            = {0}\n", ValidationDomain);

      return builder.ToString();
    }

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Returns a clone of these options.
    /// </summary>
    /// <returns>A clone of these options.</returns>
    public object Clone()
    {
      return this.MemberwiseClone();
    }

    #endregion
  }
}

