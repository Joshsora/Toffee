/*
	Lexer written for Toffee Definition Language
	by Joshsora (Joshua Scott)
*/
%using Toffee.Core;

%namespace Toffee.Core.Parser

%option stack, minimize, parser, persistbuffer, unicode, compressNext, embedBuffers

// Error function
%{
public override void yyerror(string format, params object[] args)
{
	throw new TdlParserException("Error on line {0}: {1}", yyline, string.Format(format, args));
}
%}

// Various
Whitespace				[ \t]
Eol						(\r\n?|\n)
EndOfStatement			";"

// Comments
CommentStart 			\/\*
CommentEnd 				\*\/
LineComment 			"//".*

// Blocks
BlockStart				"{"
BlockEnd				"}"
ParamsStart				"("
ParamsEnd				")"

// Keyword definitions
Import					import
Network					network
Namespace				namespace
Enum					enum
Class 					class
Struct					struct
Service					service
Bool					bool
Char					char
Int8					sbyte
UInt8					byte
Int16					short
UInt16					ushort
Int32					int
UInt32					uint
Int64					long
String					string
UInt64					ulong
Float32					float
Float64					double
Array					"[]"

// Modifier definitions
AnonSend				anonsend
ClientSend				clsend
OwnerSend				ownsend
FriendSend				friendsend
Broadcast				broadcast
GlobalFriends			gfriends
LocalFriends			lfriends
Ram						ram
Db						db
Required				required
Encrypted				encrypted

// Literal definitions
Identifier				([a-zA-Z]([a-zA-Z0-9_])*)
StringLiteral    		\".*\"
NamespaceIdentifier		([a-zA-Z]+(\.+([a-zA-Z]+))*)
Filename				([a-zA-Z]+(\/+([a-zA-Z]+))*)

// FSM states
%x LINE_COMMENT
%x INSIDE_COMMENT
%%

// Remove whitespaces
{Whitespace}+ { ; }

// Endings
{Eol} { ; }
{EndOfStatement} { return (int)Tokens.END_OF_STATEMENT; }

// Remove comments
{LineComment}+ { yy_push_state(LINE_COMMENT); }
<LINE_COMMENT>{
	{Eol} { yy_pop_state(); }
}
{CommentStart}+ { yy_push_state(INSIDE_COMMENT); }
<INSIDE_COMMENT>{
	[^*\n]+ { ; }
	"*" { ; }
	{CommentEnd} { yy_pop_state(); }
	<<EOF>> { ; }
}

// Blocks
{BlockStart} { return (int)Tokens.BLOCK_START; }
{BlockEnd} { return (int)Tokens.BLOCK_END; }
{ParamsStart} { return (int)Tokens.PARAMS_START; }
{ParamsEnd} { return (int)Tokens.PARAMS_END; }

// Keywords
{Import} { return (int)Tokens.IMPORT; }
{Network} { return (int)Tokens.NETWORK; }
{Namespace} { return (int)Tokens.NAMESPACE; }
{Enum} { return (int)Tokens.ENUM; }
{Class} { return (int)Tokens.CLASS; }
{Struct} { return (int)Tokens.STRUCT; }
{Service} { return (int)Tokens.SERVICE; }
{Bool} {
	yylval.Type = ToffeeValueType.Bool;
	return (int)Tokens.TYPE;
}
{Char} {
	yylval.Type = ToffeeValueType.Char;
	return (int)Tokens.TYPE;
}
{Int8} {
	yylval.Type = ToffeeValueType.Int8;
	return (int)Tokens.TYPE;
}
{UInt8} {
	yylval.Type = ToffeeValueType.UInt8;
	return (int)Tokens.TYPE;
}
{Int16}	{
	yylval.Type = ToffeeValueType.Int16;
	return (int)Tokens.TYPE;
}
{UInt16} {
	yylval.Type = ToffeeValueType.UInt16;
	return (int)Tokens.TYPE;
}
{Int32} {
	yylval.Type = ToffeeValueType.Int32;
	return (int)Tokens.TYPE;
}
{UInt32} {
	yylval.Type = ToffeeValueType.UInt32;
	return (int)Tokens.TYPE;
}
{Int64} {
	yylval.Type = ToffeeValueType.Int64;
	return (int)Tokens.TYPE;
}
{UInt64} {
	yylval.Type = ToffeeValueType.UInt64;
	return (int)Tokens.TYPE;
}
{String} {
	yylval.Type = ToffeeValueType.String;
	return (int)Tokens.TYPE;
}
{Float32} {
	yylval.Type = ToffeeValueType.Float32;
	return (int)Tokens.TYPE;
}
{Float64} {
	yylval.Type = ToffeeValueType.Float64;
	return (int)Tokens.TYPE;
}
{Array}	{ return (int)Tokens.ARRAY; }

// Modifiers
{AnonSend} {
	yylval.Modifier = ToffeeModifiers.AnonSend;
	return (int)Tokens.MODIFIER;
}
{ClientSend} {
	yylval.Modifier = ToffeeModifiers.ClientSend;
	return (int)Tokens.MODIFIER;
}
{OwnerSend} {
	yylval.Modifier = ToffeeModifiers.OwnerSend;
	return (int)Tokens.MODIFIER;
}
{FriendSend} {
	yylval.Modifier = ToffeeModifiers.FriendSend;
	return (int)Tokens.MODIFIER;
}
{Broadcast} {
	yylval.Modifier = ToffeeModifiers.Broadcast;
	return (int)Tokens.MODIFIER;
}
{GlobalFriends} {
	yylval.Modifier = ToffeeModifiers.GlobalFriends;
	return (int)Tokens.MODIFIER;
}
{LocalFriends} {
	yylval.Modifier = ToffeeModifiers.LocalFriends;
	return (int)Tokens.MODIFIER;
}
{Ram} {
	yylval.Modifier = ToffeeModifiers.Ram;
	return (int)Tokens.MODIFIER;
}
{Db} {
	yylval.Modifier = ToffeeModifiers.Db;
	return (int)Tokens.MODIFIER;
}
{Required} {
	yylval.Modifier = ToffeeModifiers.Required;
	return (int)Tokens.MODIFIER;
}
{Encrypted} {
	yylval.Modifier = ToffeeModifiers.Encrypted;
	return (int)Tokens.MODIFIER;
}

{Identifier} {
	yylval.String = yytext;
	return (int)Tokens.IDENTIFIER;
}

{StringLiteral} {
	if (yytext.Length > 2)
		yylval.String = yytext.Substring(1, yytext.Length - 2);
    else
		yylval.String = "";
	return (int)Tokens.STRING_LITERAL;
}

{NamespaceIdentifier} {
	yylval.String = yytext;
	return (int)Tokens.IDENTIFIER;
}

{Filename} {
	yylval.String = yytext;
	return (int)Tokens.FILENAME;
}