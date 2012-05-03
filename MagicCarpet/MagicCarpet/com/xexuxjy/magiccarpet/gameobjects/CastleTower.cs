using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;
using com.xexuxjy.magiccarpet.spells;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.util;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.combat;
using com.xexuxjy.magiccarpet.interfaces;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class CastleTower : GameObject, ICastlePart
    {
        public CastleTower(Castle castle,Vector3 startPosition)
            : base(startPosition, GameObjectType.castle)
        
        {
            m_castle = castle;
            StickToGround = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void InitializeModel()
        {
            m_modelHelperData = Globals.MCContentManager.GetModelHelperData("CastleTower");

            // scale the base of the tower to one unit?
            Vector3 scale = Castle.s_castleTowerSize / (m_modelHelperData.m_boundingBox.Max - m_modelHelperData.m_boundingBox.Min);
            m_scaleTransform = Matrix.CreateScale(scale);

            m_spellCastPosition = Position;
            // offset this a bit so we fire from the top of the tower.

            m_spellCastPosition.Y += m_modelHelperData.m_boundingBox.Max.Y * scale.Y;



        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            base.Initialize();
            float height = (BoundingBox.Max.Y - BoundingBox.Min.Y);

            Vector3 newPos = Position;

            newPos.Y += height / 2.0f;

            Position = newPos;

            Vector3 scale = new Vector3(0.5f);
            Vector3 defaultFlagDimensions = Globals.FlagManager.DefaultFlagDimensions;

            Vector3 adjustedPosition = defaultFlagDimensions * scale;
            
            Vector3 flagPosition = SpellCastPosition + new Vector3(0, adjustedPosition.Y*3f, 0);
            Globals.FlagManager.AddFlagForObject(this,flagPosition, scale);

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	


        public override CollisionFilterGroups GetCollisionFlags()
        {
            return  (CollisionFilterGroups)GameObjectType.castle;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionFilterGroups GetCollisionMask()
        {
            return (CollisionFilterGroups)(GameObjectType.spell);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ActionState == ActionState.None)
            {
                QueueAction(Globals.ActionPool.GetActionIdle(this));
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	


        public override GameObject Owner
        {
            get
            {
                return m_castle.Owner;
            }
            set { }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Searching):
                    {
                        ActionFind actionFind = action as ActionFind;
                        // If we found something nearby
                        if (actionFind.Target != null)
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackRange(this, actionFind.Target, Globals.s_castleTurretSearchRadius, Globals.s_castleTurretAttackDamage, SpellType.Fireball));
                        }
                        break;
                    }
                case (ActionState.Idle):
                    {
                        // if we've finished an idle then look around for something
                        // to do.

                        BaseAction newAction = Globals.ActionPool.GetActionFind(FindData.GetActionFindEnemy(Owner, Globals.s_castleTurretSearchRadius));;
                        QueueAction(newAction);
                        break;
                    }
                case (ActionState.AttackingRange):
                    {
                        // if we've finished attacking at range. then we need to see if 
                        // our target is dead or if we should do something else.

                        if (action.Target.Alive && GameUtil.InRange(this, action.Target, Globals.s_monsterRangedDamage))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackRange(Owner, action.Target, Globals.s_monsterRangedRange, Globals.s_monsterRangedDamage, SpellType.Fireball));
                        }
                        break;
                    }

                case (ActionState.Dieing):
                    {
                        // when we've finished dieing then we want to spawn a manaball here.
                        Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.manaball, Position);
                        Cleanup();
                        break;
                    }

                default:
                    {
                        QueueAction(Globals.ActionPool.GetActionIdle(Owner,Globals.s_castleTurretSearchFrequency));
                        break;
                    }
            }
        }

        public override void Damaged(DamageData damageData)
        {
            // let the main castle body handle it for now
            m_castle.Damaged(damageData);
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override Texture2D GetTexture()
        {
            return Globals.MCContentManager.GetTexture("CastleTower");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override Vector3 SpellCastPosition
        {
            get
            {
                return m_spellCastPosition;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Cleanup()
        {
            Globals.FlagManager.RemoveFlagForObject(this);
            base.Cleanup();
        }

        private Vector3 m_spellCastPosition;
        private Castle m_castle;
    }
            
}

