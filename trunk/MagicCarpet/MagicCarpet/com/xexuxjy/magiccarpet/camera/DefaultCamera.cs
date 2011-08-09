//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using com.xexuxjy.magiccarpet.interfaces;
//using Microsoft.Xna.Framework;

//namespace com.xexuxjy.magiccarpet.camera
//{
//    public class DefaultCamera : ICamera
//    {

//        public DefaultCamera(float fov, float aspect,float near, float far)
//        {
//            m_nearPlane = near;
//            m_farPlane = far;
//            m_aspect = aspect;
//            m_fov = fov;
//            m_distance = 50f;
//        }

//        public void Initialize()
//        {
//            m_view = Matrix.Identity;
//            m_projection = Matrix.CreatePerspectiveFieldOfView(m_fov, m_aspect, m_nearPlane, m_farPlane);
//        }
        


//        public Matrix View
//        {
//            get
//            {
//                return m_view;
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public Matrix Projection
//        {
//            get
//            {
//                return m_projection;
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public Vector3 Position
//        {
//            get { return m_position; }
//            set 
//            { 
//                m_position = value; 
//            }
//        }


//        public bool IsInViewFrustum(terrain.WorldObject worldObject)
//        {
//            return true;
//        }



//        public bool Enabled
//        {
//            get { return true; }
//        }

//        public event EventHandler<EventArgs> EnabledChanged;

//        public void Update(GameTime gameTime)
//        {
//            //m_view = Matrix.CreateLookAt(m_position, m_lookAtPosition, m_up);
//            float rele = m_pitch;
//            float razi = m_yaw;

//            Quaternion q = Quaternion.CreateFromYawPitchRoll(-m_yaw, m_pitch, 0);
//            m_view = Matrix.Invert(Matrix.CreateFromQuaternion(q) * Matrix.CreateTranslation(m_position));

//            //m_up = Vector3.Transform(m_up, rot);
//            //m_up.Normalize();
//            //Quaternion roll = Quaternion.CreateFromAxisAngle(Vector3.Right, -rele);
//            //m_right = Vector3.Transform(Vector3.Right, roll);
//            //m_right.Normalize();
//            ////m_forward = Vector3.Cross(m_up,m_right);
//            //m_forward = Vector3.Cross(m_right, m_up);
//            //m_forward.Normalize();

//            //m_targetPosition = m_position + m_forward;

//            //m_view= Matrix.CreateLookAt(m_position, m_targetPosition, m_up);
//        }

//        public int UpdateOrder
//        {
//            get { return 0; }
//        }

//        public float Distance
//        {
//            get { return m_distance; }
//            set
//            {
//                m_distance = value;
//                if (m_distance < 0.1f)
//                {
//                    m_distance = 0.1f;
//                }
//            }
//        }

//        public float Pitch
//        {
//            get { return m_pitch; }
//            set { m_pitch = value; }
//        }

//        public float Yaw
//        {
//            get { return m_yaw; }
//            set { m_yaw = value; }
//        }

//        public Vector3 Forward
//        {
//            get { return m_view.Backward; }
//        }

//        public Vector3 Up
//        {
//            get { return m_view.Up; }
//        }

//        public Vector3 Right
//        {
//            get { return m_view.Right; }
//        }


//        // move relative to camera
//        public void TranslateLocal(Vector3 vec)
//        {
//            Quaternion q = Quaternion.CreateFromYawPitchRoll(-m_yaw, m_pitch, 0);
//            Vector3 localTranslate = Vector3.Transform(vec, q);
//            m_position += localTranslate;
//        }



//        public event EventHandler<EventArgs> UpdateOrderChanged;
//        protected Vector3 m_position;
//        protected Vector3 m_targetPosition;
//        protected Vector3 m_lookAtPosition;
//        //protected Vector3 m_up;
//        //protected Vector3 m_forward;
//        //protected Vector3 m_right;

//        protected Matrix m_view;
//        protected Matrix m_projection;
//        protected float m_nearPlane;
//        protected float m_farPlane;
//        protected float m_aspect;
//        protected float m_fov;
//        protected float m_yaw;
//        protected float m_pitch;
//        protected float m_distance;

//    }
//}
