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
            Model model = ContentManager.Load<Model>("Models/Shapes/UnitCube");
            m_boxModelData = new ModelData(model, 1f,0,null);

            Vector3 floorScale = new Vector3(m_arena.Width, 1, m_arena.Breadth);
            m_floorModelData = new ModelData(ContentManager.Load<Model>("Models/Arena/Arena2/Floor"), floorScale, 0f, ContentManager.Load<Texture2D>("Models/Arena/Arena2/ArenaFloor_0"));
            m_wallModelData = new ModelData(ContentManager.Load<Model>("Models/Arena/Arena2/Wall"), 1f, 0f, ContentManager.Load<Texture2D>("Models/Arena/Arena2/ArenaWall_0"));
            m_pillarModelData = new ModelData(ContentManager.Load<Model>("Models/Arena/Common/Pillar1"), new Vector3(0.5f,3,0.5f), 0.5f, ContentManager.Load<Texture2D>("Models/Arena/Common/PillarTexture_0"));

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


            // Draw the ground....
            Vector3 topLeft = new Vector3(-m_arena.Width / 2f, 0, -m_arena.Breadth / 2f);

            //Vector3 scale = new Vector3(m_arena.Width, 1f, m_arena.Breadth) / 2f;

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            Vector3 translation = new Vector3(0, 0, 0);

            // draw floor and walls.
            m_floorModelData.Draw(camera, translation);
            translation = new Vector3(0,0,topLeft.Z);
            //Matrix rotation = Matrix.CreateRotationY()
            m_wallModelData.Draw(camera, translation);
            translation = new Vector3(0, 0, -topLeft.Z);
            m_wallModelData.Draw(camera, translation);
            Matrix rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            translation = new Vector3(topLeft.X, 0, 0);
            m_wallModelData.Draw(camera, translation,Vector3.One,rotation);
            translation = new Vector3(-topLeft.X, 0, 0);
            m_wallModelData.Draw(camera, translation, Vector3.One, rotation);


            translation = new Vector3(6,0,6);
            rotation = Matrix.Identity;
            //DrawBarrel(graphicsDevice,m_barrelModelData, translation, Vector3.One, rotation);
            //DrawModelData(m_pillarModelData, translation, Vector3.One, rotation);


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
                    float heightAtPoint = m_arena.GetHeightAtLocation(new Point(i, j));
                    texture2d = null;
                    switch (squareType)
                    {

                        case (SquareType.Wall):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Brown, graphicsDevice);
                                break;
                            }
                        case (SquareType.Level1):
                        case (SquareType.Level2):
                        case (SquareType.Level3):
                            {
                                texture2d = ColouredTextureDictionary.GetTexture(Color.Wheat, graphicsDevice);
                                boxScale = new Vector3(0.5f, heightAtPoint/2f, 0.5f);
                                break;
                            }
                        case (SquareType.Pillar):
                            {
                                rotation = Matrix.Identity;
                                translation = m_arena.ArenaToWorld(new Point(i, j));
                                m_pillarModelData.Draw(camera, translation, m_pillarModelData.ModelScale, rotation);
                                break;
                            }
                    }

                    if (texture2d != null)
                    {
                        translation = m_arena.ArenaToWorld(new Point(i, j));
                        //translation += new Vector3(0.5f, 0, 0.5f);
                        translation.Y -= boxScale.Y; 
                        DrawBox(boxScale, texture2d, translation,camera);
                    }
                }
            }



            Globals.DrawCameraDebugText(m_spriteBatch, m_spriteFont, m_gameScreen.ScreenManager.FPS);
        }


        public void DrawBox(Vector3 scale, Texture2D t,Vector3 position,ICamera camera)
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
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = m_boxModelData.BoneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }
        }


        public void DrawBarrel(GraphicsDevice device,ModelData modelData, Vector3 position, Vector3 scale, Matrix rotation)
        {
            Matrix world = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
            //Matrix world = Matrix.CreateTranslation(position);
            device.SamplerStates[0].AddressU = TextureAddressMode.Mirror;
            device.SamplerStates[0].AddressV = TextureAddressMode.Mirror;
            
            foreach (ModelMesh mm in modelData.Model.Meshes)
            {
                int count = 0;
                foreach (ModelMeshPart mp in mm.MeshParts)
                {
                    BasicEffect effect = mp.Effect as BasicEffect;
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = (count == 0 )? modelData.Texture2 : modelData.Texture;
                    effect.View = m_view;
                    effect.Projection = m_projection;
                    effect.World = modelData.BoneTransforms[mm.ParentBone.Index] * world;
                    ++count;
                }
                mm.Draw();
            }
            device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            device.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
        }



        SpriteBatch m_spriteBatch;
        SpriteFont m_spriteFont;

        Matrix m_view;
        Matrix m_projection;
        
        ModelData m_boxModelData;
        ModelData m_floorModelData;
        ModelData m_wallModelData;
        ModelData m_viewingBoxModelData;
        ModelData m_barrelModelData;
        ModelData m_pillarModelData;

        //MovementGrid m_movementGrid;
        Arena m_arena;
    }
}
