public class battle_kingofthehill_template : BaseScript
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
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[2],hill[3])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[2],hill[3]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[4],hill[5])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[4],hill[5]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[6],hill[7])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[6],hill[7]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[8],hill[9])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[8],hill[9]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[10],hill[11])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[10],hill[11]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[12],hill[13])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[12],hill[13]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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
        }
        ;
        if(unitmap.IsLocationEmpty(hill[14],hill[15])!=1){
        scoringUnit=unitmap.GetUnitAtLocation(hill[14],hill[15]) ;
        ;
        scoringTeam=scoringUnit.GetCurrentTeam() ;
        ;
        print(" = scoringTeam.\n");
        if(scoringUnit.IsAlive()){
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

    public void VictoryConditions(UNKNOWN timer,UNKNOWN teampoints)
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
        battle=InterfaceBattle() ;
        ;
        if(wintemp==999){
        battle_kingofthehill_template.ReportScore(teampoints);
        battle.DisableBattleInput();
        tb1=globals.CreateTalkBox(50,50,"League Officer",battle_utils.GetOfficerPortrait()) ;
        ;
        tb1.SetText("##--Tie game!  Overtime!");
        tb1.WaitUntilDone();
        tb1.Close();
        battle.EnableBattleInput();
        }
        else {
        battle_kingofthehill_template.ReportScore(teampoints);
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
        tactics.AddTeamPriorityEnterLocation(n,hill[6],hill[5],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[4],hill[7],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[8],hill[9],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[10],hill[11],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[12],hill[13],1);
        tactics.AddTeamPriorityEnterLocation(n,hill[14],hill[15],1);
        };
        battle=InterfaceBattle() ;
        ;
        battle.SetDeadBodyTimeout(1);
    }
}
