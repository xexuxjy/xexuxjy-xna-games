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
using BulletXNA;

namespace com.xexuxjy.magiccarpet.gui
{
    public class MiniMap : GuiComponent
    {
        public MiniMap(Point topLeft,int width) : base(topLeft,width)
        {
            m_gameObjectList = new List<GameObject>(128);
            m_radius = 100.0f;
            m_mapWorldPosition = IndexedVector3.Zero;
            Zoom = 5;
            Globals.TrackedObjectChanged += new Globals.TrackedObjectChangedEventHandler(Globals_TrackedObjectChanged);

        }

        void Globals_TrackedObjectChanged(object sender, GameObject oldObject, GameObject newObject)
        {
            m_trackedObject = newObject;
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
            m_texture = new Texture2D(Game.GraphicsDevice, m_width, m_width,false, SurfaceFormat.Color);
            Color[] colorData = new Color[m_width*m_width];
            for(int i=0;i<colorData.Length;++i)
            {
                colorData[i] = Color.BlanchedAlmond;
            }
            m_texture.SetData<Color>(colorData);

            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_mapSpriteAtlas = Globals.MCContentManager.GetTexture("MiniMapAtlas");
        }

        public override void Update(GameTime gameTime)
        {
            // draw the terrain   ?? scale issues?

        }

        public Color ColorForObject(GameObject gameObject)
        {
            return gameObject.BadgeColor;

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
                //m_spriteBatch.Draw(m_texture, m_rectangle, Color.White);
                int textureWidth = Globals.Terrain.BaseTexture.Width;

                Vector3 objectPosition = m_trackedObject.Position;

                Vector2 playerPositionInWorld = new Vector2(objectPosition.X, objectPosition.Z);

                float worldWidth =  Globals.WorldWidth;
                float halfWidth = worldWidth / 2.0f;

                Vector2 halfOffset = new Vector2(m_halfSpan);
                Vector2 topLeft = playerPositionInWorld - halfOffset;
                topLeft += new Vector2(halfWidth);
                topLeft /= worldWidth;
    
                Vector2 bottomRight = playerPositionInWorld + halfOffset;
                bottomRight += new Vector2(halfWidth);
                bottomRight /= worldWidth;

                topLeft *= textureWidth;
                bottomRight *= textureWidth;
                int width = (int)(bottomRight.X - topLeft.X);

                Rectangle terrainTexelRectangle = new Rectangle((int)topLeft.X, (int)topLeft.Y, width, width);

                m_spriteBatch.Draw(Globals.Terrain.BaseTexture, m_rectangle, terrainTexelRectangle, Color.White);
        
                int counter = 0;
                foreach (GameObject gameObject in m_gameObjectList)
                {
                    //Vector3 playerRelativePosition = objectPosition - m_trackedObject.Position;
                    //Vector3 playerRelativePosition = objectPosition - Globals.Camera.Position;
                    Vector3 playerRelativePosition = Globals.Camera.Position - objectPosition;

                    if (InBounds((playerRelativePosition)))
                    {
                        Vector2 mapPos = new Vector2((playerRelativePosition.X + m_halfSpan) / m_span, (playerRelativePosition.Z + m_halfSpan) / m_span);
                        mapPos *= m_width;

                        mapPos.X  += m_componentTopCorner.X;
                        mapPos.Y += m_componentTopCorner.Y;


                        Rectangle? sourceRectangle = Globals.MCContentManager.MiniMapSpritePositionForGameObject(gameObject);

                        if (sourceRectangle.HasValue)
                        {
                            Color objectColor = ColorForObject(gameObject);
                            m_spriteBatch.Draw(m_mapSpriteAtlas, mapPos, sourceRectangle, objectColor);
                        }
                    }

                }

                m_spriteBatch.Draw(Globals.MCContentManager.GetTexture("MiniMapFrame"), m_rectangle, Color.White);

                m_spriteBatch.End();
            }
        }

        private float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        public void ZoomIn()
        {
            Zoom += 1;
        }

        public void ZoomOut()
        {
            Zoom -= 1;
        }

        public int Zoom
        {
            get
            {
                return m_zoomLevel;
            }

            set
            {
                m_zoomLevel = MathUtil.Clamp(value, 1, 10);
                m_halfSpan = Globals.WorldWidth / (2 * m_zoomLevel);
                m_span = 2 * m_halfSpan;

                m_bounds = new Vector4(m_mapWorldPosition.X - m_halfSpan, m_mapWorldPosition.Z - m_halfSpan, m_span, m_span);
                m_boundsRect = new Rectangle((int)m_bounds.X, (int)m_bounds.Y, (int)m_bounds.Z, (int)m_bounds.W);

            }
        }

        private Texture2D m_mapSpriteAtlas;
        private List<GameObject> m_gameObjectList;
        private float m_radius;
        
        private int m_zoomLevel;
        private int m_halfSpan;
        private int m_span;

        private Vector4 m_bounds;
        private Rectangle m_boundsRect;
        private IndexedVector3 m_mapWorldPosition;

        private GameObject m_trackedObject;
    }
}
