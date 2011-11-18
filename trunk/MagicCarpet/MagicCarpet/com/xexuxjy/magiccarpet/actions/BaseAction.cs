using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.actions
{
    public class BaseAction : IUpdateable
    {
        public BaseAction(GameObject owner, GameObject target,ActionState actionState)
        {
            m_actionState = actionState;
            m_actionPool = owner.ActionPool;
            m_owner = owner;
            m_target = target;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Initialize()
        {
         
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public virtual void Update(GameTime gameTime)
        {
            if (!Complete)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_currentTime += elapsedSeconds;
            }
            else
            {
                ActionComplete();
            }

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ActionComplete()
        {
            InternalComplete();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // let the sub class to something before we complete.
        protected virtual void InternalComplete()
        {

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
            set { m_target = value; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 TargetLocation
        {
            get { return m_targetLocation; }
            set { m_targetLocation = value; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Started
        {
            get { return m_started; }
            set { m_started = value; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private ActionPool m_actionPool;
        private GameObject m_owner; 
        private GameObject m_target;
        private Vector3 m_targetLocation;
        private ActionState m_actionState;
        private float m_duration;
        private float m_currentTime;
        private bool m_started;

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;
    }



    public enum ActionState
    {
        None,
        Idle,
        Moving,
        Loading,
        Unloading,
        Casting,
        Dieing,
        Dead,
        Attacking,
        Searching,
        Travelling

    }

}
