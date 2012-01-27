using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.renderer;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.manager;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.gui
{
    public class MiniMap : DrawableGameComponent
    {
        public MiniMap(int width,int x,int y) : base(Globals.Game)
        {
            m_miniMapWidth = width;
            m_gameObjectList = new List<GameObject>(128);
            m_radius = 100.0f;
            m_bounds = new Rectangle(x, y, width, width);
            m_zoomLevel = 10;
            m_mapWorldPosition = IndexedVector3.Zero;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void SetTrackedObject(GameObject trackedObject)
        {
            m_trackedObject = trackedObject;
        }


        protected override void LoadContent()
        {
            m_minimapTexture = new Texture2D(Game.GraphicsDevice, m_miniMapWidth, m_miniMapWidth,false, SurfaceFormat.Color);
            Color[] colorData = new Color[m_miniMapWidth*m_miniMapWidth];
            for(int i=0;i<colorData.Length;++i)
            {
                colorData[i] = Color.BlanchedAlmond;
            }
            m_minimapTexture.SetData<Color>(colorData);

            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_mapSpriteAtlas = Game.Content.Load<Texture2D>("textures/ui/MapTextureAtlas");
        }

        public override void Update(GameTime gameTime)
        {
            // draw the terrain   ?? scale issues?

        }


        public Vector2 WorldToMap(IndexedVector3 position,int i)
        {
            IndexedVector3 mapRelativePosition = position - m_mapWorldPosition;

            float scale = ((float)m_miniMapWidth / (float)Globals.WorldWidth * m_zoomLevel);

            Vector2 screen = new Vector2(mapRelativePosition.X, mapRelativePosition.Z);
            // center the object in the world

            screen += new Vector2(Globals.WorldWidth / 2, Globals.WorldWidth / 2);

            screen *= scale;

            screen.X += m_bounds.Left;
            screen.Y += m_bounds.Top;

            return screen;
        }

        public Color ColorForObject(GameObject gameObject)
        {
            return gameObject.BadgeColor;

        }

        public Rectangle? SpritePositionForGameObject(GameObject gameObject)
        {
            int xpos = 0;
            int ypos = 0;
            bool found = false;
            Rectangle? result = null;
            switch (gameObject.GameObjectType)
            {
                case (GameObjectType.balloon):
                    {
                        xpos = 0;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (GameObjectType.castle):
                    {
                        xpos = 1;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (GameObjectType.manaball):
                    {
                        xpos = 2;
                        ypos = 0;
                        found = true;
                        break;
                    }
                default:
                    {
                        break;
                    }

            }

            if (found)
            {
                result = new Rectangle(xpos * m_spriteWidth,ypos * m_spriteWidth,m_spriteWidth,m_spriteWidth);
            }
            return result;
        }


        public override void Draw (GameTime gameTime)
        {
            if (m_trackedObject != null)
            {
                m_gameObjectList.Clear();
                // draw the components.
                Globals.GameObjectManager.FindObjects(GameObjectManager.m_allActiveObjectTypes, m_trackedObject.Position, m_radius, m_trackedObject, m_gameObjectList, true);

                m_spriteBatch.Begin();
                m_spriteBatch.Draw(m_minimapTexture, m_bounds, Color.White);
        
                int counter = 0;
                foreach (GameObject gameObject in m_gameObjectList)
                {
                    Vector2 position = WorldToMap(gameObject.Position,counter++);
                    Rectangle? sourceRectangle = SpritePositionForGameObject(gameObject);
                    Color objectColor = ColorForObject(gameObject);

                    if (sourceRectangle.HasValue)
                    {
                        m_spriteBatch.Draw(m_mapSpriteAtlas, position, sourceRectangle, objectColor);
                    }

                }
                m_spriteBatch.End();
            }
        }

        private float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }



        private Texture2D m_mapSpriteAtlas;
        private List<GameObject> m_gameObjectList;
        private float m_radius;
        private int m_miniMapWidth;
        private int m_spriteWidth = 16;
        private int m_zoomLevel;
        private Rectangle m_bounds;
        private IndexedVector3 m_mapWorldPosition;

        private Color[] m_colorDataArray;
        private Texture2D m_minimapTexture;
        private SpriteBatch m_spriteBatch;
        private GameObject m_trackedObject;
    }
}
