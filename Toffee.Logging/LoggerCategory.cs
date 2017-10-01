namespace Toffee.Logging
{
    public class LoggerCategory
    {
        public string Name { get; private set; }
        public Logger Logger { get; private set; }

        public LoggerCategory(string name, Logger logger)
        {
            Name = name;
            Logger = logger;
        }

        private void Log(LoggerSeverity severity, string message, params object[] args)
        {
            string severityStr = severity.ToString().ToUpper();
            message = string.Format("[{0}][{1}]: {2}", Name, severityStr, message);
            Logger.Log(message, args);
        }

        public void Info(string message, params object[] args)
        {
            Log(LoggerSeverity.Info, message, args);
        }

        public void Debug(string message, params object[] args)
        {
            bool debug = Logger.ForceDebug;
#if DEBUG
            debug = true;
#endif
            if (debug)
                Log(LoggerSeverity.Debug, message, args);
        }

        public void Warning(string message, params object[] args)
        {
            Log(LoggerSeverity.Warning, message, args);
        }

        public void Error(string message, params object[] args)
        {
            Log(LoggerSeverity.Error, message, args);
        }
    }
}
