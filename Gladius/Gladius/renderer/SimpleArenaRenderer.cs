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
using Gladius.gamestatemanagement.screens;
using Gladius.renderer;
using Gladius.gamestatemanagement.screenmanager;

namespace Gladius.renderer
{
    public class SimpleArenaRenderer : GameScreenComponent
    {
        public SimpleArenaRenderer(Arena arena,GameScreen gameScreen) : base(gameScreen)
        {
            m_arena = arena;
            DrawOrder = Globals.ArenaDrawOrder;
        }

        public void LoadContent()
        {   
            Model model = ContentManager.Load<Model>("UnitCube");
            m_boxModelData = new ModelData(model, null);

            m_floorModelData = new ModelData(ContentManager.Load<Model>("Models/Arena/Arena2/Floor"), ContentManager.Load<Texture2D>("Models/Arena/Arena2/ArenaFloor_0"));
            m_wallModelData = new ModelData(ContentManager.Load<Model>("Models/Arena/Arena2/Wall"), ContentManager.Load<Texture2D>("Models/Arena/Arena2/ArenaWall_0"));

            //Model pillarModel = ContentManager.Load<Model>("Models/Arena/Common/Pillar1");

            //m_baseActorTexture = ContentManager.Load<Texture2D>("Models/ThirdParty/test_m");
            //m_baseActorModel = ContentManager.Load<Model>("Models/ThirdParty/test_XNA");
            //m_baseActorBoneTransforms = new Matrix[m_baseActorModel.Bones.Count];
            //m_baseActorModel.CopyAbsoluteBoneTransformsTo(m_baseActorBoneTransforms);

            //BoundingSphere actorBs = m_baseActorModel.Meshes[0].BoundingSphere;
            //m_baseActorScale = Vector3.One;// new Vector3(1f / actorBs.Radius);

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

            //Vector3 scale = new Vector3(m_arena.Width, 1f, m_arena.Breadth) / 2f;


            Vector3 translation = new Vector3(0, 0, 0);

            // draw floor and walls.
            DrawModelData(m_floorModelData, translation);
            translation = new Vector3(0,0,topLeft.Z);
            //Matrix rotation = Matrix.CreateRotationY()
            DrawModelData(m_wallModelData, translation);
            translation = new Vector3(0, 0, -topLeft.Z);
            DrawModelData(m_wallModelData, translation);
            Matrix rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            translation = new Vector3(topLeft.X, 0, 0);
            DrawModelData(m_wallModelData, translation,Vector3.One,rotation);
            translation = new Vector3(-topLeft.X, 0, 0);
            DrawModelData(m_wallModelData, translation, Vector3.One, rotation);


            //DrawBox(scale, ColouredTextureDictionary.GetTexture(Color.LawnGreen, graphicsDevice), translation);
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



            Globals.DrawCameraDebugText(m_spriteBatch, m_spriteFont,1);
        }


        public void DrawBox(Vector3 scale, Texture2D t,Vector3 position)
        {
            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            //Matrix world = Matrix.CreateTranslation(position);
            foreach (ModelMesh mm in m_boxModelData.Model.Meshes)
            {
                foreach (BasicEffect effect in mm.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = t;
                    effect.View = m_view;
                    effect.Projection = m_projection;
                    effect.World = m_boxModelData.BoneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }
        }

        public void DrawModelData(ModelData modelData, Vector3 position)
        {
            DrawModelData(modelData, position, Vector3.One,Matrix.Identity);
        }

        public void DrawModelData(ModelData modelData,Vector3 position,Vector3 scale,Matrix rotation)
        {
            Matrix world = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
            //Matrix world = Matrix.CreateTranslation(position);
            foreach (ModelMesh mm in modelData.Model.Meshes)
            {
                foreach (BasicEffect effect in mm.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = modelData.Texture;
                    effect.View = m_view;
                    effect.Projection = m_projection;
                    effect.World = modelData.BoneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }
        }


        SpriteBatch m_spriteBatch;
        SpriteFont m_spriteFont;

        Matrix m_view;
        Matrix m_projection;
        
        ModelData m_boxModelData;
        ModelData m_floorModelData;
        ModelData m_wallModelData;
        ModelData m_viewingBoxModelData;

        //MovementGrid m_movementGrid;
        Arena m_arena;
    }
}
