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
using Gladius.control;

namespace Gladius.renderer
{
    public class SimpleArenaRenderer 
    {
        public SimpleArenaRenderer(Arena arena)
        {
            m_arena = arena;
        }


        public void LoadContent(Game game,GraphicsDevice device)
        {
            //m_contentManager = new ContentManager(game.Services,"Content");
            //m_boxModel = m_contentManager.Load<Model>("Models/Shapes/UnitCube");
            m_boxModel = game.Content.Load<Model>("UnitCube");

            m_boneTransforms = new Matrix[m_boxModel.Bones.Count];
            m_boxModel.CopyAbsoluteBoneTransformsTo(m_boneTransforms);

            m_movementGrid = new MovementGrid(game,m_arena);
            game.Components.Add(m_movementGrid);
            m_movementGrid.LoadContent();
            m_movementGrid.CurrentPosition = new Point(12, 12);
        }



        public void Draw(ICamera camera,GraphicsDevice graphicsDevice)
        {
            float aspectRatio = graphicsDevice.Viewport.AspectRatio;
            //m_projection=
            //       Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
            //       aspectRatio, 1.0f, 10000.0f);
            //m_view = Matrix.CreateLookAt(new Vector3(0.0f, 50.0f, 1f),
            //    Vector3.Zero, Vector3.Up);

            m_projection = camera.ProjectionMatrix;
            m_view = camera.ViewMatrix;


            // Draw the ground....
            Vector3 topLeft = new Vector3(-m_arena.Width / 2f, 0, -m_arena.Breadth / 2f);

            Vector3 scale = new Vector3(m_arena.Width, 1f, m_arena.Breadth);
            Vector3 translation = new Vector3(0, -0.5f, 0);

            DrawBox(scale, ColouredTextureDictionary.GetTexture(Color.LawnGreen, graphicsDevice), translation);
            Texture2D texture2d = null;

            for (int i = 0; i < m_arena.Width; ++i)
            {
                for (int j = 0; j < m_arena.Breadth; ++j)
                {
                    //m_basicEffect.World = Matrix.CreateFromT
                    float groundHeight = 0f;
                    Vector3 boxScale = new Vector3(0.5f);

                    SquareType squareType = m_arena.SquareTypeAtLocation(i, j);
                    texture2d = null;
                    switch (squareType)
                    {
                        case (SquareType.Wall):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Brown, graphicsDevice);
                                break;
                            }
                        case (SquareType.Level1):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Wheat, graphicsDevice);
                                boxScale = new Vector3(0.5f, 0.25f, 0.5f);
                                break;
                            }
                        case (SquareType.Level2):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Wheat, graphicsDevice);
                                boxScale = new Vector3(0.5f);
                                break;
                            }
                        case (SquareType.Level3):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Wheat, graphicsDevice);
                                boxScale = new Vector3(0.75f);
                                break;
                            }
                    }

                    if (texture2d != null)
                    {
                        translation = topLeft + new Vector3(i, boxScale.Y, j);
                        DrawBox(boxScale, texture2d, translation);
                    }
                }
            }

            // Draw Actors.
            texture2d = ColouredTextureDictionary.GetTexture(Color.Pink, graphicsDevice);

            foreach (BaseActor ba in m_arena.PointActorMap.Values)
            {
                float groundHeight = m_arena.GetHeightAtLocation(ba.CurrentPoint);
                translation = topLeft + new Vector3(ba.CurrentPoint.X, groundHeight+0.5f, ba.CurrentPoint.Y);
                DrawBox(Vector3.One, texture2d, translation);
            }

            m_movementGrid.Draw(graphicsDevice, camera);

        }

        public void DrawBox(Vector3 scale, Texture2D t,Vector3 position)
        {
            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            //Matrix world = Matrix.CreateTranslation(position);
            foreach (ModelMesh mm in m_boxModel.Meshes)
            {
                foreach (BasicEffect effect in mm.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = t;
                    effect.View = m_view;
                    effect.Projection = m_projection;
                    effect.World = m_boneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }
        }


        Matrix m_view;
        Matrix m_projection;

        BasicEffect m_basicEffect;
        Matrix[] m_boneTransforms;
        ContentManager m_contentManager;
        Model m_boxModel;
        MovementGrid m_movementGrid;
        Arena m_arena;

    }
}
