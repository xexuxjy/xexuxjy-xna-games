using System;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class SkyDome : GameObject
    {
        public SkyDome()
            : base(GameObjectType.skydome)
        {
            m_model = Globals.MCContentManager.ModelForObjectType(GameObjectType);
            DrawOrder = Globals.TERRAIN_DRAW_ORDER;
        }

        public override void Initialize()
        {
            m_scaleTransform = IndexedMatrix.CreateScale(10);

        }

        public override void Draw(GameTime gameTime)
        {
        }


        public override void Update(GameTime gameTime)
        {
        }


        public override Texture2D GetTexture()
        {
            return Globals.MCContentManager.GetTexture("SkyDome");
            //return Globals.MCContentManager.GetTexture(Color.HotPink);

        }

        public override IndexedVector3 Position
        {
            get
            {
                IndexedVector3 position = Globals.Player.Position;
                position.Y += m_heightOffset;
                return position;
            }
        }

        public override IndexedMatrix WorldTransform
        {
            get
            {
                IndexedMatrix im = Globals.Player.WorldTransform;
                im._origin.Y += m_heightOffset;
                return im;
          
            }
            set { }
        }

        private float m_heightOffset = -10f;
    }
}
