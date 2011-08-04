using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.interfaces;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.camera
{
    public class DefaultCamera : ICamera
    {

        public DefaultCamera(float fov, float aspect,float near, float far)
        {
            m_nearPlane = near;
            m_farPlane = far;
            m_aspect = aspect;
            m_fov = fov;
            m_up = Vector3.Up;
            m_distance = 50f;
        }

        public void Initialize()
        {
            m_view = Matrix.Identity;
            m_projection = Matrix.CreatePerspectiveFieldOfView(m_fov, m_aspect, m_nearPlane, m_farPlane);
        }
        


        public Matrix View
        {
            get
            {
                return m_view;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Matrix Projection
        {
            get
            {
                return m_projection;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }


        public bool IsInViewFrustum(terrain.WorldObject worldObject)
        {
            return true;
        }



        public bool Enabled
        {
            get { return true; }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public void Update(GameTime gameTime)
        {
            //m_view = Matrix.CreateLookAt(m_position, m_lookAtPosition, m_up);
            float rele = m_pitch;
            float razi = m_yaw;

            Quaternion rot = Quaternion.CreateFromAxisAngle(m_up, razi);

            Vector3 eyePos = new Vector3();
            eyePos.Z = -m_distance;

            Vector3 forward = eyePos;
            if (forward.LengthSquared() < float.Epsilon)
            {
                forward = new Vector3(0, 0, -1);
            }
            Vector3 right = Vector3.Cross(m_up, Vector3.Normalize(forward));
            Quaternion roll = Quaternion.CreateFromAxisAngle(right, -rele);
            rot.Normalize();
            roll.Normalize();

            Matrix m1 = Matrix.CreateFromQuaternion(rot);
            Matrix m2 = Matrix.CreateFromQuaternion(roll);
            Matrix m3 = m1 * m2;


            eyePos = Vector3.Transform(eyePos, (rot * roll));

            //m_cameraTargetPosition = m_cameraPosition + eyePos;
            m_position = eyePos;

            m_position += m_targetPosition;

            //if (m_glutScreenWidth == 0 && m_glutScreenHeight == 0)
            //    return;

            m_view= Matrix.CreateLookAt(m_position, m_targetPosition, m_up);
            int ibreak = 0;
        }

        public int UpdateOrder
        {
            get { return 0; }
        }

        public float Distance
        {
            get { return m_distance; }
            set
            {
                m_distance = value;
                if (m_distance < 0.1f)
                {
                    m_distance = 0.1f;
                }
            }
        }

        public float Pitch
        {
            get { return m_pitch; }
            set { m_pitch = value; }
        }

        public float Yaw
        {
            get { return m_yaw; }
            set { m_yaw = value; }
        }


        public event EventHandler<EventArgs> UpdateOrderChanged;
        protected Vector3 m_position;
        protected Vector3 m_targetPosition;
        protected Vector3 m_lookAtPosition;
        protected Vector3 m_up;
        protected Matrix m_view;
        protected Matrix m_projection;
        protected float m_nearPlane;
        protected float m_farPlane;
        protected float m_aspect;
        protected float m_fov;
        protected float m_yaw;
        protected float m_pitch;
        protected float m_distance;

    }
}
