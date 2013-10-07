using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.control;
using Gladius.modes.arena;
using GameStateManagement;
using Gladius.gamestatemanagement.screens;

namespace Gladius.renderer
{
    public class SimpleArenaRenderer : GameScreenComponent
    {
        public SimpleArenaRenderer(Arena arena,GameScreen gameScreen) : base(gameScreen)
        {
            m_arena = arena;
        }

        public void LoadContent()
        {   
            m_boxModel = ContentManager.Load<Model>("UnitCube");
            m_boneTransforms = new Matrix[m_boxModel.Bones.Count];
            m_boxModel.CopyAbsoluteBoneTransformsTo(m_boneTransforms);

            m_baseActorTexture = ContentManager.Load<Texture2D>("Models/ThirdParty/test_m");
            m_baseActorModel = ContentManager.Load<Model>("Models/ThirdParty/test_XNA");
            m_baseActorBoneTransforms = new Matrix[m_baseActorModel.Bones.Count];
            m_baseActorModel.CopyAbsoluteBoneTransformsTo(m_baseActorBoneTransforms);

            BoundingSphere actorBs = m_baseActorModel.Meshes[0].BoundingSphere;
            m_baseActorScale = Vector3.One;// new Vector3(1f / actorBs.Radius);

            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_spriteFont = ContentManager.Load<SpriteFont>("UI/fonts/DebugFont8");
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(Globals.Camera, Game.GraphicsDevice,gameTime);
        }


        public void Draw(ICamera camera,GraphicsDevice graphicsDevice,GameTime gameTime)
        {
            float aspectRatio = graphicsDevice.Viewport.AspectRatio;

            m_projection = camera.Projection;
            m_view = camera.View;


            // Draw the ground....
            Vector3 topLeft = new Vector3(-m_arena.Width / 2f, 0, -m_arena.Breadth / 2f);

            Vector3 scale = new Vector3(m_arena.Width, 1f, m_arena.Breadth) / 2f;


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

                    SquareType squareType = m_arena.GetSquareTypeAtLocation(i, j);
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
                                boxScale = new Vector3(0.5f,0.5f,0.5f);
                                break;
                            }
                        case (SquareType.Level3):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Wheat, graphicsDevice);
                                boxScale = new Vector3(0.5f, 0.75f, 0.5f);
                                break;
                            }
                    }

                    if (texture2d != null)
                    {
                        translation = topLeft + new Vector3(i, boxScale.Y, j) + new Vector3(0.5f,0,0.5f);
                        DrawBox(boxScale, texture2d, translation);
                    }
                }
            }

            // Draw Actors.
            texture2d = ColouredTextureDictionary.GetTexture(Color.Pink, graphicsDevice);

            //foreach (BaseActor ba in m_arena.PointActorMap.Values)
            //{
            //    float groundHeight = m_arena.GetHeightAtLocation(ba.CurrentPoint);
            //    translation = topLeft + new Vector3(ba.CurrentPoint.X, groundHeight+0.5f, ba.CurrentPoint.Y);
            //    //DrawBox(Vector3.One, texture2d, translation);
            //    DrawBaseActor3(m_baseActorScale, m_baseActorTexture, translation);
            //}

            //m_movementGrid.Draw(graphicsDevice, camera);


            Globals.DrawCameraDebugText(m_spriteBatch, m_spriteFont,1);
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

        SpriteBatch m_spriteBatch;
        SpriteFont m_spriteFont;

        Matrix m_view;
        Matrix m_projection;
        Matrix[] m_boneTransforms;
        Model m_boxModel;
        //MovementGrid m_movementGrid;
        Arena m_arena;


        Model m_baseActorModel;
        Matrix[] m_baseActorBoneTransforms;
        Vector3 m_baseActorScale;
        Texture2D m_baseActorTexture;
    }
}
