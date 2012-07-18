using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameStateManagement;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.util.debug;
using com.xexuxjy.magiccarpet.spells;

namespace com.xexuxjy.magiccarpet.gui
{
    public class PlayerHud : GameObject
    {
        public PlayerHud(GameplayScreen gamePlayScreen) : base(GameObjectType.gui)
        {
            m_gamePlayScreen = gamePlayScreen;
            m_guiComponents = new List<GuiComponent>(10);
         
        }

        public void Initialise()
        {
            // after init so we get the right draw order.
            DrawOrder = Globals.GUI_DRAW_ORDER;

            //Globals.GameObjectManager.AddAndInitializeObject(new FrameRateCounter(Globals.DebugTextFPS, Globals.DebugDraw));
            Globals.GameObjectManager.AddAndInitializeObject(this);

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
            m_spellSelector = new SpellSelector(spellSelectorTopLeft, width);
            AddAndInitializeGuiComponent(m_spellSelector);

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

        public override void Update(GameTime gameTime)
{
            foreach(GuiComponent guiComponent in m_guiComponents)
            {
                if (guiComponent.Enabled)
                {
                    guiComponent.Update(gameTime);
                }
            }

            if (Globals.TrackedObject != null)
            {
                Vector3 location = Globals.DebugTextCamera;
                Vector3 colour = new Vector3(1, 1, 1);

                if (Globals.TrackedObject != null && Globals.playerController != null)
                {
                    String baseInfo = String.Format("Player Forward[{1}] Camera Eye[{2}]. ", Globals.TrackedObject.Position, Globals.TrackedObject.Forward, Globals.Camera.Eye);
                    Globals.DebugDraw.DrawText(baseInfo, location, colour);
                    baseInfo = String.Format("Y[{0:0.000}] P[{1:0.000}] LY[{2:0.000}] LP[{3:0.000}]. ", Globals.playerController.CurrentYaw, Globals.playerController.CurrentPitch, Globals.playerController.LastYaw, Globals.playerController.LastPitch);
                    Globals.DebugDraw.DrawText(baseInfo, location + new Vector3(0, 10, 0), colour);
                }
                if (Globals.TrackedObject.SpellComponent.GetActiveSpells().Count > 0)
                {
                    Spell spell = Globals.TrackedObject.SpellComponent.GetActiveSpells()[0];
                    if (spell.Heading != Globals.TrackedObject.Heading)
                    {
                        int ibreak = 0;
                    }
                    //String spellInfo = String.Format("Spell Forward[{1}]. ", spell.Position, spell.Forward);
                    //location.Y += 15;
                    //Globals.DebugDraw.DrawText(spellInfo, location,colour);
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            foreach(GuiComponent guiComponent in m_guiComponents)
            {
                if (guiComponent.Visible)
                {
                    //guiComponent.Draw(gameTime);
                }
            }

            // restore from sprite batch changes.
            //Game.GraphicsDevice.BlendState = BlendState.Opaque;
            //Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        }

        public void HandleInput(InputState inputState)
        {
            foreach (GuiComponent guiComponent in m_guiComponents)
            {
                if (guiComponent.HasGuiControl)
                {
                    guiComponent.HandleInput(inputState);
                }
            }


        }



        public void ToggleSpellSelector()
        {
            m_spellSelector.Visible = !m_spellSelector.Visible;
            m_spellSelector.HasGuiControl = m_spellSelector.Visible;
        }



        private List<GuiComponent> m_guiComponents;


        private SpriteBatch m_spriteBatch;
        private GameplayScreen m_gamePlayScreen;
        private SpellSelector m_spellSelector;
    }
}
