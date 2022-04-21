grammar GladiusEncounter;

root: encounterName ursulaEase? valensEase? prizeTier* scene gridFile propsFile? music? cameraTrack* battleScript? totalPop? crowdLevel? cutscene* candie?  team+;

encounterName
: 'ENCOUNTERNAME:' STRING;

ursulaEase
: 'URSULAEASE:'	INT;

valensEase
: 'VALENSEASE:'	INT;

prizeTier
: 'PRIZETIER' STRING INT;

scene
: 'SCENE:' STRING;

gridFile
: 'GRIDFILE:' STRING;

propsFile
: 'PROPSFILE:' STRING;

music
: 'MUSIC:' STRING;

battleScript
: 'BATTLESCRIPT:' STRING;

cameraTrack
: 'CAMERATRACK:' STRING COMMA STRING;

totalPop
: 'TOTALPOP:' INT;

crowdLevel
: 'CROWDLEVEL:' INT;

candie
: 'CANDIE:';

cutscene
: 'CUTSCENE:' STRING COMMA STRING COMMA 'NULL';

team
: 'TEAM:' INT COMMA INT gridFile school? unitDB+;

school
: 'SCHOOL:' STRING;

unitDB
: 'UNITDB:' STRING COMMA INT COMMA STRING COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA STRING COMMA STRING COMMA STRING COMMA STRING COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT;

/*
 * Parser Rules
 */

ID : [a-z]+ ;             // match lower-case identifiers

WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines


//INT: '-'? '0' | [1-9] [0-9]*; 
INT: '-'? [0-9]+; 
//FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
OBR: '{';
CBR: '}';
COMMA: ',';


NL      : '\r'? '\n' | '\r';

//STRING: '"' (ESC | ~["\\])* '"';  

STRING
:   '"'
    (   '\\' .
    |   '"' '"'
    |   ~[\\"]
    )*
    '"'
;

//UNQUOTEDSTRING: [0-9a-zA-Z\\' ''_'''']+ NL;

fragment ESC: '\\' (["\\/bfnrt] | UNICODE);  
fragment UNICODE : 'u' HEX HEX HEX HEX;  
fragment HEX : [0-9a-fA-F]; 

SINGLELINE_COMMENT
 : '//' ~('\r' | '\n')* -> skip
 ;

