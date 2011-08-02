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
            m_view = Matrix.CreateLookAt(m_position, m_lookAtPosition, m_up);

        }

        public int UpdateOrder
        {
            get { return 0; }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;
        protected Vector3 m_position;
        protected Vector3 m_lookAtPosition;
        protected Vector3 m_up;
        protected Matrix m_view;
        protected Matrix m_projection;
        protected float m_nearPlane;
        protected float m_farPlane;
        protected float m_aspect;
        protected float m_fov;

    }
}
