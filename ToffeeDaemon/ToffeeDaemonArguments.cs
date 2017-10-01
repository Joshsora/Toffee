using Ookii.CommandLine;

namespace ToffeeDaemon
{
    public class ToffeeDaemonArguments
    {
        [CommandLineArgument(Position = 0, IsRequired = true)]
        public string ConfigurationFile { get; set; }

        [CommandLineArgument(DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}
