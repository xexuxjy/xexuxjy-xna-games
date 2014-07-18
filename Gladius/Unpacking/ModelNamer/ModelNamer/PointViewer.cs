﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
//using OpenTK.Platform;

namespace ModelNamer
{
    public class PointViewer : GameWindow
    {
        static int size = 5;
        Vector3 eyePos = new Vector3(size * 5, size * 5, 0);
        Vector3 eyeLookat = new Vector3(0, 0, 0);
        const float rotation_speed = 180.0f;
        float angle;

        private Matrix4 cameraMatrix;
        private float[] mouseSpeed = new float[2];
        private Vector2 mouseDelta;
        private Vector3 location;
        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;
        public String textureBasePath = @"D:\gladius-extracted\test-extract\";


        public PointViewer()
            : base(800, 600)
        {
            this.VSync = VSyncMode.Off;
            m_modelReader = new GCModelReader();
            //m_modelReader.LoadModels(@"D:\gladius-extracted-archive\gc-compressed\probable-models", @"D:\gladius-extracted-archive\gc-compressed\model-results", 100);
            m_modelReader.LoadModels(@"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed", @"D:\gladius-extracted-archive\gc-compressed\model-results");
            //m_modelReader.LoadModels(@"C:\tmp\unpacking\gc-models", @"C:\tmp\unpacking\gc-models\model-results", 100);

            ChangeModelNext();


            Keyboard.KeyDown += delegate(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.O)
                    this.ChangeModelPrev();
                if (e.Key == Key.P)
                    this.ChangeModelNext();
            };

            Keyboard.KeyUp += delegate(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.F11)
                    if (this.WindowState == WindowState.Fullscreen)
                        this.WindowState = WindowState.Normal;
                    else
                        this.WindowState = WindowState.Fullscreen;
            };



            cameraMatrix = Matrix4.Identity;
            location = new Vector3(0f, 10f, 0f);
            mouseDelta = new Vector2();

            System.Windows.Forms.Cursor.Position = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);

            Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

            m_textPrinter = new OpenTK.Graphics.TextPrinter();
        }

        void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            mouseDelta = new Vector2(e.XDelta, e.YDelta);
        }



        public void BuildCube()
        {
            m_points.Add(new Vector3(-size, -size, -size));
            m_points.Add(new Vector3(size, -size, -size));
            m_points.Add(new Vector3(-size, -size, size));
            m_points.Add(new Vector3(size, -size, size));
            m_points.Add(new Vector3(-size, size, -size));
            m_points.Add(new Vector3(size, size, -size));
            m_points.Add(new Vector3(-size, size, size));
            m_points.Add(new Vector3(size, size, size));
        }

        public void PopulateList(List<Vector3> points)
        {
            m_points.Clear();
            foreach (Vector3 sv in points)
            {
                m_points.Add(sv * size);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(System.Drawing.Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Lighting);

        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Contains information on the new Width and Size of the GameWindow.</param>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 50.0f);
            GL.LoadMatrix(ref p);

            //GL.MatrixMode(MatrixMode.Modelview);
            //Matrix4 mv = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
            //GL.LoadMatrix(ref mv);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //if(Keyboard[Key.F1])
            //{
            //    switchToMode(ViewMode.CubemapCross);
            //}

            //if (Keyboard[Key.F2])
            //{
            //    switchToMode(ViewMode.Scene);
            //}
            base.OnUpdateFrame(e);

            if (Keyboard[Key.F5])
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
            }
            if (Keyboard[Key.F6])
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            if (Keyboard[Key.F7])
            {

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            if (Keyboard[Key.Escape])
            {
                this.Exit();
            }
            if (Keyboard[Key.W])
            {
                location.X += (float)Math.Cos(facing) * 0.1f;
                location.Z += (float)Math.Sin(facing) * 0.1f;
            }

            if (Keyboard[Key.S])
            {
                location.X -= (float)Math.Cos(facing) * 0.1f;
                location.Z -= (float)Math.Sin(facing) * 0.1f;
            }

            if (Keyboard[Key.A])
            {
                location.X -= (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
                location.Z -= (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
            }

            if (Keyboard[Key.D])
            {
                location.X += (float)Math.Cos(facing + Math.PI / 2) * 0.1f;
                location.Z += (float)Math.Sin(facing + Math.PI / 2) * 0.1f;
            }

            mouseSpeed[0] *= 0.9f;
            mouseSpeed[1] *= 0.9f;
            mouseSpeed[0] += mouseDelta.X / 1000f;
            mouseSpeed[1] += mouseDelta.Y / 1000f;
            mouseDelta = new Vector2();

            facing += mouseSpeed[0];
            pitch += mouseSpeed[1];
            Vector3 lookatPoint = new Vector3((float)Math.Cos(facing), (float)Math.Sin(pitch), (float)Math.Sin(facing));
            cameraMatrix = Matrix4.LookAt(location, location + lookatPoint, up);



        }

        void setPerspective()
        {
            OpenTK.Matrix4 proj;
            proj = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 200f);
            GL.LoadMatrix(ref proj);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //this.Title = VisibleParticleCount + " Points. FPS: " + string.Format("{0:F}", 1.0 / e.Time);
            GCModel model = m_modelReader.m_models[m_currentModel];

            this.Title = "Name: " + model.m_name + "  Loc: " + string.Format("{0:F}.{1:F}.{2:f}", location.X, location.Y, location.Z);

            GL.Enable(EnableCap.Texture2D);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.PointSize(5f);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref cameraMatrix);


            //Matrix4 lookat = Matrix4.LookAt(eyePos,eyeLookat,Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref lookat);
            //Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            //Matrix4 lookat = Matrix4.LookAt(eyePos, eyeLookat, Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref lookat);

            angle += rotation_speed * (float)e.Time;
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);


            //GL.Begin(PrimitiveType.Points);

            m_textPrinter.Begin();
            m_textPrinter.Print(m_debugLine, font, Color.White);
            m_textPrinter.End();

            if (model.Valid)
            {

                foreach (DisplayListHeader header in model.m_displayListHeaders)
                {
                    if (header.primitiveFlags == 0x90)
                    {
                        int textureIndex = 1;
                        bool foundTexture = false;
                        for (int i = 0; i < model.m_textures.Count;++i)
                        {
                            if (m_textureDictionary.ContainsKey(model.m_textures[i]))
                            {
                                GL.BindTexture(TextureTarget.Texture2D, m_textureDictionary[model.m_textures[i]]);
                                GL.Color3(System.Drawing.Color.White);
                                foundTexture = true;
                                break;
                            }
                        }
                        if(!foundTexture)
                        {
                            GL.Color3(System.Drawing.Color.ForestGreen);
                        }

                        GL.Begin(PrimitiveType.Triangles);
                        
                        for (int i = 0; i < header.entries.Count; ++i)
                        {
                            GL.TexCoord2(model.m_uvs[header.entries[i].UVIndex]);
                            GL.Vertex3(model.m_points[header.entries[i].PosIndex]);
                            GL.Normal3(model.m_normals[header.entries[i].NormIndex]);
                        }
                        GL.End();
                    }
                }
            }

            //GL.Begin(PrimitiveType.Quads);

            //GL.Vertex3(1.0f, -1.0f, -1.0f);
            //GL.Vertex3(1.0f, 1.0f, -1.0f);
            //GL.Vertex3(1.0f, 1.0f, 1.0f);
            //GL.Vertex3(1.0f, -1.0f, 1.0f);


            //for (int i = 0; i < m_points.Count; ++i)
            //{
            //    GL.Vertex3(m_points[i]);
            //}


            SwapBuffers();
        }


        [STAThread]
        public static void Main()
        {
            using (PointViewer example = new PointViewer())
            {
                //OpenTK.Platform.Utilities.SetWindowTitle(example);
                //example.PopulateList(modelReader.m_models[3].m_points);
                example.Run(30, 30);
            }
        }


        public void ChangeModelNext()
        {
            if (m_modelReader.m_models.Count > 0)
            {
                m_currentModel++;
                m_currentModel %= m_modelReader.m_models.Count;
                ChangeModel();

            }
        }

        public void ChangeModel()
        {
            m_points = m_modelReader.m_models[m_currentModel].m_points;
            Vector3 mid = new Vector3(m_modelReader.m_models[m_currentModel].MaxBB = m_modelReader.m_models[m_currentModel].MinBB) / 2f;

            location = m_modelReader.m_models[m_currentModel].Center;

            location = mid - (Vector3.UnitX * 2f);



            facing = 0;
            pitch = 0;

            GCModel currentModel = m_modelReader.m_models[m_currentModel];
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Model : " + currentModel.m_name);
            sb.AppendLine("Textures : ");
            foreach (string textureName in currentModel.m_textures)
            {
                if (!m_textureDictionary.ContainsKey(textureName))
                {
                    int key;
                    if (LoadTexture(textureName, out key))
                    {
                        m_textureDictionary[textureName] = key;
                    }
                }
                sb.AppendLine(textureName);
            }
            m_debugLine = sb.ToString();
        }

        public void ChangeModelPrev()
        {
            if (m_modelReader.m_models.Count > 0)
            {
                m_currentModel--;
                if (m_currentModel < 0)
                {
                    m_currentModel += m_modelReader.m_models.Count;
                }
                ChangeModel();
            }

        }
 
        public bool LoadTexture(string filename,out int textureHandle)
        {
            String textureFileName = textureBasePath + filename + ".png";

            try
            {
                FileInfo fileInfo = new FileInfo(textureFileName);
                if (fileInfo.Exists)
                {
                    Bitmap bmp = new Bitmap(textureFileName);

                    int id = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, id);

                    BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                    bmp.UnlockBits(bmp_data);

                    // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
                    // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                    // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    textureHandle = id;
                    return true;
                }
            }
            catch (FileNotFoundException fnfe)
            {
            }
            textureHandle = -1;
            return false;
        }


        int m_currentModel = 0;
        GCModelReader m_modelReader;
        OpenTK.Graphics.TextPrinter m_textPrinter;
        String m_debugLine;

        private readonly Font font = new Font("Verdana", 10);

        public Dictionary<String, int> m_textureDictionary = new Dictionary<String, int>();


        List<Vector3> m_points = new List<Vector3>();
    }


    



}

