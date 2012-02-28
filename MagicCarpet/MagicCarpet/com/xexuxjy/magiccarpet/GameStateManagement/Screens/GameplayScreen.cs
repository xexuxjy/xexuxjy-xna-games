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
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.util;
using BulletXNADemos.Demos;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.manager;
using com.xexuxjy.magiccarpet.terrain;
using Dhpoware;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA.LinearMath;
using System.Collections.Generic;
using com.xexuxjy.magiccarpet.util.console;
using com.xexuxjy.magiccarpet.util.debug;
using com.xexuxjy.magiccarpet.gui;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            Globals.LoadConfig();

            if (m_content == null)
            {
                m_content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            m_debugDrawMode = DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits | DebugDrawModes.DBG_DrawAabb | DebugDrawModes.DBG_DrawWireframe;

            m_gameFont = m_content.Load<SpriteFont>("fonts/gamefont");


            //var profilerGameComponent = new ProfilerGameComponent(Globals.Game, "ProfilerFont"); 
            //ProfilingManager.Run = true; 
            //AddComponent(profilerGameComponent); 

            
            CameraComponent camera = new CameraComponent(Globals.Game);
            
            
            // stop camera going through terrain?
            camera.ClipToWorld = true;
            camera.Position = new Vector3(0, 5, 0);
            
            Globals.Camera = camera;

            Globals.GraphicsDevice = Globals.Game.GraphicsDevice;



            Globals.DebugDraw = new XNA_ShapeDrawer(Globals.Game);
            Globals.DebugDraw.SetDebugMode(m_debugDrawMode);
            if (Globals.DebugDraw != null)
            {
                Globals.DebugDraw.LoadContent();
            }

            Globals.CollisionManager = new CollisionManager(Globals.worldMinPos, Globals.worldMaxPos);
            AddComponent(Globals.CollisionManager);

            Globals.GameObjectManager = new GameObjectManager(this);
            AddComponent(Globals.GameObjectManager);

            Globals.SimpleConsole = new SimpleConsole(Globals.DebugDraw);
            Globals.SimpleConsole.Enabled = false;
            AddComponent(Globals.SimpleConsole);


            Globals.MCContentManager = new MCContentManager();
            Globals.MCContentManager.Initialize();

            Globals.DebugObjectManager = new DebugObjectManager(ScreenManager.Game, Globals.DebugDraw);
            Globals.DebugObjectManager.Enabled = true;
            AddComponent(Globals.DebugObjectManager);


            Globals.Terrain = (Terrain)Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.terrain,Vector3.Zero);


            Globals.ActionPool = new ActionPool();

            //Globals.Player = (Magician)Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.magician, new Vector3(0, 10, 0));
            //Globals.DebugObjectManager.DebugObject = Globals.Player;

            AddComponent(camera);


            AddComponent(new SkyDome(),true);


            m_keyboardController = new KeyboardController();
            m_mouseController = new MouseController();


            m_playerHud = new PlayerHud(this);
            m_playerHud.Initialise();



            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                foreach (GameComponent gameComponent in m_componentCollection)
                {
                    gameComponent.Update(gameTime);
                }
            }

            if (Globals.s_initialScript != null && !loadedInitialScript)
            {
                loadedInitialScript = true;
                Globals.SimpleConsole.LoadScript(Globals.s_initialScript);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            m_mouseController.HandleInput(input);
            m_keyboardController.HandleInput(input);
            Globals.Camera.HandleInput(input);

            foreach (GuiComponent guiComponent in m_guiComponents)
            {
                if (guiComponent.HasGuiControl)
                {
                    guiComponent.HandleInput(input);
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            //Globals.CollisionManager.Draw(gameTime);

            m_drawableList.Clear();
            foreach (GameComponent gameComponent in m_componentCollection)
            {
                IDrawable drawable = gameComponent as IDrawable;
                if (drawable != null)
                {
                    m_drawableList.Add(drawable);
                }
            }
            IList<GameObject> gameObjectManagerComponents = Globals.GameObjectManager.DebugObjectList;
            foreach (GameComponent gameComponent in gameObjectManagerComponents)
            {
                IDrawable drawable = gameComponent as IDrawable;
                if (drawable != null)
                {
                    m_drawableList.Add(drawable);
                }
            }

            m_drawableList.Sort(drawComparator);


            // reset the blendstats as spritebatch probably trashed them.
            Globals.Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (IDrawable drawable in m_drawableList)
            {
                drawable.Draw(gameTime);
            }


            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        static Comparison<IDrawable> drawComparator = new Comparison<IDrawable>(DrawableSortPredicate);
        private static int DrawableSortPredicate(IDrawable lhs, IDrawable rhs)
        {
            int result = lhs.DrawOrder - rhs.DrawOrder ;
            return result;
        }

        public void AddComponent(GameComponent gameComponent)
        {
            AddComponent(gameComponent, false);
        }

        public void AddComponent(GameComponent gameComponent,bool initialise)
        {
            m_componentCollection.Add(gameComponent);

            IDrawable drawable = gameComponent as IDrawable;
            if (drawable != null)
            {
                m_drawableList.Add(drawable);
            }
            GuiComponent guiComponent = gameComponent as GuiComponent;
            if (guiComponent != null)
            {
                m_guiComponents.Add(guiComponent);
            }

            if(initialise)
            {
                gameComponent.Initialize();
            }
        }

        public void RemoveComponent(GameComponent gameComponent)
        {
            m_componentCollection.Remove(gameComponent);
            IDrawable drawable = gameComponent as IDrawable;
            if (drawable != null)
            {
                m_drawableList.Remove(drawable);
            }
            GuiComponent guiComponent = gameComponent as GuiComponent;
            if (guiComponent != null)
            {
                m_guiComponents.Remove(guiComponent);
            }
        }



        #endregion
        #region Fields

        PlayerHud m_playerHud;
        ContentManager m_content;
        SpriteFont m_gameFont;

        DebugDrawModes m_debugDrawMode;

        GameComponentCollection m_componentCollection = new GameComponentCollection();
        List<IDrawable> m_drawableList = new List<IDrawable>();
        List<GuiComponent> m_guiComponents = new List<GuiComponent>();

        MouseController m_mouseController;
        KeyboardController m_keyboardController;   
 
        static bool loadedInitialScript = false;

        float pauseAlpha;



        #endregion

    }
}













