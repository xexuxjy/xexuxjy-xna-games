using System;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA.LinearMath;

//-----------------------------------------------------------------------------
// Based on.
// ChaseCamera.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


namespace com.xexuxjy.magiccarpet.camera
{
    public class ChaseCamera : EmptyGameObject
    {
        public ChaseCamera()
            : base(GameObjectType.camera)
        {
        }

        #region Chased object properties (set externally each frame)

        /// <summary>
        /// Position of object being chased.
        /// </summary>
        public IndexedVector3 ChasePosition
        {
            get { return chasePosition; }
            set { chasePosition = value; }
        }
        private IndexedVector3 chasePosition;

        /// <summary>
        /// Direction the chased object is facing.
        /// </summary>
        public IndexedVector3 ChaseDirection
        {
            get { return chaseDirection; }
            set { chaseDirection = value; }
        }
        private IndexedVector3 chaseDirection = IndexedVector3.Forward;

        /// <summary>
        /// Chased object's Up vector.
        /// </summary>
        public IndexedVector3 Up
        {
            get { return up; }
            set { up = value; }
        }
        private IndexedVector3 up = IndexedVector3.Up;

        #endregion

        #region Desired camera positioning (set when creating camera or changing view)

        /// <summary>
        /// Desired camera position in the chased object's coordinate system.
        /// </summary>
        public IndexedVector3 DesiredPositionOffset
        {
            get { return desiredPositionOffset; }
            set { desiredPositionOffset = value; }
        }
        private IndexedVector3 desiredPositionOffset = new IndexedVector3(2, 0, 0f);
        //private IndexedVector3 desiredPositionOffset = new IndexedVector3(0, 0, -2.0f);

        /// <summary>
        /// Desired camera position in world space.
        /// </summary>
        public IndexedVector3 DesiredPosition
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return desiredPosition;
            }
        }
        private IndexedVector3 desiredPosition;

        /// <summary>
        /// Look at point in the chased object's coordinate system.
        /// </summary>
        public IndexedVector3 LookAtOffset
        {
            get { return lookAtOffset; }
            set { lookAtOffset = value; }
        }
        private IndexedVector3 lookAtOffset = new IndexedVector3(0, 0, 0);

        /// <summary>
        /// Look at point in world space.
        /// </summary>
        public IndexedVector3 LookAt
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return lookAt;
            }
        }
        private IndexedVector3 lookAt;

        #endregion

        #region Camera physics (typically set when creating camera)

        /// <summary>
        /// Physics coefficient which controls the influence of the camera's position
        /// over the spring force. The stiffer the spring, the closer it will stay to
        /// the chased object.
        /// </summary>
        public float Stiffness
        {
            get { return stiffness; }
            set { stiffness = value; }
        }
        private float stiffness = 1800.0f;

        /// <summary>
        /// Physics coefficient which approximates internal friction of the spring.
        /// Sufficient damping will prevent the spring from oscillating infinitely.
        /// </summary>
        public float Damping
        {
            get { return damping; }
            set { damping = value; }
        }
        private float damping = 600.0f;

        /// <summary>
        /// Mass of the camera body. Heaver objects require stiffer springs with less
        /// damping to move at the same rate as lighter objects.
        /// </summary>
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        private float mass = 50.0f;

        #endregion

        #region Current camera properties (updated by camera physics)

        /// <summary>
        /// Position of camera in world space.
        /// </summary>
        public IndexedVector3 Position
        {
            get { return position; }
        }
        private IndexedVector3 position;

        /// <summary>
        /// Velocity of camera.
        /// </summary>
        public IndexedVector3 Velocity
        {
            get { return velocity; }
        }
        private IndexedVector3 velocity;


        public IndexedVector3 MaxVelocity
        {
            get{return maxVelocity;}
            set
            {
                maxVelocity = value;
            }
        }

        IndexedVector3 maxVelocity;
        #endregion


        #region Perspective properties

        /// <summary>
        /// Perspective aspect ratio. Default value should be overriden by application.
        /// </summary>
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }
        private float aspectRatio = 4.0f / 3.0f;

        /// <summary>
        /// Perspective field of view.
        /// </summary>
        public float FieldOfView
        {
            get { return fieldOfView; }
            set { fieldOfView = value; }
        }
        private float fieldOfView = MathHelper.ToRadians(45.0f);

        /// <summary>
        /// Distance to the near clipping plane.
        /// </summary>
        public float NearPlaneDistance
        {
            get { return nearPlaneDistance; }
            set { nearPlaneDistance = value; }
        }
        private float nearPlaneDistance = 1.0f;

        /// <summary>
        /// Distance to the far clipping plane.
        /// </summary>
        public float FarPlaneDistance
        {
            get { return farPlaneDistance; }
            set { farPlaneDistance = value; }
        }
        private float farPlaneDistance = 100000.0f;

        #endregion

        #region Matrix properties

        /// <summary>
        /// View transform matrix.
        /// </summary>
        public IndexedMatrix View
        {
            get { return view; }
        }
        private IndexedMatrix view;

        /// <summary>
        /// Projecton transform matrix.
        /// </summary>
        public IndexedMatrix Projection
        {
            get 
            { 
                //return projection.ToMatrixProjection(); 
                return projection;
            }
        }
        private IndexedMatrix projection;


        public bool ClipToWorld
        {
            set
            {
                clipToWorld = value;
            }
        }
        private bool clipToWorld;


        public GameObject FollowTarget
        {
            get { return m_followTarget; }
            set { m_followTarget = value; }
        }
        private GameObject m_followTarget;


        public void Zoom(float amount)
        {
            IndexedVector3 offset = DesiredPositionOffset;
            offset.Normalize();

            // clamp the values a bit.

            float minzoom = 1f;
            float maxzoom = 100f;

            IndexedVector3 iv = DesiredPositionOffset + (offset * amount);
            if (iv.Length() >= minzoom && iv.Length() <= maxzoom)
            {
                DesiredPositionOffset += offset * amount;
            }
        }


        #endregion


        #region Methods

        /// <summary>
        /// Rebuilds object space values in world space. Invoke before publicly
        /// returning or privately accessing world space values.
        /// </summary>
        private void UpdateWorldPositions()
        {
            // Construct a matrix to transform from object space to worldspace
            IndexedBasisMatrix transform = IndexedBasisMatrix.Identity;
            transform.Forward = ChaseDirection;
            transform.Up = Up;
            transform.Right = IndexedVector3.Cross(ChaseDirection,Up);

            // Calculate desired camera properties in world space
            desiredPosition = ChasePosition +(DesiredPositionOffset * transform);
            lookAt = ChasePosition + (LookAtOffset * transform);
        }

        /// <summary>
        /// Rebuilds camera's view and projection matricies.
        /// </summary>
        private void UpdateMatrices()
        {
            //m_lightView = IndexedMatrix.CreateLookAt(m_lightPosition, target, new IndexedVector3(0, 1, 0));
            //m_lightProjection = IndexedMatrix.CreatePerspectiveFieldOfView(fov, aspect, 1f, 500f);

            view = IndexedMatrix.CreateLookAt(this.Position, this.LookAt, this.Up);
            projection = IndexedMatrix.CreatePerspectiveFieldOfView(FieldOfView,
                AspectRatio, NearPlaneDistance, FarPlaneDistance);
        }

        /// <summary>
        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        /// </summary>
        public void Reset()
        {
            UpdateWorldPositions();

            // Stop motion
            velocity = IndexedVector3.Zero;

            // Force desired position
            position = desiredPosition;

            UpdateMatrices();
        }

        /// <summary>
        /// Animates the camera from its current position towards the desired offset
        /// behind the chased object. The camera's animation is controlled by a simple
        /// physical spring attached to the camera and anchored to the desired position.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            UpdateWorldPositions();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate spring force
            IndexedVector3 stretch = position - desiredPosition;
            IndexedVector3 force = -stiffness * stretch - damping * velocity;

            // Apply acceleration
            IndexedVector3 acceleration = force / mass;
            velocity += acceleration * elapsed;

            // Apply velocity
            position += velocity * elapsed;

            position = desiredPosition;

            UpdateMatrices();
        }

        #endregion
    }
}
