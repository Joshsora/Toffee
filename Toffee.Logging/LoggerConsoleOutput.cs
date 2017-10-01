using System;

namespace Toffee.Logging
{
    public class LoggerConsoleOutput : ILoggerOutput
    {
        public void Log(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}
