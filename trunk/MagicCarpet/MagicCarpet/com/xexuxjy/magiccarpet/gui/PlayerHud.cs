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
            m_guiComponents = new List<GuiComponent>(10);
         
        }

        public void Initialise()
        {
            // after init so we get the right draw order.
            DrawOrder = Globals.GUI_DRAW_ORDER;

            m_gamePlayScreen.AddComponent(new FrameRateCounter(Globals.Game, Globals.DebugTextFPS, Globals.DebugDraw));
            m_gamePlayScreen.AddComponent(this);

            int inset = 10;

            Point playerStatsTopLeft = new Point(inset, inset);
            PlayerStats playerStats = new PlayerStats(playerStatsTopLeft, Globals.MCContentManager.GetTexture("PlayerStatsFrame").Width);
            AddAndInitializeGuiComponent(playerStats);

            int width = 100;
            //PerlinTest perlinTest = new PerlinTest(x, y, width);
            //perlinTest.Initialize();
            //AddComponent(perlinTest);


            int miniMapWidth = Globals.MCContentManager.GetTexture("MiniMapFrame").Width;
            int foo = Globals.GraphicsDeviceManager.PreferredBackBufferWidth - miniMapWidth - inset;

            Point minimapTopLeft = new Point(foo, inset);
            Globals.MiniMap = new MiniMap(minimapTopLeft, miniMapWidth);
            AddAndInitializeGuiComponent(Globals.MiniMap);

            Point spellSelectorTopLeft = new Point(300, 300);
            width = 300;
            SpellSelector spellSelector = new SpellSelector(spellSelectorTopLeft, width);
            AddAndInitializeGuiComponent(spellSelector);

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

        public void AddAndInitializeGuiComponent(GuiComponent guiComponent)
        {
            guiComponent.Initialize();
            m_guiComponents.Add(guiComponent);
        }

        public override void  Update(GameTime gameTime)
{
            foreach(GuiComponent guiComponent in m_guiComponents)
            {
                guiComponent.Update(gameTime);
            }
        }

        public override void  Draw(GameTime gameTime)
        {
            foreach(GuiComponent guiComponent in m_guiComponents)
            {
                guiComponent.Draw(gameTime);
            }

            // restore from sprite batch changes.
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        }



        private List<GuiComponent> m_guiComponents;


        private SpriteBatch m_spriteBatch;
        private GameplayScreen m_gamePlayScreen;
        private SpellSelector m_spellSelector;
    }
}
