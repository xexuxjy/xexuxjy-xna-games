using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletXNA.BulletDynamics;
using BulletXNA.BulletCollision;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;
using System.Diagnostics;
using BulletXNA;
using com.xexuxjy.magiccarpet.interfaces;
using BulletXNADemos.Demos;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.collision
{
    public class CollisionManager : EmptyGameObject
    {
        public CollisionManager(IndexedVector3 worldMin, IndexedVector3 worldMax)
            : base(GameObjectType.manager)
        {
            //game.Components.Add(this);
            m_collisionConfiguration = new DefaultCollisionConfiguration();

            ///use the default collision dispatcher. For parallel processing you can use a diffent dispatcher (see Extras/BulletMultiThreaded)
            m_dispatcher = new CollisionDispatcher(m_collisionConfiguration);

            //BulletGlobals.gContactAddedCallback = new CustomMaterialCombinerCallback();

            m_broadphase = new DbvtBroadphase();
            IOverlappingPairCache pairCache = null;
            //pairCache = new SortedOverlappingPairCache();

            //m_broadphase = new SimpleBroadphase(1000, pairCache);

            ///the default constraint solver. For parallel processing you can use a different solver (see Extras/BulletMultiThreaded)
            SequentialImpulseConstraintSolver sol = new SequentialImpulseConstraintSolver();
            m_constraintSolver = sol;

            m_dynamicsWorld = new DiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_constraintSolver, m_collisionConfiguration);

            m_dynamicsWorld.SetGravity(ref m_gravity);

            m_ghostPairCallback = new GhostPairCallback();
            m_dynamicsWorld.GetBroadphase().GetOverlappingPairCache().SetInternalGhostPairCallback(m_ghostPairCallback);	// Needed once to enable ghost objects inside Bullet


            DrawOrder = Globals.DEBUG_DRAW_ORDER;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void Reset()
        {


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            if (Globals.DebugDraw != null)
            {
                m_dynamicsWorld.SetDebugDrawer(Globals.DebugDraw);

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Update(GameTime gameTime)
        {
            float ms = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //ms *= 0.1f;
            ///step the simulation
            m_dynamicsWorld.StepSimulation(ms, 1);
            //m_dynamicsWorld.DebugDrawWorld();

            ProcessCollisions();

            String debugText = String.Format("CollisionManager Objects[{0}] Constraints[{1}] Pairs[{2}] Manifolds[{3}].", m_dynamicsWorld.GetNumCollisionObjects(), m_dynamicsWorld.GetNumConstraints(), m_broadphase.GetOverlappingPairCache().GetNumOverlappingPairs(), m_dispatcher.GetNumManifolds());
            Globals.DebugDraw.DrawText(debugText, Globals.DebugTextCollisionManager, IndexedVector3.One);

            base.Update(gameTime);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected void ProcessCollisions()
        {
            IDispatcher dispatcher = m_dynamicsWorld.GetDispatcher();
            int numManifolds = dispatcher.GetNumManifolds();
	        for (int i=0;i<numManifolds;i++)
	        {
                PersistentManifold contactManifold = dispatcher.GetManifoldByIndexInternal(i);
		        CollisionObject obA = (contactManifold.GetBody0() as CollisionObject);
		        CollisionObject obB = (contactManifold.GetBody1() as CollisionObject);

                if (obA != null && obB != null)
                {
                    ICollideable user0 = obA.GetUserPointer() as ICollideable;
                    ICollideable user1 = obB.GetUserPointer() as ICollideable;

                    if (user0 != null && user0.Active() && user1 != null && user1.Active())
                    {
                        int numContacts = contactManifold.GetNumContacts();
                        for (int j = 0; j < numContacts; j++)
                        {
                            ManifoldPoint pt = contactManifold.GetContactPoint(j);
                            if (pt.GetDistance() < 0.0f)
                            {
                                user0.ProcessCollision(user1, pt);
                                user1.ProcessCollision(user0, pt);
                            }
                        }
                    }
                }
	        }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape, IMotionState motionState, bool addToWorld,Object userPointer)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape,motionState, addToWorld,userPointer);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public RigidBody LocalCreateRigidBody(float mass, ref Matrix startTransform, CollisionShape shape, IMotionState motionState, bool addToWorld, Object userPointer)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape, motionState, addToWorld, userPointer, CollisionFilterGroups.StaticFilter,(CollisionFilterGroups.AllFilter ^ CollisionFilterGroups.StaticFilter));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape, IMotionState motionState, bool addToWorld, Object userPointer, CollisionFilterGroups filterGroup, CollisionFilterGroups filterMask)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape, motionState, addToWorld, userPointer, filterGroup, filterMask);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public RigidBody LocalCreateRigidBody(float mass, ref Matrix startTransform, CollisionShape shape, IMotionState motionState, bool addToWorld, Object userPointer, CollisionFilterGroups filterGroup, CollisionFilterGroups filterMask)
        {

            Debug.Assert((shape == null || shape.GetShapeType() != BroadphaseNativeTypes.INVALID_SHAPE_PROXYTYPE));

            //rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = !MathUtil.CompareFloat(mass, 0f);

            IndexedVector3 localInertia = IndexedVector3.Zero;
            if (isDynamic)
            {
                shape.CalculateLocalInertia(mass, out localInertia);
            }
            //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects

            //#define USE_MOTIONSTATE 1
            //#ifdef USE_MOTIONSTATE
            if (motionState == null)
            {
                motionState = new DefaultMotionState(startTransform, Matrix.Identity);
            }

            RigidBodyConstructionInfo cInfo = new RigidBodyConstructionInfo(mass, motionState, shape, localInertia);


            RigidBody body = new RigidBody(cInfo);
            // disable all gravity for now?
            body.SetFlags(body.GetFlags() | RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY);
            // most objects should stay live?
            body.SetActivationState(ActivationState.DISABLE_DEACTIVATION);
            body.SetUserPointer(userPointer);
            body.SetGravity(ref m_gravity);
            if (addToWorld)
            {
                m_dynamicsWorld.AddRigidBody(body,filterGroup,filterMask);
            }

            return body;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        
        public GhostObject LocalCreateGhostObject(Matrix startTransform, CollisionShape shape, IMotionState motionState, bool addToWorld, Object userPointer, CollisionFilterGroups filterGroup, CollisionFilterGroups filterMask)
        {
            return LocalCreateGhostObject(ref startTransform, shape, motionState, addToWorld, userPointer, filterGroup, filterMask);
        }

        public GhostObject LocalCreateGhostObject(ref Matrix startTransform, CollisionShape shape, IMotionState motionState, bool addToWorld, Object userPointer, CollisionFilterGroups filterGroup, CollisionFilterGroups filterMask)
        {
            GhostObject ghostObject = new GhostObject();
            ghostObject.SetCollisionShape(shape);
            ghostObject.SetCollisionFlags(CollisionFlags.CF_NO_CONTACT_RESPONSE);		// We can choose to make it "solid" if we want...
            ghostObject.SetUserPointer(userPointer);
            m_dynamicsWorld.AddCollisionObject(ghostObject, filterGroup, filterMask);
            return ghostObject;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void AddToWorld(CollisionObject collisionObject)
        {
            m_dynamicsWorld.AddCollisionObject(collisionObject);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void AddToWorld(CollisionObject collisionObject,CollisionFilterGroups group,CollisionFilterGroups mask)
        {
            m_dynamicsWorld.AddCollisionObject(collisionObject,group,mask);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void RemoveFromWorld(CollisionObject collisionObject)
        {
            m_dynamicsWorld.RemoveCollisionObject(collisionObject);
            collisionObject.Cleanup();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        // a check against terrain only.
        public bool CastCameraGroundRay(Vector3 startPos, Vector3 endPos, ref Vector3 collisionPoint, ref Vector3 collisionNormal)
        {
            if (m_dynamicsWorld != null)
            {
                IndexedVector3 iv3start = startPos;
                IndexedVector3 iv3end = endPos;
                if (m_groundCallback == null)
                {
                    m_groundCallback = new ClosestRayResultCallback(ref iv3start, ref iv3end);
                }
                else
                {
                    m_groundCallback.Initialize(ref iv3start, ref iv3end);
                }

                m_groundCallback.m_collisionFilterMask = (CollisionFilterGroups)(GameObjectType.terrain);
                m_groundCallback.m_collisionFilterGroup = (CollisionFilterGroups)(GameObjectType.camera);
                m_dynamicsWorld.RayTest(ref iv3start, ref iv3end, m_groundCallback);

            }
            if (m_groundCallback != null)
            {
                if (m_groundCallback.HasHit())
                {
                    collisionPoint = m_groundCallback.m_hitPointWorld.ToVector3();
                    collisionNormal = m_groundCallback.m_hitNormalWorld.ToVector3();
                    return true;
                }
            }
            return false;

        }


        public bool CastRay(Vector3 startPos, Vector3 endPos, ref Vector3 collisionPoint, ref Vector3 collisionNormal)
        {
            if (m_dynamicsWorld != null)
            {
                IndexedVector3 iv3start = startPos;
                IndexedVector3 iv3end = endPos;
                if (m_callback == null)
                {
                    m_callback = new ClosestRayResultCallback(ref iv3start, ref iv3end);
                }
                else
                {
                    m_callback.Initialize(ref iv3start, ref iv3end);
                }
                m_callback.m_collisionFilterMask = (CollisionFilterGroups)(-1);
                m_callback.m_collisionFilterGroup = (CollisionFilterGroups)( -1);
                m_dynamicsWorld.RayTest(ref iv3start, ref iv3end, m_callback);
                
            }
            if (m_callback != null)
            {
                if (m_callback.HasHit())
                {
                    collisionPoint = m_callback.m_hitPointWorld.ToVector3();
                    collisionNormal = m_callback.m_hitNormalWorld.ToVector3();
                    return true;
                }
            }
            return false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Draw(GameTime gameTime)
        {
            // do these last.
            if (Globals.DebugDraw != null)
            {
                Matrix view = Globals.Camera.ViewMatrix;
                Matrix projection = Globals.Camera.ProjectionMatrix;

                m_dynamicsWorld.DebugDrawWorld();

                bool drawGrid = false;
                if(drawGrid)
                {
                    IndexedVector3 color = new IndexedVector3(1,0,0);
                    int gridSize = 20;
                    int gridHeight = 10;
                    int xStep = 2;
                    int yStep = 2;
                    int zStep = 1;
                    for (int i = 0; i < gridSize; i+=xStep)
                    {
                        for (int j = 0; j < gridHeight; j+=yStep)
                        {
                            IndexedVector3 from = new IndexedVector3(-100, j, i);
                            IndexedVector3 to = new IndexedVector3(100, j, i);

                            Globals.DebugDraw.DrawLine(from, to, color);
                        }
                    }
                }

                ((XNA_ShapeDrawer)Globals.DebugDraw).RenderDebugLines(gameTime, view, projection);
                ((XNA_ShapeDrawer)Globals.DebugDraw).RenderOthers(gameTime, view, projection);
                ((XNA_ShapeDrawer)Globals.DebugDraw).RenderStandard(gameTime, view, projection, false);

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected ClosestRayResultCallback m_callback = null;
        protected ClosestRayResultCallback m_groundCallback = null;


        protected IBroadphaseInterface m_broadphase;
        protected CollisionDispatcher m_dispatcher;
        protected IConstraintSolver m_constraintSolver;
        protected DefaultCollisionConfiguration m_collisionConfiguration;
        protected DynamicsWorld m_dynamicsWorld;
        protected GhostPairCallback m_ghostPairCallback;
        protected IndexedVector3 m_gravity = new IndexedVector3(0, -10, 0);
    }

    public class CustomMaterialCombinerCallback : IContactAddedCallback
    {
        #region IContactAddedCallback Members

        public bool Callback(ref ManifoldPoint cp, CollisionObject colObj0, int partId0, int index0, CollisionObject colObj1, int partId1, int index1)
        {
            float friction0 = colObj0.GetFriction();
            float friction1 = colObj1.GetFriction();
            float restitution0 = colObj0.GetRestitution();
            float restitution1 = colObj1.GetRestitution();

            if ((colObj0.GetCollisionFlags() & CollisionFlags.CF_CUSTOM_MATERIAL_CALLBACK) != 0)
            {
                friction0 = 1.0f;//partId0,index0
                restitution0 = 0.0f;
            }
            if ((colObj1.GetCollisionFlags() & CollisionFlags.CF_CUSTOM_MATERIAL_CALLBACK) != 0)
            {
                if ((index1 & 1) != 0)
                {
                    friction1 = 1.0f;//partId1,index1
                }
                else
                {
                    friction1 = 0f;
                }
                restitution1 = 0f;
            }

            cp.m_combinedFriction = CalculateCombinedFriction(friction0, friction1);
            cp.m_combinedRestitution = calculateCombinedRestitution(restitution0, restitution1);

            //this return value is currently ignored, but to be on the safe side: return false if you don't calculate friction
            return true;
        }

        #endregion


        ///User can override this material combiner by implementing gContactAddedCallback and setting body0.m_collisionFlags |= btCollisionObject::customMaterialCallback;
        public static float CalculateCombinedFriction(float friction0, float friction1)
        {
            float friction = friction0 * friction1;

            const float MAX_FRICTION = 10.0f;
            if (friction < -MAX_FRICTION)
                friction = -MAX_FRICTION;
            if (friction > MAX_FRICTION)
                friction = MAX_FRICTION;
            return friction;
        }

        public static float calculateCombinedRestitution(float restitution0, float restitution1)
        {
            return restitution0 * restitution1;
        }

    }




}
