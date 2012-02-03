using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameStateManagement;
using BulletXNADemos.Demos;

namespace com.xexuxjy.magiccarpet.gui
{
    public class PlayerHud : DrawableGameComponent
    {
        public PlayerHud(GameplayScreen gamePlayScreen) : base(Globals.Game)
        {
            m_gamePlayScreen = gamePlayScreen;
        }

        public void Initialise()
        {

            m_gamePlayScreen.AddComponent(new FrameRateCounter(Globals.Game, Globals.DebugTextFPS, Globals.DebugDraw));

            //int x = 650;
            //int y = 10;
            //int width = 100;
            //Globals.MiniMap = new MiniMap(x,y,width);
            //Globals.MiniMap.Initialize();
            //AddComponent(Globals.MiniMap);


            int x = 600;
            int y = 100;
            int width = 100;
            //PerlinTest perlinTest = new PerlinTest(x, y, width);
            //perlinTest.Initialize();
            //AddComponent(perlinTest);
            Globals.MiniMap = new MiniMap(x, y, width);
            Globals.MiniMap.Initialize();
            m_gamePlayScreen.AddComponent(Globals.MiniMap);

            //SpellSelector spellSelector = new SpellSelector(x, y, width);
            //spellSelector.Initialize();
            //AddComponent(spellSelector);

            //EventWindow eventWindow = new EventWindow(x, y, width);
            //eventWindow.Initialize();
            //AddComponent(eventWindow);
            //int counter = 1;
            //int numLines = 13;
            //for (int i = 0; i < numLines; ++i)
            //{
            //    eventWindow.AddEventText("This is line " + counter++);
            //}


        }



        private GameplayScreen m_gamePlayScreen;
        private SpellSelector m_spellSelector;
        private MiniMap m_miniMap;
        private EventWindow m_eventWindow;
    }
}
