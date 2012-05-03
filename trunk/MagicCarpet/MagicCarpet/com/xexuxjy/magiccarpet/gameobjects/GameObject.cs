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
using com.xexuxjy.magiccarpet.manager;
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
            m_motionState = new DefaultMotionState(Matrix.CreateTranslation(startPosition), Matrix.Identity);
            m_id = "" + (++s_idCounter);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual float GetHoverHeight()
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
            switch (baseAction.ActionState)
            {
                case (ActionState.Dieing):
                    {
                        // default behaviour on death is simple cleanup
                        Cleanup();
                        break;
                    }
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void InitializeModel()
        {
            m_modelHelperData = Globals.MCContentManager.GetModelHelperData(GameObjectType.ToString());
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            m_scaleTransform = Matrix.Identity;

            m_actionComponent = new ActionComponent(this);
            m_actionComponent.Initialize();

            m_spellComponent = new SpellComponent(this);
            m_spellComponent.Initialize();

            m_threatComponent = new ThreatComponent(this);

            SetStartAttributes();

            DrawOrder = Globals.NORMAL_DRAW_ORDER;

            base.Initialize();

            InitializeModel();
            // call this here so we've loaded any content?
            BuildCollisionObject();

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
            float baseVal = m_attributes[(int)attributeType].BaseValue;
            float current = m_attributes[(int)attributeType].CurrentValue;
            float result = 0f;
            if (baseVal > 0f)
            {
                result = current / baseVal;
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

            if (m_actionComponent != null)
            {
                m_actionComponent.Update(gameTime);
            }

            if (m_spellComponent != null)
            {
                m_spellComponent.Update(gameTime);
            }

            if (m_threatComponent != null)
            {
                m_threatComponent.UpdateThreats();
            }


            // no acceleration)
            if (!MathUtil.CompareFloat(TargetSpeed, Speed))
            {
                Speed = TargetSpeed;
            }


            // movement here?
            if (Speed > 0)
            {
                //Vector3 movement = Forward * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector3 movement = Heading * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += movement;
            }
            // no health left so die.
            if (!IsAlive())
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
                Owner = null;
                Globals.GameObjectManager.RemoveObject(this);
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
            // box based on model is the default.
            if (m_modelHelperData != null)
            {
                Vector3 halfExtents = (m_modelHelperData.m_boundingBox.Max - m_modelHelperData.m_boundingBox.Min) / 2f;

                Vector3 scale;
                Quaternion rotation;
                Vector3 translation;

                // adjust for scale transform.
                m_scaleTransform.Decompose(out scale,out rotation,out translation);

                halfExtents *= scale;

                CollisionShape cs = new BoxShape(halfExtents);
                float mass = 0f;
                m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(mass, Matrix.CreateTranslation(Position), cs, m_motionState, true, this,GetCollisionFlags(),GetCollisionMask());
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual  CollisionFilterGroups GetCollisionFlags()
        {
            return CollisionFilterGroups.StaticFilter;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual CollisionFilterGroups GetCollisionMask()
        {
            return (CollisionFilterGroups.AllFilter ^ CollisionFilterGroups.StaticFilter);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Draw(GameTime gameTime)
        {
            DrawEffect(Game.GraphicsDevice, Globals.Camera.ViewMatrix, WorldTransform, Globals.Camera.ProjectionMatrix);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected virtual void DrawEffect(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            //return;
            if (m_modelHelperData.m_model != null)
            {
                if(Globals.s_currentCameraFrustrum.Intersects(BoundingBox))
                {
                    foreach (ModelMesh mesh in m_modelHelperData.m_model.Meshes)
                    {
                        if (m_boneTransforms == null)
                        {
                            m_boneTransforms = new Matrix[m_modelHelperData.m_model.Bones.Count];
                        }
                        m_modelHelperData.m_model.CopyAbsoluteBoneTransformsTo(m_boneTransforms);
                        foreach (Effect effect in mesh.Effects)
                        {
                            effect.CurrentTechnique = effect.Techniques["SimpleTechnique"];
                            Globals.MCContentManager.ApplyCommonEffectParameters(effect);
                            SetEffectParameters(effect);

                            Matrix result = m_scaleTransform * world;

                            effect.Parameters["WorldMatrix"].SetValue(m_boneTransforms[mesh.ParentBone.Index] * result);
                            Texture2D texture = GetTexture();
                            if (texture != null)
                            {
                                effect.Parameters["Texture"].SetValue(texture);
                            }
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void SetEffectParameters(Effect effect)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual Texture2D GetTexture()
        {
            // cheat for now.
            Vector3 colour = Owner != null ? new Vector3(1, 0, 0) : new Vector3(1, 1, 1);
            return Globals.MCContentManager.GetTexture(ref colour);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        [DescriptionAttribute("Position in the world")]
        public virtual Vector3 Position
        {
            get
            {
                return WorldTransform.Translation;
            }
            set
            {
                Matrix m = WorldTransform;

                Vector3 clampedValue = value;
                Globals.Terrain.ClampToTerrain(ref clampedValue);

                if (StickToGround)
                {
                    //float height = Globals.Terrain.GetHeightAtPointWorld(clampedValue);
                    float height = Globals.Terrain.GetHeightAtPointWorldSmooth(clampedValue);
                    clampedValue.Y = height +  GetHoverHeight();
                }

                // stop us going into the ground.


                m.Translation = clampedValue;
                WorldTransform = m;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual Matrix WorldTransform
        {
            get
            {
                IndexedMatrix m;
                GetMotionState().GetWorldTransform(out m);
                return m.ToMatrix();
            }
            set
            {
                IndexedMatrix m = value;
                GetMotionState().SetWorldTransform(ref m);
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
            get { return m_actionComponent.ActionState; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionComponent ActionComponent
        {
            get { return m_actionComponent; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public BoundingBox BoundingBox
        {
            get
            {
                IndexedVector3 min, max;
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

        public Vector3 Forward
        {
            get { return WorldTransform.Forward; }
            set
            {
                Matrix worldTransform = WorldTransform;
                if (worldTransform.Forward != value)
                {
                    Matrix m = GameUtil.RebaseMatrixOnForward(value);
                    m.Translation = worldTransform.Translation;
                    WorldTransform = m;
                }
            }
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

        public virtual GameObject Owner
        {
            get { return m_owner; }
            set
            {
                if (value != m_owner)
                {
                    // Tell the old owner we've gone
                    if (m_owner != null)
                    {
                        m_owner.NotifyOwnershipLost(this);
                    }
                    if (value != null)
                    {
                        // and let the new one know we exist.
                        value.NotifyOwnershipGained(this);
                    }
                }
                m_owner = value;
            }
        }


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
                return m_actionComponent.ActionState;
            }
            set
            {

            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool QueueContainsAction(ActionState actionState)
        {
            return m_actionComponent.QueueContainsAction(actionState);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void QueueAction(BaseAction baseAction)
        {
            m_actionComponent.QueueAction(baseAction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearAction()
        {
            m_actionComponent.ClearCurrentAction();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearAllActions()
        {
            m_actionComponent.ClearAllActions();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual Vector3 SpellCastPosition
        {
            get
            {
                return Position;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell(SpellType spellType, Vector3 startPosition, Vector3 direction)
        {
            m_spellComponent.CastSpell(spellType, startPosition, direction);
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
            return !m_awaitingRemoval && CurrentActionState != ActionState.Dieing;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Idle()
        {
            TargetSpeed = 0f;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool StickToGround
        {
            get { return m_stickToGround; }
            set { m_stickToGround = value; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Damaged(DamageData damageData)
        {
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] Damaged [{2}][{3}][{4}].", Id, GameObjectType, damageData.m_damager.Id, damageData.m_damager.GameObjectType, damageData.m_damage));
#endif


            m_attributes[(int)GameObjectAttributeType.Health].CurrentValue -= damageData.m_damage;

            // got hit so update our threat lists
            if (m_threatComponent != null)
            {
                m_threatComponent.AddOrUpdateThreat(damageData.m_damager);
            }

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual bool ShouldCollideWith(ICollideable partner)
        {
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ProcessCollision(ICollideable partner, ManifoldPoint manifoldPont)
        {
            return false;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual GameObject GetGameObject()
        {
            return this;
        }

        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // These are pseudo-events but not going to bother registering listeners and will brute force it.
        public void WorldObjectAdded(GameObject gameObject)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void WorldObjectRemoved(GameObject gameObject)
        {
            if (m_threatComponent != null)
            {
                m_threatComponent.RemoveThreat(gameObject);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void NotifyOwnershipGained(GameObject gameObject)
        {
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] Gained Ownership [{2}][{3}].", Id, GameObjectType, gameObject.Id,gameObject.GameObjectType));
#endif

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void NotifyOwnershipLost(GameObject gameObject)
        {
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] Lost Ownership [{2}][{3}].", Id, GameObjectType, gameObject.Id, gameObject.GameObjectType));
#endif
        }



        public SpellComponent SpellComponent
        {
            get { return m_spellComponent; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual Color BadgeColor
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.BadgeColor;
                }
                else
                {
                    if (this is Magician)
                    {
                        return Color.Blue;
                    }
                }
                return Color.White;
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool PlayerControlled
        {
            get { return m_playerControlled; }
            set { m_playerControlled = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public IMotionState GetMotionState()
        {
            RigidBody rb = m_collisionObject as RigidBody;
            if (rb != null)
            {
                return rb.GetMotionState();
            }
            return m_motionState;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 Heading
        {
            get { return m_heading; }
            set{m_heading = value;}
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////

       // And others

        protected ObjectArray<GameObjectAttribute> m_attributes = new ObjectArray<GameObjectAttribute>();

        protected String m_id;
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI

        protected GameObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

        protected Color m_badgeColor; // represents the color for multiplayer type stuff.

        protected GameObjectType m_gameObjectType;

        protected IMotionState m_motionState;
        protected CollisionObject m_collisionObject;

        protected float m_speed;
        protected float m_targetSpeed;

        protected bool m_awaitingRemoval;

        protected Matrix m_scaleTransform;
        protected ModelHelperData m_modelHelperData;

        protected bool m_debugEnabled;
        protected bool m_playerControlled;

        protected ActionComponent m_actionComponent;
        protected SpellComponent m_spellComponent;
        protected ThreatComponent m_threatComponent;

        protected bool m_stickToGround = true;
        protected Matrix[] m_boneTransforms;

        // keeping this separate from facing on purpose.
        protected Vector3 m_heading;



        public static int s_idCounter = 0;
    }















}