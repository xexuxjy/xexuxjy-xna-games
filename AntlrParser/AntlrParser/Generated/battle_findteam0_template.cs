public class battle_findteam0_template : BaseScript
{
    public void GetUnitOnTeam(int team,int whenToReturn)
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
        while(TestUnit){if(TestUnit.GetCurrentTeam()==team){
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
}
