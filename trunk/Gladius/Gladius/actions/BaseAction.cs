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


        protected Arena m_arena;
        protected BaseActor m_baseActor;

        public abstract String Name
        {
            get;
        }
    }
}
