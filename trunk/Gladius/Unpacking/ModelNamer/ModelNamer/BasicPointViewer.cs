using System;
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
    public class BasicPointViewer : GameWindow
    {
        static int size = 2;
        Vector3 eyePos = new Vector3(size * 5, size * 5, 0);
        Vector3 eyeLookat = new Vector3(0, 0, 0);
        const float rotation_speed = 60.0f;
        float angle;

        private Matrix4 cameraMatrix;
        private float[] mouseSpeed = new float[2];
        private Vector2 mouseDelta;
        private Vector2 lastMouse;

        private Vector3 location;
        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;
        public String textureBasePath = @"d:\gladius-extracted\test-extract\";

        public List<String> m_fileNames = new List<string>();
        public Dictionary<String, GCModel> m_modelMap = new Dictionary<string, GCModel>();

        public bool readDisplayLists = true;


        public BasicPointViewer()
            : base(800, 600)
        {
            this.VSync = VSyncMode.Off;
     
            Keyboard.KeyDown += delegate(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.F)
                    this.rotateX = !this.rotateX;
                if (e.Key == Key.G)
                    this.rotateY = !this.rotateY;
                if (e.Key == Key.H)
                    this.rotateZ = !this.rotateZ;

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
            lastMouse = mouseDelta;

            System.Windows.Forms.Cursor.Position = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);

            Mouse.Move += new EventHandler<MouseMoveEventArgs>(OnMouseMove);

            m_textPrinter = new OpenTK.Graphics.TextPrinter();
        }

        void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            Vector2 newMouse = new Vector2(e.XDelta, e.YDelta);
            //mouseDelta = new Vector2(e.XDelta, e.YDelta);
            mouseDelta = newMouse - lastMouse;
            lastMouse = newMouse;
        }


        public void SetBounds(Vector3 min, Vector3 max)
        {
            Vector3 mid = new Vector3(max - min) / 2f;

            float longest = Math.Max(mid.X, Math.Max(mid.Y, mid.Z));

            //location = m_currentModel.Center;

            float val = longest * 3.5f;
            if (val < 2f)
            {
                val = 2f;
            }

            location = min + mid;

            location = location - (Vector3.UnitX * val);

            facing = 0;
            pitch = 0;

        }


        public void ChangeTextureNext()
        {
            if (m_currentModel.m_textures.Count > 1)
            {
                m_currentTextureIndex++;
                m_currentTextureIndex %= m_currentModel.m_textures.Count;
            }
        }

        public void ChangeTexturePrev()
        {
            if (m_currentModel.m_textures.Count > 1)
            {
                m_currentTextureIndex--;
                if (m_currentTextureIndex < 0)
                {
                    m_currentTextureIndex += m_currentModel.m_textures.Count;
                }
            }
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

        void CalcBindFinalMatrix(BoneNode bone, Matrix4 parentMatrix)
        {
            bone.combinedMatrix = bone.localMatrix * parentMatrix;
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
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
            Matrix4 p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 300.0f);
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
            //mouseDelta = new Vector2();

            //facing += mouseSpeed[0];
            //pitch += mouseSpeed[1];

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

            //GL.Enable(EnableCap.Texture2D);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PointSize(10f);

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
            if (rotateX || rotateY || rotateZ)
            {
                GL.Rotate(angle, rotateX ? 1.0f : 0.0f, rotateY ? 1.0f : 0.0f, rotateZ ? 1.0f : 0.0f);
            }

            GL.Color3(System.Drawing.Color.White);
            GL.Begin(PrimitiveType.Points);


            for (int i = 0; i < m_points.Count; ++i)
            {
                GL.Vertex3(m_points[i]);
            }

            GL.End();

            SwapBuffers();
        }


        [STAThread]
        public static void Main()
        {

            String sourceFile = @"D:\gladius-extracted-archive\gc-compressed\test-models\tag-ouput\Bow.mdl\SKIN";
            //String infoFile = @"D:\gladius-extracted-archive\gc-compressed\test-models\tag-ouput\Bow.mdl\info.txt";
            //List<Vector3> points = new List<Vector3>();
            //using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            //{
            //    using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile, FileMode.Open)))
            //    {
            //        try
            //        {
            //            binReader.BaseStream.Position = 0x90;
            //            byte[] holder = new byte[3];
            //            float[] fholder = new float[3];
            //            int numpoints = 30;
            //            int counter;
            //            int stride = 9 - 3;
            //            while (true)
            //            {
            //                for (int i = 0; i < holder.Length; ++i)
            //                {
            //                    holder[i] = binReader.ReadByte();
            //                    //fholder[i] = (holder[i] / (127.5f)) + 1.0f;
            //                    fholder[i] = (holder[i] / (128f));
            //                }

            //                points.Add(new Vector3(fholder[0],fholder[1],fholder[2]));
            //                infoStream.WriteLine(String.Format("[{0},{1},{2}]]", fholder[0], fholder[1], fholder[2]));
            //                binReader.BaseStream.Position += stride;
            //                if (points.Count > numpoints)
            //                {
            //                    break;
            //                }
            //            }
            //        }
            //        catch (System.Exception ex)
            //        {
            //            infoStream.Flush();
            //        }
            //    }
            //}

            GCModelReader reader = new GCModelReader();
            //GCModel model = reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\test-models\bow.mdl");
            GCModel model = reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\test-models\File 021091");
            model.BuildBB();
            using (BasicPointViewer example = new BasicPointViewer())
            {
                example.PopulateList(model.m_points);
                example.SetBounds(model.MinBB, model.MaxBB);
                //OpenTK.Platform.Utilities.SetWindowTitle(example);
                //example.PopulateList(modelReader.m_models[3].m_points);
                example.Run(30, 30);
            }
        }

        


        bool rotateX = false;
        bool rotateY = false;
        bool rotateZ = false;

        int m_currentModelIndex = 0;
        int m_currentTextureIndex = 0;
        int m_currentModelSubIndex = 0;
        GCModel m_currentModel;

        GCModelReader m_modelReader;
        OpenTK.Graphics.TextPrinter m_textPrinter;
        String m_debugLine;

        private readonly Font font = new Font("Verdana", 10);

        public Dictionary<String, int> m_textureDictionary = new Dictionary<String, int>();


        List<Vector3> m_points = new List<Vector3>();
    }






}

