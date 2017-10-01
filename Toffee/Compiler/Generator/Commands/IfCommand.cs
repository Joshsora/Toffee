using Toffee.Compiler.Writer;
using System.Collections.Generic;

namespace Toffee.Compiler.Generator.Commands
{
    public class IfCommand : IGeneratorCommand
    {
        public string Define { get; private set; }
        public bool Not { get; private set; }
        public List<IGeneratorCommand> TrueCode { get; private set; }
        public List<IGeneratorCommand> FalseCode { get; private set; }         
        public bool WasElseIf { get; private set; }

        public IfCommand(string define, bool not = false)
        {
            Define = define;
            Not = not;
            TrueCode = new List<IGeneratorCommand>();
            FalseCode = new List<IGeneratorCommand>();
        }

        public void Execute(ToffeeWriter writer)
        {
            if (Not)
            {
                writer.Log("Checking if '{0}' is not defined", Define);
                if (!writer.Defines.Contains(Define))
                {
                    writer.Log("'{0}' was not defined, entering TrueCode", Define);
                    foreach (IGeneratorCommand command in TrueCode)
                        command.Execute(writer);
                }
                else
                {
                    writer.Log("'{0}' was defined, entering FalseCode", Define);
                    foreach (IGeneratorCommand command in FalseCode)
                        command.Execute(writer);
                }
            }
            else
            {
                writer.Log("Checking if '{0}' is defined", Define);
                if (writer.Defines.Contains(Define))
                {
                    writer.Log("'{0}' was defined, entering TrueCode", Define);
                    foreach (IGeneratorCommand command in TrueCode)
                        command.Execute(writer);
                }
                else
                {
                    writer.Log("'{0}' was not defined, entering FalseCode", Define);
                    foreach (IGeneratorCommand command in FalseCode)
                        command.Execute(writer);
                }
            }
            writer.Log("Left Code for '{0}'", Define);
        }

        [GeneratorCommandParser("if")]
        public static void Parse(ToffeeGenerator generator, string[] args)
        {
            // Do we have the correct arguments?
            if (args.Length != 1)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'if' command.");
            IfCommand command = new IfCommand(args[0]);
            ToffeeGeneratorStackFrame handle = new ToffeeGeneratorStackFrame("if");
            handle.State = command;
            command.TrueCode = handle.Commands;
            generator.AddCommand(command);
            generator.Stack.Push(handle);

            generator.Log("Enter If Block (Condition: {0})", args[0]);
        }

        [GeneratorCommandParser("ifn")]
        public static void ParseNot(ToffeeGenerator generator, string[] args)
        {
            // Do we have the correct arguments?
            if (args.Length != 1)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'if' command.");
            IfCommand command = new IfCommand(args[0], true);
            ToffeeGeneratorStackFrame handle = new ToffeeGeneratorStackFrame("if");
            handle.State = command;
            command.TrueCode = handle.Commands;
            generator.AddCommand(command);
            generator.Stack.Push(handle);

            generator.Log("Enter If Block (Condition: {0})", args[0]);
        }

        [GeneratorCommandParser("endif")]
        public static void ParseEnd(ToffeeGenerator generator, string[] args)
        {
            // Could we even be in an if block?
            if (generator.Stack.Count < 2)
                throw new ToffeeGeneratorException("Encountered 'endif' command without being inside an if block.");
            // Are we in an if block?
            if (generator.Stack.Peek().Name != "if")
                throw new ToffeeGeneratorException("Encountered 'endif' command instead of 'endpattern'");

            // Get the last stack handle (The If block)
            ToffeeGeneratorStackFrame handle = generator.Stack.Pop();

            // Get the If command
            IfCommand command = (IfCommand)handle.State;
            
            // Was this IfCommand created via an ElseIf?
            // Then, we need to end the Else that was before this If
            if (command.WasElseIf)
                ParseEnd(generator, null);

            generator.Log("Exit If Block (Condition: {0}, TrueSize: {1}, FalseSize: {2})",
                command.Define, command.TrueCode.Count, command.FalseCode.Count);
        }

        [GeneratorCommandParser("else")]
        public static void ParseElse(ToffeeGenerator generator, string[] args)
        {
            // Could we even be in an if block?
            if (generator.Stack.Count < 2)
                throw new ToffeeGeneratorException("Encountered 'else' command without being inside an if block.");
            // Are we in an if block?
            if (generator.Stack.Peek().Name != "if")
                throw new ToffeeGeneratorException("Encountered 'else' command instead of 'endpattern'");

            // Get the last stack handle (The If block)
            ToffeeGeneratorStackFrame handle = generator.Stack.Pop();

            // Create an else handle (This will be the Else block)
            ToffeeGeneratorStackFrame elseHandle = new ToffeeGeneratorStackFrame("if");

            // Set the state of the Else handle to the same state as the If handle
            // (They are both part of the same IfCommand)
            elseHandle.State = handle.State;
            
            // Get the original If command, and set the ConditionFalse to the Else handle's commands
            ((IfCommand)elseHandle.State).FalseCode = elseHandle.Commands;

            // Push the else handle onto the stack
            generator.Stack.Push(elseHandle);

            generator.Log("Exit If, Enter Else Block (Condition: {0})", ((IfCommand)handle.State).Define);
        }

        [GeneratorCommandParser("elseif")]
        public static void ParseElseIf(ToffeeGenerator generator, string[] args)
        {
            // Do we have the correct arguments?
            if (args.Length != 1)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'elseif' command.");
            // Could we even be in an if block?
            if (generator.Stack.Count < 2)
                throw new ToffeeGeneratorException("Encountered 'elseif' command without being inside an if block.");
            // Are we in an if block?
            if (generator.Stack.Peek().Name != "if")
                throw new ToffeeGeneratorException("Encountered 'elseif' command instead of 'endpattern'");

            // Get the last stack handle (The If block)
            ToffeeGeneratorStackFrame handle = generator.Stack.Pop();

            // Create an else handle (This will be the ElseIf block)
            ToffeeGeneratorStackFrame elseHandle = new ToffeeGeneratorStackFrame("if");

            // Set the state of the Else handle to the same state as the If handle
            // (They are both part of the same IfCommand)
            elseHandle.State = handle.State;

            // Get the original If command, and set the ConditionFalse to the Else handle's commands
            IfCommand command = (IfCommand)handle.State;
            command.FalseCode = elseHandle.Commands;

            // Push the else handle onto the stack
            generator.Stack.Push(elseHandle);

            generator.Log("Exit If, Enter Else Block (Condition: {0})", ((IfCommand)handle.State).Define);

            // Parse the "elseif" as if it was an "if"
            // The IfCommand will be placed into the Else block for this If, creating an Else If
            Parse(generator, args);

            // The IfCommand that was just added onto the stack has created via ElseIf
            ((IfCommand)generator.Stack.Peek().State).WasElseIf = true;
        }
    }
}
