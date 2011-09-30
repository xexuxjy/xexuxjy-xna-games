using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.terrain;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.actions;
using com.xexuxjy.magiccarpet.spells;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Castle : GameObject
    {
        public Castle(Game game)
            : base(game, GameObjectType.castle)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Castle(Vector3 position, Game game)
            : base(position, game,GameObjectType.castle)
        {
            m_initialHeight = position.Y;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            // got to terrain and flatten a square of appropriate size;
            Vector3 halfExtents = new Vector3(CastleSizes[Level], 5, CastleSizes[Level]);
            halfExtents *= 0.5f;
            m_boundingBox = new BoundingBox(Position - halfExtents, Position + halfExtents);
            base.Initialize();

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            foreach (Turret turret in m_turrets)
            {
                turret.Update(gameTime);
            }
            base.Update(gameTime);
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Cleanup()
        {
            m_turrets.Clear();
            m_turrets = null;
            base.Cleanup();
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
            int width = CastleSizes[size];

            Vector3 startPos = Position;
            startPos -= new Vector3(-width / 2, 0, -width / 2);


            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    Vector3 point = startPos + new Vector3(i, 0, j);
                    point.Y = m_initialHeight;
                    Globals.Terrain.SetHeightAtPointWorld(ref point);
                    Globals.Terrain.SetTerrainTypeAndOccupier(point, TerrainType.castle,this);
                }
            }

            m_scaleTransform = Matrix.CreateScale(width, 2, width);
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

        // Different castle sizes.
        public static int[] CastleSizes = new int[]{2,4,6};

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private class Turret
        {

            public Turret(Castle castle,Vector3 position)
            {
                m_position = position;
                m_castle = castle;
            }

            public void Initialize()
            {

            }

            public void Update(GameTime gameTime)
            {

                if (m_cooldownTime > 0f)
                {
                    // busy doing something . need to wait.
                    m_cooldownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    m_cooldownTime = Math.Max(m_cooldownTime, 0f);
                }
                else
                {
                    // if idle then look for new target
                    // if target is found then fire spell (arrow?) at that targets position
                    // 
                    if (m_actionState == ActionState.Idle)
                    {
                        m_actionState = ActionState.Searching;
                        List<GameObject> searchResults = new List<GameObject>();
                        GameObjectType searchMask = GameObjectType.magician | GameObjectType.balloon | GameObjectType.monster;
                        Globals.GameObjectManager.FindObjects(searchMask, m_position, turretSearchRadius, null, searchResults);

                        if (searchResults.Count == 0)
                        {
                            m_actionState = ActionState.Idle;
                            m_cooldownTime = searchFrequency;
                        }
                        else
                        {
                            // nearest target is first in list.
                            GameObject targetObject = searchResults[0];
                            m_actionState = ActionState.Attacking;
                            m_cooldownTime = attackFrequency;
                            m_castle.CastSpell(SpellType.Fireball,m_position,targetObject.Position);
                        }
                    }
                }
            }

            public void Cleanup()
            {
            
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////////

            Vector3 m_position;
            float m_cooldownTime;
            float m_damage;
            ActionState m_actionState;
            Castle m_castle;

            const float turretSearchRadius = 10.0f;
            const float searchFrequency = 2.0f;
            const float attackFrequency = 3.0f;

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private int m_level;
        private float m_storedMana;
        private float m_initialHeight;

        private List<Turret> m_turrets = new List<Turret>();



    }
}
