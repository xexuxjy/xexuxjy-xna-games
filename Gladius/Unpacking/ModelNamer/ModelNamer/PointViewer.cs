
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.Xna.Framework;

namespace ModelNamer
{
    public class PointViewer : Game
    {
        static int size = 5;
        Vector3 eyePos = new Vector3(size * 5, size * 5, 0);
        Vector3 eyeLookat = new Vector3(0, 0, 0);
        const float rotation_speed = 60.0f;
        float angle;

        private Matrix cameraMatrix;
        private float[] mouseSpeed = new float[2];
        private Vector2 mouseDelta;
        private Vector3 location;
        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;
        public String textureBasePath = @"D:\gladius-extracted-archive\gc-compressed\textures\";
        public int m_missingTextureHandle;
        public List<String> m_fileNames = new List<string>();
        public Dictionary<String, WrappedModel> m_modelMap = new Dictionary<string, WrappedModel>();

        string vertexShaderSource = @"
#version 140

precision highp float;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv;

//out vec3 pos;
//out vec3 normal;
//out vec2 uv;

void main(void)
{
  //works only for orthogonal modelview
  //pos = (modelview_matrix * vec4(in_position, 0)).xyz;
  //normal = in_normal;
  //uv = in_uv;
  
  gl_Position = projection_matrix * modelview_matrix * vec4(in_position, 1);
  gl_TexCoord[0] = in_uv;
}";

        string fragmentShaderSource = @"
#version 140

precision highp float;

//uniform sampler2D MyTexture0;
//uniform sampler2D MyTexture1;

const vec3 ambient = vec3(0.1, 0.1, 0.1);
const vec3 lightVecNormalized = normalize(vec3(0.5, 0.5, 2.0));
const vec3 lightColor = vec3(0.9, 0.9, 0.7);

//in vec3 pos;
//in vec3 normal;
//in vec2 uv;

out vec4 out_frag_color;

void main(void)
{
//  float diffuse = clamp(dot(lightVecNormalized, normalize(normal)), 0.0, 1.0);
//  out_frag_color = vec4(ambient + diffuse * lightColor, 1.0);
    //out_frag_color = texture2D(MyTexture0,gl_TexCoord[0].st);
    out_frag_color = vec4(1.0,1.0,1.0,1.0);
}";

        int vertexShaderHandle;
        int fragmentShaderHandle;
        int shaderProgramHandle;
        int modelviewMatrixLocation;
        int projectionMatrixLocation;

        Matrix projectionMatrix;
        Matrix modelviewMatrix;

        //modelviewMatrixLocation,
        //    projectionMatrixLocation,
        //    vaoHandle,
        //    positionVboHandle,
        //    normalVboHandle,
        //    eboHandle;



        void CreateShaders()
        {
            //vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            //fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            //GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            //GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            //GL.CompileShader(vertexShaderHandle);
            //GL.CompileShader(fragmentShaderHandle);

            //Debug.WriteLine(GL.GetShaderInfoLog(vertexShaderHandle));
            //Debug.WriteLine(GL.GetShaderInfoLog(fragmentShaderHandle));

            //// Create program
            //shaderProgramHandle = GL.CreateProgram();

            //GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            //GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            //GL.BindAttribLocation(shaderProgramHandle, 0, "in_position");
            //GL.BindAttribLocation(shaderProgramHandle, 1, "in_normal");

            //GL.LinkProgram(shaderProgramHandle);
            //Debug.WriteLine(GL.GetProgramInfoLog(shaderProgramHandle));
            //GL.UseProgram(shaderProgramHandle);

            //// Set uniforms
            //projectionMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");
            //modelviewMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "modelview_matrix");

            //float aspectRatio = ClientSize.Width / (float)(ClientSize.Height);
            //Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspectRatio, 1, 100, out projectionMatrix);
            ////modelviewMatrix = Matrix.LookAt(new Vector3(0, 3, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            //GL.UniformMatrix(projectionMatrixLocation, false, ref projectionMatrix);
        }



        public PointViewer()
        //: base(800, 600)
        {
            //this.VSync = VSyncMode.Off;
            //m_modelReader = new GCModelReader();
            ////m_fileNames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed", "*"));
            ////m_fileNames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\characters", "*"));
            ////m_fileNames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\yeti.mdl");
            //m_fileNames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\characters\bear.mdl");
            //ChangeModelNext();


            //Keyboard.KeyDown += delegate(object sender, KeyboardKeyEventArgs e)
            //{
            //    if (e.Key == Key.O)
            //        this.ChangeModelPrev();
            //    if (e.Key == Key.P)
            //        this.ChangeModelNext();
            //    if (e.Key == Key.K)
            //        this.ChangeTexturePrev();
            //    if (e.Key == Key.L)
            //        this.ChangeTextureNext();
            //    if (e.Key == Key.N)
            //        this.ChangeSubModelPrev();
            //    if (e.Key == Key.M)
            //        this.ChangeSubModelNext();
            //    if (e.Key == Key.F)
            //        this.rotateX = !this.rotateX;
            //    if (e.Key == Key.G)
            //        this.rotateY = !this.rotateY;
            //    if (e.Key == Key.H)
            //        this.rotateZ = !this.rotateZ;
            //    if (e.Key == Key.Z)
            //        this.displayAll = !this.displayAll;


            //};

            //Keyboard.KeyUp += delegate(object sender, KeyboardKeyEventArgs e)
            //{
            //    if (e.Key == Key.F11)
            //        if (this.WindowState == WindowState.Fullscreen)
            //            this.WindowState = WindowState.Normal;
            //        else
            //            this.WindowState = WindowState.Fullscreen;
            //};



            //cameraMatrix = Matrix.Identity;
            //location = new Vector3(0f, 10f, 0f);
            //mouseDelta = new Vector2();

            //System.Windows.Forms.Cursor.Position = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);

            ////Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

            //CreateShaders();
            //CreateEmptyTexture();
            //m_textPrinter = new OpenTK.Graphics.TextPrinter();
        }

        //void OnMouseMove(object sender, MouseMoveEventArgs e)
        //{
        //    mouseDelta = new Vector2(e.XDelta, e.YDelta);
        //}


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
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
            }
        }

        public void DrawSkeleton()
        {
            //GCModel model = m_currentModel;
            //GL.Begin(PrimitiveType.Points);

            //BoneNode start = model.m_rootBone;
            //CalcBindFinalMatrix(start, Matrix.Identity);

            //foreach (BoneNode node in model.m_bones)
            //{
            //    GL.Vertex3(node.finalMatrix.ExtractTranslation());
            //}

            //GL.End();
        }


        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    GL.ClearColor(System.Drawing.Color.MidnightBlue);
        //    GL.Enable(EnableCap.DepthTest);
        //    //GL.Enable(EnableCap.Lighting);

        //}

        //protected override void OnUnload(EventArgs e)
        //{
        //    base.OnUnload(e);
        //}

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Contains information on the new Width and Size of the GameWindow.</param>
        //protected override void OnResize(EventArgs e)
        //{
        //    GL.Viewport(0, 0, Width, Height);

        //    GL.MatrixMode(MatrixMode.Projection);
        //    Matrix p = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 300.0f);
        //    GL.LoadMatrix(ref p);

        //    //GL.MatrixMode(MatrixMode.Modelview);
        //    //Matrix mv = Matrix.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
        //    //GL.LoadMatrix(ref mv);
        //}


        //protected override void OnUpdateFrame(FrameEventArgs e)
        //{
        //    //if(Keyboard[Key.F1])
        //    //{
        //    //    switchToMode(ViewMode.CubemapCross);
        //    //}

        //    //if (Keyboard[Key.F2])
        //    //{
        //    //    switchToMode(ViewMode.Scene);
        //    //}
        //    base.OnUpdateFrame(e);

        //    if (Keyboard[Key.F5])
        //    {
        //        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
        //    }
        //    if (Keyboard[Key.F6])
        //    {
        //        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        //    }
        //    if (Keyboard[Key.F7])
        //    {

        //        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        //    }
        //    if (Keyboard[Key.Escape])
        //    {
        //        this.Exit();
        //    }
        //    if (Keyboard[Key.W])
        //    {
        //        location.X += (float)Math.Cos(facing) * 0.1f;
        //        location.Z += (float)Math.Sin(facing) * 0.1f;
        //    }

        //    if (Keyboard[Key.S])
        //    {
        //        location.X -= (float)Math.Cos(facing) * 0.1f;
        //        location.Z -= (float)Math.Sin(facing) * 0.1f;
        //    }

        //    if (Keyboard[Key.A])
        //    {
        //        location.X -= (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
        //        location.Z -= (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
        //    }

        //    if (Keyboard[Key.D])
        //    {
        //        location.X += (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
        //        location.Z += (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
        //    }

        //    mouseSpeed[0] *= 0.9f;
        //    mouseSpeed[1] *= 0.9f;
        //    mouseSpeed[0] += mouseDelta.X / 1000f;
        //    mouseSpeed[1] += mouseDelta.Y / 1000f;
        //    mouseDelta = new Vector2();

        //    facing += mouseSpeed[0];
        //    pitch += mouseSpeed[1];
        //    Vector3 lookatPoint = new Vector3((float)Math.Cos(facing), (float)Math.Sin(pitch), (float)Math.Sin(facing));
        //    modelviewMatrix = Matrix.LookAt(location, location + lookatPoint, up);



        //}

        //void setPerspective()
        //{
        //    OpenTK.Matrix proj;
        //    proj = OpenTK.Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 200f);
        //    GL.LoadMatrix(ref proj);
        //}


        //protected override void OnRenderFrame(FrameEventArgs e)
        //{
        //    base.OnRenderFrame(e);
        //    if (m_currentModel != null)
        //    {
        //        this.Title = "Name: " + m_currentModel.m_model.m_name + "  Loc: " + string.Format("{0:F}.{1:F}.{2:f}", location.X, location.Y, location.Z);

        //        GL.Enable(EnableCap.Texture2D);
        //        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //        GL.PointSize(10f);

        //        angle += rotation_speed * (float)e.Time;

        //        Matrix rotationMatrix = Matrix.Identity;
        //        if (rotateX)
        //        {
        //            rotationMatrix *= Matrix.CreateRotationX(angle);
        //        }
        //        if (rotateY)
        //        {
        //            rotationMatrix *= Matrix.CreateRotationY(angle);
        //        }
        //        if (rotateZ)
        //        {
        //            rotationMatrix *= Matrix.CreateRotationZ(angle);
        //        }

        //        //rotationMatrix *= modelviewMatrix;
        //        Matrix final = modelviewMatrix * rotationMatrix;

        //        GL.UniformMatrix(modelviewMatrixLocation, false, ref final);


        //        //GL.MatrixMode(MatrixMode.Modelview);
        //        //GL.LoadMatrix(ref cameraMatrix);

        //        //angle += rotation_speed * (float)e.Time;
        //        //if (rotateX || rotateY || rotateZ)
        //        //{
        //        //    GL.Rotate(angle, rotateX ? 1.0f : 0.0f, rotateY ? 1.0f : 0.0f, rotateZ ? 1.0f : 0.0f);
        //        //}


        //        //GL.Begin(PrimitiveType.Points);

        //        //m_textPrinter.Begin();

        //        //m_textPrinter.Print(BuildDebugString(), font, Color.White);
        //        //m_textPrinter.End();
        //        bool drawSkeleton = false;


        //        if (drawSkeleton)
        //        {
        //            DrawSkeleton();
        //        }
        //        else
        //        {
        //            DrawModel();
        //        }
        //    }
        //    else
        //    {
        //        this.Title = "Null Model";
        //    }

        //    SwapBuffers();
        //}


        [STAThread]
        public static void Main()
        {
            PointViewer pointViewer = new PointViewer();
            pointViewer.Run();
            //using (PointViewer example = new PointViewer())
            //{

            //    //OpenTK.Platform.Utilities.SetWindowTitle(example);
            //    //example.PopulateList(modelReader.m_models[3].m_points);
            //    //example.Run(30, 30);
            //}
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
                    if (header.m_header.MeshId != 4)
                    {
                        //continue;
                    }

                    if (header.m_header.LodLevel != 0x01)
                    {
                        continue;
                    }
                }



                ShaderData sd = m_currentModel.m_model.m_shaderData[header.m_header.MeshId];

                //SetTexture(sd.textureId1,TextureUnit.Texture0,"MyTexture0");

                header.Render();
                //GL.BindVertexArray(header.m_vaoHandle);
                //GL.DrawElements(PrimitiveType.Triangles, header.m_header.entries.Count,
                //    DrawElementsType.UnsignedInt, IntPtr.Zero);

                // unbind your Vertex Array Object
                //GL.BindVertexArray(0);

                //// unbind your index buffer
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


                //GL.Begin(PrimitiveType.Triangles);

                //int entryCount = header.entries.Count;

                //for (int i = 0; i < entryCount; ++i)
                //{
                //    if (header.adjustedSizeInt > 5)
                //    {
                //        GL.TexCoord2(m_currentModel.m_uvs[header.entries[i].UVIndex]);
                //    }

                //    if (m_currentModel.m_skinned)
                //    {
                //        GL.Vertex3(header.skinBlock.m_points[header.entries[i].PosIndex]);
                //        if (header.adjustedSizeInt > 6 && header.entries[i].NormIndex < header.skinBlock.m_normals.Count)
                //        {
                //            GL.Normal3(header.skinBlock.m_normals[header.entries[i].NormIndex]);
                //        }
                //    }
                //    else
                //    {
                //        GL.Vertex3(m_currentModel.m_points[header.entries[i].PosIndex]);
                //        if (header.adjustedSizeInt > 6 && header.entries[i].NormIndex < header.skinBlock.m_normals.Count)
                //        {
                //            GL.Normal3(m_currentModel.m_normals[header.entries[i].NormIndex]);
                //        }
                //    }
                //}
                //GL.End();
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
                    wrappedModel = new WrappedModel(model);
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
            sb.AppendLine("MaxVertex : " + currentModel.m_maxVertex);
            sb.AppendLine(String.Format("BB : {0:0.00000000} {1:0.00000000} {2:0.00000000}][{3:0.00000000} {4:0.00000000} {5:0.00000000}]", currentModel.MinBB.X, currentModel.MinBB.Y, currentModel.MinBB.Z, currentModel.MaxBB.X, currentModel.MaxBB.Y, currentModel.MaxBB.Z));

            if (currentModel is GCModel)
            {
                GCModel gcModel = currentModel as GCModel;
                sb.AppendLine(String.Format("DSL [{0}/{1}] Length [{2}] Valid[{3}] ", m_currentModelSubIndex, gcModel.m_modelMeshes.Count, gcModel.m_modelMeshes[m_currentModelSubIndex].NumIndices, gcModel.m_modelMeshes[m_currentModelSubIndex].Valid));
            }
            sb.AppendLine("Textures : ");
            int counter = 0;
            foreach (TextureData textureData in currentModel.m_textures)
            {
                if (!m_textureDictionary.ContainsKey(textureData.textureName))
                {
                    int key;
                    if (LoadTexture(textureData.textureName, out key))
                    {
                        m_textureDictionary[textureData.textureName] = key;
                    }
                }
                sb.AppendLine(textureData.textureName);
            }
            sb.AppendLine();
            sb.AppendFormat("Loc [{0:0.00000000} {1:0.00000000} {2:0.00000000}]", location.X, location.Y, location.Z);
            return sb.ToString();

        }

        public void CreateEmptyTexture()
        {
            //int id = GL.GenTexture();
            //GL.BindTexture(TextureTarget.Texture2D, id);
            //Bitmap bmp = new Bitmap(4, 4);

            //BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);


            //// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            //// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            //// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //m_missingTextureHandle = id;

        }

        public bool LoadTexture(string filename, out int textureHandle)
        {
            //String textureFileName = textureBasePath + filename + ".png";

            //try
            //{
            //    FileInfo fileInfo = new FileInfo(textureFileName);
            //    if (false && fileInfo.Exists)
            //    {
            //        Bitmap bmp = new Bitmap(textureFileName);

            //        int id = GL.GenTexture();
            //        GL.BindTexture(TextureTarget.Texture2D, id);

            //        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            //            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            //        bmp.UnlockBits(bmp_data);

            //        // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            //        // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            //        // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            //        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //        textureHandle = id;
            //        return true;
            //    }
            //    else
            //    {
            //        textureHandle = m_missingTextureHandle;
            //        return true;
            //    }

            //}
            //catch (FileNotFoundException fnfe)
            //{
            //}
            textureHandle = -1;
            return false;
        }


        //public void SetTexture(int index, TextureUnit textureUnit, string UniformName)
        //{

        //    bool foundTexture = false;

        //    if (m_currentModel.m_model.m_textures.Count > 0 && index < m_currentModel.m_model.m_textures.Count)
        //    {
        //        String textureName = m_currentModel.m_model.m_textures[index];
        //        int handle;
        //        if (!m_textureDictionary.ContainsKey(textureName))
        //        {
        //            LoadTexture(textureName, out handle);
        //            m_textureDictionary[textureName] = handle;
        //        }

        //        handle = m_textureDictionary[textureName];

        //        GL.ActiveTexture(textureUnit);
        //        GL.BindTexture(TextureTarget.Texture2D, handle);
        //        GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, UniformName), textureUnit - TextureUnit.Texture0);

        //        foundTexture = true;
        //    }

        //}


        bool rotateX = false;
        bool rotateY = false;
        bool rotateZ = false;
        bool displayAll = true;

        int m_currentModelIndex = 0;
        int m_currentTextureIndex = 0;
        int m_currentModelSubIndex = 0;
        WrappedModel m_currentModel;

        GCModelReader m_modelReader;
        //OpenTK.Graphics.TextPrinter m_textPrinter;
        String m_debugLine;

        private readonly Font font = new Font("Verdana", 10);

        public Dictionary<String, int> m_textureDictionary = new Dictionary<String, int>();


        List<Vector3> m_points = new List<Vector3>();
    }


    public class WrappedModel : IDisposable
    {
        public BaseModel m_model;
        public List<WrappedDisplayListHeader> m_wrappedHeaderList = new List<WrappedDisplayListHeader>();

        public WrappedModel(BaseModel model)
        {
            m_model = model;
            foreach (ModelSubMesh header in model.m_modelMeshes)
            {
                WrappedDisplayListHeader wrappedHeader = new WrappedDisplayListHeader(header);
                m_wrappedHeaderList.Add(wrappedHeader);
            }
        }

        public void Dispose()
        {
            foreach (WrappedDisplayListHeader header in m_wrappedHeaderList)
            {
                header.Dispose();
            }
        }
    }


    public class WrappedDisplayListHeader : IDisposable
    {
        public ModelSubMesh m_header;
        //public int m_vbPosHandle;
        //public int m_vbNorHandle;
        //public int m_vbUVHandle;
        //public int m_ibHandle;
        //public int m_vaoHandle;


        public WrappedDisplayListHeader(ModelSubMesh header)
        {
            m_header = header;

            //m_vbPosHandle = GL.GenBuffer();
            //m_vbNorHandle = GL.GenBuffer();
            //m_vbUVHandle = GL.GenBuffer();
            //m_ibHandle = GL.GenBuffer();


            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ibHandle);
            //int[] Indices = new int[m_header.entries.Count];
            //for (int i = 0; i < m_header.entries.Count; ++i)
            //{
            //    Indices[i] = m_header.entries[i].PosIndex;
            //}
            //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(ushort)), Indices, BufferUsageHint.StaticDraw);


            //float[] vfa = new float[m_header.entries.Count * 3];
            //for (int i = 0; i < m_header.entries.Count; ++i)
            //{
            //    Vector3 v = m_header.Points[m_header.entries[i].PosIndex];

            //    vfa[(i * 3) + 0] = v.X;
            //    vfa[(i * 3) + 1] = v.Y;
            //    vfa[(i * 3) + 2] = v.Z;
            //}

            //GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbPosHandle);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vfa.Length * sizeof(float)), vfa, BufferUsageHint.StaticDraw);

            //float[] nfa = new float[m_header.entries.Count * 3];
            //for (int i = 0; i < m_header.entries.Count; ++i)
            //{
            //    Vector3 v = Vector3.Zero;
            //    if (m_header.entries[i].NormIndex < m_header.Normals.Count)
            //    {
            //        v = m_header.Normals[m_header.entries[i].NormIndex];
            //    }
            //    nfa[(i * 3) + 0] = v.X;
            //    nfa[(i * 3) + 1] = v.Y;
            //    nfa[(i * 3) + 2] = v.Z;
            //}
            //GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbNorHandle);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(nfa.Length * sizeof(float)), nfa, BufferUsageHint.StaticDraw);

            //float[] uva = new float[m_header.entries.Count * 2];
            //for (int i = 0; i < m_header.entries.Count; ++i)
            //{
            //    Vector2 v = m_header.UVs[m_header.entries[i].UVIndex];
            //    uva[(i * 2) + 0] = v.X;
            //    uva[(i * 2) + 1] = v.Y;
            //}
            //GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbNorHandle);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uva.Length * sizeof(float)), uva, BufferUsageHint.StaticDraw);


            CreateVAOs();
        }

        void CreateVAOs()
        {
            // GL3 allows us to store the vertex layout in a "vertex array object" (VAO).
            // This means we do not have to re-issue VertexAttribPointer calls
            // every time we try to use a different vertex layout - these calls are
            // stored in the VAO so we simply need to bind the correct VAO.
            //m_vaoHandle = GL.GenVertexArray();
            //GL.BindVertexArray(m_vaoHandle);

            //GL.EnableVertexAttribArray(0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbPosHandle);
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

            //GL.EnableVertexAttribArray(1);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbNorHandle);
            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

            //GL.EnableVertexAttribArray(2);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbUVHandle);
            //GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ibHandle);

            //GL.BindVertexArray(0);
        }


        public void Render()
        {
            // use correct shader
            //currentShader.Use();

            // render
            //GL.BindVertexArray(m_vaoHandle);
            ////GL.DrawElements(BeginMode.Triangles, m_header.entries.Count/3, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.DrawElements(PrimitiveType.Triangles, m_header.entries.Count / 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

        }
        //public void BindToShader(Shader shader)
        //{
        //    GL.BindAttribLocation(shader.program, 0, "in_position");
        //    GL.BindAttribLocation(shader.program, 1, "in_normal");
        //    currentShader = shader;
        //}

        public void Dispose()
        {
            //GL.DeleteBuffers(m_vbos.Length, ref m_vbos);
        }
    }


}

