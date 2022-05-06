//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from GladiusStatFile.g4 by ANTLR 4.9.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public partial class GladiusStatFileLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, WS=2, INT=3, FLOAT=4, COMMA=5, UNDERSCORE=6, DASH=7, STRING=8, 
		SINGLELINE_COMMENT=9;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "WS", "INT", "FLOAT", "COMMA", "UNDERSCORE", "DASH", "STRING", 
		"ESC", "UNICODE", "HEX", "SINGLELINE_COMMENT"
	};


	public GladiusStatFileLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public GladiusStatFileLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'MODCORESTATSCOMP:'", null, null, null, "','", "'_'", "'-'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, "WS", "INT", "FLOAT", "COMMA", "UNDERSCORE", "DASH", "STRING", 
		"SINGLELINE_COMMENT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "GladiusStatFile.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static GladiusStatFileLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\v', 'u', '\b', '\x1', '\x4', '\x2', '\t', '\x2', '\x4', 
		'\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', 
		'\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', 
		'\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', '\t', 
		'\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x6', '\x3', '/', '\n', '\x3', 
		'\r', '\x3', '\xE', '\x3', '\x30', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x4', '\x5', '\x4', '\x36', '\n', '\x4', '\x3', '\x4', '\x6', '\x4', 
		'\x39', '\n', '\x4', '\r', '\x4', '\xE', '\x4', ':', '\x3', '\x5', '\x5', 
		'\x5', '>', '\n', '\x5', '\x3', '\x5', '\x6', '\x5', '\x41', '\n', '\x5', 
		'\r', '\x5', '\xE', '\x5', '\x42', '\x3', '\x5', '\x3', '\x5', '\x6', 
		'\x5', 'G', '\n', '\x5', '\r', '\x5', '\xE', '\x5', 'H', '\x5', '\x5', 
		'K', '\n', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', 
		'\x3', '\b', '\x3', '\b', '\x3', '\t', '\x5', '\t', 'T', '\n', '\t', '\x3', 
		'\t', '\x3', '\t', '\x3', '\t', '\a', '\t', 'Y', '\n', '\t', '\f', '\t', 
		'\xE', '\t', '\\', '\v', '\t', '\x3', '\n', '\x3', '\n', '\x3', '\n', 
		'\x5', '\n', '\x61', '\n', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\v', 
		'\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', 
		'\r', '\x3', '\r', '\x3', '\r', '\x3', '\r', '\a', '\r', 'o', '\n', '\r', 
		'\f', '\r', '\xE', '\r', 'r', '\v', '\r', '\x3', '\r', '\x3', '\r', '\x2', 
		'\x2', '\xE', '\x3', '\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', 
		'\a', '\r', '\b', '\xF', '\t', '\x11', '\n', '\x13', '\x2', '\x15', '\x2', 
		'\x17', '\x2', '\x19', '\v', '\x3', '\x2', '\t', '\x5', '\x2', '\v', '\f', 
		'\xF', '\xF', '\"', '\"', '\x3', '\x2', '\x32', ';', '\x4', '\x2', '\x43', 
		'\\', '\x63', '|', '\b', '\x2', '\"', '\"', ')', ')', '\x32', ';', '\x43', 
		'\\', '^', '^', '\x63', '|', '\n', '\x2', '$', '$', '\x31', '\x31', '^', 
		'^', '\x64', '\x64', 'h', 'h', 'p', 'p', 't', 't', 'v', 'v', '\x5', '\x2', 
		'\x32', ';', '\x43', 'H', '\x63', 'h', '\x4', '\x2', '\f', '\f', '\xF', 
		'\xF', '\x2', '}', '\x2', '\x3', '\x3', '\x2', '\x2', '\x2', '\x2', '\x5', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x11', '\x3', '\x2', '\x2', '\x2', '\x2', '\x19', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x5', 
		'.', '\x3', '\x2', '\x2', '\x2', '\a', '\x35', '\x3', '\x2', '\x2', '\x2', 
		'\t', '=', '\x3', '\x2', '\x2', '\x2', '\v', 'L', '\x3', '\x2', '\x2', 
		'\x2', '\r', 'N', '\x3', '\x2', '\x2', '\x2', '\xF', 'P', '\x3', '\x2', 
		'\x2', '\x2', '\x11', 'S', '\x3', '\x2', '\x2', '\x2', '\x13', ']', '\x3', 
		'\x2', '\x2', '\x2', '\x15', '\x62', '\x3', '\x2', '\x2', '\x2', '\x17', 
		'h', '\x3', '\x2', '\x2', '\x2', '\x19', 'j', '\x3', '\x2', '\x2', '\x2', 
		'\x1B', '\x1C', '\a', 'O', '\x2', '\x2', '\x1C', '\x1D', '\a', 'Q', '\x2', 
		'\x2', '\x1D', '\x1E', '\a', '\x46', '\x2', '\x2', '\x1E', '\x1F', '\a', 
		'\x45', '\x2', '\x2', '\x1F', ' ', '\a', 'Q', '\x2', '\x2', ' ', '!', 
		'\a', 'T', '\x2', '\x2', '!', '\"', '\a', 'G', '\x2', '\x2', '\"', '#', 
		'\a', 'U', '\x2', '\x2', '#', '$', '\a', 'V', '\x2', '\x2', '$', '%', 
		'\a', '\x43', '\x2', '\x2', '%', '&', '\a', 'V', '\x2', '\x2', '&', '\'', 
		'\a', 'U', '\x2', '\x2', '\'', '(', '\a', '\x45', '\x2', '\x2', '(', ')', 
		'\a', 'Q', '\x2', '\x2', ')', '*', '\a', 'O', '\x2', '\x2', '*', '+', 
		'\a', 'R', '\x2', '\x2', '+', ',', '\a', '<', '\x2', '\x2', ',', '\x4', 
		'\x3', '\x2', '\x2', '\x2', '-', '/', '\t', '\x2', '\x2', '\x2', '.', 
		'-', '\x3', '\x2', '\x2', '\x2', '/', '\x30', '\x3', '\x2', '\x2', '\x2', 
		'\x30', '.', '\x3', '\x2', '\x2', '\x2', '\x30', '\x31', '\x3', '\x2', 
		'\x2', '\x2', '\x31', '\x32', '\x3', '\x2', '\x2', '\x2', '\x32', '\x33', 
		'\b', '\x3', '\x2', '\x2', '\x33', '\x6', '\x3', '\x2', '\x2', '\x2', 
		'\x34', '\x36', '\a', '/', '\x2', '\x2', '\x35', '\x34', '\x3', '\x2', 
		'\x2', '\x2', '\x35', '\x36', '\x3', '\x2', '\x2', '\x2', '\x36', '\x38', 
		'\x3', '\x2', '\x2', '\x2', '\x37', '\x39', '\t', '\x3', '\x2', '\x2', 
		'\x38', '\x37', '\x3', '\x2', '\x2', '\x2', '\x39', ':', '\x3', '\x2', 
		'\x2', '\x2', ':', '\x38', '\x3', '\x2', '\x2', '\x2', ':', ';', '\x3', 
		'\x2', '\x2', '\x2', ';', '\b', '\x3', '\x2', '\x2', '\x2', '<', '>', 
		'\a', '/', '\x2', '\x2', '=', '<', '\x3', '\x2', '\x2', '\x2', '=', '>', 
		'\x3', '\x2', '\x2', '\x2', '>', '@', '\x3', '\x2', '\x2', '\x2', '?', 
		'\x41', '\t', '\x3', '\x2', '\x2', '@', '?', '\x3', '\x2', '\x2', '\x2', 
		'\x41', '\x42', '\x3', '\x2', '\x2', '\x2', '\x42', '@', '\x3', '\x2', 
		'\x2', '\x2', '\x42', '\x43', '\x3', '\x2', '\x2', '\x2', '\x43', 'J', 
		'\x3', '\x2', '\x2', '\x2', '\x44', '\x46', '\a', '\x30', '\x2', '\x2', 
		'\x45', 'G', '\t', '\x3', '\x2', '\x2', '\x46', '\x45', '\x3', '\x2', 
		'\x2', '\x2', 'G', 'H', '\x3', '\x2', '\x2', '\x2', 'H', '\x46', '\x3', 
		'\x2', '\x2', '\x2', 'H', 'I', '\x3', '\x2', '\x2', '\x2', 'I', 'K', '\x3', 
		'\x2', '\x2', '\x2', 'J', '\x44', '\x3', '\x2', '\x2', '\x2', 'J', 'K', 
		'\x3', '\x2', '\x2', '\x2', 'K', '\n', '\x3', '\x2', '\x2', '\x2', 'L', 
		'M', '\a', '.', '\x2', '\x2', 'M', '\f', '\x3', '\x2', '\x2', '\x2', 'N', 
		'O', '\a', '\x61', '\x2', '\x2', 'O', '\xE', '\x3', '\x2', '\x2', '\x2', 
		'P', 'Q', '\a', '/', '\x2', '\x2', 'Q', '\x10', '\x3', '\x2', '\x2', '\x2', 
		'R', 'T', '\t', '\x4', '\x2', '\x2', 'S', 'R', '\x3', '\x2', '\x2', '\x2', 
		'T', 'Z', '\x3', '\x2', '\x2', '\x2', 'U', 'Y', '\t', '\x5', '\x2', '\x2', 
		'V', 'Y', '\x5', '\r', '\a', '\x2', 'W', 'Y', '\x5', '\xF', '\b', '\x2', 
		'X', 'U', '\x3', '\x2', '\x2', '\x2', 'X', 'V', '\x3', '\x2', '\x2', '\x2', 
		'X', 'W', '\x3', '\x2', '\x2', '\x2', 'Y', '\\', '\x3', '\x2', '\x2', 
		'\x2', 'Z', 'X', '\x3', '\x2', '\x2', '\x2', 'Z', '[', '\x3', '\x2', '\x2', 
		'\x2', '[', '\x12', '\x3', '\x2', '\x2', '\x2', '\\', 'Z', '\x3', '\x2', 
		'\x2', '\x2', ']', '`', '\a', '^', '\x2', '\x2', '^', '\x61', '\t', '\x6', 
		'\x2', '\x2', '_', '\x61', '\x5', '\x15', '\v', '\x2', '`', '^', '\x3', 
		'\x2', '\x2', '\x2', '`', '_', '\x3', '\x2', '\x2', '\x2', '\x61', '\x14', 
		'\x3', '\x2', '\x2', '\x2', '\x62', '\x63', '\a', 'w', '\x2', '\x2', '\x63', 
		'\x64', '\x5', '\x17', '\f', '\x2', '\x64', '\x65', '\x5', '\x17', '\f', 
		'\x2', '\x65', '\x66', '\x5', '\x17', '\f', '\x2', '\x66', 'g', '\x5', 
		'\x17', '\f', '\x2', 'g', '\x16', '\x3', '\x2', '\x2', '\x2', 'h', 'i', 
		'\t', '\a', '\x2', '\x2', 'i', '\x18', '\x3', '\x2', '\x2', '\x2', 'j', 
		'k', '\a', '\x31', '\x2', '\x2', 'k', 'l', '\a', '\x31', '\x2', '\x2', 
		'l', 'p', '\x3', '\x2', '\x2', '\x2', 'm', 'o', '\n', '\b', '\x2', '\x2', 
		'n', 'm', '\x3', '\x2', '\x2', '\x2', 'o', 'r', '\x3', '\x2', '\x2', '\x2', 
		'p', 'n', '\x3', '\x2', '\x2', '\x2', 'p', 'q', '\x3', '\x2', '\x2', '\x2', 
		'q', 's', '\x3', '\x2', '\x2', '\x2', 'r', 'p', '\x3', '\x2', '\x2', '\x2', 
		's', 't', '\b', '\r', '\x2', '\x2', 't', '\x1A', '\x3', '\x2', '\x2', 
		'\x2', '\xF', '\x2', '\x30', '\x35', ':', '=', '\x42', 'H', 'J', 'S', 
		'X', 'Z', '`', 'p', '\x3', '\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}