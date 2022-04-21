grammar GladiusItem;

root: numEntries item+;

numEntries
: 'NUMENTRIES:' INT;

item
: itemCreate itemDescriptionId? itemDisplayNameId? itemCost itemMinLevel itemRarity itemAttribute* itemRegion* itemMesh* itemMaterial? itemHideSet* itemShowSet* itemAffinity? itemSkill* itemStatMod*;

itemCreate
: 'ITEMCREATE:' STRING COMMA STRING COMMA STRING COMMA STRING COMMA INT;

itemDescriptionId
: '.ITEMDESCRIPTIONID:' INT;

itemDisplayNameId
: '.ITEMDISPLAYNAMEID:' INT;

itemCost
: '.ITEMCOST:' INT;

itemMinLevel
: '.ITEMMINLEVEL:' INT;

itemRarity
: '.ITEMRARITY:' STRING;

itemRegion
: '.ITEMREGION:' STRING;

itemHideSet
: '.ITEMHIDESET:' STRING;

itemShowSet
: '.ITEMSHOWSET:' STRING;

itemMesh
: ('.ITEMMESH:' | '.ITEMMESH2:') STRING;

itemMaterial
: '.ITEMMATERIAL:' STRING;

itemSkill
: '.ITEMSKILL:' STRING;

itemAffinity
: '.ITEMAFFINITY:' STRING COMMA INT;

itemStatMod
: '.ITEMSTATMOD:' STRING COMMA INT;

itemAttribute
: '.ITEMATTRIBUTE:' STRING;

/*
 * Parser Rules
 */


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

