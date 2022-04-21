grammar GladiusProps;

root: version (prop)+ endfile;


/*
VERSION: 1

PROP: "prop1" 
MODELS: "mesh\misc\crates\crate2_01.pax" 
LIGHTING: ON
SHADOW: OFF
HEIGHTS: 1.96
POS: 14.0000 18.0000 0.8970
ROT: 0.0000 0.0000 0.0000
ATTRIBUTES: 1
ATTRIBUTE: "No_Stand"
PROP_END

*/

version
: 'VERSION:' INT;

prop
: propname models lighting shadow scriptname? heights heightsoffset? pos rot attributescount (attribute)* propend;

propname
: 'PROP:' QUOTEDSTRING;

models
: 'MODELS:' ('No' | QUOTEDSTRING);

lighting
: 'LIGHTING:' onoff;

shadow
: 'SHADOW:' onoff;

scriptname
: 'SCRIPTNAME:' QUOTEDSTRING;

heights
: 'HEIGHTS:' FLOAT;

heightsoffset
: 'HEIGHTOFFSET:' FLOAT;

pos
: 'POS:' FLOAT FLOAT FLOAT;

rot 
: 'ROT:' FLOAT FLOAT FLOAT;

attributescount
: 'ATTRIBUTES:' INT;

attribute
: 'ATTRIBUTE:' QUOTEDSTRING;

propend
: 'PROP_END';

onoff
: ('ON' | 'OFF') ;

endfile
: 'ENDFILE';
/*
 * Parser Rules
 */


WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines

INT: '-'? [0-9]+; 

FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
COMMA: ',';
UNDERSCORE: '_';
DASH: '-';

QUOTEDSTRING: '"' (ESC | ~["])* '"';  
//STRING : ([a-z]|[A-Z])(([a-zA-Z0-9'\\] | UNDERSCORE | DASH)*);



fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;

