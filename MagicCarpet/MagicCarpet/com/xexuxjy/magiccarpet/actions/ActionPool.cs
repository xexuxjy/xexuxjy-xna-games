using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionPool : IUpdateable
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionPool(GameObject gameObject)
        {
            m_owner = gameObject;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseAction GetAction(ActionState actionState)
        {
            return null;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void StartAction(BaseAction baseAction)
        {
            if (m_currentAction != null)
            {
                ActionComplete(m_currentAction);
                m_currentAction = null;
            }

            Debug.Assert(m_currentAction == null);
            Debug.Assert(baseAction != null);

            m_currentAction = baseAction;

            baseAction.ActionStarted += new BaseAction.ActionStartedHandler(ActionStarted);
            baseAction.ActionComplete += new BaseAction.ActionCompleteHandler(ActionComplete);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void StartAction(ActionState actionState)
        {
            if (m_currentAction!= null && actionState == m_currentAction.ActionState)
            {
                return;
            }


            if (m_currentAction != null)
            {
                ActionComplete(m_currentAction);
                m_currentAction = null;
            }


            // new action
            if (actionState == ActionState.Idle)
            {
                m_currentAction = new ActionIdle(m_owner);
            }

            m_owner.ActionStarted(m_currentAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
            StartAction(ActionState.Idle);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Update(GameTime gameTime)
        {
            // should always be some form of action
            Debug.Assert(m_currentAction != null);
            m_currentAction.Update(gameTime);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionState ActionState
        {
            get { return m_currentAction.ActionState; }
            set 
            {
                m_currentAction = null;
                // end old action, start new one?
            
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void ActionStarted(BaseAction baseAction)
        {
            m_owner.ActionStarted(baseAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void ActionComplete(BaseAction baseAction)
        {
            baseAction.ActionStarted -= new BaseAction.ActionStartedHandler(ActionStarted);
            baseAction.ActionComplete -= new BaseAction.ActionCompleteHandler(ActionComplete);
            m_owner.ActionComplete(baseAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private BaseAction m_currentAction;
        private GameObject m_owner;


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
}
