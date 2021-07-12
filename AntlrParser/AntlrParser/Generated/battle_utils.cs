public class battle_utils : BaseScript{    public void PerformBestAction(ScriptActor attacker)    {
        InterfaceBattle battle = InterfaceBattle();
        ;
        battle.PauseCountdownTimer();
        cursor = InterfaceMapCursor();
        ;
        cursor.SetLocation(attacker.GetGridX(), attacker.GetGridY(), false);
        Sleep(0.0f);
        while (!battle.IsCameraSettled()) { Sleep(0.0f); };
        Sleep(0.0f);
        ai = InterfaceUnitAI();
        ;
        if (ai.Init(attacker))
        {
           while (ai.Update()) { Sleep(0.0f); };
           ai.Cleanup();
        }
        else
        {
           attacker.OrderPass();
        }
        ;
        battle.UnpauseCountdownTimer();
    }
   public static void PerformBestActionOnUnit(UNKNOWN attacker, UNKNOWN defenderName)
    {
        ai = InterfaceUnitAI();
        ;
        ai.SetTargetOverride(defenderName);
        PerformBestAction(attacker);
        ai.ClearTargetOverride();
    }
   public static int GetNumEnemies(int ToWho)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numEnemies = 0;
        ;
        while (TestUnit)
        {
            if (TestUnit.IsAlive() && TestUnit.IsEnemy(ToWho))
            {
               name = TestUnit.GetName();
               ;
               printf(".\n");
               printf("\n");
               numEnemies += 1;
               ;
               printf("\n");
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numEnemies;
        ;
    }
   public static int GetNumFriends(UNKNOWN ToWho)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numFriends = 0;
        ;
        while (TestUnit)
        {
            if (TestUnit != ToWho && TestUnit.IsAlive() && TestUnit.IsFriend(ToWho))
            {
               numFriends += 1;
               ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numFriends;
        ;
    }
   public static int IsNamedUnitOnTeam(string name, int TeamNum)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numUnitszz = 0;
        ;
        int listSize = 0;
        ;
        while (TestUnit)
        {
            listSize += 1;
           if (TestUnit.GetName() == name && TestUnit.GetCurrentTeam() == TeamNum)
            {
               numUnitszz += 1;
               ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        if (numUnitszz > 0)
        {
           return 1;
           ;
        }
        else
        {
           return 0;
           ;
        }
        ;
    }
   public static int GetNumOnTeam(int TeamNum)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numUnitszz = 0;
        ;
        int listSize = 0;
        ;
        while (TestUnit)
        {
            listSize += 1;
           if (TestUnit.IsAlive() && TestUnit.GetCurrentTeam() == TeamNum)
            {
               numUnitszz += 1;
               ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numUnitszz;
        ;
    }
   public static int FindNumberTeams()
    {
        int numTemp = 0;
        ;
        for (int i = 0; i < 3; i += 1)
        {
            int teamtemp = battle_utils.GetNumEverOnTeam(i);
           ;
           if (teamtemp > 0)
            {
               numTemp += 1;
               ;
           }
        ;
        };
        return numTemp;
        ;
    }
   public static int GetNumClassOnTeam(UNKNOWN TeamNum, UNKNOWN ClassName)
    {
        gb = InterfaceGlobals();
        ;
        UnitList = gb.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numUnitszz = 0;
        ;
        int listSize = 0;
        ;
        while (TestUnit)
        {
            listSize += 1;
           if (TestUnit.IsAlive() && TestUnit.GetCurrentTeam() == TeamNum)
            {
               if (TestUnit.GetBaseClassName() == ClassName)
                {
                   numUnitszz += 1;
                   ;
               }
           ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numUnitszz;
        ;
    }
   public static void GetNumberTargetClass(UNKNOWN classname)
    {
        numClass = 0;
        ;
        int numTeams = 1;
        ;
        for (int j = 1; j < numTeams; j += 1)
        {
            int numClassTemp = battle_utils.GetNumClassOnTeam(j, classname);
           ;
           numClass = numClassTemp;
           ;
        };
        numClassTemp = battle_utils.GetNumClassOnTeam(1, classname);
        ;
        numClass = numClassTemp;
        ;
        return numClass;
        ;
    }
   public static void OrderProtectClass(UNKNOWN classname)
    {
        for (int k = 1; k < 3; k += 1)
        {
            int teamtemp = battle_utils.GetNumOnTeam(k);
           ;
           if (teamtemp > 0)
            {
               battle_utils.AddTeamPriorityDefendClass(k, classname, 1);
           }
        ;
        };
        return numTemp;
        ;
    }
   public static void AddTeamPriorityDefendClass(int TeamNum, int ClassName, UNKNOWN Priority)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TeamNum == TestUnit.GetCurrentTeam() && TestClass == TestUnit.GetBaseClassName())
            {
               AddTeamPriorityDefendUnit(TeamNum, TestUnit.GetName(), Priority);
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
    }
   public static void AddTeamPriorityTargetClass(string TeamNum, int ClassName, UNKNOWN Priority)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TeamNum == TestUnit.GetCurrentTeam() && TestClass == TestUnit.GetBaseClassName())
            {
               AddTeamPriorityTarget(TeamNum, TestUnit.GetName(), Priority);
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
    }
   public static void AddUnitIgnoreTeam(string UnitName, int TeamNumber)
    {
        globals = InterfaceGlobals();
        ;
        tactics = InterfaceBattleTactics();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TestUnit.GetCurrentTeam() == TeamNumber)
            {
               IgnoreName = TestUnit.GetName();
               ;
               tactics.AddUnitIgnoreUnit(UnitName, IgnoreName);
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
    }
   public static int GetClassExterminated(UNKNOWN triggerClass)
    {
        globals = InterfaceGlobals();
        ;
        tactics = InterfaceBattleTactics();
        ;
        UnitList = globals.GetUnitList();
        ;
        effects = InterfaceEffects();
        ;
        int numClass = 0;
        ;
        int numDead = 0;
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TestUnit.GetBaseClassName() == triggerClass && TestUnit.GetCurrentTeam() != 0)
            {
               numClass += 1;
               ;
               if (TestUnit.IsDead())
                {
                   numDead += 1;
                   ;
               }

;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        if (numDead == numClass && numClass)
        {
           return 1;
           ;
        }
        ;
        return 0;
        ;
    }
   public static void ActivateReinforcements(UNKNOWN startLoc, UNKNOWN appearArea)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        unitList.ActivateReinforcementWave(startLoc);
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TestUnit.GetStartPos() == startLoc)
            {
               battle_utils.TeleportToArea(TestUnit, appearArea);
           }
           if (TestUnit.GetBaseClassName() == "Prop")
            {
               TestUnit.Deactivate();
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
    }
   public static void TeleportToArea(UNKNOWN unit, UNKNOWN appearArea)
    {
        math = InterfaceMath();
        ;
        unitMap = InterfaceUnitMap();
        ;
        effects = InterfaceEffects();
        ;
        gridX = math.RandomInRange(appearArea[0], appearArea[2]);
        ;
        gridY = math.RandomInRange(appearArea[1], appearArea[3]);
        ;
        while (!unitmap.IsLocationEmpty(gridX, gridY))
        {
            gridX = math.RandomInRange(appearArea[0], appearArea[2]);
           gridY = math.RandomInRange(appearArea[1], appearArea[3]);
           ;
        };
        if (unitmap.IsLocationEmpty(gridX, gridY))
        {
           unitName = unit.GetName();
           ;
           unit.SetGridPos(gridX, gridY);
           effects.FireOnUnit("summoner_AffinityPortalFlash.emt", unitName);
        }
        ;
    }
   public static int GetNumDeadPlayerEnemies()
    {
        gb = InterfaceGlobals();
        ;
        UnitList = gb.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numUnitszz = 0;
        ;
        while (TestUnit)
        {
            if (TestUnit.IsAlive() && TestUnit.GetCurrentTeam() != 0)
            {
               numUnitszz += 1;
               ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numUnitszz;
        ;
    }
   public static int IsTeamExterminated(int triggerTeam)
    {
        globals = InterfaceGlobals();
        ;
        tactics = InterfaceBattleTactics();
        ;
        UnitList = globals.GetUnitList();
        ;
        effects = InterfaceEffects();
        ;
        int numClass = 0;
        ;
        int numDead = 0;
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TestUnit.GetCurrentTeam() == triggerTeam)
            {
               numClass += 1;
               ;
               if (TestUnit.IsDead())
                {
                   numDead += 1;
                   ;
               }

;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        if (numDead == numClass && numClass)
        {
           return 1;
           ;
        }
        ;
        return 0;
        ;
    }
   public static string GetOfficerPortrait()
    {
        gb = InterfaceGlobals();
        ;
        town = gb.GetCurTownRefName();
        ;
        printf(town);
        string leagueOfficerPic = ".tga";
        ;
        return leagueOfficerPic;
        ;
    }
   public static int GetNumFullClassOnTeam(UNKNOWN TeamNum, UNKNOWN ClassName)
    {
        gb = InterfaceGlobals();
        ;
        UnitList = gb.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numUnitszz = 0;
        ;
        int listSize = 0;
        ;
        while (TestUnit)
        {
            listSize += 1;
           if (TestUnit.IsAlive() && TestUnit.GetCurrentTeam() == TeamNum)
            {
               if (TestUnit.GetClassName() == ClassName)
                {
                   numUnitszz += 1;
                   ;
               }
           ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numUnitszz;
        ;
    }
   public static void GetNumberTargetFullClass(UNKNOWN classname)
    {
        numClass = 0;
        ;
        int numTeams = 1;
        ;
        for (int j = 1; j < numTeams; j += 1)
        {
            int numClassTemp = battle_utils.GetNumFullClassOnTeam(j, classname);
           ;
           numClass = numClassTemp;
           ;
        };
        return numClass;
        ;
    }
   public static int GetNumEverOnTeam(UNKNOWN TeamNum)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        int numUnitszz = 0;
        ;
        int listSize = 0;
        ;
        while (TestUnit)
        {
            listSize += 1;
           if (TestUnit.GetCurrentTeam() == TeamNum)
            {
               numUnitszz += 1;
               ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        return numUnitszz;
        ;
    }
   public static void GetUnitOnTeam(int TeamNum)
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            if (TestUnit.GetCurrentTeam() == TeamNum)
            {
               printf("\n");
               LeaveCriticalSection();
               return TestUnit;
               ;
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        ;
    }
   public static void ActivateEveryone()
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            TestUnit.Activate()TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        ;
    }
   public static void DeactivateEveryone()
    {
        globals = InterfaceGlobals();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            TestUnit.Deactivate()TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
        ;
    }
   public static void AddTeamIgnoreBaseClass(int BaseClass, UNKNOWN TeamNumber)
    {
        globals = InterfaceGlobals();
        ;
        tactics = InterfaceBattleTactics();
        ;
        UnitList = globals.GetUnitList();
        ;
        EnterCriticalSection();
        TestUnit = UnitList.GetFirst();
        ;
        while (TestUnit)
        {
            printf("\n")printf("\n")if (TestUnit.GetBaseClassName() == BaseClass)
            {
               IgnoreName = TestUnit.GetName();
               ;
               printf("\n");
               tactics.AddTeamIgnoreUnit(TeamNumber, IgnoreName);
           }
           TestUnit = UnitList.GetNext();
           ;
        };
        LeaveCriticalSection();
    }
}
