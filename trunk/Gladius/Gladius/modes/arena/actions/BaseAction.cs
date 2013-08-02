using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gladius.modes.arena.actions
{
    public abstract class BaseAction
    {
        public String ActioName
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void TurnStart()
        {

        }

        public void TurnEnd()
        {

        }

        public abstract bool Complete
        {
            get;
        }

    }
}
