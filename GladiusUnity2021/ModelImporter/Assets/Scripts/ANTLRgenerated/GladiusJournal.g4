grammar GladiusJournal;

root
: (rumour | gossip | quest)+;

rumour
: 'RUMOR' STRING conversation? minDays? ursulaChoice? valensChoice? journalTitle? journalText? shop*;

conversation
: 'CONVERSATION' STRING;

minDays
: 'MINDAYS' INT;

ursulaChoice
: 'URSULACHOICE' INT;

valensChoice
: 'VALENSCHOICE' INT;

journalTitle
: 'JOURNALTITLE' INT;

journalText
: 'JOURNALTEXT' INT;

shop
: ('Shop' | 'SHOP') STRING;


gossip
: 'GOSSIP' STRING conversation minLevel? minDays? ursulaChoice valensChoice journalTitle? journalText? shop*;


minLevel
: 'MINLEVEL' INT;

quest
: 'QUEST' STRING prize? conversation? success? failure? minLevel? minDays? ursulaChoice? valensChoice? journalTitle? journalText? shop* type? encounter? lastDay? relativeDay? onetime? item* yank? location?;


prize
: 'PRIZE' STRING;

success
: 'SUCCESS' STRING;

failure
: 'FAILURE' STRING;

type
: 'TYPE' INT;

encounter
: 'ENCOUNTER' STRING;

onetime
: 'ONETIME';

lastDay
: 'LASTDAY' INT;

relativeDay
: 'RELATIVEDAY';

item
: 'ITEM' STRING;

yank
: 'YANK';


location
: 'LOCATION' STRING;


ID : [a-z]+ ;             // match lower-case identifiers

WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines


INT: '-'? '0' | [1-9] [0-9]*; 
FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
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

