using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.combat;
using Gladius.actions;
using Gladius.renderer;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using Gladius.renderer.animation;
using Microsoft.Xna.Framework.Content;
using Gladius.util;

namespace Gladius.actors
{
    public class BaseActor : DrawableGameComponent
    {
        public BaseActor(Game game) : base(game)
        {
            m_animatedModel = new AnimatedModel(this);
            m_animatedModel.OnAnimationStarted += new AnimatedModel.AnimationStarted(m_animatedModel_OnAnimationStarted);
            m_animatedModel.OnAnimationStopped += new AnimatedModel.AnimationStopped(m_animatedModel_OnAnimationStopped);
            Rotation = QuaternionHelper.LookRotation(Vector3.Forward);
        }

        void m_animatedModel_OnAnimationStarted(AnimationEnum anim)
        {
            switch (anim)
            {
                case (AnimationEnum.Attack1):
                    {
                        break;
                    }
                case(AnimationEnum.Die):
                    {
                        break;
                    }
            }
        }

        void m_animatedModel_OnAnimationStopped(AnimationEnum anim)
        {
            switch (anim)
            {
                case (AnimationEnum.Attack1):
                    {
                        StopAttack();
                        break;
                    }
                case (AnimationEnum.Die):
                    {
                        StopDeath();
                        break;
                    }
            }
        }



        public String Name
        {
            get;
            set;
        }

        public String DebugName
        {
            get;
            set;
        }

        public String ModelName
        {
            get;
            set;
        }

        public Arena Arena
        {
            get;
            set;
        }

        public float GetAttributeValue(GameObjectAttributeType attributeType)
        {
            return m_attributeDictionary[attributeType].CurrentValue;
        }

        public void SetAttributeValue(GameObjectAttributeType attributeType,float val)
        {
            m_attributeDictionary[attributeType].CurrentValue = val;
        }

        public void PlayAnimation(AnimationEnum animation)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.PlayAnimation(animation);
            }
        }

        public void LoadContent(ContentManager contentManager)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.LoadContent(contentManager);
                m_animatedModel.PlayAnimation(AnimationEnum.Walk);
            }
        }


        Point m_currentPosition;
        public Point CurrentPosition
        {
            get
            {
                return m_currentPosition;
            }

            set
            {
                m_currentPosition = value;
                Position = Arena.ArenaToWorld(CurrentPosition);
            }


        }

        private bool m_attackRequested;
        public bool AttackRequested
        {
            get { return m_attackRequested; }
            set { m_attackRequested = value; }
        }


        public Vector3 Position
        {
            get;
            set;
        }

        public Matrix World
        {
            get;
            set;
        }

        public BoundingBox BoundingBox
        {
            get;
            set;
        }

        public String Team
        {
            get;
            set;
        }

        public ActorClass ActorClass
        {
            get;
            set;
        }

        public void TakeDamage(AttackResult attackResult)
        {
            if(attackResult.resultType != AttackResultType.Miss)
            {
                m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue -= attackResult.damageDone;
                UpdateThreatList(attackResult.damageCauser); 
            }
        }

        private void UpdateThreatList(BaseActor actor)
        {
            // todo
        }


        public override void Update(GameTime gameTime)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.Update(gameTime);
            }

            if(UnitActive)
            {
                UpdateMovement(gameTime);
                UpdateAttack(gameTime);
            }

        }

        private void UpdateMovement(GameTime gameTime)
        {
            if (Turning)
            {
                TurnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Rotation = Quaternion.Slerp(Rotation, TargetRotation, (TurnTimer / m_turnSpeed));
                // close enough now to stop?
                //if (QuaternionHelper.FuzzyEquals(Rotation, TargetRotation))
                if (TurnTimer >= m_turnSpeed)
                {
                    Rotation = TargetRotation;
                    Turning = false;
                }
            }
            else
            {
                if (FollowingWayPoints)
                {
                    // mvoe towards the next point.
                    if (WayPointList.Count > 0)
                    {
                        Vector3 target = Arena.ArenaToWorld(WayPointList[0]);
                        Vector3 diff = target - Position;
                        float closeEnough = 0.01f;
                        if (diff.LengthSquared() < closeEnough)
                        {
                            // check that nothings blocked us since this was set.
                            if (Arena.CanMoveActor(this, WayPointList[0]))
                            {
                                Arena.MoveActor(this, WayPointList[0]);

                                diff.Normalize();

                                Quaternion currentHeading = QuaternionHelper.LookRotation(diff);
                                CurrentPosition = WayPointList[0];

                                WayPointList.RemoveAt(0);
                                // check and see if we need to turn
                                if (WayPointList.Count > 0)
                                {
                                    Vector3 nextTarget = Arena.ArenaToWorld(WayPointList[0]);
                                    Vector3 nextDiff = nextTarget - Position;
                                    nextDiff.Normalize();
                                    Quaternion newHeading = QuaternionHelper.LookRotation(nextDiff);
                                    if (newHeading != currentHeading)
                                    {
                                        FaceDirection(newHeading, m_turnSpeed);
                                    }
                                }
                            }

                        }
                        else
                        {
                            diff.Normalize();
                            {
                                Position += diff * (float)gameTime.ElapsedGameTime.TotalSeconds * m_movementSpeed;
                            }
                        }
                    }
                    else
                    {
                        // finished moving.
                        FollowingWayPoints = false;
                    }
                }
            }
        }

        public void FaceDirection(Quaternion newDirection, float turnSpeed)
        {
            if (!Turning)
            {
                Turning = true;
                OriginalRotation = Rotation;
                TargetRotation = newDirection;
                TurnTimer = 0f;
            }
            else
            {
                int ibreak = 0;
            }

        }

        public void SetTarget(BaseActor target)
        {
            m_currentTarget = target;
        }

        public void StartAttack()
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack started on [{1}].", DebugName,m_currentTarget != null ?m_currentTarget.DebugName :"NoActorTarget"));
            m_animatedModel.PlayAnimation(AnimationEnum.Attack1,false);
            Attacking = true;
            AttackRequested = false;
        }




        public void StopAttack()
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack stopped.", DebugName));
            m_currentTarget = null;
            Attacking = false;
        }

        public void UpdateAttack(GameTime gameTime)
        {
            if (AttackRequested)
            {
                if (!FollowingWayPoints)
                {
                    if (Globals.NextToTarget(this, m_currentTarget))
                    {
                        StartAttack();
                    }
                }
            }
        }


        public bool Attacking
        {
            get;
            set;
        }

        public void StartDeath()
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death started.", DebugName));

        }

        public void StopDeath()
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death stopped.", DebugName));

        }

        public virtual void CheckState()
        {
            if(m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue <= 0f)
            {
                StartDeath();
            }
        }

        //public virtual void StartAction(ActionTypes actionType)
        //{
        //    Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] started.", DebugName, actionType));
        //}

        //public virtual void StopAction(ActionTypes actionType)
        //{
        //    Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] stopped.", DebugName, actionType));
        //}

        public override void Draw(GameTime gameTime)
        {
            Draw(Game.GraphicsDevice, Globals.Camera, gameTime);
        }

        public virtual void Draw(GraphicsDevice device, ICamera camera, GameTime gameTime)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.Draw(device, camera, gameTime);
            }
        }

        public List<Point> WayPointList
        {
            get { return m_wayPointList; }

        }

        // 
        public void ConfirmMove()
        {
            FollowingWayPoints = true;
        }

        public bool UnitActive
        {
            get;
            set;
        }

        bool Turning
        {
            get;
            set;
        }

        bool FollowingWayPoints
        {
            get;
            set;
        }


        public float TurnTimer
        {
            get;
            set;
        }

        public Quaternion Rotation
        {
            get;
            set;
        }

        private Quaternion OriginalRotation
        {
            get;
            set;
        }

        private Quaternion TargetRotation
        {
            get;
            set;
        }




        private BaseActor m_currentTarget = null;
        private List<BaseActor> m_threatList = new List<BaseActor>();
        private List<Point> m_wayPointList = new List<Point>();

        private List<AttackSkill> m_knownAttacks;
        private Dictionary<GameObjectAttributeType,BoundedAttribute> m_attributeDictionary;
        private AnimatedModel m_animatedModel;

        private float m_movementSpeed = 1f;
        private float m_turnSpeed = 1f;

    }

    public enum ActorClass
    {
        Light,
        Medium,
        Heavy
    }

}
