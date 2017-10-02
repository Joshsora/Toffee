using Ookii.CommandLine;

namespace ToffeeCompiler
{
    public class ToffeeCompilerArguments
    {
        [CommandLineArgument(Position = 0, IsRequired = true)]
        public string GeneratorScript { get; set; }

        [CommandLineArgument(Position = 1, IsRequired = true)]
        public string InputFile { get; set; }

        [CommandLineArgument(DefaultValue = "")]
        public string OutputDirectory { get; set; }

        [CommandLineArgument(DefaultValue = "")]
        public string SkipNamespaceDirectory { get; set; }

        [CommandLineArgument(DefaultValue = "")]
        public string BaseNamespace { get; set; }

        [CommandLineArgument(DefaultValue = false)]
        public bool Verbose { get; set; }

        [CommandLineArgument(DefaultValue = false)]
        public bool VerboseGenerator { get; set; }

        [CommandLineArgument(DefaultValue = false)]
        public bool VerboseParser { get; set; }

        [CommandLineArgument(DefaultValue = false)]
        public bool VerboseWriter { get; set; }
    }
}
