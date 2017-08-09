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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using PDDLParser.Action;
using PDDLParser.Exception;
using PDDLParser.Exp;
using PDDLParser.Exp.Comparison;
using PDDLParser.Exp.Constraint;
using PDDLParser.Exp.Constraint.TLPlan;
using PDDLParser.Exp.Effect;
using PDDLParser.Exp.Effect.Assign;
using PDDLParser.Exp.Formula;
using PDDLParser.Exp.Formula.TLPlan;
using PDDLParser.Exp.Formula.TLPlan.LocalVar;
using PDDLParser.Exp.Logical;
using PDDLParser.Exp.Logical.TLPlan;
using PDDLParser.Exp.Metric;
using PDDLParser.Exp.Numeric;
using PDDLParser.Exp.Numeric.TLPlan;
using PDDLParser.Exp.Struct;
using PDDLParser.Exp.Term;
using PDDLParser.Exp.Term.Type;
using PDDLParser.Extensions;
using PDDLParser.Parser.Lexer;

using Action = PDDLParser.Action.Action;
using Number = PDDLParser.Exp.Numeric.Number;
using Type = PDDLParser.Exp.Term.Type.Type;

namespace PDDLParser.Parser
{
  /**
 * Implements the <tt>Parser</tt> of PDD4L library. The parser can be
 * configured to accept only specified requirements of PDDL langage. The list of
 * requirements accepted are as follow:
 * <ul>
 * <li><tt>:strips</tt> - Basic STRIPS-style.</li>
 * <li><tt>:typing</tt> - Allows type names in declaration of variables</li>
 * <li><tt>:negative-preconditions</tt> - Allows <tt>not</tt> in goal and
 * preconditions descriptions.</li>
 * <li><tt>:disjunctive-preconditions</tt> - Allows <tt>or</tt> in goal
 * and preconditions descriptions.</li>
 * <li><tt>:equality</tt> - Supports <tt>=</tt> as built-in predicate.</li>
 * <li><tt>:existential-preconditions</tt> - Allows <tt>exists</tt> in goal
 * and preconditions descriptions.</li>
 * <li><tt>:universal-preconditions</tt> - Allows <tt>forall</tt> in goal
 * and preconditions descriptions.</li>
 * <li><tt>:quantified-preconditions</tt> - Is equivalent to <tt>:existential-preconditions</tt> +
 * <tt>:universal-preconditions</tt>.</li>
 * <li><tt>:conditional-effects</tt> - Allows <tt>when</tt> clause in
 * actionCtx effects.</li>
 * <li><tt>:fluents</tt> - Allows function definitions and use of effects
 * using assignement operators and numeric preconditions.</li>
 * <li><tt>:adl</tt> - Is equivalent to <tt>:strips</tt> + <tt>:typing</tt> +
 * <tt>:negative-preconditions</tt> + <tt>:disjunctive-preconditions</tt> +
 * <tt>:equality</tt> + <tt>:quantified-preconditions</tt> + <tt>:conditional-effects</tt>.</li>
 * <li><tt>:durative-actions</tt> - Allows durative actions. Note that this
 * does note imply <tt>:fluents</tt>.</li>
 * <li><tt>:derived-predicate</tt> - Allows predicates whose truth value is
 * defined by a formula.</li>
 * <li><tt>:time-initial-literals</tt> - Allows the initial state to specify
 * literals that will become true at a specified time point implies <tt>durative-actions</tt>.</li>
 * <li><tt>:preferences</tt> - Allows use of preferences in actionCtx
 * preconditions and goals.</li>
 * <li><tt>:constraints</tt> - Allows use of constraints fields in domain and
 * problem description. These may contain modal operator supporting trajectory
 * constraints.</li>
 * </ul>

 * @author Damien Pellier
 * @version 1.0, 14/01/03
 */
  public class Parser
  {
    #region Nested Classes

    #region private class ContextStack

    private class ContextStack
    {
      private Parser parser;
      private Stack<IDictionary<string, IVariable>> contexts;
      private IDictionary<string, IVariable> allQuantifiedVariables;
      private IDictionary<string, IVariable> allVariables;

      public ContextStack(Parser parser)
      {
        this.parser = parser;
        this.contexts = new Stack<IDictionary<string, IVariable>>();
        this.allQuantifiedVariables = new Dictionary<string, IVariable>();
        this.allVariables = new Dictionary<string, IVariable>();
      }

      public void pushContext(IEnumerable<IVariable> variables)
      {
        pushContext(variables, null);
      }

      public void pushContext(IEnumerable<IVariable> variables, SimpleNode node)
      {
        IDictionary<string, IVariable> context = new Dictionary<string, IVariable>();
        foreach (IVariable var in variables)
        {
          try
          {
            context.Add(var.Name, var);
            this.allVariables.Add(var.Name, var);
          }
          catch (ArgumentException)
          {
            this.parser.m_mgr.logParserError("IVariable \""
                        + var.ToTypedString()
                        + "\" duplicated.", parser.m_file,
                        (node != null) ? node.getLine() : 0,
                        (node != null) ? node.getColumn() : 0);
          }
        }
        this.contexts.Push(context);
      }

      public void pushQuantifiedContext(IEnumerable<IVariable> variables, SimpleNode node)
      {
        pushContext(variables, node);
        foreach (IVariable var in variables)
          this.allQuantifiedVariables[var.Name] = var;
      }

      public void popContext()
      {
        IDictionary<string, IVariable> context = this.contexts.Pop();
        foreach (KeyValuePair<string, IVariable> var in context)
        {
          this.allVariables.Remove(var.Key);
          this.allQuantifiedVariables.Remove(var.Key);
        }
      }

      public void popQuantifiedContext()
      {
        popContext();
      }

      public T getVariable<T>(T var, SimpleNode node) where T : IVariable
      {
        IVariable existing = null;
        if (allVariables.TryGetValue(var.Name, out existing))
        {
          if (existing is T)
          {
            return (T)existing;
          }
          else
          {
            this.parser.m_mgr.logParserError("Variable \""
              + existing + "\" is invalid; \"" + typeof(T).ToString() + "\" was expected instead.",
              this.parser.m_file, node.getLine(), node.getColumn());
            return var;
          }
        }
        else
        {
          this.parser.m_mgr.logParserError("Variable \""
                      + var.ToString()
                      + "\" undefined.", this.parser.m_file, node.getLine(), node.getColumn());
          return var;
        }
      }

      public IEnumerable<T> getAllQuantifiedVariables<T>() where T : IVariable
      {
        return allQuantifiedVariables.Values.OfType<T>();
      }
    }

    #endregion

    #region private class DummyFormulaApplication

    private class DummyFormulaApplication : FormulaApplication
    {
      public bool MayBeConstant { get; protected set; }

      public DummyFormulaApplication(string name, List<ITerm> arguments)
        : base(new AtomicFormula(name, 
                                 new List<ObjectParameterVariable>(),
                                 DescribedFormula.DefaultAttributes), 
               arguments)
      {
        MayBeConstant = false;
      }

      public DummyFormulaApplication(string name, bool mayBeConstant)
        : base(new AtomicFormula(name, 
                                 new List<ObjectParameterVariable>(),
                                 DescribedFormula.DefaultAttributes), 
               new List<ITerm>())
      {
        MayBeConstant = mayBeConstant;
      }

      public override FormulaApplication Apply(List<ITerm> arguments)
      {
        throw new NotSupportedException();
      }
    }

    #endregion

    #region Error Structures

    private struct GetFluentError
    {
      public enum ErrorType
      {
        NONE,
        DOES_NOT_MATCH,
        WRONG_TYPE,
        UNDEFINED,
      }

      public ErrorType Type { get; set; }
      public string Message { get; private set; }

      public GetFluentError(ErrorType type, string message)
        : this()
      {
        Type = type;
        Message = message;
      }
    }

    #endregion

    #endregion

    /**
     * The pddl file extension.
     */
    private const string PDDL_EXTENTION = ".pddl";

    private const string CONSTANT_STRING = "constant";
    private const string PREDICATE_STRING = "predicate";
    private const string FUNCTION_STRING = "function";

    /**
     * The error manager of the parser.
     */
    private ErrorManager m_mgr;

    /**
     * The parser input file.
     */
    private string m_file;

    /**
     * The PDDL object returned by the parser. 
     */
    private PDDLObject m_obj;

    private PDDLObject m_domain;

    private ContextStack m_contextStack;

    /**
       * The requirement accepted by the parser.
       * @uml.property  name="options"
       */
    private HashSet<RequireKey> m_acceptedRequirements;

    private enum ConditionTime
    {
      START,
      OVER,
      CONTINUOUS,
      END,
    }

    private enum DurationTime
    {
      NONE,
      START,
      END,
    }

    /**
     * Create a new <tt>Parser</tt>.
     */
    public Parser()
      : this(new HashSet<RequireKey>(RequireKey.AllKeys), new ErrorManager())
    {
    }

    /**
     * Create a new <tt>Parser</tt> with a specific errors manager.
     * 
     * @param acceptedRequirements the accepted requirements
     */
    public Parser(HashSet<RequireKey> acceptedRequirements)
      : this(acceptedRequirements, new ErrorManager())
    {
    }

    /**
     * Create a new <tt>Parser</tt> with a specific errors manager.
     * 
     * @param mgr the error manager of the parser.
     */
    public Parser(ErrorManager mgr)
      : this(new HashSet<RequireKey>(RequireKey.AllKeys), mgr)
    {
    }

    /**
     * Create a new <tt>Parser</tt> with a specific errors manager.
     * 
     * @param acceptedRequirements the accepted requirements.
     * @param mgr the error manager of the parser.
     */
    public Parser(HashSet<RequireKey> acceptedRequirements, ErrorManager mgr)
      : base()
    {
      this.m_acceptedRequirements = acceptedRequirements;
      this.m_mgr = mgr;
      this.m_domain = null;
      this.m_contextStack = new ContextStack(this);
    }

    private PDDLObject getDomain()
    {
      return (m_domain != null) ? m_domain : m_obj;
    }

    private HashSet<RequireKey> getFileRequirements()
    {
      HashSet<RequireKey> requirements = new HashSet<RequireKey>();
      if (m_obj != null)
        requirements.UnionWith(m_obj.Requirements);
      if (getDomain() != null)
        requirements.UnionWith(getDomain().Requirements);

      // When parsing expressions not related to a domain or problem
      if (requirements.Count == 0)
        requirements.UnionWith(RequireKey.AllKeys);

      return requirements;
    }

    /**
     * Parses a PDDL file.
     * 
     * @param file the file to parse.
     * @return The PDDL object representing the PDDL file or null if the parsing fails.
     * @throws FileNotFoundException if the file to parse does not exist.
     * @throws NullReferenceException if <code>file == null</code>.
     */
    public PDDLObject parse(string file)
    {
      PDDLObject obj = null;

      //try
      //{
      //  string ext = file.Substring(file.LastIndexOf(".")).ToLowerInvariant();
      //  if (!ext.Equals(Parser.PDDL_EXTENTION))
      //  {
      //    this.mgr.logParserWarning("string  \""
      //            + file + "\": " + " string should have \"" + Parser.PDDL_EXTENTION + "\" extension.", file);
      //  }
      //}
      //catch (ArgumentOutOfRangeException)
      //{
      //  this.mgr.logParserWarning("string  \""
      //              + file + "\": " + " string should have \"" + Parser.PDDL_EXTENTION + "\" extension.", file);
      //}
      SimpleNode root = null;
      this.m_file = file;
      Lexer.Lexer lexer = new Lexer.Lexer(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
      lexer.setErrorManager(this.m_mgr);
      lexer.setFile(this.m_file);

      root = lexer.parse();
      if (m_mgr.Contains(ErrorManager.Message.ERROR))
      {
        m_mgr.print(ErrorManager.Message.ALL);
        throw new ParseException("There were parsing errors.");
      }
      if (root != null)
      {
        obj = this.root(root);
      }

      return obj;
    }

    /// <summary>
    /// Parses an effect from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable effect.</param>
    /// <returns>The parsed effect.</returns>
    /// <seealso cref="Parser.effect"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IEffect parseEffect(string input)
    {
      return parseEffect(input, new IVariable[] { });
    }

    /// <summary>
    /// Parses an effect from the given string with the given context.
    /// </summary>
    /// <param name="input">The string representing a parsable effect.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed effect.</param>
    /// <returns>The parsed effect.</returns>
    /// <seealso cref="Parser.effect"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IEffect parseEffect(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
                  lexer => lexer.effect(),
                  (parser, node) => parser.effect(node));
    }

    /// <summary>
    /// Parses a logical expression from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable logical expression.</param>
    /// <returns>The parsed logical expression.</returns>
    /// <seealso cref="Parser.gd"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public ILogicalExp parseLogicalExp(string input)
    {
      return parseLogicalExp(input, new IVariable[] { });
    }

    /// <summary>
    /// Parses a logical expression from the given string with the given context.
    /// </summary>
    /// <param name="input">The string representing a parsable logical expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed logical expression.</param>
    /// <returns>The parsed logical expression.</returns>
    /// <seealso cref="Parser.gd"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public ILogicalExp parseLogicalExp(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
                  lexer => lexer.gd(),
                  (parser, node) => parser.gd(node));
    }

    /// <summary>
    /// Parses a numeric expression from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable numeric expression.</param>
    /// <returns>The parsed numeric expression.</returns>
    /// <seealso cref="Parser.f_exp"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public INumericExp parseNumericExp(string input)
    {
      return parseNumericExp(input, new IVariable[] { });
    }

    /// <summary>
    /// Parses a numeric expression from the given string with the given context.
    /// </summary>
    /// <param name="input">The string representing a parsable numeric expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed numeric expression.</param>
    /// <returns>The parsed numeric expression.</returns>
    /// <seealso cref="Parser.f_exp"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public INumericExp parseNumericExp(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
                  lexer => lexer.f_exp(),
                  (parser, node) => parser.f_exp(node));
    }

    /// <summary>
    /// Parses a term from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable term.</param>
    /// <returns>The parsed term.</returns>
    /// <seealso cref="Parser.term"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public ITerm parseTerm(string input)
    {
      return parseTerm(input, new IVariable[] { });
    }

    /// <summary>
    /// Parses a term from the given string with the given context.
    /// </summary>
    /// <param name="input">The string representing a parsable term.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed term.</param>
    /// <returns>The parsed term.</returns>
    /// <seealso cref="Parser.term"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public ITerm parseTerm(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
                  lexer => lexer.term(),
                  (parser, node) => parser.term(node));
    }

    /// <summary>
    /// Parses a constraint expression from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable constraint expression.</param>
    /// <returns>The parsed constraint expression.</returns>
    /// <seealso cref="Parser.con_gd"/>
    /// <seealso cref="Parser.sub_con_gd"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IConstraintExp parseConstraintExp(string input)
    {
      return parseConstraintExp(input, new IVariable[] { });
    }

    /// <summary>
    /// Parses a constraint expression from the given string with the given context.
    /// </summary>
    /// <param name="input">The string representing a parsable constraint expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed constraint expression.</param>
    /// <returns>The parsed constraint expression.</returns>
    /// <seealso cref="Parser.con_gd"/>
    /// <seealso cref="Parser.sub_con_gd"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IConstraintExp parseConstraintExp(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
        new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IConstraintExp>>[] {
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IConstraintExp>>(
            lexer => lexer.con_gd(),
            (parser, node) => parser.con_gd(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IConstraintExp>>(
            lexer => lexer.sub_con_gd(),
            (parser, node) => parser.sub_con_gd(node))
        });
    }

    /// <summary>
    /// Parses an evaluable expression from the given string.
    /// </summary>
    /// <remarks>
    /// An evaluable expression is either a logical expression, a numeric expression, or a term.
    /// </remarks>
    /// <param name="input">The string representing a parsable evaluable expression.</param>
    /// <returns>The parsed evaluable expression.</returns>
    /// <seealso cref="Parser.parseLogicalExp(string)"/>
    /// <seealso cref="Parser.parseNumericExp(string)"/>
    /// <seealso cref="Parser.parseTerm(string)"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IEvaluableExp parseEvaluableExp(string input)
    {
      return parseEvaluableExp(input, new List<IVariable>());
    }

    /// <summary>
    /// Parses an evaluable expression from the given string with the given context.
    /// </summary>
    /// <remarks>
    /// An evaluable expression is either a logical expression, a numeric expression, or a term.
    /// </remarks>
    /// <param name="input">The string representing a parsable evaluable expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed evaluable expression.</param>
    /// <returns>The parsed evaluable expression.</returns>
    /// <seealso cref="Parser.parseLogicalExp(string)"/>
    /// <seealso cref="Parser.parseNumericExp(string)"/>
    /// <seealso cref="Parser.parseTerm(string)"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IEvaluableExp parseEvaluableExp(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
        new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IEvaluableExp>>[] {
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IEvaluableExp>>(
            lexer => lexer.gd(),
            (parser, node) => parser.gd(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IEvaluableExp>>(
            lexer => lexer.f_exp(),
            (parser, node) => parser.f_exp(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IEvaluableExp>>(
            lexer => lexer.term(),
            (parser, node) => parser.term(node)) 
        });
    }

    /// <summary>
    /// Parses an expression from the given string.
    /// </summary>
    /// <remarks>
    /// The expression parsed is either a logical expression, a numeric expression, a term,
    /// a constraint expression, an effect, or a metric.
    /// </remarks>
    /// <param name="input">The string representing a parsable expression.</param>
    /// <returns>The parsed expression.</returns>
    /// <seealso cref="Parser.parseLogicalExp(string)"/>
    /// <seealso cref="Parser.parseNumericExp(string)"/>
    /// <seealso cref="Parser.parseTerm(string)"/>
    /// <seealso cref="Parser.parseEffect(string)"/>
    /// <seealso cref="Parser.parseConstraintExp(string)"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IExp parseExp(string input)
    {
      return parseExp(input, new List<IVariable>());
    }

    /// <summary>
    /// Parses an expression from the given string with the given context.
    /// </summary>
    /// <remarks>
    /// The expression parsed is either a logical expression, a numeric expression, a term,
    /// a constraint expression, an effect, or a metric.
    /// </remarks>
    /// <param name="input">The string representing a parsable expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed expression.</param>
    /// <returns>The parsed expression.</returns>
    /// <seealso cref="Parser.parseLogicalExp(string)"/>
    /// <seealso cref="Parser.parseNumericExp(string)"/>
    /// <seealso cref="Parser.parseTerm(string)"/>
    /// <seealso cref="Parser.parseEffect(string)"/>
    /// <seealso cref="Parser.parseConstraintExp(string)"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IExp parseExp(string input, IEnumerable<IVariable> context)
    {
      return parseExpression(input, context,
        new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>[] {
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>(
            lexer => lexer.gd(),
            (parser, node) => parser.gd(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>(
            lexer => lexer.f_exp(),
            (parser, node) => parser.f_exp(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>(
            lexer => lexer.term(),
            (parser, node) => parser.term(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>(
            lexer => lexer.con_gd(),
            (parser, node) => parser.con_gd(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>(
            lexer => lexer.effect(),
            (parser, node) => parser.effect(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IExp>>(
            lexer => lexer.metric_spec(),
            (parser, node) => parser.metric_spec(node))
        });
    }

    /// <summary>
    /// Parses an action from the given string.
    /// </summary>
    /// <remarks>
    /// The parsed action may be a simple STRIPS-like action or a durative action.
    /// </remarks>
    /// <param name="input">The string representing a parsable action.</param>
    /// <returns>The parsed action.</returns>
    /// <seealso cref="Parser.action_def"/>
    /// <seealso cref="Parser.durative_action_def"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IActionDef parseAction(string input)
    {
      return parseExpression(input, new List<IVariable>(),
        new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IActionDef>>[] {
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IActionDef>>(
            lexer => lexer.action_def(),
            (parser, node) => parser.action_def(node)),
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<IActionDef>>(
            lexer => lexer.durative_action_def(),
            (parser, node) => parser.durative_action_def(node))
        });
    }

    /// <summary>
    /// Parses an initial literal from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable initial literal.</param>
    /// <returns>The parsed initial literal.</returns>
    /// <seealso cref="Parser.init_el"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public IInitEl parseInitEl(string input)
    {
      return parseExpression(input, new List<IVariable>(),
                  lexer => lexer.init_el(),
                  (parser, node) => parser.init_el(node));
    }

    /// <summary>
    /// Parses the structure definition of a PDDL domain from the given string.
    /// </summary>
    /// <param name="input">The string representing a parsable structure definition of a PDDL domain.</param>
    /// <returns>The parsed structure definition of a PDDL domain.</returns>
    /// <seealso cref="Parser.structure_def"/>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    public object parseStructure(string input)
    {
      return parseExpression(input, new List<IVariable>(),
                  lexer => lexer.structure_def(),
                  (parser, node) => parser.structure_def(node));
    }

    /// <summary>
    /// A delegate used to provide the right lexer function to parse the wanted expression.
    /// </summary>
    /// <param name="lexer">The lexer used to parse the expression.</param>
    private delegate void lexerFunctDelegate(Lexer.Lexer lexer);
    /// <summary>
    /// A delegate used to provide the right parser function to parse the wanted expression
    /// from the node issued by the lexer.
    /// </summary>
    /// <typeparam name="T">The type of the returned expression.</typeparam>
    /// <param name="parser">The parser used to parse the expression from the lexer's node.</param>
    /// <param name="node">The lexer's node.</param>
    /// <returns>The parsed expression.</returns>
    private delegate T parserFunctDelegate<T>(Parser parser, SimpleNode node);

    /// <summary>
    /// Parses an expression from a string and a context, using the provided lexer and parser functions.
    /// </summary>
    /// <typeparam name="T">The type of the returned expression.</typeparam>
    /// <param name="input">The string representing the parsable expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed expression.</param>
    /// <param name="lexerFunct">The delegate used to provide the right lexer function to parse the wanted expression.</param>
    /// <param name="parserFunct">The delegate used to provide the right parser function to parse the wanted expression
    /// from the node issued by the lexer.</param>
    /// <returns>The parsed expression.</returns>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    private T parseExpression<T>(string input, IEnumerable<IVariable> context,
                                lexerFunctDelegate lexerFunct, parserFunctDelegate<T> parserFunct)
    {
      return parseExpression<T>(input, context,
        new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<T>>[] {
          new KeyValuePair<lexerFunctDelegate, parserFunctDelegate<T>>(lexerFunct, parserFunct)
        });
    }

    /// <summary>
    /// Parses an expression from a string and a context, using the first of several provided 
    /// lexer and parser functions that manages to parse the input.
    /// </summary>
    /// <typeparam name="T">The type of the returned expression.</typeparam>
    /// <param name="input">The string representing the parsable expression.</param>
    /// <param name="context">An enumeration of variable that might be in the parsed expression.</param>
    /// <param name="functions">An array of lexer and parser functions used to parse the wanted expression.</param>
    /// <returns>The parsed expression.</returns>
    /// <exception cref="System.Exception">Thrown when parsing fails.</exception>
    private T parseExpression<T>(string input, IEnumerable<IVariable> context,
                            KeyValuePair<lexerFunctDelegate, parserFunctDelegate<T>>[] functions)
    {
      Lexer.Lexer lexer = new Lexer.Lexer(new StringReader(input));
      this.m_mgr.clear();
      lexer.setErrorManager(this.m_mgr);

      bool parserException = false;
      System.Exception exception = null;
      foreach (KeyValuePair<lexerFunctDelegate, parserFunctDelegate<T>> functs in functions)
      {
        try
        {
          // Lexer
          functs.Key(lexer);
          if (this.m_mgr.Contains(ErrorManager.Message.ERROR))
          {
            throw new System.Exception(m_mgr.getUntypedMessages(ErrorManager.Message.ERROR).Aggregate((s1, s2) => s1 + "\n" + s2));
          }

          // Parser
          this.m_contextStack.pushContext(context);
          T t = functs.Value(this, lexer.popRootNode());
          this.m_contextStack.popContext();
          if (this.m_mgr.Contains(ErrorManager.Message.ERROR))
          {
            System.Exception e = new System.Exception(m_mgr.getUntypedMessages(ErrorManager.Message.ERROR).Aggregate((s1, s2) => s1 + "\n" + s2));
            exception = e;
            parserException = true;
            throw e;
          }
          return t;
        }
        catch (System.Exception e)
        {
          if (!parserException)
            exception = e;

          this.m_mgr.clear();
          lexer.ReInit(new StringReader(input));
        }
      }
      throw new System.Exception("Could not parse input: " + input + ", exception was: " + exception);
    }

    /**
     * Prints the syntaxic tree of a specific pddl file.
     * 
     * @param file the pddl file.
     * @throws FileNotFoundException if the file to parse does not exist.
     * @throws NullReferenceException if <code>file == null</code>.
     */
    public void printSyntaxicTree(string file)
    {
      try
      {
        this.m_file = file;
        Lexer.Lexer lexer = new Lexer.Lexer(new FileStream(file, FileMode.Open, FileAccess.Read));
        lexer.setFile(file);
        lexer.setErrorManager(this.m_mgr);
        SimpleNode root = lexer.parse();
        root.dump("");
      }
      catch (ParseException pe)
      {
        Console.Error.WriteLine(pe.StackTrace);
      }
    }

    /**
     * Returns the error manager of the compiler.
     * 
     * @return the error manager of the compiler.
     */
    public ErrorManager getErrorManager()
    {
      return this.m_mgr;
    }

    /**
     * Sets a new error manager to the compiler.
     * 
     * @param mgr the new error manager of the compiler.
     */
    public void setErrorManager(ErrorManager mgr)
    {
      this.m_mgr = mgr;
    }

    /**
     * Creates a new PDDL
     * object containing also the domain informations. This method is usefull to
     * check the symbol shared between domain and problem input files. 
     * 
     * @param domain the domain. 
     * @param problem the problem.
     * @return a PDDL object containing all the information needed for planning.
     * @throws NullReferenceException if <code>domain == null || problem == null</code>.
     */
    public PDDLObject link(IDomain domain, IProblem problem)
    {
      PDDLObject dom = (PDDLObject)domain;
      PDDLObject pb = (PDDLObject)problem;
      bool failure = false;
      PDDLObject obj = new PDDLObject();

      obj.Content = PDDLObject.PDDLContent.FULL_PROBLEM;
      obj.DomainFile = domain.DomainFile;
      obj.ProblemFile = problem.ProblemFile;
      obj.ProblemName = problem.ProblemName;
      obj.Actions = dom.Actions;
      obj.TypeSetSet = dom.TypeSetSet;
      obj.Constants = new Dictionary<string, Constant>(dom.Constants);
      obj.ContainsDurativeActions = dom.ContainsDurativeActions;

      foreach (Constant cts in pb.Constants.Values)
      {
        if (obj.Constants.ContainsKey(cts.Name))
        {
          this.m_mgr.logLinkerError("Constant " + cts.Name + " duplicated in domain and problem.", obj.DomainFile);
        }
        else
        {
          obj.Constants.Add(cts.Name, cts);
        }
      }

      // Reconstruct type domains
      IDictionary<Type, HashSet<Constant>> typeDomains = new Dictionary<Type, HashSet<Constant>>();

      foreach (Constant cst in obj.Constants.Values)
      {
        foreach (Type type in cst.GetTypeSet())
        {
          HashSet<Constant> setCst;
          if (!typeDomains.TryGetValue(type, out setCst))
          {
            setCst = new HashSet<Constant>();
            typeDomains[type] = setCst;
          }
          setCst.Add(cst);
        }
      }

      foreach (KeyValuePair<Type, HashSet<Constant>> typeDomain in typeDomains)
      {
        typeDomain.Key.TypeDomain = typeDomain.Value;
      }

      if (!(dom.Constraints is TrueExp))
      {
        if (!(pb.Constraints is TrueExp))
        {
          obj.Constraints = new AndConstraintExp(dom.Constraints, pb.Constraints);
        }
        else
        {
          obj.Constraints = dom.Constraints;
        }
      }
      else
      {
        obj.Constraints = pb.Constraints;
      }

      obj.DomainName = domain.DomainName;
      obj.Goal = pb.Goal;
      obj.InitialWorld.AddRange(pb.InitialWorld);
      obj.InitialWorld.AddRange(dom.InitialWorld); // This may contain dummy literals (e.g. when using the "hold-during" constraint)
      obj.Metric = pb.Metric ?? new MinimizeExp(new TotalTimeExp(), null);
      obj.GetAllGoalWorldsInstance = dom.GetAllGoalWorldsInstance;

      // Initialize all preference counters to zero (all preferences with the same name have the same counter)
      foreach (NumericFluentApplication counter in dom.AllPreferenceCounters.Values)
      {
        obj.InitialWorld.Add(new NumericAssign(counter, new Number(0)));
      }

      // Merge all preferences into the full problem
      obj.AllPreferences.DictionaryMergeList(dom.AllPreferences);
      obj.AllPreferences.DictionaryMergeList(pb.AllPreferences);

      // Merge the preference counters
      foreach (KeyValuePair<string, NumericFluentApplication> pair in dom.AllPreferenceCounters)
        obj.AllPreferenceCounters[pair.Key] = pair.Value;

      // Merge all the constraint preferences into the full problem
      obj.ConstraintPreferences.AddRange(dom.ConstraintPreferences);
      obj.ConstraintPreferences.AddRange(pb.ConstraintPreferences);

      // Set all "is-violated" expressions
      obj.AllIsViolatedExpressions = pb.AllIsViolatedExpressions;

      if (pb.Requirements.Count > 1 /* Minimum is :STRIPS */ && !pb.Requirements.SetEquals(dom.Requirements))
      {
        this.m_mgr.logLinkerWarning("domain and problem should have the same requirements.",
                    problem.ProblemFile);
      }
      obj.Requirements = dom.Requirements;
      obj.Requirements.UnionWith(pb.Requirements);

      foreach (KeyValuePair<string, RootFormula> pair in dom.Formulas)
        obj.Formulas.Add(pair.Key, pair.Value);
      foreach (KeyValuePair<string, RootFormula> pair in pb.Formulas)
        obj.Formulas.Add(pair.Key, pair.Value);


      obj.Preprocess();

      return failure ? null : obj;
    }

    /**
     * Extracts the object structures from the <code>ROOT</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ROOT</code> node.
     * @return the domain extract.
     * @throws ParserException if an error occurs while parsing.
     */
    private PDDLObject root(SimpleNode node)
    {
      SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
      switch (cn.getId())
      {
        case LexerTreeConstants.JJTDOMAIN:
          this.m_domain = null;
          this.m_domain = this.domain(cn);
          return this.m_domain;
        case LexerTreeConstants.JJTPROBLEM:
          return this.problem(cn);
        default:
          throw new ParserException(
                      "An internal parser error occurs: node "
                                  + cn.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from a <code>PROBLEM</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PROBLEM</code> node.
     * @return the problem structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private PDDLObject problem(SimpleNode node)
    {
      this.m_obj = new PDDLObject();
      this.m_obj.Content = PDDLObject.PDDLContent.PARTIAL_PROBLEM;
      this.m_obj.ProblemFile = this.m_file;
      getDomain().TypeSetSet.ClearAllButDomainConstants();

      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTPROBLEM_NAME:
            //string name = new FileInfo(this.file).Name;
            //int length = name.LastIndexOf(".");
            //if (!cn.GetImage().Equals((length < 0 ? name : name.Substring(0, length))))
            //{
            //  this.mgr.logParserWarning("problem \"" + cn.GetImage() + "\" must be defined in a file \""
            //              + cn.GetImage() + ".pddl\".", this.file, cn.getLine(),
            //              cn.getColumn());
            //}
            this.m_obj.ProblemName = cn.GetImage();
            break;
          case LexerTreeConstants.JJTDOMAIN_NAME:
            this.m_obj.DomainName = cn.GetImage();
            break;
          case LexerTreeConstants.JJTREQUIRE_DEF:
            this.requireDef(cn);
            break;
          case LexerTreeConstants.JJTOBJECT_DECLARATION:
            this.object_declaration(cn);
            break;
          case LexerTreeConstants.JJTINIT:
            this.init(cn);
            break;
          case LexerTreeConstants.JJTGOAL:
            this.goal(cn);
            break;
          case LexerTreeConstants.JJTCONSTRAINTS:
            this.constraints(cn);
            break;
          case LexerTreeConstants.JJTMETRIC_SPEC:
            this.metric_spec(cn);
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return m_obj;
    }

    /**
     * Extracts the object structures from the <code>METRIC_SPEC</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>METRIC_SPEC</code> node.
     * @return the metric expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private MetricExp metric_spec(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        List<string> preferences = new List<string>();

        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTMAXIMIZE:
            m_obj.Metric = this.maximize(cn, ref preferences);
            break;
          case LexerTreeConstants.JJTMINIMIZE:
            m_obj.Metric = this.minimize(cn, ref preferences);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                            + cn.getLabel() + " unexpected.");
        }

        // Verify if some named preferences weren't referred to in the metric.
        IEnumerable<string> missedNamedPrefs = m_obj.AllPreferences.Where(p => !p.Value.First().Unnamed).Keys() // Unnamed in problem
                                               .Union(getDomain().AllPreferences.Where(p => !p.Value.First().Unnamed).Keys()) // Unnamed in domain
                                               .Except(preferences); // Referred preferences in metric
        if (!missedNamedPrefs.IsEmpty())
        {
          this.m_mgr.logParserWarning("The following named preferences were not referred to : " +
                                    missedNamedPrefs.Aggregate((s1, s2) => s1 + ", " + s2), this.m_file);
        }

        return m_obj.Metric;
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    private INumericExp GetUnnamedPreferencesExp()
    {
      INumericExp unnamedPrefsExp = null;

      List<IPrefExp> lstUnnamedPrefs = new List<IPrefExp>();
      foreach (List<IPrefExp> lst in getDomain().AllPreferences.Values)
        if (lst.First().Unnamed)
          lstUnnamedPrefs.AddRange(lst);
      foreach (List<IPrefExp> lst in m_obj.AllPreferences.Values)
        if (lst.First().Unnamed)
          lstUnnamedPrefs.AddRange(lst);

      if (!lstUnnamedPrefs.IsEmpty())
      {
        List<INumericExp> unnamedViolatedPrefs = lstUnnamedPrefs.Select<IPrefExp, INumericExp>(pref =>
        {
          IEnumerable<AtomicFormulaApplication> constraintsAtoms = Enumerable.Empty<AtomicFormulaApplication>();
          NumericFluentApplication counter = getDomain().GetPreferenceCounter(pref.Name);

          IsViolatedExp violated = new IsViolatedExp(pref.Name, counter);
          m_obj.AllIsViolatedExpressions.Add(violated);

          return violated;
        }).ToList();

        unnamedPrefsExp = new NArityAdd(unnamedViolatedPrefs);
      }

      return unnamedPrefsExp;
    }

    /**
     * Extracts the object structures from the <code>MAXIMIZE</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MAXIMIZE</code> node.
     * @return the maximize expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private MetricExp maximize(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTMETRIC_F_EXP)
        {
          INumericExp exp = this.metric_f_exp(cn, ref preferences);

          // Take unnamed preferences into account
          return new MaximizeExp(exp, GetUnnamedPreferencesExp());
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>MINIMIZE</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MINIMIZE</code> node.
     * @return the minimize expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private MetricExp minimize(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTMETRIC_F_EXP)
        {
          INumericExp exp = this.metric_f_exp(cn, ref preferences);

          // Take unnamed preferences into account
          return new MinimizeExp(exp, GetUnnamedPreferencesExp());
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>METRIC_F_EXP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>METRIC_F_EXP</code> node.
     * @return the metric function expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp metric_f_exp(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTMULTI_OP_METRIC_F_EXP:
            return this.multi_op_metric_f_exp(cn, ref preferences);
          case LexerTreeConstants.JJTNUMBER:
            return this.number(cn);
          case LexerTreeConstants.JJTCONSTANT_F_HEAD:
            return this.constant_f_head(cn);
          case LexerTreeConstants.JJTTOTAL_TIME:
            return new TotalTimeExp();
          case LexerTreeConstants.JJTVIOLATED_PREF_EXP:
            return this.violated_pref_exp(cn, ref preferences);

          default:
            throw new ParserException("An internal parser error occurs: node "
                            + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>VIOLATED_PREF_EXP</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>VIOLATED_PREF_EXP</code> node.
     * @return the violated preference expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ReservedNumericExp violated_pref_exp(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTPREF_NAME)
        {
          string prefName = this.pref_name(cn);
          List<IPrefExp> pref1, pref2;
          if (!getDomain().AllPreferences.TryGetValue(prefName, out pref1) & // THIS CONJUNCTION SHALL NOT SHORTCIRCUIT!
              !m_obj.AllPreferences.TryGetValue(prefName, out pref2))
          {
            this.m_mgr.logParserError(string.Format("Preference \"{0}\" is undefined.", prefName),
                                    m_file, cn.getLine(), cn.getColumn());
          }

          preferences.Add(prefName);

          // Get all the preferences named "prefName" in the domain and the problem
          List<IPrefExp> pref = new List<IPrefExp>((pref1 ?? new List<IPrefExp>()).Concat(pref2 ?? new List<IPrefExp>()));

          NumericFluentApplication counter = getDomain().GetPreferenceCounter(prefName);

          IsViolatedExp violated = new IsViolatedExp(prefName, counter);
          m_obj.AllIsViolatedExpressions.Add(violated);

          return violated;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>CONSTANT_F_HEAD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>CONSTANT_F_HEADF_HEAD</code> node.
     * @return the function head structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp constant_f_head(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTCONSTANT_FUNCTION_HEAD_OR_FUNCTOR:
            DummyFormulaApplication dummyFormula = constant_function_head_or_functor(cn);
            INumericExp fluent = getFormulaApplication<INumericExp>(dummyFormula, false, cn);
            if (fluent == null)
              fluent = new NumericFluentApplication(new NumericFluent(dummyFormula.Name, 
                                                                      new List<ObjectParameterVariable>(), 
                                                                      DescribedFormula.DefaultAttributes),
                                                    dummyFormula.GetArguments());
            return fluent;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DummyFormulaApplication predicate_head(SimpleNode node)
    {
      if (node.jjtGetNumChildren() > 0)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTPREDICATE)
        {
          string predicate = this.predicate(cn1);

          List<ITerm> arguments = new List<ITerm>();
          for (int i = 1; i < node.jjtGetNumChildren(); i++)
          {
            SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
            switch (cn.getId())
            {
              case LexerTreeConstants.JJTTERM:
                arguments.Add(this.term(cn));
                break;
              default:
                throw new ParserException("An internal parser error occurs: node "
                            + cn.getLabel()
                            + " unexpected.");
            }
          }
          return new DummyFormulaApplication(predicate, arguments);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DummyFormulaApplication constant_predicate_head(SimpleNode node)
    {
      if (node.jjtGetNumChildren() > 0)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTPREDICATE)
        {
          string predicate = this.predicate(cn1);

          List<Constant> arguments = new List<Constant>();
          for (int i = 1; i < node.jjtGetNumChildren(); i++)
          {
            SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
            switch (cn.getId())
            {
              case LexerTreeConstants.JJTCONSTANT:
                arguments.Add(this.constant(cn));
                break;
              default:
                throw new ParserException("An internal parser error occurs: node "
                            + cn.getLabel()
                            + " unexpected.");
            }
          }
          return new DummyFormulaApplication(predicate, arguments.Cast<ITerm>().ToList());
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DummyFormulaApplication function_head(SimpleNode node)
    {
      if (node.jjtGetNumChildren() > 0)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTFUNCTOR)
        {
          string functor = this.functor(cn1);

          List<ITerm> arguments = new List<ITerm>();
          for (int i = 1; i < node.jjtGetNumChildren(); i++)
          {
            SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
            switch (cn.getId())
            {
              case LexerTreeConstants.JJTTERM:
                arguments.Add(this.term(cn));
                break;
              default:
                throw new ParserException("An internal parser error occurs: node "
                            + cn.getLabel()
                            + " unexpected.");
            }
          }
          return new DummyFormulaApplication(functor, arguments);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DummyFormulaApplication function_head_or_functor(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        switch (cn1.getId())
        {
          case LexerTreeConstants.JJTFUNCTOR:
            return new DummyFormulaApplication(this.functor(cn1), true);
          case LexerTreeConstants.JJTFUNCTION_HEAD:
            return this.function_head(cn1);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DummyFormulaApplication constant_function_head(SimpleNode node)
    {
      if (node.jjtGetNumChildren() > 0)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTFUNCTOR)
        {
          string functor = this.functor(cn1);

          List<Constant> arguments = new List<Constant>();
          for (int i = 1; i < node.jjtGetNumChildren(); i++)
          {
            SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
            switch (cn.getId())
            {
              case LexerTreeConstants.JJTCONSTANT:
                arguments.Add(this.constant(cn));
                break;
              default:
                throw new ParserException("An internal parser error occurs: node "
                            + cn.getLabel()
                            + " unexpected.");
            }
          }
          return new DummyFormulaApplication(functor, arguments.Cast<ITerm>().ToList());
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DummyFormulaApplication constant_function_head_or_functor(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        switch (cn1.getId())
        {
          case LexerTreeConstants.JJTFUNCTOR:
            return new DummyFormulaApplication(this.functor(cn1), true);
          case LexerTreeConstants.JJTCONSTANT_FUNCTION_HEAD:
            return this.constant_function_head(cn1);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>MULTI_OP_METRIC_F_EXP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MULTI_OP_METRIC_F_EXP</code> node.
     * @return the binray numeric operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AbstractFunctionExp multi_op_metric_f_exp(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTADD_NARITY_OP_METRIC_F_EXP:
            return this.add_narity_op_metric_f_exp(cn, ref preferences);
          case LexerTreeConstants.JJTSUBSTRACT_NARITY_OP_METRIC_F_EXP:
            return this.substract_narity_op_metric_f_exp(cn, ref preferences);
          case LexerTreeConstants.JJTMULTIPLY_NARITY_OP_METRIC_F_EXP:
            return this.multiply_narity_op_metric_f_exp(cn, ref preferences);
          case LexerTreeConstants.JJTDIVIDE_NARITY_OP_METRIC_F_EXP:
            return this.divide_narity_op_metric_f_exp(cn, ref preferences);
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ADD_NARITY_OP_METRIC_F_EXP</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ADD_NARITY_OP_METRIC_F_EXP</code> node.
     * @return the multi divide function expression build
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityAdd add_narity_op_metric_f_exp(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTMETRIC_F_EXP:
              arguments.Add(this.metric_f_exp(cn, ref preferences));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + node.getLabel() + " unexpected.");
          }
        }
        return new NArityAdd(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SUBSTRACT_NARITY_OP_METRIC_F_EXP</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SUBSTRACT_NARITY_OP_METRIC_F_EXP</code> node.
     * @return the multi divide function expression build
     * @throws ParserException if an error occurs while parsing.
     */
    private NAritySubstract substract_narity_op_metric_f_exp(SimpleNode node, ref List<string> preferences)
    {
      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTMETRIC_F_EXP:
              arguments.Add(this.metric_f_exp(cn, ref preferences));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + node.getLabel() + " unexpected.");
          }
        }
        return new NAritySubstract(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>MULTIPLY_NARITY_OP_METRIC_F_EXP</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MULTIPLY_NARITY_OP_METRIC_F_EXP</code> node.
     * @return the multi divide function expression build
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityMultiply multiply_narity_op_metric_f_exp(SimpleNode node, ref List<string> preferences)
    {
      List<INumericExp> args = new List<INumericExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTMETRIC_F_EXP:
            args.Add(this.metric_f_exp(cn, ref preferences));
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new NArityMultiply(args);
    }

    /**
     * Extracts the object structures from the <code>DIVIDE_NARITY_OP_METRIC_F_EXP</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DIVIDE_NARITY_OP_METRIC_F_EXP</code> node.
     * @return the multi divide function expression build
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityDivide divide_narity_op_metric_f_exp(SimpleNode node, ref List<string> preferences)
    {
      List<INumericExp> args = new List<INumericExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTMETRIC_F_EXP:
            args.Add(this.metric_f_exp(cn, ref preferences));
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new NArityDivide(args);
    }

    /**
     * Extracts the object structures from the <code>INIT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INIT</code> node.
     * @return the initial state structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp goal(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTPRE_GD)
        {
          // Preferences don't need further processing in goals
          List<IConditionPrefExp> preferences = new List<IConditionPrefExp>();

          // If the goal contains only preferences, then it always evaluates to true.
          m_obj.Goal = this.pre_gd(cn, ref preferences) ?? TrueExp.True;

          foreach (IConditionPrefExp pref in preferences)
            pref.IsGoalPreference = true;

          foreach (IConditionPrefExp pref in preferences)
            m_obj.AddPreference(pref);

          return m_obj.Goal;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>INIT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INIT</code> node.
     * @return the initial state structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private List<IInitEl> init(SimpleNode node)
    {
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTINIT_EL:
            m_obj.InitialWorld.Add(this.init_el(cn));
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return m_obj.InitialWorld;
    }

    /**
     * Extracts the object structures from the <code>INIT_EL</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INIT_EL</code> node.
     * @return initial state element.
     * @throws ParserException if an error occurs while parsing.
     */
    private IInitEl init_el(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTLITERAL:
            return this.ground_literal(cn);
          case LexerTreeConstants.JJTEQUAL_INIT_EL:
            return this.equal_init_el(cn);
          case LexerTreeConstants.JJTTIMED_LITERAL:
            return this.timed_literal(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }


    /**
     * Extracts the object structures from the <code>INIT_EL</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INIT_EL</code> node.
     * @return initial state element.
     * @throws ParserException if an error occurs while parsing.
     */
    private TimedLiteral timed_literal(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTNUMBER
                    && cn2.getId() == LexerTreeConstants.JJTLITERAL)
        {
          Number number = this.number(cn1);
          if (number.Value < 0)
          {
            this.m_mgr.logParserError("time stamp of timed literal must be positive",
                        m_file, cn1.getLine(), cn1.getColumn());
          }
          return new TimedLiteral(number.Value, this.ground_literal(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>INIT_EL</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INIT_EL</code> node.
     * @return initial state element.
     * @throws ParserException if an error occurs while parsing.
     */
    private IInitEl equal_init_el(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTCONSTANT_FUNCTION_HEAD_OR_FUNCTOR
                    && cn2.getId() == LexerTreeConstants.JJTNUMBER_OR_CONSTANT)
        {
          DummyFormulaApplication dummyHead = constant_function_head_or_functor(cn1);
          IExp exp = number_or_constant(cn2);
          if (exp is Number)
          {
            Number body = (Number)exp;
            if (!this.getFileRequirements().Contains(RequireKey.NUMERIC_FLUENTS))
            {
              this.m_mgr.logParserError("Numeric fluents assignments cannot be defined without require key \""
                            + RequireKey.NUMERIC_FLUENTS + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            NumericFluentApplication head = getFormulaApplication<NumericFluentApplication>(dummyHead, false, cn1);
            if (head == null)
              head = new NumericFluentApplication(new NumericFluent(dummyHead.Name, 
                                                                    new List<ObjectParameterVariable>(), 
                                                                    DescribedFormula.DefaultAttributes),
                                                    dummyHead.GetArguments());
            return new NumericAssign(head, (Number)body);
          }
          else if (exp is Constant)
          {
            if (!this.getFileRequirements().Contains(RequireKey.OBJECT_FLUENTS))
            {
              this.m_mgr.logParserError("Object fluents assignments cannot be defined without require key \""
                            + RequireKey.OBJECT_FLUENTS + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            Constant body = (Constant)exp;
            ObjectFluentApplication head = getFormulaApplication<ObjectFluentApplication>(dummyHead, false, cn1);
            if (head == null)
              head = new ObjectFluentApplication(new ObjectFluent(dummyHead.Name, 
                                                                  new List<ObjectParameterVariable>(), 
                                                                  getDomain().TypeSetSet.Object,
                                                                  DescribedFormula.DefaultAttributes),
                                                    dummyHead.GetArguments());
            if (!head.CanBeAssignedFrom(body))
            {
              this.m_mgr.logParserError("Function \""
                          + head.ToTypedString()
                          + "\" cannot be assigned constant \""
                          + body.ToTypedString() + "\", since their types are not compatible.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return new ObjectAssign(head, body);
          }
          else
          {
            throw new ParserException("An internal parser error occurs: node "
                        + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private IExp number_or_constant(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            return number(cn);
          case LexerTreeConstants.JJTCONSTANT:
            return constant(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>OBJECT_DECLARATION</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OBJECT_DECLARATION</code> node.
     * @return the object declartion structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, Constant> object_declaration(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTTYPED_LIST)
        {
          this.constant_typed_list(cn, m_obj.Constants);
          return m_obj.Constants;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                          + node.getLabel() + " unexpected.");
    }


    /**
     * Extracts the object structures from a <code>DOMAIN</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DOMAIN</code> node.
     * @return the domain structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private PDDLObject domain(SimpleNode node)
    {
      this.m_obj = new PDDLObject();
      this.m_obj.Content = PDDLObject.PDDLContent.DOMAIN;
      this.m_obj.DomainFile = this.m_file;
      this.m_obj.TypeSetSet = new TypeSetSet(); // TypeSetSet for untyped domains

      List<int> childrenOrder = new List<int>(Enumerable.Repeat(-1, node.jjtGetNumChildren()));
      int currentIndex = childrenOrder.Count - 1;
      for (int i = node.jjtGetNumChildren() - 1; i >= 0; i--)
      {
        if (((SimpleNode)node.jjtGetChild(i)).getId() == LexerTreeConstants.JJTCONSTRAINTS)
        {
          childrenOrder[currentIndex--] = i;
        }
      }
      for (int i = node.jjtGetNumChildren() - 1; i >= 0; i--)
      {
        if (!childrenOrder.Contains(i))
          childrenOrder[currentIndex--] = i;
      }

      for (int i = 0; i < childrenOrder.Count; i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(childrenOrder[i]);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTDOMAIN_NAME:
            m_obj.DomainName = cn.GetImage();
            //string name = new FileInfo(file).Name;
            //if (!cn.GetImage().Equals(name.Substring(0, name.LastIndexOf("."))))
            //{
            //  this.mgr.logParserWarning("Domain \"" + cn.GetImage() + "\" should be defined in a file \""
            //              + cn.GetImage() + ".pddl\".", this.file, cn.getLine(),
            //              cn.getColumn());
            //}
            break;
          case LexerTreeConstants.JJTREQUIRE_DEF:
            this.requireDef(cn);
            break;
          case LexerTreeConstants.JJTTYPE_DEF:
            this.types_def(cn);
            break;
          case LexerTreeConstants.JJTCONSTANT_DEF:
            this.constants_def(cn);
            break;
          case LexerTreeConstants.JJTPREDICATE_DEF:
            this.predicates_def(cn);
            break;
          case LexerTreeConstants.JJTFUNCTION_DEF:
            this.functions_def(cn);
            break;
          case LexerTreeConstants.JJTCONSTRAINTS:
            this.constraints(cn);
            break;
          case LexerTreeConstants.JJTSTRUCTURE_DEF:
            this.structure_def(cn);
            break;
          case LexerTreeConstants.JJTDECLARE_DEFINED_SYMBOLS:
            this.declare_defined_symbols(cn);
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      foreach (DefinedFormula definedFormula in this.getDomain().Formulas.Values.OfType<DefinedFormula>())
      {
        if (definedFormula.Body == null)
        {
          this.m_mgr.logParserError("Defined formula \"" + definedFormula.ToTypedString() + "\"'s body has not been defined.",
            this.m_file, node.getLine(), node.getColumn());
        }
      }
      return m_obj;
    }

    /**
     * Extracts the object structures from the <code>STRUCTURE_DEF</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>STRUCTURE_DEF</code> node.
     * @throws ParserException if an error occurs while parsing.
     */
    private object structure_def(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTACTION_DEF:
            return this.action_def(cn);
          case LexerTreeConstants.JJTDURATION_ACTION_DEF:
            if (!this.getFileRequirements().Contains(RequireKey.DURATIVE_ACTIONS))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.DURATIVE_ACTIONS
                          + "\" needed to specify durative actions.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return this.durative_action_def(cn);
          case LexerTreeConstants.JJTDEFINED_FORMULA:
            return this.defined_formula(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                + node.getLabel() + " unexpected.");
    }

    private void declare_defined_symbols(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to declare defined symbols.", this.m_file, 
                    node.getLine(), node.getColumn());
      }

      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTDEFINED_FORMULA_DECLARATION:
            this.defined_formula_declaration(cn);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
    }

    /**
     * Extracts the object structures from the <code>DURATIVE_ACTION</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DURATIVE_ACTION</code> node.
     * @return the durative actionCtx structure built. 
     * @throws ParserException if an error occurs while parsing.
     */
    private DurativeAction durative_action_def(SimpleNode node)
    {
      SimpleNode cnName;
      SimpleNode cnArgs;
      SimpleNode cnBody;
      double priority;

      if (node.jjtGetNumChildren() == 3)
      {
        priority = 0;
        cnName = (SimpleNode)node.jjtGetChild(0);
        cnArgs = (SimpleNode)node.jjtGetChild(1);
        cnBody = (SimpleNode)node.jjtGetChild(2);
      }
      else if (node.jjtGetNumChildren() == 4)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(1);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            priority = this.number(cn).Value;
            if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                          + "\" needed to specify action priority.", this.m_file, 
                          node.getLine(), node.getColumn());
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
        cnName = (SimpleNode)node.jjtGetChild(0);
        cnArgs = (SimpleNode)node.jjtGetChild(2);
        cnBody = (SimpleNode)node.jjtGetChild(3);
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
            + node.getLabel() + " unexpected.");
      }

      if (cnName.getId() == LexerTreeConstants.JJTACTION_NAME
          && cnArgs.getId() == LexerTreeConstants.JJTTYPED_LIST
          && cnBody.getId() == LexerTreeConstants.JJTDA_DEF_BODY)
      {
        Dictionary<string, ObjectParameterVariable> parameters = new Dictionary<string, ObjectParameterVariable>();
        this.var_typed_list(cnArgs, parameters, false);

        DurativeAction action = new DurativeAction(this.action_name(cnName), priority, new List<ObjectParameterVariable>(parameters.Values));

        this.m_contextStack.pushContext(action.GetParameters().Cast<IVariable>(), cnArgs);
        this.da_def_body(cnBody, action);
        this.m_contextStack.popContext();

        if (this.getDomain().Actions.ContainsKey(action.Name))
        {
          this.m_mgr.logParserError("Duplicated action \"" + action.Name
                      + " \".", this.m_file, node.getLine(), node.getColumn());
        }
        else
        {
          this.getDomain().Actions[action.Name] = action;
        }

        this.getDomain().ContainsDurativeActions = true;
        return action;
      }

      throw new ParserException("An internal parser error occurs: node "
                          + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_DEF_BODY</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_DEF_BODY</code> node.
     * @param action the duratiive actionCtx to initialize.
     * @throws ParserException if an error occurs while parsing.
     */
    private void da_def_body(SimpleNode node, DurativeAction action)
    {
      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);
        if (cn1.getId() == LexerTreeConstants.JJTDURATION_CONSTRAINT
            && cn2.getId() == LexerTreeConstants.JJTDA_GD
            && cn3.getId() == LexerTreeConstants.JJTDA_EFFECT)
        {
          IDictionary<DurationTime, List<ILogicalExp>> durationConstraint = this.duration_constraint(cn1);

          if (durationConstraint[DurationTime.END].Count != 0 ||
              durationConstraint[DurationTime.START].Count != 0 ||
              durationConstraint[DurationTime.NONE].Count != 1)
          {
            this.m_mgr.logParserError("Duration inequalities are currently not supported.",
                                    this.m_file, cn1.getLine(), cn1.getColumn());
          }
          else
          {
            action.Duration = durationConstraint[DurationTime.NONE][0];
          }

          IDictionary<ConditionTime, List<IConditionPrefExp>> preferences;
          IDictionary<ConditionTime, List<ILogicalExp>> conditions = da_gd(cn2, out preferences);
          IDictionary<ConditionTime, List<IEffect>> effects = da_effect(cn3);

          // Transform "over all" preferences in the right form.
          List<IConditionPrefExp> overPrefs = new List<IConditionPrefExp>();
          foreach (IConditionPrefExp pref in preferences[ConditionTime.OVER])
          {
            // Wrap the preference.
            overPrefs.Add(new OverAllConditionPrefExp(pref, action));

            // Create a new counter for this preference.
            m_obj.CreatePreferenceCounter(pref.Name);
          }

          preferences[ConditionTime.OVER] = overPrefs;

          // Add all preferences to the known preferences list
          foreach (List<IConditionPrefExp> lst in preferences.Values)
            foreach (IConditionPrefExp pref in lst)
              m_obj.AddPreference(pref);

          // Add the effects which increment the preference counters
          effects[ConditionTime.START].AddRange(preferences[ConditionTime.START].Select(pref => pref.ConvertToEffect(m_obj.CreatePreferenceCounter(pref.Name))));
          effects[ConditionTime.END].AddRange(preferences[ConditionTime.END].Select(pref => pref.ConvertToEffect(m_obj.CreatePreferenceCounter(pref.Name))));

          action.StartCondition = ExtractCondition(conditions, ConditionTime.START);
          action.OverallCondition = ExtractCondition(conditions, ConditionTime.OVER);
          action.EndCondition = ExtractCondition(conditions, ConditionTime.END);

          action.StartEffect = ExtractEffect(effects, ConditionTime.START);
          action.EndEffect = ExtractEffect(effects, ConditionTime.END);
          action.ContinuousEffect = ExtractEffect(effects, ConditionTime.CONTINUOUS);
          action.OverallEffect = ExtractEffect(effects, ConditionTime.OVER); // Used for preferences and when "over all" conditions
        }
        else
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    private DefinedFormula defined_formula(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTDERIVED_DEF:
            return this.derived_def(cn);
          case LexerTreeConstants.JJTDEFINED_PREDICATE:
            return this.defined_predicate(cn);
          case LexerTreeConstants.JJTDEFINED_FUNCTION:
            return this.defined_function(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                              + node.getLabel() + " unexpected.");
    }

    private DefinedFormula defined_formula_declaration(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTDERIVED_DEF_DECLARATION:
            return this.derived_def_declaration(cn);
          case LexerTreeConstants.JJTDEFINED_PREDICATE_DECLARATION:
            return this.defined_predicate_declaration(cn);
          case LexerTreeConstants.JJTDEFINED_FUNCTION_DECLARATION:
            return this.defined_function_declaration(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                              + node.getLabel() + " unexpected.");
    }


    //private IEffect AddDurativeActionEffect(IEffect originalEffect, IEnumerable<IEffect> addedEffects)
    //{
    //  IEnumerable<IEffect> originalEffects = originalEffect.Once();

    //  if (originalEffect is AndEffect)
    //  {
    //    originalEffects = ((AndEffect)originalEffect).Cast<IEffect>();
    //  }

    //  return new AndEffect(addedEffects.Concat(originalEffects));
    //}

    private ILogicalExp ExtractCondition(IDictionary<ConditionTime, List<ILogicalExp>> conditions, ConditionTime time)
    {
      List<ILogicalExp> lst = conditions[time];

      if (lst.Count == 0)
        return TrueExp.True;
      else if (lst.Count == 1)
        return lst.First();

      return new AndExp(lst);
    }

    private IEffect ExtractEffect(IDictionary<ConditionTime, List<IEffect>> effects, ConditionTime time)
    {
      List<IEffect> lst = effects[time];

      if (lst.Count == 0)
        return new AndEffect(); // No effect
      else if (lst.Count == 1)
        return lst.First();

      return new AndEffect(lst);
    }

    private IDictionary<ConditionTime, List<IEffect>> da_effect(SimpleNode node)
    {
      IDictionary<ConditionTime, List<IEffect>> effects = CreateDictionaryOfEnumAndList<ConditionTime, IEffect>();

      da_effect(node, ref effects);

      return effects;
    }

    /**
     * Extracts the object structures from the <code>DA_EFFECT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_EFFECT</code> node.
     * @return the durative actionCtx effect  expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void da_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAND_DA_EFFECT:
            this.and_da_effect(cn, ref effects);
            return;
          case LexerTreeConstants.JJTTIMED_EFFECT:
            this.timed_effect(cn, ref effects);
            return;
          case LexerTreeConstants.JJTFORALL_DA_EFFECT:
            if (!this.getFileRequirements().Contains(RequireKey.CONDITIONAL_EFFECTS))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.CONDITIONAL_EFFECTS
                          + "\" needed to specify conditional durative effect.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            this.forall_da_effect(cn, ref effects);
            return;
          case LexerTreeConstants.JJTWHEN_DA_EFFECT:
            if (!this.getFileRequirements().Contains(RequireKey.CONDITIONAL_EFFECTS))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.CONDITIONAL_EFFECTS
                          + "\" needed to specify conditional durative effect.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            this.when_da_effect(cn, ref effects);
            return;
          case LexerTreeConstants.JJTASSIGN_OP:
            if (!this.getFileRequirements().Contains(RequireKey.NUMERIC_FLUENTS))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.NUMERIC_FLUENTS
                          + "\" needed to specify assign operation in durative action effect.",
                          this.m_file, node.getLine(), node.getColumn());
            }
//#warning I don't know if this is correct, as the BNF is not quite clear about this...
            this.m_mgr.logParserWarning("An effect of a durative action has no time specifier! "
                                    + "(hence it is considered as an end effect)",
                                      this.m_file, node.getLine(), node.getColumn());
            effects[ConditionTime.END].Add(this.assign_op(cn));
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>WHEN_DA_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>WHEN_DA_EFFECT</code> node.
     * @return the when duratve actionCtx effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void when_da_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTDA_GD
                    && cn2.getId() == LexerTreeConstants.JJTTIMED_EFFECT)
        {
          IDictionary<ConditionTime, List<IConditionPrefExp>> preferences;
          IDictionary<ConditionTime, List<ILogicalExp>> conditions = da_gd(cn1, out preferences);

          // According to Alfonso Gerevini and Derek Long in "Plan Constraints and Preferences in PDDL3",
          // it is considered "a syntax violation if a preference appears in the condition of a conditional
          // effect" [p.5].
          if (preferences.Values.Any(lst => lst.Count != 0))
          {
            this.m_mgr.logParserError("Preferences are not allowed in the condition of a conditional effect.",
                                    this.m_file, node.getLine(), node.getColumn());
          }

          IDictionary<ConditionTime, List<IEffect>> conditionalEffects = CreateDictionaryOfEnumAndList<ConditionTime, IEffect>();
          this.timed_effect(cn2, ref conditionalEffects);

          // See section 8.1 of "pddl2.1 : An Extension to pddl for Expressing Temporal Planning Domains" 
          // by Maria Fox and Derek Long for an explanation of the following.
          if (conditionalEffects[ConditionTime.START].Count != 0 &&
              (conditions[ConditionTime.OVER].Count != 0 || conditions[ConditionTime.END].Count != 0))
          {
            this.m_mgr.logParserError("Conditional effects cannot occur before their conditions are asserted; " +
                                    "this is the reverse of the expected behaviour of causality.",
                                    this.m_file, node.getLine(), node.getColumn());
          }
          else
          {
            List<ObjectParameterVariable> args = new List<ObjectParameterVariable>(this.m_contextStack.getAllQuantifiedVariables<ObjectParameterVariable>());

            List<KeyValuePair<AtomicFormulaApplication, bool>> startAtoms = new List<KeyValuePair<AtomicFormulaApplication, bool>>();
            List<KeyValuePair<AtomicFormulaApplication, bool>> overallAtoms = new List<KeyValuePair<AtomicFormulaApplication, bool>>();

            if (conditions[ConditionTime.START].Count != 0)
            {
              if (conditionalEffects[ConditionTime.END].Count != 0)
              {
                // Create unique formulas to be used only in the action context.
                foreach (ILogicalExp cond in conditions[ConditionTime.START])
                  startAtoms.Add(new KeyValuePair<AtomicFormulaApplication, bool>(m_obj.CreateDummyFormula("when-start", args, false, false), true));

                // Ensure that the end effect can assert that initial conditions were met.
                effects[ConditionTime.START].Add(new DurativeWhenEffect(ExtractCondition(conditions, ConditionTime.START),
                                                                        Enumerable.Empty<KeyValuePair<AtomicFormulaApplication, bool>>(), // No contextual conditions
                                                                        ExtractEffect(conditionalEffects, ConditionTime.START),
                                                                        startAtoms.Keys()));
              }
              else
              {
                // There are only start conditions and start effects; add a simple WhenEffect at start.
                effects[ConditionTime.START].Add(new WhenEffect(ExtractCondition(conditions, ConditionTime.START),
                                                                ExtractEffect(conditionalEffects, ConditionTime.START)));
              }
            }

            if (conditions[ConditionTime.OVER].Count != 0)
            {
              // Create unique formulas to be used only in the action context.
              foreach (ILogicalExp cond in conditions[ConditionTime.OVER])
                overallAtoms.Add(new KeyValuePair<AtomicFormulaApplication, bool>(m_obj.CreateDummyFormula("when-overall", args, false, false), true));

              // Ensure that the end effect can assert that overall conditions were met.
              effects[ConditionTime.OVER].Add(new DurativeWhenEffect(ExtractCondition(conditions, ConditionTime.OVER),
                // TODO: Add start conditions here?
                                                                     overallAtoms.Select(pair => new KeyValuePair<AtomicFormulaApplication, bool>(pair.Key, false)),
                                                                     null, // No effect in the world
                                                                     overallAtoms.Keys()));
            }

            if (conditionalEffects[ConditionTime.END].Count != 0)
            {
              IEnumerable<KeyValuePair<AtomicFormulaApplication, bool>> allAtoms = startAtoms.Concat(overallAtoms);

              if (!allAtoms.IsEmpty())
              {
                // The durative when must check contextual facts
                effects[ConditionTime.END].Add(new DurativeWhenEffect(ExtractCondition(conditions, ConditionTime.END),
                                                                      allAtoms,
                                                                      ExtractEffect(conditionalEffects, ConditionTime.END),
                                                                      Enumerable.Empty<AtomicFormulaApplication>())); // No contextual modification
              }
              else
              {
                // There are only end conditions and end effects; create add a simple WhenEffect at end.
                effects[ConditionTime.END].Add(new WhenEffect(ExtractCondition(conditions, ConditionTime.END),
                                                              ExtractEffect(conditionalEffects, ConditionTime.END)));
              }
            }
          }

          return;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FORALL_DA_EFFECT</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_DA_EFFECT</code> node.
     * @return the universal durative effect expression structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void forall_da_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTDA_EFFECT)
        {
          Dictionary<string, ObjectParameterVariable> varMap = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, varMap, true);

          HashSet<ObjectParameterVariable> vars = new HashSet<ObjectParameterVariable>(varMap.Values);

          this.m_contextStack.pushQuantifiedContext(vars.Cast<IVariable>(), cn1);

          IDictionary<ConditionTime, List<IEffect>> quantifiedEffects = this.da_effect(cn2);

          foreach (KeyValuePair<ConditionTime, List<IEffect>> pair in quantifiedEffects.Where(p => p.Value.Count != 0))
            effects[pair.Key].Add(new ForallEffect(vars, (pair.Value.Count == 1 ? pair.Value.First() : new AndEffect(pair.Value))));

          this.m_contextStack.popQuantifiedContext();

          return;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>TIMED_EFFECT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TIMED_EFFECT</code> node.
     * @return the timed effect expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void timed_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      if (node.jjtGetNumChildren() > 0)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        switch (cn1.getId())
        {
          case LexerTreeConstants.JJTAT_DA_EFFECT:
            this.at_da_effect(cn1, ref effects);
            return;
          case LexerTreeConstants.JJTAT_F_ASSIGN_DA_EFFECT:
            this.at_f_assign_da_effect(cn1, ref effects);
            return;
          case LexerTreeConstants.JJTASSIGN_OP_T:
            if (!this.getFileRequirements().Contains(RequireKey.CONDITIONAL_EFFECTS))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.CONDITIONAL_EFFECTS
                          + "\" needed to specify conditional durative effect.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            effects[ConditionTime.CONTINUOUS].Add(this.assign_op_t(cn1));
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>ASSIGN_OP_T</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ASSIGN_OP_T</code> node.
     * @return the durative assign operation expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NumericAssignEffect assign_op_t(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTINCREASE_ASSIGN_OP_T:
            return increase_assign_op_t(cn);
          case LexerTreeConstants.JJTDECREASE_ASSIGN_OP_T:
            return decrease_assign_op_t(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>INCREASE_ASSIGN_OP_T</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INCREASE_ASSIGN_OP_T</code> node.
     * @return the durative assign operation expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Increase increase_assign_op_t(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == LexerTreeConstants.JJTF_EXP_T)
        {
          return new Increase(this.numeric_fluent(cn1), this.f_exp_t(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DECREASE_ASSIGN_OP_T</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DECREASE_ASSIGN_OP_T</code> node.
     * @return the durative assign operation expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Decrease decrease_assign_op_t(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == LexerTreeConstants.JJTF_EXP_T)
        {
          return new Decrease(this.numeric_fluent(cn1), this.f_exp_t(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ASSIGN_OP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ASSIGN_OP</code> node.
     * @return the durative assign operation expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NumericAssignEffect assign_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTASSIGN:
            return this.assign(cn);
          case LexerTreeConstants.JJTSCALE_UP:
            return this.scale_up(cn);
          case LexerTreeConstants.JJTSCALE_DOWN:
            return this.scale_down(cn);
          case LexerTreeConstants.JJTINCREASE:
            return this.increase(cn);
          case LexerTreeConstants.JJTDECREASE:
            return this.decrease(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
            + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ASSIGN</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ASSIGN</code> node.
     * @return the propositional effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NumericAssign assign(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == node.GetSuffixId())
        {
          switch (cn2.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              return new NumericAssign(this.numeric_fluent(cn1), this.f_exp(cn2));
            case LexerTreeConstants.JJTF_EXP_DA:
              return new NumericAssign(this.numeric_fluent(cn1), this.f_exp_da(cn2));
            default:
              throw new ParserException("An internal parser error occurs: node "
                          + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SCALE_UP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SCALE_UP</code> node.
     * @return the assign scale up expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ScaleUp scale_up(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == node.GetSuffixId())
        {
          switch (cn2.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              return new ScaleUp(this.numeric_fluent(cn1), this.f_exp(cn2));
            case LexerTreeConstants.JJTF_EXP_DA:
              return new ScaleUp(this.numeric_fluent(cn1), this.f_exp_da(cn2));
            default:
              throw new ParserException("An internal parser error occurs: node "
                          + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SCALE_DOWN</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SCALE_DOWN</code> node.
     * @return the assign scale up expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ScaleDown scale_down(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == node.GetSuffixId())
        {
          switch (cn2.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              return new ScaleDown(this.numeric_fluent(cn1), this.f_exp(cn2));
            case LexerTreeConstants.JJTF_EXP_DA:
              return new ScaleDown(this.numeric_fluent(cn1), this.f_exp_da(cn2));
            default:
              throw new ParserException("An internal parser error occurs: node "
                          + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }


    /**
     * Extracts the object structures from the <code>INCREASE</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INCREASE</code> node.
     * @return the increase expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Increase increase(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == node.GetSuffixId())
        {
          switch (cn2.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              return new Increase(this.numeric_fluent(cn1), this.f_exp(cn2));
            case LexerTreeConstants.JJTF_EXP_DA:
              return new Increase(this.numeric_fluent(cn1), this.f_exp_da(cn2));
            default:
              throw new ParserException("An internal parser error occurs: node "
                          + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DECREASE</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DECREASE</code> node.
     * @return the descrease expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Decrease decrease(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD &&
            cn2.getId() == node.GetSuffixId())
        {
          switch (cn2.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              return new Decrease(this.numeric_fluent(cn1), this.f_exp(cn2));
            case LexerTreeConstants.JJTF_EXP_DA:
              return new Decrease(this.numeric_fluent(cn1), this.f_exp_da(cn2));
            default:
              throw new ParserException("An internal parser error occurs: node "
                          + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>F_EXP_T</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>F_EXP_T</code> node.
     * @return the durative fonction expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp f_exp_t(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        ReservedNumericExp cv = new ContinuousVariable();
        List<INumericExp> arguments = new List<INumericExp>();
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP
         && cn2.getId() == LexerTreeConstants.JJTCONTINOUS_VARIABLE)
        {
          arguments.Add(this.f_exp(cn1));
          arguments.Add(cv);
        }
        else if (cn1.getId() == LexerTreeConstants.JJTCONTINOUS_VARIABLE
               && cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          arguments.Add(cv);
          arguments.Add(this.f_exp(cn2));
        }
        return new NArityMultiply(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_F_ASSIGN_DA_EFFECT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_F_ASSIGN_DA_EFFECT</code> node.
     * @return the at durative actionCtx effect expression that Contains a fonction assign.
     * @throws ParserException if an error occurs while parsing.
     */
    private void at_f_assign_da_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAT_END_F_ASSIGN_DA_EFFECT:
            effects[ConditionTime.END].Add(this.at_end_f_assign_da_effect(cn));
            return;
          case LexerTreeConstants.JJTAT_START_F_ASSIGN_DA_EFFECT:
            effects[ConditionTime.START].Add(this.at_start_f_assign_da_effect(cn));
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>AT_START_F_ASSIGN_DA_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_START_F_ASSIGN_DA_EFFECT</code> node.
     * @return the at start durative actionCtx effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect at_start_f_assign_da_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTASSIGN_OP:
            return this.assign_op(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                         + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_END_F_ASSIGN_DA_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_END_F_ASSIGN_DA_EFFECT</code> node.
     * @return the at start durative actionCtx effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect at_end_f_assign_da_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTASSIGN_OP:
            return this.assign_op(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                         + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_SCALE_UP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_SCALE_UP</code> node.
     * @return the durative actionCtx assign scale up expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ScaleUp da_scale_up(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          return new ScaleUp(this.numeric_fluent(cn1), this.f_exp_da(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_DECREASE</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_DECREASE</code> node.
     * @return the durative actionCtx descrease expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Decrease da_decrease(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP_DA)
        {
          return new Decrease(this.numeric_fluent(cn1), this.f_exp_da(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_INCREASE</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_INCREASE</code> node.
     * @return the durative actionCtx increase expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Increase da_increase(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP_DA)
        {
          return new Increase(this.numeric_fluent(cn1), this.f_exp_da(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_SCALE_DOWN</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_SCALE_DOWN</code> node.
     * @return the surative actionCtx assign scale up expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ScaleDown da_scale_down(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP_DA)
        {
          return new ScaleDown(this.numeric_fluent(cn1), this.f_exp_da(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_ASSIGN</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_ASSIGN</code> node.
     * @return the durative assign effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NumericAssign da_assign(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_HEAD
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP_DA)
        {
          return new NumericAssign(this.numeric_fluent(cn1), this.f_exp_da(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>F_EXP_DA</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>F_EXP_DA</code> node.
     * @return the durative fonction expression structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp f_exp_da(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return this.f_exp(cn);
          case LexerTreeConstants.JJTDA_OP:
            return this.da_op(cn);
          case LexerTreeConstants.JJTVAR_DURATION:
            if (!this.getFileRequirements().Contains(RequireKey.DURATION_INEQUALITIES))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.DURATION_INEQUALITIES
                          + "\" needed to specify durative inequalities in actionCtx effect.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return new DurationVariable();
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_BINARY_OP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_BINARY_OP</code> node.
     * @return the surative binary numeric operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AbstractFunctionExp da_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTADD_OP:
            return this.da_add_op(cn);
          case LexerTreeConstants.JJTSUBSTRACT_OP:
            return this.da_substract_op(cn);
          case LexerTreeConstants.JJTMULTIPLY_OP:
            return this.da_multiply_op(cn);
          case LexerTreeConstants.JJTDIVIDE_OP:
            return this.da_divide_op(cn);
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_ADD_OPERATION</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_ADD_OPERATION</code> node.
     * @return the add operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityAdd da_add_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP_DA:
              arguments.Add(this.f_exp_da(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + node.getLabel() + " unexpected.");
          }
        }
        return new NArityAdd(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_SUBSTRACT_OP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_SUBSTRACT_OP</code> node.
     * @return the substract operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NAritySubstract da_substract_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP_DA:
              arguments.Add(this.f_exp_da(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + node.getLabel() + " unexpected.");
          }
        }
        return new NAritySubstract(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_MULTIPLY_OP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_MULTIPLY_OP</code> node.
     * @return the durative multiply operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityMultiply da_multiply_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP_DA:
              arguments.Add(this.f_exp_da(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + node.getLabel() + " unexpected.");
          }
        }
        return new NArityMultiply(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DA_DIVIDE_OP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_DIVIDE_OP</code> node.
     * @return the durative divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityDivide da_divide_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP_DA:
              arguments.Add(this.f_exp_da(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + node.getLabel() + " unexpected.");
          }
        }
        return new NArityDivide(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_DA_EFFECT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_DA_EFFECT</code> node.
     * @return the at durative actionCtx effect expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void at_da_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAT_END_DA_EFFECT:
            effects[ConditionTime.END].Add(this.at_end_da_effect(cn));
            return;
          case LexerTreeConstants.JJTAT_START_DA_EFFECT:
            effects[ConditionTime.START].Add(this.at_start_da_effect(cn));
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>AT_END_DA_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_END_DA_EFFECT</code> node.
     * @return the at start durative actionCtx effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect at_end_da_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTEFFECT)
        {
          return this.effect(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_START_DA_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_START_DA_EFFECT</code> node.
     * @return the at start durative actionCtx effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect at_start_da_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTEFFECT)
        {
          return this.effect(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_DA_EFFECT</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_DA_EFFECT</code> node.
     * @return the conjunctive durative actionCtx effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void and_da_effect(SimpleNode node, ref IDictionary<ConditionTime, List<IEffect>> effects)
    {
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTDA_EFFECT:
            this.da_effect(cn, ref effects);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
    }

    private IDictionary<EnumType, List<ListType>> CreateDictionaryOfEnumAndList<EnumType, ListType>()
    {
      IDictionary<EnumType, List<ListType>> dic = new Dictionary<EnumType, List<ListType>>();

      foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
      {
        dic[value] = new List<ListType>();
      }

      return dic;
    }

    private IDictionary<ConditionTime, List<ILogicalExp>> da_gd(SimpleNode node, out IDictionary<ConditionTime, List<IConditionPrefExp>> preferences)
    {
      IDictionary<ConditionTime, List<ILogicalExp>> gds = CreateDictionaryOfEnumAndList<ConditionTime, ILogicalExp>();

      preferences = CreateDictionaryOfEnumAndList<ConditionTime, IConditionPrefExp>();

      da_gd(node, ref gds, ref preferences);

      return gds;
    }

    /**
     * Extracts the object structures from the <code>DA_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DA_GD</code> node.
     * @return the durative actionCtx goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void da_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds, ref IDictionary<ConditionTime, List<IConditionPrefExp>> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTPREF_TIMED_GD:
            this.pref_timed_gd(cn, ref gds, ref preferences);
            return;
          case LexerTreeConstants.JJTAND_DA_GD:
            this.and_da_gd(cn, ref gds, ref preferences);
            return;
          case LexerTreeConstants.JJTFORALL_DA_GD:
            if (!this.getFileRequirements().Contains(RequireKey.UNIVERSAL_PRECONDITIONS)
                        && !this.getFileRequirements().Contains(RequireKey.QUANTIFIED_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Universal formula cannot be defined without require keys \""
                         + RequireKey.UNIVERSAL_PRECONDITIONS
                         + "\" or \""
                         + RequireKey.QUANTIFIED_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            this.forall_da_gd(cn, ref gds, ref preferences);
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>FORALL_DA_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_DA_GD</code> node.
     * @return the universal duartive actionCtx goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void forall_da_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds, ref IDictionary<ConditionTime, List<IConditionPrefExp>> preferences)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
                    && cn2.getId() == LexerTreeConstants.JJTDA_GD)
        {
          Dictionary<string, ObjectParameterVariable> varMap = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, varMap, true);

          HashSet<ObjectParameterVariable> vars = new HashSet<ObjectParameterVariable>(varMap.Values);

          this.m_contextStack.pushQuantifiedContext(varMap.Values.Cast<IVariable>(), cn1);

          IDictionary<ConditionTime, List<ILogicalExp>> quantifiedGDs = CreateDictionaryOfEnumAndList<ConditionTime, ILogicalExp>();
          IDictionary<ConditionTime, List<IConditionPrefExp>> quantifiedPrefs = CreateDictionaryOfEnumAndList<ConditionTime, IConditionPrefExp>();

          this.da_gd(cn2, ref quantifiedGDs, ref quantifiedPrefs);

          // Add a new ForAllExp to the returns GDs (in each time)
          foreach (KeyValuePair<ConditionTime, List<ILogicalExp>> pair in quantifiedGDs.Where(p => p.Value.Count != 0))
          {
            gds[pair.Key].Add(new ForallExp(vars, (pair.Value.Count == 1 ? pair.Value.First()
                                                                         : new AndExp(pair.Value))));
          }

          // Replace and add extracted preferences by ForAll preferences
          foreach (KeyValuePair<ConditionTime, List<IConditionPrefExp>> pair in quantifiedPrefs)
          {
            preferences[pair.Key].AddRange(pair.Value.Select<IConditionPrefExp, IConditionPrefExp>(pref => new ForallConditionPrefExp(vars, pref)));
          }

          this.m_contextStack.popQuantifiedContext();

          return;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_DA_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_DA_GD</code> node.
     * @return the conjunctive durative actionCtx goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void and_da_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds, ref IDictionary<ConditionTime, List<IConditionPrefExp>> preferences)
    {
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTDA_GD:
            this.da_gd(cn, ref gds, ref preferences);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
    }

    /**
     * Extracts the object structures from the <code>PREF_TIMED_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREF_TIMED_GD</code> node.
     * @return the preference timed goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void pref_timed_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds, ref IDictionary<ConditionTime, List<IConditionPrefExp>> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTTIMED_GD:
            this.timed_gd(cn, ref gds);
            return;
          case LexerTreeConstants.JJTNAMED_PREF_TIMED_GD:
            if (!this.getFileRequirements().Contains(RequireKey.PREFERENCES))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.PREFERENCES
                          + "\" needed to specify preferences.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            this.named_pref_timed_gd(cn, ref gds, ref preferences);
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>NAMED_PREF_TIMED_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>NAMED_PREF_TIMED_GD</code> node.
     * @return the name preference timed goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void named_pref_timed_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds, ref IDictionary<ConditionTime, List<IConditionPrefExp>> preferences)
    {
      IDictionary<ConditionTime, List<ILogicalExp>> prefGDs = CreateDictionaryOfEnumAndList<ConditionTime, ILogicalExp>();

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTTIMED_GD)
        {
          this.timed_gd(cn1, ref prefGDs);

          System.Diagnostics.Debug.Assert(prefGDs.Values.Select(lst => lst.AsEnumerable<ILogicalExp>()).Flatten().Count() == 1);
          foreach (KeyValuePair<ConditionTime, List<ILogicalExp>> pair in prefGDs)
          {
            if (pair.Value.Count != 0)
              preferences[pair.Key].Add(m_obj.CreateUnnamedConditionPreference(pair.Value.First()));
          }

          return;
        }
      }
      else if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTPREF_NAME
                    && cn2.getId() == LexerTreeConstants.JJTTIMED_GD)
        {
          string name = this.pref_name(cn1);
          this.timed_gd(cn2, ref prefGDs);

          System.Diagnostics.Debug.Assert(prefGDs.Values.Select(lst => lst.AsEnumerable<ILogicalExp>()).Flatten().Count() == 1);
          foreach (KeyValuePair<ConditionTime, List<ILogicalExp>> pair in prefGDs)
          {
            if (pair.Value.Count != 0)
              preferences[pair.Key].Add(new ConditionPrefExp(name, pair.Value.First()));
          }

          return;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>TIMED_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TIMED_GD</code> node.
     * @return the timed goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void timed_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAT_TIMED_GD:
            this.at_timed_gd(cn, ref gds);
            return;
          case LexerTreeConstants.JJTOVER_TIMED_GD:
            this.over_timed_gd(cn, ref gds);
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>OVER_TIMED_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OVER_TIMED_GD</code> node.
     * @return the over timed goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void over_timed_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTOVER_ALL_TIMED_GD:
            gds[ConditionTime.OVER].Add(this.over_all_timed_gd(cn));
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>OVER_ALL_TIMED_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OVER_ALL_TIMED_GD</code> node.
     * @return the over all timed goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp over_all_timed_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTGD)
        {
          return this.gd(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_TIMED_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_TIMED_GD</code> node.
     * @return the at timed goal description expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void at_timed_gd(SimpleNode node, ref IDictionary<ConditionTime, List<ILogicalExp>> gds)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAT_START_TIMED_GD:
            gds[ConditionTime.START].Add(this.at_start_timed_gd(cn));
            return;
          case LexerTreeConstants.JJTAT_END_TIMED_GD:
            gds[ConditionTime.END].Add(this.at_end_timed_gd(cn));
            return;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>AT_START_TIMED_GD</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_START_TIMED_GD</code> node.
     * @return the at start timed goal description expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp at_start_timed_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTGD)
        {
          return this.gd(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_END_TIMED_GD</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_END_TIMED_GD</code> node.
     * @return the at end timed goal description expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp at_end_timed_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTGD)
        {
          return this.gd(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    private IDictionary<DurationTime, List<ILogicalExp>> duration_constraint(SimpleNode node)
    {
      IDictionary<DurationTime, List<ILogicalExp>> durationConstraints = CreateDictionaryOfEnumAndList<DurationTime, ILogicalExp>();

      duration_constraint(node, ref durationConstraints);

      return durationConstraints;
    }

    /**
     * Extracts the object structures from the <code>DURATION_CONSTRAINT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DURATION_CONSTRAINT</code> node.
     * @return the duration constraint expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void duration_constraint(SimpleNode node, ref IDictionary<DurationTime, List<ILogicalExp>> durationConstraints)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTEMPTY_OR:
            this.empty_or(cn);
            break;
          case LexerTreeConstants.JJTAND_SIMPLE_DURATION_CONSTRAINT:
            if (!this.getFileRequirements().Contains(RequireKey.DURATION_INEQUALITIES))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.DURATION_INEQUALITIES
                          + "\" needed to specify conjunction of durative constraints.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            this.and_simple_duration_constraint(cn, DurationTime.NONE, ref durationConstraints);
            break;
          case LexerTreeConstants.JJTSIMPLE_DURATION_CONSTRAINT:
            this.simple_duration_constraint(cn, DurationTime.NONE, ref durationConstraints);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>AND_SIMPLE_DURATION_CONSTRAINT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_SIMPLE_DURATION_CONSTRAINT</code> node.
     * @return the conjuncive simple duration constraint expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void and_simple_duration_constraint(SimpleNode node, DurationTime time, ref IDictionary<DurationTime, List<ILogicalExp>> durationConstraints)
    {
      if (node.jjtGetNumChildren() >= 1)
      {
        for (int i = 0; i < node.jjtGetNumChildren(); i++)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          if (cn.getId() == LexerTreeConstants.JJTSIMPLE_DURATION_CONSTRAINT)
          {
            this.simple_duration_constraint(cn, time, ref durationConstraints);
          }
          else
            throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SIMPLE_DURATION_CONSTRAINT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SIMPLE_DURATION_CONSTRAINT</code> node.
     * @return the simple duration constraint expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private void simple_duration_constraint(SimpleNode node, DurationTime time, ref IDictionary<DurationTime, List<ILogicalExp>> durationConstraints)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTD_OP:
            durationConstraints[time].Add(this.d_op(cn));
            break;
          case LexerTreeConstants.JJTAT_SIMPLE_DURATION_CONSTRAINT:
            this.at_simple_duration_constraint(cn, time, ref durationConstraints);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>AT_SIMPLE_DURATION_CONSTRAINT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_SIMPLE_DURATION_CONSTRAINT</code> node.
     * @return the at simple duration constraint expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void at_simple_duration_constraint(SimpleNode node, DurationTime time, ref IDictionary<DurationTime, List<ILogicalExp>> durationConstraints)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAT_END_SIMPLE_DURATION_CONSTRAINT:
            this.at_end_simple_duration_constraint(cn, time, ref durationConstraints);
            break;
          case LexerTreeConstants.JJTAT_START_SIMPLE_DURATION_CONSTRAINT:
            this.at_start_simple_duration_constraint(cn, time, ref durationConstraints);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>AT_END_SIMPLE_DURATION_CONSTRAINT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_END_SIMPLE_DURATION_CONSTRAINT</code> node.
     * @return the at end simple durative constraint expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void at_end_simple_duration_constraint(SimpleNode node, DurationTime time, ref IDictionary<DurationTime, List<ILogicalExp>> durationConstraints)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSIMPLE_DURATION_CONSTRAINT)
        {
          if (time != DurationTime.NONE && time != DurationTime.END)
          {
            this.m_mgr.logParserError("Conflicting duration constraint time specifier: cannot mix (at end ...) with (at start ...)",
                                    this.m_file, node.getLine(), node.getColumn());
          }

          this.simple_duration_constraint(cn, DurationTime.END, ref durationConstraints);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_START_SIMPLE_DURATION_CONSTRAINT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_START_SIMPLE_DURATION_CONSTRAINT</code> node.
     * @return the at start simple durative constraint expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void at_start_simple_duration_constraint(SimpleNode node, DurationTime time, ref IDictionary<DurationTime, List<ILogicalExp>> durationConstraints)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSIMPLE_DURATION_CONSTRAINT)
        {
          if (time != DurationTime.NONE && time != DurationTime.START)
          {
            this.m_mgr.logParserError("Conflicting duration constraint time specifier: cannot mix (at start ...) with (at end ...)",
                                    this.m_file, node.getLine(), node.getColumn());
          }

          this.simple_duration_constraint(cn, DurationTime.START, ref durationConstraints);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>D_OP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>D_OP</code> node.
     * @return the durative operation expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp d_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTGEQUAL_D_OP:
            if (!this.getFileRequirements().Contains(RequireKey.DURATION_INEQUALITIES))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.DURATION_INEQUALITIES
                          + "\" needed to specify durative inequalitities.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return this.gequal_d_op(cn);
          case LexerTreeConstants.JJTLEQUAL_D_OP:
            if (!this.getFileRequirements().Contains(RequireKey.DURATION_INEQUALITIES))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.DURATION_INEQUALITIES
                          + "\" needed to specify durative inequalitities.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return this.lequal_d_op(cn);
          case LexerTreeConstants.JJTEQUAL_D_OP:
            return this.equal_d_op(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>EQUAL_D_OP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EQUAL_D_OP</code> node.
     * @return the equal durative operation expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private NumericEqualComp equal_d_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTVAR_DURATION
                    && cn2.getId() == LexerTreeConstants.JJTD_VALUE)
        {
          ReservedNumericExp cv = new DurationVariable();
          return new NumericEqualComp(cv, this.d_value(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LEQUAL_D_OP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LEQUAL_D_OP</code> node.
     * @return the less equal durative operation expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private LessEqualComp lequal_d_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTVAR_DURATION
                    && cn2.getId() == LexerTreeConstants.JJTD_VALUE)
        {
          ReservedNumericExp cv = new DurationVariable();
          return new LessEqualComp(cv, this.d_value(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>GEQUAL_D_OP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>GEQUAL_D_OP</code> node.
     * @return the greater equal durative operation expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private GreaterEqualComp gequal_d_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTVAR_DURATION
                    && cn2.getId() == LexerTreeConstants.JJTD_VALUE)
        {
          ReservedNumericExp cv = new DurationVariable();
          return new GreaterEqualComp(cv, this.d_value(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>GEQUAL_D_OP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>D_OP</code> node.
     * @return the greater equal durative operation expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp d_value(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            return this.number(cn);
          case LexerTreeConstants.JJTF_EXP:
            if (!this.getFileRequirements().Contains(RequireKey.NUMERIC_FLUENTS))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.NUMERIC_FLUENTS
                          + "\" needed to specify fluent expressions.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return this.f_exp(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>EMPTY_OR</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EMPTY_OR</code> node.
     * @return the empty or expression built
     * @throws ParserException if an error occurs while parsing.
     */
    private TrueExp empty_or(SimpleNode node)
    {
      return TrueExp.True;
    }

    private DerivedPredicate derived_def_declaration(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FORMULA_SKELETON:
            AtomicFormula head = this.atomic_formula_skeleton(cn, false);
            DerivedPredicate derived = new DerivedPredicate(head.Name,
                                         new List<ObjectParameterVariable>(head.Parameters));
            RootFormula other;
            if (this.getDomain().Formulas.TryGetValue(derived.Name, out other))
            {
              this.m_mgr.logParserError("Derived predicate \""
                          + derived.ToTypedString()
                          + "\" is already defined as \""
                          + other.ToTypedString() + "\".", this.m_file, node.getLine(),
                          node.getColumn());
            }
            else
            {
              this.getDomain().Formulas.Add(derived.Name, derived);
            }
            return derived;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                     + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DERIVED_DEF</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DERIVED_DEF</code> node.
     * @return the actionCtx structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private DerivedPredicate derived_def(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cnBody = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTATOMIC_FORMULA_SKELETON
         && cnBody.getId() == LexerTreeConstants.JJTGD)
        {
          AtomicFormula head = this.atomic_formula_skeleton(cn1, false);
          DerivedPredicate derived = new DerivedPredicate(head.Name,
                              new List<ObjectParameterVariable>(head.Parameters));
          // The new predicate should be added immediately since it likely self-references itself.

          DerivedPredicate existing = getExistingDefinedFormula<DerivedPredicate>(derived, node);
          if (existing != null)
          {
            existing.CopyFrom(derived);
            derived = existing;
          }
          else
          {
            this.getDomain().Formulas.Add(derived.Name, derived);
          }

          if (derived.Body != null)
          {
            this.m_mgr.logParserError("Derived predicate \""
                                  + derived.ToTypedString()
                                  + "\" already has a body.",
                                  this.m_file, node.getLine(), node.getColumn());
          }
          else
          {
            this.m_contextStack.pushContext(derived.Parameters.Cast<IVariable>(), node);
            ILogicalExp body = this.gd(cnBody);
            derived.Body = body;
            this.m_contextStack.popContext();
          }

          return derived;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private DefinedPredicate defined_predicate_declaration(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FORMULA_SKELETON:
            AtomicFormula head = this.atomic_formula_skeleton(cn, false);
            DefinedPredicate definedPred = new DefinedPredicate(head.Name,
                                             new List<ObjectParameterVariable>(head.Parameters),
                                             new List<ILocalVariable>());
            RootFormula other;
            if (this.getDomain().Formulas.TryGetValue(definedPred.Name, out other))
            {
              this.m_mgr.logParserError("Defined predicate \""
                          + definedPred.ToTypedString()
                          + "\" is already defined as \""
                          + other.ToTypedString() + "\".", this.m_file, node.getLine(),
                          node.getColumn());
            }
            else
            {
              this.getDomain().Formulas.Add(definedPred.Name, definedPred);
            }
            return definedPred;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                     + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DEFINED_PREDICATE</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DEFINED_PREDICATE</code> node.
     * @return the actionCtx structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private DefinedPredicate defined_predicate(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to specify defined predicates.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      DefinedPredicate definedPred = null;
      SimpleNode cnHead = null;
      SimpleNode cnBody = null;
      SimpleNode cnVars = null;
      if (node.jjtGetNumChildren() == 2)
      {
        cnHead = (SimpleNode)node.jjtGetChild(0);
        cnBody = (SimpleNode)node.jjtGetChild(1);
        if (cnHead.getId() != LexerTreeConstants.JJTATOMIC_FORMULA_SKELETON
         || cnBody.getId() != LexerTreeConstants.JJTGD)
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      else if (node.jjtGetNumChildren() == 3)
      {
        cnHead = (SimpleNode)node.jjtGetChild(0);
        cnVars = (SimpleNode)node.jjtGetChild(1);
        cnBody = (SimpleNode)node.jjtGetChild(2);
        if (cnHead.getId() != LexerTreeConstants.JJTATOMIC_FORMULA_SKELETON
         || cnVars.getId() != LexerTreeConstants.JJTLOCAL_VARS
         || cnBody.getId() != LexerTreeConstants.JJTGD)
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                    + node.getLabel() + " unexpected.");
      }

      AtomicFormula head = this.atomic_formula_skeleton(cnHead, false);
      List<ILocalVariable> vars = (cnVars != null) ? this.local_vars(cnVars) :
                                  new List<ILocalVariable>();
      definedPred = new DefinedPredicate(head.Name,
                                         head.Parameters.ToList(),
                                         vars);
      // The new axiom should be added immediately since it likely self-references itself.
      DefinedPredicate existing = getExistingDefinedFormula<DefinedPredicate>(definedPred, node);
      if (existing != null)
      {
        existing.CopyFrom(definedPred);
        definedPred = existing;
      }
      else
      {
        this.getDomain().Formulas.Add(definedPred.Name, definedPred);
      }

      if (definedPred.Body != null)
      {
        this.m_mgr.logParserError("Defined predicate \""
                              + definedPred.ToTypedString()
                              + "\" already has a body.",
                              this.m_file, node.getLine(), node.getColumn());
      }
      else
      {
        this.m_contextStack.pushContext(definedPred.Parameters.Cast<IVariable>().Concat(
                                      definedPred.LocalVariables.Cast<IVariable>()), node);
        ILogicalExp body = this.gd(cnBody);
        definedPred.Body = body;
        this.m_contextStack.popContext();
      }

      return definedPred;
    }

    private DefinedFunction defined_function_declaration(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTTYPED_FUNCTION:
            Fluent head = this.typed_function(cn, false);
            DefinedFunction definedFunction = null;
            if (head is NumericFluent)
            {
              definedFunction = new DefinedNumericFunction(head.Name,
                                                           head.Parameters.ToList(),
                                                           new List<ILocalVariable>());
            }
            else if (head is ObjectFluent)
            {
              definedFunction = new DefinedObjectFunction(head.Name,
                                                          head.Parameters.ToList(),
                                                          ((ObjectFluent)head).GetTypeSet(),
                                                          new List<ILocalVariable>());
            }
            else
            {
              throw new NotSupportedException(head.ToTypedString() + " is not supported.");
            }

            RootFormula other;
            if (this.getDomain().Formulas.TryGetValue(definedFunction.Name, out other))
            {
              this.m_mgr.logParserError("Defined function \""
                          + definedFunction.ToTypedString()
                          + "\" is already defined as \""
                          + other.ToTypedString() + "\".", this.m_file, node.getLine(),
                          node.getColumn());
            }
            else
            {
              this.getDomain().Formulas.Add(definedFunction.Name, definedFunction);
            }
            return definedFunction;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                     + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DEFINED_FUNCTION</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DEFINED_FUNCTION</code> node.
     * @return the actionCtx structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private DefinedFunction defined_function(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to specify defined function.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      DefinedFunction definedFunction = null;
      SimpleNode cnHead = null;
      SimpleNode cnBody = null;
      SimpleNode cnVars = null;
      if (node.jjtGetNumChildren() == 2)
      {
        cnHead = (SimpleNode)node.jjtGetChild(0);
        cnBody = (SimpleNode)node.jjtGetChild(1);
        if (cnHead.getId() != LexerTreeConstants.JJTTYPED_FUNCTION
         || cnBody.getId() != LexerTreeConstants.JJTGD)
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      else if (node.jjtGetNumChildren() == 3)
      {
        cnHead = (SimpleNode)node.jjtGetChild(0);
        cnVars = (SimpleNode)node.jjtGetChild(1);
        cnBody = (SimpleNode)node.jjtGetChild(2);
        if (cnHead.getId() != LexerTreeConstants.JJTTYPED_FUNCTION
         || cnVars.getId() != LexerTreeConstants.JJTLOCAL_VARS
         || cnBody.getId() != LexerTreeConstants.JJTGD)
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                    + node.getLabel() + " unexpected.");
      }

      Fluent head = this.typed_function(cnHead, false);
      List<ILocalVariable> vars = (cnVars != null) ? this.local_vars(cnVars) :
                                  new List<ILocalVariable>();
      if (head is NumericFluent)
      {
        definedFunction = new DefinedNumericFunction(head.Name,
                                                     head.Parameters.ToList(),
                                                     vars);
      }
      else if (head is ObjectFluent)
      {
        definedFunction = new DefinedObjectFunction(head.Name,
                                                    head.Parameters.ToList(),
                                                    ((ObjectFluent)head).GetTypeSet(),
                                                    vars);
      }
      else
      {
        throw new NotSupportedException(head.ToTypedString() + " is not supported.");
      }

      // The new axiom should be added immediately since it likely self-references itself.
      DefinedFunction existing = getExistingDefinedFormula<DefinedFunction>(definedFunction, node);
      if (existing != null)
      {
        existing.CopyFrom(definedFunction);
        definedFunction = existing;
      }
      else
      {
        this.getDomain().Formulas.Add(definedFunction.Name, definedFunction);
      }

      if (definedFunction.Body != null)
      {
        this.m_mgr.logParserError("Defined function \""
                              + definedFunction.ToTypedString()
                              + "\" already has a body.",
                              this.m_file, node.getLine(), node.getColumn());
      }
      else
      {
        this.m_contextStack.pushContext(definedFunction.Parameters.Cast<IVariable>().Concat(
                                      definedFunction.LocalVariables.Cast<IVariable>()).Concat(
                                      Enumerable.Repeat((IVariable)definedFunction.FunctionVariable, 1)),
                                      node);
        ILogicalExp body = this.gd(cnBody);
        definedFunction.Body = body;
        this.m_contextStack.popContext();
      }

      return definedFunction;
    }

    /**
     * Extracts the object structures from the <code>LOCAL_VARS</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LOCAL_VARS</code> node.
     * @return the actionCtx structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private List<ILocalVariable> local_vars(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use local variables.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTION_TYPED_LIST:
            IDictionary<string, ILocalVariable> vars = this.local_vars_typed_list(cn);
            return vars.Values.ToList();
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>TYPED_LIST</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPED_LIST</code> node.
     * @param tl the typed list of constants already built.
     * @return the typed list structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, ILocalVariable> local_vars_typed_list(SimpleNode node)
    {
      IDictionary<string, ILocalVariable> all = new Dictionary<string, ILocalVariable>();
      IDictionary<string, ILocalVariable> current = new Dictionary<string, ILocalVariable>();

      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTVARIABLE:
            BooleanLocalVariable var = def_local_variable(cn);
            if (all.ContainsKey(var.Name) || current.ContainsKey(var.Name))
            {
              this.m_mgr.logParserError("Variable \""
                          + var.ToTypedString()
                          + "\" duplicated.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            else
            {
              current.Add(var.Name, var);
            }
            break;
          case LexerTreeConstants.JJTFUNCTION_TYPE:
            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify typed variables.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            TypeSet typeSet = this.function_type(cn);
            if (typeSet == this.getDomain().TypeSetSet.Number)
            {
              foreach (ILocalVariable variable in current.Values)
              {
                NumericLocalVariable newVariable = new NumericLocalVariable(variable.Name);
                all.Add(newVariable.Name, newVariable);
              }
            }
            else
            {
              foreach (ILocalVariable variable in current.Values)
              {
                if (!this.getFileRequirements().Contains(RequireKey.OBJECT_FLUENTS))
                {
                  this.m_mgr.logParserError("Object fluents cannot be defined without require key \""
                                + RequireKey.OBJECT_FLUENTS + "\".", this.m_file, node.getLine(), node.getColumn());
                }
                all.Add(variable.Name, new ObjectLocalVariable(variable.Name, typeSet));
              }
            }
            current.Clear();
            break;
          case LexerTreeConstants.JJTFUNCTION_TYPED_LIST:
            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify typed variables.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            IDictionary<string, ILocalVariable> other = this.local_vars_typed_list(cn);
            foreach (ILocalVariable variable in other.Values)
            {
              if (all.ContainsKey(variable.Name))
              {
                this.m_mgr.logParserError("Variable \""
                            + variable.ToTypedString()
                            + "\" duplicated.", this.m_file,
                            node.getLine(), node.getColumn());
              }
              else
              {
                all.Add(variable.Name, variable);
              }
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      foreach (ILocalVariable variable in current.Values)
      {
        all.Add(variable.Name, variable);
        if (this.getFileRequirements().Contains(RequireKey.TYPING))
        {
          this.m_mgr.logParserWarning("Variable \"" + variable.ToString() +
                                    "\" was not specified a type, and TYPING is required; default type BOOLEAN is assumed.",
                                    this.m_file, node.getLine(), node.getColumn());
        }
      }

      return all;
    }

    /**
     * Extracts the object structures from the <code>ACTION_DEF</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ACTION_DEF</code> node.
     * @return the actionCtx structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private PDDLParser.Action.Action action_def(SimpleNode node)
    {
      SimpleNode cnName;
      SimpleNode cnArgs;
      SimpleNode cnBody;
      double priority;

      if (node.jjtGetNumChildren() == 3)
      {
        priority = 0;
        cnName = (SimpleNode)node.jjtGetChild(0);
        cnArgs = (SimpleNode)node.jjtGetChild(1);
        cnBody = (SimpleNode)node.jjtGetChild(2);
      }
      else if (node.jjtGetNumChildren() == 4)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(1);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            priority = this.number(cn).Value;
            if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                          + "\" needed to specify action priority.", this.m_file, 
                          node.getLine(), node.getColumn());
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
        cnName = (SimpleNode)node.jjtGetChild(0);
        cnArgs = (SimpleNode)node.jjtGetChild(2);
        cnBody = (SimpleNode)node.jjtGetChild(3);
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
            + node.getLabel() + " unexpected.");
      }

      if (cnName.getId() == LexerTreeConstants.JJTACTION_NAME
          && cnArgs.getId() == LexerTreeConstants.JJTTYPED_LIST
          && cnBody.getId() == LexerTreeConstants.JJTACTION_DEF_BODY)
      {
        Dictionary<string, ObjectParameterVariable> parameters = new Dictionary<string, ObjectParameterVariable>();
        this.var_typed_list(cnArgs, parameters, false);

        PDDLParser.Action.Action action = new PDDLParser.Action.Action(this.action_name(cnName), priority, new List<ObjectParameterVariable>(parameters.Values));

        this.m_contextStack.pushContext(action.GetParameters().Cast<IVariable>(), cnArgs);
        this.action_def_body(cnBody, action);
        this.m_contextStack.popContext();

        if (this.getDomain().Actions.ContainsKey(action.Name))
        {
          this.m_mgr.logParserError("Duplicated action \"" + action.Name
                      + " \".", this.m_file, node.getLine(), node.getColumn());
        }
        else
        {
          this.getDomain().Actions[action.Name] = action;
        }
        return action;
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                                  + node.getLabel() + " unexpected.");
      }
    }


    /**
     * Extracts the object structures from the <code>ACTION_DEF_BODY</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ACTION_DEF_BODY</code> node.
     * @param action the actionCtx to initialize.
     * @throws ParserException if an error occurs while parsing.
     */
    private void action_def_body(SimpleNode node, PDDLParser.Action.Action action)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTPRE_GD
            && cn2.getId() == LexerTreeConstants.JJTEFFECT)
        {
          List<IConditionPrefExp> preferences = new List<IConditionPrefExp>();
          action.Precondition = this.pre_gd(cn1, ref preferences) ?? TrueExp.True;

          IEffect effect = this.effect(cn2);

          foreach (IConditionPrefExp pref in preferences)
            m_obj.AddPreference(pref);

          if (preferences.Count != 0)
            effect = new AndEffect(preferences.Select(pref => pref.ConvertToEffect(m_obj.CreatePreferenceCounter(pref.Name))).Concat(effect.Once()));

          action.Effect = effect;
        }
      }
    }

    /**
     * Extracts the object structures from the <code>EFFECT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EFFECT</code> node.
     * @return the effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAND_C_EFFECT:
            return this.and_c_effect(cn);
          case LexerTreeConstants.JJTC_EFFECT:
            return this.c_effect(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      return null;
    }

    /**
     * Extracts the object structures from the <code>EFFECT</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EFFECT</code> node.
     * @return the effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect c_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFORALL_EFFECT:
            return this.forall_effect(cn);
          case LexerTreeConstants.JJTWHEN_CON_EFFECT:
            return this.when_con_effect(cn);
          case LexerTreeConstants.JJTP_EFFECT:
            return this.p_effect(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>WHEN_CON_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>WHEN_CON_EFFECT</code> node.
     * @return the when effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private WhenEffect when_con_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTGD
                    && cn2.getId() == LexerTreeConstants.JJTCOND_EFFECT)
        {
          ILogicalExp cond = this.gd(cn1);
          IEffect effect = this.cond_effect(cn2);
          return new WhenEffect(cond, effect);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>WHEN_C_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>WHEN_C_EFFECT</code> node.
     * @return the when effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect cond_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAND_P_EFFECT:
            return this.and_p_effect(cn);
          case LexerTreeConstants.JJTP_EFFECT:
            return this.p_effect(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_P_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_P_EFFECT</code> node.
     * @return the conjuncive propositional effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndEffect and_p_effect(SimpleNode node)
    {
      List<IEffect> effects = new List<IEffect>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTP_EFFECT:
            effects.Add(this.p_effect(cn));
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new AndEffect(effects);
    }

    /**
     * Extracts the object structures from the <code>P_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>P_EFFECT</code> node.
     * @return the propositional effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEffect p_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTLITERAL:
            return this.literal(cn);
          case LexerTreeConstants.JJTASSIGN_OP:
            return this.assign_op(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
            + cn.getLabel() + " unexpected.");
        }
      }
      else if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTFUNCTION_HEAD_OR_FUNCTOR &&
            cn2.getId() == LexerTreeConstants.JJTF_EXP_OR_TERM_OR_UNDEFINED)
        {
          DummyFormulaApplication dummyHead = function_head_or_functor(cn1);
          IEvaluableExp body = f_exp_or_term_or_undefined(cn2);
          if (body is ITerm)
          {
            ObjectFluentApplication head = getFormulaApplication<ObjectFluentApplication>(dummyHead, false, cn1);
            if (head == null)
              head = new ObjectFluentApplication(new ObjectFluent(dummyHead.Name, 
                                                                  new List<ObjectParameterVariable>(), 
                                                                  getDomain().TypeSetSet.Object,
                                                                  DescribedFormula.DefaultAttributes),
                                                 dummyHead.GetArguments());
            if (!head.CanBeAssignedFrom((ITerm)body))
            {
              this.m_mgr.logParserError("Function \""
                          + head.ToTypedString()
                          + "\" cannot be assigned term \""
                          + body.ToTypedString() + "\", since their types are not compatible.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            return new ObjectAssign(head, (ITerm)body);
          }
          else if (body is INumericExp)
          {
            NumericFluentApplication head = getFormulaApplication<NumericFluentApplication>(dummyHead, false, cn1);
            if (head == null)
              head = new NumericFluentApplication(new NumericFluent(dummyHead.Name, 
                                                                    new List<ObjectParameterVariable>(),
                                                                    DescribedFormula.DefaultAttributes),
                                                    dummyHead.GetArguments());
            return new NumericAssign(head, (INumericExp)body);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private IEvaluableExp f_exp_or_term_or_undefined(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 0 && node.GetSuffixId() == LexerConstants.UNDEFINED)
      {
        return UndefinedTerm.Undefined;
      }
      else if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP_OR_TERM:
            return f_exp_or_term(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private IEvaluableExp f_exp_or_term(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTION_HEAD_OR_FUNCTOR:
            GetFluentError error;
            DummyFormulaApplication dummyFormula = function_head_or_functor(cn);
            IEvaluableExp formula = getFormulaApplication<IEvaluableExp>(dummyFormula, true, false, cn, out error) ??
                                       (error.Type == GetFluentError.ErrorType.WRONG_TYPE ? getFormulaApplication<IEvaluableExp>(dummyFormula, true, true, cn, out error) : null);
            if (formula != null)
            {
              return formula;
            }
            else
            {
              if (error.Type == GetFluentError.ErrorType.UNDEFINED && dummyFormula.MayBeConstant)
              {
                Constant constant = GetConstant(dummyFormula.Name);
                if (constant != null)
                  return constant;
              }

              string errorMsg = error.Message;
              if (error.Type == GetFluentError.ErrorType.UNDEFINED)
              {
                errorMsg = string.Format("{0} \"{1}\" undefined",
                                         dummyFormula.MayBeConstant ? "Function or constant" : "Function",
                                         dummyFormula.ToString());
              }
              this.m_mgr.logParserError(errorMsg, this.m_file, node.getLine(), node.getColumn());
              // Continue parsing
              return new NumericFluentApplication(new NumericFluent(dummyFormula.Name, 
                                                                    new List<ObjectParameterVariable>(),
                                                                    DescribedFormula.DefaultAttributes),
                                                  dummyFormula.GetArguments());
            }
          case LexerTreeConstants.JJTF_EXP:
            return f_exp(cn);
          case LexerTreeConstants.JJTTERM:
            return term(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FORALL_C_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_C_EFFECT</code> node.
     * @return the universal effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ForallEffect forall_effect(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTEFFECT)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IEffect effect = this.effect(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ForallEffect(new HashSet<ObjectParameterVariable>(vars.Values), effect);
        }
      }

      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_C_EFFECT</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_C_EFFECT</code> node.
     * @return the conjuncive effect expression built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndEffect and_c_effect(SimpleNode node)
    {
      List<IEffect> effects = new List<IEffect>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        if (cn.getId() == LexerTreeConstants.JJTC_EFFECT)
        {
          effects.Add(this.c_effect(cn));
        }
        else
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      return new AndEffect(effects);
    }

    /**
     * Extracts the object structures from the <code>PRE_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PRE_GD</code> node.
     * @return the precondition goal description built
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp pre_gd(SimpleNode node, ref List<IConditionPrefExp> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTEMPTY_OR:
            return this.empty_or(cn);
          case LexerTreeConstants.JJTPREF_GD:
            return this.pref_gd(cn, ref preferences);
          case LexerTreeConstants.JJTAND_PRE_GD:
            return this.and_pre_gd(cn, ref preferences);
          case LexerTreeConstants.JJTFORALL_PRE_GD:
            if (!this.getFileRequirements().Contains(RequireKey.UNIVERSAL_PRECONDITIONS)
                        && !this.getFileRequirements().Contains(RequireKey.QUANTIFIED_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Universal formula cannot be defined without require keys \""
                         + RequireKey.UNIVERSAL_PRECONDITIONS
                         + "\" or \""
                         + RequireKey.QUANTIFIED_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.forall_pre_gd(cn, ref preferences);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FORALL_PRE_GD</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_PRE_GD</code> node.
     * @return the universal precondition goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ForallExp forall_pre_gd(SimpleNode node, ref List<IConditionPrefExp> preferences)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTPRE_GD)
        {
          Dictionary<string, ObjectParameterVariable> varMap = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, varMap, true);

          HashSet<ObjectParameterVariable> vars = new HashSet<ObjectParameterVariable>(varMap.Values);

          this.m_contextStack.pushQuantifiedContext(varMap.Values.Cast<IVariable>(), cn1);
          List<IConditionPrefExp> quantifiedPrefs = new List<IConditionPrefExp>();
          ILogicalExp exp = this.pre_gd(cn2, ref quantifiedPrefs);
          preferences.AddRange(quantifiedPrefs.Select<IConditionPrefExp, IConditionPrefExp>(pref => new ForallConditionPrefExp(vars, pref)));
          this.m_contextStack.popQuantifiedContext();

          return (exp == null ? null : new ForallExp(vars, exp));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_PRE_GD</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_PRE_GD</code> node.
     * @return the conjuncive precondition goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndExp and_pre_gd(SimpleNode node, ref List<IConditionPrefExp> preferences)
    {
      List<ILogicalExp> expressions = new List<ILogicalExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); ++i)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        if (cn.getId() == LexerTreeConstants.JJTPRE_GD)
        {
          ILogicalExp logicalExp = this.pre_gd(cn, ref preferences);
          if (logicalExp != null)
            expressions.Add(logicalExp);
        }
        else
        {
          throw new ParserException("An internal parser error occurs: node "
                      + cn.getLabel() + " unexpected.");
        }
      }
      return (expressions.Count != 0 ? new AndExp(expressions) : null);
    }

    /**
     * Extracts the object structures from the <code>PREF_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREF_GD</code> node.
     * @return the precondition goal description built
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp pref_gd(SimpleNode node, ref List<IConditionPrefExp> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNAMED_PREF_GD:
            if (!this.getFileRequirements().Contains(RequireKey.PREFERENCES))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.PREFERENCES
                          + "\" needed to specify preferences.",
                          this.m_file, node.getLine(), node.getColumn());
            }
            this.named_pref_gd(cn, ref preferences);
            return null;
          case LexerTreeConstants.JJTGD:
            return this.gd(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>PREF_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREF_GD</code> node.
     * @return the precondition goal description built
     * @throws ParserException if an error occurs while parsing.
     */
    private void named_pref_gd(SimpleNode node, ref List<IConditionPrefExp> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTGD)
        {
          ILogicalExp exp = this.gd(cn1);
          preferences.Add(m_obj.CreateUnnamedConditionPreference(exp));
          return;
        }
      }
      else if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTPREF_NAME
                    && cn2.getId() == LexerTreeConstants.JJTGD)
        {
          string name = this.pref_name(cn1);
          ILogicalExp exp = this.gd(cn2);
          preferences.Add(new ConditionPrefExp(name, exp));
          return;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ACTION_NAME</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ACTION_NAME</code> node.
     * @return the actionCtx name structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private string action_name(SimpleNode node)
    {
      return node.GetImage();
    }

    /**
     * Extracts the object structures from the <code>REQUIRE_DEF</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>REQUIRE_DEF</code> node.
     * @return the require definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private HashSet<RequireKey> requireDef(SimpleNode node)
    {
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTREQUIRE_KEY:
            RequireKey rk = RequireKey.GetRequireKey(cn.GetImage());

            if (rk == null)
            {
              this.m_mgr.logParserError("Invalid require key \"" + cn.GetImage() + "\".",
                          m_file, node.getLine(), node.getColumn());
            }
            else if (rk != null && !this.m_acceptedRequirements.Contains(rk))
            {
              this.m_mgr.logParserError("Require key \"" + rk.GetImage()
                          + "\" forbidden due to accepted requirements.",
                          m_file, cn.getLine(), cn.getColumn());
            }
            else
            {
              if (rk.Equals(RequireKey.ADL))
              {
                this.m_obj.Requirements.Add(RequireKey.ADL);
                this.m_obj.Requirements.Add(RequireKey.STRIPS);
                this.m_obj.Requirements.Add(RequireKey.TYPING);
                this.m_obj.Requirements.Add(RequireKey.EXISTENTIAL_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.UNIVERSAL_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.QUANTIFIED_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.NEGATIVE_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.DISJUNCTIVE_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.EQUALITY);
                this.m_obj.Requirements.Add(RequireKey.CONDITIONAL_EFFECTS);
              }
              else if (rk.Equals(RequireKey.QUANTIFIED_PRECONDITIONS))
              {
                this.m_obj.Requirements.Add(RequireKey.QUANTIFIED_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.EXISTENTIAL_PRECONDITIONS);
                this.m_obj.Requirements.Add(RequireKey.UNIVERSAL_PRECONDITIONS);
              }
              else if (rk.Equals(RequireKey.FLUENTS))
              {
                this.m_obj.Requirements.Add(RequireKey.OBJECT_FLUENTS);
                this.m_obj.Requirements.Add(RequireKey.NUMERIC_FLUENTS);
                this.m_obj.Requirements.Add(RequireKey.FLUENTS);
              }
              else if (rk.Equals(RequireKey.TIMED_INITIAL_LITERALS))
              {
                this.m_obj.Requirements.Add(RequireKey.TIMED_INITIAL_LITERALS);
                this.m_obj.Requirements.Add(RequireKey.DURATIVE_ACTIONS);
              }
              else
              {
                this.m_obj.Requirements.Add(rk);
              }
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return this.m_obj.Requirements;
    }

    /**
     * Extracts the object structures from the <code>TYPE_DEF</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPE_DEF</code> node.
     * @return the type definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, HashSet<string>> types_def(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TYPING))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                    + "\" needed to specify typed terms.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      IDictionary<string, HashSet<string>> types = new Dictionary<string, HashSet<string>>();
      types[Type.OBJECT_SYMBOL] = new HashSet<string>(); // Add the object type.

      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTTYPED_LIST:
            this.type_typed_list(cn, types);

            // Create all types from the types hierarchy
            getDomain().TypeSetSet = new TypeSetSet(types);

            // Add object subtypes
            HashSet<string> obj = types[Type.OBJECT_SYMBOL];
            foreach (string t in types.Keys)
              if (!t.Equals(Type.NUMBER_SYMBOL) && !t.Equals(Type.OBJECT_SYMBOL))
                obj.Add(t);

            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return types;
    }


    /**
     * Extracts the object structures from the <code>TYPED_LIST</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPED_LIST</code> node.
     * @param tl the types list of primitive type already built.
     * @return the typed list structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, HashSet<string>> type_typed_list(SimpleNode node,
                IDictionary<string, HashSet<string>> tl)
    {
      HashSet<string> ptl = new HashSet<string>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTPRIMITIVE_TYPE:
            string pt = primitive_type(cn);
            if (pt.Equals(Type.NUMBER_SYMBOL) || pt.Equals(Type.OBJECT_SYMBOL))
            {
              this.m_mgr.logParserError("Type \"" + pt + "\" is a reserved type.",
                          this.m_file, cn.getLine(), cn.getColumn());
            }
            else if (tl.ContainsKey(pt))
            {
              this.m_mgr.logParserError("Type \"" + pt + "\" duplicated definition.",
                          this.m_file, cn.getLine(), cn.getColumn());
            }
            else
            {
              tl[pt] = new HashSet<string>();
              ptl.Add(pt);
            }
            break;
          case LexerTreeConstants.JJTTYPE:
            HashSet<string> type = this.types(cn);
            foreach (string pti in type)
            {
              if (pti.Equals(Type.NUMBER_SYMBOL))
              {
                this.m_mgr.logParserError("Type \"" + pti + "\" not allowed in type declaration.",
                            this.m_file, node.getLine(), node.getColumn());
              }
              else if (!tl.ContainsKey(pti))
              {
                this.m_mgr.logParserError("Type \"" + pti + "\" undefined.",
                            this.m_file, node.getLine(), node.getColumn());
              }
            }

            if (type.Count == 1)
            {
              // If this is single inheritance, add children to the supertype
              tl[type.First()].UnionWith(ptl);
            }
            else
            {
              // If this is multiple inheritance, add the alleged subtype as a supertype of all given supertypes
              foreach (string pti in ptl)
              {
                tl[pti].UnionWith(type);
              }
            }

            break;
          case LexerTreeConstants.JJTTYPED_LIST:
            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify types.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            IDictionary<string, HashSet<string>> typed_list = this.type_typed_list(cn, tl);
            if (typed_list != tl)
            {
              foreach (KeyValuePair<string, HashSet<string>> pair in typed_list)
                tl[pair.Key] = pair.Value;
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return tl;
    }

    /**
     * Extracts the object structures from the <code>TYPED_LIST</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPED_LIST</code> node.
     * @param tl the typed list of variables already built.
     * @return the typed list structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, ObjectParameterVariable> var_typed_list(SimpleNode node,
                Dictionary<string, ObjectParameterVariable> tl, bool freeVariables)
    {
      List<ObjectParameterVariable> vtl = new List<ObjectParameterVariable>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTVARIABLE:
            ObjectParameterVariable v = this.def_variable(cn, freeVariables);
            if (tl.ContainsKey(v.Name))
            {
              this.m_mgr.logParserError("ObjectVariable \"" + v + "\" duplicated definition.",
                          this.m_file, cn.getLine(), cn.getColumn());
            }
            else
            {
              vtl.Add(v);
              tl[v.Name] = v;
            }

            break;
          case LexerTreeConstants.JJTTYPE:
            TypeSet type = this.type(cn);
            foreach (ObjectParameterVariable vi in vtl)
            {
              vi.SetTypeSet(type);
            }
            vtl.Clear();
            break;
          case LexerTreeConstants.JJTTYPED_LIST:

            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify typed variables.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            this.var_typed_list(cn, tl, freeVariables);
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      if (this.getFileRequirements().Contains(RequireKey.TYPING))
      {
        foreach (ObjectParameterVariable var in vtl)
        {
          this.m_mgr.logParserWarning("ObjectVariable \"" + var.ToString() + "\" was not specified a type, and TYPING is required"
                                  + "; default type OBJECT is assumed.",
                                    this.m_file, node.getLine(), node.getColumn());
        }
      }
      return tl;
    }


    /**
     * Extracts the object structures from the <code>TYPED_LIST</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPED_LIST</code> node.
     * @param tl the typed list of constants already built.
     * @return the typed list structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, Constant> constant_typed_list(SimpleNode node,
                IDictionary<string, Constant> tl)
    {
      List<Constant> ctl = new List<Constant>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTCONSTANT:
            Constant c = this.constant_def(cn);
            if (tl.ContainsKey(c.Name))
            {
              this.m_mgr.logParserError("Constant \"" + c + "\" duplicated definition.",
                          this.m_file, cn.getLine(), cn.getColumn());
            }
            else
            {
              ctl.Add(c);
              tl[c.Name] = c;
            }
            break;
          case LexerTreeConstants.JJTTYPE:
            TypeSet type = this.type(cn);
            foreach (Constant ci in ctl)
            {
              ci.SetTypeSet(type);
            }
            break;
          case LexerTreeConstants.JJTTYPED_LIST:
            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify typed constants.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            this.constant_typed_list(cn, tl);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      return tl;
    }


    /**
     * Extracts the object structures from the <code>PRIMITIVE_TYPE</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PRIMITIVE_TYPE</code> node.
     * @return the primitive type structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private string primitive_type(SimpleNode node)
    {
      return node.GetImage();
    }

        /**
     * Extracts the object structures from the <code>TYPE</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPE</code> node.
     * @return the type structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private TypeSet type(SimpleNode node)
    {
      HashSet<string> types = this.types(node);

      HashSet<string> validTypes = new HashSet<string>();
      foreach (string pti in types)
      {
        if (pti.Equals(Type.NUMBER_SYMBOL))
        {
          this.m_mgr.logParserError("Type \"" + pti + "\" not allowed in type declaration.",
                      this.m_file, node.getLine(), node.getColumn());
        }
        else if (!this.getDomain().TypeSetSet.TypeExists(pti))
        {
          this.m_mgr.logParserError("Type \"" + pti + "\" undefined.",
                      this.m_file, node.getLine(), node.getColumn());
        }
        else
        {
          validTypes.Add(pti);
        }
      }
      return getDomain().TypeSetSet.GetTypeSet(validTypes);
    }

    /**
     * Extracts the object structures from the <code>TYPE</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPE</code> node.
     * @return the type structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private HashSet<string> types(SimpleNode node)
    {
      HashSet<string> type = new HashSet<string>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        if (cn.getId() == LexerTreeConstants.JJTPRIMITIVE_TYPE)
        {
          string primitiveType = this.primitive_type(cn);
          if (!type.Add(primitiveType))
          {
            this.m_mgr.logParserWarning("Type \"" + primitiveType + "\" used more than once as a variable type.",
                                      this.m_file, node.getLine(), node.getColumn());
          }
        }
        else
        {
          throw new ParserException("An internal parser error occurs: node "
                      + node.getLabel() + " unexpected.");
        }
      }
      return type;
    }

    /**
     * Extracts the object structures from the <code>CONSTANTS_DEF</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>CONSTANTS_DEF</code> node.
     * @return the constants definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, Constant> constants_def(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTTYPED_LIST)
        {
          this.constant_typed_list(cn, m_obj.Constants);
          // Adding the constants to the type domains is done during the linking phase.
          // However, we want to flag these constants as being part of the domain!
          m_obj.TypeSetSet.FlagAsDomainConstants(m_obj.Constants.Values);

          return m_obj.Constants;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                          + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>CONSTANT</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>CONSTANT</code> node.
     * @return the constant structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Constant constant(SimpleNode node)
    {
      Constant constant;
      string cst = node.GetImage();

      if ((constant = GetConstant(cst)) == null)
      {
        this.m_mgr.logParserError("Constant \"" + cst.ToString() + "\" undefined.",
                    this.m_file, node.getLine(), node.getColumn());
        constant = new Constant(cst, getDomain().TypeSetSet.Object);
      }
      return constant;
    }

    private Constant constant_def(SimpleNode node)
    {
      Constant constant;
      string typeStr;
      string cst = node.GetImage();

      if (this.m_obj.Constants.TryGetValue(cst, out constant) ||
          this.getDomain().Constants.TryGetValue(cst, out constant))
      {
        this.m_mgr.logParserError("Duplicated constant \"" + cst + "\".",
                    this.m_file, node.getLine(), node.getColumn());
      }
      else
      {
        if (this.m_obj.ExistingNames.TryGetValue(cst, out typeStr) ||
            this.getDomain().ExistingNames.TryGetValue(cst, out typeStr))
        {
          this.m_mgr.logParserError(string.Format("Cannot create constant \"{0}\" as a {1} already exists with that name.", cst, typeStr),
                                  this.m_file, node.getLine(), node.getColumn());
        }
        else
          this.m_obj.ExistingNames[cst] = CONSTANT_STRING;

        constant = new Constant(node.GetImage(), getDomain().TypeSetSet.Object);
      }
      return constant;
    }

    /**
     * Extracts the object structures from the <code>PREDICATES_DEF</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREDICATES_DEF</code> node.
     * @return the predicates definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, RootFormula> predicates_def(SimpleNode node)
    {
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FORMULA_SKELETON_DECLARATION:
            AtomicFormula atom = this.atomic_formula_skeleton(cn, true);

            RootFormula other = null;
            if (m_obj.Formulas.TryGetValue(atom.Name, out other))
            {
              this.m_mgr.logParserError("Predicate \""
                          + atom.ToTypedString()
                          + "\" is already defined as \""
                          + other.ToTypedString() + "\".", this.m_file, node.getLine(),
                          node.getColumn());
            }
            else
            {
              string typeStr;
              if (m_obj.ExistingNames.TryGetValue(atom.Name, out typeStr))
              {
                this.m_mgr.logParserError(string.Format("Cannot create predicate \"{0}\" as a {1} already exists with that name.", atom.Name, typeStr),
                                        this.m_file, node.getLine(), node.getColumn());
              }
              else
                this.m_obj.ExistingNames[atom.Name] = PREDICATE_STRING;

              m_obj.Formulas.Add(atom.Name, atom);
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      return m_obj.Formulas;
    }

    private DescribedFormula.Attributes getAttributes(IEnumerable<SimpleNode> nodes)
    {
      DescribedFormula.Attributes attributes = DescribedFormula.DefaultAttributes;

      foreach (SimpleNode node in nodes)
      {
        if (node.GetSuffixId() == LexerConstants.NO_CYCLE_CHECK)
        {
          attributes.DetectCycles = false;

          if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
          {
            this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                        + "\" needed to specify no-cycle-check.", this.m_file, 
                        node.getLine(), node.getColumn());
          }
        }
      }
      return attributes;
    }

    /**
     * Extracts the object structures from the <code>PREDICATES_DEF</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREDICATES_DEF</code> node.
     * @return the predicates definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AtomicFormula atomic_formula_skeleton(SimpleNode node, bool parseAttributes)
    {
      if (node.jjtGetNumChildren() == 2 || (node.jjtGetNumChildren() > 2 && parseAttributes))
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTPREDICATE
         && cn2.getId() == LexerTreeConstants.JJTTYPED_LIST)
        {
          string predicateName = this.predicate(cn1);
          Dictionary<string, ObjectParameterVariable> arguments = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn2, arguments, false);
          
          LinkedList<SimpleNode> nodes = new LinkedList<SimpleNode>();
          for (int i = 2; i < node.jjtGetNumChildren(); ++i)
          {
            nodes.AddLast((SimpleNode)node.jjtGetChild(i));
          }
          DescribedFormula.Attributes attributes = getAttributes(nodes);

          return new AtomicFormula(predicateName, new List<ObjectParameterVariable>(arguments.Values),
                                   attributes);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>PREDICATE</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREDICATE</code> node.
     * @return the predicate structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private string predicate(SimpleNode node)
    {
      return node.GetImage();
    }

    /**
     * Extracts the object structures from the <code>FUNCTIONS_DEF</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FUNCTIONS_DEF</code> node.
     * @return the functions definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, Fluent> functions_def(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.NUMERIC_FLUENTS) &&
          !this.getFileRequirements().Contains(RequireKey.OBJECT_FLUENTS))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.NUMERIC_FLUENTS
                    + "\" or \"" + RequireKey.OBJECT_FLUENTS
                    + "\" needed to defined functions.", this.m_file, node.getLine(),
                    node.getColumn());
      }
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTION_TYPED_LIST:
            IDictionary<string, Fluent> functions = this.atomic_function_skeleton_typed_list(cn);
            foreach (Fluent fluent in functions.Values)
            {
              RootFormula other = null;
              if (m_obj.Formulas.TryGetValue(fluent.Name, out other))
              {
                this.m_mgr.logParserError("Fluent \""
                            + fluent.ToTypedString()
                            + "\" is already defined as \""
                            + other.ToTypedString() + "\".", this.m_file, node.getLine(),
                            node.getColumn());
              }
              else
              {
                m_obj.Formulas.Add(fluent.Name, fluent);
              }
            }
            return functions;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>TYPED_LIST</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPED_LIST</code> node.
     * @param tl the typed list of constants already built.
     * @return the typed list structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IDictionary<string, Fluent> atomic_function_skeleton_typed_list(SimpleNode node)
    {
      IDictionary<string, Fluent> all = new Dictionary<string, Fluent>();
      IDictionary<string, Fluent> current = new Dictionary<string, Fluent>();

      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FUNCTION_SKELETON_DECLARATION:
            Fluent fhead = this.atomic_function_skeleton(cn, true);
            if (all.ContainsKey(fhead.Name) || current.ContainsKey(fhead.Name))
            {
              this.m_mgr.logParserError("Function \""
                          + fhead.ToTypedString()
                          + "\" is already defined.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            else
            {
              string typeStr;
              if (m_obj.ExistingNames.TryGetValue(fhead.Name, out typeStr))
              {
                this.m_mgr.logParserError(string.Format("Cannot create function \"{0}\" as a {1} already exists with that name.", fhead.Name, typeStr),
                                        this.m_file, node.getLine(), node.getColumn());
              }
              else
                this.m_obj.ExistingNames[fhead.Name] = FUNCTION_STRING;


              current.Add(fhead.Name, fhead);
            }
            break;
          case LexerTreeConstants.JJTFUNCTION_TYPE:
            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify typed functions.", this.m_file,
                          cn.getLine(), cn.getColumn());
            }
            TypeSet typeSet = this.function_type(cn);
            if (typeSet == getDomain().TypeSetSet.Number)
            {
              foreach (Fluent fluent in current.Values)
              {
                all.Add(fluent.Name, fluent);
              }
            }
            else
            {
              foreach (Fluent fluent in current.Values)
              {
                if (!this.getFileRequirements().Contains(RequireKey.OBJECT_FLUENTS))
                {
                  this.m_mgr.logParserError("Require key \"" + RequireKey.OBJECT_FLUENTS
                                        + "\" needed to define object fluents.", this.m_file,
                                        cn.getLine(), cn.getColumn());
                }
                all.Add(fluent.Name, new ObjectFluent(fluent, typeSet));
              }
            }
            current.Clear();
            break;
          case LexerTreeConstants.JJTFUNCTION_TYPED_LIST:
            if (!this.getFileRequirements().Contains(RequireKey.TYPING))
            {
              this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                          + "\" needed to specify typed functions.", this.m_file,
                          node.getLine(), node.getColumn());
            }
            IDictionary<string, Fluent> other = this.atomic_function_skeleton_typed_list(cn);
            foreach (Fluent fluent in other.Values)
            {
              if (all.ContainsKey(fluent.Name))
              {
                this.m_mgr.logParserError("Function \""
                            + fluent.ToTypedString()
                            + "\" is already defined.", this.m_file,
                            node.getLine(), node.getColumn());
              }
              else
              {
                all.Add(fluent.Name, fluent);
              }
            }
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      foreach (Fluent fluent in current.Values)
      {
        all.Add(fluent.Name, fluent);
        if (this.getFileRequirements().Contains(RequireKey.TYPING))
        {
          this.m_mgr.logParserWarning("Function \"" + fluent.ToString() +
                                    "\" was not specified a type, and TYPING is required; default type NUMBER is assumed.",
                                    this.m_file, node.getLine(), node.getColumn());
        }
      }

      return all;
    }

    /**
     * Extracts the object structures from the <code>TYPED_FUNCTION</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TYPED_FUNCTION</code> node.
     * @param tl the typed list of constants already built.
     * @return the typed list structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Fluent typed_function(SimpleNode node, bool parseAttributes)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FUNCTION_SKELETON:
            return this.atomic_function_skeleton(node, parseAttributes);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      else if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTATOMIC_FUNCTION_SKELETON &&
            cn2.getId() == LexerTreeConstants.JJTFUNCTION_TYPE)
        {
          if (!this.getFileRequirements().Contains(RequireKey.TYPING))
          {
            this.m_mgr.logParserError("Require key \"" + RequireKey.TYPING
                        + "\" needed to specify typed functions.", this.m_file,
                        node.getLine(), node.getColumn());
          }
          NumericFluent fluent = this.atomic_function_skeleton(cn1, parseAttributes);
          TypeSet typeSet = this.function_type(cn2);
          if (typeSet == getDomain().TypeSetSet.Number)
          {
            return fluent;
          }
          else
          {
            return new ObjectFluent(fluent, typeSet);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
            + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FUNCTION_TYPE</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FUNCTION_TYPE</code> node.
     * @return the type structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private TypeSet function_type(SimpleNode node)
    {
      HashSet<string> type = new HashSet<string>();
      if (node.GetImage() == Type.NUMBER_SYMBOL)
      {
        return this.getDomain().TypeSetSet.Number;
      }
      else
      {
        if (node.jjtGetNumChildren() == 1)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
          if (cn.getId() == LexerTreeConstants.JJTTYPE)
          {
            return this.type(cn);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                        + node.getLabel() + " unexpected.");
    }


    /**
     * Extracts the object structures from the
     * <code>ATOMIC_FUNCTION_SKELETON</code> node of the syntaxic tree and
     * implements the semantic check.
     * 
     * @param node the <code>ATOMIC_FUNCTION_SKELETON</code> node.
     * @return the atomic function skeleton structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NumericFluent atomic_function_skeleton(SimpleNode node, bool parseAttributes)
    {
      if (node.jjtGetNumChildren() == 2 || (node.jjtGetNumChildren() > 2 && parseAttributes))
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTFUNCTOR
                    && cn2.getId() == LexerTreeConstants.JJTTYPED_LIST)
        {
          string functionName = this.functor(cn1);
          Dictionary<string, ObjectParameterVariable> arguments = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn2, arguments, false);

          LinkedList<SimpleNode> nodes = new LinkedList<SimpleNode>();
          for (int i = 2; i < node.jjtGetNumChildren(); ++i)
          {
            nodes.AddLast((SimpleNode)node.jjtGetChild(i));
          }
          DescribedFormula.Attributes attributes = getAttributes(nodes);

          return new NumericFluent(functionName, 
                                   new List<ObjectParameterVariable>(arguments.Values),
                                   attributes);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FUNCTOR</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FUNCTOR</code> node.
     * @return the function structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private string functor(SimpleNode node)
    {
      return node.GetImage();
    }

    /**
     * Extracts the object structures from the <code>TERM</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TERM</code> node.
     * @return the exp structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ObjectParameterVariable def_variable(SimpleNode node, bool isFree)
    {
      return new ObjectParameterVariable(node.GetImage(), getDomain().TypeSetSet.Object, isFree);
    }

    /**
     * Extracts the object structures from the <code>TERM</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TERM</code> node.
     * @return the exp structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private BooleanLocalVariable def_local_variable(SimpleNode node)
    {
      return new BooleanLocalVariable(node.GetImage());
    }

    /**
     * Extracts the object structures from the <code>VARIABLE</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>VARIABLE</code> node.
     * @return the exp structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Variable variable(SimpleNode node)
    {
      ObjectLocalVariable var = new ObjectLocalVariable(node.GetImage(), getDomain().TypeSetSet.Object);

      return this.m_contextStack.getVariable<Variable>(var, node);
    }

    /**
     * Returns the local variable corresponding to the function name
     * referenced in the specified node.
     * 
     * @param node the local variable corresponding to the function name
     * @return the exp structure built.
     */
    private ILocalVariable local_function_name(SimpleNode node)
    {
      ObjectLocalVariable var = new ObjectLocalVariable(node.GetImage(), getDomain().TypeSetSet.Object);

      return this.m_contextStack.getVariable<ILocalVariable>(var, node);
    }

    /**
     * Returns the local variable referenced in the specified node.
     * 
     * @param node the local variable node
     * @return the exp structure built.
     */
    private ILocalVariable local_variable(SimpleNode node)
    {
      ObjectLocalVariable var = new ObjectLocalVariable(node.GetImage(), getDomain().TypeSetSet.Object);

      return this.m_contextStack.getVariable<ILocalVariable>(var, node);
    }

    /**
     * Returns the object variable referenced in the specified node.
     * 
     * @param node the object variable node
     * @return the exp structure built.
     */
    private ObjectVariable object_variable(SimpleNode node)
    {
      ObjectVariable var = new ObjectLocalVariable(node.GetImage(), getDomain().TypeSetSet.Object);

      return this.m_contextStack.getVariable<ObjectVariable>(var, node);
    }

    /**
     * Returns the object local variable referenced in the specified node.
     * 
     * @param node the object local variable node
     * @return the exp structure built.
     */
    private ObjectLocalVariable object_local_variable(SimpleNode node)
    {
      ObjectLocalVariable var = new ObjectLocalVariable(node.GetImage(), getDomain().TypeSetSet.Object);

      return this.m_contextStack.getVariable<ObjectLocalVariable>(var, node);
    }

    /**
     * Returns the object local variable referenced in the specified node.
     * 
     * @param node the object local variable node
     * @return the exp structure built.
     */
    private ObjectLocalVariable object_function_or_variable(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTOR:
            return this.object_local_variable(cn);
          case LexerTreeConstants.JJTVARIABLE:
            return this.object_local_variable(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Returns the numeric variable referenced in the specified node.
     * 
     * @param node the numeric variable node
     * @return the exp structure built.
     */
    private NumericVariable numeric_variable(SimpleNode node)
    {
      NumericVariable var = new NumericLocalVariable(node.GetImage());

      return this.m_contextStack.getVariable<NumericVariable>(var, node);
    }

    /**
     * Returns the object local variable referenced in the specified node.
     * 
     * @param node the object local variable node
     * @return the exp structure built.
     */
    private NumericLocalVariable numeric_local_variable(SimpleNode node)
    {
      NumericLocalVariable var = new NumericLocalVariable(node.GetImage());

      return this.m_contextStack.getVariable<NumericLocalVariable>(var, node);
    }

    /**
     * Returns the numeric local variable referenced in the specified node.
     * 
     * @param node the numeric local variable node
     * @return the exp structure built.
     */
    private NumericLocalVariable numeric_function_or_variable(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTOR:
            return this.numeric_local_variable(cn);
          case LexerTreeConstants.JJTVARIABLE:
            return this.numeric_local_variable(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Returns the boolean variable referenced in the specified node.
     * 
     * @param node the boolean variable node
     * @return the exp structure built.
     */
    private BooleanVariable boolean_variable(SimpleNode node)
    {
      BooleanVariable var = new BooleanLocalVariable(node.GetImage());

      return this.m_contextStack.getVariable<BooleanVariable>(var, node);
    }

    /**
     * Returns the boolean local variable referenced in the specified node.
     * 
     * @param node the boolean local variable node
     * @return the exp structure built.
     */
    private BooleanLocalVariable boolean_local_variable(SimpleNode node)
    {
      BooleanLocalVariable var = new BooleanLocalVariable(node.GetImage());

      return this.m_contextStack.getVariable<BooleanLocalVariable>(var, node);
    }

    /**
     * Returns the boolean local variable referenced in the specified node.
     * 
     * @param node the boolean local variable node
     * @return the exp structure built.
     */
    private BooleanLocalVariable boolean_function_or_variable(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTOR:
            return this.boolean_local_variable(cn);
          case LexerTreeConstants.JJTVARIABLE:
            return this.boolean_local_variable(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>CONSTRAINTS</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>CONSTRAINTS</code> node.
     * @return the constraints definition structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IConstraintExp constraints(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.CONSTRAINTS))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.CONSTRAINTS
                    + "\" missing to define constraints.", this.m_file,
                    node.getLine(),
                    node.getColumn());
      }
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTPREF_CON_GD)
        {
          List<IConstraintPrefExp> preferences = new List<IConstraintPrefExp>();
          m_obj.Constraints = this.pref_con_gd(cn, ref preferences) ?? TrueExp.True;

          foreach (IConstraintPrefExp pref in preferences)
          {
            m_obj.AddPreference(pref);
            m_obj.ConstraintPreferences.Add(pref);
          }
        }
      }
      return m_obj.Constraints;
    }

    /**
     * Extracts the object structures from the <code>PREF_CON_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREF_CON_GD</code> node.
     * @return the conditional goal description preference structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IConstraintExp pref_con_gd(SimpleNode node, ref List<IConstraintPrefExp> preferences)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAND_PREF_CON_GD:
            return this.and_pref_con_gd(cn, ref preferences);
          case LexerTreeConstants.JJTFORALL_PREF_CON_GD:
            return this.forall_pref_con_gd(cn, ref preferences);
          case LexerTreeConstants.JJTNAMED_PREF_CON_GD:
            this.named_pref_con_gd(cn, ref preferences);
            return null;
          case LexerTreeConstants.JJTCON_GD:
            return this.con_gd(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_PREF_CON_GD</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_PREF_CON_GD</code> node.
     * @return the and condition goal description preference structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndConstraintExp and_pref_con_gd(SimpleNode node, ref List<IConstraintPrefExp> preferences)
    {
      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); ++i)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        if (cn.getId() == LexerTreeConstants.JJTPREF_CON_GD)
        {
          IConstraintExp exp = pref_con_gd(cn, ref preferences);

          if (exp != null)
            expressions.Add(exp);
        }
        else
        {
          throw new ParserException("An internal parser error occurs: node "
                                  + cn.getLabel() + " unexpected.");
        }
      }
      return expressions.Count != 0 ? new AndConstraintExp(expressions) : null;
    }

    /**
     * Extracts the object structures from the <code>FORALL_PREF_CON_GD</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_PREF_CON_GD</code> node.
     * @return the forall condition goal description preference structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ForallConstraintExp forall_pref_con_gd(SimpleNode node, ref List<IConstraintPrefExp> preferences)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTPREF_CON_GD)
        {
          Dictionary<string, ObjectParameterVariable> varMap = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, varMap, true);

          HashSet<ObjectParameterVariable> vars = new HashSet<ObjectParameterVariable>(varMap.Values);
          List<IConstraintPrefExp> quantifiedPreferences = new List<IConstraintPrefExp>();
          this.m_contextStack.pushQuantifiedContext(varMap.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.pref_con_gd(cn2, ref quantifiedPreferences);
          preferences.AddRange(quantifiedPreferences.Select<IConstraintPrefExp, IConstraintPrefExp>(pref => new ForallConstraintPrefExp(vars, pref)));
          this.m_contextStack.popQuantifiedContext();

          return exp != null ? new ForallConstraintExp(vars, exp) : null;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>PREFERENCE</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREFERENCE</code> node.
     * @return the preference structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private void named_pref_con_gd(SimpleNode node, ref List<IConstraintPrefExp> preferences)
    {
      if (!this.getFileRequirements().Contains(RequireKey.PREFERENCES))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.PREFERENCES
                    + "\" missing.", this.m_file, node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTCON_GD)
        {
          IConstraintExp exp = this.con_gd(cn1);

          List<ObjectParameterVariable> vars = new List<ObjectParameterVariable>(this.m_contextStack.getAllQuantifiedVariables<ObjectParameterVariable>());
          preferences.Add(m_obj.CreateUnnamedConstraintPreference(exp, vars));
          return;
        }

      }
      else if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTPREF_NAME
                    && cn2.getId() == LexerTreeConstants.JJTCON_GD)
        {
          string name = this.pref_name(cn1);
          IConstraintExp exp = this.con_gd(cn2);

          List<ObjectParameterVariable> vars = new List<ObjectParameterVariable>(this.m_contextStack.getAllQuantifiedVariables<ObjectParameterVariable>());
          AtomicFormulaApplication atom = m_obj.CreateDummyFormula(name, vars, false, true);
          preferences.Add(new ConstraintPrefExp(name, exp, atom));
          return;
        }
      }
      else
      {
        throw new ParserException("An internal parser error occurs: node "
                    + node.getLabel() + " unexpected.");
      }
    }

    /**
     * Extracts the object structures from the <code>PREF_NAME</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>PREF_NAME</code> node.
     * @return the preference image structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private string pref_name(SimpleNode node)
    {
      return node.GetImage();
    }

    /**
     * Extracts the object structures from the <code>CON_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>CON_GD</code> node.
     * @return the conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IConstraintExp con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAND_CON_GD:
            return this.and_con_gd(cn);
          case LexerTreeConstants.JJTOR_CON_GD:
            return this.or_con_gd(cn);
          case LexerTreeConstants.JJTXOR_UNIQUE_CON_GD:
            return this.xor_con_gd(cn);
          case LexerTreeConstants.JJTNOT_CON_GD:
            return this.not_con_gd(cn);
          case LexerTreeConstants.JJTIMPLY_CON_GD:
            return this.imply_con_gd(cn);
          case LexerTreeConstants.JJTIF_THEN_ELSE_CON_GD:
            return this.if_then_else_con_gd(cn);
          case LexerTreeConstants.JJTEXISTS_CON_GD:
            return this.exists_con_gd(cn);
          case LexerTreeConstants.JJTEXISTS_UNIQUE_CON_GD:
            return this.exists_unique_con_gd(cn);
          case LexerTreeConstants.JJTFORALL_CON_GD:
            return this.forall_con_gd(cn);
          case LexerTreeConstants.JJTAT_END_CON_GD:
            return this.at_end_con_gd(cn);
          case LexerTreeConstants.JJTT_ALWAYS_CON_GD:
            return this.t_always_con_gd(cn);
          case LexerTreeConstants.JJTT_EVENTUALLY_CON_GD:
            return this.t_eventually_con_gd(cn);
          case LexerTreeConstants.JJTT_UNTIL_CON_GD:
            return this.t_until_con_gd(cn);
          case LexerTreeConstants.JJTALWAYS_CON_GD:
            return this.always_con_gd(cn);
          case LexerTreeConstants.JJTWEAK_UNTIL_CON_GD:
            return this.weak_until_con_gd(cn);
          case LexerTreeConstants.JJTUNTIL_CON_GD:
            return this.until_con_gd(cn);
          case LexerTreeConstants.JJTNEXT_CON_GD:
            return this.next_con_gd(cn);
          case LexerTreeConstants.JJTSOMETIME_CON_GD:
            return this.sometime_con_gd(cn);
          case LexerTreeConstants.JJTWITHIN_CON_GD:
            return this.within_con_gd(cn);
          case LexerTreeConstants.JJTAT_MOST_ONCE_CON_GD:
            return this.at_most_once_con_gd(cn);
          case LexerTreeConstants.JJTSOMETIME_AFTER_CON_GD:
            return this.sometime_after_con_gd(cn);
          case LexerTreeConstants.JJTSOMETIME_BEFORE_CON_GD:
            return this.sometime_before_con_gd(cn);
          case LexerTreeConstants.JJTALWAYS_WITHIN_CON_GD:
            return this.always_within_con_gd(cn);
          case LexerTreeConstants.JJTHOLD_DURING_CON_GD:
            return this.hold_during_con_gd(cn);
          case LexerTreeConstants.JJTHOLD_AFTER_CON_GD:
            return this.hold_after_con_gd(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                              + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>HOLD_AFTER</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>HOLD_AFTER</code> node.
     * @return the "hold after" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private HoldAfterExp hold_after_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTNUMBER
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Number time = this.number(cn1);
          IConstraintExp exp = this.sub_con_gd(cn2);

          try
          {
            return new HoldAfterExp(exp, time.Value);
          }
          catch (System.Exception e)
          {
            this.m_mgr.logParserError(e.Message, this.m_file, node.getLine(), node.getColumn());
            return new HoldAfterExp(exp, 1);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>HOLD_DURING</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>HOLD_DURING</code> node.
     * @return the "hold during" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private HoldDuringExp hold_during_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);
        if (cn1.getId() == LexerTreeConstants.JJTNUMBER
                    && cn2.getId() == LexerTreeConstants.JJTNUMBER
                    && cn3.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Number lt = this.number(cn1);
          Number ut = this.number(cn2);
          IConstraintExp exp = this.sub_con_gd(cn3);

          try
          {
            return new HoldDuringExp(lt.Value, ut.Value, exp);
          }
          catch (System.Exception e)
          {
            this.m_mgr.logParserError(e.Message, this.m_file, node.getLine(), node.getColumn());
            return new HoldDuringExp(1, 2, exp);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ALWAYS_WITHIN</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ALWAYS_WITHIN</code> node.
     * @return the "always within" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AlwaysWithinExp always_within_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);
        if (cn1.getId() == LexerTreeConstants.JJTNUMBER
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Number time = this.number(cn1);
          IConstraintExp arg1 = this.sub_con_gd(cn2);
          IConstraintExp arg2 = this.sub_con_gd(cn3);

          try
          {
            return new AlwaysWithinExp(arg1, arg2, time.Value);
          }
          catch (System.Exception e)
          {
            this.m_mgr.logParserError(e.Message, this.m_file, node.getLine(), node.getColumn());
            return new AlwaysWithinExp(arg1, arg2, 1);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SOMETIME_BEFORE</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SOMETIME_BEFORE</code> node.
     * @return the "sometime before" conditional goal description structure
     *         built.
     * @throws ParserException if an error occurs while parsing.
     */
    private SometimeBeforeExp sometime_before_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTSUB_CON_GD
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp arg1 = this.sub_con_gd(cn1);
          IConstraintExp arg2 = this.sub_con_gd(cn2);
          return new SometimeBeforeExp(arg1, arg2);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SOMETIME_AFTER</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SOMETIME_AFTER</code> node.
     * @return the "sometime after" conditional goal description structure
     *         built.
     * @throws ParserException if an error occurs while parsing.
     */
    private SometimeAfterExp sometime_after_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTSUB_CON_GD
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp arg1 = this.sub_con_gd(cn1);
          IConstraintExp arg2 = this.sub_con_gd(cn2);
          return new SometimeAfterExp(arg1, arg2);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_MOST_ONCE</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_MOST_ONCE</code> node.
     * @return the "at most once" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AtMostOnceExp at_most_once_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp exp = this.sub_con_gd(cn);
          return new AtMostOnceExp(exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>WITHIN</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>WITHIN</code> node.
     * @return the "within" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private WithinExp within_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTNUMBER
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Number time = this.number(cn1);
          IConstraintExp exp = this.sub_con_gd(cn2);

          try
          {
            return new WithinExp(exp, time.Value);
          }
          catch (System.Exception e)
          {
            this.m_mgr.logParserError(e.Message, this.m_file, node.getLine(), node.getColumn());
            return new WithinExp(exp, 1);
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SOMETIME</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SOMETIME</code> node.
     * @return the "sometime" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private SometimeExp sometime_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp exp = this.sub_con_gd(cn);
          return new SometimeExp(exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ALWAYS</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ALWAYS</code> node.
     * @return the "always" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AlwaysExp always_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp exp = this.sub_con_gd(cn);
          return new AlwaysExp(exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private TimeInterval time_interval(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);

        if (cn1.getId() == LexerTreeConstants.JJTLOWER_TIME_BOUND &&
            cn2.getId() == LexerTreeConstants.JJTUPPER_TIME_BOUND)
        {
          TimeValue lowerBound = lower_time_bound(cn1);
          TimeValue upperBound = upper_time_bound(cn2);

          TimeInterval interval = new TimeInterval();
          try
          {
            interval = new TimeInterval(lowerBound.Time, lowerBound.IsOpen,
                                        upperBound.Time, upperBound.IsOpen);
          }
          catch (System.Exception e)
          {
            this.m_mgr.logParserError(e.Message, this.m_file, 
                                    node.getLine(), node.getColumn());
          }
          return interval;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private TimeValue lower_time_bound(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);

        if (cn1.getId() == LexerTreeConstants.JJTLOWER_BOUND_TYPE &&
            cn2.getId() == LexerTreeConstants.JJTLOWER_BOUND_TIME)
        {
          bool isOpen = lower_bound_type(cn1);
          double time = lower_bound_time(cn2);
          return new TimeValue(time, isOpen, true);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private bool lower_bound_type(SimpleNode node)
    {
      switch (node.GetImage())
      {
        case "(":
          return true;
        case "[":
          return false;
      }
      throw new ParserException("An internal parser error occurs: lower bound type\""
                  + node.GetImage() + "\" unrecognized.");
    }

    private double lower_bound_time(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            return number(cn).Value;
          case LexerTreeConstants.JJTNEGATIVE_INFINITY:
            return negative_infinity(cn);
        }

        throw new ParserException("An internal parser error occurs: node "
                  + cn.getLabel() + " unexpected.");
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private double negative_infinity(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to specify negative infinity.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      return double.NegativeInfinity;
    }

    private TimeValue upper_time_bound(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);

        if (cn1.getId() == LexerTreeConstants.JJTUPPER_BOUND_TIME &&
            cn2.getId() == LexerTreeConstants.JJTUPPER_BOUND_TYPE)
        {
          double time = upper_bound_time(cn1);
          bool isOpen = upper_bound_type(cn2);
          return new TimeValue(time, isOpen, false);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private bool upper_bound_type(SimpleNode node)
    {
      switch (node.GetImage())
      {
        case ")":
          return true;
        case "]":
          return false;
      }
      throw new ParserException("An internal parser error occurs: lower bound type\""
                  + node.GetImage() + "\" unrecognized.");
    }

    private double upper_bound_time(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            return number(cn).Value;
          case LexerTreeConstants.JJTPOSITIVE_INFINITY:
            return positive_infinity(cn);
        }

        throw new ParserException("An internal parser error occurs: node "
                  + cn.getLabel() + " unexpected.");
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private double positive_infinity(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to specify positive infinity.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      return double.PositiveInfinity;
    }

    private TAlwaysExp t_always_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use temporal expression \"t-always\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);

        if (cn1.getId() == LexerTreeConstants.JJTTIME_INTERVAL &&
            cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          TimeInterval interval = time_interval(cn1);
          IConstraintExp exp = sub_con_gd(cn2);
          return new TAlwaysExp(interval, exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private TEventuallyExp t_eventually_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use temporal expression \"t-eventually\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);

        if (cn1.getId() == LexerTreeConstants.JJTTIME_INTERVAL &&
            cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          TimeInterval interval = time_interval(cn1);
          IConstraintExp exp = sub_con_gd(cn2);
          return new TEventuallyExp(interval, exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private TUntilExp t_until_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use temporal expression \"t-until\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);

        if (cn1.getId() == LexerTreeConstants.JJTTIME_INTERVAL &&
            cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD &&
            cn3.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          TimeInterval interval = time_interval(cn1);
          IConstraintExp arg1 = sub_con_gd(cn2);
          IConstraintExp arg2 = sub_con_gd(cn3);
          return new TUntilExp(interval, arg1, arg2);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>NEXT</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>NEXT</code> node.
     * @return the "next" conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NextExp next_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use temporal expression \"next\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp exp = this.sub_con_gd(cn);
          return new NextExp(exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>WEAK_UNTIL</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>WEAK_UNTIL</code> node.
     * @return the "weak until" conditional goal description structure
     *         built.
     * @throws ParserException if an error occurs while parsing.
     */
    private WeakUntilExp weak_until_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use temporal expression \"weak-until\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTSUB_CON_GD
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp arg1 = this.sub_con_gd(cn1);
          IConstraintExp arg2 = this.sub_con_gd(cn2);
          return new WeakUntilExp(arg1, arg2);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>UNTIL</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>UNTIL</code> node.
     * @return the "until" conditional goal description structure
     *         built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UntilExp until_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use temporal expression \"until\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTSUB_CON_GD
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          IConstraintExp arg1 = this.sub_con_gd(cn1);
          IConstraintExp arg2 = this.sub_con_gd(cn2);
          return new UntilExp(arg1, arg2);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_CON_GD</code> node.
     * @return the and conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndConstraintExp and_con_gd(SimpleNode node)
    {
      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTCON_GD:
            expressions.Add(this.con_gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new AndConstraintExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>OR_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OR_CON_GD</code> node.
     * @return the and conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private OrConstraintExp or_con_gd(SimpleNode node)
    {
      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTCON_GD:
            expressions.Add(this.con_gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new OrConstraintExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>XOR_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>XOR_CON_GD</code> node.
     * @return the and conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private XorUniqueConstraintExp xor_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"xor!\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTCON_GD:
            expressions.Add(this.con_gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new XorUniqueConstraintExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>NOT_CON_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>NOT_CON_GD</code> node.
     * @return the negative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NotConstraintExp not_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTCON_GD)
        {
          return new NotConstraintExp(this.con_gd(cn));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>IMPLY_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>IMPLY_CON_GD</code> node.
     * @return the implicative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ImplyConstraintExp imply_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTGD
                    && cn2.getId() == LexerTreeConstants.JJTCON_GD)
        {
          ILogicalExp head = this.gd(cn1);
          IConstraintExp body = this.con_gd(cn2);
          return new ImplyConstraintExp(head, body);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>IF_THEN_ELSE_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>IF_THEN_ELSE_CON_GD</code> node.
     * @return the implicative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IfThenElseConstraintExp if_then_else_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"if-then-else\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);
        if (cn1.getId() == LexerTreeConstants.JJTGD
         && cn2.getId() == LexerTreeConstants.JJTCON_GD
         && cn3.getId() == LexerTreeConstants.JJTCON_GD)
        {
          ILogicalExp ifExp = this.gd(cn1);
          IConstraintExp thenExp = this.con_gd(cn2);
          IConstraintExp elseExp = this.con_gd(cn3);
          return new IfThenElseConstraintExp(ifExp, thenExp, elseExp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXISTS_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXISTS_CON_GD</code> node.
     * @return the existential goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ExistsConstraintExp exists_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTCON_GD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.con_gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ExistsConstraintExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXISTS_UNIQUE_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXISTS_UNIQUE_CON_GD</code> node.
     * @return the existential goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ExistsUniqueConstraintExp exists_unique_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"exists!\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTCON_GD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.con_gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ExistsUniqueConstraintExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FORALL_CON_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_CON_GD</code> node.
     * @return the universal conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ForallConstraintExp forall_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTCON_GD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.con_gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ForallConstraintExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AT_END_CON_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AT_END_CON_GD</code> node.
     * @return the at end conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AtEndConstraintExp at_end_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTGD)
        {
          ILogicalExp exp = this.gd(cn);
          return new AtEndConstraintExp(exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SUB_CON_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SUB_CON_GD</code> node.
     * @return the conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IConstraintExp sub_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTAND_SUB_CON_GD:
            return this.and_sub_con_gd(cn);
          case LexerTreeConstants.JJTOR_SUB_CON_GD:
            return this.or_sub_con_gd(cn);
          case LexerTreeConstants.JJTXOR_UNIQUE_SUB_CON_GD:
            return this.xor_sub_con_gd(cn);
          case LexerTreeConstants.JJTNOT_SUB_CON_GD:
            return this.not_sub_con_gd(cn);
          case LexerTreeConstants.JJTIMPLY_SUB_CON_GD:
            return this.imply_sub_con_gd(cn);
          case LexerTreeConstants.JJTIF_THEN_ELSE_SUB_CON_GD:
            return this.if_then_else_sub_con_gd(cn);
          case LexerTreeConstants.JJTEXISTS_SUB_CON_GD:
            return this.exists_sub_con_gd(cn);
          case LexerTreeConstants.JJTEXISTS_UNIQUE_SUB_CON_GD:
            return this.exists_unique_sub_con_gd(cn);
          case LexerTreeConstants.JJTFORALL_SUB_CON_GD:
            return this.forall_sub_con_gd(cn);
          case LexerTreeConstants.JJTCON_GD:
            return this.con_gd(cn);
          case LexerTreeConstants.JJTGD:
            return this.gd(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>AND_SUB_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_SUB_CON_GD</code> node.
     * @return the and conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndConstraintExp and_sub_con_gd(SimpleNode node)
    {
      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTSUB_CON_GD:
            expressions.Add(this.sub_con_gd(cn));
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new AndConstraintExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>OR_SUB_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OR_SUB_CON_GD</code> node.
     * @return the and conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private OrConstraintExp or_sub_con_gd(SimpleNode node)
    {
      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTSUB_CON_GD:
            expressions.Add(this.sub_con_gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new OrConstraintExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>XOR_SUB_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>XOR_SUB_CON_GD</code> node.
     * @return the and conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private XorUniqueConstraintExp xor_sub_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"xor!\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      List<IConstraintExp> expressions = new List<IConstraintExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTSUB_CON_GD:
            expressions.Add(this.sub_con_gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new XorUniqueConstraintExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>NOT_SUB_CON_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>NOT_SUB_CON_GD</code> node.
     * @return the negative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NotConstraintExp not_sub_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          return new NotConstraintExp(this.sub_con_gd(cn));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>IMPLY_SUB_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>IMPLY_SUB_CON_GD</code> node.
     * @return the implicative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ImplyConstraintExp imply_sub_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTGD
                    && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          ILogicalExp head = this.gd(cn1);
          IConstraintExp body = this.sub_con_gd(cn2);
          return new ImplyConstraintExp(head, body);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
      * Extracts the object structures from the <code>IF_THEN_ELSE_SUB_CON_GD</code> node of
      * the syntaxic tree and implements the semantic check.
      * 
      * @param node the <code>IF_THEN_ELSE_SUB_CON_GD</code> node.
      * @return the implicative goal description structure built.
      * @throws ParserException if an error occurs while parsing.
      */
    private IfThenElseConstraintExp if_then_else_sub_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"if-then-else\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);
        if (cn1.getId() == LexerTreeConstants.JJTGD
         && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD
         && cn3.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          ILogicalExp ifExp = this.gd(cn1);
          IConstraintExp elseExp = this.sub_con_gd(cn2);
          IConstraintExp thenExp = this.sub_con_gd(cn3);

          return new IfThenElseConstraintExp(ifExp, elseExp, thenExp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXISTS_SUB_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXISTS_SUB_CON_GD</code> node.
     * @return the existential goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ExistsConstraintExp exists_sub_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.sub_con_gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ExistsConstraintExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXISTS_UNIQUE_SUB_CON_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXISTS_UNIQUE_SUB_CON_GD</code> node.
     * @return the existential goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ExistsUniqueConstraintExp exists_unique_sub_con_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"exists!\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.sub_con_gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ExistsUniqueConstraintExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FORALL_SUB_CON_GD</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_SUB_CON_GD</code> node.
     * @return the universal conditional goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ForallConstraintExp forall_sub_con_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTSUB_CON_GD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          IConstraintExp exp = this.sub_con_gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ForallConstraintExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>GD</code> node.
     * @return the goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FORMULA:
            return this.logical_formula(cn);
          case LexerTreeConstants.JJTAND_GD:
            return this.and_gd(cn);
          case LexerTreeConstants.JJTOR_GD:
            if (!this.getFileRequirements().Contains(RequireKey.DISJUNCTIVE_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Disjunctive formula cannot be defined without require key \""
                         + RequireKey.DISJUNCTIVE_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.or_gd(cn);
          case LexerTreeConstants.JJTXOR_UNIQUE_GD:
            // TODO: What should the requirements be (considering that xor = !(p and q) and (p or q)) ?
            if (!this.getFileRequirements().Contains(RequireKey.DISJUNCTIVE_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Disjunctive formula cannot be defined without require key \""
                         + RequireKey.DISJUNCTIVE_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            if (!this.getFileRequirements().Contains(RequireKey.NEGATIVE_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Negative formula cannot be defined without require key \""
                         + RequireKey.NEGATIVE_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.xor_gd(cn);
          case LexerTreeConstants.JJTNOT_GD:
            if (!this.getFileRequirements().Contains(RequireKey.NEGATIVE_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Negative formula cannot be defined without require key \""
                         + RequireKey.NEGATIVE_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.not_gd(cn);
          case LexerTreeConstants.JJTIMPLY_GD:
            if (!this.getFileRequirements().Contains(RequireKey.DISJUNCTIVE_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Implication formula cannot be defined without require key \""
                          + RequireKey.DISJUNCTIVE_PRECONDITIONS
                          + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.imply_gd(cn);
          case LexerTreeConstants.JJTIF_THEN_ELSE_GD:
            if (!this.getFileRequirements().Contains(RequireKey.DISJUNCTIVE_PRECONDITIONS))
            {
              this.m_mgr.logParserError("If-then-else formula cannot be defined without require key \""
                          + RequireKey.DISJUNCTIVE_PRECONDITIONS
                          + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.if_then_else_gd(cn);
          case LexerTreeConstants.JJTFORALL_GD:
            if (!this.getFileRequirements().Contains(RequireKey.UNIVERSAL_PRECONDITIONS)
                        && !this.getFileRequirements().Contains(RequireKey.QUANTIFIED_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Universal formula cannot be defined without require keys \""
                         + RequireKey.UNIVERSAL_PRECONDITIONS
                         + "\" or \""
                         + RequireKey.QUANTIFIED_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.forall_gd(cn);
          case LexerTreeConstants.JJTEXISTS_GD:
            if (!this.getFileRequirements().Contains(RequireKey.EXISTENTIAL_PRECONDITIONS)
                        && !this.getFileRequirements().Contains(RequireKey.QUANTIFIED_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Existential formula cannot be defined without require keys \""
                         + RequireKey.EXISTENTIAL_PRECONDITIONS
                         + "\" or \""
                         + RequireKey.QUANTIFIED_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.exists_gd(cn);
          case LexerTreeConstants.JJTEXISTS_UNIQUE_GD:
            if (!this.getFileRequirements().Contains(RequireKey.EXISTENTIAL_PRECONDITIONS)
                        && !this.getFileRequirements().Contains(RequireKey.QUANTIFIED_PRECONDITIONS))
            {
              this.m_mgr.logParserError("Existential formula cannot be defined without require keys \""
                         + RequireKey.EXISTENTIAL_PRECONDITIONS
                         + "\" or \""
                         + RequireKey.QUANTIFIED_PRECONDITIONS
                         + "\".", this.m_file, node.getLine(), node.getColumn());
            }
            return this.exists_unique_gd(cn);
          case LexerTreeConstants.JJTF_COMP:
            return this.f_comp(cn);
          case LexerTreeConstants.JJTGOAL_MODALITY:
            return this.goal_modality(cn);
          case LexerTreeConstants.JJTASSIGN_LOCAL_VAR:
            return this.assign_local_var(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                              + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FCOMP</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FCOMP</code> node.
     * @return the comparaison function structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp f_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTBINARY_COMP)
        {
          return this.binary_comp(cn);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>GOAL_MODALITY</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>GOAL_MODALITY</code> node.
     * @return the comparaison function structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private GoalModalityExp goal_modality(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use the GOAL predicate.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTGD)
        {
          ILogicalExp exp = this.gd(cn);
          GoalModalityExp goalModality = new GoalModalityExp(exp, getDomain().GetAllGoalWorldsInstance.getAllGoalWorlds);

          return goalModality;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ASSIGN_LOCAL_VAR</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ASSIGN_LOCAL_VAR</code> node.
     * @return the comparaison function structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AssignLocalVar assign_local_var(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use local variables.", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTFUNCTION_OR_VARIABLE &&
            cn2.getId() == LexerTreeConstants.JJTLOCAL_VALUE)
        {
          IEvaluableExp body = this.local_value(cn2);
          if (body is ILogicalExp)
          {
            BooleanLocalVariable var = this.boolean_function_or_variable(cn1);

            return new AssignBooleanLocalVar((BooleanLocalVariable)var, (ILogicalExp)body);
          }
          else if (body is INumericExp)
          {
            NumericLocalVariable var = this.numeric_function_or_variable(cn1);

            return new AssignNumericLocalVar((NumericLocalVariable)var, (INumericExp)body);
          }
          else if (body is ITerm)
          {
            ObjectLocalVariable var = this.object_function_or_variable(cn1);

            if (!var.CanBeAssignedFrom((ITerm)body))
            {
              this.m_mgr.logParserError("Local variable \""
                          + var.ToTypedString()
                          + "\" cannot be assigned body \""
                          + body.ToTypedString() + "\", since their types are not compatible.",
                          this.m_file, node.getLine(), node.getColumn());
            }

            return new AssignObjectLocalVar((ObjectLocalVariable)var, (ITerm)body);
          }
          else
          {
            throw new ParserException("An internal parser error occurs: node "
                        + cn2.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FUNCTION_OR_VARIABLE</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FUNCTION_OR_VARIABLE</code> node.
     * @return the comparaison function structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILocalVariable function_or_variable(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTOR:
            return this.local_function_name(cn);
          case LexerTreeConstants.JJTVARIABLE:
            return this.local_variable(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                  + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LOCAL_VALUE</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LOCAL_VALUE</code> node.
     * @return the comparaison function structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IEvaluableExp local_value(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTVARIABLE:
            return this.variable(cn);
          case LexerTreeConstants.JJTF_EXP:
            return this.f_exp(cn);
          case LexerTreeConstants.JJTTERM:
            return this.term(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                  + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>BINARY_COMP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>BINARY_COMP</code> node.
     * @return the binray numeric operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp binary_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTGREATER_COMP:
            return this.greater_comp(cn);
          case LexerTreeConstants.JJTGREATER_EQUAL_COMP:
            return this.greater_equal_comp(cn);
          case LexerTreeConstants.JJTLESS_COMP:
            return this.less_comp(cn);
          case LexerTreeConstants.JJTLESS_EQUAL_COMP:
            return this.less_equal_comp(cn);
          case LexerTreeConstants.JJTEQUAL_COMP:
            return this.equal_comp(cn);
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>GREATER_COMP</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>GREATER_COMP</code> node.
     * @return the greater compraison structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private GreaterComp greater_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          return new GreaterComp(this.f_exp(cn1), this.f_exp(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LESS_COMP</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LESS_COMP</code> node.
     * @return the less compraison structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private LessComp less_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          return new LessComp(this.f_exp(cn1), this.f_exp(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EQUAL_COMP</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EQUAL_COMP</code> node.
     * @return the equal compraison structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILogicalExp equal_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP_OR_TERM
         && cn2.getId() == LexerTreeConstants.JJTF_EXP_OR_TERM)
        {
          IEvaluableExp arg1 = f_exp_or_term(cn1);
          IEvaluableExp arg2 = f_exp_or_term(cn2);

          if (arg1 is ITerm && arg2 is ITerm)
          {
            ITerm t1 = (ITerm)arg1;
            ITerm t2 = (ITerm)arg2;

            ObjectEqualComp comp = new ObjectEqualComp(t1, t2);

            if (!t1.IsComparableTo(t2))
            {
              this.m_mgr.logParserError(string.Format("Error in expression \"{0}\", terms of type \"{1}\" and \"{2}\" respectively " +
                                      "cannot be compared since their domains do not overlap.",
                                      comp.ToString(), t1.GetTypeSet().ToString(), t2.GetTypeSet().ToString()),
                                      this.m_file, node.getLine(), node.getColumn());
            }

            return comp;
          }
          else if (arg1 is INumericExp && arg2 is INumericExp)
          {
            return new NumericEqualComp((INumericExp)arg1, (INumericExp)arg2);
          }
          else
          {
            this.m_mgr.logParserError(" \""
                        + arg1.ToTypedString()
                        + "\" and \""
                        + arg2.ToTypedString()
                        + "\" cannot be checked for equality since they are not compatible.",
                        this.m_file, node.getLine(), node.getColumn());
            // Return dummy comparison
            if (arg1 is ITerm)
              return new ObjectEqualComp((ITerm)arg1, (ITerm)arg1);
            else if (arg1 is INumericExp)
              return new NumericEqualComp((INumericExp)arg1, new Number(0));
            else
              throw new ParserException("An internal parser error occurs: node "
                          + cn1.getLabel() + " unexpected.");
          }
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>GREATER_EQUAL_COMP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>GREATER_EQUAL_COMP</code> node.
     * @return the greater equal compraison structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private GreaterEqualComp greater_equal_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          return new GreaterEqualComp(this.f_exp(cn1), this.f_exp(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LESS_EQUAL_COMP</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LESS_EQUAL_COMP</code> node.
     * @return the less equal compraison structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private LessEqualComp less_equal_comp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP
                    && cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          return new LessEqualComp(this.f_exp(cn1), this.f_exp(cn2));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>F_EXP</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>F_EXP</code> node.
     * @return the fonction expression structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp f_exp(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTNUMBER:
            return this.number(cn);
          case LexerTreeConstants.JJTVARIABLE:
            return this.numeric_variable(cn);
          case LexerTreeConstants.JJTOP:
            return this.op(cn);
          case LexerTreeConstants.JJTF_HEAD:
            return this.f_head(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private NumericFluentApplication numeric_fluent(SimpleNode node)
    {
      INumericExp function = f_head(node);
      if (function is NumericFluentApplication)
      {
        return (NumericFluentApplication)function;
      }
      else
      {
        FormulaApplication other = (FormulaApplication)function;

        NumericFluentApplication newFluent = new NumericFluentApplication(
                        new NumericFluent(other.Name, 
                                          new List<ObjectParameterVariable>(),
                                          DescribedFormula.DefaultAttributes),
                        other.GetArguments());
        this.m_mgr.logParserError("Numeric fluent expected instead of \""
                              + other.ToTypedString() + "\".", this.m_file,
                                node.getLine(), node.getColumn());
        // Continue parsing
        return newFluent;
      }
    }

    /**
     * Extracts the object structures from the <code>F_HEAD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>F_HEAD</code> node.
     * @return the function head structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private INumericExp f_head(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTION_HEAD_OR_FUNCTOR:
            DummyFormulaApplication dummyFormula = function_head_or_functor(cn);
            INumericExp numericFunction = getFormulaApplication<INumericExp>(dummyFormula, false, cn);
            if (numericFunction == null)
              numericFunction = new NumericFluentApplication(new NumericFluent(dummyFormula.Name,
                                                                               new List<ObjectParameterVariable>(),
                                                                               DescribedFormula.DefaultAttributes),
                                                             dummyFormula.GetArguments());
            return numericFunction;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>OPERATION</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OPERATION</code> node.
     * @return the operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AbstractFunctionExp op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTADD_OP:
            return this.add_op(cn);
          case LexerTreeConstants.JJTSUBSTRACT_OP:
            return this.substract_op(cn);
          case LexerTreeConstants.JJTMULTIPLY_OP:
            return this.multiply_op(cn);
          case LexerTreeConstants.JJTDIVIDE_OP:
            return this.divide_op(cn);
          case LexerTreeConstants.JJTMOD_OP:
            return this.mod_op(cn);
          case LexerTreeConstants.JJTMAX_OP:
            return this.max_op(cn);
          case LexerTreeConstants.JJTMIN_OP:
            return this.min_op(cn);
          case LexerTreeConstants.JJTEXPT_OP:
            return this.expt_op(cn);
          case LexerTreeConstants.JJTSQRT_OP:
            return this.sqrt_op(cn);
          case LexerTreeConstants.JJTABS_OP:
            return this.abs_op(cn);
          case LexerTreeConstants.JJTLOG_OP:
            return this.log_op(cn);
          case LexerTreeConstants.JJTEXP_OP:
            return this.exp_op(cn);
          case LexerTreeConstants.JJTROUND_OP:
            return this.round_op(cn);
          case LexerTreeConstants.JJTINT_OP:
            return this.int_op(cn);
          case LexerTreeConstants.JJTFLOOR_OP:
            return this.floor_op(cn);
          case LexerTreeConstants.JJTCEIL_OP:
            return this.ceil_op(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ADD_OPERATION</code> node
     * of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ADD_OPERATION</code> node.
     * @return the add operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityAdd add_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityAdd(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SUBSTRACT_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SUBSTRACT_OPERATION</code> node.
     * @return the substract operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NAritySubstract substract_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NAritySubstract(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>MULTIPLY_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MULTIPLY_OPERATION</code> node.
     * @return the multiply operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityMultiply multiply_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityMultiply(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>DIVIDE_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>DIVIDE_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityDivide divide_op(SimpleNode node)
    {
      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityDivide(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>MOD_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MOD_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityModulo mod_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"mod\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityModulo(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>MAX_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>MAX_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityMax max_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"max\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityMax(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>NARITY_MIN</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>NARITY_MIN</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityMin min_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"min\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityMin(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXPT_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXPT_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityExponent expt_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"expt\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() >= 2)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityExponent(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>SQRT_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>SQRT_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UnarySquareRoot sqrt_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"sqrt\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnarySquareRoot(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ABS_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ABS_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NArityAbsoluteValue abs_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"abs\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() >= 1)
      {
        List<INumericExp> arguments = new List<INumericExp>();
        for (int i = 0; i < node.jjtGetNumChildren(); ++i)
        {
          SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
          switch (cn.getId())
          {
            case LexerTreeConstants.JJTF_EXP:
              arguments.Add(this.f_exp(cn));
              break;
            default:
              throw new ParserException("An internal parser error occurs: node "
                                        + cn.getLabel() + " unexpected.");
          }
        }
        return new NArityAbsoluteValue(arguments);
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LOG_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LOG_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AbstractFunctionExp log_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"log\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnaryNaturalLogarithm(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      else if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTF_EXP &&
            cn2.getId() == LexerTreeConstants.JJTF_EXP)
        {
          return new BinaryLogarithm(this.f_exp(cn1), this.f_exp(cn2));
        }
        else
        {
          throw new ParserException("An internal parser error occurs: node "
                                    + node.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXP_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXP_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UnaryExponential exp_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"exp\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnaryExponential(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ROUND_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ROUND_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UnaryRound round_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"round\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnaryRound(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>INT_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>INT_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UnaryTruncate int_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"int\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnaryTruncate(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FLOOR_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FLOOR_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UnaryFloor floor_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"floor\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnaryFloor(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>CEILING_OPERATION</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>CEILING_OPERATION</code> node.
     * @return the divide operation structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private UnaryCeiling ceil_op(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use function \"ceil\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTF_EXP:
            return new UnaryCeiling(this.f_exp(cn));
          default:
            throw new ParserException("An internal parser error occurs: node "
                                      + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>Number</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>Number</code> node.
     * @return the number structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private Number number(SimpleNode node)
    {
      return new Number(System.Double.Parse(node.GetImage(), new System.Globalization.CultureInfo("en-US")));
    }

    /**
     * Extracts the object structures from the <code>AND_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>AND_GD</code> node.
     * @return the conjunctive goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AndExp and_gd(SimpleNode node)
    {
      List<ILogicalExp> expressions = new List<ILogicalExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTGD:
            expressions.Add(this.gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new AndExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>OR_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>OR_GD</code> node.
     * @return the disjunctive goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private OrExp or_gd(SimpleNode node)
    {
      List<ILogicalExp> expressions = new List<ILogicalExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTGD:
            expressions.Add(this.gd(cn));
            break;
          default:
            throw new ParserException(
                        "An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new OrExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>XOR_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>XOR_GD</code> node.
     * @return the disjunctive goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private XorUniqueExp xor_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"xor!\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      List<ILogicalExp> expressions = new List<ILogicalExp>();
      for (int i = 0; i < node.jjtGetNumChildren(); i++)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTGD:
            expressions.Add(this.gd(cn));
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return new XorUniqueExp(expressions);
    }

    /**
     * Extracts the object structures from the <code>NOT_GD</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>NOT_GD</code> node.
     * @return the negative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private NotExp not_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTGD)
        {
          return new NotExp(this.gd(cn));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>IMPLY_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>IMPLY_GD</code> node.
     * @return the implicative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ImplyExp imply_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTGD
                    && cn2.getId() == LexerTreeConstants.JJTGD)
        {
          ILogicalExp head = this.gd(cn1);
          ILogicalExp body = this.gd(cn2);
          return new ImplyExp(head, body);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>IF_THEN_ELSE_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>IF_THEN_ELSE_GD</code> node.
     * @return the implicative goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private IfThenElseExp if_then_else_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"if-then-else\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 3)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        SimpleNode cn3 = (SimpleNode)node.jjtGetChild(2);
        if (cn1.getId() == LexerTreeConstants.JJTGD
         && cn2.getId() == LexerTreeConstants.JJTGD
         && cn3.getId() == LexerTreeConstants.JJTGD)
        {
          ILogicalExp ifExp = this.gd(cn1);
          ILogicalExp elseExp = this.gd(cn2);
          ILogicalExp thenExp = this.gd(cn3);
          return new IfThenElseExp(ifExp, elseExp, thenExp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>FORALL_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>FORALL_GD</code> node.
     * @return the universal goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ForallExp forall_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTGD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          ILogicalExp exp = this.gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ForallExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXISTS_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXISTS_GD</code> node.
     * @return the existential goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ExistsExp exists_gd(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTGD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          ILogicalExp exp = this.gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ExistsExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>EXISTS_GD</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>EXISTS_GD</code> node.
     * @return the existential goal description structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ExistsUniqueExp exists_unique_gd(SimpleNode node)
    {
      if (!this.getFileRequirements().Contains(RequireKey.TLPLAN))
      {
        this.m_mgr.logParserError("Require key \"" + RequireKey.TLPLAN
                    + "\" needed to use \"exists!\".", this.m_file,
                    node.getLine(), node.getColumn());
      }

      if (node.jjtGetNumChildren() == 2)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        SimpleNode cn2 = (SimpleNode)node.jjtGetChild(1);
        if (cn1.getId() == LexerTreeConstants.JJTTYPED_LIST
         && cn2.getId() == LexerTreeConstants.JJTGD)
        {
          Dictionary<string, ObjectParameterVariable> vars = new Dictionary<string, ObjectParameterVariable>();
          this.var_typed_list(cn1, vars, true);

          this.m_contextStack.pushQuantifiedContext(vars.Values.Cast<IVariable>(), cn1);
          ILogicalExp exp = this.gd(cn2);
          this.m_contextStack.popQuantifiedContext();

          return new ExistsUniqueExp(new HashSet<ObjectParameterVariable>(vars.Values), exp);
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LITERAL</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LITERAL</code> node.
     * @return the exp literal structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILiteral literal(SimpleNode node)
    {
      ILiteral exp = null;
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FORMULA:
            exp = this.atomic_formula(cn);
            break;
          case LexerTreeConstants.JJTNOT_ATOMIC_FORMULA:
            exp = this.not_atomic_formula(cn);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return exp;
    }

    /**
     * Extracts the object structures from the <code>LITERAL</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LITERAL</code> node.
     * @return the exp literal structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILiteral ground_literal(SimpleNode node)
    {
      ILiteral exp = null;
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTATOMIC_FORMULA:
            exp = this.ground_atomic_formula(cn);
            break;
          case LexerTreeConstants.JJTNOT_ATOMIC_FORMULA:
            exp = this.not_ground_atomic_formula(cn);
            break;
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      return exp;
    }

    /**
     * Extracts the object structures from the <code>LITERAL</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LITERAL</code> node.
     * @return the exp literal structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILiteral not_atomic_formula(SimpleNode node)
    {
      //Literal exp = null;
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTATOMIC_FORMULA)
        {
          return new NotAtomicFormulaApplication(this.atomic_formula(cn));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>LITERAL</code> node of
     * the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>LITERAL</code> node.
     * @return the exp literal structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ILiteral not_ground_atomic_formula(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTATOMIC_FORMULA)
        {
          return new NotAtomicFormulaApplication(this.ground_atomic_formula(cn));
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                                      + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>ATOMIC_FORMULA</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ATOMIC_FORMULA</code> node.
     * @return the atomic formula structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AtomicFormulaApplication atomic_formula(SimpleNode node)
    {
      ILogicalExp form = logical_formula(node);

      if (form is AtomicFormulaApplication)
      {
        return (AtomicFormulaApplication)form;
      }
      else
      {
        FormulaApplication other = (FormulaApplication)form;

        AtomicFormulaApplication newForm = new AtomicFormulaApplication(
                        new AtomicFormula(other.Name, 
                                          new List<ObjectParameterVariable>(),
                                          DescribedFormula.DefaultAttributes),
                        other.GetArguments());
        this.m_mgr.logParserError("Atomic formula expected instead of \""
                              + other.ToTypedString() + "\".", this.m_file,
                                node.getLine(), node.getColumn());
        // Continue parsing
        return newForm;
      }
    }

    private ILogicalExp logical_formula(SimpleNode node)
    {
      DummyFormulaApplication dummyFormula = predicate_head(node);
      ILogicalExp formula = getFormulaApplication<ILogicalExp>(dummyFormula, false, node);
      if (formula == null)
      {
        formula = new AtomicFormulaApplication(new AtomicFormula(dummyFormula.Name, 
                                                                 new List<ObjectParameterVariable>(),
                                                                 DescribedFormula.DefaultAttributes),
                                              dummyFormula.GetArguments());
      }
      return formula;
    }

    /**
     * Extracts the object structures from the <code>ATOMIC_FORMULA</code>
     * node of the syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>ATOMIC_FORMULA</code> node.
     * @return the atomic formula structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private AtomicFormulaApplication ground_atomic_formula(SimpleNode node)
    {
      if (node.jjtGetNumChildren() > 0)
      {
        SimpleNode cn1 = (SimpleNode)node.jjtGetChild(0);
        if (cn1.getId() == LexerTreeConstants.JJTPREDICATE)
        {
          AtomicFormulaApplication atomic_formula = null;
          string predicateName = this.predicate(cn1);

          List<ITerm> arguments = new List<ITerm>();
          for (int i = 1; i < node.jjtGetNumChildren(); i++)
          {
            SimpleNode cn = (SimpleNode)node.jjtGetChild(i);
            if (cn.getId() == LexerTreeConstants.JJTCONSTANT)
            {
              arguments.Add(this.constant(cn));
            }
            else
            {
              throw new ParserException("An internal parser error occurs: node "
                                  + cn.getLabel() + " unexpected.");
            }
          }

          RootFormula other = null;
          if (getDomain().Formulas.TryGetValue(predicateName, out other))
          {
            if (other is AtomicFormula)
            {
              atomic_formula = new AtomicFormulaApplication((AtomicFormula)other, arguments);
            }
            else
            {
              atomic_formula = new AtomicFormulaApplication(new AtomicFormula(predicateName, 
                                                                              new List<ObjectParameterVariable>(),
                                                                              DescribedFormula.DefaultAttributes), 
                                                            arguments);
              this.m_mgr.logParserError("Ground predicate expected instead of \""
                          + other.ToTypedString() + "\".", this.m_file,
                          node.getLine(), node.getColumn());
            }
          }
          else
          {
            atomic_formula = new AtomicFormulaApplication(new AtomicFormula(predicateName, 
                                                                            new List<ObjectParameterVariable>(),
                                                                            DescribedFormula.DefaultAttributes), 
                                                          arguments);
            this.m_mgr.logParserError("Predicate \""
                          + atomic_formula.ToString()
                          + "\" undefined.", this.m_file, node.getLine(), node.getColumn());
          }

          return atomic_formula;
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    /**
     * Extracts the object structures from the <code>TERM</code> node of the
     * syntaxic tree and implements the semantic check.
     * 
     * @param node the <code>TERM</code> node.
     * @return the exp structure built.
     * @throws ParserException if an error occurs while parsing.
     */
    private ITerm term(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTCONSTANT:
            return this.constant(cn);
          case LexerTreeConstants.JJTVARIABLE:
            return this.object_variable(cn);
          case LexerTreeConstants.JJTFUNCTION_TERM_OR_CONSTANT:
            return this.function_term_or_constant(cn);
          default:
            throw new ParserException("An internal parser error occurs: node "
                                    + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                              + node.getLabel() + " unexpected.");
    }

    private ITerm function_term(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        switch (cn.getId())
        {
          case LexerTreeConstants.JJTFUNCTION_HEAD:
            DummyFormulaApplication dummyFormula = function_head(cn);
            ITerm fluent = getFormulaApplication<ITerm>(dummyFormula, false, cn);
            if (fluent == null)
              fluent = new ObjectFluentApplication(new ObjectFluent(dummyFormula.Name, 
                                                                    new List<ObjectParameterVariable>(), 
                                                                    getDomain().TypeSetSet.Object,
                                                                    DescribedFormula.DefaultAttributes),
                                                    dummyFormula.GetArguments());
            return fluent;
          default:
            throw new ParserException("An internal parser error occurs: node "
                        + cn.getLabel() + " unexpected.");
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private ITerm function_term_or_constant(SimpleNode node)
    {
      if (node.jjtGetNumChildren() == 1)
      {
        SimpleNode cn = (SimpleNode)node.jjtGetChild(0);
        if (cn.getId() == LexerTreeConstants.JJTFUNCTION_HEAD_OR_FUNCTOR)
        {
          GetFluentError error;
          DummyFormulaApplication dummyFormula = function_head_or_functor(cn);
          ITerm formula = getFormulaApplication<ITerm>(dummyFormula, true, true, cn, out error);
          if (formula != null)
          {
            return formula;
          }
          else if (error.Type == GetFluentError.ErrorType.UNDEFINED && dummyFormula.MayBeConstant)
          {
            Constant constant = GetConstant(dummyFormula.Name);
            if (constant != null)
              return constant;
          }

          string errorMsg = error.Message;
          if (error.Type == GetFluentError.ErrorType.UNDEFINED)
          {
            errorMsg = string.Format("{0} \"{1}\" undefined.",
                                     dummyFormula.MayBeConstant ? "Function or constant" : "Function",
                                     dummyFormula.ToTypedString());
          }

          this.m_mgr.logParserError(errorMsg, this.m_file, node.getLine(), node.getColumn());

          return new ObjectFluentApplication(new ObjectFluent(dummyFormula.Name, 
                                                              new List<ObjectParameterVariable>(), 
                                                              getDomain().TypeSetSet.Object,
                                                              DescribedFormula.DefaultAttributes),
                                                      dummyFormula.GetArguments());
        }
      }
      throw new ParserException("An internal parser error occurs: node "
                  + node.getLabel() + " unexpected.");
    }

    private T getFormulaApplication<T>(FormulaApplication formula, bool ignoreNotFoundErrors, SimpleNode node)
      where T : class
    {
      GetFluentError error;
      return getFormulaApplication<T>(formula, ignoreNotFoundErrors, false, node, out error);
    }

    private T getFormulaApplication<T>(FormulaApplication formula, bool ignoreNotFoundErrors, bool ignoreMatchErrors, SimpleNode node, out GetFluentError error)
      where T : class
    {
      T t = null;
      RootFormula existing;
      error = new GetFluentError();
      if (getDomain().Formulas.TryGetValue(formula.Name, out existing))
      {
        if (existing.Match(formula))
        {
          FormulaApplication formulaApplication = existing.Instantiate(formula.GetArguments());
          if (formulaApplication is T)
          {
            return (T)(object)formulaApplication;
          }
          else
          {
            error = new GetFluentError(GetFluentError.ErrorType.WRONG_TYPE,
                           "Function \""
                           + formula.ToTypedString()
                           + "\" is invalid; \"" + typeof(T).ToString() + "\" was expected instead\""
                           + existing.ToTypedString() + "\".");
            if (!ignoreNotFoundErrors)
              this.m_mgr.logParserError(error.Message, this.m_file, node.getLine(), node.getColumn());
          }
        }
        else
        {
          error = new GetFluentError(GetFluentError.ErrorType.DOES_NOT_MATCH,
                      "Function \""
                      + formula.ToTypedString()
                      + "\" does not match existing function \""
                      + existing.ToTypedString() + "\".");

          if (!ignoreMatchErrors)
            this.m_mgr.logParserError(error.Message, this.m_file, node.getLine(), node.getColumn());

          FormulaApplication formulaApplication = existing.Instantiate(formula.GetArguments());
          if (formulaApplication is T)
            t = (T)(object)formulaApplication;
        }
      }
      else
      {
        error = new GetFluentError(GetFluentError.ErrorType.UNDEFINED,
                                   "Function \"" + formula.ToString() + "\" undefined.");
        if (!ignoreNotFoundErrors)
          this.m_mgr.logParserError(error.Message, this.m_file, node.getLine(), node.getColumn());
      }
      return t;
    }

    private T getExistingDefinedFormula<T>(T formula, SimpleNode node)
      where T : DefinedFormula
    {
      T t = null;
      RootFormula existing;
      if (getDomain().Formulas.TryGetValue(formula.Name, out existing))
      {
        if (existing is DefinedFormula)
        {
          DefinedFormula other = (DefinedFormula)existing;
          if (other.Match(formula))
          {
            t = (T)(object)other;
          }
          else
          {
            this.m_mgr.logParserError("Formula \""
                                   + formula.ToTypedString()
                                   + "\" does not match existing defined formula \""
                                   + other.ToTypedString() + "\".",
                                   this.m_file, node.getLine(), node.getColumn());
          }
        }
        else
        {
          this.m_mgr.logParserError("Formula \""
                                 + formula.ToTypedString()
                                 + "\" has the same name as the existing formula \""
                                 + existing.ToTypedString() + "\".",
                                 this.m_file, node.getLine(), node.getColumn());
        }
      }
      return t;
    }

    private Constant GetConstant(string name)
    {
      Constant constant;

      if (!this.m_obj.Constants.TryGetValue(name, out constant) &&
          !this.getDomain().Constants.TryGetValue(name, out constant))
      {
        return null;
      }

      return constant;
    }
  }
}

