using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.actions
{
    public class BaseAction : IUpdateable
    {
        public BaseAction(GameObject owner, GameObject target,ActionState actionState)
        {
            m_actionState = actionState;
            m_actionComponent = owner.ActionComponent;
            m_owner = owner;
            m_target = target;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Initialize()
        {
            m_initialized = true; 
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Start()
        {
            m_started = true;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Update(GameTime gameTime)
        {
            if (Started)
            {
                if (!Complete)
                {
                    float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    m_currentTime += elapsedSeconds;
                }
                if (Complete)
                {
                    ActionComplete();
                }
            }

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // action completeness states shouldn't automatically add new states to the pool, this
        // will be done in actionpool update.
        public void ActionComplete()
        {
            InternalComplete();
            Globals.ActionPool.ReleaseAction(this);
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

        public virtual bool Complete
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

        public Vector3? TargetLocation
        {
            get { return m_targetLocation; }
            set { m_targetLocation = value; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Started
        {
            get { return m_started; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Initialized
        {
            get { return m_initialized; }
            set { m_initialized = value; }
        }


        private ActionComponent m_actionComponent;
        private GameObject m_owner; 
        private GameObject m_target;
        private IndexedVector3? m_targetLocation;
        private ActionState m_actionState;
        private float m_duration;
        private float m_currentTime;
        private bool m_started;
        private bool m_initialized;


        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get { throw new NotImplementedException(); }
        }


        public static bool IsPassive(ActionState state)
        {
            return state >= ActionState.None && state <= ActionState.Turning;
        }

        public static bool IsAttacking(ActionState state)
        {
            return state >= ActionState.AttackingMelee && state <= ActionState.AttackingRange;
        }

        public static bool IsDieing(ActionState state)
        {
            return state == ActionState.Dieing;
        }

        public static bool IsCasting(ActionState state)
        {
            return state == ActionState.Casting;
        }

        public static bool IsLoading(ActionState state)
        {
            return state == ActionState.Loading || state == ActionState.Unloading;
        }


        public event EventHandler<EventArgs> UpdateOrderChanged;
    }





    public enum ActionState
    {
        None = 0,
        Idle = 1,
        Searching = 2,
        Travelling = 3,
        Turning = 4,
        Fleeing = 5,
        Loading = 6,
        Unloading = 7,
        Casting = 8,
        AttackingMelee= 9,
        AttackingRange=10,
        Dieing = 11,

    }

}
