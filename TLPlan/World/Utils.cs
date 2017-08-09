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
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PDDLParser;
using PDDLParser.Exp;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Struct;
using PDDLParser.Extensions;
using PDDLParser.Parser;

namespace TLPlan.World
{
  /// <summary>
  /// Represents a set of utility functions for worlds.
  /// </summary>
  public static class WorldUtils
  {
    #region Enumerations

    /// <summary>
    /// A flag enumeration indicating which parts of the world should be printed.
    /// </summary>
    [Flags]
    public enum PrintWorldPart
    {
      /// <summary>
      /// No part of the world will be printed.
      /// </summary>
      None = 0,
      /// <summary>
      /// The last operator leading to the world will be printed.
      /// </summary>
      LastOperator = 1,
      /// <summary>
      /// The elapsed time up to the world will be printed.
      /// </summary>
      ElapsedTime = 2,
      /// <summary>
      /// The remaining events in the world will be printed.
      /// </summary>
      RemainingEvents = 4,
      /// <summary>
      /// The variant facts of the world will be printed.
      /// </summary>
      Facts = 8,
      /// <summary>
      /// The invariant facts of the world will be printed.
      /// </summary>
      InvariantFacts = 16,
      /// <summary>
      /// The variant numeric fluents of the world will be printed.
      /// </summary>
      NumericFluents = 32,
      /// <summary>
      /// The invariant numeric fluents of the world will be printed.
      /// </summary>
      InvariantNumericFluents = 64,
      /// <summary>
      /// The variant object fluents of the world will be printed.
      /// </summary>
      ObjectFluents = 128,
      /// <summary>
      /// The invariant object fluents of the world will be printed.
      /// </summary>
      InvariantObjectFluents = 256,
      /// <summary>
      /// The current constraints of the world will be printed.
      /// </summary>
      CurrentConstraints = 512,
      /// <summary>
      /// The next constraints of the world will be printed.
      /// </summary>
      NextConstraints = 1024,
      /// <summary>
      /// The constraint preferences of the world will be printed.
      /// </summary>
      ConstraintPreferences = 2048,

      /// <summary>
      /// All facts of the world will be printed.
      /// </summary>
      AllFacts = Facts | InvariantFacts,
      /// <summary>
      /// All numeric fluents of the world will be printed.
      /// </summary>
      AllNumericFluents = NumericFluents | InvariantNumericFluents,
      /// <summary>
      /// All object fluents of the world will be printed.
      /// </summary>
      AllObjectFluents = ObjectFluents | InvariantObjectFluents,
      /// <summary>
      /// All fluents of the world will be printed.
      /// </summary>
      AllFluents = AllNumericFluents | AllObjectFluents,
      /// <summary>
      /// All facts and fluents of the world will be printed.
      /// </summary>
      AllFactsAndFluents = AllFacts | AllFluents,

      /// <summary>
      /// Every information about the world will be printed.
      /// </summary>
      All = ~None
    };

    #endregion

    #region Delegates

    /// <summary>
    /// A delegate to retrieve the value of a given <see cref="PDDLParser.Exp.Formula.FormulaApplication"/>.
    /// </summary>
    /// <typeparam name="ResultType">The type of the value.</typeparam>
    /// <typeparam name="ApplicationType">The specific type of <see cref="PDDLParser.Exp.Formula.FormulaApplication"/>.</typeparam>
    /// <param name="world">The world is in which the values are retrieved.</param>
    /// <param name="appl">The formula application whose value is to be known.</param>
    /// <returns>The value of the <see cref="PDDLParser.Exp.Formula.FormulaApplication"/> in the given world.</returns>
    private delegate ResultType ResultGetter<ResultType, ApplicationType>(TLPlanReadOnlyDurativeClosedWorld world, ApplicationType appl);

    #endregion

    #region Publics Methods

    /// <summary>
    /// Prints the given parts of the specified world on the <see cref="System.Console.Out"/> console output.
    /// </summary>
    /// <param name="problem">The PDDL problem.</param>
    /// <param name="node">The node containing the world to print.</param>
    /// <param name="printParts">The parts of the world to print.</param>
    public static void PrintWorld(PDDLObject problem, Node node, PrintWorldPart printParts)
    {
      PrintWorld(problem, node, printParts, false, false, Console.Out, Console.Out);
    }

    /// <summary>
    /// Prints the given parts of the specified world on the specified output stream, using different options.
    /// </summary>
    /// <param name="problem">The PDDL problem.</param>
    /// <param name="node">The node containing the world to print.</param>
    /// <param name="printParts">The parts of the world to print.</param>
    /// <param name="printAllPredicates">Whether to print all predicates, or only true facts and defined fluents.</param>
    /// <param name="alignLines">Whether to align the prints facts and fluents on new lines.</param>
    public static void PrintWorld(PDDLObject problem, Node node, PrintWorldPart printParts, bool printAllPredicates, bool alignLines)
    {
      PrintWorld(problem, node, printParts, printAllPredicates, alignLines, Console.Out, Console.Out);
    }

    /// <summary>
    /// Prints the given parts of the specified world on the specified output stream, using different options.
    /// </summary>
    /// <param name="problem">The PDDL problem.</param>
    /// <param name="node">The node containing the world to print.</param>
    /// <param name="printParts">The parts of the world to print.</param>
    /// <param name="printAllPredicates">Whether to print all predicates, or only true facts and defined fluents.</param>
    /// <param name="alignLines">Whether to align the prints facts and fluents on new lines.</param>
    /// <param name="titleWriter">The title text writer.</param>
    /// <param name="infoWriter">The main text writer.</param>
    public static void PrintWorld(PDDLObject problem, Node node, PrintWorldPart printParts, bool printAllPredicates, bool alignLines, TextWriter titleWriter, TextWriter infoWriter)
    {
      string atomPrintFormat = (printAllPredicates ? "{0} = {1}" : "{0}");
      string fluentPrintFormat = (printAllPredicates ? "{0} = {1}" : "(= {0} {1})");

      TLPlanReadOnlyDurativeClosedWorld world = node.World;

      // Print the last operator.
      if ((printParts & PrintWorldPart.LastOperator) != 0)
      {
        IOperator op = node.Operator;
        titleWriter.Write("Last operator: ");
        infoWriter.WriteLine(op != null ? op.Name : "none");
      }

      // Print the elapsed time.
      if ((printParts & PrintWorldPart.ElapsedTime) != 0)
      {
        titleWriter.Write("Elapsed time: ");
        infoWriter.WriteLine(world.TimeStamp.ToString("0.000000"));
      }

      // Print the remaining events.
      if ((printParts & PrintWorldPart.RemainingEvents) != 0 && !world.IsEventQueueEmpty)
      {
        string strRemainingEvents = "Remaining events: ";
        titleWriter.Write(strRemainingEvents);

        int numLength = ((int)world.EventQueue.Keys().Max()).ToString().Length;
        int totalLength = strRemainingEvents.Length + numLength + 9;

        LinkedList<string> printableEvents = new LinkedList<string>();
        foreach (KeyValuePair<double, Event> pair in world.EventQueue)
        {
          string strEvent = string.Join("\n", AlignLines(pair.Value.ToString().Split(new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries), totalLength).ToArray());
          printableEvents.AddLast(string.Format("{0:0.000000}: {1}", pair.Key, strEvent));
        }

        foreach (string str in AlignLines(printableEvents, strRemainingEvents.Length))
          infoWriter.WriteLine(str);
      }

      // Print the facts.
      if ((printParts & PrintWorldPart.AllFacts) != 0)
      {
        List<AtomicFormula> allFacts = new List<AtomicFormula>(problem.Formulas.Values.OfType<AtomicFormula>());

        if (allFacts.Count != 0)
        {
          Predicate<bool> hider = hider = v => false;
          if (!printAllPredicates)
            hider = v => !v;

          if ((printParts & PrintWorldPart.Facts) != 0)
          {
            List<AtomicFormula> facts = new List<AtomicFormula>(allFacts.Where(atom => !atom.Invariant));
            PrintFormulas<AtomicFormula, AtomicFormulaApplication, bool>(facts, world, "Facts: ", (w, a) => w.IsSet(a), hider, atomPrintFormat, alignLines, titleWriter, infoWriter);
          }
          if ((printParts & PrintWorldPart.InvariantFacts) != 0)
          {
            List<AtomicFormula> invFacts = new List<AtomicFormula>(allFacts.Where(atom => atom.Invariant));
            PrintFormulas<AtomicFormula, AtomicFormulaApplication, bool>(invFacts, world, "Invariant facts: ", (w, a) => w.IsSet(a), hider, atomPrintFormat, alignLines, titleWriter, infoWriter);
          }
        }
      }

      // Print the numeric fluents.
      if ((printParts & PrintWorldPart.AllNumericFluents) != 0)
      {
        List<NumericFluent> allNumericFluents = new List<NumericFluent>(problem.Formulas.Values.OfType<NumericFluent>());

        if (allNumericFluents.Count != 0)
        {
          Predicate<PDDLParser.Exp.Struct.Double> hider = value => false;
          if (!printAllPredicates)
            hider = value => value.Equals(PDDLParser.Exp.Struct.Double.Undefined);

          if ((printParts & PrintWorldPart.NumericFluents) != 0)
          {
            List<NumericFluent> numericFluents = new List<NumericFluent>(allNumericFluents.Where(fluent => !fluent.Invariant));
            PrintFormulas<NumericFluent, NumericFluentApplication, PDDLParser.Exp.Struct.Double>(numericFluents, world, "Numeric fluents: ", (w, a) => w.GetNumericFluent(a), hider, fluentPrintFormat, alignLines, titleWriter, infoWriter);
          }
          if ((printParts & PrintWorldPart.InvariantNumericFluents) != 0)
          {
            List<NumericFluent> invNumericFluents = new List<NumericFluent>(allNumericFluents.Where(fluent => fluent.Invariant));
            PrintFormulas<NumericFluent, NumericFluentApplication, PDDLParser.Exp.Struct.Double>(invNumericFluents, world, "Invariant numeric fluents: ", (w, a) => w.GetNumericFluent(a), hider, fluentPrintFormat, alignLines, titleWriter, infoWriter);
          }
        }
      }

      // Print the object fluents.
      if ((printParts & PrintWorldPart.AllObjectFluents) != 0)
      {
        List<ObjectFluent> allObjectFluents = new List<ObjectFluent>(problem.Formulas.Values.OfType<ObjectFluent>());

        if (allObjectFluents.Count != 0)
        {
          Predicate<ConstantExp> hider = value => false;
          if (!printAllPredicates)
            hider = value => value.Equals(ConstantExp.Undefined);

          if ((printParts & PrintWorldPart.ObjectFluents) != 0)
          {
            List<ObjectFluent> objectFluents = new List<ObjectFluent>(allObjectFluents.Where(fluent => !fluent.Invariant));
            PrintFormulas<ObjectFluent, ObjectFluentApplication, ConstantExp>(objectFluents, world, "Object fluents: ", (w, a) => w.GetObjectFluent(a), hider, fluentPrintFormat, alignLines, titleWriter, infoWriter);
          }
          if ((printParts & PrintWorldPart.InvariantObjectFluents) != 0)
          {
            List<ObjectFluent> invObjectFluents = new List<ObjectFluent>(allObjectFluents.Where(fluent => fluent.Invariant));
            PrintFormulas<ObjectFluent, ObjectFluentApplication, ConstantExp>(invObjectFluents, world, "Invariant object fluents: ", (w, a) => w.GetObjectFluent(a), hider, fluentPrintFormat, alignLines, titleWriter, infoWriter);
          }
        }
      }

      // Print the current constraints.
      if ((printParts & PrintWorldPart.CurrentConstraints) != 0 && world.CurrentConstraints != null && !(world.CurrentConstraints is TrueExp))
      {
        titleWriter.Write("Current constraint: ");
        infoWriter.WriteLine(world.CurrentConstraints.ToString());
      }

      // Print the next constraints.
      if ((printParts & PrintWorldPart.NextConstraints) != 0 && world.NextConstraints != null && !(world.NextConstraints is TrueExp))
      {
        titleWriter.Write("Next constraint: ");
        infoWriter.WriteLine(world.NextConstraints.ToString());
      }

      // Print the constraint preferences.
      if ((printParts & PrintWorldPart.ConstraintPreferences) != 0 && world.ConstraintPreferences.Count != 0)
      {
        string constraintPref = "Constraint preferences: ";
        titleWriter.Write(constraintPref);

        List<string> strConstraintPrefs = new List<string>();
        foreach (IConstraintPrefExp pref in world.ConstraintPreferences)
          strConstraintPrefs.Add(pref.ToString());

        foreach (string str in AlignLines(strConstraintPrefs, constraintPref.Length))
          infoWriter.WriteLine(str);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Pads the given strings with spaces so that they align nicely when printed. The first string is not padded.
    /// </summary>
    /// <param name="strings">The strings to pad with spaces.</param>
    /// <param name="startOffset">The number of spaces to add at the beginning of the strings.</param>
    /// <returns>An enumeration containing the padded strings.</returns>
    private static IEnumerable<string> AlignLines(IEnumerable<string> strings, int startOffset)
    {
      IEnumerator<string> strEnum = strings.GetEnumerator();

      if (strEnum.MoveNext())
        yield return strEnum.Current;

      while (strEnum.MoveNext())
        yield return string.Format("{0}{1}", new string(' ', startOffset), strEnum.Current);
    }

    /// <summary>
    /// Prints the given formulas, along with their values, to the specified output streams.
    /// </summary>
    /// <typeparam name="FormulaType">The specific subtype of <see cref="PDDLParser.Exp.Formula.DescribedFormula"/> that is used.</typeparam>
    /// <typeparam name="ApplicationType">The specfied subtype of <see cref="PDDLParser.Exp.Formula.FormulaApplication"/> that is used.</typeparam>
    /// <typeparam name="ResultType">The type of the values returned by the getter.</typeparam>
    /// <param name="formulas">The list of formulas that need to be printed.</param>
    /// <param name="world">The world to query for the formulas' values.</param>
    /// <param name="category">The string mentioning the type of formulas being printed.</param>
    /// <param name="getter">The function used to retrieve the formulas' values in the world.</param>
    /// <param name="hider">The predicate used to determine whether to print the formula or not. If the call to the hider returns true, the formula is not printed.</param>
    /// <param name="printFormat">The format string used for printing.</param>
    /// <param name="alignLines">Whether to align the output so that it prints nicely on multiple lines.</param>
    /// <param name="categoryWriter">The writer used to write the <paramref name="category"/> string.</param>
    /// <param name="infoWriter">The writer used to print the formulas and their values.</param>
    private static void PrintFormulas<FormulaType, ApplicationType, ResultType>(List<FormulaType> formulas, TLPlanReadOnlyDurativeClosedWorld world,
                                                                                string category, ResultGetter<ResultType, ApplicationType> getter,
                                                                                Predicate<ResultType> hider, string printFormat, bool alignLines,
                                                                                TextWriter categoryWriter, TextWriter infoWriter)
      where FormulaType : DescribedFormula
      where ApplicationType : FormulaApplication
    {
      if (formulas.Count != 0)
      {
        LinkedList<string> printableFormulas = new LinkedList<string>();

        foreach (FormulaType formula in formulas)
        {
          for (int i = 0; i < formula.GetDomainCardinality(); ++i)
          {
            ApplicationType formulaAppl = (ApplicationType)formula.InstantiateFromID(i);
            ResultType result = getter(world, formulaAppl);
            if (!hider(result))
              printableFormulas.AddLast(string.Format(printFormat, formulaAppl, result.ToString()));
          }
        }

        if (printableFormulas.Count != 0)
          categoryWriter.Write(category);

        if (alignLines)
        {
          foreach (string str in AlignLines(printableFormulas, category.Length))
            infoWriter.WriteLine(str);
        }
        else
        {
          infoWriter.WriteLine(string.Join(" ", printableFormulas.ToArray()));
        }
      }
    }

    #endregion
  }
}
