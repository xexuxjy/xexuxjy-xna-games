using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework;

namespace Gladius.modes.arena.actions
{
    public class MoveAction : BaseAction
    {


        public MoveAction(BaseActor baseActor, Vector3 location, Arena arena)
        {

        }

        public override bool Complete
        {
            get
            {
                return true;
            }
        }

    }
}
