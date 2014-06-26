using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using OpenTK.Input;
//using OpenTK.Platform;

namespace ModelNamer
{
    public class PointViewer : GameWindow
    {
        static int size = 5;
        Vector3 eyePos = new Vector3(size*5, size*5, 0);
        Vector3 eyeLookat = new Vector3(0, 0, 0);
        const float rotation_speed = 180.0f;
        float angle;

        public PointViewer()
            : base(800, 600)
        {
            this.VSync = VSyncMode.Off;
        }

        public void BuildCube()
        {
            m_points.Add(new sVector3(-size, -size, -size));
            m_points.Add(new sVector3(size, -size, -size));
            m_points.Add(new sVector3(-size, -size, size));
            m_points.Add(new sVector3(size, -size, size));
            m_points.Add(new sVector3(-size, size, -size));
            m_points.Add(new sVector3(size, size, -size));
            m_points.Add(new sVector3(-size, size, size));
            m_points.Add(new sVector3(size, size, size));
        }

        public void PopulateList(List<sVector3> points)
        {
            m_points.Clear();
            foreach (sVector3 sv in points)
            {
                m_points.Add(new sVector3(sv.x*size,sv.y*size,sv.z*size));
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
            this.Title = "FPS: " + string.Format("{0:F}", 1.0 / e.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PointSize(5f);

            //Matrix4 lookat = Matrix4.LookAt(eyePos,eyeLookat,Vector3.UnitY);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref lookat);
            //Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            Matrix4 lookat = Matrix4.LookAt(eyePos, eyeLookat, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            angle += rotation_speed * (float)e.Time;
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);


            GL.Begin(PrimitiveType.Points);
            //GL.Begin(PrimitiveType.Lines);

            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(1, 1, 1);

            //GL.Begin(PrimitiveType.Quads);

            GL.Color3(System.Drawing.Color.ForestGreen);
            //GL.Vertex3(1.0f, -1.0f, -1.0f);
            //GL.Vertex3(1.0f, 1.0f, -1.0f);
            //GL.Vertex3(1.0f, 1.0f, 1.0f);
            //GL.Vertex3(1.0f, -1.0f, 1.0f);


            for (int i = 0; i < m_points.Count; ++i)
            {
                GL.Vertex3(m_points[i].x, m_points[i].y, m_points[i].z);
            }

            GL.End();
            SwapBuffers();
        }


        [STAThread]
        public static void Main()
        {
            using (PointViewer example = new PointViewer())
            {
                //OpenTK.Platform.Utilities.SetWindowTitle(example);
                GCModelReader modelReader = new GCModelReader();
                modelReader.LoadModels();

                example.PopulateList(modelReader.m_models[3].m_points);
                example.Run(30, 30);
            }
        }



        List<sVector3> m_points = new List<sVector3>();
    }
}
