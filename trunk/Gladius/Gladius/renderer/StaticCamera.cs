using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gladius.gamestatemanagement.screenmanager;

namespace Gladius.renderer
{
    public class StaticCamera : ICamera
    {
        public StaticCamera(CameraManager cameraManager) 
        {
            m_cameraManager = cameraManager;
        }

        #region ICamera Members

        public Microsoft.Xna.Framework.Matrix View
        {
            get;
            set;

        }

        public Microsoft.Xna.Framework.Matrix Projection
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector3 Position
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector3 Target
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector3 TargetDirection
        {
            get;
            set;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }

        public void UpdateInput(InputState inputState)
        {
            
        }

        public Microsoft.Xna.Framework.BoundingFrustum Bounds
        {
            get
            {
                return m_boundingFrustum;
            }
            set
            {
                m_boundingFrustum = value;
            }
        }

        public Microsoft.Xna.Framework.Vector3 Velocity
        {
            get;
            set;

        }

        public Microsoft.Xna.Framework.Vector3 Acceleration
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector3 Forward
        {
            get;
            set;
        
        }

        public Microsoft.Xna.Framework.Vector3 Up
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector3 DesiredPositionOffset
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector3 LookAtOffset
        {
            get;
            set;
        }


        public void SnapTo(Microsoft.Xna.Framework.Vector3 eye, Microsoft.Xna.Framework.Vector3 lookat)
        {
            Position = eye;
            Target = lookat;
        }

        #endregion

        #region IGameComponent Members

        public void Initialize()
        {
        }

        #endregion

        #region IUpdateable Members

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get { return 1; }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;


        private BoundingFrustum m_boundingFrustum;
        private CameraManager m_cameraManager;
        #endregion
    }
}
