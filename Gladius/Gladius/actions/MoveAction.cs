using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gladius.actors;

namespace Gladius.actions
{
    public class MoveAction : BaseAction
    {
        public MoveAction(Arena arena, BaseActor baseActor)
            : base(arena, baseActor)
        {

        }

        public void SetupPath(Point location)
        {

        }

        public void Update(GameTime gameTime)
        {
        }


        public override string Name
        {
            get { return "Move"; }
        }


        List<Point> m_wayPoints = new List<Point>();
    }
}
