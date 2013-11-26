using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Gladius.actors;
using Gladius.modes.arena;
using Gladius.renderer;
using Gladius.util;

namespace Gladius.gamestatemanagement.screens
{
    public interface IUIElement
    {
        void LoadContent(ThreadSafeContentManager manager, GraphicsDevice device);
        void Update(GameTime gameTime);
        void DrawElement(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch);
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
        //Arena Arena
        //{
        //    get;
        //    set;
        //}

        OverlandScreen OverlandScreen
        {
            get;
            set;
        }

        GladiatorChoiceScreen GladiatorChoiceScreen
        {
            get;
            set;
        }
    
    }
}
