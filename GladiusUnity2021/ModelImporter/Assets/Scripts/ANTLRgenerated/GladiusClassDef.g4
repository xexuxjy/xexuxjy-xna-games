grammar GladiusClassDef;

root
: createClass+;


createClass
: 'CREATECLASS:' STRING displayName descriptionId unitSet? skillUse? voiceLinePrefix? soundTable? mesh? derivationClass? headIcon? classIcon? gridSize? xpAward? itemSizes? affinity? attribute* rockScissorsPaper? itemCat* fx* levelUpXpNeeded? levelUpJPGiven? levelStatAwards* levelZeroStats?;

displayName
: 'DISPLAYNAMEID:' INT;

descriptionId
: 'DESCRIPTIONID:' INT;

unitSet
: 'UNITSETNAME:' STRING;

skillUse
:'SKILLUSENAME:' STRING;

voiceLinePrefix
: 'VOICELINEPREFIX:' STRING;

soundTable
: 'SOUNDTABLENAME:' STRING;

mesh
: 'MESH:' STRING;

classIcon
: 'CLASSICON:' STRING;

headIcon
: 'HEADICON:' STRING;

gridSize
: 'GRIDSIZE:' INT;

xpAward
: 'XPAWARD:' INT COMMA (INT|FLOAT);

itemSizes
: 'ITEMSIZES:' itemSubSize COMMA itemSubSize COMMA itemSubSize COMMA;

itemSubSize
: (FLOAT |INT );

attribute
: 'ATTRIBUTE:' STRING;


itemCat
: 'ITEMCAT:' STRING COMMA STRING COMMA STRING;


levelUpXpNeeded
: 'LEVELUPXPNEEDED:' INT COMMA INT COMMA INT COMMA INT COMMA (INT | FLOAT);

levelUpJPGiven
: 'LEVELUPJPGIVEN:' INT COMMA INT COMMA INT COMMA INT COMMA INT;

levelStatAwards
:'LEVELSTATAWARDS:' INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT;

levelZeroStats
:'LEVELZEROSTATS:' INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT;

affinity
: 'AFFINITY:' STRING;

rockScissorsPaper
: 'ROCKSCISSORSPAPER:' STRING;

fx
: 'FX:' STRING COMMA STRING;

derivationClass
: 'DERIVATIONCLASSNAME:' STRING;
			  
/*
 * Parser Rules
 */

compileUnit
	:	EOF
	;

/*
 * Lexer Rules
 */


WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines

INT: '-'? [0-9]+; 

FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
COMMA: ',';
UNDERSCORE: '_';
DASH: '-';
LTHAN : '<';
GTHAN : '>';
PERIOD: '.';

STRING : ([a-z]|[A-Z] | LTHAN | GTHAN)(([a-zA-Z0-9' \\] | UNDERSCORE | DASH | LTHAN | GTHAN | PERIOD)*);

fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;
