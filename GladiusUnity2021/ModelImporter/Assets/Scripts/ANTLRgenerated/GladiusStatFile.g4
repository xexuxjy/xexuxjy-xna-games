grammar GladiusStatFile;

root
: modCoreStatsComp+;

// STATSET export Class: AffinityBeast
// Level Power/Acc/Defense/Initiative/Constitution/MoveRate
//                              variant,     Level   Constit     Power  Accuracy   Defense   Initive     Move
//MODCORESTATSCOMP:               statset0,         1         7         6         5         6         6         1

modCoreStatsComp
: 'MODCORESTATSCOMP:' STRING COMMA INT INT INT INT INT INT (INT | FLOAT);


WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines

INT: '-'? [0-9]+; 

FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
COMMA: ',';
UNDERSCORE: '_';
DASH: '-';

STRING : ([a-z]|[A-Z])(([a-zA-Z0-9' \\] | UNDERSCORE | DASH)*);

fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;

