using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;

namespace Gladius.actions
{
    public abstract class BaseAction
    {
        public BaseAction(Arena arena,BaseActor baseActor)
        {
            m_arena = arena;
            m_baseActor = baseActor;
        }


        private Arena m_arena;
        private BaseActor m_baseActor;

        public abstract String Name
        {
            get;
        }
    }
}
