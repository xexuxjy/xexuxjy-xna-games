using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    public class GameStateManager
    {
        private GameState m_gameState;
        private CommonState m_stateData = null;

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

            LocalisationData = new LocalisationData();
            LocalisationData.Load();

            ItemManager = new ItemManager();
            ItemManager.Load();

            TownManager = new TownManager();
            TownManager.Load();


            ActorGenerator.InitCategories();

        }


        //public static ItemManager ItemManager = new ItemManager();
        //public static EventLogger EventLogger = new EventLogger(null);

        public void SetStateData(CommonState o)
        {
            m_stateData = o;
        }

        public void SetNewState(GameState gameState,object o)
        {
                if (m_gameState != gameState)
                {
                    m_gameState = gameState;

                    switch (m_gameState)
                    {
                        case(GameState.GameOverLose):
                            {
                                Application.LoadLevel("GameOverLose");
                                break;
                            }
                        case(GameState.GameOverWin):
                            {
                                Application.LoadLevel("GameOverLose");
                                break;
                            }
                        case(GameState.Town):
                            {
                                Application.LoadLevel("Town");
                                break;
                            }
                        case(GameState.Shop):
                            {
                                Application.LoadLevel("GameOverLose");
                                break;
                            }
                        case(GameState.OverlandImperia):
                            {
                                Application.LoadLevel("ImperiaWorldMap");
                                break;
                            }
                        case(GameState.OverlandNordargh):
                            {
                                Application.LoadLevel("NordarghWorldMap");
                                break;
                            }

                    }
                }
        }

        public UserControl UserControl
        {
            get;
            set;
        }

        public CommonState CurrentStateData
        {
            get
            {
                return m_stateData;
            }
        }

        public ArenaStateCommon ArenaStateCommon
        {
            get 
            {
                System.Diagnostics.Debug.Assert(m_stateData is ArenaStateCommon);
                return m_stateData as ArenaStateCommon; 
            }
        }

        public OverlandStateCommon OverlandStateCommon
        {
            get
            {
                System.Diagnostics.Debug.Assert(m_stateData is OverlandStateCommon);
                return m_stateData as OverlandStateCommon; 
            }

        }

        public ShopStateCommon ShopStateCommon
        {
            get
            {
                System.Diagnostics.Debug.Assert(m_stateData is ShopStateCommon);
                return m_stateData as ShopStateCommon;
            }

        }

    }

    public class CommonState
    {
        public CameraManager CameraManager;
        public GladiatorSchool GladiatorSchool;
        public TownData TownData;
    }

    public class ArenaStateCommon : CommonState
    {
        public TurnManager TurnManager;
        public Arena Arena;
        public MovementGrid MovementGrid;
        public CombatEngine CombatEngine;
        public CombatEngineUI CombatEngineUI;
        public Crowd Crowd;
        public BattleData BattleData;
        public LOSTester LOSTester;
    }

    public class OverlandStateCommon : CommonState
    {
    }

    public class ShopStateCommon : CommonState
    {

    }

