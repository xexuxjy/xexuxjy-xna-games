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

namespace com.xexuxjy.magiccarpet.collision
{
    public class CollisionManager : GameComponent
    {
        public CollisionManager(Game game, Vector3 worldMin, Vector3 worldMax)
            : base(game)
        {
            //game.Components.Add(this);
            m_collisionConfiguration = new DefaultCollisionConfiguration();

            ///use the default collision dispatcher. For parallel processing you can use a diffent dispatcher (see Extras/BulletMultiThreaded)
            m_dispatcher = new CollisionDispatcher(m_collisionConfiguration);

            m_broadphase = new DbvtBroadphase();
            IOverlappingPairCache pairCache = null;
            //pairCache = new SortedOverlappingPairCache();

            m_broadphase = new SimpleBroadphase(1000, pairCache);

            ///the default constraint solver. For parallel processing you can use a different solver (see Extras/BulletMultiThreaded)
            SequentialImpulseConstraintSolver sol = new SequentialImpulseConstraintSolver();
            m_constraintSolver = sol;

            m_dynamicsWorld = new DiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_constraintSolver, m_collisionConfiguration);

            Vector3 gravity = new Vector3(0, -10, 0);
            m_dynamicsWorld.SetGravity(ref gravity);
        }

        //----------------------------------------------------------------------------------------------

        public void Reset()
        {


        }

        //----------------------------------------------------------------------------------------------

        public override void Initialize()
        {
            if (Globals.DebugDraw != null)
            {
                m_dynamicsWorld.SetDebugDrawer(Globals.DebugDraw);
            }
        }

        //----------------------------------------------------------------------------------------------

        public override void Update(GameTime gameTime)
        {
            float ms = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //ms *= 0.1f;
            ///step the simulation
            m_dynamicsWorld.StepSimulation(ms, 1);
            m_dynamicsWorld.DebugDrawWorld();
            base.Update(gameTime);
        }

        //----------------------------------------------------------------------------------------------

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape, bool addToWorld)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape, addToWorld);
        }

        public RigidBody LocalCreateRigidBody(float mass, ref Matrix startTransform, CollisionShape shape, bool addToWorld)
        {

            Debug.Assert((shape == null || shape.GetShapeType() != BroadphaseNativeTypes.INVALID_SHAPE_PROXYTYPE));

            //rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = !MathUtil.CompareFloat(mass, 0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
            {
                shape.CalculateLocalInertia(mass, out localInertia);
            }
            //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects

            //#define USE_MOTIONSTATE 1
            //#ifdef USE_MOTIONSTATE
            DefaultMotionState myMotionState = new DefaultMotionState(startTransform, Matrix.Identity);

            RigidBodyConstructionInfo cInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);

            RigidBody body = new RigidBody(cInfo);


            if (addToWorld)
            {
                m_dynamicsWorld.AddRigidBody(body);
            }

            return body;
        }


        public bool CastRay(Vector3 startPos, Vector3 endPos, ref Vector3 collisionPoint, ref Vector3 collisionNormal)
        {
            ClosestRayResultCallback callback = null;
            if (m_dynamicsWorld != null)
            {
                callback = new ClosestRayResultCallback(startPos, endPos);
                m_dynamicsWorld.RayTest(ref startPos, ref endPos, callback);
            }
            if (callback != null)
            {
                if (callback.HasHit())
                {
                    collisionPoint = callback.m_hitPointWorld;
                    collisionNormal = callback.m_hitNormalWorld;
                    return true;
                }
            }
            return false;
        }


        //----------------------------------------------------------------------------------------------

        protected IBroadphaseInterface m_broadphase;
        protected CollisionDispatcher m_dispatcher;
        protected IConstraintSolver m_constraintSolver;
        protected DefaultCollisionConfiguration m_collisionConfiguration;
        protected DynamicsWorld m_dynamicsWorld;
    }
}
