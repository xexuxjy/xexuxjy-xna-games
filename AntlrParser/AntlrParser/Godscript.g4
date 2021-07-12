grammar Godscript;

@members {
	public Scope m_rootScope;
	public Scope m_currentScope;
	public ScriptScope m_scriptScope;
	public FunctionSymbol m_currentFunction;
}
programStart
: functionDef*
; 


functionDef 
	: functionStart block? functionEnd
	;


block
:  ( expression | statement) ( expression | statement)*
;

functionStart 
	: FUNCTION functionName '(' functionArguments? ')'
{
/*
	FunctionSymbol functionSymbol = new FunctionSymbol($functionName.text,null,m_scriptScope);
	m_currentScope.Define(functionSymbol);
	m_currentScope = functionSymbol;
	m_functionSymbol = functionSymbol;
	*/
}
;

functionName
: IDENTIFIER
{
    string functionName = $IDENTIFIER.text;
    FunctionSymbol functionSymbol = m_currentScope.Resolve(functionName) as FunctionSymbol;
    if (functionSymbol == null)
    {
        functionSymbol = new FunctionSymbol(functionName, null, m_currentScope);
        m_currentScope.Define(functionSymbol);
    }
    m_currentScope = functionSymbol;
    m_currentFunction = functionSymbol;
}
;

booleanType 
: (TRUE | FALSE)
;


argumentList
	: expression (',' expression)*
	;

functionArguments
	: functionArgument (',' functionArgument)*
;

functionArgument
: IDENTIFIER
{
/*
	VariableSymbol vs = new VariableSymbol($IDENTIFIER.text,null);
	m_functionSymbol.AddArgument(vs);
	*/
}
;


functionEnd 
	: ENDFUNCTION
{

	m_currentScope = m_currentScope.GetParentScope();
	m_currentFunction = null;

}	
;



leftHandSide returns [Type type]
	:	
	expressionName
	| arrayAccess
	| instanceVariable
{
	
}
	;

arrayAccess returns [Type type]
: expressionName '[' expression ']'
{
	$type = $expressionName.type;
}
;

assignmentExpression returns [Type type]
: leftHandSide assignmentOperator expression ';'?
{
	$type = $expression.type;
	$leftHandSide.type = $expression.type;
}
;

assignmentOperator
	:	'='
	|	'*='
	|	'/='
	|	'%='
	|	'+='
	|	'-='
	|	'<<='
	|	'>>='
	|	'>>>='
	|	'&='
	|	'^='
	|	'|='
	;

expressionName returns [Type type] 
	:	(GLOBAL | SHARED )? (INT)? IDENTIFIER ('[' expression ']' ';')?
	{
	if($IDENTIFIER.text == "self")
	{
	$type = SymbolTable._ACTOR;
	}
	else
	{
	Symbol objectName = m_currentScope.Resolve($IDENTIFIER.text);
	if(objectName != null && objectName is ScopedSymbol )
	{
		ScopedSymbol ssObjectName = objectName as ScopedSymbol;
		$type = ssObjectName.Type;
	}
	}

	}
	;

methodName
	:	IDENTIFIER
	;


ifStatement
    : IF '(' expression ')' block? elseIfStatement* elseStatement? ENDIF
    ;


elseIfStatement
    : ELSEIF '(' expression ')' block?
    ;

elseStatement
    : ELSE block?
    ;

whileStatement
: WHILE '(' expression ')' block ENDWHILE
;

forStatement
: FOR IDENTIFIER '=' expression TO expression (STEP expression)? block NEXT IDENTIFIER;

/*
numberOrIdent
: (NUMBER | IDENTIFIER)
;

stringOrIdent
: (STRING | IDENTIFIER)
;
*/


gotoStatement
: GOTO IDENTIFIER
;

labelStatement
: IDENTIFIER ':'
;


/*	shared globals = InterfaceGlobals()
	shared battle  = InterfaceBattle()
	shared int teampoints[4]
	shared int timer[1]
	
	shared int teamtemp
	//You must do at least this much damage to score -- multiply by 1 for amateur, 2 for semi-prom and 3 for pro
	shared int pointThreshold = call FindTier@utilgen() * 8 //so, 8 for amateur, 16 for semi-pro, and 24 for pro
*/
sharedVariableArrayDecl
: SHARED (INT)? IDENTIFIER '[' INTEGER ']'
;

sharedVariableDecl
: SHARED (INT)? IDENTIFIER ('=' expression)?

;


expression returns [Type type]
: NULL {$type = SymbolTable._NULL;} # NULL 
| STRING {$type = SymbolTable._STRING;}# STRING 
| FLOAT {$type = SymbolTable._FLOAT;}# FLOAT 
| INTEGER {$type = SymbolTable._INTEGER;}# INTEGER
| booleanType {$type = SymbolTable._BOOL;}	# BOOLEAN 
| methodInvoke {$type = $methodInvoke.type;}	#METHINV 
| functionCall	{$type = $functionCall.type;}	# FUNCCALL
| sharedVariableDecl	# SHAREDVDECL
| sharedVariableArrayDecl # SHAREDARRACCVDECL
| expressionName	{$type = $expressionName.type;} # EXPRNAME
| instanceVariable	{$type = $instanceVariable.type;}# IVAR
| assignmentExpression  {$type = $assignmentExpression.type;} # ASSIGN
| arrayAccess		{$type = $arrayAccess.type;} # ARRACC
| operatorUnary expression {$type = SymbolTable._BOOL;} #OPUNARY
| a=expression operatorMulDivMod b=expression {$type = $a.type;}#OPMDM
| a=expression operatorAddSub b=expression {$type = $a.type;} # OPAS
| expression operatorComparison expression {$type = SymbolTable._BOOL;} # OPCOMP
| expression AND expression {$type = SymbolTable._BOOL;} # OPAND
| expression OR expression {$type = SymbolTable._BOOL;}# OPOR
| expression operatorBitwise expression {$type = SymbolTable._INTEGER;}# OPBW
| '(' expression ')' {$type = $expression.type;} # OPENCLOSED
;

statement
    : 
ifStatement  
| whileStatement 
| forStatement 
| gotoStatement
| labelStatement
| returnStatement
;

instanceVariable returns[Type type]
: a=IDENTIFIER '.' b=IDENTIFIER
{
	Symbol objectName = m_currentScope.Resolve($a.text);
	if(objectName != null && objectName is ScopedSymbol )
	{
		ScopedSymbol ssObjectName = objectName as ScopedSymbol;
		Symbol methodName = ssObjectName.Resolve($b.text);
		if(methodName != null)
		{
			$type = methodName.Type;
		}
	}
}
;

methodInvoke returns [Type type]
: a=IDENTIFIER '.' b=IDENTIFIER '(' argumentList? ')' (';')?
{
	Symbol objectName = m_currentScope.Resolve($a.text);
	if(objectName != null && objectName is ScopedSymbol )
	{
		ScopedSymbol ssObjectName = objectName as ScopedSymbol;
		Symbol methodName = ssObjectName.Resolve($b.text);
		if(methodName != null)
		{
			$type = methodName.Type;
		}
	}
}
;

returnStatement returns [Type type]
: 
 voidReturnStatement {$type = $voidReturnStatement.type;}	
| expressionReturnStatement	{$type = $expressionReturnStatement.type;}
;


voidReturnStatement returns [Type type]
: 'return' '(' ')' | 'return' '(None)'
{
	$type = SymbolTable._VOID;
}
;

expressionReturnStatement returns [Type type]
:	'return' '(' expression ')'
{
	$type = $expression.type;
}
;

expressionList
: expression (',' expression)*
;


functionCall returns [Type type]
: (CALL)? (a=IDENTIFIER | b=IDENTIFIER '@' c=IDENTIFIER) '(' argumentList? ')' ';'?
{
	if($a != null)
	{
		Symbol objectName =m_currentScope.Resolve($a.text);
		if(objectName != null)
		{
			$type = objectName.Type;
		}
	}
	else if ($b != null && $c != null)
	{
		string functionName = $b.text;
		string functionScript = $c.text;
		if(functionScript != null)
		{
			functionScript = functionScript.ToLower();
		}
		ScriptScope scriptScope = (ScriptScope)m_rootScope.Resolve(functionScript);
		if(scriptScope != null)
		{
			Symbol objectName =scriptScope.Resolve(functionName);
			if(objectName != null)
			{
				$type = objectName.Type;
			}
		}
	}
	
}
;

operatorAddSub
	: '+' | '-';

operatorMulDivMod
	: '*' | '/' | '%';

operatorComparison 
	: '<' | '>' | '<=' | '>=' | '!=' | '==';

operatorBitwise
	: '&' | '|' | '^' | '<<' | '>>';

operatorUnary
: '-' | '!';


fragment A
   : ('a' | 'A')
   ;


fragment B
   : ('b' | 'B')
   ;


fragment C
   : ('c' | 'C')
   ;


fragment D
   : ('d' | 'D')
   ;


fragment E
   : ('e' | 'E')
   ;


fragment F
   : ('f' | 'F')
   ;


fragment G
   : ('g' | 'G')
   ;


fragment H
   : ('h' | 'H')
   ;


fragment I
   : ('i' | 'I')
   ;


fragment J
   : ('j' | 'J')
   ;


fragment K
   : ('k' | 'K')
   ;


fragment L
   : ('l' | 'L')
   ;


fragment M
   : ('m' | 'M')
   ;


fragment N
   : ('n' | 'N')
   ;


fragment O
   : ('o' | 'O')
   ;


fragment P
   : ('p' | 'P')
   ;


fragment Q
   : ('q' | 'Q')
   ;


fragment R
   : ('r' | 'R')
   ;


fragment S
   : ('s' | 'S')
   ;


fragment T
   : ('t' | 'T')
   ;


fragment U
   : ('u' | 'U')
   ;


fragment V
   : ('v' | 'V')
   ;


fragment W
   : ('w' | 'W')
   ;


fragment X
   : ('x' | 'X')
   ;


fragment Y
   : ('y' | 'Y')
   ;


fragment Z
   : ('z' | 'Z')
   ;

GOTO
: G O T O
;

FUNCTION
 : F U N C T I O N
 ;

ENDFUNCTION
 : E N D F U N C T I O N
 ;

FOR
   : F O R
   ;

STEP
 : S T E P
 ;
 
 NEXT
 : N E X T
 ;

IF
   : I F
   ;

ELSEIF
 : E L S E I F
 ;
 
ELSE
 : E L S E
 ;

ENDIF
: E N D I F
;

WHILE
: W H I L E
;

ENDWHILE
: E N D W H I L E
;

CALL
: C A L L
;

GLOBAL
: G L O B A L
;

SHARED
: S H A R E D
;

INT
: I N T
;

TRUE
: T R U E
;

FALSE
: F A L S E
;


PLUS
   : '+'
   ;


MINUS
   : '-'
   ;


STAR
   : '*'
   ;


SLASH
   : '/'
   ;


ASSIGN
   : '='
   ;


COMMA
   : ','
   ;


SEMI
   : ';'
   ;


COLON
   : ':'
   ;

NOT_EQUAL
   : '<>'
   ;


LT
   : '<'
   ;


LE
   : '<='
   ;


GE
   : '>='
   ;


GT
   : '>'
   ;


LPAREN
   : '('
   ;


RPAREN
   : ')'
   ;


LBRACK
   : '['
   ;


LBRACK2
   : '(.'
   ;


RBRACK
   : ']'
   ;


RBRACK2
   : '.)'
   ;


DOT
   : '.'
   ;


DOTDOT
   : '..'
   ;


LCURLY
   : '{'
   ;


RCURLY
   : '}'
   ;



AND 
: '&&'
;

OR 
: '||'
;

TO 
: T O
;

NULL
: N U L L
;

WS
   : [ \t\r\n] -> skip
   ;


COMMENT
    :   '/*' .*? '*/' -> skip
    ;

LINE_COMMENT
    :   '//' ~('\r' | '\n')* -> skip
    ;

IDENTIFIER
   : ('a' .. 'z' | 'A' .. 'Z' | '_') ('a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_')*
   ;


STRING_LITERAL
   : '\'' ('\'\'' | ~ ('\''))* '\''
   ;

STRING: '"' (ESC | ~["\\])* '"';  
fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

INTEGER
 : [0-9]+ 
 | '0' [xX] HEX+
 ;

FLOAT
 : [0-9]+ ('.' [0-9]+)
 ;

fragment EXPONENT
   : ('e') ('+' | '-')? ('0' .. '9') +
   ;