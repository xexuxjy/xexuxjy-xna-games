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

        public BaseAction CurrentAction
        {
            get
            {
                return m_actionQueue.Count > 0 ? m_actionQueue.Peek() : null;
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void StartAction(BaseAction baseAction)
        {
            Debug.Assert(baseAction != null);
            Debug.Assert(!baseAction.Started);

            baseAction.Started = true;
            m_owner.ActionStarted(baseAction);
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ActionPool[{0}][{1}] StartAction [{2}].", m_owner.Id, m_owner.GameObjectType, baseAction));
#endif

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CompleteAction(BaseAction baseAction)
        {
            Debug.Assert(baseAction.Complete);
            
            m_owner.ActionComplete(baseAction);
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ActionPool[{0}][{1}] CompleteAction [{2}].", m_owner.Id, m_owner.GameObjectType, baseAction));
#endif
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearCurrentAction()
        {
            CompleteAction(CurrentAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearAllActions()
        {
            m_actionQueue.Clear();

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ActionPool[{0}][{1}] ClearAllAction [{2}].", m_owner.Id, m_owner.GameObjectType));
#endif

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Update(GameTime gameTime)
        {
            // should always be some form of action
            //Debug.Assert(m_currentAction != null);
            if (CurrentAction != null)
            {
                if (!CurrentAction.Started)
                {
                    StartAction(CurrentAction);
                }

                CurrentAction.Update(gameTime);

                if (CurrentAction.Complete)
                {
                    BaseAction completedAction = m_actionQueue.Dequeue();
                    CompleteAction(completedAction);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionState ActionState
        {
            get 
            {
                ActionState current = CurrentAction != null ? CurrentAction.ActionState : ActionState.None;
                //ActionState next = m_nextAction != null ? m_nextAction.ActionState : ActionState.None;
                //return current != ActionState.None ? current : next;
                return current;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void QueueAction(BaseAction baseAction)
        {
            if (!baseAction.Initialized)
            {
                baseAction.Initialize();
            }
            m_actionQueue.Enqueue(baseAction);

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ActionPool[{0}][{1}] QueueAction [{2}].", m_owner.Id, m_owner.GameObjectType, baseAction.ActionState));
#endif

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private Queue<BaseAction> m_actionQueue = new Queue<BaseAction>();

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
