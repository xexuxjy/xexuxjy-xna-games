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
            m_id = "" + (++s_idCounter);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Vector3 spawnPosition, GameObjectType gameObjectType)
            : base(Globals.Game)
        {
            SpawnPosition = spawnPosition;
            m_gameObjectType = gameObjectType;
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
            if (m_modelHelperData != null)
            {
                Vector3 bbSize = (m_modelHelperData.m_boundingBox.Max - m_modelHelperData.m_boundingBox.Min);

                // make sure the height offset by default is the center of the box.
                m_modelHeightOffset = bbSize.Y / 2.0f;
            }
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
            Position = SpawnPosition;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void SetStartAttributes()
        {
            m_attributes[(int)GameObjectAttributeType.Health] = new GameObjectAttribute(GameObjectAttributeType.Health, 100);
            m_attributes[(int)GameObjectAttributeType.Mana] = new GameObjectAttribute(GameObjectAttributeType.Mana, 100);
            m_attributes[(int)GameObjectAttributeType.DamageReduction] = new GameObjectAttribute(GameObjectAttributeType.DamageReduction, 0f);
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

            if (m_targetSpeed > 0)
            {
                int ibreak = 0;
            }

            // no acceleration)
            //if (!MathUtil.CompareFloat(TargetSpeed, Speed))
            {
                Speed = TargetSpeed;
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

        public virtual Vector3 ObjectDimensions
        {
            get { return new Vector3(1); }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual CollisionShape BuildCollisionShape()
        {
            Vector3 halfExtents = ObjectDimensions / 2f;
            CollisionShape cs = new BoxShape(halfExtents);
            return cs;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	


        public virtual void BuildCollisionObject()
        {
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(Mass, Matrix.Identity, BuildCollisionShape(), out m_motionState, true, this,GetCollisionFlags(),GetCollisionMask());
            m_rigidBody = m_collisionObject as RigidBody;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual float Mass
        {
            get { return 0f; }
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
            try
            {
                DrawEffect(Game.GraphicsDevice, Globals.Camera.ViewMatrix, WorldTransform, Globals.Camera.ProjectionMatrix);
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;            	
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected virtual void DrawEffect(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            //return;
            if (m_modelHelperData.m_model != null)
            {
                if (Globals.s_currentCameraFrustrum.Contains(BoundingBox) != ContainmentType.Disjoint)
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

                            Texture2D normalTexture = GetNormalTexture();
                            if (texture != null)
                            {
                                effect.Parameters["NormalTexture"].SetValue(normalTexture);
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
            effect.Parameters["OwnerColor"].SetValue(BadgeColor.ToVector3());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual String GetBaseTextureName()
        {
            return null;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual Texture2D GetTexture()
        {
            String baseTextureName = GetBaseTextureName();
            if (baseTextureName == null)
            {
                // cheat for now.
                Vector3 colour = Owner != null ? new Vector3(1, 0, 0) : new Vector3(1, 1, 1);
                return Globals.MCContentManager.GetTexture(ref colour);
            }
            else
            {
                return Globals.MCContentManager.GetTexture(baseTextureName);
                //return GetNormalTexture();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual Texture2D GetNormalTexture()
        {
            String baseTextureName = GetBaseTextureName();
            if (baseTextureName == null)
            {
                // cheat for now.
                Vector3 colour = new Vector3(1, 1, 1);
                return Globals.MCContentManager.GetTexture(ref colour);
            }
            else
            {
                return Globals.MCContentManager.GetTexture(baseTextureName+"_Normal");
            }
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
                if(this is CastleTower)
                {
                    int ibreak = 0;
                }
                Matrix m = WorldTransform;

                Vector3 clampedValue = value;
                Globals.Terrain.ClampToTerrain(ref clampedValue);

                m.Translation = clampedValue;
                WorldTransform = m;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // set the position of the objects 'base' , real position will then be offset by height etc
        public Vector3 PositionBase
        {
            get 
            {
                Vector3 centerPos = WorldTransform.Translation;
                centerPos.Y -= m_modelHeightOffset;
                return centerPos;
            }
            set
            {
                Vector3 clampedValue = value;
                Globals.Terrain.ClampToTerrain(ref clampedValue);
                if (StickToGround)
                {
                    float height = Globals.Terrain.GetHeightAtPointWorldSmooth(clampedValue);
                    clampedValue.Y = height + GetHoverHeight() + m_modelHeightOffset;
                }

                Position = clampedValue;
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
                    //m_collisionObject.GetCollisionShape().GetAabb(Matrix.Identity, out min, out max);
                    ((RigidBody)m_collisionObject).GetAabb(out min, out max);
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

        public virtual float Speed
        {
            get 
            { 
                return m_speed; 
            }
            set 
            { 
                m_speed = value; 
                if(m_rigidBody != null)
                {
                    if (m_speed > 0f)
                    {
                        m_rigidBody.SetLinearVelocity(new IndexedVector3(0, 0, m_speed));
                    }
                    else
                    {
                        m_rigidBody.ClearForces();
                        m_rigidBody.SetLinearVelocity(IndexedVector3.Zero);
                        m_rigidBody.SetAngularVelocity(IndexedVector3.Zero);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float TargetSpeed
        {
            get 
            { 
                return m_targetSpeed; 
            }
            set 
            { 
                m_targetSpeed = value; 
            }
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

        public void CastSpell(SpellType spellType, Vector3 startPosition, Matrix direction)
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
            float adjustedDamage = damageData.m_damage * (1f - m_attributes[(int)GameObjectAttributeType.DamageReduction].CurrentValue);

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] Damaged [{2}][{3}][{4}][{5}].", Id, GameObjectType, damageData.m_damager.Id, damageData.m_damager.GameObjectType, damageData.m_damage,adjustedDamage));
#endif


            m_attributes[(int)GameObjectAttributeType.Health].CurrentValue -= adjustedDamage;

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

        public DefaultMotionState GetMotionState()
        {
            Debug.Assert(m_motionState != null);
            return m_motionState;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Matrix Heading
        {
            get
            {
                Matrix m = m_collisionObject.GetWorldTransform();
                m.Translation = Vector3.Zero;
                return m;
            }
            set
            {
                IndexedMatrix m = value;
                if (m_collisionObject == null)
                {
                    int ibreak = 0;
                }
                m._origin= m_collisionObject.GetWorldTransform()._origin;
                m_collisionObject.SetWorldTransform(m);

            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Rotate(Vector3 axis, float angle)
        {
            Quaternion currentRotation = Quaternion.CreateFromRotationMatrix(m_collisionObject.GetWorldTransform());
            GameUtil.Rotate(axis, angle, ref currentRotation);
            IndexedMatrix result = (Matrix.CreateFromQuaternion(currentRotation) * Matrix.CreateTranslation(Position));
            m_collisionObject.SetWorldTransform(result);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // And others

        protected ObjectArray<GameObjectAttribute> m_attributes = new ObjectArray<GameObjectAttribute>();

        protected String m_id;
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI

        protected float m_modelHeightOffset = 0;


        protected GameObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

        protected Color m_badgeColor; // represents the color for multiplayer type stuff.

        protected GameObjectType m_gameObjectType;

        protected SimpleMotionState m_motionState;
        protected CollisionObject m_collisionObject;
        protected RigidBody m_rigidBody;

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
        protected static Matrix[] m_boneTransforms;

        // keeping this separate from facing on purpose.
        //protected Vector3 m_heading;

        protected Effect m_effect;

        public static int s_idCounter = 0;


    }

}