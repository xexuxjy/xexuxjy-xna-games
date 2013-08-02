using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using Gladius.util;
using Microsoft.Xna.Framework;

namespace Gladius.renderer
{
    public class SimpleArenaRenderer 
    {

        public void LoadContent(Game game,GraphicsDevice device)
        {
            m_contentManager = new ContentManager(game.Services,"GladiusContent");
            m_boxModel = m_contentManager.Load<Model>("Models/Shapes/UnitCube");
            m_basicEffect = new BasicEffect(device);
            m_basicEffect.TextureEnabled = true;
            
        }



        public void Draw(Arena arena,ICamera camera,GraphicsDevice graphicsDevice)
        {
            m_basicEffect.View = camera.ViewMatrix;
            m_basicEffect.Projection = camera.ProjectionMatrix;


            // Draw the ground....

            Vector3 scale = new Vector3(arena.Width, 1f, arena.Breadth);
            Vector3 translation = new Vector3(0, -0.5f, 0);
            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(translation);

            m_basicEffect.World = world;
            m_basicEffect.Texture = ColouredTextureDictionary.GetTexture(Color.LawnGreen,graphicsDevice);
            m_boxModel.Draw(world,camera.ViewMatrix,camera.ProjectionMatrix);


            for (int i = 0; i < arena.Width; ++i)
            {
                for (int j = 0; j < arena.Breadth; ++i)
                {
                    //m_basicEffect.World = Matrix.CreateFromT
                    SquareType squareType = arena.SquareTypeAtLocation(i, j);
                    Texture2D texture2d = null;



                    switch (squareType)
                    {
                        case (SquareType.Wall):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Brown, graphicsDevice);
                                break;
                            }

                    }

                    if (texture2d != null)
                    {
                        m_basicEffect.Texture = texture2d;
                        translation = new Vector3(i, 0.5f, j);
                        world = Matrix.CreateTranslation(translation);
                        m_basicEffect.World = world;
                        m_boxModel.Draw(world, camera.ViewMatrix, camera.ProjectionMatrix);
                    }
                }
            }
        }

        BasicEffect m_basicEffect;
        ContentManager m_contentManager;
        Model m_boxModel;

    }
}
