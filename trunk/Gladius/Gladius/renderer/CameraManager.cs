using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dhpoware;
using GameStateManagement;

namespace Gladius.renderer
{
    public class CameraManager
    {

        public CameraManager(Game game)
        {
            m_chaseCamera = new ChaseCamera();
            m_freeCamera = new CameraComponent(game);
            m_staticCamera = new StaticCamera();
            SetChaseCamera();
        }
       
        public ICamera ActiveCamera
        {
            get
            {
                return m_activeCamera;
            }
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
            ActiveCamera.UpdateInput(inputState);
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


        private ICamera m_activeCamera;
        private ChaseCamera m_chaseCamera;
        private CameraComponent m_freeCamera;
        private StaticCamera m_staticCamera;

        private Vector3 m_position;
        private Matrix m_view;
        private Matrix m_projection;

    }
}
