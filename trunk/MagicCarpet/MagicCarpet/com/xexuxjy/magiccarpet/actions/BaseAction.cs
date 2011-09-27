using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.actions
{
    public class BaseAction
    {
        public BaseAction(GameObject owner, GameObject target,ActionState actionState)
        {
            m_actionState = actionState;
            m_owner = owner;
            m_target = target;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Initialize()
        {
            ActionStarted(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Update(GameTime gameTime)
        {
            if (!Complete)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_currentTime += elapsedSeconds;
                if (Complete)
                {
                    ActionComplete(this);
                }
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Cleanup()
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Complete
        {
            get { return m_currentTime >= m_duration; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // How far through the action we are? 0->1
        public float Completion
        {
            get
            {
                return m_currentTime / m_duration;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public float Duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public ActionState ActionState
        {
            get { return m_actionState; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject Owner
        {
            get { return m_owner; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject Target
        {
            get { return m_target; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public delegate void ActionStartedHandler(BaseAction baseAction);
        public event ActionStartedHandler ActionStarted;

        public delegate void ActionCompleteHandler(BaseAction action);
        public event ActionCompleteHandler ActionComplete;

        private GameObject m_owner;
        private GameObject m_target;
        private ActionState m_actionState;
        private float m_duration;
        private float m_currentTime;
    }



    public enum ActionState
    {
        Idle,
        Moving,
        Loading,
        Unloading,
        Casting,
        Dieing,
        Dead,
        Attacking,
        Searching

    }

}
