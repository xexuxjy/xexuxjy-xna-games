using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;
using System.Diagnostics;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Balloon : GameObject
    {
        public Balloon(Game game)
            : base(game,GameObjectType.BALLOON)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Balloon(Vector3 startPosition, Game game)
            : base(startPosition,game,GameObjectType.BALLOON)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            base.Initialize();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            if (m_currentActionState == ActionState.Idle)
            {
                if (Full)
                {
                    StartAction(new ActionFindCastle(this,null,s_castleSearchRadius));
                }
                else
                {
                    StartAction(new ActionFindManaball(this,null,s_balloonSearchRadius));
                }
            }
            else
            if (m_currentActionState == ActionState.Moving)
            {
                if (m_currentTarget != null)
                {
                    // if we're moving then check to see if the object is still valid.
                    if (!TargetValid())
                    {
                        // it's not, so set out state back to idle.
                        m_currentActionState = ActionState.Idle;
                    }
                }


            }
            base.Update(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool TargetValid()
        {
            if (m_currentTarget != null)
            {
                return m_currentTarget.Active();
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Load(ManaBall manaBall)
        {
            CurrentAction = new ActionLoad(this,manaBall);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Unload(Castle castle)
        {
            CurrentAction = new ActionUnload(this,castle);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void value_ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Loading):
                    {
                        ManaBall manaball = action.Target as ManaBall;
                        Debug.Assert(manaball != null);
                        m_currentLoad += manaball.ManaValue;
                        // loaded now so remove object.
                        manaball.Cleanup();

                        break;
                    }

                case (ActionState.Unloading):
                    {
                        // change this so unload is gradual
                        Castle castle = action.Target as Castle;
                        Debug.Assert(castle != null);
                        castle.StoredMana += CurrentLoad;
                        CurrentLoad = 0f;

                        break;
                    }
                case (ActionState.Dead):
                    {
                        // drop current load as series of mana balls
                        // then remove ourselves from the game.
                        Cleanup();
                        break;
                    }
                case (ActionState.Searching):
                    {
                        // if the search has completed then grab whatever it found and aim for that.
                        m_currentTarget = action.Target;
                        break;
                    }
                default:
                    break;
            }
            // action has completed so clear it and let another be assigned.
            m_currentAction = null;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float CurrentLoad
        {
            get { return m_currentLoad; }
            set { m_currentLoad = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Full
        {
            get
            {
                return MathUtil.CompareFloat(m_currentLoad, m_maxLoad);
            }
        }

        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void value_ActionStarted(BaseAction action)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public override String DebugText
        {
            get
            {
                return String.Format("Id [{0}] Pos[{1}] Action[{2}] Mana[{3}].", Id, Position, ActionState, CurrentLoad);
            }

        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private float m_currentLoad;
        private float m_maxLoad;

        private GameObject m_currentTarget;
        private const float s_balloonSearchRadius = 50f;
        private const float s_castleSearchRadius = 250f;

    }

}
