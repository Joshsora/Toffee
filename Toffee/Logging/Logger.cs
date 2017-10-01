using System.Collections.Generic;

namespace Toffee.Logging
{
    public class Logger
    {
        /// <summary>
        /// The outputs that this Logger logs to.
        /// </summary>
        private List<ILoggerOutput> Outputs { get; set; }

        /// <summary>
        /// If true, then Debug severities will output regardless of build configuration.
        /// </summary>
        public bool ForceDebug { get; private set; }

        /// <summary>
        /// Creates a new Logger.
        /// </summary>
        public Logger(bool forceDebug = false)
        {
            Outputs = new List<ILoggerOutput>();
            ForceDebug = forceDebug;
        }

        /// <summary>
        /// Logs the given message through all outputs.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The formatting arguments.</param>
        public void Log(string message, params object[] args)
        {
            foreach (ILoggerOutput output in Outputs)
                output.Log(message, args);
        }

        /// <summary>
        /// Adds an output to this logger.
        /// </summary>
        /// <param name="output">The output.</param>
        public void AddOutput(ILoggerOutput output)
        {
            if (Outputs.Contains(output))
                return;
            Outputs.Add(output);
        }

        /// <summary>
        /// Makes a logging category.
        /// </summary>
        /// <param name="category">The name of the category.</param>
        /// <returns>The created category.</returns>
        public LoggerCategory MakeCategory(string category)
        {
            return new LoggerCategory(category, this);
        }
    }
}
