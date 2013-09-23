﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dhpoware;
using GameStateManagement;
using Gladius.actors;
using Gladius.modes.arena;

namespace Gladius.gamestatemanagement.screens
{
    public interface IUIElement
    {
        void LoadContent(ContentManager manager,GraphicsDevice device);
        void Update(GameTime gameTime);
        void DrawElement(GameTime gameTime, SpriteBatch spriteBatch);
        void DrawElement(GameTime gameTime, GraphicsDevice device,ICamera camera);

        void RegisterListeners();
        void UnregisterListeners();

        Rectangle Rectangle
        {
            get;
            set;
        }

        bool Visible
        {
            get;
            set;
        }

        ArenaScreen ArenaScreen
        {
            get;
            set;
        }
        Arena Arena
        {
            get;
            set;
        }
    }
}