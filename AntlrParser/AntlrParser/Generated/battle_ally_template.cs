public class battle_ally_template : BaseScript
{
    public void DeactivateReinforcementUnit(UNKNOWN theUnit)
    {
        theUnit.SetUntargetable();
        theUnit.Deactivate();
    }

    public void GetUnitToAssociate(string startLoc,int whenToReturn)
    {
        globals=InterfaceGlobals() ;
        ;
        UnitList=globals.GetUnitList() ;
        ;
        int count=0 ;
        ;
        unitToReturn=None ;
        ;
        EnterCriticalSection();
        TestUnit=UnitList.GetFirst() ;
        ;
        while(TestUnit){if(TestUnit.GetStartPos()==startLoc){
        count+=1 ;
        ;
        if(count==whenToReturn){
        unitToReturn=TestUnit ;
        ;
        }
        ;
        }
        TestUnit=UnitList.GetNext() ;
        ;};
        LeaveCriticalSection();
        return unitToReturn;
        ;
    }

    public void ActivateReinforcementUnit(UNKNOWN theUnit,UNKNOWN theProp,UNKNOWN teamToJoin,UNKNOWN appearArea)
    {
        tactics=InterfaceBattleTactics() ;
        ;
        theUnit.SetUntargetable();
        theUnit.Activate();
        appearArea[0]=2 ;
        ;
        appearArea[1]=2 ;
        ;
        appearArea[2]=2 ;
        ;
        appearArea[3]=2 ;
        ;
        if(teamToJoin==0){
        theUnit.SetTeamID(0);
        }
        ;
        battle_utils.TeleportToArea(theUnit,appearArea);
    }

    public void GetPropToAssociate(int className,UNKNOWN whenToReturn)
    {
        globals=InterfaceGlobals() ;
        ;
        UnitList=globals.GetUnitList() ;
        ;
        int count=0 ;
        ;
        unitToReturn=None ;
        ;
        EnterCriticalSection();
        TestUnit=UnitList.GetFirst() ;
        ;
        while(TestUnit){if(TestUnit.GetClassName()==className){
        count+=1 ;
        ;
        if(count==whenToReturn){
        unitToReturn=TestUnit ;
        ;
        }
        ;
        }
        TestUnit=UnitList.GetNext() ;
        ;};
        LeaveCriticalSection();
        return unitToReturn;
        ;
    }

    public void SkirmishLoop(UNKNOWN theskirmish,UNKNOWN propToCheck,UNKNOWN unitToSpawn,UNKNOWN appearArea)
    {
        int TARGET_RESULT_HIT=0x00000002 ;
        ;
        targets=theskirmish.GetTargetList() ;
        ;
        skirmishNode=targets.GetFirst() ;
        ;
        print("Wait for the chatter to end\n");
        while(theskirmish.DoSkirmishEndClassChatter(skirmishNode)!=0){print(".")sleep(0);};
        print("\n");
        EnterCriticalSection();
        targets=theskirmish.GetTargetList() ;
        ;
        skirmishNode=targets.GetFirst() ;
        ;
        while(skirmishNode!=){if(skirmishNode.mFlags&&TARGET_RESULT_HIT){
        unit=skirmishNode.GetDefender() ;
        ;
        name=unit.GetClassName() ;
        ;
        if(unit==propToCheck&&unit.IsDead()){
        attacker=skirmishNode.GetAttacker() ;
        ;
        teamtemp=attacker.GetCurrentTeam() ;
        ;
        ActivateReinforcementUnit(unitToSpawn,propToCheck,teamtemp,appearArea);
        }
        ;
        }
        skirmishNode=targets.GetNext() ;
        ;};
        LeaveCriticalSection();
    }

    public void BattleEnumSetup()
    {
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
        ;
    }
}
