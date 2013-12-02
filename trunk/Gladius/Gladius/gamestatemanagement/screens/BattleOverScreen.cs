using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.gamestatemanagement.screenmanager;
using Microsoft.Xna.Framework;

namespace Gladius.gamestatemanagement.screens
{
    public class BattleOverScreen : GameScreen
    {
        public BattleOverScreen(bool win)
        {
            Win = win;
        }

        public bool Win
        {
            get;
            set;
        }

        public override void LoadContent()
        {
        }

        public override void Draw(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

        }
    }
}
