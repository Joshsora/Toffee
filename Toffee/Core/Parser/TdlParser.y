/*
	Parser written for Toffee Definition Language
	by Joshsora (Joshua Scott)
*/
%using Toffee.Core;
%using Toffee.Core.Parser.Definitions;

%namespace Toffee.Core.Parser

%{
	public bool Verbosity { get; private set; }

	private TdlType ParsedType { get; set; }
	private ToffeeModifiers ParsedModifiers { get; set; }

	public TdlFile DefinitionFile { get; private set; }
	private TdlNamespace CurrentNamespace { get; set; }
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

	private TdlMethod CurrentMethod { get; set; }

	private string ObjectKind
	{
		get
		{
			if (CurrentObject.GetType() == typeof(TdlStruct))
				return "struct";
			else if (CurrentObject.GetType() == typeof(TdlClass))
				return "class";
			else if (CurrentObject.GetType() == typeof(TdlService))
				return "service";
			return "";
		}
	}
%}

%start file

%union {
	public string String;
	public ToffeeValueType Type;
	public ToffeeModifiers Modifier;
}

// Tokens
%token 				COMMENT
%token <String> 	IDENTIFIER
%token <String> 	STRING_LITERAL
%token <String> 	FILENAME
%token <Type>		TYPE
%token <Modifier>	MODIFIER
%token				IMPORT
%token				NETWORK
%token 				NAMESPACE
%token				ENUM
%token				CLASS
%token				STRUCT
%token				SERVICE
%token				END_OF_STATEMENT
%token				BLOCK_START
%token				BLOCK_END
%token				PARAMS_START
%token				PARAMS_END
%token				ARRAY

// Rules
%%
file
	: 
	| file import
	| file NETWORK IDENTIFIER END_OF_STATEMENT
	{
		DefinitionFile.NetworkName = $3;
	}
	| file namespace
	| file definitions
	;
import
	: IMPORT FILENAME END_OF_STATEMENT
	{
		Log("Importing {0}...", $2);
		if (DefinitionFile.ParseFile($2  + ".tdl"))
			Log("Imported {0} successfully...", $2);
		else
			Scanner.yyerror("Could not import '{0}'.", $2);
	}
	| IMPORT IDENTIFIER END_OF_STATEMENT
	{
		Log("Importing {0}...", $2);
		if (DefinitionFile.ParseFile($2 + ".tdl"))
			Log("Imported {0} successfully...", $2);
		else
			Scanner.yyerror("Could not import '{0}'.", $2);
	}
	;
type
	: TYPE
	{
		ParsedType = new TdlType($1);
		Log("Parsed Type to {0}", ParsedType);
	}
	| TYPE ARRAY
	{
		ParsedType = new TdlType($1, true);
		Log("Parsed Type to {0}", ParsedType);
	}
	| IDENTIFIER
	{
		if (!DefinitionFile.IsStructDefined($1))
			Scanner.yyerror("Unknown Type '{0}' encountered while parsing {1} '{2}'", $1, ObjectKind, CurrentObject.Identifier);
		ParsedType = new TdlType(ToffeeValueType.Struct, DefinitionFile.FindStruct($1));
		Log("Parsed Type to {0}", ParsedType);
	}
	| IDENTIFIER ARRAY
	{
		if (!DefinitionFile.IsStructDefined($1))
			Scanner.yyerror("Unknown Type '{0}' encountered while parsing {1} '{2}'", $1, ObjectKind, CurrentObject.Identifier);
		ParsedType = new TdlType(ToffeeValueType.Struct, DefinitionFile.FindStruct($1), true);
		Log("Parsed Type to {0}", ParsedType);
	}
	;
namespace
	: NAMESPACE IDENTIFIER BLOCK_START
	{
		CurrentNamespace = CurrentNamespace.AddNamespace($2);
		Log("Entering Namespace '{0}'", $2);
	}
	  definitions BLOCK_END
	{
		Log("Leaving Namespace '{0}'", CurrentNamespace.FullName);
		CurrentNamespace = DefinitionFile;
	}
	;
definitions
	: 
	| definitions enum_definition
	| definitions struct_definition
	| definitions class_definition
	| definitions service_definition
	;
enum_definition
	: ENUM IDENTIFIER BLOCK_START BLOCK_END
	;
struct_definition
	:
	| STRUCT IDENTIFIER BLOCK_START
	{
		if (CurrentNamespace == null)
			Scanner.yyerror("Tried to define struct '{0}' but 'CurrentNamespace' is null?", $2);
		if (CurrentNamespace.IsTypeDefined($2))
			Scanner.yyerror("A type has already been defined with the identifier '{0}'", $2);
		CurrentStruct = CurrentNamespace.AddStruct($2);
		Log("Added struct '{0}'", CurrentStruct.Identifier);
	}
	  struct_fields BLOCK_END
	{
		CurrentStruct = null;
	}
	;
class_definition
	:
	| CLASS IDENTIFIER BLOCK_START
	{
		if (CurrentNamespace == null)
			Scanner.yyerror("Tried to define class '{0}' but 'CurrentNamespace' is null?", $2);
		if (CurrentNamespace.IsTypeDefined($2))
			Scanner.yyerror("A type has already been defined with the identifier '{0}'", $2);
		CurrentClass = CurrentNamespace.AddClass($2);
		Log("Added class '{0}'", CurrentClass.Identifier);
	}
	  class_fields BLOCK_END
	{
		CurrentClass = null;
	}
	;
service_definition
	:
	| SERVICE IDENTIFIER BLOCK_START
	{
		if (CurrentNamespace == null)
			Scanner.yyerror("Tried to define service '{0}' but 'CurrentNamespace' is null?", $2);
		if (CurrentNamespace.IsTypeDefined($2))
			Scanner.yyerror("A type has already been defined with the identifier '{0}'", $2);
		CurrentService = CurrentNamespace.AddService($2);
		Log("Added service '{0}'", CurrentService.Identifier);
	}
	  service_fields BLOCK_END
	{
		CurrentClass = null;
	}
	;
modifiers
	:
	| modifiers MODIFIER
	{
		ParsedModifiers |= $2;
		Log("ParsedModifiers |= {0}", $2);
	}
	;
parameters
	:
	| parameters type IDENTIFIER ','
	{
		if (CurrentMethod == null)
			Scanner.yyerror("Tried to add parameter to method but 'CurrentMethod' is null?");
		if (ParsedType == null)
			Scanner.yyerror("Tried to add parameter to method '{0}' but 'ParsedType' is null?", CurrentMethod.Identifier);
		if (CurrentMethod.HasParameter($3))
			Scanner.yyerror("Tried to add parameter '{0}' to method '{1}' but a parameter already exists with that name.", $3, CurrentMethod.Identifier);
		CurrentMethod.AddParameter($3, ParsedType);
		Log("Added parameter '{0}' ({1}) to method '{2}'", $3, ParsedType, CurrentMethod.Identifier);
	}
	| parameters type IDENTIFIER
	{
		if (CurrentMethod == null)
			Scanner.yyerror("Tried to add parameter to method but 'CurrentMethod' is null?");
		if (ParsedType == null)
			Scanner.yyerror("Tried to add parameter to method '{0}' but 'ParsedType' is null?", CurrentMethod.Identifier);
		if (CurrentMethod.HasParameter($3))
			Scanner.yyerror("Tried to add parameter '{0}' to method '{1}' but a parameter already exists with that name.", $3, CurrentMethod.Identifier);
		CurrentMethod.AddParameter($3, ParsedType);
		Log("Added parameter '{0}' ({1}) to method '{2}'", $3, ParsedType, CurrentMethod.Identifier);
	}
	;
struct_fields
	: 
	| struct_fields type IDENTIFIER END_OF_STATEMENT
	{
		if (CurrentStruct == null)
			Scanner.yyerror("Tried to add property to struct but 'CurrentStruct' is null?");
		if (ParsedType == null)
			Scanner.yyerror("Tried to add property to struct '{0}' but 'ParsedType' is null?", CurrentStruct.Identifier);
		if (CurrentStruct.HasProperty($3))
			Scanner.yyerror("Tried to add property '{0}' to struct '{1}' but a property already exists with that name.", $3, CurrentStruct.Identifier);
		CurrentStruct.AddProperty($3, ParsedType);
		Log("Added {0} property '{1}' to struct '{2}'", ParsedType, $3, CurrentStruct.Identifier);
	}
	;
class_fields
	:
	| class_fields type IDENTIFIER modifiers END_OF_STATEMENT
	{
		if (CurrentClass == null)
			Scanner.yyerror("Tried to add property to class but 'CurrentClass' is null?");
		if (ParsedType == null)
			Scanner.yyerror("Tried to add property to class '{0}' but 'ParsedType' is null?", CurrentClass.Identifier);
		if (CurrentClass.HasProperty($3))
			Scanner.yyerror("Tried to add property '{0}' to class '{1}' but a property/method already exists with that name.", $3, CurrentClass.Identifier);
		CurrentClass.AddProperty($3, ParsedType, ParsedModifiers);
		ParsedModifiers = ToffeeModifiers.None;
		Log("Added {0} property '{1}' to class '{2}'", ParsedType, $3, CurrentClass.Identifier);
	}
	| class_fields IDENTIFIER PARAMS_START
	{
		if (CurrentClass == null)
			Scanner.yyerror("Tried to add method to class but 'CurrentClass' is null?");
		if (CurrentClass.HasMethod($2))
			Scanner.yyerror("Tried to add method '{0}' to class '{1}' but a property/method already exists with that name.", $3, CurrentClass.Identifier);
		CurrentMethod = CurrentClass.AddMethod($2);
		ParsedModifiers = ToffeeModifiers.None;
		Log("Added method '{0}' to class '{1}'", CurrentMethod.Identifier, CurrentClass.Identifier);
	}
	  parameters PARAMS_END modifiers END_OF_STATEMENT
	{
		CurrentMethod.Modifiers = ParsedModifiers;
		ParsedModifiers = ToffeeModifiers.None;
	}
	;
service_fields
	:
	| service_fields IDENTIFIER PARAMS_START
	{
		if (CurrentService == null)
			Scanner.yyerror("Tried to add method to service but 'CurrentService' is null?");
		if (CurrentService.HasMethod($2))
			Scanner.yyerror("Tried to add method '{0}' to service '{1}' but a property/method already exists with that name.", $3, CurrentService.Identifier);
		CurrentMethod = CurrentService.AddMethod($2);
		ParsedModifiers = ToffeeModifiers.None;
		Log("Added method '{0}' to service '{1}'", CurrentMethod.Identifier, CurrentService.Identifier);
	}
	  parameters PARAMS_END modifiers END_OF_STATEMENT
	{
		CurrentMethod.Modifiers = ParsedModifiers;
		ParsedModifiers = ToffeeModifiers.None;
	}
	;
%%

internal Parser(TdlFile file, bool verbosity, Scanner scanner) : base(scanner)
{
	DefinitionFile = file;
	CurrentNamespace = file;
	Verbosity = verbosity;
	ParsedModifiers = ToffeeModifiers.None;
}

internal void Log(string message)
{
	if (Verbosity)
		Console.WriteLine("Parser: " + message);
}

internal void Log(string message, params object[] args)
{
	if (Verbosity)
		Console.WriteLine("Parser: " + message, args);
}
