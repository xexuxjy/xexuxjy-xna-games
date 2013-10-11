using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gladius.renderer
{
    public class FreeCamera : ICamera
    {
        #region ICamera Members

        public FreeCamera(CameraManager cameraManager)
        {
            m_cameraManager = cameraManager;
            Bounds = new BoundingFrustum(Matrix.Identity);
        }

        public Game Game
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Matrix View
        {
            get
            {
                return m_view;
            }
            set
            {
                m_view = value;
            }
        }

        public Microsoft.Xna.Framework.Matrix Projection
        {
            get
            {
                return m_projection;
            }
            set
            {
                m_projection = value;
            }
        }

        public Microsoft.Xna.Framework.Vector3 Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public Microsoft.Xna.Framework.Vector3 Target
        {
            get
            {
                return Vector3.Zero;
            }
            set
            {
                
            }
        }

        public Microsoft.Xna.Framework.Vector3 TargetDirection
        {
            get
            {
                return Vector3.Forward;
            }
            set
            {
                
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float rotationScalar = .01f;

            if (m_smoothedMouseMovement.Y < 0f)
            {
                int ibreak = 0;
            }

            m_yaw -= (m_smoothedMouseMovement.X * rotationScalar * elapsed);
            m_pitch -= (m_smoothedMouseMovement.Y * rotationScalar * elapsed);




            Quaternion rotation;
            Quaternion.CreateFromYawPitchRoll(m_yaw, m_pitch, 0f, out rotation);

            float movementScalar = 5f;

            DeltaPosition *= (elapsed * movementScalar);
            DeltaPosition = Vector3.TransformNormal(DeltaPosition, Matrix.Invert(View));

            Position += DeltaPosition;
            
            Matrix m = Matrix.CreateFromYawPitchRoll(m_yaw,m_pitch,0f);
            m.Translation = Position;
            View = Matrix.Invert(m);
            //m.Translation = Position;
            //Vector3.Forward = m.Forward;
            //View = Matrix.CreateLookAt(Position,m.Backward,m.Up);



            //Matrix.C

            Bounds.Matrix = Projection * View;

        }

        public void UpdateInput(GameStateManagement.InputState inputState)
        {
            DeltaPosition = Vector3.Zero;
                 
            if (inputState.IsHeldKey(Keys.A))
            {
                DeltaPosition += Vector3.Left;
            }
            if (inputState.IsHeldKey(Keys.D))
            {
                DeltaPosition += Vector3.Right;
            }
            if (inputState.IsHeldKey(Keys.W))
            {
                DeltaPosition += Vector3.Forward;
            }
            if (inputState.IsHeldKey(Keys.S))
            {
                DeltaPosition += Vector3.Backward;
            }
            if (inputState.IsHeldKey(Keys.Q))
            {
                DeltaPosition += Vector3.Up;
            }
            if (inputState.IsHeldKey(Keys.Z))
            {
                DeltaPosition += Vector3.Down;
            }

            Rectangle bounds = Game.Window.ClientBounds;
            int centerX = bounds.Width / 2;
            int centerY = bounds.Height / 2;

            int mouseDeltaX = inputState.CurrentMouseState.Position.X - centerX;
            int mouseDeltaY = inputState.CurrentMouseState.Position.Y - centerY;

            PerformMouseFiltering(mouseDeltaX, mouseDeltaY);

            CenterMouse();
            
//            smoothedMouseMovement.X
            
        }

        private void CenterMouse()
        {
            Rectangle clientBounds = Game.Window.ClientBounds;
            Mouse.SetPosition(clientBounds.Width / 2, clientBounds.Height / 2);
        }

        public Microsoft.Xna.Framework.BoundingFrustum Bounds
        {
            get
            {
                return m_frustum;
            }
            set
            {
                m_frustum = value;
            }
        }

        public Microsoft.Xna.Framework.Vector3 Velocity
        {
            get
            {
                return Vector3.Zero;
            }
            set
            {
                
            }
        }

        public Microsoft.Xna.Framework.Vector3 Acceleration
        {
            get
            {
                return Vector3.Zero;
            }
            set
            {
                
            }
        }

        public Microsoft.Xna.Framework.Vector3 Forward
        {
            get
            {
                return m_view.Forward;        
            }
            set
            {
                
            }
        }

        public Microsoft.Xna.Framework.Vector3 Up
        {
            get
            {
                return m_view.Up;
            }
            set
            {
                
            }
        }

        public Microsoft.Xna.Framework.Vector3 DesiredPositionOffset
        {
            get
            {
                return Vector3.Zero;
            }
            set
            {
                
            }
        }

        public Microsoft.Xna.Framework.Vector3 LookAtOffset
        {
            get
            {
                return Vector3.Zero;
            }
            set
            {
                
            }
        }

        public void SnapTo(Microsoft.Xna.Framework.Vector3 eye, Microsoft.Xna.Framework.Vector3 lookat)
        {
            
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
            get 
            {
                return true;
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get 
            {
                return 1;
            }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;

    private void PerformMouseFiltering(float x, float y)
    {
        if (x != 0 || y != 0)
        {
            int ibreak = 0;
        }
        float smoothingSensitivity = 0.5f;
        // Shuffle all the entries in the cache.
        // Newer entries at the front. Older entries towards the back.
        for (int i = m_mouseSmoothingCache.Length - 1; i > 0; --i)
        {
            m_mouseSmoothingCache[i].X = m_mouseSmoothingCache[i - 1].X;
            m_mouseSmoothingCache[i].Y = m_mouseSmoothingCache[i - 1].Y;
        }

        // Store the current mouse movement entry at the front of cache.
        m_mouseSmoothingCache[0].X = x;
        m_mouseSmoothingCache[0].Y = y;

        float averageX = 0.0f;
        float averageY = 0.0f;
        float averageTotal = 0.0f;
        float currentWeight = 1.0f;

        // Filter the mouse movement with the rest of the cache entries.
        // Use a weighted average where newer entries have more effect than
        // older entries (towards the back of the cache).
        for (int i = 0; i < m_mouseSmoothingCache.Length; ++i)
        {
            averageX += m_mouseSmoothingCache[i].X * currentWeight;
            averageY += m_mouseSmoothingCache[i].Y * currentWeight;
            averageTotal += 1.0f * currentWeight;
            currentWeight *= smoothingSensitivity;
        }

        // Calculate the new smoothed mouse movement.
        m_smoothedMouseMovement.X = averageX / averageTotal;
        m_smoothedMouseMovement.Y = averageY / averageTotal;
    }


    /// <summary>
    /// Resets all mouse states. This is called whenever the mouse input
    /// behavior switches from click-and-drag mode to real-time mode.
    /// </summary>
    private void ResetMouse()
    {
        //currentMouseState = Mouse.GetState();
        //previousMouseState = currentMouseState;

        for (int i = 0; i < m_mouseMovement.Length; ++i)
            m_mouseMovement[i] = Vector2.Zero;

        for (int i = 0; i < m_mouseSmoothingCache.Length; ++i)
            m_mouseSmoothingCache[i] = Vector2.Zero;

        //savedMousePosX = -1;
        //savedMousePosY = -1;

        //smoothedMouseMovement = Vector2.Zero;
        //mouseIndex = 0;

        //Rectangle clientBounds = Game.Window.ClientBounds;

        //int centerX = clientBounds.Width / 2;
        //int centerY = clientBounds.Height / 2;
        //int deltaX = centerX - currentMouseState.X;
        //int deltaY = centerY - currentMouseState.Y;

        //Mouse.SetPosition(centerX, centerY);
    }

        private Vector3 DeltaPosition
        {   
            get;set;
    
        }


        Matrix m_view;
        Matrix m_projection;
        float m_yaw;
        float m_pitch;
        Vector3 m_position;
        BoundingFrustum m_frustum;
        MouseState m_lastMouseState;
        const int m_numPoints = 5;
        private Vector2[] m_mouseMovement = new Vector2[m_numPoints];
        private Vector2[] m_mouseSmoothingCache = new Vector2[m_numPoints];
        private Vector2 m_smoothedMouseMovement = Vector2.Zero;
        private CameraManager m_cameraManager;
        #endregion
    }
}
