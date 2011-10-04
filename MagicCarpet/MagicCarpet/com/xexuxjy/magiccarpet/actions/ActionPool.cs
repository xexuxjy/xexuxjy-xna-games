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

        public void StartAction(BaseAction baseAction)
        {
            Debug.Assert(!m_startingAction);
            if (m_currentAction != null && baseAction.ActionState == m_currentAction.ActionState)
            {
                return;
            }
            
            m_startingAction = true;

            if (m_currentAction != null)
            {
                CompleteAction(m_currentAction);
                m_currentAction = null;
            }

            Debug.Assert(m_currentAction == null);
            Debug.Assert(baseAction != null);

            m_currentAction = baseAction;

            m_owner.ActionStarted(baseAction);
            m_startingAction = false;

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void StartAction(ActionState actionState)
        {
            // new action
            BaseAction baseAction = null;
            if (actionState == ActionState.Idle)
            {
                baseAction = new ActionIdle(m_owner);
            }


            QueueAction(baseAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CompleteAction(BaseAction baseAction)
        {
            m_owner.ActionComplete(baseAction);
            m_currentAction = null;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearAction()
        {
            CompleteAction(m_currentAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Update(GameTime gameTime)
        {
            // should always be some form of action
            //Debug.Assert(m_currentAction != null);
            if (m_currentAction != null)
            {
                m_currentAction.Update(gameTime);
            }
            else
            {
                // if we have another one, then go for it.
                if (m_nextAction != null)
                {
                    StartAction(m_nextAction);
                    m_nextAction = null;
                }
            }


        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionState ActionState
        {
            get 
            {
                ActionState current = m_currentAction != null ? m_currentAction.ActionState : ActionState.None;
                ActionState next = m_nextAction != null ? m_nextAction.ActionState : ActionState.None;
                return current != ActionState.None ? current : next;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void QueueAction(BaseAction baseAction)
        {
            Debug.Assert(m_nextAction == null);
            m_nextAction = baseAction;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private BaseAction m_currentAction;
        private BaseAction m_nextAction;

        private GameObject m_owner;
        private bool m_startingAction;
        private bool m_completingAction;


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
