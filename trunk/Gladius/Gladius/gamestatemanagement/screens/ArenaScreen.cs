#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dhpoware;
using System.Collections.Generic;
using Gladius.renderer;
using Gladius.actors;
using Gladius;
using Gladius.control;
using Gladius.combat;
using Gladius.util;
using Gladius.gamestatemanagement.screens;
//using com.xexuxjy.magiccarpet.control;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class ArenaScreen : GameScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ArenaScreen()
        {
            //TransitionOnTime = TimeSpan.FromSeconds(1.5);
            //TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (m_content == null)
            {
                m_content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            m_screenComponents = new ScreenGameComponents(ScreenManager.Game);

            Globals.AttackSkillDictionary = new AttackSkillDictionary();
            Globals.AttackSkillDictionary.Populate(m_content);

            SetupArena();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            m_content.Unload();
        }




        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            m_updateCalls++;
            base.Update(gameTime, otherScreenHasFocus, false);

            m_screenComponents.Update(gameTime);

            foreach (IUIElement uiElement in m_uiElementsList)
            {
                uiElement.Update(gameTime);
            }




            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            m_drawCalls++;
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            m_screenComponents.Draw(gameTime);


            // bit yucky... ui elements have one form of draw or another. not both
            foreach (IUIElement uiElement in m_uiElementsList)
            {
                if (uiElement.Visible)
                {
                    uiElement.DrawElement(gameTime, ScreenManager.Game.GraphicsDevice,Globals.Camera);
                }
            }


            m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            foreach (IUIElement uiElement in m_uiElementsList)
            {
                if (uiElement.Visible)
                {
                    uiElement.DrawElement(gameTime, m_spriteBatch);
                }
            }

            m_spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        public void SetupArena()
        {

            m_spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            // Load the sprite font. The sprite font has a 3 pixel outer glow
            // baked into it so we need to decrease the spacing so that the
            // SpriteFont will render correctly.
            m_gameFont= ScreenManager.Game.Content.Load<SpriteFont>("GameFont");
            m_gameFont.Spacing = -4.0f;


            Globals.UserControl = new UserControl(ScreenManager.Game, ScreenManager.input);
            //m_screenComponents.Components.Add(Globals.UserControl);

            m_camera = new Dhpoware.CameraComponent(ScreenManager.Game);
            m_camera.HandleInput(ScreenManager.input);
    
            IGameComponent igc = m_camera as IGameComponent;
            m_screenComponents.Components.Add(igc);

            m_camera.Position = new Vector3(0, 5, -10);
            Globals.Camera = m_camera;

            m_arena = new Arena(32, 32);

            m_camera.CurrentBehavior = Camera.Behavior.FirstPerson;

            m_arenaRenderer = new SimpleArenaRenderer(m_arena,ScreenManager.Game);
            m_screenComponents.Components.Add(m_arenaRenderer);
            //m_arenaRenderer.LoadContent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice);
            m_turnManager = new TurnManager(ScreenManager.Game);
            m_turnManager.ArenaScreen = this;
            m_screenComponents.Components.Add(m_turnManager);

            //String modelName = "Models/ThirdParty/monster-animated-character-XNA";
            String modelName = "Models/ThirdParty/01_warrior";

            List<BaseActor> actors = new List<BaseActor>();
            int numActors = 4;
            for (int i = 0; i < numActors; ++i)
            {
                BaseActor ba1 = new BaseActor(ScreenManager.Game);
                ba1.ModelName = modelName;
                ba1.LoadContent(ScreenManager.Game.Content);
                ba1.Arena = m_arena;
                ba1.DebugName = "Monster" + i;
                actors.Add(ba1);
                m_screenComponents.Components.Add(ba1);
                ba1.SetupSkills(Globals.AttackSkillDictionary);
            }


            actors[0].CurrentPosition = new Point(10, 10);
            actors[1].CurrentPosition = new Point(10, 20);
            actors[2].CurrentPosition = new Point(20, 10);
            actors[3].CurrentPosition = new Point(20, 20);

            for (int i = 0; i < numActors; ++i)
            {
                m_arena.MoveActor(actors[i], actors[i].CurrentPosition);
                m_turnManager.QueueActor(actors[i]);

            }

            Globals.AttackBar= new AttackBar();
            Globals.AttackBar.Rectangle = new Rectangle(20, 300, 600, 30);
            Globals.AttackBar.InitializeCombatBar(3, 0.7f, 0.85f, 5f);
            //m_screenComponents.Components.Add(attackBar);
            m_uiElementsList.Add(Globals.AttackBar);


            Globals.PlayerChoiceBar = new PlayerChoiceBar();
            Globals.PlayerChoiceBar.Rectangle = new Rectangle(20, 400, 600, 30);

            m_uiElementsList.Add(Globals.PlayerChoiceBar);
            
            Globals.CombatEngine = new CombatEngine();

            Globals.MovementGrid = new MovementGrid(m_arena);
            m_uiElementsList.Add(Globals.MovementGrid);

            foreach (IUIElement uiElement in m_uiElementsList)
            {
                uiElement.LoadContent(m_content,ScreenManager.Game.GraphicsDevice);
                uiElement.Arena = m_arena;
                uiElement.ArenaScreen = this;
            }

            //Globals.MovementGrid.TurnManager = m_turnManager;

            //Globals.MovementGrid.CurrentActor = actors[0];
            actors[0].PlayerControlled = true;


        }

        public void SetMovementGridVisible(bool value)
        {
            Globals.MovementGrid.Visible = value;
            if (value)
            {
                Globals.MovementGrid.RegisterListeners();
            }
            else
            {
                Globals.MovementGrid.UnregisterListeners();
            }
        }

        public void SetPlayerChoiceBarVisible(bool value)
        {
            Globals.PlayerChoiceBar.Visible = value;
            {
                if (value)
                {
                    Globals.PlayerChoiceBar.RegisterListeners();
                }
                else
                {
                    Globals.PlayerChoiceBar.UnregisterListeners();
                }
            }
        }

        public void SetAttackBarVisible(bool value)
        {
            Globals.AttackBar.Visible = value;
            if (value)
            {
                Globals.AttackBar.RegisterListeners();
            }
            else
            {
                Globals.AttackBar.UnregisterListeners();
            }
        }

        public PlayerChoiceBar PlayerChoiceBar
        {
            get { return Globals.PlayerChoiceBar; }
        }


        #endregion
        #region Fields

        protected int m_updateCalls;
        protected int m_drawCalls;


        ContentManager m_content;
        SpriteFont m_gameFont;
        SpriteBatch m_spriteBatch;

        float pauseAlpha;

        private volatile GameTime gameTime;

        private SimpleArenaRenderer m_arenaRenderer;
        private Arena m_arena;

        protected CameraComponent m_camera;
        protected TurnManager m_turnManager;
        //protected GameComponentCollection Components = new GameComponentCollection();
        protected ScreenGameComponents m_screenComponents;
        protected List<IUIElement> m_uiElementsList = new List<IUIElement>();

        //MovementGrid m_movementGrid;
        //PlayerChoiceBar m_playerChoiceBar;
        //AttackBar m_attackBar;
        #endregion

    }
}













