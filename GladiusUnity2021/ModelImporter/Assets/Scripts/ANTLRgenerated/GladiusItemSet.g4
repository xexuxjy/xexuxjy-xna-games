grammar GladiusItemSet;

root: (itemSetClassLine | modItemsComp)+; 

/*
////// ITEMSET export Class: Amazon Amazon  Region: Imperia Affinity: Earth
*/
itemSetClassLine
: 'ITEMSET export Class:' STRING STRING 'Region:' STRING 'Affinity:' STRING;

/*
//          :      variant, Lv,Lv,                          weapon,                           armor,                          shield,                          helmet             accessory
MODITEMSCOMP:    itemsetIE, 11,11,              "Earth Silver Bow",            "Earth Cured Bikini",                              "",           "Earth Soul's Diadem",          "Earth Stone Bracelet" 	//, Cost, 004576,092262,Acc,Def,-004,0000
*/
modItemsComp
: 'MODITEMSCOMP:' STRING COMMA INT COMMA INT COMMA ITEMNAME COMMA ITEMNAME COMMA ITEMNAME COMMA ITEMNAME COMMA ITEMNAME; 


/*
 * Parser Rules
 */


WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines

INT: '-'? [0-9]+; 

FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
COMMA: ',';
UNDERSCORE: '_';
DASH: '-';

ITEMNAME: '"' (ESC | ~["])* '"';  
STRING : ([a-z]|[A-Z])(([a-zA-Z0-9'\\] | UNDERSCORE | DASH)*);


fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;

