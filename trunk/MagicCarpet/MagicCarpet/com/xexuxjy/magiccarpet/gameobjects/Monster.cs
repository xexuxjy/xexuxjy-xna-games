using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Monster : GameObject
    {
        public Monster(Game game)
            : base(game,GameObjectType.monster)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Monster(Vector3 startPosition, Game game)
            : base(startPosition,game,GameObjectType.monster)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

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

                default:
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private GameObject m_currentTarget;
        private const float s_searchRadius = 50f;
        private const float s_monsterSpeed = 5f;
    }
}
