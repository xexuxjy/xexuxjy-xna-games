using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.combat;
using Gladius.renderer;
using Microsoft.Xna.Framework.Graphics;
using Gladius.renderer.animation;
using Microsoft.Xna.Framework.Content;
using Gladius.util;
using Gladius.control;
using Gladius.modes.arena;
using GameStateManagement;
using System.Xml;

namespace Gladius.actors
{
    public class BaseActor : GameScreenComponent
    {
        public BaseActor(GameScreen gameScreen)
            : base(gameScreen)
        {
            ModelHeight = 1f;
            m_animatedModel = new AnimatedModel(ModelHeight);
            m_animatedModel.OnAnimationStarted += new AnimatedModel.AnimationStarted(m_animatedModel_OnAnimationStarted);
            m_animatedModel.OnAnimationStopped += new AnimatedModel.AnimationStopped(m_animatedModel_OnAnimationStopped);
            Rotation = QuaternionHelper.LookRotation(Vector3.Forward);
            m_animatedModel.ModelRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
            SetupAttributes();
        }

        public float ModelHeight
        {
            get;
            set;
        }

        public void SetupAttributes()
        {

            m_attributeDictionary[GameObjectAttributeType.Health] = new BoundedAttribute(100);
            m_attributeDictionary[GameObjectAttributeType.Accuracy] = new BoundedAttribute(10);

        }


        void m_animatedModel_OnAnimationStarted(AnimationEnum anim)
        {
            switch (anim)
            {
                case (AnimationEnum.Attack1):
                    {
                        break;
                    }
                case (AnimationEnum.Die):
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
                default:
                    PlayAnimation(AnimationEnum.Idle);
                    break;
            }
        }

        public void Block(BaseActor actor)
        {
            SnapToFace(actor);
            PlayAnimation(AnimationEnum.Block);
        }



        public String Name
        {
            get;
            set;
        }

        public String DebugName
        {
            get
            {
                return m_debugName;
            }

            set
            {
                m_debugName = value;
                m_animatedModel.DebugName = value;
            }
        }

        public String ModelName
        {
            get
            {
                return m_animatedModel.ModelName;
            }
            set
            {
                m_animatedModel.ModelName = value;
            }
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

        public void SetAttributeValue(GameObjectAttributeType attributeType, float val)
        {
            m_attributeDictionary[attributeType].CurrentValue = val;
        }

        public void PlayAnimation(AnimationEnum animation,bool loopClip=true)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.PlayAnimation(animation,loopClip);
            }
        }

        public void LoadContent()
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.LoadContent(ContentManager);

                // test for now.
                m_animatedModel.SetMeshActive("w_helmet_01", false);
                m_animatedModel.SetMeshActive("w_shoulder_01", false);

                m_animatedModel.SetMeshActive("bow_01", false);
                m_animatedModel.SetMeshActive("shield_01", false);


                m_animatedModel.PlayAnimation(AnimationEnum.Idle);
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

        private Matrix m_world;
        public Matrix World
        {
            get
            {
                return m_world;
            }
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

        public bool HasShield
        {
            get;
            set;
        }

        public ActorCategory ActorClass
        {
            get;
            set;
        }

        public void TakeDamage(AttackResult attackResult)
        {
            if (attackResult.resultType != AttackResultType.Miss)
            {
                m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue -= attackResult.damageDone;
                UpdateThreatList(attackResult.damageCauser);
                PlayAnimation(AnimationEnum.Stagger,false);
            }
        }

        private void UpdateThreatList(BaseActor actor)
        {
            // todo
        }


        public override void VariableUpdate(GameTime gameTime)
        {
            m_world = Matrix.CreateFromQuaternion(Rotation);
            m_world.Translation = Position;

            if (m_animatedModel != null)
            {
                m_animatedModel.ActorRotation = Rotation;
                m_animatedModel.ActorPosition = Position;
                m_animatedModel.Update(gameTime);
            }

            if (UnitActive)
            {
                if (FollowingWayPoints)
                {
                    UpdateMovement(gameTime);
                }
                //if (Attacking)
                {
                    UpdateAttack(gameTime);
                }
            }
            CheckState();
        }

        public void Think()
        {
            if (!FollowingWayPoints)
            {
                // Are we next to an enemy
                BaseActor enemy = Arena.NextToEnemy(this);
                if (enemy != null)
                {
                    Target = enemy;
                    AttackRequested = true;
                }
                else
                {
                    // pick random spot on arena and pathfind for now.
                    Point result;
                    BaseActor target = Arena.FindNearestEnemy(this);
                    if (target != null)
                    {
                        Point nearestPoint = Arena.PointNearestLocation(target.CurrentPosition, false);
                        if (Arena.FindPath(CurrentPosition, nearestPoint, WayPointList))
                        {
                            ConfirmMove();
                        }
                    }
                }
            }
        }




        private void UpdateMovement(GameTime gameTime)
        {
            if (m_currentMovePoints <= 0)
            {
                TurnComplete = true;
            }


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
                    // this is called every update and animation system doesn't reset if it's current anim
                    m_animatedModel.PlayAnimation(AnimationEnum.Walk);
                    ChooseWalkSkill();
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

                                // we've moved one step so reduce our movement.
                                m_currentMovePoints--;

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
                            else
                            {
                                // we've been blocked from where we were hoping for.
                                // clear state and force a rethink.
                                WayPointList.Clear();
                                Think();
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
                        // reached our end point. think and see if we do something else.
                        Think();
                        //TurnComplete = true;
                    }
                }
                else
                {
                    // not sure why not here but
                    if (!PlayerControlled)
                    {
                        TurnComplete = true;
                    }
                }
            }
        }

        public void SnapToFace(BaseActor actor)
        {
            Vector3 nextDiff = actor.Position - Position;
            nextDiff.Y = 0;
            nextDiff.Normalize();
            Rotation = QuaternionHelper.LookRotation(nextDiff);
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

        public BaseActor Target
        {
            get
            {
                return m_currentTarget;
            }
            set
            {
                m_currentTarget = value;
            }
        }

        private void ChooseAttackSkill()
        {
            CurrentAttackSkill = m_knownAttacks.FirstOrDefault(a => a.AttackType == AttackType.SingleOrtho);
        }

        private void ChooseWalkSkill()
        {
            CurrentAttackSkill = m_knownAttacks.FirstOrDefault(a => a.Name == "Move");
        }


        public void StartAttack()
        {
            ChooseAttackSkill();
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack started on [{1}] Skill[{2}].", DebugName, m_currentTarget != null ? m_currentTarget.DebugName : "NoActorTarget",CurrentAttackSkill.Name));
            m_animatedModel.PlayAnimation(AnimationEnum.Attack1, false);
            Attacking = true;
            AttackRequested = false;
            Globals.CombatEngine.ResolveAttack(this, m_currentTarget, CurrentAttackSkill);
        }

        public void StopAttack()
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack stopped.", DebugName));
            m_currentTarget = null;
            Attacking = false;
            // FIXME - need to worry about out of turn attacks (ripostes, groups etc)
            TurnComplete = true;
        }

        public void UpdateAttack(GameTime gameTime)
        {
            if (AttackRequested)
            {
                if (!FollowingWayPoints)
                {
                    if (Globals.NextToTarget(this, m_currentTarget))
                    {
                        SnapToFace(m_currentTarget);
                        m_currentTarget.SnapToFace(this);
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
            Dead = true;
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death started.", DebugName));
            m_animatedModel.PlayAnimation(AnimationEnum.Die, false);


        }

        public void StopDeath()
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death stopped.", DebugName));

        }

        public virtual void CheckState()
        {
            if (m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue <= 0f && !Dead)
            {
                StartDeath();
            }
        }

        public void StartBlock(BaseActor attacker)
        {
            SnapToFace(attacker);
            m_animatedModel.PlayAnimation(AnimationEnum.Block, false);
        }

        public void EndBlock()
        {


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

        public Vector3 CameraFocusPoint
        {
            get
            {
                return Position + new Vector3(0, 0.5f, 0);
            }
        }


        public void StartTurn()
        {
            UnitActive = true;
            TurnComplete = false;

            m_currentMovePoints = m_totalMovePoints;

            if (PlayerControlled == false)
            {
                Think();
            }
        }


        public void EndTurn()
        {
            UnitActive = false;
            CurrentAttackSkill = null;
            m_animatedModel.PlayAnimation(AnimationEnum.Idle);
            TurnManager.QueueActor(this);
        }

        public bool TurnComplete
        {
            get;
            set;
        }

        public bool PlayerControlled
        {
            get;
            set;
        }

        public TurnManager TurnManager
        {
            get;
            set;
        }

        public void SetupSkills(AttackSkillDictionary skillDictionary)
        {
            // simple for now.
            m_knownAttacks.Clear();
            foreach (AttackSkill attackSkill in skillDictionary.Data.Values)
            {
                m_knownAttacks.Add(attackSkill);
            }
        }

        public List<AttackSkill> AttackSkills
        {
            get
            {
                return m_knownAttacks;
            }
        }

        public void ApplyBlockSkill(AttackSkill skill)
        {
            // different blocks may have different bonuses.
            TurnComplete = true;
        }

        public AttackSkill CurrentAttackSkill
        {
            get;
            set;
        }


        public void AttachModelToLeftHand(Model model)
        {
            m_leftHandModel = model;
        }

        public void AttachModelToRightHand(Model model)
        {
            m_rightHandModel = model;
        }

        public bool Dead
        {
            get;
            set;
        }


        /*
         *   <Character Name="" Accuracy ="" Defense= "" Power= "" Consitution= "" Experience="" Level ="">
    <Skills>
    
    </Skills>
    <Equipment Head ="" Arm1="" Arm2="" Body="" Special=""/>
  </Character>
*/
        public void SetupCharacterData(XmlElement element)
        {
            Name = element.Attributes["Name"].Value;
            XmlElement attributes =(XmlElement) element.SelectSingleNode("Attributes");
            foreach (XmlAttribute attr in attributes.Attributes)
            {
                try
                {
                    GameObjectAttributeType attrType = (GameObjectAttributeType)Enum.Parse(typeof(GameObjectAttributeType), attr.Name);
                    float val = float.Parse(attr.Value);
                    m_attributeDictionary[attrType] = new BoundedAttribute(val);
                }
                catch (System.Exception ex)
                {
                	
                }

            }

        }

        public Dictionary<GameObjectAttributeType, BoundedAttribute> AttributeDictionary
        {
            get { return m_attributeDictionary; }
        }

        private BaseActor m_currentTarget = null;
        private List<BaseActor> m_threatList = new List<BaseActor>();
        private List<Point> m_wayPointList = new List<Point>();

        private List<AttackSkill> m_knownAttacks = new List<AttackSkill>();
        private Dictionary<GameObjectAttributeType, BoundedAttribute> m_attributeDictionary = new Dictionary<GameObjectAttributeType, BoundedAttribute>();
        private AnimatedModel m_animatedModel;

        private float m_movementSpeed = 2f;
        private float m_turnSpeed = 1f;

        private int m_currentMovePoints;
        private int m_totalMovePoints = 10;


        private Random m_rng = new Random();

        private Model m_leftHandModel;
        private Model m_rightHandModel;
        private String m_debugName;

        public const int MinLevel = 1;
        public const int MaxLevel = 15;


    }

    public enum ActorCategory
    {
        Light,
        Medium,
        Heavy
    }

    public enum ActorClass
    {
        Amazon,
        Archer,
        Bandit,
        Barbarian,
        Bear,
        Berserker,
        Centurion,
        Channeler,
        Cyclops,
        Dervish,
        Eiji,
        Gungir,
        Gwazi,
        Legionnaire,
        Ludo,
        Minotaur,
        Mongrel,
        MongrelShaman,
        Murmillo,
        Ogre,
        Peltast,
        PlainsCat,
        Samnite,
        Satyr,
        Scarab,
        Scorpion,
        Secutor,
        Summoner,
        UndeadLegionnaire,
        UndeadSummoner,
        Urlan,
        Ursula,
        Valens,
        Wolf,
        Yeti
    }

}
