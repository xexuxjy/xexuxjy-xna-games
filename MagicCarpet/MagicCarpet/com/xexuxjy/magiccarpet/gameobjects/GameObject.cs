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
using com.xexuxjy.utils.debug;
using Dhpoware;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.interfaces;
namespace com.xexuxjy.magiccarpet.gameobjects
{

    public abstract class GameObject : DrawableGameComponent , IDebuggable,ICollideable
	{

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Game game,GameObjectType gameObjectType)
            : base(game)
        {
            m_gameObjectType = gameObjectType;
            m_motionState = new DefaultMotionState();
            game.Components.Add(this);
            m_id = "" + (++s_idCounter);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Vector3 startPosition, Game game,GameObjectType gameObjectType)
            : base(game)
        {
            m_gameObjectType = gameObjectType;
            m_motionState = new DefaultMotionState(Matrix.CreateTranslation(startPosition), Matrix.Identity);
            game.Components.Add(this);
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
            m_attributes[GameObjectAttributeType.Health] = new GameObjectAttribute(GameObjectAttributeType.Health, 100);
            m_attributes[GameObjectAttributeType.Mana] = new GameObjectAttribute(GameObjectAttributeType.Mana, 100);
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
            if (m_attributes[GameObjectAttributeType.Health].CurrentValue <= 0.0f)
            {
                Die();
            }


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

            //DrawDebugAxes(Game.GraphicsDevice);
            //if (ShouldDrawBoundingBox())
            //{
            //    DrawBoundingBox(Game.GraphicsDevice);
            //}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected virtual void DrawEffect(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            //return;
            if (m_model != null)
            {
                //Matrix scale = Matrix.CreateScale(modelScalingData.scale);
                Matrix[] transforms = new Matrix[m_model.Bones.Count];
                //BasicEffect basicEffect = Globals.MCContentManager.BasicEffect;
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
                        Vector3 colour = Owner != null ? new Vector3(1,0,0) : new Vector3(1,1,1);
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
            get{return m_spawnPosition;}
            set{m_spawnPosition = value;}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
 
        [DescriptionAttribute("Id")]
		virtual public System.String Id
		{
			get{return m_id;}
            set{m_id = value;}
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
                 return m_boundingBox; 
            }
            set 
            { 
                m_boundingBox = value; 
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
            return m_attributes[type];
        }

        public void SetAttribute(GameObjectAttributeType type,float value)
        {
            m_attributes[type].BaseValue = value;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Die()
        {
            QueueAction(new ActionDie(this));

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
            return (worldPoint - m_boundingBox.Min);
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
            return (localPoint + m_boundingBox.Min);
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
            return CurrentActionState != ActionState.Dead && CurrentActionState != ActionState.Dieing;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Idle()
        {
            TargetSpeed = 0f;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void DoDamage(float points)
        {
            m_attributes[GameObjectAttributeType.Health].CurrentValue -= points;
            Damaged(this, null);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual bool ShouldCollideWith(ICollideable partner)
        {
            return false;
        }

        public virtual void ProcessCollision(ICollideable partner, ManifoldPoint manifoldPont)
        {
        }

        public virtual GameObject GetGameObject()
        {
            return this;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Delegates and events



        public delegate void OwnerChangedHandler(GameObject oldOwner,GameObject newOwner);
        public event OwnerChangedHandler OwnerChanged;

        public delegate void DamagedHandler(GameObject sender, EventArgs e);
        public event DamagedHandler Damaged;

        // And others

        private Dictionary<GameObjectAttributeType, GameObjectAttribute> m_attributes = new Dictionary<GameObjectAttributeType, GameObjectAttribute>();

        protected String m_id;
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI
 
        protected GameObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

        protected BoundingBox m_boundingBox;
 
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


        private ActionPool m_actionPool;
        private SpellPool m_spellPool;

        public static int s_idCounter = 0;
    }















}