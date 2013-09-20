using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using Microsoft.Xna.Framework.Content;
using Gladius.actors;
using GameStateManagement;
using Gladius.modes.arena;

namespace Gladius.gamestatemanagement.screens
{
    public class BaseUIElement : IUIElement
    {
        public bool Visible
        {
            get;
            set;
        }

        public Rectangle Rectangle
        {
            get;
            set;
        }

        public Arena Arena
        {
            get;
            set;
        }

        public ArenaScreen ArenaScreen
        {
            get;
            set;
        }


        public virtual void DrawElement(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public virtual void DrawElement(GameTime gameTime, GraphicsDevice graphicsDevice, ICamera camera)
        {

        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void LoadContent(ContentManager manager,GraphicsDevice device)
        {

        }

        public virtual void RegisterListeners()
        {
        }

        public virtual void UnregisterListeners()
        {
        }

        public int RegisterCount
        {
            get;
            set;
        }

    }
}
