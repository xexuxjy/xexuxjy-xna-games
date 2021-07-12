public class battle_points_template : BaseScript
{
    public void OnInitBattle()
    {
    }

    public void OnStartBattle()
    {
    }

    public void OnExecBattle()
    {
    }

    public void UpdateTeamPoints(UNKNOWN team,UNKNOWN points,UNKNOWN teampoints)
    {
        for(int n=0;n<3;n+=1)
        {if(team==n&&points>0){
        teampoints[n]+=points ;
        ;
        }
        ;
        };
    }

    public void OnSkirmishEnd(UNKNOWN theskirmish)
    {
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

    public void ExecLoop(UNKNOWN timer,UNKNOWN teampoints)
    {
        battle=InterfaceBattle() ;
        ;
        if(timer[0]==0){
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
        battle_points_template.ReportWinningTeam(winner,teampoints);
        if(winner!=999){
        battle.SetWinningTeam(winner);
        SetIntVar("JSDCurrentPoints",1);
        timer[0]=1 ;
        ;
        }
        else {
        battle.SetCountDownTimer(60);
        timer[0]=battle.GetCountDownTimer() ;
        ;
        }
        ;
        }
        ;
        sleep(0);
    }

    public void SkirmishLoop(UNKNOWN theskirmish,UNKNOWN pointThreshold,UNKNOWN teampoints)
    {
        if(! GetIntVar("JSDCurrentPoints")){
        BattleEnumSetup();
        int DamageCalc=0 ;
        ;
        targets=theskirmish.GetTargetList() ;
        ;
        thetarget=targets.GetFirst() ;
        ;
        while(thetarget!=){if(thetarget.mFlags& TARGET_RESULT_HIT){
        DamageCalc=thetarget.mResult_Damage ;
        ;
        if(DamageCalc<0){
        DamageCalc=0 ;
        ;
        }
        ;
        attacker=thetarget.GetAttacker() ;
        ;
        team=attacker.GetCurrentTeam() ;
        ;
        UpdateTeamPoints(team,DamageCalc,teampoints);
        }
        thetarget=targets.GetNext() ;
        ;};
        battle_points_template.ReportScore(teampoints);
        default_battle.DefaultCrowdUpdate(theskirmish);
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

    public void ReportWinningTeam(UNKNOWN wintemp,UNKNOWN teampoints)
    {
        globals=InterfaceGlobals() ;
        ;
        if(wintemp==999){
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Overtime!");
        Sleep(2.0f);
        tb1.Close();
        }
        else {
        winteam=1 ;
        ;
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        string stringToSet="##-- wins!" ;
        ;
        tb1.SetText(stringToSet);
        Sleep(2.0f);
        tb1.Close();
        }
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

    public int BattleInstructions(int doOnce,UNKNOWN pointThreshold)
    {
        CreateIntVar("JSDPointsInstruct",0);
        CreateIntVar("JSDCurrentPoints",0);
        battle=InterfaceBattle() ;
        ;
        globals=InterfaceGlobals() ;
        ;
        cursor=InterfaceMapCursor() ;
        ;
        if(doOnce==0){
        battle.DisableAutoWinCheck();
        battle.SetDamageMode(2);
        SetIntVar("JSDCurrentPoints",0);
        if(! GetIntVar("JSDPointsInstruct")){
        battle.DisableBattleInput();
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Welcome!  Do you need instructions for Points Battles?");
        tb1.Choice("##--Yes, how is this game won?");
        tb1.Choice("##--Maybe later.");
        tb1.Choice("##--Don't ask me again.");
        tb1.WaitUntilDone();
        tb1.Close();
        if(tb1.GetChoice()==1){
        Sleep(1);
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--It's all about hitting as hard as you can.");
        tb1.AddText("##--You will score a point for each point of damage you do.");
        tb1.WaitUntilDone();
        tb1.Close();
        }
        else if(tb1.GetChoice()==3){
        SetIntVar("JSDPointsInstruct",1);
        }
        ;
        }
        ;
        int doOnce=1 ;
        ;
        }
        ;
        battle.EnableBattleInput();
        return doOnce;
        ;
    }
}
