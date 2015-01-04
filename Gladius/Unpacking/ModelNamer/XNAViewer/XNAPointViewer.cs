using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ModelNamer;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace XNAViewer
{
    public class XNAPointViewer : Microsoft.Xna.Framework.Game
    {
        static int size = 5;
        Vector3 eyePos = new Vector3(size * 5, size * 5, 0);
        Vector3 eyeLookat = new Vector3(0, 0, 0);
        const float rotation_speed = 1.0f;
        float angle;
        SpriteFont viewerFont;
        SpriteBatch spriteBatch;
        private Matrix cameraMatrix;
        private Matrix rotationMatrix;
        private float[] mouseSpeed = new float[2];
        private Vector2 mouseDelta;
        private Vector3 location;
        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;
        //public String textureBasePath = @"D:\gladius-extracted-archive\gc-compressed\textures.jpg\";
        public String textureBasePath = @"c:\tmp\gladius-extracted-archive\gc-compressed\textures-resized\";
        //public String textureBasePath = @"C:\temp\textures\";
        public Texture2D m_missingTexture;
        public List<String> m_fileNames = new List<string>();
        public Dictionary<String, WrappedModel> m_modelMap = new Dictionary<string, WrappedModel>();

        public GraphicsDeviceManager m_graphicsDeviceManager;

        public Model m_unitSphere;
        public Model m_unitCone;
        public Model m_unitCylinder;
        public Model m_unitCube;


        RasterizerState m_noCullState = new RasterizerState();



        InputState inputState;


        Matrix projectionMatrix;
        Matrix modelviewMatrix;

        BasicEffect m_basicEffect;
        Effect m_metalEffect;

        public XNAPointViewer()
        {
            m_graphicsDeviceManager = new GraphicsDeviceManager(this);
            m_graphicsDeviceManager.PreferredBackBufferWidth = 1024;
            m_graphicsDeviceManager.PreferredBackBufferHeight = 768;
            //.GraphicsDeviceManager.Globals.GraphicsDeviceManager.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
            Content.RootDirectory = "Content";
            inputState = new InputState();
            //CreateEmptyTexture();
            //m_textPrinter = new OpenTK.Graphics.TextPrinter();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            CreateShaders();

            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rs;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            viewerFont = Content.Load <SpriteFont> ("ViewerFont");

            m_unitCone = Content.Load<Model>("UnitCone");
            m_unitSphere = Content.Load<Model>("UnitSphere");
            m_unitCube = Content.Load<Model>("UnitCube");
            m_unitCylinder = Content.Load<Model>("UnitCylinder");


            m_missingTexture = GetTexture(Color.Pink);


            //this.VSync = VSyncMode.Off;
            //m_modelReader = new GCModelReader();
            m_modelReader = new XboxModelReader();
            string baseModelPath = m_modelReader is GCModelReader ? @"c:\tmp\gladius-extracted-archive\gc-compressed\AllModelsRenamed\" : @"c:\tmp\gladius-extracted-archive\xbox-decompressed\ModelFilesRenamed\";
            //string baseModelPath = m_modelReader is GCModelReader ? @"D:\gladius-extracted-archive\gc-compressed\UnidentifiedModels\" : @"D:\gladius-extracted-archive\xbox-decompressed\ModelFilesRenamed\";

            if (m_modelReader is XboxModelReader)
            {
                m_noCullState.CullMode = CullMode.None;
                GraphicsDevice.RasterizerState = m_noCullState;
            }

            
            //m_modelReader = new XboxModelReader();
            //m_fileNames.AddRange(Directory.GetFiles(@"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed", "*"));
            //m_fileNames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\weapons", "*"));
            //m_fileNames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\yeti.mdl");
            //m_xbModelReader = new XboxModelReader();
            //m_fileNames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\alpha_box.mdl");
            //m_fileNames.Add(@"D:\gladius-extracted-archive\xbox-decompressed\ModelFilesRenamed\alpha_box.mdl");

            //NB Bounding volumes for xbox models and GC models appear to be the same.
            //Handy debug test.

            //todo - write something to compare bb's of each to text file for general testing.

            //m_fileNames.Add(baseModelPath+"anklet_queen.mdl");
            //m_fileNames.Add(baseModelPath + @"characters\prop_practicepost1.mdl");
            //m_fileNames.Add(baseModelPath + @"characters\prop_practicepost2.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bow_amazon.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\swordM_gladius.mdl");
            m_fileNames.Add(baseModelPath + @"characters\bear.mdl");
            //m_fileNames.Add(baseModelPath + @"characters\scarab.mdl");
            //m_fileNames.Add(baseModelPath + @"characters\galverg.mdl");
            //m_fileNames.Add(baseModelPath + @"arenas\belfortgatenor.mdl");
            
            //m_fileNames.Add(baseModelPath + @"characters\amazon.mdl");
            //m_fileNames.Add(baseModelPath + @"characters\cat.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bow_bow.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bow_coral.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bow_hunters.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bow_flaming.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bowCS_ramshead.mdl");
            //m_fileNames.Add(baseModelPath + @"weapons\bowCS_snipers.mdl");
            //m_fileNames.Add(baseModelPath + "animalskull_uv.mdl");
            //m_fileNames.Add(baseModelPath + "alpha_box.mdl");
            //m_fileNames.AddRange(Directory.GetFiles(baseModelPath+@"arenas", "*"));
            //m_fileNames.AddRange(Directory.GetFiles(baseModelPath+@"characters\", "*"));
            //m_fileNames.AddRange(Directory.GetFiles(baseModelPath+@"characters\temp", "*"));
            //m_fileNames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\UnidentifiedModels","*"));
            ChangeModelNext();




            //cameraMatrix = Matrix.Identity;
            //location = new Vector3(0f, 10f, 0f);
            //mouseDelta = new Vector2();

            //System.Windows.Forms.Cursor.Position = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);

            ////Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

        }


        protected override void Update(GameTime gameTime)
        {
            inputState.Update();
            HandleInput(inputState, gameTime);

            facing += mouseSpeed[0];
            pitch += mouseSpeed[1];

            Vector3 lookatPoint = new Vector3((float)Math.Cos(facing), (float)Math.Sin(pitch), (float)Math.Sin(facing));
            modelviewMatrix = Matrix.CreateLookAt(location, location + lookatPoint, up);

            //modelviewMatrix = Matrix.CreateLookAt(new Vector3(0,0,-10), Vector3.Zero, up);
        }

        public void HandleInput(InputState inputState, GameTime gameTime)
        {
            GenerateKeyEvents(ref inputState.LastKeyboardStates[0], ref inputState.CurrentKeyboardStates[0], gameTime);
        }


        void CreateShaders()
        {
            m_basicEffect = new BasicEffect(m_graphicsDeviceManager.GraphicsDevice);
            m_basicEffect.TextureEnabled = true;
            //m_basicEffect.VertexColorEnabled = false;
            m_basicEffect.EnableDefaultLighting();

            //projectionMatrix = Matrix.CreatePerspective(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, 500);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 300.0f);

            m_basicEffect.Projection = projectionMatrix;


            //m_metalEffect = Content.Load<Effect>("Effect1");
            m_metalEffect = Content.Load<Effect>("metal");

        }



        public static Enum[] GetEnumValues(Type enumType)
        {

            if (enumType.BaseType == typeof(Enum))
            {
                FieldInfo[] info = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                Enum[] values = new Enum[info.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = (Enum)info[i].GetValue(null);
                }
                return values;
            }
            else
            {
                throw new Exception("Given type is not an Enum type");
            }
        }

        static Enum[] keysEnumValues = GetEnumValues(typeof(Keys));
        private void GenerateKeyEvents(ref KeyboardState old, ref KeyboardState current, GameTime gameTime)
        {
            foreach (Keys key in keysEnumValues)
            {
                bool released = WasReleased(ref old, ref current, key);
                if (released || IsHeldKey(ref current, key))
                {
                    KeyboardCallback(key, released, ref current, ref old, gameTime);
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        // This is a way of generating 'pressed' events for keys that we want to hold down
        private bool IsHeldKey(ref KeyboardState current, Keys key)
        {
            return (current.IsKeyDown(key) && ((key == Keys.Left || key == Keys.Right || key == Keys.Up ||
                key == Keys.Down || key == Keys.PageUp || key == Keys.PageDown || key == Keys.A ||
                key == Keys.W || key == Keys.S || key == Keys.D || key == Keys.Q || key == Keys.Z)));
        }
        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref KeyboardState old, ref KeyboardState current, Keys key)
        {
            // figure out if the key was released between states.
            return old.IsKeyDown(key) && !current.IsKeyDown(key);
        }


        public virtual void KeyboardCallback(Keys key, bool released, ref KeyboardState newState, ref KeyboardState oldState, GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (key == Keys.O)
                this.ChangeModelPrev();
            if (key == Keys.P)
                this.ChangeModelNext();
            if (key == Keys.K)
                this.ChangeTexturePrev();
            if (key == Keys.L)
                this.ChangeTextureNext();
            if (key == Keys.N)
                this.ChangeSubModelPrev();
            if (key == Keys.M)
                this.ChangeSubModelNext();
            if (key == Keys.F)
                this.rotateX = !this.rotateX;
            if (key == Keys.G)
                this.rotateY = !this.rotateY;
            if (key == Keys.H)
                this.rotateZ = !this.rotateZ;
            if (key == Keys.Escape)
                Exit();

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.W))
            {
                location.X += (float)Math.Cos(facing) * 0.1f;
                location.Z += (float)Math.Sin(facing) * 0.1f;
            }

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.S))
            {
                location.X -= (float)Math.Cos(facing) * 0.1f;
                location.Z -= (float)Math.Sin(facing) * 0.1f;
            }

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.A))
            {
                location.X -= (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
                location.Z -= (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
            }

            if (IsHeldKey(ref  inputState.CurrentKeyboardStates[0], Keys.D))
            {
                location.X += (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
                location.Z += (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
            }



        }

        public void ChangeTextureNext()
        {
            if (m_currentModel.m_model.m_textures.Count > 1)
            {
                m_currentTextureIndex++;
                m_currentTextureIndex %= m_currentModel.m_model.m_textures.Count;
            }
        }

        public void ChangeTexturePrev()
        {
            if (m_currentModel.m_model.m_textures.Count > 1)
            {
                m_currentTextureIndex--;
                if (m_currentTextureIndex < 0)
                {
                    m_currentTextureIndex += m_currentModel.m_model.m_textures.Count;
                }
            }
        }



        void CalcBindFinalMatrix(BoneNode bone, Matrix parentMatrix)
        {
            bone.combinedMatrix = bone.localMatrix * parentMatrix;
            //bone.combinedMatrix = parentMatrix * bone.localMatrix;
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
            }
        }

        public void DrawSkeleton()
        {
            
            BoneNode start = m_currentModel.m_model.m_rootBone;
            Vector3 modelBounds = m_currentModel.m_model.MaxBB - m_currentModel.m_model.MinBB;
            Vector3 skelBounds = m_currentModel.m_model.SkelMaxBB - m_currentModel.m_model.SkelMinBB;


            
            Vector3 scaleVector = new Vector3(skelBounds.X > 0 ? modelBounds.X / skelBounds.X : 1.0f,
                skelBounds.Y > 0 ? modelBounds.Y / skelBounds.Y : 1.0f,
                skelBounds.Z > 0 ? modelBounds.Z / skelBounds.Z : 1.0f);

            float longest = Math.Max(scaleVector.X, Math.Max(scaleVector.Y, scaleVector.Z));
            scaleVector = new Vector3(longest);

            scaleVector = (m_currentModel.m_model.MaxBB - m_currentModel.m_model.MinBB) / (m_currentModel.m_model.SkelMaxBB - m_currentModel.m_model.SkelMinBB);
            Matrix skelScale = Matrix.CreateScale(scaleVector);

            CalcBindFinalMatrix(start, skelScale);

            

            Vector3 mid = new Vector3();
            //if (readDisplayLists)
            {
                mid = (m_currentModel.m_model.MaxBB - m_currentModel.m_model.MinBB) / 2f;
            }

            //float longest = Math.Max(mid.X, Math.Max(mid.Y, mid.Z));


            float scale = longest * 0.05f;

            Matrix modelScaleMatrix = Matrix.CreateScale(scale);

            foreach (BoneNode node in m_currentModel.m_model.m_bones)
            {

                Matrix m = Matrix.Identity;
                m.Translation = node.finalMatrix.Translation;
                DrawModel(m_unitSphere, modelScaleMatrix, m);

                if (node.parent != null)
                {


                    Vector3 diff = node.parent.finalMatrix.Translation - node.finalMatrix.Translation;
                    Vector3 newForward = Vector3.Normalize(diff);
                    // calc the rotation so the avatar faces the target
                    Quaternion q = GetRotation(Vector3.Forward, newForward, Vector3.Up);
                    //Quaternion q = GetRotation(node.parent.finalMatrix.Translation, node.finalMatrix.Translation,Vector3.Up);
                    
                    Matrix rot = Matrix.CreateFromQuaternion(q);
                    float len = diff.Length();
                    Vector3 v = new Vector3(len * 0.1f, len * 0.1f,len);
                    Matrix boneScale = Matrix.CreateScale(v);
                    rot = boneScale * rot;

                    rot.Translation = node.parent.finalMatrix.Translation - (diff/2f);


                    DrawModel(m_unitCone, Matrix.Identity, rot);

                }

            }


        }

        public static Quaternion GetRotation(Vector3 source, Vector3 dest, Vector3 up)
        {
            float dot = Vector3.Dot(source, dest);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                // vector a and b point exactly in the opposite direction, 
                // so it is a 180 degrees turn around the up-axis
                return new Quaternion(up, MathHelper.ToRadians(180.0f));
            }
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                // vector a and b point exactly in the same direction
                // so we return the identity quaternion
                return Quaternion.Identity;
            }

            float rotAngle = (float)Math.Acos(dot);
            Vector3 rotAxis = Vector3.Cross(source, dest);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }

        public void DrawModel(Model model,Matrix modelScaleMatrix,Matrix objectMatrix)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = modelviewMatrix;
                    effect.Projection = projectionMatrix;
                    //effect.World = node.finalMatrix * rotationMatrix * scaleMatrix;
                    //effect.World = scaleMatrix * rotationMatrix * node.finalMatrix;
                    //effect.World = modelScaleMatrix * objectMatrix * rotationMatrix;
                    effect.World = modelScaleMatrix * objectMatrix * rotationMatrix;

                }
                mesh.Draw();
            }

        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_basicEffect.Projection = projectionMatrix;

            if (m_currentModel != null)
            {
                //this.Title = "Name: " + m_currentModel.m_model.m_name + "  Loc: " + string.Format("{0:F}.{1:F}.{2:f}", location.X, location.Y, location.Z);

                //GL.Enable(EnableCap.Texture2D);
                //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //GL.PointSize(10f);

                angle += rotation_speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                rotationMatrix = Matrix.Identity;
                if (rotateX)
                {
                    rotationMatrix *= Matrix.CreateRotationX(angle);
                }
                if (rotateY)
                {
                    rotationMatrix *= Matrix.CreateRotationY(angle);
                }
                if (rotateZ)
                {
                    rotationMatrix *= Matrix.CreateRotationZ(angle);
                }

                //rotationMatrix *= modelviewMatrix;
                Matrix final = modelviewMatrix * rotationMatrix;

                //GL.Begin(PrimitiveType.Points);

                //m_textPrinter.Begin();

                //m_textPrinter.Print(BuildDebugString(), font, Color.White);
                //m_textPrinter.End();

                if (m_modelReader is XboxModelReader)
                {
                    m_noCullState.CullMode = CullMode.None;
                    GraphicsDevice.RasterizerState = m_noCullState;
                }


                DrawModel();

                bool drawSkeleton = false;

                if (m_currentModel.m_model.m_skinned && drawSkeleton)
                {
                    DrawSkeleton();
                }

                spriteBatch.Begin();
                spriteBatch.DrawString(viewerFont, BuildDebugString(), Vector2.Zero, Color.White);
                spriteBatch.End();

                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            else
            {
                //this.Title = "Null Model";
            }


            base.Draw(gameTime);

        }


        public void DrawModel()
        {
            int counter = -1;
            foreach (WrappedDisplayListHeader header in m_currentModel.m_wrappedHeaderList)
            {
                counter++;
                if (displayAll == false && counter != m_currentModelSubIndex)
                {
                    continue;
                }


                if (m_currentModel.m_model.m_skinned)
                {
                    if (header.m_modelSubMesh.MeshId != 0)
                    {
                        //continue;
                    }

                    // include levels 1 and 2
                    if ((header.m_modelSubMesh.LodLevel & 0x01) == 0)
                    {
                        //continue;
                    }
                }



                ShaderData sd = m_currentModel.m_model.m_shaderData[header.m_modelSubMesh.MeshId];


                //SetTexture(sd.textureId1);

                //header.Render();

                GraphicsDevice.SetVertexBuffer(header.m_vertexBuffer);
                GraphicsDevice.Indices = header.m_indexBuffer;
                //m_basicEffect.Texture = m_missingTexture;

                Effect effect;
                SetShaderData(sd, out effect);

                if (effect != null)
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        //m_graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_header.entries.Count, 0, m_header.entries.Count / 3);
                        //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
                        //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, header.m_modelSubMesh.NumIndices, 0, header.m_modelSubMesh.NumIndices / 3);
                        //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, header.m_modelSubMesh.NumIndices, 0, header.m_modelSubMesh.NumIndices-2);
                        if (header.m_modelSubMesh is DisplayListHeader)
                        {
                            int start = 0;
                            int range = (header.m_modelSubMesh.NumIndices / 3) - start;
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, header.m_modelSubMesh.NumIndices, start, range);
                        }
                        if (header.m_modelSubMesh is XBoxSubMesh)
                        {
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, header.m_modelSubMesh.NumIndices, 0, header.m_modelSubMesh.NumIndices-2);
                        }

                        
                    }
                }



            }
        }

        public void SetShaderData(ShaderData sd, out Effect effect)
        {
            Texture2D tex1 = SetTexture(sd);
            //Texture2D tex1 = SetTexture(1);
            //Texture2D tex2 = SetTexture(sd.textureId2);


            if (false && sd.shaderName == "metal")
            {
                m_metalEffect.Parameters["Texture1"].SetValue(tex1);
                //m_metalEffect.Parameters["Texture2"].SetValue(tex2);
                m_metalEffect.Parameters["World"].SetValue(rotationMatrix);
                m_metalEffect.Parameters["View"].SetValue(modelviewMatrix);
                m_metalEffect.Parameters["Projection"].SetValue(projectionMatrix);
                
                //ApplyLightToEffect(m_metalEffect);
                effect = m_metalEffect;
            }
            else
            {
                m_basicEffect.Texture = tex1;
                m_basicEffect.World = rotationMatrix;
                m_basicEffect.View = modelviewMatrix;


                effect = m_basicEffect;
            }
        }


        public void ChangeModelNext()
        {
            if (m_fileNames.Count > 0)
            {
                int attempts = 0;
                while (attempts < m_fileNames.Count)
                {
                    attempts++;

                    m_currentModelIndex++;
                    m_currentModelIndex %= m_fileNames.Count;
                    if (ChangeModel())
                    {
                        m_currentModel = m_modelMap[m_fileNames[m_currentModelIndex]];
                        break;
                    }
                }
            }
        }

        public void ChangeModelPrev()
        {
            if (m_fileNames.Count > 0)
            {
                int attempts = 0;
                while (attempts < m_fileNames.Count)
                {
                    attempts++;

                    m_currentModelIndex--;
                    if (m_currentModelIndex < 0)
                            {
                        m_currentModelIndex += m_fileNames.Count;
                    }
                    if (ChangeModel())
                    {
                        m_currentModel = m_modelMap[m_fileNames[m_currentModelIndex]];
                        break;
                    }
                }


            }

        }


        public void ChangeSubModelNext()
        {
            if (!displayAll)
            {
                if (m_currentModel.m_model.m_modelMeshes.Count > 1)
                {
                    m_currentModelSubIndex++;
                    m_currentModelSubIndex %= m_currentModel.m_model.m_modelMeshes.Count;
                    ChangeSubModel();
                }
            }
        }

        public void ChangeSubModelPrev()
        {
            if (!displayAll)
            {
                if (m_currentModel.m_model.m_modelMeshes.Count > 1)
                {
                    m_currentModelSubIndex--;
                    if (m_currentModelSubIndex < 0)
                    {
                        m_currentModelSubIndex += m_currentModel.m_model.m_modelMeshes.Count;
                        ChangeSubModel();
                    }
                }
            }

        }

        public void ChangeSubModel()
        {
            ModelSubMesh header = m_currentModel.m_model.m_modelMeshes[m_currentModelSubIndex];
            Lookat(header.MinBB, header.MaxBB);
        }



        public bool ChangeModel()
        {
            bool valid;
            BaseModel model = null;
            WrappedModel wrappedModel = null;
            if (!m_modelMap.ContainsKey(m_fileNames[m_currentModelIndex]))
            {
                try
                {
                    model = m_modelReader.LoadSingleModel(m_fileNames[m_currentModelIndex]);
                }
                catch (System.Exception ex)
                {

                }
                if (model != null)
                {
                    wrappedModel = new WrappedModel(model, m_graphicsDeviceManager.GraphicsDevice, m_basicEffect);
                    m_modelMap[m_fileNames[m_currentModelIndex]] = wrappedModel;
                }
            }
            else
            {
                wrappedModel = m_modelMap[m_fileNames[m_currentModelIndex]];
            }

            if (wrappedModel.m_model != null && wrappedModel.m_model.Valid)
            {

                //m_currentModel = m_modelReader.m_models[m_currentModelIndex];
                m_currentTextureIndex = 0;
                m_currentModelSubIndex = 0;

                //GCModel currentModel = m_currentModel;

                Lookat(wrappedModel.m_model.MinBB, wrappedModel.m_model.MaxBB);


                m_currentTextureIndex = wrappedModel.m_model.m_textures.Count - 1;
            }


            return wrappedModel.m_model != null ? wrappedModel.m_model.Valid : false;
        }


        public void Lookat(Vector3 min, Vector3 max)
        {
            Vector3 mid = new Vector3();
            //if (readDisplayLists)
            {
                mid = (max - min) / 2f;
            }

            float longest = Math.Max(mid.X, Math.Max(mid.Y, mid.Z));

            //location = model.Center;

            float val = longest * 3.5f;
            if (val < 1f)
            {
                val = 1f;
            }

            location = min + mid;

            location = location - (Vector3.UnitX * val);

            facing = 0;
            pitch = 0;


        }

        public String BuildDebugString()
        {
            BaseModel currentModel = m_currentModel.m_model;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Model : " + currentModel.m_name);
            sb.AppendLine("Bones : " + currentModel.m_bones.Count);
            sb.AppendLine("MaxVertex : " + currentModel.MaxVertex);

            sb.AppendLine(String.Format("BB : {0:0.00000000} {1:0.00000000} {2:0.00000000}][{3:0.00000000} {4:0.00000000} {5:0.00000000}]", currentModel.MinBB.X, currentModel.MinBB.Y, currentModel.MinBB.Z, currentModel.MaxBB.X, currentModel.MaxBB.Y, currentModel.MaxBB.Z));
            if (currentModel.m_skinned)
            {
                sb.AppendLine(String.Format("SBB : {0:0.00000000} {1:0.00000000} {2:0.00000000}][{3:0.00000000} {4:0.00000000} {5:0.00000000}]", currentModel.SkelMinBB.X, currentModel.SkelMinBB.Y, currentModel.SkelMinBB.Z, currentModel.SkelMaxBB.X, currentModel.SkelMaxBB.Y, currentModel.SkelMaxBB.Z));
            }
            sb.AppendLine(String.Format("DSL [{0}/{1}] Length [{2}] LOD[{3}] Valid[{4}] ", m_currentModelSubIndex, currentModel.m_modelMeshes.Count, currentModel.m_modelMeshes[m_currentModelSubIndex].NumIndices, currentModel.m_modelMeshes[m_currentModelSubIndex].LodLevel, currentModel.m_modelMeshes[m_currentModelSubIndex].Valid));

            sb.AppendLine("Textures : ");
            int counter = 0;
            foreach (TextureData textureData in currentModel.m_textures)
            {
                //if (!m_textureDictionary.ContainsKey(textureData.textureName))
                //{
                //    Texture2D texture;
                //    if (LoadTexture(textureData.textureName, out texture))
                //    {
                //        m_textureDictionary[textureData.textureName] = texture;
                //    }
                //}
                sb.AppendLine(textureData.textureName);
            }
            sb.AppendLine();
            sb.AppendFormat("Loc [{0:0.00000000} {1:0.00000000} {2:0.00000000}]", location.X, location.Y, location.Z);
            return sb.ToString();

        }

        public Texture2D GetTexture(Color color)
        {
            Texture2D newTexture = new Texture2D(m_graphicsDeviceManager.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = color;
            newTexture.SetData(colorData);
            return newTexture;
        }


        public bool LoadTexture(string filename, out Texture2D texture)
            {
                //if(filename.EndsWith(".tga"))
                //{
                //    filename = filename.Substring(0, filename.Length - 4);
                //}
            
                String textureFileName = textureBasePath + filename + ".jpg";
                FileInfo fileInfo = new FileInfo(textureFileName);
                if (fileInfo.Exists)
                {
                    Bitmap  i = Bitmap.FromFile(fileInfo.FullName, true) as Bitmap;
                    texture = BitmapToTexture2D(GraphicsDevice, i);
                    return true;
                }
                texture = m_missingTexture;
                return true;
            }

        public static Texture2D BitmapToTexture2D(GraphicsDevice GraphicsDevice, System.Drawing.Bitmap image)
        {
            // Buffer size is size of color array multiplied by 4 because   
            // each pixel has four color bytes  
            int bufferSize = image.Height * image.Width * 4;

            // Create new memory stream and save image to stream so   
            // we don't have to save and read file  
            System.IO.MemoryStream memoryStream =
                new System.IO.MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Creates a texture from IO.Stream - our memory stream  
            Texture2D texture = Texture2D.FromStream(GraphicsDevice, memoryStream);

            return texture;
        }


        public Texture2D SetTexture(ShaderData sd)
        {
            int index = sd.textureId1;
            if (index == 255)
            {
                index = 0;
            }
            String textureName = m_currentModel.m_model.m_textures[index].textureName;
            if (textureName.Contains("skygold"))
            {
                index = sd.textureId2;
            }

            if (index == 255)
            {
                index = 0;
            }

            Texture2D texture = null;
            if (m_currentModel.m_model.m_textures.Count > 0 && index < m_currentModel.m_model.m_textures.Count)
            {
                textureName = m_currentModel.m_model.m_textures[index].textureName;
                if (!m_textureDictionary.ContainsKey(textureName))
                {
                    LoadTexture(textureName, out texture);
                    m_textureDictionary[textureName] = texture;
                }

                texture = m_textureDictionary[textureName];

                
            }
            return texture;
        }

        public void ApplyLightToEffect(Effect effect)
        {
            Vector3 ambientLightColor = new Vector3(1f);
            float ambientLightIntensity = 0.1f;
            Vector3 directionalLightColor = new Vector3(1f);
            float directionalLightIntensity = 1f;

            Vector3 specularLightColor = new Vector3(1f);
            float specularLightIntensity = 1f;

            //effect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            //effect.Parameters["AmbientLightIntensity"].SetValue(ambientLightIntensity);

            //effect.Parameters["DirectionalLightColor"].SetValue(directionalLightColor);
            //effect.Parameters["DirectionalLightIntensity"].SetValue(directionalLightIntensity);

            //effect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            //effect.Parameters["SpecularLightIntensity"].SetValue(specularLightIntensity);

            //effect.Parameters["LightPosition"].SetValue(location);
            //effect.Parameters["LightDirection"].SetValue(Globals.Player.Forward);
            Vector3 lightDir = new Vector3(0, 1, -1);
            lightDir.Normalize();
            //effect.Parameters["LightDirection"].SetValue(lightDir);
            //effect.Parameters["LightPosition"].SetValue(new Vector3(0, 40, 0));
            //Vector3 lightDirection = new Vector3(10, -10, 0);
            //lightDirection.Normalize();

            //effect.Parameters["LightDirection"].SetValue(lightDirection);

        }



        bool rotateX = false;
        bool rotateY = false;
        bool rotateZ = false;
        bool displayAll = true;

        int m_currentModelIndex = 0;
        int m_currentTextureIndex = 0;
        int m_currentModelSubIndex = 0;
        WrappedModel m_currentModel;

        
        BaseModelReader m_modelReader;

        //OpenTK.Graphics.TextPrinter m_textPrinter;
        String m_debugLine;

        //private readonly Font font = new Font("Verdana", 10);

        public Dictionary<String, Texture2D> m_textureDictionary = new Dictionary<String, Texture2D>();


        List<Vector3> m_points = new List<Vector3>();
    }


    public class WrappedModel
    {
        public BaseModel m_model;
        public List<WrappedDisplayListHeader> m_wrappedHeaderList = new List<WrappedDisplayListHeader>();

        public WrappedModel(BaseModel model, GraphicsDevice graphicsDevice, Effect effect)
        {
            m_model = model;
            int counter = 0;
            foreach (ModelSubMesh modelMesh in model.m_modelMeshes)
            {
                counter++;
                if (counter < 2)
                {
                    //continue;
                }
                WrappedDisplayListHeader wrappedHeader = new WrappedDisplayListHeader(modelMesh, graphicsDevice, effect);
                m_wrappedHeaderList.Add(wrappedHeader);
            }
        }

        public void Draw()
        {

        }

    }


    public class WrappedDisplayListHeader 
    {
        public ModelSubMesh m_modelSubMesh;
        public VertexBuffer m_vertexBuffer;
        public IndexBuffer m_indexBuffer;
        public GraphicsDevice m_graphicsDevice;
        public Effect m_effect;

        //public WrappedDisplayListHeader(XboxModel model, GraphicsDevice graphicsDevice, Effect effect)
        //{
        //    VertexPositionNormalTexture[] vertexData = model.m_points.ToArray();
        //    m_vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
        //    m_vertexBuffer.SetData(vertexData);

        //    ushort[] indices = model.m_indices.ToArray();
        //    m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count(), BufferUsage.None);
        //    m_indexBuffer.SetData(indices);

        //}

        public WrappedDisplayListHeader(ModelSubMesh modelSubMesh, GraphicsDevice graphicsDevice, Effect effect)
        {
            m_modelSubMesh = modelSubMesh;
            m_graphicsDevice = graphicsDevice;
            m_effect = effect;

            if (modelSubMesh is DisplayListHeader)
            {
                DisplayListHeader displayListHeader = modelSubMesh as DisplayListHeader;

                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[displayListHeader.entries.Count];
                int maxEntryVertex = 0;
                for (int i = 0; i < displayListHeader.entries.Count; ++i)
                {
                    maxEntryVertex = Math.Max(maxEntryVertex, displayListHeader.entries[i].PosIndex);
                }

                int ibreak = 0;

                // dslc - 2 in first entry suggests that display list uses first block twice?

                for (int i = 0; i < displayListHeader.entries.Count; ++i)
                {

                    vertexData[i].Position = displayListHeader.Vertices[displayListHeader.entries[i].PosIndex];
                    Vector3 v = Vector3.Up;
                    if (displayListHeader.entries[i].NormIndex < displayListHeader.Normals.Count)
                    {
                        v = displayListHeader.Normals[displayListHeader.entries[i].NormIndex];
                    }
                    vertexData[i].Normal = v;
                    Vector2 uvtemp = displayListHeader.UVs[displayListHeader.entries[i].UVIndex];
                    uvtemp.X -= (int)uvtemp.X;
                    uvtemp.Y -= (int)uvtemp.Y;
                    vertexData[i].TextureCoordinate = uvtemp;
                }

                m_vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
                m_vertexBuffer.SetData(vertexData);


                ushort[] indexData = new ushort[displayListHeader.entries.Count];
                for (int i = 0; i < indexData.Length; i += 3)
                {
                    indexData[i + 0] = (ushort)(i + 0);
                    indexData[i + 1] = (ushort)(i + 2);
                    indexData[i + 2] = (ushort)(i + 1);
                }
                m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexData.Count(), BufferUsage.None);
                m_indexBuffer.SetData(indexData);
            }
            else if (modelSubMesh is XBoxSubMesh)
            {
                XBoxSubMesh doegSection = modelSubMesh as XBoxSubMesh;
                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[modelSubMesh.Vertices.Count];
                for (int i = 0; i < vertexData.Length; ++i)
                {
                    vertexData[i].Position = modelSubMesh.Vertices[i];
                    vertexData[i].TextureCoordinate= modelSubMesh.UVs[i];
                    vertexData[i].Normal = modelSubMesh.Normals[i];
                }
                m_vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
                m_vertexBuffer.SetData(vertexData);

                ushort[] indexData = modelSubMesh.Indices.ToArray();

                m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexData.Length, BufferUsage.None);
                m_indexBuffer.SetData(indexData);

            }
      }


    }
}
