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
using com.xexuxjy.magiccarpet.interfaces;
using BulletXNA;
using com.xexuxjy.magiccarpet.collision;

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


        public override CollisionFilterGroups GetCollisionFlags()
        {
            return (CollisionFilterGroups)GameObjectType.castle;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionFilterGroups GetCollisionMask()
        {
            return (CollisionFilterGroups)(GameObjectType.spell);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            m_motionState = new SimpleMotionState();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public override void SetStartAttributes()
        {
            m_attributes[(int)GameObjectAttributeType.Health] = new GameObjectAttribute(GameObjectAttributeType.Health, 100);
            // silly amount of mana.
            m_attributes[(int)GameObjectAttributeType.Mana] = new GameObjectAttribute(GameObjectAttributeType.Mana, float.MaxValue);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public override void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in m_castleParts)
            {
                gameObject.Update(gameTime);
            }
            base.Update(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            foreach (GameObject gameObject in m_castleParts)
            {
                gameObject.Draw(gameTime);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Cleanup()
        {
            if (m_castleParts != null)
            {
                foreach (GameObject gameObject in m_castleParts)
                {
                    gameObject.Cleanup();
                }
                m_castleParts.Clear();
            }

            m_castleParts = null;
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
            return CanPlaceLevel(Position,level);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        public static bool CanPlaceLevel(Vector3 position,int level)
        {
            // go to the terrain and make sure that there is nothing nearby that will interfere.
            int width = GetWidthForLevel(level) ;

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

        public static int GetWidthForLevel(int level)
        {
            return s_levelMap[level][0].Length;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////



        public void GrowToLevel(int level)
        {
            if (level >= 0 && level <= s_levelMap.Length)
            {
                Debug.Assert(CanPlaceLevel(Position, level));

                Level = level;

                float width = GetWidthForLevel(level);
                String[] dataForLevel = s_levelMap[level];


                Vector3 startPos = Position;
                Vector3 offset = new Vector3(width / 2, 0, width / 2);

                int borderWidth = 1;

                // fudge factor for the slope leading up to the flat area...

                startPos -= (offset + new Vector3(borderWidth, 0, borderWidth));

                int areaWidth = (int)(width + (borderWidth * 2));

                Globals.Terrain.SetHeightForArea(startPos, areaWidth, areaWidth, Position.Y);
                //TerrainUpdater.ApplyImmediate(Position, 12, 4, Globals.Terrain);

                Globals.Terrain.UpdateHeightMap();


                Vector3 xOffset = new Vector3(1, 0, 0);
                Vector3 zOffset = new Vector3(0, 0, 1);
                Matrix verticalRotation = Matrix.CreateRotationY(MathUtil.SIMD_HALF_PI);
                Vector3 currentPosition = startPos;
                for (int i = 0; i < dataForLevel.Length; ++i)
                {
                    // reset to start of line
                    currentPosition.X = startPos.X;
                    for (int j = 0; j < dataForLevel[i].Length; ++j)
                    {
                        GameObject castlePart = null;
                        if (dataForLevel[i][j] == 'T')
                        {
                            castlePart = new CastleTower(this, currentPosition);
                        }
                        else if (dataForLevel[i][j] == 'V')
                        {
                            castlePart = new CastleWall(this, currentPosition, verticalRotation);
                        }
                        else if (dataForLevel[i][j] == 'H')
                        {
                            castlePart = new CastleWall(this, currentPosition, Matrix.Identity);
                        }

                        // only add if we created.
                        if (castlePart != null)
                        {
                            castlePart.Initialize();
                            //castlePart.PositionBase = currentPosition;
                            m_castleParts.Add(castlePart);
                        }
                        currentPosition += xOffset;
                    }
                    currentPosition += zOffset;
                }
            }

            //CreateBalloon();
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
        public static int[] CastleSizes = new int[]{6,8,12};



        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public static String[][] s_levelMap = new String[][]{
                                                       new String[]
                                                       {"T"},
                                                       new String[]
                                                       {"THHT",
                                                        "V  V", 
                                                        "V  V",  
                                                        "THHT"},
                                                       new String[]
                                                       {"THHTHHT",
                                                        "V     V", 
                                                        "V     V", 
                                                        "T  T  T",
                                                        "V     V", 
                                                        "V     V", 
                                                        "THHTHHT"},
                                                        new String[]
                                                       {"THHTHHTHHT",
                                                        "V        V", 
                                                        "V        V", 
                                                        "T        T",
                                                        "V        V", 
                                                        "V        V", 
                                                        "T        T",
                                                        "V        V", 
                                                        "V        V", 
                                                        "THHTHHTHHT"}};

        private int m_level;
        private float m_storedMana;
        private float m_initialHeight;

        public static Vector3 s_castleTowerSize = new Vector3(1,2,1);
        public static Vector3 s_castleWallSize = new Vector3(1, 1, 0.5f);


        private List<GameObject> m_castleParts = new List<GameObject>();

    }
}
