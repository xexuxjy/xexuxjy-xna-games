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
    public class CastleWall : GameObject, ICastlePart
    {
        public CastleWall(Castle castle, Vector3 startPosition,Matrix rotation)
            : base(startPosition, GameObjectType.castle)
        {
            m_castle = castle;
            m_rotation = rotation;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void InitializeModel()
        {
            m_modelHelperData = Globals.MCContentManager.GetModelHelperData("CastleWall");

            Vector3 scale = Globals.s_castleWallSize / (m_modelHelperData.m_boundingBox.Max - m_modelHelperData.m_boundingBox.Min);
            m_scaleTransform = Matrix.CreateScale(scale);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            base.Initialize();
            float height = ObjectDimensions.Y;
            m_modelHeightOffset = height / 2.0f;
            m_rotation.Translation = SpawnPosition;
            WorldTransform = m_rotation;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	
        public override Vector3 ObjectDimensions
        {
            get { return Globals.s_castleWallSize; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	


        public override CollisionFilterGroups GetCollisionFlags()
        {
            return (CollisionFilterGroups)GameObjectType.castle;
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
                case (ActionState.Spawning):
                    {
                        // force an update on spawn positions and the like?
                        PositionBase = Position;
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
                        QueueAction(Globals.ActionPool.GetActionIdle(this, Globals.s_castleTurretSearchFrequency));
                        break;
                    }
            }
        }

        public override void Damaged(DamageData damageData)
        {
            // let the main castle body handle it for now
            m_castle.Damaged(damageData);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override String GetBaseTextureName()
        {
            return "CastleTower";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private Castle m_castle;
        private Matrix m_rotation;
    }

}


