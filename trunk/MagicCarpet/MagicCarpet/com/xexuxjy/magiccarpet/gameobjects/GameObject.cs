using Microsoft.Xna.Framework;
using System.ComponentModel;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA.BulletDynamics;
using BulletXNA;
using System;
using System.Collections.Generic;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.spells;
using com.xexuxjy.magiccarpet.actions;
using Dhpoware;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.interfaces;
using GameStateManagement;
using com.xexuxjy.magiccarpet.util.debug;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.combat;
namespace com.xexuxjy.magiccarpet.gameobjects
{

    public class GameObject : DrawableGameComponent, IDebuggable, ICollideable
    {

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // no-arg ctor for list classes
        public GameObject()
            : base(Globals.Game)
        {
        }

        public GameObject(GameObjectType gameObjectType)
            : base(Globals.Game)
        {
            m_gameObjectType = gameObjectType;
            m_motionState = new DefaultMotionState();
            m_id = "" + (++s_idCounter);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Vector3 startPosition, GameObjectType gameObjectType)
            : base(Globals.Game)
        {
            m_gameObjectType = gameObjectType;

            startPosition.Y += GetStartOffsetHeight();

            m_motionState = new DefaultMotionState(Matrix.CreateTranslation(startPosition), Matrix.Identity);
            m_id = "" + (++s_idCounter);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual float GetStartOffsetHeight()
        {
            return 0f;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void ActionStarted(BaseAction baseAction)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void ActionComplete(BaseAction baseAction)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            m_scaleTransform = Matrix.Identity;
            m_actionPool = new ActionPool(this);
            m_actionPool.Initialize();

            m_spellPool = new SpellPool(this);
            m_spellPool.Initialize();

            m_model = Globals.MCContentManager.ModelForObjectType(GameObjectType);

            BuildCollisionObject();
            SetStartAttributes();
            base.Initialize();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void SetStartAttributes()
        {
            m_attributes[(int)GameObjectAttributeType.Health] = new GameObjectAttribute(GameObjectAttributeType.Health, 100);
            m_attributes[(int)GameObjectAttributeType.Mana] = new GameObjectAttribute(GameObjectAttributeType.Mana, 100);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float GetAttributePercentage(GameObjectAttributeType attributeType)
        {
            float max = m_attributes[(int)attributeType].MaxValue;
            float current = m_attributes[(int)attributeType].CurrentValue;
            float result = 0f;
            if (max > 0f)
            {
                result = current / max;
            }
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	
        public float Health
        {
            get
            {
                return m_attributes[(int)GameObjectAttributeType.Health].CurrentValue;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float MaxHealth
        {
            get
            {
                return m_attributes[(int)GameObjectAttributeType.Health].MaxValue;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float Mana
        {
            get
            {
                return m_attributes[(int)GameObjectAttributeType.Mana].CurrentValue;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float MaxMana
        {
            get
            {
                return m_attributes[(int)GameObjectAttributeType.Mana].MaxValue;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool Alive
        {
            get
            {
                return Health > 0f;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            m_actionPool.Update(gameTime);

            m_spellPool.Update(gameTime);

            // no acceleration)
            if (!MathUtil.CompareFloat(TargetSpeed, Speed))
            {
                Speed = TargetSpeed;
            }


            // movement here?
            if (Speed > 0)
            {
                Vector3 movement = Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += movement;
            }

            // no health left so die.
            if (Health <= 0.0f)
            {
                Die();
            }


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool IsAlive()
        {
            return Health > 0f;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void Cleanup()
        {
            if (!m_awaitingRemoval)
            {
                Globals.GameObjectManager.RemoveGameObject(this);
                m_awaitingRemoval = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public CollisionObject CollisionObject
        {
            get { return m_collisionObject; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void BuildCollisionObject()
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Draw(GameTime gameTime)
        {

            ICamera camera = Globals.Camera;

            // only one of these should be active.

            DrawEffect(Game.GraphicsDevice, camera.ViewMatrix, WorldTransform, camera.ProjectionMatrix);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected virtual void DrawEffect(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            //return;
            if (m_model != null)
            {
                Matrix[] transforms = new Matrix[m_model.Bones.Count];
                foreach (ModelMesh mesh in m_model.Meshes)
                {
                    m_model.CopyAbsoluteBoneTransformsTo(transforms);
                    foreach (Effect effect in mesh.Effects)
                    {

                        BasicEffect basicEffect = (BasicEffect)effect;
                        basicEffect.View = view;
                        basicEffect.Projection = projection;
                        basicEffect.World = transforms[mesh.ParentBone.Index] * m_scaleTransform * world;


                        // cheat for now.
                        Vector3 colour = Owner != null ? new Vector3(1, 0, 0) : new Vector3(1, 1, 1);
                        basicEffect.Texture = Globals.MCContentManager.GetTexture(ref colour);


                    }
                    mesh.Draw();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void DrawDebugAxes(GraphicsDevice graphicsDevice)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void DrawBoundingBox(GraphicsDevice graphicsDevice)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        [DescriptionAttribute("Position in the world")]
        virtual public Vector3 Position
        {
            get
            {
                Matrix m;
                m_motionState.GetWorldTransform(out m);
                return m.Translation;
            }
            set
            {
                Matrix m;
                m_motionState.GetWorldTransform(out m);

                Vector3 clampedValue = value;
                Globals.Terrain.ClampToTerrain(ref clampedValue);
                m.Translation = clampedValue;
                m_motionState.SetWorldTransform(ref m);
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Matrix WorldTransform
        {
            get
            {
                Matrix m;
                m_motionState.GetWorldTransform(out m);
                return m;
            }
            set
            {
                Matrix m = value;
                m_motionState.SetWorldTransform(ref m);
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        [DescriptionAttribute("Spawn Position in the world")]
        virtual public Vector3 SpawnPosition
        {
            get { return m_spawnPosition; }
            set { m_spawnPosition = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        [DescriptionAttribute("Id")]
        virtual public System.String Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionState ActionState
        {
            get { return m_actionPool.ActionState; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionPool ActionPool
        {
            get { return m_actionPool; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public BoundingBox BoundingBox
        {
            get
            {
                Vector3 min, max;
                BoundingBox bb = new BoundingBox();
                if (m_collisionObject != null)
                {
                    m_collisionObject.GetCollisionShape().GetAabb(Matrix.Identity, out min, out max);
                    bb = new BoundingBox(min, max);
                }
                return bb;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float TargetSpeed
        {
            get { return m_targetSpeed; }
            set { m_targetSpeed = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject Owner
        {
            get { return m_owner; }
            set
            {
                if (value != m_owner)
                {
                    if (OwnerChanged != null)
                    {
                        OwnerChanged(m_owner, value);
                    }
                }
                m_owner = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectAttribute GetAttribute(GameObjectAttributeType type)
        {
            return m_attributes[(int)type];
        }

        public void SetAttribute(GameObjectAttributeType type, float value)
        {
            m_attributes[(int)type].BaseValue = value;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Die()
        {
            if (!QueueContainsAction(ActionState.Dieing))
            {
                QueueAction(new ActionDie(this));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionState CurrentActionState
        {
            get
            {
                return m_actionPool.ActionState;
            }
            set
            {

            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool QueueContainsAction(ActionState actionState)
        {
            return m_actionPool.QueueContainsAction(actionState);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void QueueAction(BaseAction baseAction)
        {
            m_actionPool.QueueAction(baseAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearAction()
        {
            m_actionPool.ClearCurrentAction();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearAllActions()
        {
            m_actionPool.ClearAllActions();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CastSpell(SpellType spellType, Vector3 startPosition, Vector3 direction)
        {
            m_spellPool.CastSpell(spellType, startPosition, direction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public Vector3 WorldToLocal(Vector3 worldPoint)
        {
            return (worldPoint - BoundingBox.Min);
            //Matrix m;
            //m_motionState.GetWorldTransform(out m);
            //m = Matrix.Invert(m);
            //return Vector3.Transform(worldPoint, m);
        }

        public Vector3 LocalToWorld(Vector3 localPoint)
        {
            //Matrix m;
            //m_motionState.GetWorldTransform(out m);
            ////m = Matrix.Invert(m);
            //return Vector3.Transform(localPoint, m);
            return (localPoint + BoundingBox.Min);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectType GameObjectType
        {
            get { return m_gameObjectType; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual String DebugText
        {
            get { return ""; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool DebugEnabled
        {
            get { return m_debugEnabled; }
            set { m_debugEnabled = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Active()
        {
            return CurrentActionState != ActionState.Dieing;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Idle()
        {
            TargetSpeed = 0f;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Damaged(DamageData damageData)
        {
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] Damaged [{2}][{3}][{4}].", Id, GameObjectType, damageData.m_damager.Id, damageData.m_damager.GameObjectType, damageData.m_damage));
#endif


            m_attributes[(int)GameObjectAttributeType.Health].CurrentValue -= damageData.m_damage;

            // got hit so update our threat lists
            AddOrUpdateThreat(damageData.m_damager);

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual bool ShouldCollideWith(ICollideable partner)
        {
            return false;
        }

        public virtual bool ProcessCollision(ICollideable partner, ManifoldPoint manifoldPont)
        {
            return false;
        }

        public virtual GameObject GetGameObject()
        {
            return this;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateThreats()
        {
            for (int i = 0; i < m_recentThreats.Count; ++i)
            {
                if (m_recentThreats[i].m_threatLevel > 0)
                {
                    ThreatData.UpdateThreatLevel(ref m_recentThreats.GetRawArray()[i], -ThreatData.s_threatDecay);
                }
            }


        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddOrUpdateThreat(GameObject gameObject)
        {

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] AddThreat [{2}][{3}].", Id, GameObjectType, gameObject.Id, gameObject.GameObjectType));
#endif

            int index = FindThreatIndex(gameObject);
            if (index >= 0)
            {
                ThreatData.UpdateThreatLevel(ref m_recentThreats.GetRawArray()[index], ThreatData.s_threatIncrement);
            }
            else
            {
                m_recentThreats.Add(new ThreatData(gameObject, ThreatData.s_initialThreatLevel, 0));
            }




        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected int FindThreatIndex(GameObject gameObject)
        {
            int index = -1;
            for (int i = 0; i < m_recentThreats.Count; ++i)
            {
                if (m_recentThreats[i].m_gameObject == gameObject)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveThreat(GameObject gameObject)
        {

            int index = FindThreatIndex(gameObject);
            if (index >= 0)
            {
#if LOG_EVENT
                Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] RemoveThreat [{2}][{3}].", Id, GameObjectType, gameObject.Id, gameObject.GameObjectType));
#endif
                m_recentThreats.RemoveAt(index);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // These are pseudo-events but not going to bother registering listeners and will brute force it.
        public void WorldObjectAdded(GameObject gameObject)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void WorldObjectRemoved(GameObject gameObject)
        {
            RemoveThreat(gameObject);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 GetFleeDirection()
        {
            // find out which is the 
            float totalThreatLevel = 0f;
            Vector3 threatDirection = Vector3.Zero;

            foreach (ThreatData threatData in m_recentThreats)
            {
                totalThreatLevel += threatData.m_threatLevel;
            }

            // if we have a threat level.
            if (totalThreatLevel > 0)
            {

                // now go through and find the direction of greatest threat?
                foreach (ThreatData threatData in m_recentThreats)
                {
                    float contribution = (float)threatData.m_threatLevel / totalThreatLevel;
                    threatDirection += (GameUtil.DirectionToTarget(this, threatData.m_gameObject) * contribution);
                }
                // flee away from the area of greatest threats...
                threatDirection.Y = 0f;
                threatDirection = -threatDirection;
            }
            if (MathUtil.FuzzyZero(threatDirection.LengthSquared()))
            {
                // either equal threats around us or no threats, in which case chose random direction
                Vector3 randomPosition = Globals.Terrain.GetRandomWorldPositionXZ();
                threatDirection = GameUtil.DirectionToTarget(this, randomPosition);
            }
            threatDirection.Normalize();

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] FleeDirection [{2}].", Id, GameObjectType, threatDirection));
#endif
            return threatDirection;



        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Delegates and events


        public ObjectArray<ThreatData> m_recentThreats = new ObjectArray<ThreatData>();



        public delegate void OwnerChangedHandler(GameObject oldOwner, GameObject newOwner);
        public event OwnerChangedHandler OwnerChanged;

        //public delegate void DamagedHandler(GameObject sender, EventArgs e);
        //public event DamagedHandler Damaged;

        // And others

        private ObjectArray<GameObjectAttribute> m_attributes = new ObjectArray<GameObjectAttribute>();

        protected String m_id;
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI

        protected GameObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

        protected Color m_badgeColor; // represents the color for multiplayer type stuff.

        protected GameObjectType m_gameObjectType;

        protected IMotionState m_motionState;
        protected CollisionObject m_collisionObject;

        protected Vector3 m_direction;
        protected float m_speed;
        protected float m_targetSpeed;

        protected bool m_awaitingRemoval;

        protected Matrix m_scaleTransform;
        protected Model m_model;

        protected bool m_debugEnabled;


        protected ActionPool m_actionPool;
        protected SpellPool m_spellPool;

        public static int s_idCounter = 0;
    }















}