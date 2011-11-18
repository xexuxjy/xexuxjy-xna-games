using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;
using GameStateManagement;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Monster : GameObject
    {
        public Monster()
            : base(GameObjectType.monster)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Monster(Vector3 startPosition)
            : base(startPosition,GameObjectType.monster)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /************************************************************************/
        /* 
         * if we're not doing anything, then 
         *  look to see if there is anything nearby that poses a threat
         *  
         * if there is then start attacking it
         * 
         * if there isn't then find somewhere else to go
         
         */
        /************************************************************************/

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ActionState == ActionState.None)
            {
                QueueAction(new ActionIdle(this));
            }
            else if (ActionState == ActionState.Moving)
            {
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public override void ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Dead):
                    {
                        // drop current load as series of mana balls
                        // then remove ourselves from the game.
                        Cleanup();
                        break;
                    }
                case (ActionState.Searching):
                    {
                        QueueAction(new ActionTravel(Owner, null, action.TargetLocation, s_monsterSpeed));                           
                        break;
                    }
                case (ActionState.Idle):
                    {
                        QueueAction(new ActionFindManaball(this, null, s_searchRadius));
                        break;
                    }
                case (ActionState.Travelling):
                    {
                        Owner.TargetSpeed = 0f;

                        if (action.Target.GameObjectType == GameObjectType.castle)
                        {
                            QueueAction(new ActionUnload(this, action.Target));
                        }
                        else if (action.Target.GameObjectType == GameObjectType.manaball)
                        {
                            QueueAction(new ActionLoad(this, action.Target));
                        }
                        break;
                    }
                case(ActionState.Dieing):
                    {
                        // when we've finished dieing then we want to spawn a manaball here.
                        Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.manaball, Position);
                        break;
                    }

                default:
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private GameObject m_currentTarget;
        private const float s_searchRadius = 50f;
        private const float s_monsterSpeed = 5f;
    }
}
