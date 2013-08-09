using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Gladius.combat;

namespace Gladius.actions
{
    public abstract class BaseAction
    {
        public BaseAction(Arena arena,BaseActor baseActor)
        {
            m_arena = arena;
            m_baseActor = baseActor;
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {

        }

        protected Arena m_arena;
        protected BaseActor m_baseActor;

        public abstract String Name
        {
            get;
        }
    }


    public enum ActionTypes
    {
        Idle,
        Death,
        Stagger
    }
}
