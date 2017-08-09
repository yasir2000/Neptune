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
using PDDLParser;
using PDDLParser.Action;
using PDDLParser.Exp;
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Metric;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Extensions;
using PDDLParser.Parser;
using TLPlan.Algorithms;
using TLPlan.Utils;
using TLPlan.World;
using TLPlan.World.Implementations;

using Action = PDDLParser.Action.Action;

namespace TLPlan
{
  /// <summary>
  /// TLPlan planner class.
  /// </summary>
  public class Planner
  {
    #region Private Fields

    /// <summary>
    /// The current PDDL problem.
    /// </summary>
    private PDDLObject m_currentProblem;
    /// <summary>
    /// The current TLPlan options.
    /// </summary>
    private TLPlanOptions m_options;
    /// <summary>
    /// The current statistics.
    /// </summary>
    private Statistics m_statistics;
    /// <summary>
    /// The error stream connected to the UI.
    /// </summary>
    private TextWriter m_errorStream;
    /// <summary>
    /// The stream to which traces are to be written.
    /// </summary>
    private TraceWriter m_traceWriter;
    /// <summary>
    /// The internal graph search implementation.
    /// </summary>
    private GraphSearch m_graphSearch;

    #endregion

    #region Properties

    /// <summary>
    /// The Started event is fired when the search starts.
    /// It is forwarded to the graph search implementation.
    /// </summary>
    public event EventHandler Started
    {
      add { m_graphSearch.Started += value; }
      remove { m_graphSearch.Started -= value; }
    }

    /// <summary>
    /// The Paused event is fired when the search is paused.
    /// It is forwarded to the graph search implementation.
    /// </summary>
    public event EventHandler Paused
    {
      add { m_graphSearch.Paused += value; }
      remove { m_graphSearch.Paused -= value; }
    }

    /// <summary>
    /// The Unpaused event is fired when the search is resumed.
    /// It is forwarded to the graph search implementation.
    /// </summary>
    public event EventHandler Unpaused
    {
      add { m_graphSearch.Unpaused += value; }
      remove { m_graphSearch.Unpaused -= value; }
    }

    /// <summary>
    /// The Stopped event is fired when the search is stopped.
    /// It is forwarded to the graph search implementation.
    /// </summary>
    public event EventHandler Stopped
    {
      add { m_graphSearch.Stopped += value; }
      remove { m_graphSearch.Stopped -= value; }
    }

    /// <summary>
    /// The PlanningFinished event is fired when the planning is finished.
    /// It is forwarded to the graph search implementation.
    /// </summary>
    public event EventHandler<Planner.PlanningFinishedEventArgs> PlanningFinished
    {
      add { m_graphSearch.PlanningFinished += value; }
      remove { m_graphSearch.PlanningFinished -= value; }
    }

    /// <summary>
    /// The current node which is being explored by the graph search algorithm.
    /// </summary>
    public Node CurrentNode
    {
      get
      {
        if (!IsPaused)
          throw new InvalidOperationException("Cannot fetch the current node while the planner is running.");

        return m_graphSearch.CurrentNode;
      }
    }

    /// <summary>
    /// Whether the search is currently paused.
    /// </summary>
    public bool IsPaused
    { 
      get { return m_graphSearch.IsPaused; }
    }

    /// <summary>
    /// Whether the search should be paused when the goal world is found.
    /// </summary>
    public bool PauseOnGoalWorld
    {
      get { return m_graphSearch.PauseOnGoalWorld; }
      set { m_graphSearch.PauseOnGoalWorld = value; }
    }

    /// <summary>
    /// The reason why the planning failed.
    /// </summary>
    public Statistics.FailureCause FailureReason
    { 
      get { return m_statistics.FailureReason; }
    }

    /// <summary>
    /// The error stream connected to the UI.
    /// </summary>
    public TextWriter ErrorStream
    {
      get { return m_errorStream; }
      set { m_errorStream = value; }
    }

    /// <summary>
    /// The current TLPlan options.
    /// </summary>
    public TLPlanOptions Options { get { return m_options; } }

    /// <summary>
    /// The current statistics.
    /// </summary>
    public Statistics Statistics { get { return m_statistics; } }

    #endregion

    #region public class PlanningFinishedEventArgs

    /// <summary>
    /// Arguments of the PlanningFinished event.
    /// </summary>
    public class PlanningFinishedEventArgs : EventArgs
    {
      /// <summary>
      /// The event indicates the plan found.
      /// It is null if the problem has not been solved.
      /// </summary>
      public Plan Plan { get; private set; }
      /// <summary>
      /// The event indicates whether the problem was solved.
      /// </summary>
      public bool ProblemSolved { get; private set; }
      
      /// <summary>
      /// Creates a new instance of PlanningFinishedEventArgs with the specified plan.
      /// </summary>
      /// <param name="plan">The plan found.</param>
      public PlanningFinishedEventArgs(Plan plan)
      {
        this.ProblemSolved = true;
        this.Plan = plan;
      }

      /// <summary>
      /// Creates a new instance of PlanningFinishedEventArgs without any plan.
      /// </summary>
      public PlanningFinishedEventArgs()
      {
        this.ProblemSolved = false;
        this.Plan = null;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new TLPlan planner with the default options.
    /// </summary>
    public Planner()
      : this(new TLPlanOptions(), new Statistics(), Console.Out, Console.Out)
    { }

    /// <summary>
    /// Creates a new TLPlan planner with the specified options.
    /// </summary>
    /// <param name="options">The TLPlan options.</param>
    public Planner(TLPlanOptions options)
      : this(options, new Statistics(), Console.Out, Console.Out)
    { }

    /// <summary>
    /// Creates a new TLPlan planner with the specified options and statistics.
    /// </summary>
    /// <param name="options">The TLPlan options.</param>
    /// <param name="statistics">The statistics computed up to this point 
    /// (mostly concerning parsing).</param>
    public Planner(TLPlanOptions options, Statistics statistics)
      : this(options, statistics, Console.Out, Console.Out)
    { }

    /// <summary>
    /// Creates a new TLPlan planner with the specified options, statistics, and
    /// error stream.
    /// </summary>
    /// <param name="options">The TLPlan options.</param>
    /// <param name="statistics">The statistics computed up to this point 
    /// (mostly concerning parsing)</param>
    /// <param name="errorStream">The error stream connected to the UI.</param>
    /// <param name="traceStream">The stream to which traces are to be written.</param>
    public Planner(TLPlanOptions options, Statistics statistics, TextWriter errorStream, TextWriter traceStream)
    {
      m_statistics = statistics;
      m_statistics.Options = options;
      m_options = options;

      m_errorStream = errorStream;
      m_traceWriter = new TraceWriter(traceStream);

      m_graphSearch = GetGraphSearchImplementation(options.SearchStrategy);
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Returns the appropriate graph search implementation given the GraphSearchStrategy.
    /// </summary>
    /// <param name="strategy">The GraphSearchStrategy to use.</param>
    /// <returns>The appropriate graph search implementation.</returns>
    private GraphSearch GetGraphSearchImplementation(TLPlanOptions.GraphSearchStrategy strategy)
    {
      switch (strategy)
      {
        case TLPlanOptions.GraphSearchStrategy.BEST_FIRST:
          return new BestFirstSearch(m_options, m_statistics, m_traceWriter);
        case TLPlanOptions.GraphSearchStrategy.BREADTH_FIRST:
        case TLPlanOptions.GraphSearchStrategy.BREADTH_FIRST_PRIORITY:
          return new BreadthFirstSearch(m_options, m_statistics, m_traceWriter);
        case TLPlanOptions.GraphSearchStrategy.DEPTH_FIRST:
        case TLPlanOptions.GraphSearchStrategy.DEPTH_FIRST_PRIORITY:
          return new DepthFirstSearch(m_options, m_statistics, m_traceWriter);
        case TLPlanOptions.GraphSearchStrategy.DEPTH_FIRST_NO_BACKTRACKING:
          return new DepthFirstSearchNoBacktracking(m_options, m_statistics, m_traceWriter);
        case TLPlanOptions.GraphSearchStrategy.DEPTH_BEST_FIRST:
          return new DepthBestFirstSearch(m_options, m_statistics, m_traceWriter);
        default:
          throw new NotSupportedException("Search strategy\"" + strategy + "\" is not supported.");
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Pauses the search.
    /// </summary>
    /// <returns>True if search was not already paused.</returns>
    public bool Pause()
    {
      return m_graphSearch.Pause();
    }

    /// <summary>
    /// Resumes the search.
    /// </summary>
    /// <returns>True if the search was paused.</returns>
    public bool Unpause()
    {
      return m_graphSearch.Unpause();
    }

    /// <summary>
    /// Stops the search.
    /// </summary>
    public void Stop()
    {
      m_graphSearch.Stop();
    }

    /// <summary>
    /// Resets the planning.
    /// </summary>
    public void Reset()
    {
      m_statistics.Reset();
      m_graphSearch.Reset();
    }

    /// <summary>
    /// Sets the PDDL problem that must be solved by the planner.
    /// </summary>
    /// <param name="problem">The problem to solve.</param>
    public void SetPDDLProblem(PDDLObject problem)
    {
      PreprocessProblem(problem);
      this.m_currentProblem = problem;
      this.m_traceWriter.FullProblem = problem;
    }

    /// <summary>
    /// Attempts to solve the given PDDL problem.
    /// </summary>
    /// <param name="problem">The problem to solve.</param>
    /// <returns>A plan that solves the problem, or null if no plan was found.</returns>
    public Plan Solve(PDDLObject problem)
    {
      SetPDDLProblem(problem);
      return Solve();
    }

    /// <summary>
    /// Attempts to solve the current PDDL problem.
    /// </summary>
    /// <returns>A plan that solves the problem, or null if no plan was found.</returns>
    public Plan Solve()
    {
      if (m_currentProblem == null)
      {
        throw new System.Exception("Error in Planner.Solve(): A PDDL problem must be specified!");
      }
      else if (!m_options.ConcurrentActions &&
          !m_currentProblem.InitialWorld.OfType<TimedLiteral>().IsEmpty())
      {
        // TODO: this should not be a problem (because wait-for-next-event is available)

        m_statistics.FailureReason = Statistics.FailureCause.IncompatibleOptions;
        m_errorStream.WriteLine("Problems containing timed initial literals must have the ConcurrentAction option set to true.");
        return null;
      }

      try
      {
        m_statistics.StartSolve();

        TLPlanDurativeClosedWorld initialWorld = GetInitialWorld();
        InvariantWorld invariants = initialWorld.Invariants;
        
        // Instantiate operators
        List<IOperator> operators = InstantiateOperators(m_currentProblem.Actions.Values, 
                                                         invariants, 
                                                         m_currentProblem.ContainsDurativeActions);

        // Simplify the goal formulation if possible
        ILogicalExp goal = PreprocessFormula(m_currentProblem.Goal, invariants).GetEquivalentExp();

        return this.Solve(initialWorld, operators, goal, m_currentProblem.Metric);
      }
      finally
      {
        m_statistics.StopSolve();
      }
    }

    /// <summary>
    /// Preprocesses the given PDDL problem.
    /// The preprocess phase consists of setting offsets for all formulas, and specifying
    /// whether some described formulas are considered invariant.
    /// </summary>
    /// <param name="pb">The PDDL problem to process.</param>
    private void PreprocessProblem(PDDLObject pb)
    {
      HashSet<DescribedFormula> invariantFormulas;

      if (m_options.ComputeInvariants)
      {
        invariantFormulas = pb.GetInvariantDescribedFormulas();
      }
      else
      {
        invariantFormulas = new HashSet<DescribedFormula>();
      }

      foreach (DescribedFormula formula in pb.Formulas.Values.OfType<DescribedFormula>())
      {
        formula.Invariant = invariantFormulas.Contains(formula);
      }

      // The offsets must be in the correct order since some world implementations rely
      // on these values.
      //
      // Order by:
      // 1 - Is invariant (not applicable if the invariants are not computed)
      // 2 - Cycle checking
      Comparison<DescribedFormula> comparison = (f1, f2) =>
      {
        int value = f1.Invariant.CompareTo(f2.Invariant);
        if (value == 0)
          value = f1.DetectCycles.CompareTo(f2.DetectCycles);
        return value;
      };

      SetFormulasOffset(pb.Formulas.Values.OfType<AtomicFormula>(), comparison);
      SetFormulasOffset(pb.Formulas.Values.OfType<NumericFluent>(), comparison);
      SetFormulasOffset(pb.Formulas.Values.OfType<ObjectFluent>(), comparison);

      SetFormulasOffset(pb.Formulas.Values.OfType<DefinedPredicate>());
      SetFormulasOffset(pb.Formulas.Values.OfType<DefinedNumericFunction>());
      SetFormulasOffset(pb.Formulas.Values.OfType<DefinedObjectFunction>());
    }

    /// <summary>
    /// Sets the offset of the specified formulas.
    /// </summary>
    /// <typeparam name="T">The type of the formulas.</typeparam>
    /// <param name="formulas">The formulas to preprocess.</param>
    private void SetFormulasOffset<T>(IEnumerable<T> formulas)
      where T : RootFormula
    {
      SetFormulasOffset<T, T>(formulas, null);
    }

    /// <summary>
    /// Sets the offset of the specified formulas.
    /// The formulas are sorted according to the comparison function specified.
    /// </summary>
    /// <typeparam name="T">The type of the formulas.</typeparam>
    /// <typeparam name="U">The type of the comparison function operands.</typeparam>
    /// <param name="formulas">The formulas to preprocess.</param>
    /// <param name="comparisonFunction">The comparison function used to sort the formulas.</param>
    private void SetFormulasOffset<T, U>(IEnumerable<T> formulas,
                                         Comparison<U> comparisonFunction)
      where T : U
      where U : RootFormula
    {
      IEnumerable<T> allFormulas;
      if (comparisonFunction != null)
      {
        List<T> list = formulas.ToList();
        list.Sort((t1, t2) => comparisonFunction(t1, t2));
        allFormulas = list;
      }
      else
      {
        allFormulas = formulas;
      }

      int offset = 0;
      foreach (T formula in allFormulas)
      {
        formula.Offset = offset;
        offset += formula.GetDomainCardinality();
      }
    }

    /// <summary>
    /// Attempts to solve a given problem.
    /// </summary>
    /// <param name="initialWorld">The initial world.</param>
    /// <param name="operators">The list of operators to use.</param>
    /// <param name="goal">The goal formulation.</param>
    /// <param name="metric">The metric evaluation function.</param>
    /// <returns>A plan that solves the problem, or null if no plan was found.</returns>
    private Plan Solve(TLPlanDurativeClosedWorld initialWorld, List<IOperator> operators, ILogicalExp goal, MetricExp metric)
    {
      return m_graphSearch.Solve(initialWorld, operators, goal, metric);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the initial world of the current PDDL problem.
    /// </summary>
    /// <returns>The initial world of the current PDDL problem.</returns>
    private TLPlanDurativeClosedWorld GetInitialWorld()
    {
      // Initial facts/fluents of an initial world.
      ExtendedOpenWorld initialFacts = m_options.CreateWorld(m_currentProblem);

      // Invariant facts/fluents
      InvariantWorld invariants = m_options.CreateInvariantWorld(m_currentProblem);

      // The initialWorld world is the actual first world of the search graph.
      TLPlanDurativeClosedWorld initialWorld =
        new TLPlanDurativeClosedWorld(0.0, 
                                initialFacts, 
                                invariants, 
                                m_currentProblem.Constraints,
                                m_options);

      // This "transitive" initial world is used solely to setup the initial world and the
      // invariants.
      TransitiveInitialWorld transitiveInitialWorld = new TransitiveInitialWorld(initialWorld, invariants);
      foreach (IInitEl el in m_currentProblem.InitialWorld)
      {
        ExpController.Update(el, transitiveInitialWorld, transitiveInitialWorld);
      }

      // Progress the initial preference constraints.
      initialWorld.ProgressPreferences(m_currentProblem.ConstraintPreferences);

      // Progress the initial constraints.
      if (!initialWorld.ProgressCurrentConstraints())
      {
        throw new System.Exception("The initial state does not respect the constraints!");
      }

      return initialWorld;
    }

    /// <summary>
    /// Preprocesses a given logical expression.
    /// If invariants should be used to preprocess logical formulas (as specified in options),
    /// then the given logical expression is simplified.
    /// Else the expression is returned as is.
    /// </summary>
    /// <param name="exp">The logical expression to simplify.</param>
    /// <param name="invariants">Invariants information.</param>
    /// <returns>True, false, undefined, or the simplified expression.</returns>
    private LogicalValue PreprocessFormula(ILogicalExp exp, InvariantWorld invariants)
    {
      LogicalValue result;
      if (m_options.PreprocessLevel > TLPlanOptions.PreprocessingLevel.NONE)
      {
        result = ExpController.Simplify(exp, invariants);
      }
      else
      {
        return new LogicalValue(exp);
      }
      return result;
    }

    #region Instantiate Operators

    /// <summary>
    /// Returns a list of all instantiated operators, given a list of actions.
    /// </summary>
    /// <param name="actions">All possible actions.</param>
    /// <param name="invariants">Invariant information.</param>
    /// <param name="containsDurativeActions">Whether there are durative actions in the list.</param>
    /// <returns>A list of instantiated operators.</returns>
    private List<IOperator> InstantiateOperators(IEnumerable<IActionDef> actions, InvariantWorld invariants, bool containsDurativeActions)
    {
      m_statistics.StartOperators();

      // The actions are ordered by priority if the option is set.
      // Else they are used in the same way they were defined in the PDDL problem.
      List<IActionDef> orderedActions = actions.ToList();
      if (m_options.OrderActionsByPriority)
      {
        orderedActions.Sort((a1, a2) => -a1.Priority.CompareTo(a2.Priority));
      }

      List<IOperator> ops = new List<IOperator>();
      foreach (IActionDef a in orderedActions)
      {
        AtomicFormula name = new AtomicFormula(a.Name, a.GetParameters().ToList(), DescribedFormula.DefaultAttributes);
        InstantiateOperators(ops, a, name, 0, new LocalBindings(), invariants, containsDurativeActions);
      }

      // Domains with no durative actions can still need the wait-for-next-event operator if they contain
      // timed initial literals.
      if (m_options.ConcurrentActions)
      {
        // TODO: Let the user choose in which expression s/he wants to see the wait-for-next-event operator
        // Would this imply parsing it differently? Or just having some more work in the parser?
        ops.Add(new WaitForNextEventOperator(m_options.ElideWaitEvent));
        m_statistics.OperatorCount++;
      }

      m_statistics.StopOperators();
      m_statistics.FilteredOperatorCount = ops.Count;

      return ops;
    }

    /// <summary>
    /// Instantiate all possible operators by enumerating over all variables substitutions.
    /// </summary>
    /// <param name="ops">The list of operators to fetch.</param>
    /// <param name="action">The current action.</param>
    /// <param name="actionName">The name of the current action (represented as an atomic formula).
    /// </param>
    /// <param name="currentParam">The current parameter index.</param>
    /// <param name="bindings">The current set of bindings.</param>
    /// <param name="invariants">Invariants information.</param>
    /// <param name="containsDurativeActions">Whether the list of actions contains durative
    /// actions.</param>
    private void InstantiateOperators(List<IOperator> ops,
                                      IActionDef action,
                                      AtomicFormula actionName,
                                      int currentParam,
                                      LocalBindings bindings,
                                      InvariantWorld invariants,
                                      bool containsDurativeActions)
    {
      if (currentParam >= actionName.GetArity())
      {
        AddNewOp(ops, action, actionName, bindings, invariants, containsDurativeActions);
      }
      else
      {
        ObjectParameterVariable var = actionName.GetParameter(currentParam);
        List<Constant> values = var.GetTypeSet().Domain;
        foreach (Constant c in values)
        {
          bindings.Bind(var, c);
          InstantiateOperators(ops, action, actionName, currentParam + 1, bindings, invariants, 
                               containsDurativeActions);
        }
      }
    }

    /// <summary>
    /// Adds a new operator created from a specific action.
    /// </summary>
    /// <param name="ops">The list of operators to fetch.</param>
    /// <param name="action">The current action.</param>
    /// <param name="actionName">The name of the current action (represented as an atomic formula).
    /// </param>
    /// <param name="bindings">The current set of bindings.</param>
    /// <param name="invariants">Invariants information.</param>
    /// <param name="containsDurativeActions">Whether the list of actions contains durative
    /// actions.</param>
    /// <returns>Whether the new operator was added.</returns>
    private bool AddNewOp(List<IOperator> ops,
                          IActionDef action,
                          AtomicFormula actionName,
                          LocalBindings bindings,
                          InvariantWorld invariants,
                          bool containsDurativeActions)
    {
      m_statistics.OperatorCount++;

      List<ITerm> arguments = new List<ITerm>();
      foreach (ITerm p in action.GetParameters())
      {
        arguments.Add((Constant)bindings.GetBinding((ObjectParameterVariable)p));
      }
      AtomicFormulaApplication name = new AtomicFormulaApplication(actionName, arguments);

      IOperator op = null;
      if (action is Action)
        op = CreateOp((Action)action, name.ToString(), bindings, invariants, containsDurativeActions);
      else if (action is DurativeAction)
        op = CreateDurativeOp((DurativeAction)action, name.ToString(), bindings, invariants);
      else
        throw new NotSupportedException(string.Format("Actions of type {0} are not yet supported.", action.GetType().Name));

      if (op != null)
      {
        ops.Add(op);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Creates a new operator given an action and a set of bindings.
    /// </summary>
    /// <param name="action">The action to create an operator from.</param>
    /// <param name="name">The name of the operator to create.</param>
    /// <param name="bindings">The set of action's variables bindings.</param>
    /// <param name="invariants">Invariants information.</param>
    /// <param name="containsDurativeActions">Whether the list of actions contains 
    /// durative actions.</param>
    /// <returns>The new operator, or null if it was not created.</returns>
    private IOperator CreateOp(Action action, string name, LocalBindings bindings, InvariantWorld invariants, bool containsDurativeActions)
    {
      ILogicalExp precondition = (ILogicalExp)action.Precondition.Apply(bindings);

      LogicalValue result = PreprocessFormula(precondition, invariants);
      if (result.Exp != null)
      {
        // Expression cannot be evaluated using invariants only; add operator.
        precondition = result.Exp;
      }
      else if (result.Value.ToBool())
      {
        // Expression evaluates to true using invariants; add operator with a trivial precondition.
        precondition = TrueExp.True;
      }
      else
      {
        // Expression evaluates to false using invariants; do not instantiate this operator.
        return null;
      }

      return new Operator(name, precondition, (IEffect)action.Effect.Apply(bindings), m_options.ConcurrentActions && containsDurativeActions, false);
    }

    /// <summary>
    /// Creates a new operator given an action and a set of bindings.
    /// </summary>
    /// <param name="action">The action to create an operator from.</param>
    /// <param name="name">The name of the operator to create.</param>
    /// <param name="bindings">The set of action's variables bindings.</param>
    /// <param name="invariants">Invariants information.</param>
    /// <returns>The new operator, or null if it was not created.</returns>
    private IOperator CreateDurativeOp(DurativeAction action, string name, LocalBindings bindings, InvariantWorld invariants)
    {
      // TODO: evaluate "over all" and "at end"? should an action be terminated in a valid plan?      
      ILogicalExp precondition = (ILogicalExp)action.StartCondition.Apply(bindings);

      LogicalValue result = PreprocessFormula(precondition, invariants);
      if (result.Exp != null)
      {
        // Expression cannot be evaluated using invariants only; add operator.
        precondition = result.Exp;
      }
      else if (result.Value.ToBool())
      {
        // Expression evaluates to true using invariants; add operator with a trivial precondition.
        precondition = TrueExp.True;
      }
      else
      {
        // Expression evaluates to false using invariants; do not instantiate this operator.
        return null;
      }

      return new DurativeOperator(name, (ILogicalExp)action.Duration.Apply(bindings),
                                        precondition,
                                        (ILogicalExp)action.OverallCondition.Apply(bindings),
                                        (ILogicalExp)action.EndCondition.Apply(bindings),
                                        (IEffect)action.StartEffect.Apply(bindings),
                                        (IEffect)action.ContinuousEffect.Apply(bindings),
                                        (IEffect)action.OverallEffect.Apply(bindings),
                                        (IEffect)action.EndEffect.Apply(bindings),
                                        m_options.ConcurrentActions,
                                        false);
    }

    #endregion

    #endregion
  }
}
