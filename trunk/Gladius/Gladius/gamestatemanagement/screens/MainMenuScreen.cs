#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Gladius.gamestatemanagement.screens;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Gladius.gamestatemanagement.screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        private Texture2D m_background;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playArenaGameMenuEntry = new MenuEntry("Play Arena Game");
            MenuEntry playOverlandMenuEntry = new MenuEntry("Play Overland Game");

            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            playArenaGameMenuEntry.Colour = Color.Black;
            playOverlandMenuEntry.Colour = Color.Black;
            optionsMenuEntry.Colour = Color.Black;
            exitMenuEntry.Colour = Color.Black;


            // Hook up menu event handlers.
            playArenaGameMenuEntry.Selected += PlayArenaGameMenuEntrySelected;
            playOverlandMenuEntry.Selected += PlayOverlandGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playArenaGameMenuEntry);
            MenuEntries.Add(playOverlandMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        public override void LoadContent()
        {
            base.LoadContent();
            MenuBackground = ScreenManager.Game.Content.Load<Texture2D>("UI/backgrounds/ScrollBackground");
            TitlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 100);
            TitleColour = Color.Black;
        }

        //public override void UnloadContent()
        //{
        //    base.UnloadContent();
        //    ScreenManager.Game.Content.Unload()
        //}

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayArenaGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new ArenaScreen());
        }

        void PlayOverlandGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new OverlandScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
