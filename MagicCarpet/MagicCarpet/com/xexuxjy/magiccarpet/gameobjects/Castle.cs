using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.terrain;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.actions;
using com.xexuxjy.magiccarpet.spells;
using GameStateManagement;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.combat;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Castle : GameObject
    {
        public Castle()
            : base(GameObjectType.castle)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Castle(Vector3 position)
            : base(position, GameObjectType.castle)
        {
            m_initialHeight = position.Y;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            base.Initialize();
            //GrowToSize(0);

            // test value for now.
            m_storedMana = 500;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void SetStartAttributes()
        {
            m_attributes[(int)GameObjectAttributeType.Health] = new GameObjectAttribute(GameObjectAttributeType.Health, 100);
            // silly amount of mana.
            m_attributes[(int)GameObjectAttributeType.Mana] = new GameObjectAttribute(GameObjectAttributeType.Mana, float.MaxValue);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void BuildCollisionObject()
        {
            Vector3 halfExtents = new Vector3(CastleSizes[Level]/2, GetStartOffsetHeight(), CastleSizes[Level]/2);
           
            CollisionShape cs = new BoxShape(halfExtents);
            float mass = 0f;
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(mass, Matrix.CreateTranslation(Position), cs, m_motionState, true, this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override float GetStartOffsetHeight()
        {
            return 2.5f;
        }
        

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public override void Update(GameTime gameTime)
        {
            foreach (CastleTower tower in m_towers)
            {
                tower.Update(gameTime);
            }
            base.Update(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            foreach (CastleTower tower in m_towers)
            {
                tower.Draw(gameTime);
            }
            base.Draw(gameTime);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Cleanup()
        {
            if (m_towers != null)
            {
                m_towers.Clear();
            }

            m_towers = null;
            base.Cleanup();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void ActionComplete(BaseAction action)
        {

            switch (action.ActionState)
            {
                case (ActionState.Dieing):
                    {

                        // drop mana balls randomly withing our area?
                        int maxBalls = 10;
                        int numBalls = 0;
                        int manaValue = (int)m_storedMana / maxBalls;
                        if(manaValue == 0)
                        {
                            manaValue = 1;
                        }
                        for (int i = 0; i < maxBalls; ++i)
                        {
                            int radius = CastleSizes[Level];
                            Vector3 manaBallPosition = Globals.Terrain.GetRandomWorldPositionXZWithRange(Position, radius);
                            ManaBall manaBall = (ManaBall)Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.manaball, manaBallPosition);
                            manaBall.ManaValue = manaValue;
                        }

                        Cleanup();
                        break;
                    }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CanCreate()
        {
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CanGrow(int level)
        {
            return CanPlaceSize(Position,level);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        public static bool CanPlaceSize(Vector3 position,int size)
        {
            // go to the terrain and make sure that there is nothing nearby that will interfere.
            int width = CastleSizes[size];

            Vector3 startPos = position;
            startPos -= new Vector3(-width / 2, 0, -width / 2);


            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    Vector3 point = startPos + new Vector3(i,0,j);
                    TerrainSquare square = Globals.Terrain.GetTerrainSquareAtPointWorld(ref point);
                    if (square.Occupier != null)
                    {
                        // something already there.
                        return false;
                    }
                    if (square.Type == TerrainType.water || square.Type == TerrainType.immovable)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void GrowToSize(int size)
        {
            Debug.Assert(CanPlaceSize(Position,size));

            Level = size;

            int width = CastleSizes[size];

            Vector3 startPos = Position;
            Vector3 offset = new Vector3(width / 2, 0, width / 2);
            startPos -= offset;


            for (int j = 0; j < width; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    Vector3 point = startPos + new Vector3(i, 0, j);
                    point.Y = m_initialHeight;
                    //point.Y = 0f;
                    Globals.Terrain.SetHeightAtPointWorld(ref point);
                    Globals.Terrain.SetTerrainTypeAndOccupier(point, TerrainType.castle,this);
                }
            }

            Globals.Terrain.UpdateHeightMap();
            m_scaleTransform = Matrix.CreateScale(width/2, GetStartOffsetHeight(), width/2);

            bool enableTurrets = true;
            if (enableTurrets)
            {
                Vector3 turretPos0 = new Vector3(Position.X - offset.X, Position.Y, Position.Z - offset.Z);
                Vector3 turretPos1 = new Vector3(Position.X + offset.X, Position.Y, Position.Z - offset.Z);
                Vector3 turretPos2 = new Vector3(Position.X - offset.X, Position.Y, Position.Z + offset.Z);
                Vector3 turretPos3 = new Vector3(Position.X + offset.X, Position.Y, Position.Z + offset.Z);

                // clear or move turrets?
                if (m_towers.Count == 0)
                {
                    m_towers.Add(new CastleTower(this, turretPos0));
                    m_towers.Add(new CastleTower(this, turretPos1));
                    m_towers.Add(new CastleTower(this, turretPos2));
                    m_towers.Add(new CastleTower(this, turretPos3));

                    foreach (CastleTower tower in m_towers)
                    {
                        tower.Initialize();
                    }
                }
                else
                {
                    m_towers[0].Position = turretPos0;
                    m_towers[1].Position = turretPos1;
                    m_towers[2].Position = turretPos2;
                    m_towers[3].Position = turretPos3;

                }
            }
            CreateBalloon();
        }
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public int Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float StoredMana
        {
            get { return m_storedMana; }
            set { m_storedMana = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override String DebugText
        {
            get
            {
                return String.Format("Castle Id [{0}] Pos[{1}] Level[{2}] Mana[{3}].", Id, Position, Level, StoredMana);
            }

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        // when a castle is created it should also create a balloon? - maybe but needs to be deferred past
        // an update loop.

        public void CreateBalloon()
        {
            GameObject gameObject = Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.balloon, Position);
            gameObject.Owner = Owner;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // when we're damaged we start losing mana as manaballs. these manaballs should still be 
        // owner coloured?
        public override void Damaged(DamageData damageData)
        {
            base.Damaged(damageData);

            float damage;

            float currentHealthPercentage = GetAttributePercentage(GameObjectAttributeType.Health);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        // Different castle sizes.
        public static int[] CastleSizes = new int[]{4,6,10};



        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private int m_level;
        private float m_storedMana;
        private float m_initialHeight;

        private List<CastleTower> m_towers = new List<CastleTower>();
    }
}
