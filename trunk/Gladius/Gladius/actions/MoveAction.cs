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
            // check and see if we can move
            int nextIndex = m_currentIndex+1;
            if(m_arena.CanMoveActor(m_baseActor,m_wayPoints[nextIndex]))
            {
                m_arena.MoveActor(m_baseActor,m_wayPoints[nextIndex]);
            }
        }


        public override string Name
        {
            get { return "Move"; }
        }

        int m_currentIndex = 0;
        List<Point> m_wayPoints = new List<Point>();
    }
}
