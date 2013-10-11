using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;

namespace Gladius.renderer
{
    public class CameraManager
    {

        public CameraManager(Game game)
        {
            m_chaseCamera = new ChaseCamera(this);
            m_freeCamera = new FreeCamera(this);
            m_freeCamera.Game = game;
            m_staticCamera = new StaticCamera(this);
            SetChaseCamera();
            m_fov = MathHelper.PiOver4;
            m_aspect = (float)game.Window.ClientBounds.Width/(float)game.Window.ClientBounds.Height;
            m_near = 1f;
            m_far = 200f;
            RebuildProjection();
        }

        public float DefaultFOV
        {
            get
            {
                return m_fov;
            }
            set
            {
                m_fov = value;
                RebuildProjection();
            }
        }

        public float DefaultAspectRatio
        {
            get
            {
                return m_aspect;
            }
            set
            {
                m_aspect = value;
                RebuildProjection();
            }
        }

        public float DefaultNearPlane
        {
            get
            {
                return m_near;
            }
            set
            {
                m_near = value;
                RebuildProjection();
            }
        }

        public float DefaultFarPlane
        {
            get
            {
                return m_far;
            }
            set
            {
                m_far = value;
                RebuildProjection();
            }
        }


        public ICamera ActiveCamera
        {
            get
            {
                return m_activeCamera;
            }
            set
            {
                if (!DefaultCameraOverride)
                {
                    if (m_activeCamera != value)
                    {
                        CopyParameters(m_activeCamera, value);
                    }
                    m_activeCamera = value;
                    Globals.Camera = value;
                }
            }
        }

        // ignore player override restriction
        private ICamera InternalActiveCamera
        {
            set
            {
                if (m_activeCamera != value)
                {
                    CopyParameters(m_activeCamera, value);
                }
                m_activeCamera = value;
                Globals.Camera = value;

            }
        }

        private void CopyParameters(ICamera oldCamera,ICamera newCamera)
        {
            if (oldCamera != null)
            {
                newCamera.Position = oldCamera.Position;
                newCamera.View = oldCamera.View;
                newCamera.Projection = oldCamera.Projection;

            }
        }


        public void UpdateInput(InputState inputState)
        {
            // manual change camera.
            if (inputState.IsNewKeyPress(Keys.F1))
            {
                DefaultCameraOverride = true;
                InternalActiveCamera = m_chaseCamera;
            }
            if (inputState.IsNewKeyPress(Keys.F2))
            {
                DefaultCameraOverride = true;
                InternalActiveCamera = m_freeCamera;
            }
            if (inputState.IsNewKeyPress(Keys.F3))
            {
                DefaultCameraOverride = true;
                InternalActiveCamera = m_staticCamera;
            }
            if (inputState.IsNewKeyPress(Keys.F4))
            {
                DefaultCameraOverride = false;
            }

            ActiveCamera.UpdateInput(inputState);
        }

        bool DefaultCameraOverride
        {
            get;
            set;
        }

        public void SetChaseCamera()
        {
            ActiveCamera = m_chaseCamera;
        }

        public void SetStaticCamera()
        {
            ActiveCamera = m_staticCamera;
        }

        public void SetFreeCamera()
        {
            ActiveCamera = m_freeCamera;
        }

        public void Update(GameTime gameTime)
        {
            ActiveCamera.Update(gameTime);
        }

        private void RebuildProjection()
        {
            Matrix.CreatePerspectiveFieldOfView(m_fov,m_aspect,m_near,m_far,out m_projection);
            m_activeCamera.Projection = m_projection;
        }



        private ICamera m_activeCamera;
        private ChaseCamera m_chaseCamera;
        private FreeCamera m_freeCamera;
        private StaticCamera m_staticCamera;

        private Vector3 m_position;
        private Matrix m_view;
        private Matrix m_projection;

        private float m_fov;
        private float m_aspect;
        private float m_near;
        private float m_far;

    }
}
