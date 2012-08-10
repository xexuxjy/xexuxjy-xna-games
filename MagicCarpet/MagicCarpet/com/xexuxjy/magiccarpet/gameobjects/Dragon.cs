#define P2P
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;
using Microsoft.Xna.Framework;
using BulletXNA.BulletDynamics;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Dragon : Monster
    {
        public Dragon() 
        {
        }

        public Dragon(Vector3 startPosition)
            : base(startPosition)
        {
        }


        public override void InitializeModel()
        {
            m_modelHelperData = Globals.MCContentManager.GetModelHelperData("UnitSphere");
            Vector3 halfSphere = new Vector3(1f);
            Vector3 scale = halfSphere / (m_modelHelperData.m_boundingBox.Max - m_modelHelperData.m_boundingBox.Min);
            m_scaleTransform = Matrix.CreateScale(scale);
        }


        public override void BuildCollisionObject()
        {
            int numSpheres = 10;
            float sphereGap = 1f;
            float headRadius = 0.5f;
            float bodyRadius = 0.5f;

            SphereShape headSphereShape = new SphereShape(headRadius);
            SphereShape bodySphereShape = new SphereShape(bodyRadius);

            float headMass = 1f;
            float bodyMass = 0.9f;

            for (int i = 0; i < numSpheres; ++i)
            {
                float mass = (i == 0) ? headMass : bodyMass;
                float radius = (i == 0) ? headRadius : bodyRadius;
                SphereShape sphereShape = (i == 0) ? headSphereShape : bodySphereShape;

                DragonNode dragonNode = new DragonNode();
                Matrix startTransform = Matrix.CreateTranslation(SpawnPosition + new IndexedVector3(i * sphereGap, 0, 0));
                SimpleMotionState simpleMotionState;
                dragonNode.rigidBody = Globals.CollisionManager.LocalCreateRigidBody(mass,startTransform,sphereShape,out simpleMotionState,true,this,GetCollisionFlags(),GetCollisionMask());
                dragonNode.simpleMotionState = simpleMotionState;

                dragonNode.rigidBody.SetCollisionFlags(dragonNode.rigidBody.GetCollisionFlags() & ~CollisionFlags.CF_KINEMATIC_OBJECT);


                // copy the head motionstate to local.
                if (i == 0)
                {
                    m_motionState = simpleMotionState;
                    m_collisionObject = dragonNode.rigidBody;
                }



                m_dragonNodes.Add(dragonNode);
            }


            // Build constraints
            for(int i=1;i<numSpheres;++i)
            {
                RigidBody body0 = m_dragonNodes[i - 1].rigidBody;
                RigidBody body1 = m_dragonNodes[i].rigidBody;
                float radius = (i == 0) ? headRadius : bodyRadius;

#if P2P

		        IndexedVector3 pivotInA = new IndexedVector3(0,0,radius);
                IndexedVector3 pivotInB = -pivotInA;

                Point2PointConstraint constraint = new Point2PointConstraint(body0,body1, ref pivotInA,ref pivotInB);
#endif
#if CT
                IndexedMatrix frameInA, frameInB;
                frameInA = MathUtil.SetEulerZYX(0, 0, MathUtil.SIMD_HALF_PI);
                frameInA._origin = new IndexedVector3(0, -radius, 0);
				frameInB = MathUtil.SetEulerZYX(0, 0, MathUtil.SIMD_HALF_PI);
                frameInB._origin = new IndexedVector3(0, radius, 0);

		        ConeTwistConstraint constraint = new ConeTwistConstraint(body0, body1, ref frameInA, ref frameInB);
		        constraint.SetLimit(MathUtil.SIMD_QUARTER_PI, MathUtil.SIMD_QUARTER_PI, MathUtil.SIMD_PI * 0.8f, 1.0f,0.3f,1.0f); // soft limit == hard limit
#endif
#if SIXDOF
                IndexedMatrix frameInA, frameInB;
                frameInA = MathUtil.SetEulerZYX(0, 0, MathUtil.SIMD_HALF_PI);
                frameInA._origin = new IndexedVector3(0, -radius, 0);
				frameInB = MathUtil.SetEulerZYX(0, 0, MathUtil.SIMD_HALF_PI);
                frameInB._origin = new IndexedVector3(0, radius, 0);

		        Generic6DofConstraint constraint = new Generic6DofConstraint(body0, body1, ref frameInA, ref frameInB,true);
		        constraint.SetLinearUpperLimit(new IndexedVector3(radius, 0f, 0f));
		        constraint.SetLinearLowerLimit(new IndexedVector3(-radius, 0f, 0f));

		        constraint.SetAngularLowerLimit(new IndexedVector3(0f, 0f, -1.5f));
		        constraint.SetAngularUpperLimit(new IndexedVector3(0f, 0f, 1.5f));
		        
#endif

                
                m_nodeConstraints.Add(constraint);
                Globals.CollisionManager.AddContraint(constraint);
            }

            m_rigidBody = m_collisionObject as RigidBody;

        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentActionState == actions.ActionState.Travelling)
            {
                int ibreak = 0;
            }

            m_sineCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_sineCounter > MathUtil.SIMD_2_PI)
            {
                m_sineCounter = 0;
            }

            m_sineWave = (float)Math.Sin(m_sineCounter);



            //m_dragonNodes[0].rigidBody.ApplyCentralImpulse(ref impulse);
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = 0; i < m_dragonNodes.Count; ++i)
            {
                if (i == 0)
                {
                    m_texture = Globals.MCContentManager.GetTexture(Color.Red);
                }
                else
                {
                    m_texture = Globals.MCContentManager.GetTexture(Color.LightSeaGreen);
                }
                Matrix m = Matrix.CreateTranslation(m_dragonNodes[i].simpleMotionState.Position);
                DrawEffect(Globals.GraphicsDevice, Globals.Camera.ViewMatrix, m, Globals.Camera.ProjectionMatrix);

            }
        
        }

        public override Texture2D GetTexture()
        {
            return m_texture;
        }


        public override float Speed
        {
            get
            {
                return base.Speed;
            }
            set
            {
                base.Speed = value;

                if (m_speed > 0f)
                {
                    // make it undulate
                    m_rigidBody.SetLinearVelocity(new IndexedVector3(0, m_speed * m_sineWave * 0.5f, m_speed));
                }
                
                if (value == 0f)
                {
                    for (int i = 1; i < m_dragonNodes.Count; ++i)
                    {
                        RigidBody rigidBody = m_dragonNodes[i].rigidBody;
                        rigidBody.ClearForces();
                        rigidBody.SetLinearVelocity(IndexedVector3.Zero);
                        rigidBody.SetAngularVelocity(IndexedVector3.Zero);
                    }
                }
            }


        }


        public class DragonNode
        {
            public RigidBody rigidBody;
            public SimpleMotionState simpleMotionState;
        }

        private float m_sineCounter = 0;
        private float m_sineWave = 0f;

        public ObjectArray<DragonNode> m_dragonNodes = new ObjectArray<DragonNode>();
        public List<TypedConstraint> m_nodeConstraints = new List<TypedConstraint>();
        private Texture2D m_texture;
    }
}
