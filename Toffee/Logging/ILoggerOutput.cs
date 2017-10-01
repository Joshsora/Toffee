namespace Toffee.Logging
{
    public interface ILoggerOutput
    {
        void Log(string message, params object[] args);
    }
}
