grammar AttackSkill;

root
: numEntries create+;

numEntries
: 'NUMENTRIES:' FLOAT;

create
: createDef (skillProperty)+ ;

skillProperty
:  (useClass | attribute |combatMods | moveToAttackMod | 
	affinity | range | usabilityCondition | 
	excludeRange | multiHitData | targetCondition | 
	moveRange | moveRangeCondition | summonData| 
	proxy | subSkill | meter | comboButton | 
	animSpeed | animBlock | fxBlock |  distanceDelay |
	effectBlock | projectileBlock | statusBlock |
	shiftData | skillFree | costs | affCost | prereq | 
	jobPointCost | level | descriptionId | displayNameId |
	weaponReq | replaces | statusUseLimit
);

animBlock
: (animTime | animStartFrame | anim | loopAnim | moveAnim | defendAnim | lowAnim | chargeAnim);

fxBlock
: (fx | fxSwing | sound | fxCTAG | fxProjectile | fxProjectileImpact | fxMove);

effectBlock
: (effect | effectSkillCond | effectRange | splitEffect | splitEffectCondition | effectFX | effectCondition | splitEffectCondition | effectStatusCond);

statusBlock
: (status | statusDuration | skillStatusSituationStatusCondition | statusInterval | statusTarget | statusSituationAffinityCondition | skillStatusSituationUnitCondition | statusChance | statusCondition | statusSituationSkillCondition) ;

projectileBlock
:  (projectile | projectileSequence | projectileAttr | projectileRotation);

createDef
: 'SKILLCREATE:' STRING COMMA STRING COMMA STRING;

useClass
: 'SKILLUSECLASS:' STRING;

displayNameId
: 'SKILLDISPLAYNAMEID:' FLOAT;

descriptionId
: 'SKILLDESCRIPTIONID:' FLOAT;

level
: 'SKILLLEVEL:' FLOAT;

jobPointCost
: 'SKILLJOBPOINTCOST:' FLOAT;

prereq
: 'SKILLPREREQ:' STRING;

attribute
: 'SKILLATTRIBUTE:' STRING;

costs
: 'SKILLCOSTS:' FLOAT COMMA FLOAT;

shiftData
: 'SKILLSHIFTDATA:' STRING COMMA STRING COMMA FLOAT COMMA FLOAT COMMA FLOAT COMMA FLOAT COMMA FLOAT COMMA FLOAT COMMA FLOAT;

affCost
: 'SKILLAFFCOST:' FLOAT;

effect
: 'SKILLEFFECT:' STRING COMMA FLOAT COMMA FLOAT;

effectFX
: 'SKILLEFFECTFX:' STRING;

splitEffect
: 'SKILLSPLITEFFECT:' STRING COMMA FLOAT;
splitEffectCondition
: 'SKILLSPLITEFFECTCONDITION:' STRING;

effectSkillCond
: 'SKILLEFFECTSKILLCOND:' STRING;

combatMods
: 'SKILLCOMBATMODS:' FLOAT COMMA FLOAT;

moveToAttackMod
: 'SKILLMOVETOATTACKMOD:' FLOAT;

affinity
: 'SKILLAFFINITY:' STRING;

range
: 'SKILLRANGE:' FLOAT COMMA STRING;

excludeRange
: 'SKILLEXCLUDERANGE:' FLOAT COMMA STRING;

meter
: 'SKILLMETER:' STRING COMMA FLOAT COMMA FLOAT;

anim
: 'SKILLANIM:' STRING;

loopAnim
: 'SKILLLOOPANIM:' STRING;

moveAnim
: 'SKILLMOVEANIM:' STRING;

defendAnim
: 'SKILLDEFENDHANIM:' STRING;

lowAnim
: 'SKILLLOWANIM:' STRING;

chargeAnim
: 'SKILLCHARGEANIM:' STRING;

animTime
: 'SKILLANIMTIME:' STRING COMMA FLOAT;

animStartFrame
: 'SKILLANIMSTARTFRAME:' FLOAT;

fx
: 'SKILLFX:' STRING;

fxSwing
: 'SKILLFXSWING:' STRING;

fxCTAG
: 'SKILLFXCTAG:' STRING;

fxProjectile
: 'SKILLFXPROJECTILE:' STRING;

//fxProjectileSequence
//: 'SKILLFXPROJECTILESEQUENCE:' STRING;


projectile
: 'SKILLPROJECTILE:' STRING COMMA STRING COMMA FLOAT;

projectileSequence
: 'SKILLPROJECTILESEQUENCE:' STRING COMMA STRING COMMA STRING COMMA FLOAT;

projectileRotation
: 'SKILLPROJECTILEROTATION:' FLOAT COMMA FLOAT COMMA FLOAT COMMA FLOAT;

projectileAttr
: 'SKILLPROJECTILEATTR:' STRING;


status
: ('SKILLSTATUS:' | 'SKILLSTATUS2:') STRING COMMA FLOAT COMMA FLOAT;

statusDuration
: ('SKILLSTATUSDURATION:' | 'SKILLSTATUSDURATION2:') STRING COMMA FLOAT;

statusTarget
: ('SKILLSTATUSTARGET:' | 'SKILLSTATUSTARGET2:') STRING COMMA FLOAT COMMA STRING;

statusSituationAffinityCondition
: ('SKILLSTATUSSITUATIONAFFINITYCONDITION:' | 'SKILLSTATUSSITUATIONAFFINITYCONDITION2:') STRING;

statusChance
: ('SKILLSTATUSCHANCE:' | 'SKILLSTATUSCHANCE2:') FLOAT;

effectStatusCond
: 'SKILLEFFECTSTATUSCOND:' STRING;

statusInterval
: ('SKILLSTATUSINTERVAL:' | 'SKILLSTATUSINTERVAL2:') STRING COMMA FLOAT;

statusCondition
: ('SKILLSTATUSCONDITION:' | 'SKILLSTATUSCONDITION2:') STRING;

skillStatusSituationUnitCondition
: ('SKILLSTATUSSITUATIONUNITCONDITION:' | 'SKILLSTATUSSITUATIONUNITCONDITION2:') STRING;

skillStatusSituationStatusCondition
: ('SKILLSTATUSSITUATIONSTATUSCONDITION:' |'SKILLSTATUSSITUATIONSTATUSCONDITION2:') STRING;

animSpeed
: 'SKILLANIMSPEED:' STRING COMMA FLOAT;

targetCondition
: 'SKILLTARGETCONDITION:' STRING;

	
effectRange
: 'SKILLEFFECTRANGE:' FLOAT COMMA STRING;


effectCondition
: 'SKILLEFFECTCONDITION:' STRING;

subSkill
: 'SKILLSUBSKILL:' STRING;

proxy
: 'SKILLPROXY:' STRING COMMA STRING;

weaponReq
: 'SKILLWEAPONREQ:' STRING;

comboButton
: 'SKILLCOMBOBUTTON:' STRING;

fxProjectileImpact
: 'SKILLFXPROJECTILEIMPACT:' STRING;

sound
: 'SKILLSOUND:' STRING;


skillFree
//: 'SKILLFREE' INT ':' STRING;
: ('SKILLFREE1:' | 'SKILLFREE2:' | 'SKILLFREE3:' | 'SKILLFREE4:') STRING;

multiHitData
: 'SKILLMULTIHITDATA:' STRING;

usabilityCondition
: 'SKILLUSABILITYCONDITION:' STRING;

summonData
: 'SKILLSUMMONDATA:' STRING COMMA STRING COMMA FLOAT;

moveRange
: 'SKILLMOVERANGE:' FLOAT COMMA STRING;

moveRangeCondition
: 'SKILLMOVERANGECONDITION:' STRING;

replaces
: 'SKILLREPLACES:' STRING;

statusSituationSkillCondition
: 'SKILLSTATUSSITUATIONSKILLCONDITION:' STRING;

fxMove
: 'SKILLFXMOVE:' STRING;

distanceDelay
: 'SKILLDISTANCEDELAY:' FLOAT COMMA FLOAT;

statusUseLimit
: 'SKILLSTATUSUSELIMIT:' FLOAT;

ID : [a-z]+ ;             // match lower-case identifiers

WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines


//INT: '-'? '0' | [1-9] [0-9]*; 
//INT: '-'? [0-9]+; 
//FLOAT : '-'? [0-9]+ ('.' [0-9]+)? ;
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

