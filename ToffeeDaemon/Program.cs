using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Newtonsoft.Json;
using Ookii.CommandLine;

using Toffee.Server;
using Toffee.Server.Roles;
using Toffee.Logging;

namespace ToffeeDaemon
{
    public static class Program
    {
        public static Logger Logger { get; private set; }
        public static LoggerCategory DaemonLog { get; private set; }

        public static ToffeeDaemonArguments Arguments { get; private set; }
        public static ToffeeDaemonConfiguration Configuration { get; private set; }
        private static List<IRole> Roles { get; set; }

        public static bool InDebug { get; private set; }

        static void Main(string[] args)
        {
#if DEBUG
            if (args.Length == 0)
            {
                args = new string[]
                {
                "config.json"
                };
            }
#endif
            // First, let's parse the arguments we have
            CommandLineParser argsParser = new CommandLineParser(typeof(ToffeeDaemonArguments));
            try
            {
                Arguments = (ToffeeDaemonArguments)argsParser.Parse(args);
            }
            catch (CommandLineArgumentException e)
            {
                Console.WriteLine(e.Message);
                argsParser.WriteUsageToConsole();
                return;
            }

            // Now, let's load the configuration
            try
            {
                string configJson = File.ReadAllText(Arguments.ConfigurationFile);
                Configuration = JsonConvert.DeserializeObject<ToffeeDaemonConfiguration>(configJson);
            }
            catch (IOException)
            {
                Console.WriteLine("Configuration could not be found, or opened.");
                return;
            }
            catch (JsonException e)
            {
                Console.WriteLine("Configuration is invalid JSON.");
                Console.WriteLine(e.Message);
                return;
            }

            // Setup debug
#if DEBUG
            InDebug = true;
#else
            InDebug = Configuration.Debug;
#endif

            // Read the logger configuration
            Logger = new Logger(InDebug);
            foreach (LoggerConfiguration loggerConfig in Configuration.Loggers)
            {
                switch (loggerConfig.Kind.ToLower())
                {
                    case "console":
                        Logger.AddOutput(new LoggerConsoleOutput());
                        break;

                    default:
                        Console.WriteLine("Unknown logger kind: '{0}'", loggerConfig.Kind);
                        return;
                }
            }

            // Are we in debug without a logger?
            if ((InDebug) && (Configuration.Loggers.Length == 0))
                Logger.AddOutput(new LoggerConsoleOutput());

            // Create the daemon's category
            DaemonLog = Logger.MakeCategory("Daemon");
            DaemonLog.Info("Loaded configuration successfully!");

            // Next, load all the assemblies into the current domain
            if (Configuration.Assemblies.Length > 0)
                DaemonLog.Info("Loading assemblies...");
            foreach (string assembly in Configuration.Assemblies)
            {
                try
                {
                    DaemonLog.Info("Loading assembly: {0}", assembly);
                    AppDomain.CurrentDomain.Load(assembly);
                }
                catch
                {
                    DaemonLog.Error("Could not load assembly: {0}", assembly);
                    return;
                }
            }
            if (Configuration.Assemblies.Length > 0)
                DaemonLog.Info("Loaded {0} assemblies.", Configuration.Assemblies.Length);

            // Build a lookup for the roles we have loaded
            ToffeeRole.BuildRoleLookup(DaemonLog);

            // Create instances of the roles from the configuration
            if (Configuration.Roles.Length > 0)
                DaemonLog.Info("Creating role instances...");
            Roles = new List<IRole>();
            foreach (ToffeeRoleConfiguration roleConfig in Configuration.Roles)
            {
                // Does the role exist?
                if (!ToffeeRole.HasRole(roleConfig.Name))
                {
                    DaemonLog.Error("Unknown role: '{0}'. Aborting.", roleConfig.Name);
                    return;
                }

                // Get the roleType, and create an instance
                DaemonLog.Info("Creating instance of role: {0}.", roleConfig.Name);
                Type roleType = ToffeeRole.GetRole(roleConfig.Name);
                IRole roleInstance = (IRole)Activator.CreateInstance(roleType);
                if (!roleInstance.Configure(roleConfig.Config, Logger))
                {
                    DaemonLog.Error("Failed to configure role: '{0}'", roleConfig.Name);
                    return;
                }
                Roles.Add(roleInstance);
                roleInstance.Start();
            }

            // Is this daemon doing anything?
            if (Roles.Count == 0)
            {
                DaemonLog.Error("This daemon isn't doing anything. Aborting.");
                return;
            }
            else
                DaemonLog.Info("Running {0} roles!", Roles.Count);

            // Tick!
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();
                foreach (IRole role in Roles)
                    role.Tick();
                stopwatch.Stop();
                Thread.Sleep(Math.Max(25, Configuration.TickRate - (int)stopwatch.ElapsedMilliseconds));
            }
        }
    }
}
