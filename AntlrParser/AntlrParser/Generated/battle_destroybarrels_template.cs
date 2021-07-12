public class battle_destroybarrels_template : BaseScript
{
    public void OnInitBattle()
    {
        ;
    }

    public void OnStartBattle()
    {
    }

    public void OnExecBattle()
    {
    }

    public void OnSkirmishEnd(UNKNOWN theskirmish)
    {
    }

    public void UpdateTeamPoints(int team,UNKNOWN points,UNKNOWN teampoints)
    {
        for(int n=0;n<3;n+=1)
        {if(team==n&&points>0){
        teampoints[n]+=points ;
        ;
        }
        ;
        };
    }

    public void ReportScore(UNKNOWN teampoints)
    {
        int numTeams=battle_utils.FindNumberTeams() ;
        ;
        globals=InterfaceGlobals() ;
        ;
        battle=InterfaceBattle() ;
        ;
        if(numTeams==4){
        string score1string="\n" ;
        ;
        string score2string="\n" ;
        ;
        string score3string="\n" ;
        ;
        string score4string="\n" ;
        ;
        }
        else if(numTeams==3){
        score1string="\n" ;
        ;
        score2string="\n" ;
        ;
        score3string="\n" ;
        ;
        score4string="" ;
        ;
        }
        else if(numTeams==2){
        score1string="\n" ;
        ;
        score2string="\n" ;
        ;
        score3string="" ;
        ;
        score4string="" ;
        ;
        }
        ;
        battle.DisableBattleInput();
        battle.PauseCountdownTimer();
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("");
        tb1.WaitUntilDone();
        tb1.Close();
        battle.UnpauseCountdownTimer();
        battle.EnableBattleInput();
    }

    public string ScoreStringTest(int teamNumber)
    {
        battle=InterfaceBattle() ;
        ;
        gb=InterfaceGlobals() ;
        ;
        string teamName=battle.GetTeamName(teamNumber) ;
        ;
        if(teamNumber==0){
        if(gb.GetHero()=="Valens"){
        teamName="##--Munio's School" ;
        ;
        return teamName;
        ;
        }
        else {
        teamName="##--Orin's School" ;
        ;
        return teamName;
        ;
        }
        ;
        }
        ;
        if(teamName==""){
        string returnString="" ;
        ;
        return returnString;
        ;
        }
        else {
        return teamName;
        ;
        }
        ;
    }

    public int CheckForTie(UNKNOWN teampoints)
    {
        highScore=0 ;
        ;
        int numWithHighScore=0 ;
        ;
        for(int i=0;i<3;i+=1)
        {if(teampoints[i]>highScore){
        highScore=teampoints[i] ;
        ;
        }
        ;
        };
        for(int i=0;i<3;i+=1)
        {if(teampoints[i]==highScore){
        numWithHighScore+=1 ;
        ;
        }
        ;
        };
        if(numWithHighScore>1){
        return 1;
        ;
        }
        else {
        return 0;
        ;
        }
        ;
    }

    public void ReportWinningTeam(UNKNOWN wintemp)
    {
        globals=InterfaceGlobals() ;
        ;
        battle=InterfaceBattle() ;
        ;
        if(wintemp==999){
        battle.DisableBattleInput();
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Overtime!");
        tb1.WaitUntilDone();
        tb1.Close();
        battle.EnableBattleInput();
        }
        else {
        winteam=1 ;
        ;
        battle.DisableBattleInput();
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        string stringToSet="##-- wins!" ;
        ;
        tb1.SetText(stringToSet);
        tb1.WaitUntilDone();
        tb1.Close();
        battle.EnableBattleInput();
        }
        ;
    }

    public void SkirmishLoop(UNKNOWN theskirmish,int barrelsLeft)
    {
        BattleEnumSetup();
        targets=theskirmish.GetTargetList() ;
        ;
        skirmishNode=targets.GetFirst() ;
        ;
        while(theskirmish.DoSkirmishEndClassChatter(skirmishNode)!=0){Sleep(0);};
        targets=theskirmish.GetTargetList() ;
        ;
        skirmishNode=targets.GetFirst() ;
        ;
        beenCounted=None ;
        ;
        while(skirmishNode!=){if(skirmishNode.mFlags&&TARGET_RESULT_HIT){
        unit=skirmishNode.GetDefender() ;
        ;
        name=unit.GetClassName() ;
        ;
        if(name=="PropBarrel"&&unit.IsDead()){
        if(beenCounted!=unit){
        attacker=skirmishNode.GetAttacker() ;
        ;
        teamtemp=attacker.GetCurrentTeam() ;
        ;
        battle_destroybarrels_template.UpdateTeamPoints(teamtemp,1,teampoints);
        barrelsLeft[0]-=1 ;
        ;
        beenCounted=unit ;
        ;
        }
        ;
        }
        ;
        }
        skirmishNode=targets.GetNext() ;
        ;};
        battle_destroybarrels_template.ReportScore(teampoints);
        default_battle.DefaultCrowdUpdate(theskirmish);
    }

    public void VictoryConditions(UNKNOWN timer,UNKNOWN teampoints,UNKNOWN barrelsLeft,UNKNOWN overtimeDuration)
    {
        battle=InterfaceBattle() ;
        ;
        if(timer[0]==0||barrelsLeft[0]==0){
        bestpoints=0 ;
        ;
        int winner=1 ;
        ;
        for(int n=0;n<3;n+=1)
        {if(teampoints[n]>bestpoints){
        bestpoints=teampoints[n] ;
        ;
        winner=n ;
        ;
        }
        ;
        };
        if(CheckForTie(teampoints)){
        winner=999 ;
        ;
        printf("\n");
        }
        ;
        battle_destroybarrels_template.ReportWinningTeam(winner);
        if(winner!=999){
        battle.SetWinningTeam(winner);
        timer[0]=1 ;
        ;
        }
        else {
        battle.SetCountDownTimer(overtimeDuration);
        timer[0]=battle.GetCountDownTimer() ;
        ;
        }
        ;
        }
        ;
    }

    public int BattleInstructions(int doOnce)
    {
        battle=InterfaceBattle() ;
        ;
        globals=InterfaceGlobals() ;
        ;
        CreateIntVar("JSDBarrelInstruct",0);
        battle=InterfaceBattle() ;
        ;
        globals=InterfaceGlobals() ;
        ;
        cursor=InterfaceMapCursor() ;
        ;
        if(doOnce==0){
        int numTeams=1 ;
        ;
        for(int i=0;i<numTeams;i+=1)
        {battle_utils.AddTeamPriorityTargetClass(i,"Prop",1);
        };
        if(! GetIntVar("JSDBarrelInstruct")){
        battle.DisableBattleInput();
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Welcome!  Do you need instructions for this battle?");
        tb1.Choice("##--Yes, what's with all the barrels?");
        tb1.Choice("##--Maybe later.");
        tb1.Choice("##--Don't ask me again.");
        tb1.WaitUntilDone();
        tb1.Close();
        if(tb1.GetChoice()==1){
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetHeight(3);
        tb1.SetText("##--Destroy as many barrels as you can in the alloted time.");
        tb1.AddText("##--The winner of this battle will be the team that has destroyed the most.");
        tb1.WaitUntilDone();
        tb1.Close();
        }
        else if(tb1.GetChoice()==3){
        SetIntVar("JSDBarrelInstruct",1);
        }
        ;
        }
        ;
        battle.EnableBattleInput();
        int doOnce=1 ;
        ;
        }
        ;
        return doOnce;
        ;
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
