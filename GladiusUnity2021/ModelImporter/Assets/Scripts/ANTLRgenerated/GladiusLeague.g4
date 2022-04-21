grammar GladiusLeague;

root
: officeName officeDesc tagLine1 ('INDOORS')? tagLine2 leaguePtsNeeded? officer?;

officeName
: 'OFFICENAME' STRING COMMA INT;

officeDesc
: 'OFFICEDESC' INT;

tagLine1
: 'TAGLINE1' INT;

tagLine2
: 'TAGLINE2' INT;

leaguePtsNeeded
: 'LEAGUEPTSNEEDED' INT;

officer
: 'OFFICER' STRING;

recruit
: 'RECRUIT' STRING INT INT INT INT INT INT INT INT INT;

school
: 'SCHOOL' STRING INT;


league
: 'LEAGUE' STRING COMMA INT leagueDesc designNotes? leaguePts encptsNeeded minPop? tier? badge* onHover onSelect onWin  prizeCompletion* prizeMastery* encounter+;

leagueDesc
: 'LEAGUEDESC' INT;

leaguePts
: 'LEAGUEPTS' INT;

encptsNeeded
: 'ENCPTSNEEDED' INT;

onHover
: 'ONHOVER' INT INT;

onSelect
: 'ONSELECT' INT INT;

onWin
: 'ONWIN' INT;

designNotes
: 'DESIGNNOTES' STRING;

minPop
: 'MINPOP' INT;

tier
: 'TIER' INT;

badge
: 'BADGE' INT COMMA STRING;

prizeCompletion
: 'PRIZECOMPLETION' STRING INT;

prizeMastery
: 'PRIZEMASTERY' STRING INT;




encounter
: 'ENCOUNTER' STRING COMMA INT encDesc designNotes? encFile type? encpts entryFee teams onHover onSelect onWin frequency;

encDesc
: 'ENCDESC' INT;

encFile
: 'ENCFILE' STRING;

encpts
: 'ENCPTS' INT;

teams
: 'TEAMS' INT INT INT INT INT;

frequency
: 'FREQUENCY' INT;

type
: 'TYPE' STRING;

entryFee
: 'ENTRYFEE' INT;


WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines


INT: '-'? '0' | [1-9] [0-9]*; 
//INT: '-'? [0-9]+; 
//FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
OBR: '{';
CBR: '}';
COMMA: ',';




STRING: '"' (ESC | ~["\\,])* '"';  
fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;

