using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Gladius.Terrain7;

namespace Gladius.renderer
{
    /*
    * Created on 11-Jan-2006
    *
    * To change the template for this generated file go to
    * Window - Preferences - Java - Code Generation - Code and Comments
    */
    public class Terrain : DrawableGameComponent
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(Game game)
            : base(game)
        {
        
        }

        public void LoadContent(ContentManager contentManager)
        {
            m_heightMap = contentManager.Load<Texture2D>("Models/Terrain/WorldHeight");
            m_surface = contentManager.Load<Texture2D>("Models/Terrain/WorldSurface");
            Vector3 bounds = new Vector3(512, 10, 512);

            m_quadTree = new QuadTree(-bounds,m_heightMap,Globals.Camera.View,Globals.Camera.Projection,Game.GraphicsDevice,1,1.0f);
            m_quadTree.Texture = m_surface;
        }

        public override void Update(GameTime gameTime)
        {
            m_quadTree.View = Globals.Camera.View;
            m_quadTree.Projection = Globals.Camera.Projection;
            m_quadTree.CameraPosition = Globals.Camera.Position;
            m_quadTree.Update(gameTime);  
        }

        public override void Draw(GameTime gameTime)
        {
            m_quadTree.Draw(gameTime);
        }

        private QuadTree m_quadTree;
        Texture2D m_heightMap;
        Texture2D m_surface;
    }

}
