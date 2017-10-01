using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public interface IGeneratorCommand
    {
        void Execute(ToffeeWriter writer);
    }
}
