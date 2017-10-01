using Toffee.Core.Parser.Definitions;

namespace Toffee.Compiler.Writer
{
    public class ToffeeDataGatherer
    {
        public ToffeeWriter Writer { get; private set; }
        public TdlFile File { get; private set; }

        public ToffeeDataGatherer(ToffeeWriter writer, TdlFile file)
        {
            Writer = writer;
            File = file;
        }
    }
}
