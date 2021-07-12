public class battle_domination_template : BaseScript
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

    public void ExecLoop(UNKNOWN timer,int targetTime,int teampoints,UNKNOWN hill,UNKNOWN timeIncrement)
    {
        ;
        ;
        ;
        if(timer[0]<targetTime[0]){
        if(unitmap.IsLocationEmpty(hill[0],hill[1])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[0],hill[1]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        for(int x=0;x<3;x+=1)
        {if(scoringTeam==x){
        if(timer[1]>=timer[0]){
        teampoints[x]+=2 ;
        ;
        }
        else {
        teampoints[x]+=1 ;
        ;
        }
        ;
        }
        ;
        };
        }
        ;
        if(unitmap.IsLocationEmpty(hill[2],hill[3])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[2],hill[3]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        for(int x=0;x<3;x+=1)
        {if(scoringTeam==x){
        if(timer[1]>=timer[0]){
        teampoints[x]+=2 ;
        ;
        }
        else {
        teampoints[x]+=1 ;
        ;
        }
        ;
        }
        ;
        };
        }
        ;
        if(unitmap.IsLocationEmpty(hill[4],hill[5])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[4],hill[5]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        for(int x=0;x<3;x+=1)
        {if(scoringTeam==x){
        if(timer[1]>=timer[0]){
        teampoints[x]+=2 ;
        ;
        }
        else {
        teampoints[x]+=1 ;
        ;
        }
        ;
        }
        ;
        };
        }
        ;
        if(targetTime[0]>timeIncrement){
        targetTime[0]=timeIncrement ;
        ;
        }
        else {
        targetTime[0]=0 ;
        ;
        }
        ;
        ReportScore(teampoints);
        }
        ;
    }

    public void VictoryConditions(UNKNOWN timer,UNKNOWN teampoints,UNKNOWN targetTime)
    {
        battle=InterfaceBattle() ;
        ;
        gb=InterfaceGlobals() ;
        ;
        if(timer[1]>=timer[0]&&timer[3]==0){
        tb1=gb.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Double points!  Watch for a come-from-behind!");
        Sleep(2.0f);
        tb1.Close();
        timer[3]=1 ;
        ;
        }
        ;
        if(timer[0]==0&&timer[4]==0){
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
        battle_domination_template.ReportWinningTeam(winner,teampoints);
        if(winner!=999){
        battle.SetWinningTeam(winner);
        timer[4]=1 ;
        ;
        }
        else {
        battle.SetCountDownTimer(timer[2]);
        timer[0]=battle.GetCountDownTimer() ;
        ;
        targetTime[0]= ;
        ;
        }
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

    public void ReportWinningTeam(UNKNOWN wintemp,UNKNOWN teampoints)
    {
        globals=InterfaceGlobals() ;
        ;
        if(wintemp==999){
        battle_domination_template.ReportScore(teampoints);
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Overtime!");
        Sleep(2.0f);
        tb1.Close();
        }
        else {
        battle_domination_template.ReportScore(teampoints);
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

    public void GiveOrders(UNKNOWN hill)
    {
        tactics=InterfaceBattleTactics() ;
        ;
        int numTeams=1 ;
        ;
        for(int n=0;n<numTeams;n+=1)
        {tactics.AddTeamPriorityEnterLocation(n,hill[0],hill[1],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[2],hill[3],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[4],hill[5],1);
        };
        battle=InterfaceBattle() ;
        ;
        battle.SetDeadBodyTimeout(1);
    }

    public int BattleInstructions(int doOnce,UNKNOWN hill)
    {
        CreateIntVar("JSDDomInstruct",0);
        battle=InterfaceBattle() ;
        ;
        globals=InterfaceGlobals() ;
        ;
        cursor=InterfaceMapCursor() ;
        ;
        if(doOnce==0){
        if(! GetIntVar("JSDDomInstruct")){
        battle.DisableBattleInput();
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Welcome!  Do you need instructions for Dominance?");
        tb1.Choice("##--Yes, how is this game won?");
        tb1.Choice("##--Maybe later.");
        tb1.Choice("##--Don't ask me again.");
        tb1.WaitUntilDone();
        tb1.Close();
        if(tb1.GetChoice()==1){
        Sleep(1);
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Glad you asked!  These are hot spots:");
        tb1.WaitUntilDone();
        tb1.Close();
        cursor.SetLocation(hill[0],hill[1],1);
        Sleep(1);
        cursor.SetLocation(hill[2],hill[3],1);
        Sleep(1);
        cursor.SetLocation(hill[4],hill[5],1);
        Sleep(1);
        tb1=globals.CreateTalkBox(40.0f,10.0f,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--About every ten seconds, if a member of a team is standing on a hot spot, that team will earn a point.");
        tb1.AddText("##--The team with the most points when time runs out wins.  A tie results in more time being added to the clock.");
        tb1.AddText("##--Defeating all enemies is also a way to win -- they can't hold hot spots if they're all knocked out.");
        tb1.WaitUntilDone();
        tb1.Close();
        }
        else if(tb1.GetChoice()==3){
        SetIntVar("JSDDomInstruct",1);
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
