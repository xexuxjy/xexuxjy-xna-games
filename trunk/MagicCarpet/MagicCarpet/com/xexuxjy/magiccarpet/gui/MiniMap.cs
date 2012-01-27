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
    public class MiniMap : GuiComponent
    {
        public MiniMap(int x,int y,int width) : base(x,y,width)
        {
            m_miniMapWidth = width;
            m_gameObjectList = new List<GameObject>(128);
            m_radius = 100.0f;
            m_mapWorldPosition = IndexedVector3.Zero;

            m_halfSpan = Globals.WorldWidth / 2 * m_zoomLevel;
            m_span = 2 * m_halfSpan;

            m_bounds = new Vector4(m_mapWorldPosition.X - m_halfSpan, m_mapWorldPosition.Z - m_halfSpan, m_span, m_span);
            m_boundsRect = new Rectangle((int)m_bounds.X, (int)m_bounds.Y, (int)m_bounds.Z, (int)m_bounds.W);
            m_zoomLevel = 10;


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

        // xz coordinates from position against a vector4 as left,top,width,height
        public bool InBounds(IndexedVector3 position)
        {
            if (position.X < m_bounds.X) return false;
            if (position.Z < m_bounds.Y) return false;
            if (position.X > m_bounds.X + m_bounds.Z) return false;
            if (position.Z > m_bounds.Y + m_bounds.W) return false;
            return true;
        }


         
 
        public override void Draw (GameTime gameTime)
        {
            if (m_trackedObject != null)
            {
                m_gameObjectList.Clear();
                // draw the components.
                Globals.GameObjectManager.FindObjects(GameObjectManager.m_allActiveObjectTypes, m_trackedObject.Position, m_radius, m_trackedObject, m_gameObjectList, true);

                m_spriteBatch.Begin();
                m_spriteBatch.Draw(m_minimapTexture, m_boundsRect, Color.White);
        
                int counter = 0;
                foreach (GameObject gameObject in m_gameObjectList)
                {
                    if (InBounds((gameObject.Position)))
                    {
                        Vector2 mapPos = new Vector2((gameObject.Position.X + m_halfSpan) / m_span, (gameObject.Position.Z + m_halfSpan) / m_span);
                        mapPos += m_mapTopCorner;


                        Rectangle? sourceRectangle = SpritePositionForGameObject(gameObject);
                        Color objectColor = ColorForObject(gameObject);

                        if (sourceRectangle.HasValue)
                        {
                            m_spriteBatch.Draw(m_mapSpriteAtlas, mapPos, sourceRectangle, objectColor);
                        }
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
        private int m_halfSpan;
        private int m_span;
        private Vector4 m_bounds;
        private Rectangle m_boundsRect;
        private IndexedVector3 m_mapWorldPosition;
        private Vector2 m_mapTopCorner;


        // need cone texture for facing or something.

        private Color[] m_colorDataArray;
        private Texture2D m_minimapTexture;
        private GameObject m_trackedObject;
    }
}
