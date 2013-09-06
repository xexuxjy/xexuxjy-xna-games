using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gladius.gamestatemanagement.screens
{
    public interface IUIElement
    {
        void LoadContent(ContentManager manager);
        void Update(GameTime gameTime);
        void DrawElement(GameTime gameTime, SpriteBatch spriteBatch);
        void RegisterListeners();
        void UnregisterListeners();

        Rectangle Rectangle
        {
            get;
            set;
        }
    }
}
