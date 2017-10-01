using System;
using System.Linq;
using Toffee.Core;
using Toffee.Compiler.Writer;
using Toffee.Core.Parser.Definitions;

namespace ToffeeCompiler.Gatherers
{
    [ToffeeDataGatherer("csharp")]
    public class CSharpDataGatherer : ToffeeDataGatherer
    {
        public TdlObject[] AllObjects { get; private set; }
        private int CurrentObjectIndex { get; set; }
        private TdlObject CurrentObject { get; set; }

        private TdlStruct CurrentStruct
        {
            get
            {
                return (TdlStruct)CurrentObject;
            }

            set
            {
                CurrentObject = value;
            }
        }

        private TdlClass CurrentClass
        {
            get
            {
                return (TdlClass)CurrentObject;
            }

            set
            {
                CurrentObject = value;
            }
        }

        private TdlService CurrentService
        {
            get
            {
                return (TdlService)CurrentObject;
            }

            set
            {
                CurrentObject = value;
            }
        }

        private TdlField CurrentField { get; set; }
        private int CurrentModifierIndex { get; set; }
        private int CurrentParameterIndex { get; set; }

        private int CurrentPropertyIndex { get; set; }
        private TdlProperty CurrentProperty
        {
            get
            {
                return (TdlProperty)CurrentField;
            }

            set
            {
                CurrentField = value;
            }
        }

        private int CurrentMethodIndex { get; set; }
        private TdlMethod CurrentMethod
        {
            get
            {
                return (TdlMethod)CurrentField;
            }
            
            set
            {
                CurrentField = value;
            }
        }

        private int CurrentRequiredNamespaceIndex { get; set; }
        private string[] RequiredNamespaces { get; set; }

        private bool Done
        {
            get
            {
                return (CurrentObjectIndex >= AllObjects.Length);
            }
        }

        public int Initial
        {
            get
            {
                Writer.AddData("NETWORK_NAME", File.NetworkName);

                // Undefine the defines from last run
                Writer.Undefine("HAS_INITIAL_DATA");
                Writer.Undefine("HAS_NAMESPACE");
                Writer.Undefine("TOFFEE_STRUCTURE");
                Writer.Undefine("TOFFEE_OBJECT");
                Writer.Undefine("TOFFEE_SERVICE");
                Writer.Undefine("MORE_DATA_AVAILABLE");
                Writer.Undefine("HAS_PROPERTY");
                Writer.Undefine("MORE_PROPERTIES");
                Writer.Undefine("HAS_METHOD");
                Writer.Undefine("HAS_MORE_METHODS");
                if (Done)
                    return 0;

                // Are there still more objects to generate?
                if (CurrentObjectIndex < AllObjects.Length)
                {
                    // Get the object
                    CurrentObject = AllObjects[CurrentObjectIndex++];

                    // Make sure we're using the right namespaces for the properties
                    RequiredNamespaces = CurrentObject.RequiredNamespaces;
                    if (RequiredNamespaces.Length != 0)
                        Writer.Define("HAS_REQUIRED_NAMESPACE");

                    // Does this object reside in a namespace?
                    string namespaceIdentifier = CurrentObject.Namespace.FullName;
                    if (namespaceIdentifier != "")
                    {
                        // Set this object's namespace data
                        Writer.Define("HAS_NAMESPACE");
                        Writer.AddData("NAMESPACE", namespaceIdentifier);
                        Writer.AddData("FOLDER", namespaceIdentifier.Replace('.', '/'));
                    }

                    // Set the data for this object
                    Writer.Define("HAS_INITIAL_DATA");
                    if (CurrentObject.GetType() == typeof(TdlStruct))
                        Writer.Define("TOFFEE_STRUCTURE");
                    else if (CurrentObject.GetType() == typeof(TdlClass))
                        Writer.Define("TOFFEE_OBJECT");
                    else
                        Writer.Define("TOFFEE_SERVICE");
                    Writer.AddData("IDENTIFIER", CurrentObject.Identifier);
                    CurrentPropertyIndex = 0;
                    CurrentMethodIndex = 0;
                    CurrentModifierIndex = 0;
                    CurrentParameterIndex = 0;
                }
                else
                    Writer.Undefine("HAS_INITIAL_DATA");

                // Is there more data?
                if (!Done)
                    Writer.Define("MORE_DATA_AVAILABLE");
                return 0;
            }
        }

        public int NextProperty
        {
            get
            {
                // Remove 'MODIFIER' so it doesn't persist across fields
                Writer.RemoveData("MODIFIER");

                // If the current object is a service, don't do any of this
                if (CurrentObject.GetType() == typeof(TdlService))
                    return 0;

                // Undefine the defines from last run
                Writer.Undefine("HAS_PROPERTY");
                Writer.Undefine("PROPERTY_IS_ARRAY");
                Writer.Undefine("MORE_PROPERTIES");
                CurrentModifierIndex = 0;

                // Are there any properties?
                if (CurrentPropertyIndex < CurrentStruct.Properties.Length)
                {
                    // Get the current property, and set the data
                    CurrentProperty = CurrentStruct.Properties[CurrentPropertyIndex++];
                    Writer.Define("HAS_PROPERTY");
                    Writer.AddData("PROPERTY_TYPE", CurrentProperty.Type.ToString());
                    Writer.AddData("PROPERTY_NAME", CurrentProperty.Identifier);
                    if (CurrentProperty.Type.Array)
                        Writer.Define("PROPERTY_IS_ARRAY");
                    Writer.AddData("READ_METHOD", CurrentProperty.Type.ReadMethod);

                    // Do we need to define the property number?
                    if (Writer.Defines.Contains("TOFFEE_STRUCTURE"))
                        Writer.AddData("STRUCT_PROPERTY_NUMBER", CurrentPropertyIndex.ToString());

                    // Are there any more properties after this one?
                    if (CurrentPropertyIndex < CurrentStruct.Properties.Length)
                        Writer.Define("MORE_PROPERTIES");
                    else
                        CurrentPropertyIndex = 0;
                }
                return 0;
            }
        }

        public int NextMethod
        {
            get
            {
                // Remove these so it doesn't persist across fields
                Writer.RemoveData("MODIFIER");
                Writer.RemoveData("PARAMETER_NAME");

                // If the current object is a struct, don't do any of this
                if (CurrentObject.GetType() == typeof(TdlStruct))
                    return 0;

                // Undefine the defines from last run
                Writer.Undefine("HAS_METHOD");
                Writer.Undefine("HAS_MORE_METHODS");
                Writer.Undefine("METHOD_HAS_PARAMETERS");

                TdlMethod[] methods;
                if (CurrentObject.GetType() == typeof(TdlClass))
                    methods = CurrentClass.Methods;
                else
                    methods = CurrentService.Methods;

                // Are there any methods?
                if (CurrentMethodIndex < methods.Length)
                {
                    // Get the current method, and set the data
                    CurrentMethod = methods[CurrentMethodIndex++];
                    Writer.Define("HAS_METHOD");
                    if (CurrentMethod.Parameters.Length > 0)
                        Writer.Define("METHOD_HAS_PARAMETERS");
                    Writer.AddData("METHOD_NAME", CurrentMethod.Identifier);

                    // Are there any more methods after this one?
                    if (CurrentMethodIndex < methods.Length)
                        Writer.Define("HAS_MORE_METHODS");
                }
                return 0;
            }
        }

        public int RequiredNamespace
        {
            get
            {
                // Undefine the defines from last run
                Writer.Undefine("HAS_REQUIRED_NAMESPACE");
                Writer.Undefine("MORE_REQUIRED_NAMESPACE");

                // Are there any required namespaces?
                if (CurrentRequiredNamespaceIndex < RequiredNamespaces.Length)
                {
                    Writer.Define("HAS_REQUIRED_NAMESPACE");
                    Writer.AddData("REQUIRED_NAMESPACE", RequiredNamespaces[CurrentRequiredNamespaceIndex++]);

                    if (CurrentRequiredNamespaceIndex < RequiredNamespaces.Length)
                        Writer.Define("MORE_REQUIRED_NAMESPACE");
                }

                return 0;
            }
        }

        public int NextModifier
        {
            get
            {
                Writer.Undefine("HAS_MODIFIER");
                Writer.Undefine("LAST_MODIFIER");

                int[] modifiers = Enum.GetValues(typeof(ToffeeModifiers))
                    .Cast<int>()
                    .Where(f => ((f & (int)CurrentField.Modifiers) == f) & (f != 0))
                    .ToArray();
                if (CurrentModifierIndex < modifiers.Length)
                {
                    Writer.Define("HAS_MODIFIER");
                    Writer.AddData("MODIFIER", ((ToffeeModifiers)modifiers[CurrentModifierIndex++]).ToString());
                    if (CurrentModifierIndex == modifiers.Length)
                        Writer.Define("LAST_MODIFIER");
                }
                return 0;
            }
        }

        public int NextParameter
        {
            get
            {
                Writer.Undefine("HAS_PARAMETER");
                Writer.Undefine("LAST_PARAMETER");

                if (CurrentParameterIndex < CurrentMethod.Parameters.Length)
                {
                    Writer.Define("HAS_PARAMETER");
                    TdlParameter tdlParameter = CurrentMethod.Parameters[CurrentParameterIndex++];
                    Writer.AddData("PARAMETER_TYPE", tdlParameter.Type.ToString());
                    Writer.AddData("PARAMETER_NAME", tdlParameter.Identifier);
                    if (CurrentParameterIndex == CurrentMethod.Parameters.Length)
                    {
                        Writer.Define("LAST_PARAMETER");
                        CurrentParameterIndex = 0;
                    }
                }
                return 0;
            }
        }

        public CSharpDataGatherer(ToffeeWriter writer, TdlFile file) : base(writer, file)
        {
            AllObjects = file.AllObjects;
            CurrentObjectIndex = 0;
            CurrentPropertyIndex = 0;
        }
    }
}
