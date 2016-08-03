using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameStateManager
{
    private GameState m_gameState = GameState.Town;
    //private CommonState m_stateData = null;
    private LoadingScreen m_loadingScreen;

    private ZoneTransitionData m_zoneTransitionData;

    private LocalisationData m_localisationData;
    public LocalisationData LocalisationData
    {
        get { return m_localisationData; }
        private set
        {
            m_localisationData = value;
        }

    }

    private ItemManager m_itemManager;
    public ItemManager ItemManager
    {
        get { return m_itemManager; }
        private set { m_itemManager = value; }
    }

    private TownManager m_townManager;
    public TownManager TownManager
    {
        get { return m_townManager; }
        private set { m_townManager = value; }
    }


    public void StartGame()
    {
        Application.targetFrameRate = 30;
        //m_stateData = new CommonState();
        GameObject go = Resources.Load("Prefabs/LoadingScreenPrefab") as GameObject;
        //dfGUIManager.inAddPrefab
        //dfControl dfc = dfGUIManager.ActiveManagers.FirstOrDefault().AddPrefab(go);
        //if (dfc != null)
        //{
        //    m_loadingScreen = dfc.gameObject.GetComponent<LoadingScreen>();
        //}
        LocalisationData = new LocalisationData();
        LocalisationData.Load();

        ItemManager = new ItemManager();
        ItemManager.Load();

        TownManager = new TownManager();
        TownManager.Load();


        ActorGenerator.InitClassCategories();

    }


    //public static ItemManager ItemManager = new ItemManager();
    //public static EventLogger EventLogger = new EventLogger(null);

    //public void SetStateData(CommonState newStateData)
    //{
    //    // copy the current state data into the new?
    //    if (m_stateData != null)
    //    {
    //        m_stateData.StateCleanup();
    //    }

    //    newStateData.CopyState(m_stateData);
    //    m_stateData = newStateData;

    //    if(m_stateData != null)
    //    {
    //        m_stateData.StateInit();
    //    }
    //}



    public void SetNewState(GameState gameState)
    {
        if (m_gameState != gameState)
        {
            if (m_gameState == GameState.Arena)
            {
                //CleanupArenaState();
            }

            m_gameState = gameState;

            switch (m_gameState)
            {
                case (GameState.Arena):
                    {
                        //Application.LoadLevel("ArenaScene");
                        m_loadingScreen.Load("ArenaScene");
                        break;
                    }
                case (GameState.GameOverLose):
                    {
                        m_loadingScreen.Load("GameOverLose");
                        break;
                    }
                case (GameState.GameOverWin):
                    {
                        m_loadingScreen.Load("GameOverLose");
                        break;
                    }
                case (GameState.Town):
                    {
                        m_loadingScreen.Load("TownScene");
                        break;
                    }
                case (GameState.Overland):
                    {
                        m_loadingScreen.Load(GladiusGlobals.SceneForZone(m_zoneTransitionData.newZone));
                        break;
                    }

            }
        }
    }

    //    private void CleanupArenaState()
    //    {
    //        m_stateData.TurnManager = null;
    //        m_stateData.LOSTester = null;
    //        m_stateData.Arena = null;
    //        m_stateData.CombatEngine = null;
    //        m_stateData.CombatEngineUI = null;
    //        m_stateData.Crowd = null;
    //        m_stateData.MovementGrid = null;
    //    }

//    public UserControl UserControl
//    {
//        get;
//        set;
//    }

//    public GameState CurrentState
//    {
//        get { return m_gameState; }
//    }

//    public CommonState StateData
//    {
//        get
//        {
//            return m_stateData;
//        }
//    }


}

//public class CommonState
//{
//    public CameraManager CameraManager;
//    public GladiatorSchool GladiatorSchool;
//    public TownData TownData;
//    public ArenaEncounter Encounter;
//    public ZoneTransitionData ZoneTransitionData;

//    // Arena Specific
//    public TurnManager TurnManager;
//    public Arena Arena;
//    public MovementGrid MovementGrid;
//    public CombatEngine CombatEngine;
//    public CombatEngineUI CombatEngineUI;
//    public Crowd Crowd;
//    public BattleData BattleData;
//    public LOSTester LOSTester;

//}

