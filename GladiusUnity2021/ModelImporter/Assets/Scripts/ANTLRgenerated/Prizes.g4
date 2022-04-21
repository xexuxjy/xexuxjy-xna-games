grammar Prizes;

root
: prize+;


prize 
: 'PRIZE' STRING prizeCash? prizeExp? (prizeItem)* (prizeBadge)*
;

prizeCash
: 'PRIZECASH' level1=INT level2=INT
;

prizeExp
: 'PRIZEEXP' level1=INT level2=INT
;

prizeItem
: 'PRIZEITEM' STRING INT
;

prizeBadge
: 'PRIZEBADGE' STRING
;



ID : [a-z]+ ;             // match lower-case identifiers

WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines


//INT: '-'? '0' | [1-9] [0-9]*; 
INT: '-'? [0-9]+; 
//FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
//INT : '-'? [0-9]+ ('.' [0-9]+)? ;
OBR: '{';
CBR: '}';
COMMA: ',';


STRING: '"' (ESC | ~["\\])* '"';  
fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;

