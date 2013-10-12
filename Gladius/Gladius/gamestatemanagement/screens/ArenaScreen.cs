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
using System.Collections.Generic;
using Gladius.renderer;
using Gladius.actors;
using Gladius;
using Gladius.control;
using Gladius.combat;
using Gladius.util;
using Gladius.gamestatemanagement.screens;
using GameStateManagement;
using Gladius.modes.arena;
using System.Text;
//using com.xexuxjy.magiccarpet.control;
#endregion

namespace Gladius.gamestatemanagement.screens
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

            if (!m_battleOver)
            {
                m_screenComponents.Update(gameTime);

                foreach (IUIElement uiElement in m_uiElementsList)
                {
                    uiElement.Update(gameTime);
                }


                if (m_updateCalls % 100 == 0)
                {
                    StringBuilder textLeft = new StringBuilder("Combat Text " + m_updateCalls + " left");
                    StringBuilder textRight = new StringBuilder("Combat Text " + m_updateCalls + " right");

                    Vector3 start1 = m_turnManager.CurrentActor.CameraFocusPoint;
                    start1 -= m_turnManager.CurrentActor.World.Right * 5;

                    Vector3 start2 = m_turnManager.CurrentActor.CameraFocusPoint;
                    start2 += m_turnManager.CurrentActor.World.Right * 5;
                    start2 -= m_turnManager.CurrentActor.World.Forward * 5;


                    m_combatEngineUI.DrawFloatingText(start1, Color.White, textLeft, 4);
                    m_combatEngineUI.DrawFloatingText(start2, Color.White, textRight, 4);
                }
            }
            else
            {
                String text = m_battleWon ? "Victory!" : "Defeat!";


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

            if (m_movementGrid.Visible)
            {
                m_movementGrid.DrawElement(gameTime, ScreenManager.Game.GraphicsDevice, Globals.Camera);
            }

            m_screenComponents.Draw(gameTime);


            // bit yucky... ui elements have one form of draw or another. not both
            foreach (IUIElement uiElement in m_uiElementsList)
            {
                if (uiElement.Visible)
                {
                    if (uiElement is MovementGrid)
                    {
                        continue;
                    }
                    uiElement.DrawElement(gameTime, ScreenManager.Game.GraphicsDevice, Globals.Camera);
                }
            }


            m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            foreach (IUIElement uiElement in m_uiElementsList)
            {
                if (uiElement.Visible)
                {
                    if (uiElement is MovementGrid)
                    {
                        continue;
                    }
                    uiElement.DrawElement(gameTime, ScreenManager.GraphicsDevice, m_spriteBatch);
                }
            }

            m_spriteBatch.DrawString(m_debugFont, Globals.CombatEngine.LastCombatResult, new Vector2(600, 1), Color.Yellow);

            if (m_battleOver)
            {
                m_spriteBatch.DrawString(m_debugFont, Globals.CombatEngine.LastCombatResult, new Vector2(600, 1), Color.Yellow);
                String text = m_battleWon ? "Victory!" : "Defeat!";
                
                Vector2 midScreen = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width,ScreenManager.GraphicsDevice.Viewport.Height);
                midScreen /= 2f;
                Vector2 centeredText = Globals.CenterText(m_battleOverFont, text, midScreen);
                m_spriteBatch.DrawString(m_battleOverFont, text, centeredText, Color.White);    


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
            m_gameFont = m_content.Load<SpriteFont>("GameFont");
            m_gameFont.Spacing = -4.0f;

            m_debugFont = m_content.Load<SpriteFont>("UI/fonts/DebugFont8");
            m_battleOverFont = m_content.Load<SpriteFont>("UI/fonts/BattleOverFont");


            Globals.Camera.Position = new Vector3(0, 5, -10);

            m_arena = new Arena(32, 32);

            m_arenaRenderer = new SimpleArenaRenderer(m_arena, this);
            m_arenaRenderer.LoadContent();
            m_screenComponents.Components.Add(m_arenaRenderer);
            //m_arenaRenderer.LoadContent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice);
            m_turnManager = new TurnManager(ScreenManager.Game,this);
            m_turnManager.ArenaScreen = this;
            m_screenComponents.Components.Add(m_turnManager);

            Globals.SoundManager = new SoundManager();
            Globals.SoundManager.LoadContent(m_content);

            //String modelName = "Models/ThirdParty/monster-animated-character-XNA";
            String playerTeamModelName = "Models/ThirdParty/01_warrior";
            String enemyTeamModelName = "Models/ThirdParty/02_warrior";
            //String playerTeamModelName = enemyTeamModelName;
            List<BaseActor> actors = new List<BaseActor>();
            int numActors = 4;

            for (int i = 0; i < numActors; ++i)
            {
                BaseActor ba1 = ActorGenerator.GenerateActor(1, ActorClass.Barbarian, this);

                if (i < 2)
                {
                    ba1.ModelName = playerTeamModelName;
                    ba1.Team = Globals.PlayerTeam;
                    ba1.DebugName = "Player" + i;
                    ba1.PlayerControlled = true;
                }
                else
                {
                    ba1.DebugName = "Monster" + i;
                    ba1.ModelName = enemyTeamModelName;
                    ba1.Team = "EnemyTeam";
                }
                ba1.Arena = m_arena;
                ba1.LoadContent();
                actors.Add(ba1);
                m_screenComponents.Components.Add(ba1);
                ba1.SetupSkills(Globals.AttackSkillDictionary);
            }


            actors[0].CurrentPosition = new Point(10, 10);
            actors[1].CurrentPosition = new Point(11, 10);
            actors[2].CurrentPosition = new Point(20, 10);
            actors[3].CurrentPosition = new Point(20, 20);

            for (int i = 0; i < numActors; ++i)
            {
                m_arena.MoveActor(actors[i], actors[i].CurrentPosition);
                m_turnManager.QueueActor(actors[i]);

            }

            Globals.AttackBar = new AttackBar();
            Globals.AttackBar.Rectangle = new Rectangle(20, 300, 600, 30);
            Globals.AttackBar.InitializeCombatBar(3, 0.7f, 0.85f, 5f);
            //m_screenComponents.Components.Add(attackBar);
            m_uiElementsList.Add(Globals.AttackBar);


            Globals.PlayerChoiceBar = new PlayerChoiceBar();
            Globals.PlayerChoiceBar.Rectangle = new Rectangle(20, 400, 600, 30);
            Globals.PlayerChoiceBar.TurnManager = m_turnManager;

            m_uiElementsList.Add(Globals.PlayerChoiceBar);

            Globals.CombatEngine = new CombatEngine();

            m_movementGrid = new MovementGrid(m_arena);
            m_uiElementsList.Add(m_movementGrid);

            m_combatEngineUI = new CombatEngineUI();
            m_combatEngineUI.Visible = true;
            m_uiElementsList.Add(m_combatEngineUI);

            foreach (IUIElement uiElement in m_uiElementsList)
            {
                uiElement.LoadContent(m_content, ScreenManager.Game.GraphicsDevice);
                //uiElement.Arena = m_arena;
                uiElement.ArenaScreen = this;
            }

            // these here so that none of the key listeners are setup by default.
            SetMovementGridVisible(false);
            SetPlayerChoiceBarVisible(false);
            SetAttackBarVisible(false);

            //Globals.MovementGrid.TurnManager = m_turnManager;

            //Globals.MovementGrid.CurrentActor = actors[0];
            Globals.Camera.LookAtOffset = Vector3.Zero;//new Vector3(0.0f, 2, -2.2f) *  m_party.ModelHeight;
            Globals.Camera.DesiredPositionOffset = new Vector3(0, 2f, 4.0f) * actors[0].ModelHeight;

            List<BaseActor> loadedActors = new List<BaseActor>();
            ActorGenerator.LoadActors("Content/CharacterData/CharacterData.xml", loadedActors, this);
            int ibreak = 0;

        }

        public void SetMovementGridVisible(bool value)
        {
            MovementGrid.Visible = value;
            if (value)
            {
                MovementGrid.RegisterListeners();
            }
            else
            {
                MovementGrid.UnregisterListeners();
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

        public MovementGrid MovementGrid
        {
            get { return m_movementGrid; }
        }

        public void BattleOverVictory()
        {
            m_battleOver = true;
            m_battleWon = true;
            
        }

        public void BattleOverDefeat()
        {
            m_battleOver = true;
            m_battleWon = false;
        }


        #endregion
        #region Fields

        private bool m_battleOver;
        private bool m_battleWon;

        private SimpleArenaRenderer m_arenaRenderer;
        private Arena m_arena;

        private CombatEngineUI m_combatEngineUI;
        private SpriteFont m_debugFont;
        private SpriteFont m_battleOverFont;

        protected TurnManager m_turnManager;
        //protected GameComponentCollection Components = new GameComponentCollection();

        MovementGrid m_movementGrid;
        //PlayerChoiceBar m_playerChoiceBar;
        //AttackBar m_attackBar;
        #endregion

    }
}













