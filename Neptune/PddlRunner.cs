using System;
using System.IO;
using PDDLParser.Parser;
using TLPlan;
using GUI;

namespace Neptune
{
    class PddlRunner
    {
        const string pddlRoot = "../../pddl";
        private static int runCounter = 0;
        private int runCount;
        private string name;
        private string dfilename;
        private string pfilename;
        private DateTime started;
        private DateTime ended;
        private Planner planner;
        private Parser parser;
        private StringWriter errorLog;
        private StringWriter traceLog;
        private Statistics statistics;
        private TLPlanOptions options;
        private Plan plan;

        public PddlRunner(string name, string dir, string domainFile, string problemFile)
        {
            runCount = runCounter++;
            plan = null;
            Name = name;
            DomainFile = String.Format("{0}/{1}.pddl", dir, domainFile);
            ProblemFile = String.Format("{0}/{1}.pddl", dir, problemFile);
            Started = new DateTime();
            Ended = new DateTime();
            errorLog = new StringWriter();
            traceLog = new StringWriter();
            options = new TLPlanOptions();
            statistics = new Statistics();
            options.SetDomain(String.Format("{0}/{1}", pddlRoot, DomainFile));
            options.SetProblem(String.Format("{0}/{1}", pddlRoot, ProblemFile));
            planner = new Planner(options, statistics, errorLog, traceLog);
            parser = new Parser(new ErrorManager(errorLog, errorLog));
            planner.PlanningFinished += new EventHandler<Planner.PlanningFinishedEventArgs>(planner_PlanningFinished);
        }

        void planner_PlanningFinished(object sender, Planner.PlanningFinishedEventArgs e)
        {
            if (e.ProblemSolved)
                plan = e.Plan;
        }

        public string TraceReport()
        {
            StringWriter pw = new StringWriter();
            if (plan == null)
                pw.WriteLine("No Plan Found");
            else
            {
                pw.WriteLine("Operators:");
                plan.PrintOperators(pw);
                pw.WriteLine("\nOrder:");
                plan.PrintOrder(pw);
                pw.WriteLine("\nPlan:");
                plan.PrintPlan(pw);
                pw.Write("Plan cost: ");
                plan.PrintMetric(pw);
            }
            StringWriter sw = new StringWriter();
            sw.WriteLine(String.Format("PDDL [{0}] Report Begin ======================", Name));
            sw.WriteLine(String.Format("          domain: {0}", DomainFile));
            sw.WriteLine(String.Format("         problem: {0}", ProblemFile));
            sw.WriteLine(String.Format("         started: {0:HH:mm:ss}", Started));
            sw.WriteLine(String.Format("           ended: {0:HH:mm:ss}", Ended));
            sw.WriteLine(String.Format("      parse time: {0}", Statistics.ParseTime));
            sw.WriteLine(String.Format("      solve time: {0}", Statistics.SolveTime));
            sw.WriteLine(String.Format("      open count: {0}", Statistics.OpenCount));
            sw.WriteLine(String.Format("  filtered count: {0}", Statistics.FilteredOperatorCount));
            sw.WriteLine(String.Format("  operator count: {0}", Statistics.OperatorCount));
            sw.WriteLine(String.Format(" successor count: {0}", Statistics.SuccessorCount));
            sw.WriteLine(String.Format("  examined count: {0}", Statistics.ExaminedNodeCount));
            sw.WriteLine(String.Format(" search strategy: {0}", Statistics.Options.SearchStrategy.ToString()));
            sw.WriteLine(String.Format("Plan Begin ----------------------------------"));
            sw.WriteLine(pw.ToString().Trim());
            sw.WriteLine(String.Format("Plan End ------------------------------------"));
            sw.WriteLine(String.Format("Trace Begin ---------------------------------"));
            string trace = Trace().Trim();
            if (trace.Length > 0)
                sw.WriteLine(trace);
            sw.WriteLine(String.Format("Trace End -----------------------------------"));
            sw.WriteLine(String.Format("PDDL [{0}] Report End ========================", Name));
            return sw.ToString();
        }

        public string Errors()
        {
            return errorLog.ToString();
        }

        private string Trace()
        {
            return traceLog.ToString();
        }

        public int RunCount()
        {
            return runCount;
        }

        public string Name
        {
            get
            {
                return String.Format("{0}-({1})", name, runCount);
            }
            set
            {
                name = value;
            }
        }

        public string DomainFile
        {
            get
            {
                return dfilename;
            }
            set
            {
                dfilename = value;
            }
        }

        public string ProblemFile
        {
            get
            {
                return pfilename;
            }
            set
            {
                pfilename = value;
            }

        }

        public Statistics Statistics
        {
            get
            {
                return statistics;
            }
        }

        public DateTime Started
        {
            get
            {
                return started;
            }
            set
            {
                started = value;
            }
        }

        public DateTime Ended
        {
            get
            {
                return ended;
            }
            set
            {
                ended = value;
            }
        }

        public void Solve()
        {
            Started = DateTime.Now;
            planner.Statistics.Reset();
            planner.Statistics.StartParse();
            PDDLObject problem = null;

            try
            {
                PDDLObject domain = parser.parse(planner.Options.Domain);
                problem = parser.parse(planner.Options.Problem);
                if (domain != null && problem != null)
                {
                    problem = parser.link(domain, problem);
                }
            }
            catch { }

            planner.Statistics.StopParse();

            ErrorManager mgr = parser.getErrorManager();
            mgr.clear();

            if (mgr.Contains(ErrorManager.Message.ERROR))
            {
                problem = null;

            }
            if (problem != null)
            {
                planner.Solve(problem);
            }
            Ended = DateTime.Now;
        }

    }

}
