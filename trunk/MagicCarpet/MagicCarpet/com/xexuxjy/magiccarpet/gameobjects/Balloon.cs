using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;
using System.Diagnostics;
using BulletXNA;
using GameStateManagement;
using com.xexuxjy.magiccarpet.combat;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Balloon : GameObject
    {
        public Balloon()
            : base(GameObjectType.balloon)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Balloon(Vector3 startPosition)
            : base(startPosition,GameObjectType.balloon)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override float GetHoverHeight()
        {
            return Globals.s_balloonHoverHeight;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void InitializeModel()
        {
            m_modelHelperData = Globals.MCContentManager.GetModelHelperData("balloon");

            Vector3 scale = Globals.s_balloonSize/ (m_modelHelperData.m_boundingBox.Max - m_modelHelperData.m_boundingBox.Min);
            m_scaleTransform = Matrix.CreateScale(scale);

            StickToGround = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override Texture2D GetTexture()
        {
            return Globals.MCContentManager.GetTexture("Balloon");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ActionState == ActionState.None)
            {
                QueueAction(Globals.ActionPool.GetActionIdle(this));
            }
            else if (ActionState == ActionState.Travelling)
            {
                if (m_currentTarget != null)
                {
                    // if we're moving then check to see if the object is still valid.
                    if (!TargetValid())
                    {
                        // it's not, so set out state back to idle.
                       ClearAction();
                    }
                }
            }
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
            QueueAction(Globals.ActionPool.GetActionLoad(this, manaBall));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Unload(Castle castle)
        {
            QueueAction(Globals.ActionPool.GetActionUnload(this, castle));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Spawning):
                    {
                        // force an update on spawn positions and the like?
                        Position = Position;
                        break;
                    }

                case (ActionState.Loading):
                    {
                        ManaBall manaball = action.Target as ManaBall;
                        Debug.Assert(manaball != null);
                        m_currentLoad += manaball.ManaValue;
                        // loaded now so remove object.
                        manaball.Die();
                        if (Full)
                        {

                            QueueAction(Globals.ActionPool.GetActionFind(FindData.GetCastleFindData(this, Globals.s_balloonSearchRadiusCastle)));
                        }

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
                case (ActionState.Dieing):
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
                        if (m_currentTarget != null)
                        {
                            QueueAction(Globals.ActionPool.GetActionTravel(this, m_currentTarget, Vector3.Zero, Globals.s_balloonTravelSpeed));
                        }

                        break;
                    }
                case (ActionState.Idle):
                    {
                        FindData findData = FindData.GetManaballFindData(this, Globals.s_balloonSearchRadiusManaball);
                        findData.m_includeOwner = true;

                        QueueAction(Globals.ActionPool.GetActionFind(findData));
                        break;
                    }
                case(ActionState.Travelling):
                    {
                        TargetSpeed = 0f;

                        if (action.Target.GameObjectType == GameObjectType.castle)
                        {
                            QueueAction(Globals.ActionPool.GetActionUnload(this, action.Target));
                        }
                        else if (action.Target.GameObjectType == GameObjectType.manaball)
                        {
                            QueueAction(Globals.ActionPool.GetActionLoad(this, action.Target));
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Damaged(DamageData damageData)
        {
            base.Damaged(damageData);

            // if something attacks us then we need to hit back?

            float currentHealthPercentage = GetAttributePercentage(GameObjectAttributeType.Health);

            if (currentHealthPercentage <= Globals.s_balloonFleeHealthPercentage)
            {
                m_actionComponent.ClearAllActions();
                QueueAction(Globals.ActionPool.GetActionFlee(this, m_threatComponent.GetFleeDirection(), Globals.s_balloonFleeSpeed));
            }
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
                return MathUtil.CompareFloat(m_currentLoad, Globals.s_balloonMaxCapacity);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public override String DebugText
        {
            get
            {
                return String.Format("Balloon Id [{0}] Pos[{1}] Action[{2}] Mana[{3}].", Id, Position, ActionState, CurrentLoad);
            }

        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private float m_currentLoad;
        private GameObject m_currentTarget;


    }

}
